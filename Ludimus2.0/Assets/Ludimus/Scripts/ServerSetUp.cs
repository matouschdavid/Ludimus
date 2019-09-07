using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSetUp : MonoBehaviour
{
    ControllerBase server;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        server = GetComponent<ControllerBase>();
        server.StartServer(player);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
