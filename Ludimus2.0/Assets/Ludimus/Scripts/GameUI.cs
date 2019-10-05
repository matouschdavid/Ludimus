using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public delegate void GameStartDel(string gamename);
    public TextMeshProUGUI gamenameT;
    public RawImage iconI;
    private GameStartDel startCallback;

    public void SetUp(string name, Texture2D icon, GameStartDel startCallback)
    {
        gamenameT.text = name;
        iconI.texture = icon;
        this.startCallback = startCallback;
    }

    public void OnCLick()
    {
        startCallback(gamenameT.text);
    }
}
