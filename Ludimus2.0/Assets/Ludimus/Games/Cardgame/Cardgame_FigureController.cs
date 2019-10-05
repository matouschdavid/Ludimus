using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cardgame_FigureController : MonoBehaviour
{
    public float DmgMul;
    public float SpeedMul;
    public float BlockMul;
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
    private int speed;

    public int Speed
    {
        get { return speed; }
        set {
            if(speedObj != null)
                speedObj.SetActive(true);
            speed = value;
            speedT.text = speed.ToString();
            if (speed == 0)
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
        int d = (int)(dmg - Block);
        if (d > 0)
        {
            Health -= d;
            Block = 0;
        }
        else
            Block -= dmg;
    }

    public Cardgame_MovementType MovementType;
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
    public GameObject hoverUI;

    private void Start()
    {
        Dmg = 0;
        Speed = 0;
        Block = 0;
        Health = health;
        animator = GetComponent<Animator>();
        
        Hover(false);
    }

    public void SetStats()
    {
        speedEff.text = (int)(SpeedMul * 100) + " %";
        dmgEff.text = (int)(DmgMul * 100) + " %";
        blockEff.text = (int)(BlockMul * 100) + " %";
    }

    public void Hover(bool state)
    {
        hoverUI.SetActive(state);
    }
    
    internal void Attack(Cardgame_FigureController figure)
    {
        figure.TakeDamage(Dmg);
    }

    public void AddCard(Cardgame_Card card, Cardgame_FieldController currentField, Cardgame_MovementType movementType)
    {
        CurrentCard = card;
        Cards.Add(card);
        if(card.CardType == Cardgame_CardType.Combat)
        {
            Dmg += (int)((card as Cardgame_CombatCard).Dmg * DmgMul);
            if((card as Cardgame_CombatCard).DamageType == DamageType.Self)
                Block += (int)((card as Cardgame_CombatCard).Block * BlockMul);
            player.Attack(this, currentField, movementType);
        }
        else if(card.CardType == Cardgame_CardType.Movement)
        {
            Speed += (int)(card.Range * SpeedMul);   
            MovementType = (card as Cardgame_MovementCard).MovementType;
            player.MoveWith(this, currentField);
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
                //player.controller.SetText("New Enemy spawned");
                player.controller.Players.Remove(player);
                Destroy(player.gameObject);
            }
        }
        Destroy(gameObject);
    }
    
}
