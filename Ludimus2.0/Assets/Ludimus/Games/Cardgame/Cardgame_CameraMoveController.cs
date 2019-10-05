using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cardgame_CameraMoveController : MonoBehaviour
{
    private Vector3 lastMousePosition;
    public float Speed = 5;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (Vector3.up * (Input.mousePosition.x - lastMousePosition.x) * Time.deltaTime * Speed));
            var z = Vector3.Lerp(cam.transform.localPosition, cam.transform.localPosition - Vector3.forward * (Input.mousePosition.y - lastMousePosition.y), Speed * Time.deltaTime).z;
            z = Mathf.Min(z, 0);
            z = Mathf.Max(z, -25);
            if(Mathf.Abs(Mathf.Abs(Input.mousePosition.y) - Mathf.Abs(lastMousePosition.y)) > 3)
                cam.transform.localPosition = new Vector3(0, 13, z);
            lastMousePosition = Input.mousePosition;
        }
            
        
    }
}
