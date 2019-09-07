using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image border;
    public TextMeshProUGUI playername;

    public void SetUp(Color color, string name)
    {
        border.color = color;
        playername.text = name;
    }
}
