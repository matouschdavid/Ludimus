using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameChangeController : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        inputField.text = ConnectionController.Client.Playername;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnChange(string s)
    {
        Debug.Log("Here with " + s);
        ConnectionController.Write(new Data { Key = "Playername", Value = s }, ConnectionController.Client);
    }
}
