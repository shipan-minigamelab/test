#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "EnumData", menuName = "EnumGenerator/CreateEnum")]
public class EnumGenerator : ScriptableObject
{
	public string EnumName;
	public bool IsUpperCenter = true;
	[HideInInspector] public string FilePath;
	public string[] EnumItems;

	public void Generator()
	{
		// if (String.IsNullOrEmpty(FilePath))
		// {
		//     FilePath = Application.dataPath + "/Scripts/Enums";
		// }
		FilePath = Application.dataPath + "/Scripts/Enums";

		CreateEnum.Create(EnumName, EnumItems, FilePath, IsUpperCenter);
	}
}


[CustomEditor(typeof(EnumGenerator))]
public class EnumGeneratorButton : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EnumGenerator myScript = (EnumGenerator) target;
		if (GUILayout.Button("Generate Enum"))
		{
			myScript.Generator();
		}
	}
}


public static class CreateEnum
{
	public static void Create(string enumName, string[] enumEntries, string filePath, bool isUpperCase = true)
	{
		try
		{
			if (!Directory.Exists(filePath))
			{
				Directory.CreateDirectory(filePath);
			}

			string filePathAndName = filePath + "/" + enumName + ".cs";
			using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
			{
				streamWriter.WriteLine("public enum " + enumName);
				streamWriter.WriteLine("{");
				for (int i = 0; i < enumEntries.Length; i++)
				{
					if (isUpperCase)
					{
						streamWriter.WriteLine("\t" + enumEntries[i].ToUpper() + ",");
					}
					else
					{
						streamWriter.WriteLine("\t" + enumEntries[i] + ",");
					}
				}

				streamWriter.WriteLine("}");
			}

			AssetDatabase.Refresh();
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
	}
}


#endif