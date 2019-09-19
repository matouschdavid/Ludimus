using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playername;
    public Image ColorBar;
    public void SetUp(string name, Color color)
    {
        playername.text = name;
        ColorBar.color = color;
    }

    public void UpdatePlayer(string name)
    {
        playername.text = name;
    }
}
