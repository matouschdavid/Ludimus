using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playername;

    public void SetUp(string name)
    {
        playername.text = name;
    }

    public void UpdatePlayer(string name)
    {
        playername.text = name;
    }
}
