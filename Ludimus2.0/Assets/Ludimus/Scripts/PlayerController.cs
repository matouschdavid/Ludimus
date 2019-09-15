using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConnectionController;

public class PlayerController : MonoBehaviour
{
    protected Connection client;

    public virtual void SetUp(Connection c)
    {
        client = c;
    }

    // private HandleInputDel handlePublicMessage;

    // private Queue<Data> handleInputQ = new Queue<Data>();

    // private HandleInputDel handlePrivateMessage;

    // private ConnectionNew client;

    // // Update is called once per frame
    // void Update()
    // {
    //     if (handleInputQ.Count > 0)
    //     {
    //         HandleInputQueue(handleInputQ.Dequeue());
    //     }
    // }

    // public void SetUp(ConnectionNew c, HandleInputDel handlePublicMsg)
    // {
    //     c.HandleInput = HandleInput;
    //     client = c;
    //     handlePublicMessage = handlePublicMsg;
    // }

    // private void HandleInputQueue(Data data)
    // {
    //     Debug.Log("Private");
    //     if (handlePrivateMessage == null)
    //         return;
    //     handlePrivateMessage.Invoke(data);
    // }

    // private void HandleInput(Data data)
    // {
    //     Debug.Log("Here again with id: " + client.ClientId);
    //     if (data.Region == "Public")
    //     {
    //         Debug.Log("IsPublic");
    //         handlePublicMessage.Invoke(data);
    //         return;
    //     }
    //     else
    //         handleInputQ.Enqueue(data);

    // }

    // public void AttachMsgHandler(HandleInputDel handler)
    // {
    //     handlePrivateMessage += handler;
    // }

    // public void Write(Data data)
    // {
    //     ConnectionControllerNew.Write(data, client);
    // }
}
