using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerSetUp : MonoBehaviour
{
    ServerConnection server;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        server = GetComponent<ServerConnection>();
        server.StartServer(MessageCallback, ConnectedCallback);
        SceneManager.LoadScene("PauseOverlay_Server", LoadSceneMode.Additive);
    }

    private void MessageCallback(Data data, Connection connection)
    {
        Debug.Log("Got message: " + data);
        // ConnectionController.Write(new Data { Key = "FromServerToClient", Value = "Got message: " + data });
    }

    private void ConnectedCallback(Connection connection)
    {
        Debug.Log("New Player connected");
        GameObject p = Instantiate(player);
    }
}
