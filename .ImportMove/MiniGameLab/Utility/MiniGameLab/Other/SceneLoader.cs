using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonStuff
{
    public class SceneLoader : IDCreator
    {
        [SerializeField] private List<SceneData> AllScenes = new List<SceneData>();
        [SerializeField] [Scene] private string CommonSceneName;
        [Space(50)] public static SceneLoader instance;

        public event Action<string, string> OnSceneLoadStart;
        public event Action<string, string> OnSceneReplaced;
        public event Action<string> OnSceneUnloadOnly;
        public event Action<string> OnSceneLoadOnly;

        [HideInInspector] public string CurrentSceneName;

        private float LoadUnloadProgress;
        public float CurrentLoadUnloadProgress => LoadUnloadProgress;

        private void Awake()
        {
            if (instance == null) instance = this;
            DontDestroyOnLoad(this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            string _commonSeneName = CommonSceneController.CommonSceneName;
            if (String.IsNullOrEmpty(_commonSeneName))
            {
                if (String.IsNullOrEmpty(CommonSceneName))
                {
                    Debug.LogError("Common Scene name is not declared yet! Either declare in SceneLoader Inspector or Go to > Tools/Common Scene Controller and set the NAME_COMMON_SCENE field.");
                }
                else
                {
                    CommonSceneController.CommonSceneName = CommonSceneName;
                }
            }
        }
#endif


        public string GetSceneName(int Index)
        {
            if (Index >= AllScenes.Count) return null;
            return AllScenes[Index].SceneName;
        }

        public void LoadScene(int _sceneIndexFromEnum, bool force = true)
        {
            if (_sceneIndexFromEnum >= AllScenes.Count) return;
            StopAllCoroutines();
            //Scene to unload
            CurrentSceneName = "";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                string currentSceneName = SceneManager.GetSceneAt(i).name;
                if (currentSceneName != CommonSceneName)
                {
                    // not the common scene
                    CurrentSceneName = SceneManager.GetSceneAt(i).name;
                }
            }

            if (SceneManager.sceneCount == 1 && !String.IsNullOrEmpty(CurrentSceneName))
            {
                SceneManager.LoadScene(AllScenes[_sceneIndexFromEnum].SceneName);
            }
            
            else
                StartCoroutine(ReplaceSceneCoroutine(CurrentSceneName, AllScenes[_sceneIndexFromEnum].SceneName,
                    force));
        }

        public void LoadSceneOnly(int _sceneIndexFromEnum)
        {
            if (_sceneIndexFromEnum >= AllScenes.Count) return;
            StopAllCoroutines();
            StartCoroutine(LoadSceneCoroutine(AllScenes[_sceneIndexFromEnum].SceneName));
        }

        public void UnloadSceneOnly()
        {
            StopAllCoroutines();
            //Scene to unload
            CurrentSceneName = "";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                string currentSceneName = SceneManager.GetSceneAt(i).name;
                if (currentSceneName != CommonSceneName)
                {
                    // not the common scene
                    CurrentSceneName = SceneManager.GetSceneAt(i).name;
                }
            }

            StartCoroutine(UnloadSceneCoroutine(CurrentSceneName));
        }

        public void LoadScene(string _sceneName, bool force = true)
        {
            StopAllCoroutines();
            //Scene to unload
            CurrentSceneName = "";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                string currentSceneName = SceneManager.GetSceneAt(i).name;
                if (currentSceneName != CommonSceneName)
                {
                    // not the common scene
                    CurrentSceneName = SceneManager.GetSceneAt(i).name;
                }
            }

            if (SceneManager.sceneCount == 1 && !String.IsNullOrEmpty(CurrentSceneName))
                SceneManager.LoadScene(_sceneName);
            else
                StartCoroutine(ReplaceSceneCoroutine(CurrentSceneName, _sceneName, force));
        }

        IEnumerator ReplaceSceneCoroutine(string _sceneToUnload, string _sceneToLoad, bool _forceLoad = true)
        {
            OnSceneLoadStart?.Invoke(_sceneToUnload, _sceneToLoad);
            bool unloadSceneAvailable = false;
            LoadUnloadProgress = 0f;
            if (string.IsNullOrEmpty(_sceneToLoad) || string.IsNullOrWhiteSpace(_sceneToLoad))
            {
                throw new ArgumentException("SceneLoader: SceneToLoad is not valid");
                // yield break;
            }

            if (string.IsNullOrEmpty(_sceneToUnload) || string.IsNullOrWhiteSpace(_sceneToUnload))
            {
                if (_forceLoad)
                {
                    Debug.Log("SceneLoader: SceneToUnload is not valid. Still loading the desired scene.");
                }
                else
                {
                    throw new ArgumentException("SceneLoader: SceneToUnload is not valid");
                    // yield break;
                }
            }
            else
            {
                unloadSceneAvailable = true;
            }

            yield return new WaitForSeconds(0.15f);

            if (unloadSceneAvailable)
            {
                AsyncOperation unloadJob = SceneManager.UnloadSceneAsync(_sceneToUnload);
                while (!unloadJob.isDone)
                {
                    LoadUnloadProgress = unloadJob.progress / 2f;
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.15f);

            AsyncOperation loadJob = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
            loadJob.allowSceneActivation = true;
            while (!loadJob.isDone)
            {
                if (unloadSceneAvailable)
                {
                    LoadUnloadProgress = 0.5f + loadJob.progress / 2;
                }
                else
                {
                    LoadUnloadProgress = Mathf.Clamp(loadJob.progress, 0.2f, 1f);
                }

                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneToLoad));
            yield return new WaitForSeconds(0.1f);

            // yield return new WaitForSeconds(0.1f);

            OnSceneReplaced?.Invoke(_sceneToUnload, _sceneToLoad);
        }

        IEnumerator UnloadSceneCoroutine(string _sceneToUnload)
        {
            OnSceneLoadStart?.Invoke(_sceneToUnload, "");

            LoadUnloadProgress = 0f;

            if (string.IsNullOrEmpty(_sceneToUnload) || string.IsNullOrWhiteSpace(_sceneToUnload))
            {
                throw new ArgumentException("SceneLoader: SceneToUnload is not valid");
            }

            yield return new WaitForSeconds(0.15f);

            AsyncOperation unloadJob = SceneManager.UnloadSceneAsync(_sceneToUnload);
            while (!unloadJob.isDone)
            {
                LoadUnloadProgress = unloadJob.progress / 2f;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            OnSceneUnloadOnly?.Invoke(_sceneToUnload);
        }

        IEnumerator LoadSceneCoroutine(string _sceneToLoad)
        {
            OnSceneLoadStart?.Invoke("", _sceneToLoad);
            LoadUnloadProgress = 0f;
            if (string.IsNullOrEmpty(_sceneToLoad) || string.IsNullOrWhiteSpace(_sceneToLoad))
            {
                throw new ArgumentException("SceneLoader: SceneToLoad is not valid");
            }

            yield return new WaitForSeconds(0.15f);

            AsyncOperation loadJob = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
            loadJob.allowSceneActivation = true;
            while (!loadJob.isDone)
            {
                LoadUnloadProgress = Mathf.Clamp(loadJob.progress, 0.2f, 1f);
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneToLoad));
            yield return new WaitForSeconds(0.1f);

            OnSceneLoadOnly?.Invoke(_sceneToLoad);
        }

        public void ReloadCurrentGameScene()
        {
            LoadScene(CurrentSceneName);
        }

        public override void SetIdentifiers()
        {
            SetIdentifiers(AllScenes);
        }
    }


    [Serializable]
    public class SceneData : PropertyID
    {
        [Scene] public string SceneName;
    }
}