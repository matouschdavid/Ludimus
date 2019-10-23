using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cardgame_CameraMoveController : MonoBehaviour
{
    private Vector3 lastMousePosition;
    public float Speed = 5;
    // Start is called before the first frame update
    void Start()
    {
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
            
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (Vector3.up * (Input.mousePosition.x - lastMousePosition.x) * Time.deltaTime * Speed));
            var z = Vector3.Lerp(transform.position, transform.position - Vector3.forward * (Input.mousePosition.y - lastMousePosition.y), Speed * Time.deltaTime).z;

            z = Mathf.Min(z, 5.5f);
            z = Mathf.Max(z, -5.5f);
            //if(Mathf.Abs(Mathf.Abs(Input.mousePosition.y) - Mathf.Abs(lastMousePosition.y)) > 3)
            transform.position = new Vector3(-10, 17.56f, z);
            lastMousePosition = Input.mousePosition;
        }
            
        
    }

    public void DisableAnim()
    {
        GetComponent<Animator>().enabled = false;
    }
}
