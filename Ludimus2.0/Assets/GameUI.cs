using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI gamenameT;
    public RawImage iconI;

    public void SetUp(string name, Texture2D icon)
    {
        gamenameT.text = name;
        iconI.texture = icon;
    }
}
