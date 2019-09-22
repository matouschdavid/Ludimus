using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame1_WritingTest : MonoBehaviour
{
    ClientConnection client;
    bool canWrite = false;
    // Start is called before the first frame update
    void Start()
    {
        client = ConnectionController.GetControllerInstance<ClientConnection>();
        client.AttachMessageHandler(MessageCallback);
    }

    private void MessageCallback(Data data, Connection connection)
    {
        if (data.Key == "WannaStart")
        {
            ConnectionController.Write("IWannaStart", "true");
        }
        if (data.Key == "CanStart")
        {
            canWrite = true;
            Debug.Log("Can write now");
        }
    }

    public void EndEdit(string s)
    {
        if (!canWrite)
        {
            Debug.LogError("Cant write yet");
        }
        else
        {
            ConnectionController.Write("Message", s);
        }
    }
}
