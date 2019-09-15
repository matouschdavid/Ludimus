using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientPauseController : MonoBehaviour
{
    public GameObject inGameUI;
    public GameObject inPauseUI;
    // Start is called before the first frame update
    void Start()
    {
        ConnectionController.GetControllerInstance<ClientConnection>().AttachMessageHandler(MessageCallback);
    }

    private void MessageCallback(Data data, Connection connection)
    {
        if (data.Key == "StartPause")
        {
            inGameUI.SetActive(false);
            inPauseUI.SetActive(true);
        }
        if (data.Key == "EndPause")
        {
            inGameUI.SetActive(true);
            inPauseUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {
        ConnectionController.Write("StartPause", "");
    }

    public void UnPause()
    {
        ConnectionController.Write("EndPause", "");
    }

    public void BackToLobby()
    {
        ConnectionController.Write("Endgame", "");
    }
}
