using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestGame1_WritingController : MonoBehaviour
{
    ServerConnection server;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        server = ConnectionController.GetControllerInstance<ServerConnection>();
        server.AttachMessageHandler(MessageCallback);
        ConnectionController.Write("WannaStart", "true");
        server.WaitForGroupAction("IWannaStart", "", EveryPlayerWantsToStart);
        server.AttachPauseHandler(PauseCallback);
    }

    private void PauseCallback(bool isPaused)
    {
        Debug.Log("Game is paused: " + isPaused);
    }

    private void MessageCallback(Data data, Connection connection)
    {
        if (data.Key == "Message")
        {
            text.text += "\n" + connection.Playername + ": " + data.Value;
            ConnectionController.Write(connection, "Received", "true");
        }

    }

    private void EveryPlayerWantsToStart(string d)
    {
        ConnectionController.Write("CanStart", "true");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
