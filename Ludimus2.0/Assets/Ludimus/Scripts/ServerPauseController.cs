using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerPauseController : MonoBehaviour
{
    private ServerConnection server;
    public GameObject ui;
    private string currentGame;
    // Start is called before the first frame update
    void Start()
    {
        server = ConnectionController.GetControllerInstance<ServerConnection>();
        server.AttachPauseHandler(PauseCallback);
    }

    private void PauseCallback(bool isPaused)
    {
        if (isPaused)
        {
            ui.SetActive(true);
            ConnectionController.Write("StartPause", "");
            server.WaitForGroupAction("EndPause", "", AllClientsWantToResume);
        }
    }

    public void StartGame(string game)
    {
        currentGame = game;
        ui.SetActive(false);
    }

    public void EndGame()
    {
        ui.SetActive(true);
        SceneManager.UnloadSceneAsync(currentGame + "_Server");
    }

    private void AllClientsWantToResume(string d)
    {
        Debug.Log("All clients want to resume");
        ui.SetActive(false);
        ConnectionController.Write("EndPause", "");
    }
}

