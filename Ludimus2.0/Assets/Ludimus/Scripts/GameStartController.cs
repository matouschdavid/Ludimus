using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
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
            SceneManager.LoadScene(data.Value + "_Client", LoadSceneMode.Single);

            SceneManager.LoadScene("PauseOverlay_Client", LoadSceneMode.Additive);
            Debug.Log("Send loaded");
            ConnectionController.Write("Loaded", data.Value);
        }
        if (data.Key == "GameToStart")
        {
            Debug.Log("Got game " + data.Value);
            ConnectionController.Write("Startgame", data.Value, "Public");
            (controller as ServerConnection).WaitForGroupAction("Loaded", data.Value, EveryClientLoaded);
        }
        else if (data.Key == "Endgame")
        {
            if (ConnectionController.IsServer)
            {

                GameObject.Find("PauseController").GetComponent<ServerPauseController>().EndGame();
                ConnectionController.Write("Endgame", "");
            }
            else
            {
                if(connection.ClientId != 0)
                    SceneManager.LoadScene("ClientConnected", LoadSceneMode.Single);
                else
                    SceneManager.LoadScene("ClientShop", LoadSceneMode.Single);
            }
        }
    }

    private void EveryClientLoaded(string d)
    {
        Debug.Log("Every client loaded " + d);
        GameObject.Find("PauseController").GetComponent<ServerPauseController>().StartGame(d);
        SceneManager.LoadScene(d + "_Server", LoadSceneMode.Additive);
    }
    #region Client

    public void Startgame(string game)
    {
        Debug.Log("Startgame " + game);
        ConnectionController.Write("GameToStart", game);
    }
    #endregion
}
