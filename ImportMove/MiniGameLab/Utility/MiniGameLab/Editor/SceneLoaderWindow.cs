#if  UNITY_EDITOR


using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneLoaderWindow : EditorWindow {

    string[] allSceneGUIDs; 
    GUIStyle guiStyle; 
    GUIContent guiButtonContent; 
    Vector2 scrollPosition; 
    int OpenMode; 

    [MenuItem("Tools/Scene Controller")]
    public static void ShowLevelLoaderWindow()
    {
        EditorWindow window = GetWindow<SceneLoaderWindow>();
        window.minSize = new Vector2(400, 400);
        window.Show();
    }

    void OnEnable()
    {
        guiStyle = new GUIStyle();
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.fontSize = 18;
        guiStyle.fontStyle = FontStyle.Bold;

        guiButtonContent = new GUIContent();

        scrollPosition = new Vector2(0,0);
    }

    void OnGUI()
    {
        EditorGUILayout.Space(10);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); 
        CreateAllButtons();
        EditorGUILayout.EndScrollView(); 
        Repaint();
    }

    void CreateAllButtons()
    {
        char[] separators = {'/','.'};
        for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            string scenePath = EditorBuildSettings.scenes[i].path; 
            string[] separatedAssetPath = scenePath.Split(separators[0]); 
			
            string sceneName = separatedAssetPath[separatedAssetPath.Length-1].Split(separators[1])[0];

            guiButtonContent.text = sceneName;
            guiButtonContent.tooltip = "Path: " + scenePath;
			
            EditorGUILayout.BeginHorizontal("Box");

            if (GUILayout.Button(guiButtonContent, GUILayout.Height(25)))
            {
                if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
            if (GUILayout.Button("Add", GUILayout.Width(80),GUILayout.Height(25))) {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            if (GUILayout.Button("Find", GUILayout.Width(80),GUILayout.Height(25))) {
                EditorUtility.RevealInFinder(scenePath);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif