using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cardgame_CardController : MonoBehaviour
{
    public Image pictureI;
    public TextMeshProUGUI typeT;   
    public TextMeshProUGUI nameT;
    public TextMeshProUGUI descriptionT;
    public TextMeshProUGUI rangeT;
    public TextMeshProUGUI dmgT;
    public TextMeshProUGUI manaT;
    public TextMeshProUGUI leftTitleT;
    public GameObject selected;

    public Cardgame_Card CardData;
    public bool isActive = true;
    private Cardgame_Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<Cardgame_Controller>();
    }

    internal void SetUp(Cardgame_Card card, bool newTurn)
    {
        CardData = card;
        if (newTurn)
        {
            GetComponent<Animator>().SetTrigger("Turn");
            manaT.text = CardData.ManaCost.ToString();
        }
        else
            GetComponent<Animator>().SetTrigger("Show");
        
        
        pictureI.sprite = CardData.Picture;
        nameT.text = CardData.Name;
        descriptionT.text = CardData.Description;
        if ((CardData as Cardgame_CombatCard).Dmg != 0)
        {
            leftTitleT.text = "Damage";
            dmgT.text = (CardData as Cardgame_CombatCard).Dmg.ToString();
        }
        else
        {
            leftTitleT.text = "Block";
            dmgT.text = (CardData as Cardgame_CombatCard).Block.ToString();
        }

        if (CardData.CardType == Cardgame_CardType.Combat)
        {
            rangeT.text = CardData.Range.ToString();
            typeT.text = "Attack";
            
        }else if(CardData.CardType == Cardgame_CardType.Spell)
        {
            rangeT.text = "∞";
            typeT.text = "Spell";
        }
    }

    internal void Deactivate()
    {
        GetComponent<Animator>().SetBool("IsSelected", false);
        isActive = true;
        selected.SetActive(false);
    }

    internal void Activate()
    {
        isActive = false;
        GetComponent<Animator>().SetBool("IsSelected", true);
        selected.SetActive(true);
    }

    public void OnClick()
    {
        if(CardData.ManaCost <= controller.currentPlayer.CurrentMana)
            controller.ActivateCard(this);
    }

    public void EndShow()
    {
        gameObject.SetActive(false);
    }
}
