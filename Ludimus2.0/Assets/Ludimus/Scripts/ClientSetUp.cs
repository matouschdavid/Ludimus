using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSetUp : MonoBehaviour
{
    ControllerBase client;
    private string playername;
    private string lobbycode;

    // Start is called before the first frame update
    void Start()
    {
        client = GetComponent<ControllerBase>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Connect()
    {
        Debug.Log("Connect");
        client.AttachConnectedHandler(PlayerConnected);
        client.Connect(playername);
    }

    private void PlayerConnected(Connection connection, PlayerController player)
    {
        Debug.Log("Connected");
        SceneManager.LoadScene("ClientConnected", LoadSceneMode.Single);
    }

    public void OnPlayerNameChanged(string s)
    {
        playername = s;
    }

    public void OnLobbyCodeChanged(string s)
    {
        lobbycode = s;
    }
}
