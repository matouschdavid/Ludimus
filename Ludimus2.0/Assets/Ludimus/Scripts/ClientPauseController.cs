using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientPauseController : MonoBehaviour
{
    public GameObject inGameUI;
    public GameObject inPauseUI;
    public Color OnColor;
    public Color OffColor;
    // Start is called before the first frame update
    void Start()
    {
        ConnectionController.GetControllerInstance<ClientConnection>().AttachPauseHandler(PauseCallback);
    }

    private void PauseCallback(bool isPaused)
    {
        if (inGameUI == null)
            inGameUI = GameObject.Find("inGameMode");
        if (inPauseUI == null)
            inPauseUI = GameObject.Find("inPauseMode");
        if (isPaused)
        {
            inGameUI.GetComponent<Image>().color = OffColor;
            inPauseUI.GetComponent<Image>().color = OnColor;
        }
        else
        {
            inGameUI.GetComponent<Image>().color = OnColor;
            inPauseUI.GetComponent<Image>().color = OffColor;

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
