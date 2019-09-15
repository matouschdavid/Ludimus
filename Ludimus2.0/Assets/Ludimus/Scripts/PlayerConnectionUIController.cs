using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerConnectionUIController : MonoBehaviour
{
    private ServerConnection server;
    public GameObject playerUI;
    public Transform list;

    public TextMeshProUGUI playersText;
    public TextMeshProUGUI bigPlayersText;
    // Start is called before the first frame update
    void Start()
    {
        server = ConnectionController.GetControllerInstance<ServerConnection>();
        server.AttachNewConnectionHandler(NewPlayerConnected);
    }

    private void NewPlayerConnected(Connection connection)
    {
        Debug.Log("create new playerUI");
        //Add to list
        GameObject g = Instantiate(playerUI, list);
        g.GetComponent<PlayerUI>().SetUp(connection.Playername);
        playersText.text = ConnectionController.connectedClients.Count.ToString();
        bigPlayersText.text = ConnectionController.connectedClients.Count.ToString();
        server.AddPlayerUIRef(g.GetComponent<PlayerUI>(), connection);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
