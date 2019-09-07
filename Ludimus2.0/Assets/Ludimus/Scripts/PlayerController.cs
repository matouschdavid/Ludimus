using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Connection.HandleInputDel handlePublicMessage;

    private Queue<Data> handleInputQ = new Queue<Data>();

    private Connection.HandleInputDel handlePrivateMessage;

    private Connection client;

    // Update is called once per frame
    void Update()
    {
        if (handleInputQ.Count > 0)
        {
            HandleInputQueue(handleInputQ.Dequeue());
        }
    }

    public void SetUp(Connection c, Connection.HandleInputDel handlePublicMsg)
    {
        c.HandleInput = HandleInput;
        client = c;
        handlePublicMessage = handlePublicMsg;
    }

    private void HandleInputQueue(Data data)
    {
        Debug.Log("Private");
        if (handlePrivateMessage == null)
            return;
        handlePrivateMessage.Invoke(data);
    }

    private void HandleInput(Data data)
    {
        Debug.Log("Here again with id: " + client.ClientId);
        if (data.Region == "Public")
        {
            Debug.Log("IsPublic");
            handlePublicMessage.Invoke(data);
            return;
        }
        else
            handleInputQ.Enqueue(data);

    }

    public void AttachMsgHandler(Connection.HandleInputDel handler)
    {
        handlePrivateMessage += handler;
    }

    public void Write(Data data)
    {
        ConnectionController.Write(data, client);
    }
}
