using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cardgame_CardController : MonoBehaviour
{
    public Cardgame_Card CardData;

    public Image image;
    public MeshRenderer selectedFrame;
    public Image typeI;
    public Sprite straight;
    public Sprite diagonal;
    public Material normalMat;
    public Material selectedMat;
    public Animator animator;
    public bool isActive = true;

    public GameObject hoverBg;
    public TextMeshProUGUI text;
    public TextMeshProUGUI description;
    public TextMeshProUGUI range;
    public TextMeshProUGUI dmg;
    public GameObject dmgBg;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    internal void SetUp(Cardgame_Card card)
    {
        hoverBg.SetActive(false);
        animator.SetTrigger("Grow");
        CardData = card;
        image.sprite = CardData.Picture;
        text.text = CardData.Name;
        description.text = CardData.Description;
        range.text = CardData.Range.ToString();

        if (CardData.CardType == Cardgame_CardType.Combat)
        {
            typeI.enabled = false;
            dmgBg.SetActive(true);
            dmg.text = (CardData as Cardgame_CombatCard).Dmg.ToString();
        }
        else
        {
            typeI.enabled = true;
            dmgBg.SetActive(false);
            var movementType = (card as Cardgame_MovementCard).MovementType;
            if (movementType == Cardgame_MovementType.Diagonal)
                typeI.sprite = diagonal;
            else
                typeI.sprite = straight;
        }
    }

    public void Hover(bool state)
    {
        hoverBg.SetActive(state);
    }

    internal void Deactivate()
    {
        selectedFrame.materials[2] = normalMat;
        selectedFrame.materials[2].color = normalMat.color;
    }

    internal void Activate()
    {
        Debug.Log("Activate " + gameObject.name);
        selectedFrame.materials[2] = selectedMat;
        selectedFrame.materials[2].color = selectedMat.color;
    }

    public void OnClick()
    {
        Debug.Log("Test");
        GameObject.Find("GameController").GetComponent<Cardgame_Controller>().ActivateCard(this);
    }
}
