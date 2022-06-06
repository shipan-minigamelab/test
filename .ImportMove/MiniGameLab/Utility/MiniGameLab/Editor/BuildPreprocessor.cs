#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPreprocessor : IPreprocessBuildWithReport
{
	public int callbackOrder
	{
		get { return 0; }
	}

	public void OnPreprocessBuild(BuildReport report)
	{
		EditorDataClear.DeleteData();
		PlayerSettings.keystorePass = "minigamelab";
		PlayerSettings.keyaliasPass = "minigamelab";
		PlayerSettings.SplashScreen.show = false;
	}

}
#endif