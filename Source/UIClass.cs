using System;
using UnityEngine;
using KSP.IO;

namespace KRASH
{
	public class UICLASS: MonoBehaviour
	{
		private bool uiVisible = true;

		public UICLASS ()
		{
			Log.Info ("New instance of UICLASS: UICLASS constructor");

		}
		public void Start ()
		{
			DontDestroyOnLoad (this);
		}

		public void Awake ()
		{
			GameEvents.onShowUI.Add(onShowUI);
			GameEvents.onHideUI.Add(onHideUI);
		}

		private void onShowUI ()
		{
			uiVisible = true;
		}

		private void onHideUI ()
		{
			uiVisible = false;
		}

		public void OnDestroy ()
		{
			GameEvents.onShowUI.Remove(onShowUI);
			GameEvents.onHideUI.Remove(onHideUI);
		}

		public bool isVisible()
		{
			return uiVisible;
		}

		public void setVisible(bool b)
		{
			uiVisible = b;
		}
	}
}

