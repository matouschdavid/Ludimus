using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame2_Server : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var client in ConnectionController.connectedClients)
        {
            GameObject g = Instantiate(player, Vector3.zero, Quaternion.identity);
            g.GetComponent<TestGame2_PlayerController>().SetUp(client);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
