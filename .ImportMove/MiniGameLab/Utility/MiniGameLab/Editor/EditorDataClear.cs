#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorDataClear : EditorWindow
{
	string[] dataFilePaths;
	private Vector2 scrollRect;

	private static EditorDataClear instance;

	private void Awake()
	{
		instance = this;
	}

	[MenuItem("Tools/Editor Data Controller")]
	public static void ShowWindow()
	{
		EditorWindow window = GetWindow<EditorDataClear>();
		window.minSize = new Vector2(400, 400);
		window.Show();
	}

	public static void DeleteData()
	{
		if (instance)
		{
			instance.RefreshDataDirectory();
			instance.DeleteAll();
		}
	}

	void DeleteAll()
	{
		for (int i = 0; i < dataFilePaths.Length; i++)
		{
			File.Delete(dataFilePaths[i]);
		}
	}

	private void OnFocus()
	{
		RefreshDataDirectory();
	}

	void RefreshDataDirectory()
	{
		dataFilePaths = Directory.GetFiles(Application.persistentDataPath + "/", "*.*", SearchOption.TopDirectoryOnly);
	}

	private void OnGUI()
	{
		EditorGUILayout.BeginVertical("Box");
		if (GUILayout.Button("Delete All Custom Data", GUILayout.Width(250), GUILayout.Height(40)))
		{
			DeleteAll();
			RefreshDataDirectory();
		}

		if (GUILayout.Button("Delete All Playerprefs", GUILayout.Width(250), GUILayout.Height(40)))
		{
			PlayerPrefs.DeleteAll();
			RefreshDataDirectory();
		}

		EditorGUILayout.EndVertical();

		scrollRect = EditorGUILayout.BeginScrollView(scrollRect);
		for (int i = 0; i < dataFilePaths.Length; i++)
		{
			EditorGUILayout.BeginHorizontal("Box");
			EditorGUILayout.LabelField(dataFilePaths[i].Remove(0, Application.persistentDataPath.Length + 1));
			if (GUILayout.Button("Delete", GUILayout.Width(80)))
			{
				File.Delete(dataFilePaths[i]);
				RefreshDataDirectory();
				return;
			}

			if (GUILayout.Button("Find", GUILayout.Width(80)))
			{
				EditorUtility.RevealInFinder(dataFilePaths[i]);
			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();
	}
}
#endif