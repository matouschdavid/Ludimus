using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image border;
    public TextMeshProUGUI playername;
    public string plname;

    public void SetUp(Color color, string name)
    {
        plname = name;
        border.color = color;
        playername.text = name;
    }

    public void UpdatePlayer(string name)
    {
        plname = name;
        Debug.LogError("Here with " + name);
        playername.text = name;
    }
}
