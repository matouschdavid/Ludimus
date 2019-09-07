using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerConnectionUIController : MonoBehaviour
{
    private ControllerBase server;
    public GameObject playerUI;
    public Transform list;

    public TextMeshProUGUI playersText;
    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.Find("Controller").GetComponent<ControllerBase>();
        server.AttachConnectedHandler(NewPlayerConnected);
    }

    private void NewPlayerConnected(Connection connection, PlayerController player)
    {
        //Add to list
        GameObject g = Instantiate(playerUI, list);
        g.GetComponent<PlayerUI>().SetUp(Color.blue, connection.Playername);
        playersText.text = ConnectionController.connectedClients.Count + " / 8 players connected";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
