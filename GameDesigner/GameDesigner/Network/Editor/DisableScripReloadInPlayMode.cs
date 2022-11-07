#if UNITY_EDITOR && UNITY_2018_1_OR_NEWER
using Net.Helper;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoad]
public class DisableScripReloadInPlayMode
{
	static DisableScripReloadInPlayMode()
	{
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
	{
		switch (stateChange)
		{
			case PlayModeStateChange.EnteredPlayMode:
				EditorApplication.LockReloadAssemblies();
				break;
			case PlayModeStateChange.ExitingPlayMode:
				EditorApplication.UnlockReloadAssemblies();
				break;
		}
	}
}
#endif