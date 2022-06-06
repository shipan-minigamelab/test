#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildConfirmationWindow : EditorWindow
{
	private static bool advancedOptions;
	private static bool nukeUsersData = false;

	private static BuildPlayerOptions _buildOptions;

	private static BuildConfirmationData _buildConfirmationData;

	private static GUILayoutOption[] buttonOptions = new GUILayoutOption[]
	{
		GUILayout.Height(50),
	};

	// [MenuItem("Custom/BuildWindow")]
	public static void ShowWindow()
	{
		advancedOptions = false;
		nukeUsersData = false;
		GetBuildConfirmationData();
		EditorWindow.GetWindow(typeof(BuildConfirmationWindow));
	}

	void OnGUI()
	{
		this.titleContent = new GUIContent("BUILD CONFIRMATION WINDOW");
		if (_buildConfirmationData == null)
		{
			GUILayout.Label("No BuildConfirmationData found", EditorStyles.boldLabel);
			if (GUILayout.Button("Retry", buttonOptions))
			{
				GetBuildConfirmationData();
			}

			return;
		}


		EditorGUILayout.BeginVertical(new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true),
		});
		GUILayout.Label("Don't forget to Bake Occlusion Data if any static item has been changed!", EditorStyles.foldoutHeader);
		advancedOptions = EditorGUILayout.BeginToggleGroup("Advanced Options. Change with caution!", advancedOptions);
		nukeUsersData = EditorGUILayout.Toggle("Nuke user's Data", nukeUsersData);
		EditorGUILayout.EndToggleGroup();
		GUI.enabled = false;
		int userVersion = EditorGUILayout.IntField("SavedDataVersionUser: ", _buildConfirmationData.GetSavedDataVersionUser());
		int buildVersion = EditorGUILayout.IntField("SavedDataVersionFromBuild: ", _buildConfirmationData.GetSavedDataVersionFromBuild());
		GUI.enabled = true;
		if (userVersion != buildVersion)
		{
			GUI.color = Color.red;
			GUILayout.Label("Damn players are gonna be so sad right now. :-(", EditorStyles.whiteLabel);
		}
		else
		{
			GUI.color = Color.green;
			GUILayout.Label("Same values. User's data will not be deleted.", EditorStyles.whiteLabel);
		}

		GUI.color = Color.white;
		if (nukeUsersData)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Increament current version", buttonOptions))
			{
				_buildConfirmationData.IncrementSaveBuildVersion();
				EditorUtility.SetDirty(this);
				EditorUtility.SetDirty(_buildConfirmationData);
			}

			if (GUILayout.Button("Decreament current version", buttonOptions))
			{
				_buildConfirmationData.DecrementSaveBuildVersion();
				EditorUtility.SetDirty(this);
				EditorUtility.SetDirty(_buildConfirmationData);
			}

			EditorGUILayout.EndHorizontal();
		}


		if (GUILayout.Button("BUILD", buttonOptions))
		{
			if (nukeUsersData && userVersion != buildVersion)
			{
				if (EditorUtility.DisplayDialog("Warning!", "User's data will be completely erased! Proceed?", "Build", "No please go back"))
				{
					advancedOptions = false;
					nukeUsersData = false;
					BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(_buildOptions);
				}
				else
				{
				}
			}
			else
			{
				BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(_buildOptions);
			}
		}

		if (GUILayout.Button("CANCEL BUILD", buttonOptions))
		{
			this.Close();
		}

		EditorGUILayout.EndVertical();
	}

	public static void BuildPlayerHandler(BuildPlayerOptions options)
	{
		_buildOptions = options;
		ShowWindow();
	}

	[InitializeOnLoadMethod]
	private static void Initialize()
	{
		BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
		// BuildPlayerWindow.RegisterBuildPlayerHandler(BuildDialogBox);
	}

	private static void BuildDialogBox(BuildPlayerOptions options)
	{
		if (EditorUtility.DisplayDialog("Yo!", "Do you want to nuke user's data?", "Yes Ping PlayerData", "Build"))
		{
			string[] path = AssetDatabase.FindAssets($"t:{nameof(BuildConfirmationData)}");
			if (path.Length > 0)
			{
				BuildConfirmationData buildConfirmationData = AssetDatabase.LoadAssetAtPath<BuildConfirmationData>(AssetDatabase.GUIDToAssetPath(path[0]));
				if (BuildPlayerWindow.focusedWindow != null) BuildPlayerWindow.focusedWindow.Close();
				EditorGUIUtility.PingObject(buildConfirmationData);
				Selection.activeObject = buildConfirmationData;
			}
			else
			{
				Debug.Log("Couldn't find player data.");
			}
		}
		else
		{
			BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(_buildOptions);
		}
	}

	private static void GetBuildConfirmationData()
	{
		string[] path = AssetDatabase.FindAssets($"t:{nameof(BuildConfirmationData)}");
		if (path.Length > 0)
		{
			_buildConfirmationData = AssetDatabase.LoadAssetAtPath<BuildConfirmationData>(AssetDatabase.GUIDToAssetPath(path[0]));
		}
	}
}
#endif