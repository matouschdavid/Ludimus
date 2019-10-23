using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cardgame_FigureController : MonoBehaviour
{
    public int DmgPoints;
    public int DefensePoints;
    public int SpeedPoints;
    public int ManaPoints;
    public int health;

    public int Health
    {
        get { return health; }
        set {
            health = value;
            if (health <= 0)
            {
                animator.enabled = true;
                animator.SetTrigger("Die");
            }
            healthT.text = (health + block).ToString();
        }
    }

    private int dmg;

    public int Dmg
    {
        get { return dmg; }
        set {
            dmg = value;
            dmgT.text = dmg.ToString();
        }
    }
    private int block;

    public int Block
    {
        get { return block; }
        set {
            block = value;
            healthT.text = (health + block).ToString();
        }
    }
    private int range;

    public int Range
    {
        get { return range; }
        set {
            if(speedObj != null)
                speedObj.SetActive(true);
            range = value;
            speedT.text = range.ToString();
            if (range == 0)
                speedObj.SetActive(false);
        }
    }



    public Cardgame_FieldController GetField()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, ~player.controller.layerMask))
        {
            var res = hit.transform.gameObject.GetComponent<Cardgame_FieldController>();
            if (res != null)
                return res;
        }
        return null;
    }

    internal void TakeDamage(int dmg)
    {
        //Debug.Log(name + " takes " + dmg + " dmg");
        int d = (int)(dmg - Block);
        if (d > 0)
        {
            Health -= d;
            Block = 0;
        }
        else
            Block -= dmg;
    }
    
    public Cardgame_Player player;
    private List<Cardgame_Card> Cards = new List<Cardgame_Card>();
    public Cardgame_Card CurrentCard;
    public TextMeshProUGUI dmgT;
    public TextMeshProUGUI speedT;
    public TextMeshProUGUI healthT;
    public GameObject speedObj;
    public GameObject explosion;
    public Animator animator;


    public TextMeshProUGUI speedEff;
    public TextMeshProUGUI dmgEff;
    public TextMeshProUGUI blockEff;
    public TextMeshProUGUI manaEff;
    public GameObject hoverUI;

    private void Start()
    {
        Dmg = 0;
        Range = 0;
        Block = 0;
        Health = health;
        animator = GetComponent<Animator>();
        
        Hover(false);
    }

    public void SetStats()
    {
        speedEff.text = SpeedPoints.ToString();
        dmgEff.text = DmgPoints.ToString();
        blockEff.text = DefensePoints.ToString();
        manaEff.text = ManaPoints.ToString();
    }

    public void Hover(bool state)
    {
        hoverUI.SetActive(state);
    }
    
    internal void Attack(Cardgame_FigureController figure)
    {
        figure.TakeDamage(Dmg);
    }

    public void AddCard(Cardgame_Card card, Cardgame_FieldController currentField, bool enemy)
    {
        CurrentCard = card;
        Cards.Add(card);
        if(card.CardType == Cardgame_CardType.Combat)
        {
            Dmg += Convert.ToInt32((card as Cardgame_CombatCard).Dmg * (0.5f * DmgPoints + 1));
            if((card as Cardgame_CombatCard).DamageType == DamageType.Self)
                Block += Convert.ToInt32((card as Cardgame_CombatCard).Block * (0.5f * DefensePoints + 1));
            player.Attack(this, currentField, enemy);
        }
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        player.Figures.Remove(this);
        if (player.Figures.Count == 0)
        {
            if (player.isHuman)
            {
                player.controller.ShowDeathScreen();
            }
            else
            {
                player.controller.Players.Remove(player);
                Destroy(player.gameObject);
            }
        }
        Destroy(gameObject);
    }
    
}
