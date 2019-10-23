using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardgameUI_FaceCameraController : MonoBehaviour
{
    public Transform CameraCounterPoint;
    // Start is called before the first frame update
    void Start()
    {
        CameraCounterPoint = GameObject.Find("CounterPoint").transform;
    }

    // Update is called once per frame
    void Update()
    {

        transform.LookAt(CameraCounterPoint);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
