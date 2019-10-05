using Firebase.Database;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopController : MonoBehaviour
{
    public GameObject gameUI;
    public GameStartController gameStartController;
    public Transform list;
    private Queue<(Game, byte[])> gameQueue = new Queue<(Game, byte[])>();
    // Start is called before the first frame update
    void Start()
    {
        //GetGamesFromServer();
        GetGamesFromDisk();
    }

    private void GetGamesFromServer(string search = "")
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var root = FirebaseStorage.DefaultInstance.RootReference;
        reference.Child("gamesInDevelopment").GetValueAsync().ContinueWith(async t =>
        {
            var snapshot = t.Result;
            var dic = snapshot.Value as Dictionary<string, System.Object>;
            foreach (var dicEntry in dic)
            {
                string key = dicEntry.Key;
                var gameDic = dicEntry.Value as Dictionary<string, System.Object>;
                var gamename = gameDic["Gamename"] as string;
                var minPlayers = Convert.ToInt32(gameDic["MinPlayers"]);
                var maxPlayers = Convert.ToInt32(gameDic["MaxPlayers"]);
                var minAge = Convert.ToInt32(gameDic["MinAge"]);
                var maxAge = Convert.ToInt32(gameDic["MaxAge"]);
                var storagePath = gameDic["StoragePath"] as string;
                Game g = new Game(gamename, minPlayers, maxPlayers, minAge, maxAge, storagePath);

                //var bytes = await root.Child(g.StoragePath + "/" + key + "/icon.png").GetBytesAsync(long.MaxValue);
                var bytes = await root.Child(g.StoragePath + "/" + key + "/icon.jpg").GetBytesAsync(long.MaxValue);
                gameQueue.Enqueue((g, bytes));



            }
        });
    }

    private void GetGamesFromDisk(string search = "")
    {
        var gamefolders = Directory.GetDirectories("Assets/Ludimus/Games");
        foreach (var folder in gamefolders)
        {
            
            var gamename = Path.GetFileNameWithoutExtension(folder);
            Debug.Log(gamename.Contains(search));
            if (search == "" || gamename.Contains(search))
            {
                var g = new Game(gamename, -1, 1, -1, -1, "");
                var gUI = Instantiate(gameUI, list);
                var gameUIController = gUI.GetComponent<GameUI>();
                Texture2D tex = new Texture2D((int)gameUIController.iconI.uvRect.width, (int)gameUIController.iconI.uvRect.height);
                ImageConversion.LoadImage(tex, File.ReadAllBytes(folder + "/icon.jpg"));
                gameUIController.SetUp(gamename, tex, OnClick);
            }
        }
    }

    private void OnClick(string gamename)
    {
        gameStartController.Startgame(gamename);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameQueue.Count > 0)
        {
            var g = gameQueue.Dequeue();
            var gUI = Instantiate(gameUI, list);
            var gameUIController = gUI.GetComponent<GameUI>();
            Texture2D tex = new Texture2D((int)gameUIController.iconI.uvRect.width, (int)gameUIController.iconI.uvRect.height);
            tex.LoadImage(g.Item2);
            gameUIController.SetUp(g.Item1.Gamename, tex, OnClick);

            
        }
    }

    public void OnSearchEditEnd(string s)
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("GameUI"))
        {
            Destroy(obj);
        }
        GetGamesFromDisk(s);
    }
}
