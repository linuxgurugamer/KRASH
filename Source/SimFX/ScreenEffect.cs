using System;
using System.Globalization;
using UnityEngine;

namespace KRASH.SimFX
{
    /// <summary>What the screen effect should do this frame. Strengths are 0..1; 0 disables that layer.</summary>
    internal struct ScreenLook
    {
        internal readonly int SimFps;             // simulated frame rate of the 3D image (0 = live)
        internal readonly float Shutter;          // motion blur exposure as a fraction of a simulated frame (0 = off)
        internal readonly float Scale;            // render resolution fraction (1 = no pixelation)
        internal readonly float ScanlineStrength;
        internal readonly int ScanlineSize;       // rendered rows per scanline
        internal readonly float BandStrength;     // rolling refresh band

        internal ScreenLook(int simFps, float shutter, float scale, float scanlineStrength, int scanlineSize, float bandStrength)
        {
            SimFps = simFps;
            Shutter = shutter;
            Scale = scale;
            ScanlineStrength = scanlineStrength;
            ScanlineSize = scanlineSize;
            BandStrength = bandStrength;
        }

        internal bool Hold => SimFps > 0;
        internal bool MotionBlur => Hold && Shutter > 0.001f;
        internal bool Pixelate => Scale < 0.999f;
        internal bool Scanlines => ScanlineStrength > 0.001f;
        internal bool RefreshBand => BandStrength > 0.001f;
        internal bool Overlays => Scanlines || RefreshBand;
        internal bool Any => Hold || Pixelate || Overlays;

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "simFps={0}, motionBlur={1}, resolution={2:P0}, scanlines={3:P0}/{4}, band={5:P0}",
                SimFps, MotionBlur ? Mathf.RoundToInt(Shutter * 360f) + "°" : "off", Scale, ScanlineStrength, ScanlineSize, BandStrength);
    }

    /// <summary>
    /// Image effect on the final camera. It runs after TUFX's PostProcessLayer, so it sees the finished image:
    ///  1. Frame hold: the scene image only updates SimFps times per second, while the game keeps running
    ///     at its real frame rate. Everything drawn after this step (overlays) stays fluid.
    ///  2. Motion blur: each simulated frame is the average of the real frames inside its exposure window
    ///     (the last Shutter fraction of the frame), like a real low-frame-rate camera.
    ///  3. Pixelation: downsample, then upscale with point filtering (blocky pixels). Visual only, since the
    ///     scene is still rendered at full resolution.
    ///  4. Scanlines: screen-aligned quad with a soft beam profile, aligned to the rendered (blocky) rows,
    ///     so there is no moiré and no bending by TUFX's lens distortion.
    ///  5. Rolling refresh band: what a camera records when it films a CRT whose refresh is not in sync
    ///     with it (see <see cref="CreateBandCycle"/>).
    /// Steps 4 and 5 are drawn every real frame on top of the held, blurred image, so they are never
    /// blurred or slowed down by the simulated frame rate.
    /// Overlays are alpha-blended (towards black to darken, towards white to brighten) with Unity's
    /// built-in UI shader, so no custom shader bundle is needed. KSP's UI uses another camera and is not affected.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SimScreenEffect : MonoBehaviour
    {
        private const float BeamSigma = 0.38f;       // scanline beam width relative to half a line period

        // Refresh band tuning, at 100 % strength.
        private const float BandCycleScreens = 1.35f; // one decay cycle spans this many screen heights
        private const float BandRollSeconds = 6.5f;   // time for the beam edge to travel one cycle
        private const float BandMaxDark = 0.30f;      // darkening of the oldest phosphor, just below the edge
        private const float BandMaxGlow = 0.10f;      // brightening of the freshly lit phosphor, just above the edge
        private const float BandDecay = 3.2f;         // phosphor decay rate along the cycle
        private const float BandFlicker = 0.08f;      // relative frame-to-frame intensity jitter

        private static Shader overlayShader;
        private static bool shaderLookupDone;

        internal ScreenLook Look;

        private string resourceKey;
        private Material scanMat, bandMat;
        private Texture2D scanTex, bandTex;
        private float scanTilesY;
        private float flicker = 1f;

        // Frame hold / motion blur state.
        private RenderTexture held;     // the simulated frame currently shown
        private RenderTexture accum;    // running average of the frame being exposed
        private RenderTexture opaque;   // copy of the source without alpha (sampled alpha = 1)
        private Material blendMat;      // alpha-blends a frame into accum with weight 1/n
        private bool holdValid;
        private float nextTick;
        private int samples;
        private static bool blurUnsupportedLogged;

        private void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if (src == null)
            {
                Graphics.Blit(src, dst);
                return;
            }

            RenderTexture frame = Look.Hold ? UpdateHold(src) : src;
            if (!Look.Hold)
                ReleaseHold();

            int lowW = frame.width, lowH = frame.height;
            if (Look.Pixelate)
            {
                lowW = Mathf.Max(1, Mathf.RoundToInt(frame.width * Look.Scale));
                lowH = Mathf.Max(1, Mathf.RoundToInt(frame.height * Look.Scale));
                RenderTexture low = RenderTexture.GetTemporary(lowW, lowH, 0, frame.format);
                low.filterMode = FilterMode.Point;
                Graphics.Blit(frame, low);  // bilinear downsample (source filter) averages neighbouring pixels
                Graphics.Blit(low, dst);    // point upscale keeps the blocks crisp
                RenderTexture.ReleaseTemporary(low);
            }
            else
            {
                Graphics.Blit(frame, dst);
            }

            if (!Look.Overlays || !EnsureResources(src.width, src.height, lowW, lowH))
                return;

            RenderTexture.active = dst;
            GL.PushMatrix();
            GL.LoadOrtho();

            if (Look.Scanlines)
                DrawQuad(scanMat, 0f, scanTilesY, Color.white);

            if (Look.RefreshBand)
            {
                // The texture holds one decay cycle and wraps, so the band rolls down endlessly without popping.
                float scroll = Time.unscaledTime / BandRollSeconds;
                // Slight random intensity jitter, as the camera's exposure and the refresh beat drift.
                flicker = Mathf.Lerp(flicker, 1f + UnityEngine.Random.Range(-BandFlicker, BandFlicker), 0.35f);
                DrawQuad(bandMat, scroll, 1f / BandCycleScreens, new Color(1f, 1f, 1f, Mathf.Clamp01(flicker)));
            }

            GL.PopMatrix();
        }

        /// <summary>
        /// Returns the image to show for this real frame: the held simulated frame, refreshed every
        /// 1/SimFps seconds. With motion blur, the real frames inside the exposure window (the last
        /// Shutter fraction of the simulated frame) are averaged into the next simulated frame.
        /// </summary>
        private RenderTexture UpdateHold(RenderTexture src)
        {
            float now = Time.unscaledTime;
            float period = 1f / Look.SimFps;

            if (!EnsureHoldTargets(src))
                return src;

            if (!holdValid)
            {
                Graphics.Blit(src, held);
                holdValid = true;
                samples = 0;
                nextTick = now + period;
                return held;
            }

            bool blur = Look.MotionBlur && blendMat != null;
            if (blur && now >= nextTick - period * Look.Shutter)
            {
                // Running mean: accum = accum * (1 - 1/n) + frame * (1/n). The first sample (weight 1) replaces it.
                samples++;
                Graphics.Blit(src, opaque);
                blendMat.mainTexture = opaque;
                RenderTexture.active = accum;
                GL.PushMatrix();
                GL.LoadOrtho();
                DrawQuad(blendMat, 0f, 1f, new Color(1f, 1f, 1f, 1f / samples));
                GL.PopMatrix();
            }

            if (now >= nextTick)
            {
                if (blur && samples > 0)
                {
                    RenderTexture t = held; held = accum; accum = t; // the exposed average becomes the shown frame
                }
                else
                {
                    Graphics.Blit(src, held);
                }

                samples = 0;
                nextTick += period;
                if (now >= nextTick)
                    nextTick = now + period; // fell behind (hitch, pause): resync instead of bursting
            }

            return held;
        }

        /// <summary>Allocates the hold buffers at the source size; motion blur additionally needs an alpha-free copy.</summary>
        private bool EnsureHoldTargets(RenderTexture src)
        {
            if (held != null && held.width == src.width && held.height == src.height)
                return true;

            ReleaseHold();

            // Half floats keep the running average free of banding; fall back to the source format.
            RenderTextureFormat fmt = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)
                ? RenderTextureFormat.ARGBHalf : src.format;
            held = CreateTarget(src.width, src.height, fmt, "KRASH SimFX held frame");
            accum = CreateTarget(src.width, src.height, fmt, "KRASH SimFX exposure");

            // The camera image's alpha channel is not reliably 1, and the blend uses it. Copying into a format
            // without alpha makes the sampled alpha exactly 1, so the blend weight comes from the material only.
            Shader shader = OverlayShader();
            if (shader != null && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB111110Float))
            {
                opaque = CreateTarget(src.width, src.height, RenderTextureFormat.RGB111110Float, "KRASH SimFX opaque");
                blendMat = new Material(shader) { name = "KRASH SimFX exposure blend", hideFlags = HideFlags.DontSave };
            }
            else if (!blurUnsupportedLogged)
            {
                blurUnsupportedLogged = true;
                Log.Warn("This GPU does not support the formats motion blur needs; using frame hold without blur.");
            }

            holdValid = false;
            Log.Detail($"Frame hold buffers: {src.width}x{src.height} {fmt} (motion blur {(blendMat != null ? "available" : "unavailable")}).");
            return true;
        }

        private static RenderTexture CreateTarget(int w, int h, RenderTextureFormat fmt, string name)
        {
            var rt = new RenderTexture(w, h, 0, fmt)
            {
                name = name,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave,
            };
            rt.Create();
            return rt;
        }

        private void ReleaseHold()
        {
            if (held == null && accum == null && opaque == null && blendMat == null)
                return;
            DestroyAll(held, accum, opaque, blendMat);
            held = accum = opaque = null;
            blendMat = null;
            holdValid = false;
        }

        /// <summary>Full-screen quad; v runs bottom (0) to top (tilesY), offset by vOffset.</summary>
        private static void DrawQuad(Material mat, float vOffset, float tilesY, Color tint)
        {
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(tint);
            GL.TexCoord2(0f, vOffset); GL.Vertex3(0f, 0f, 0f);
            GL.TexCoord2(0f, vOffset + tilesY); GL.Vertex3(0f, 1f, 0f);
            GL.TexCoord2(1f, vOffset + tilesY); GL.Vertex3(1f, 1f, 0f);
            GL.TexCoord2(1f, vOffset); GL.Vertex3(1f, 0f, 0f);
            GL.End();
        }

        /// <summary>(Re)builds the tiny textures when the screen size or the options change.</summary>
        private bool EnsureResources(int screenW, int screenH, int lowW, int lowH)
        {
            Shader shader = OverlayShader();
            if (shader == null)
                return false;

            string key = string.Format(CultureInfo.InvariantCulture, "{0}x{1}|{2}x{3}|{4}", screenW, screenH, lowW, lowH, Look);
            if (key == resourceKey)
                return true;

            ReleaseResources();
            resourceKey = key;

            // One line period spans ScanlineSize rendered rows, but never less than 2 screen pixels
            // (a 1-pixel period cannot show a gap).
            float periodY = Mathf.Max(2f, (float)screenH * Look.ScanlineSize / lowH);
            scanTilesY = screenH / periodY;
            scanTex = CreateScanlineTile(Mathf.RoundToInt(periodY), Look.ScanlineStrength);
            scanMat = CreateMaterial(shader, scanTex);

            bandTex = CreateBandCycle(Look.BandStrength);
            bandMat = CreateMaterial(shader, bandTex);

            Log.Detail($"Screen effect resources: {key} (line period {periodY:F2}px).");
            return true;
        }

        /// <summary>
        /// One scanline period, 1 pixel wide. The electron beam is modelled as a Gaussian centred in the
        /// line: full brightness in the middle, fading into the dark gap (darkest = <paramref name="strength"/>).
        /// For a 2-pixel period this is simply one lit row and one dark row.
        /// </summary>
        private static Texture2D CreateScanlineTile(int period, float strength)
        {
            period = Math.Max(2, period);
            float centre = (period - 1) / 2 + 0.5f; // centre of the upper-middle row
            float half = period * 0.5f;
            var dark = new float[period];
            float maxDark = 0f;

            for (int r = 0; r < period; r++)
            {
                float d = Mathf.Abs(r + 0.5f - centre);
                d = Mathf.Min(d, period - d) / half;             // periodic distance, 0..1
                dark[r] = 1f - Mathf.Exp(-(d * d) / (2f * BeamSigma * BeamSigma));
                maxDark = Mathf.Max(maxDark, dark[r]);
            }

            var pixels = new Color32[period];
            for (int r = 0; r < period; r++)
            {
                float a = maxDark > 0f ? strength * dark[r] / maxDark : 0f;
                pixels[r] = new Color32(0, 0, 0, ToByte(a));
            }
            return CreateTexture(1, period, pixels, TextureWrapMode.Repeat, FilterMode.Point, "KRASH SimFX scanline");
        }

        /// <summary>
        /// One cycle of the rolling refresh band, bottom (v = 0) to top (v = 1).
        ///
        /// When a camera films a CRT out of sync, each recorded frame catches the beam part-way down the
        /// screen. Phosphor just above the beam was lit a moment ago and is brightest; going further up it was
        /// lit longer ago and has decayed; just below the beam it is the oldest of all, about to be refreshed.
        /// The result is a sharp edge (bright above, dark below) with a long soft gradient trailing above it,
        /// and the pattern rolls slowly down the screen. Here v = 0 is the edge and age grows with v.
        /// </summary>
        private static Texture2D CreateBandCycle(float strength)
        {
            const int n = 512;
            var pixels = new Color32[n];
            float maxDecay = 1f - Mathf.Exp(-BandDecay);

            for (int i = 0; i < n; i++)
            {
                float age = (i + 0.5f) / n;                                    // 0 = just lit, 1 = oldest
                float decay = (1f - Mathf.Exp(-BandDecay * age)) / maxDecay;  // 0..1, phosphor fade
                float glow = Mathf.Exp(-age / 0.035f);                        // short afterglow right behind the beam

                float dark = BandMaxDark * strength * decay;
                float bright = BandMaxGlow * strength * glow;

                // Alpha blending can only push towards one colour per texel: white where the glow dominates,
                // black where the decay does.
                pixels[i] = bright > dark
                    ? new Color32(255, 255, 255, ToByte(bright - dark))
                    : new Color32(0, 0, 0, ToByte(dark - bright));
            }

            // Bilinear + repeat: the wrap from the oldest row back to the freshest one forms the soft-but-sharp edge.
            return CreateTexture(1, n, pixels, TextureWrapMode.Repeat, FilterMode.Bilinear, "KRASH SimFX refresh band");
        }

        private static Texture2D CreateTexture(int w, int h, Color32[] pixels, TextureWrapMode wrap, FilterMode filter, string name)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false, true)
            {
                name = name,
                filterMode = filter,
                wrapMode = wrap,
            };
            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            return tex;
        }

        private static Material CreateMaterial(Shader shader, Texture2D tex) =>
            new Material(shader) { mainTexture = tex, name = tex.name, hideFlags = HideFlags.DontSave };

        private static byte ToByte(float a) => (byte)Mathf.RoundToInt(Mathf.Clamp01(a) * 255f);

        /// <summary>A built-in alpha-blended shader; both are in Unity's always-included set.</summary>
        private static Shader OverlayShader()
        {
            if (!shaderLookupDone)
            {
                shaderLookupDone = true;
                overlayShader = Shader.Find("UI/Default");
                if (overlayShader == null || !overlayShader.isSupported)
                    overlayShader = Shader.Find("Sprites/Default");
                if (overlayShader == null)
                    Log.Warn("No built-in UI shader found; scanlines and refresh band are disabled.");
                else
                    Log.Detail("Overlay shader: " + overlayShader.name);
            }
            return overlayShader;
        }

        private void ReleaseResources()
        {
            DestroyAll(scanMat, bandMat, scanTex, bandTex);
            scanMat = bandMat = null;
            scanTex = bandTex = null;
            resourceKey = null;
        }

        private static void DestroyAll(params UnityEngine.Object[] objects)
        {
            foreach (UnityEngine.Object o in objects)
                if (o != null) Destroy(o);
        }

        private void OnDestroy()
        {
            ReleaseResources();
            ReleaseHold();
        }
    }

    /// <summary>Keeps one <see cref="SimScreenEffect"/> on whichever camera is final for the current view.</summary>
    internal sealed class ScreenEffectHost
    {
        private SimScreenEffect effect;

        internal void Update(string scene, ScreenLook look)
        {
            if (!look.Any)
            {
                Detach();
                return;
            }

            Camera cam = FinalCamera(scene);
            if (cam == null)
            {
                Detach();
                return;
            }

            if (effect == null || effect.gameObject != cam.gameObject)
            {
                Detach();
                // Added after TUFX's PostProcessLayer, so OnRenderImage receives the post-processed image.
                effect = cam.gameObject.GetComponent<SimScreenEffect>();
                if (effect == null)
                    effect = cam.gameObject.AddComponent<SimScreenEffect>();
                Log.Detail($"Screen effect attached to camera '{cam.name}'.");
            }

            effect.Look = look;
        }

        internal void Detach()
        {
            if (effect != null)
                UnityEngine.Object.Destroy(effect);
            effect = null;
        }

        /// <summary>The camera TUFX treats as final for each view (see TexturesUnlimitedFXLoader.ApplyProfile).</summary>
        private static Camera FinalCamera(string scene)
        {
            switch (scene)
            {
                case TufxBridge.SceneMap:
                    ScaledCamera scaled = ScaledCamera.Instance;
                    return scaled != null ? scaled.cam : null;
                case TufxBridge.SceneInternal:
                    InternalCamera internalCam = InternalCamera.Instance;
                    return internalCam != null ? internalCam.GetComponent<Camera>() : null;
                default:
                    return Camera.main;
            }
        }
    }
}
