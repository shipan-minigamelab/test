#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class CommonSceneController
{
    private static string commonSceneName;
    public static string CommonSceneName {
        get => commonSceneName;
        set {
            EditorPrefs.SetString("NAME_COMMON_SCENE", value);
            commonSceneName = value;
        }
    }
    
    private static bool addCommonScene;
    public static bool AddCommonScene {
        get => addCommonScene;
        set {
            EditorPrefs.SetBool("ADD_COMMON_SCENE", value);
            addCommonScene = value;
        }
    }
    
    private static bool loadLobbyScene;
    public static bool LoadLobbyScene {
        get => loadLobbyScene;
        set {
            EditorPrefs.SetBool("LOAD_LOBBY_SCENE", value);
            loadLobbyScene = value;
        }
    }
    static CommonSceneController() {
        EditorApplication.playModeStateChanged += ConfigureOnPlay;
    }
    
    private static void ConfigureOnPlay(PlayModeStateChange _state) {
        if (EditorPrefs.HasKey("ADD_COMMON_SCENE")) {
            AddCommonScene = EditorPrefs.GetBool("ADD_COMMON_SCENE");
        }
        else {
            AddCommonScene = false;
        }
        
        if (_state != PlayModeStateChange.ExitingEditMode || !AddCommonScene) return;
        
            string commonScenePath = GetCommonScenePath();
            if (commonScenePath.Length == 0) {
                // Debug.LogError("Common Scene is not added in the build settings !!");
                return;
            }

            bool isCommonSceneAdded = false;
            for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
                if (EditorSceneManager.GetSceneAt(i).path == commonScenePath) {
                    isCommonSceneAdded = true;
                    break;
                }
                else
                {
                    SceneManager.SetActiveScene(EditorSceneManager.GetSceneAt(i)); // set the other one as active scene
                }
            }


            if (!isCommonSceneAdded) {
                EditorSceneManager.OpenScene(commonScenePath, OpenSceneMode.Additive);
            }
        }
    
    private static string GetCommonScenePath() {
        string commonScenePath = "";
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
            if (EditorBuildSettings.scenes[i].path.Contains(commonSceneName)) {
                commonScenePath = EditorBuildSettings.scenes[i].path;
            }
        }

        return commonScenePath;
    }
}

public class CommonSceneControllerWindow : EditorWindow {
    private static EditorWindow window = null;
    
    [MenuItem("Tools/Common Scene Controller")]
    public static void ShowWindow() {
        window = GetWindow<CommonSceneControllerWindow>();
        window.minSize = new Vector2(400, 400);
        window.Show();
        CommonSceneController.CommonSceneName = EditorPrefs.GetString("NAME_COMMON_SCENE", "");
        CommonSceneController.AddCommonScene = EditorPrefs.GetBool("ADD_COMMON_SCENE", false);
        CommonSceneController.LoadLobbyScene = EditorPrefs.GetBool("LOAD_LOBBY_SCENE", false);
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal("Box");
        CommonSceneController.CommonSceneName = EditorGUILayout.TextField("Common Scene Name", CommonSceneController.CommonSceneName);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal("Box");
        CommonSceneController.AddCommonScene = EditorGUILayout.Toggle(
            new GUIContent("Add Common Scene", 
                "Load Common Scene before entering play mode if already not loaded..."), CommonSceneController.AddCommonScene);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal("Box");
        CommonSceneController.LoadLobbyScene = EditorGUILayout.Toggle(
            new GUIContent("Load Lobby Scene", 
                "Load Lobby Scene after the Common Scene has been loaded..."), CommonSceneController.LoadLobbyScene);
        EditorGUILayout.EndHorizontal();
    }
}

#endif