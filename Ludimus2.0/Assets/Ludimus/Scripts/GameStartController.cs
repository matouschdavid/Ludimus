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
            Debug.Log("Load client");
            SceneManager.LoadScene(data.Value + (ConnectionController.IsServer ? "_Server" : "_Client"), LoadSceneMode.Single);
            SceneManager.LoadScene("PauseOverlay_Client", LoadSceneMode.Additive);
            if (!ConnectionController.IsServer)
            {
                Debug.Log("Send loaded");
                ConnectionController.Write("Loaded", data.Value);
            }
        }
        if (data.Key == "GameToStart")
        {
            Debug.Log("Got game " + data.Value);
            ConnectionController.Write("Startgame", data.Value, "Public");
            (controller as ServerConnection).WaitForGroupAction("Loaded", data.Value, EveryClientLoaded);
        }
    }

    private void EveryClientLoaded(string d)
    {
        Debug.Log("Every client loaded " + d);
        SceneManager.UnloadSceneAsync("PauseOverlay_Server");
        SceneManager.LoadScene(d + "_Server", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Client
    public void ChangeGamename(string gamename)
    {
        this.gamename = gamename;
        Startgame();
    }

    public void Startgame()
    {
        Debug.Log("Startgame " + gamename);
        ConnectionController.Write("GameToStart", gamename);
    }
    #endregion
}
