using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace CommonStuff
{
    public abstract class IDCreator : MonoBehaviour
    {
        [SerializeField] private string IdentifiersNameSpace;
        [SerializeField] private string IdentifiersClassName;

        [Button("Generate IDs")]
        private void SetClass()
        {
            SetIdentifiers();
        }
       /// <summary>
       /// After implementing Abstract Method, call another method with "Same Name" i,e. SetIdentifiers(...) but with parameter as a List of "PropertyID or Extended Class"
       /// </summary>
        public abstract void SetIdentifiers();

        protected void SetIdentifiers<T>(List<T> dataList) where T : PropertyID
        {
            if (String.IsNullOrEmpty(IdentifiersClassName)) return;
            
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i].GetType().GetProperty("id") == null)
                {
                    Debug.LogError("Does not have property 'id' @ index " + i);
                    return;
                }
                else
                {
                    String id = (string)dataList[i].GetType().GetProperty("id").GetValue(dataList[i], null);
                    if (String.IsNullOrEmpty(id))
                    {
                        Debug.LogError("Empty property 'id' @ index " + i);
                        return;
                    }
                }
            }
#if UNITY_EDITOR
            string filePathAndName;

            string className = GetType().Name;

            string[] res = Directory.GetFiles(Application.dataPath, className + ".cs", SearchOption.AllDirectories);
            if (res.Length == 0)
            {
                Debug.LogError("Could not get Path for "+className+".cs !!!");
                return;
            }

            string path = res[0].Replace(className + ".cs", IdentifiersClassName + ".cs")
                .Replace("\\", "/");
            path = path.Substring(path.LastIndexOf("Assets/", path.Length));

            Debug.Log(path);

            filePathAndName = path;
            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                if(!String.IsNullOrEmpty(IdentifiersNameSpace)) streamWriter.WriteLine("namespace " + IdentifiersNameSpace+" {");
                streamWriter.WriteLine("public static class " + IdentifiersClassName);
                streamWriter.WriteLine("{");
                for (int i = 0; i < dataList.Count; i++)
                {
                    String id = (string)dataList[i].GetType().GetProperty("id").GetValue(dataList[i], null);
                    streamWriter.WriteLine("\t public static string " + id + " = \"" +
                                           id + "\";");
                }

                streamWriter.WriteLine("}");
                if(!String.IsNullOrEmpty(IdentifiersNameSpace)) streamWriter.WriteLine("}");
            }

            AssetDatabase.Refresh();
#endif
        }
    }

    [Serializable]
    public class PropertyID
    {
        public string identifier;
        public string id
        {
            get { return identifier; }
            set { value = identifier; }
        }
    }
}
