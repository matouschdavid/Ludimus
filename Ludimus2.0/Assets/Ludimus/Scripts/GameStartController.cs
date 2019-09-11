using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
    private string gamename;
    private ControllerBase controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = ConnectionController.GetControllerInstance<ControllerBase>();
        controller.AttachMessageHandler(MessageCallback);
    }

    private void MessageCallback(Data data, Connection connection)
    {
        if (data.Key == "Startgame")
        {
            SceneManager.LoadScene(data.Value + (ConnectionController.IsServer ? "_Server" : "_Client"), LoadSceneMode.Single);
            if (!ConnectionController.IsServer)
            {
                ConnectionController.Write("Loaded", data.Value);
            }
        }
        if (data.Key == "GameToStart")
        {
            ConnectionController.Write("Startgame", data.Value, "Public");
            (controller as ServerConnection).WaitForGroupAction("Loaded", EveryClientLoaded);
        }
    }

    private void EveryClientLoaded(Data d)
    {
        SceneManager.LoadScene(d.Value + "_Server", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Client
    public void ChangeGamename(string gamename)
    {
        this.gamename = gamename;
    }

    public void Startgame()
    {
        ConnectionController.Write("GameToStart", gamename);
    }
    #endregion
}
