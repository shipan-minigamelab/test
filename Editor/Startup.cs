using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class Startup
{
    private const string PkgDir = "Packages";
    private const string DestDir = "Assets";
    public const string ImportMoveDir = "ImportMove~";

    static Startup()
    {
        Debug.Log("Up and running");
        CopyLionFiles();
    }

    private static string pkgId
    {
        get
        {
            string packageId = string.Empty;
            string pathTemplate = "Packages/com.minigamelab.{0}.mgl";

            string packagePath = Path.GetFullPath(string.Format(pathTemplate, "release"));
            DirectoryInfo pathToCheck = new DirectoryInfo(packagePath);
            packageId = "com.minigamelab.release.mgl";

            // if (!pathToCheck.Exists)
            // {
            //     packagePath = Path.GetFullPath(string.Format(pathTemplate, "dev"));
            //     pathToCheck = new DirectoryInfo(packagePath);
            //     packageId = "com.lionstudios.dev.lionkit";
            //
            //     if (!pathToCheck.Exists)
            //     {
            //         packagePath = Path.GetFullPath(string.Format(pathTemplate, "beta"));
            //         pathToCheck = new DirectoryInfo(packagePath);
            //         packageId = "com.lionstudios.beta.lionkit";
            //
            //         if (!pathToCheck.Exists)
            //         {
            //             packagePath = Path.GetFullPath(string.Format(pathTemplate, "internal"));
            //             pathToCheck = new DirectoryInfo(packagePath);
            //             packageId = "com.lionstudios.internal.lionkit";
            //         }
            //     }
            // }

            return packageId;
        }
    }

    public static List<string> CopyPaths
    {
        get
        {
#if UNITY_EDITOR_WIN
				return CopyPathsWIN;
#else
            return CopyPathsOSX;
#endif
        }
    }

    public static readonly List<string> CopyPathsOSX = new List<string>
    {
        "Minigamelab"
    };

    public static readonly List<string> CopyPathsWIN = new List<string>
    {
        "Minigamelab"
    };

#if UNITY_EDITOR_WIN
		private static readonly List<string> NoOverwrite = new List<string>
		{
			"Plugins\\Android\\gradleTemplate.properties",
			//"Plugins/Android/launcherTemplate.gradle",
			"Plugins\\Android\\mainTemplate.gradle",
			"Plugins\\Android\\baseProjectTemplate.gradle",
			"Plugins\\Android\\res\\xml\\network_security_config.xml",
		};
#else
    private static readonly List<string> NoOverwrite = new List<string>
    {
        "Plugins/Android/gradleTemplate.properties",
        //"Plugins/Android/launcherTemplate.gradle",
        "Plugins/Android/mainTemplate.gradle",
        "Plugins/Android/baseProjectTemplate.gradle",
        "Plugins/Android/res/xml/network_security_config.xml",
    };
#endif
    private static void CopyFileOrDirectory(string src, string dst)
    {
        bool isFile = File.Exists(src);
        bool isDirectory = Directory.Exists(src);

        // if source is a file, do move and return.
        if (isFile && !isDirectory)
        {
            if (File.Exists(dst))
            {
                File.Delete(dst);
            }

            FileUtil.MoveFileOrDirectory(src, dst);
            return;
        }
        else if (!isFile && isDirectory) // move the files/directories in this directory
        {
            // get info about src and dst
            DirectoryInfo srcInfo = new DirectoryInfo(src);
            DirectoryInfo dstInfo = new DirectoryInfo(dst);

            // if dst doesnt exist, create it.
            if (!dstInfo.Exists) dstInfo.Create();

            FileInfo[] srcFiles = srcInfo.GetFiles();
            for (int i = 0; i < srcFiles.Length; i++)
            {
                FileInfo file = srcFiles[i];
                if (file != null)
                {
#if UNITY_EDITOR_WIN
						string dstFilePath = $"{dstInfo.FullName}\\{file.Name}";
#else
                    string dstFilePath = $"{dstInfo.FullName}/{file.Name}";
#endif
                    if (File.Exists(dstFilePath))
                    {
                        File.Delete(dstFilePath);
                    }

                    FileUtil.MoveFileOrDirectory(file.FullName, dstFilePath);
                }
            }

            // move all directories
            DirectoryInfo[] srcDirectories = srcInfo.GetDirectories();
            for (int d = 0; d < srcDirectories.Length; d++)
            {
                DirectoryInfo directory = srcDirectories[d];
                if (directory != null)
                {
#if UNITY_EDITOR_WIN
						string dstDirPath = $"{dst}\\{directory.Name}";
#else
                    string dstDirPath = $"{dst}/{directory.Name}";
#endif
                    DirectoryInfo dstDirectory = new DirectoryInfo(dstDirPath);
                    CopyFileOrDirectory(directory.FullName, dstDirectory.FullName);
                }
            }
        }
    }

    [MenuItem("MGL/Advanced/Run Post-Import Move")]
    private static void CopyLionFiles()
    {
#if !LION_KIT_DEV
        foreach (string copyPath in CopyPaths)
        {
            string pkgPath = PkgDir + "/" + pkgId + "/" + ImportMoveDir + "/" + copyPath;

            if (CheckExistence(pkgPath))
            {
                if (NoOverwrite.Contains(copyPath))
                {
                    continue;
                }

                if (copyPath == "Plugins/Android/mainTemplate.gradle")
                {
                    UnityEngine.Debug.Log("mainTemplate.gradle updated - RESOLVING DEPENDENCIES");
                }

                // replace files/folders
                string destPath = DestDir + "/" + copyPath;
                CopyFileOrDirectory(pkgPath, destPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    private static bool CheckExistence(string location)
    {
        return File.Exists(location) ||
               Directory.Exists(location) ||
               location.EndsWith("/*") && Directory.Exists(Path.GetDirectoryName(location));
    }
}