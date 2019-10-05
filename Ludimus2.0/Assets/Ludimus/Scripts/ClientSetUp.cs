using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientSetUp : MonoBehaviour
{
    ClientConnection client;
    public TMP_InputField nameT;
    public TMP_InputField lobbyCT;
    private string playername;
    private string lobbycode;

    // Start is called before the first frame update
    void Start()
    {
        client = GetComponent<ClientConnection>();
        nameT.text = PlayerPrefs.GetString("Playername", "");
        lobbyCT.text = PlayerPrefs.GetString("Lobbycode", "");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Connect()
    {
        client.Connect(lobbycode, playername, null, PlayerConnected);
    }

    private void PlayerConnected(Connection connection)
    {
        Debug.Log("Connected");
        if (connection.ClientId != 0)
            SceneManager.LoadScene("ClientConnected", LoadSceneMode.Single);
        else
            SceneManager.LoadScene("ClientShop", LoadSceneMode.Single);
    }

    public void OnPlayerNameChanged(string s)
    {
        playername = s;
        PlayerPrefs.SetString("Playername", playername);
    }

    public void OnLobbyCodeChanged(string s)
    {
        lobbycode = s;
        PlayerPrefs.SetString("Lobbycode", lobbycode);
    }
}
