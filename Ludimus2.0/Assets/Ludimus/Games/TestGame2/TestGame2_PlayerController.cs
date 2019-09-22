using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame2_PlayerController : PlayerController
{
    Rigidbody rb;
    public float speed;
    public TextMesh nameField;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void SetUp(Connection c)
    {
        base.SetUp(c);
        client.HandleInput += MessageCallback;
        nameField.text = c.Playername;
    }

    private void MessageCallback(Data data, Connection connection)
    {
        Debug.Log("Got message ingame: " + data);
        if (data.Key == "Move")
        {
            Move(JsonUtility.FromJson<Vector3>(data.Value));
        }
    }

    private void Move(Vector3 value)
    {
        rb.AddForce(value * speed);
    }
}
