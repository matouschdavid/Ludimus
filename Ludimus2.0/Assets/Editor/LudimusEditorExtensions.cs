using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LudimusEditorExtensions : EditorWindow
{
    [MenuItem("Ludimus/Start in Editor/Server")]
    static void StartServer()
    {
        EditorSceneManager.OpenScene("Assets/Ludimus/Scenes/Server.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Ludimus/Start in Editor/Client")]
    static void StartClient()
    {
        EditorSceneManager.OpenScene("Assets/Ludimus/Scenes/ClientLoggedIn.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Ludimus/Build/Server/Android")]
    static void BuildServerForAndroid()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        List<string> levels = new List<string> { "Assets/Ludimus/Server.unity", "Assets/Ludimus/ClientLoggedIn.unity", "Assets/Ludimus/ClientConnected.unity", "Assets/Ludimus/PauseOverlay_Client.unity", "Assets/Ludimus/PauseOverlay_Server.unity" };
        string gamesFolderPath = Application.dataPath + "/Assets/Ludimus/Games/";
        string[] scenes = Directory.GetFiles(gamesFolderPath, "*.unity", SearchOption.AllDirectories);
        levels.AddRange(scenes);

        // Build player.
        BuildPipeline.BuildPlayer(levels.ToArray(), path + "/Server.apk", BuildTarget.Android, BuildOptions.AllowDebugging);

        Process.Start("cmd", "/c " + "adb install " + path + "/Server.apk");
    }

    [MenuItem("Ludimus/Build/Server/Windows")]
    static void BuildServerForWindows()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        List<string> levels = new List<string> { "Assets/Ludimus/Scenes/Server.unity", "Assets/Ludimus/Scenes/ClientLoggedIn.unity", "Assets/Ludimus/Scenes/ClientConnected.unity", "Assets/Ludimus/Scenes/PauseOverlay_Client.unity", "Assets/Ludimus/Scenes/PauseOverlay_Server.unity" };
        string gamesFolderPath = Application.dataPath + "/Ludimus/Games/";
        string[] scenes = Directory.GetFiles(gamesFolderPath, "*.unity", SearchOption.AllDirectories);
        levels.AddRange(scenes);

        // Build player.
        BuildPipeline.BuildPlayer(levels.ToArray(), path + "/Server.exe", BuildTarget.StandaloneWindows, BuildOptions.AllowDebugging);

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "/Server.exe";
        proc.Start();
    }

    [MenuItem("Ludimus/Build/Client/Android")]
    static void BuildClientForAndroid()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        List<string> levels = new List<string> { "Assets/Ludimus/Scenes/ClientLoggedIn.unity", "Assets/Ludimus/Scenes/ClientConnected.unity", "Assets/Ludimus/Scenes/PauseOverlay_Client.unity", "Assets/Ludimus/Scenes/PauseOverlay_Server.unity" };
        string gamesFolderPath = Application.dataPath + "/Ludimus/Games/";
        string[] scenes = Directory.GetFiles(gamesFolderPath, "*.unity", SearchOption.AllDirectories);
        levels.AddRange(scenes);

        // Build player.
        BuildPipeline.BuildPlayer(levels.ToArray(), path + "/Client.apk", BuildTarget.Android, BuildOptions.AllowDebugging);

        Process.Start("cmd", "/c " + "adb install " + path + "/Client.apk");
        Process p = new Process();
        p.StartInfo.FileName = "cmd";
        p.StartInfo.Arguments = "adb install " + path + "/Client.apk";
        p.StartInfo.CreateNoWindow = true;
        UnityEngine.Debug.Log("Before command start");
        p.Start();
        p.WaitForExit();
        UnityEngine.Debug.Log("Command executed");
    }

    [MenuItem("Ludimus/Build/Client/Windows")]
    static void BuildClientForWindows()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        List<string> levels = new List<string> { "Assets/Ludimus/Scenes/ClientLoggedIn.unity", "Assets/Ludimus/Scenes/ClientConnected.unity", "Assets/Ludimus/Scenes/PauseOverlay_Client.unity", "Assets/Ludimus/Scenes/PauseOverlay_Server.unity" };
        string gamesFolderPath = Application.dataPath + "/Ludimus/Games/";
        string[] scenes = Directory.GetFiles(gamesFolderPath, "*.unity", SearchOption.AllDirectories);
        levels.AddRange(scenes);

        // Build player.
        BuildPipeline.BuildPlayer(levels.ToArray(), path + "/Client.exe", BuildTarget.StandaloneWindows, BuildOptions.AllowDebugging);

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "/Client.exe";
        proc.Start();
    }


    [MenuItem("Ludimus/Upload Game")]
    static void BuildAllAssetBundles()
    {
        EditorWindow window = GetWindow(typeof(LudimusEditorExtensions));
        window.Show();


    }

    public int selected = 0;
    public int minplayers = 0;
    public int maxplayers = 8;
    public int minage = 0;
    public int maxage = 99;
    public Texture2D icon;
    public string gamename;
    private void OnGUI()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ludimus-1b1db.firebaseio.com/");
            }
            else
            {
                UnityEngine.Debug.LogError(string.Format(
                    "Could not resolve all Firebase dependencies: {0}", task.Result));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        string gamesFolderPath = Application.dataPath + "/Ludimus/Games/";
        var options = Directory.GetDirectories(gamesFolderPath).Select(d =>
        {
            string[] splitted = d.Split('/');
            return splitted[splitted.Length - 1];
        }).ToArray();
        selected = EditorGUILayout.Popup("Which files to upload", selected, options);
        gamename = EditorGUILayout.TextField("Name: ", gamename);
        EditorGUILayout.TextField("", "", new GUIStyle
        {
            active = new GUIStyleState()
        });
        minplayers = EditorGUILayout.IntSlider("Min. players: ",minplayers, 0, 8);
        maxplayers = EditorGUILayout.IntSlider("Max. players: ", maxplayers, minplayers, 8);
        EditorGUILayout.TextField("", "", new GUIStyle
        {
            active = new GUIStyleState()
        });
        minage = EditorGUILayout.IntSlider("Min. age: ", minage, 0, 99);
        maxage = EditorGUILayout.IntSlider("Max. age: ", maxage, minage, 99);
        EditorGUILayout.TextField("", "", new GUIStyle
        {
            active = new GUIStyleState()
        });

        icon = EditorGUILayout.ObjectField("Icon", icon, typeof(Texture2D), false) as Texture2D;
        if (GUILayout.Button("Upload"))
            StartUploadingGame(options);
    }

    private async void StartUploadingGame(string[] options)
    {
        string gamesFolderPath = Application.dataPath + "/Ludimus/Games/" + options[selected];
        string[] scenesFullPath = Directory.GetFiles(gamesFolderPath, "*.unity", SearchOption.AllDirectories);
        string[] scenes = scenesFullPath.Select(s =>
        {
            string pattern = "Assets/Ludimus/Games/" + options[selected] + ".*";
            var match = Regex.Match(s, pattern);
            UnityEngine.Debug.Log("Scene: " + match.Value);
            return match.Value;
        }).ToArray();
        string[] assetsTmp = Directory.GetFiles(gamesFolderPath, "*.*", SearchOption.AllDirectories);
        List<string> assetsFullPath = new List<string>();
        foreach (var item in assetsTmp)
        {
            string[] splitted = item.Split('.');
            if (splitted[splitted.Length - 1] != "unity" && splitted[splitted.Length - 1] != "meta" && splitted[splitted.Length - 1] != "cs")
            {
                assetsFullPath.Add(item);
            }
        }
        string[] assets = assetsFullPath.Select(s =>
        {
            string pattern = "Assets/Ludimus/Games/" + options[selected] + ".*";
            var match = Regex.Match(s, pattern);
            UnityEngine.Debug.Log("Asset: " + match.Value);
            return match.Value;
        }).ToArray();
        AssetBundleBuild[] assetBundle = new AssetBundleBuild[2];
        assetBundle[0] = new AssetBundleBuild();
        assetBundle[0].assetBundleName = options[selected] + "_Scenes";
        assetBundle[0].assetNames = scenes;
        assetBundle[1] = new AssetBundleBuild();
        assetBundle[1].assetBundleName = options[selected] + "_Assets";
        assetBundle[1].assetNames = assets;


        string assetBundleDirectory = "Assets/AssetBundles/Windows";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, assetBundle, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        assetBundleDirectory = "Assets/AssetBundles/Android";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, assetBundle, BuildAssetBundleOptions.None, BuildTarget.Android);

        string[] scriptsFullPath = Directory.GetFiles(gamesFolderPath, "*.cs", SearchOption.AllDirectories);

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var id = reference.Push().Key;
        var storage = FirebaseStorage.DefaultInstance;
        var rootRef = storage.RootReference;
        var gameRef = rootRef.Child("games/" + id);
        var iconFilePath = AssetDatabase.GetAssetPath(icon);
        var new_metadata = new Firebase.Storage.MetadataChange();
        new_metadata.ContentType = "image/jpeg";
        var result = await gameRef.Child("icon" + Path.GetExtension(iconFilePath)).PutFileAsync(iconFilePath, new_metadata);
        foreach (var script in scriptsFullPath)
        {
            var res = await gameRef.Child("/" + Path.GetFileName(script)).PutFileAsync(script);
            UnityEngine.Debug.Log(res.Name + " is completed");
        }
        var androidAssetBundels = rootRef.Child("games/" + id + "/AssetBundles/Android/");
        foreach (var file in Directory.GetFiles("Assets/AssetBundles/Android"))
        {
            if (Path.GetFileNameWithoutExtension(file).Contains(options[selected].ToLower()))
            {
                var res = await androidAssetBundels.Child(Path.GetFileName(file)).PutFileAsync(file);
            }
        }
        var windowsAssetBundels = rootRef.Child("games/" + id + "/AssetBundles/Windows/");
        foreach (var file in Directory.GetFiles("Assets/AssetBundles/Windows"))
        {
            if (Path.GetFileNameWithoutExtension(file).Contains(options[selected].ToLower()))
            {
                var res = await windowsAssetBundels.Child(Path.GetFileName(file)).PutFileAsync(file);
            }
        }

        
        Game game = new Game(gamename, minplayers, maxplayers, minage, maxage, "games");
        
        await reference.Child("gamesInDevelopment/" + id).SetRawJsonValueAsync(JsonUtility.ToJson(game));
        UnityEngine.Debug.Log("Game upload finished. To Access the game use " + id + " this key.\nIf this is your final build, you can publish your game with the publish button");
    }
}
