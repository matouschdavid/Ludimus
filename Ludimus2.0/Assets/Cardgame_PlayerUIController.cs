using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cardgame_PlayerUIController : MonoBehaviour
{
    public GameObject selected;
    public TextMeshProUGUI nameT;
    public TextMeshProUGUI currentManaT;
    public TextMeshProUGUI maxManaT;
    private Cardgame_Player player;

    public void SetUp(Cardgame_Player p)
    {
        player = p;
        nameT.text = p.Name;
        currentManaT.text = "0";
        maxManaT.text = "/ " + p.MaxMana;
    }

    public void UpdateUI()
    {
        nameT.text = "Scherge";
        currentManaT.text = player.CurrentMana.ToString();
        maxManaT.text = "/ " + player.MaxMana;
    }
}
