using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame2_Client : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // var move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Debug.Log(move);
        // ConnectionController.Write("Move", "{\"x\":" + move.x + ",\"y\":" + move.y + ",\"z\":0}");
        ConnectionController.Write("Move", "{\"x\":" + Random.Range(-1, 1) + ",\"y\":" + Random.Range(-1, 1) + ",\"z\":0}");
        //ConnectionController.Write("Move", JsonUtility.ToJson(Input.acceleration));
    }
}
