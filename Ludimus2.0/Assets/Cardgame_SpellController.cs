using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Cardgame_SpellController : MonoBehaviour
{
    public Cardgame_CardController card;
    public bool InSpell = false;
    public IEnumerator UseCard(object p)
    {
        InSpell = true;
        Debug.Log("Here in spell");
        var param = p as SpellParams;

        card.gameObject.SetActive(true);
        card.SetUp(param.Card, false);
        yield return new WaitForSeconds(3);
        Type thisType = this.GetType();
        MethodInfo theMethod = thisType.GetMethod((param.Card as Cardgame_CombatCard).Message);
        yield return theMethod.Invoke(this, new object[] { param });
        InSpell = false;
        if (param.From != null)
            param.From.Dmg = 0;
    }




    
    public void Shield(SpellParams p)
    {
        foreach (var figure in p.To.Select(f => f.indicator.figureOnField))
        {
            figure.Block += (p.Card as Cardgame_SpellCard).Block;
        }
    }

    public void Strike(SpellParams p)
    {
        foreach (var figure in p.To.Select(f => f.indicator.figureOnField))
        {
            Instantiate(p.Card.Other, figure.GetField().transform.position, Quaternion.identity);
            figure.TakeDamage((p.Card as Cardgame_SpellCard).Dmg);
        }
    }

    public void Spike(SpellParams p)
    {
        foreach (var field in p.To)
        {
            var spikes = Instantiate(p.Card.Other as GameObject, new Vector3(field.transform.position.x, 3.974f, field.transform.position.z), Quaternion.identity);
            field.FieldDmg = (p.Card as Cardgame_CombatCard).Dmg;
            field.currentTimesDmg = 0;
            field.currentDmgObj = spikes;
        }

    }



    public IEnumerator Raise(SpellParams p)
    {
        foreach (var field in p.To)
        {
            Debug.Log("Raise");
            field.GetComponentInChildren<Animator>().SetTrigger("Raise");
            var target = field.GetPlayerOnField();

            if (target != null)
                p.From.Attack(target);
            yield return new WaitForSeconds(0.2f);
        }
        
    }

    public void Slam(SpellParams p)
    {
        var target = p.To[0].GetPlayerOnField();
        if (target == null)
            return;
        p.From.Attack(target);
    }

    public void Spin(SpellParams p)
    {
        foreach (var field in p.To)
        {
            var target = field.GetPlayerOnField();
            if (target != null)
            {
                p.From.Attack(target);
                p.From.Block += (p.Card as Cardgame_CombatCard).Block;
            }
        }
        
    }

    public IEnumerator Push(SpellParams p)
    {
        Debug.Log("In Push");
        foreach (var field in p.To)
        {
            var angle = p.From.player.GetAngle(field.transform.position, p.From.transform.position);
            Debug.Log(angle);
            var target = field.indicator.figureOnField;
            if(target != null)
            {
                Debug.Log("Figure on field");
                var to = target.player.GetFields(angle, 1, field).FirstOrDefault();
                if (to != null)
                {
                    Debug.Log("Not at border");
                    if(to.indicator.figureOnField != null)
                    {
                        Debug.Log("Another figure on field");
                        p.From.Attack(target);
                    } else { 
                        Debug.Log("Can and should move");
                        target.player.MoveWith(target, field, true);
                        yield return StartCoroutine(target.player.MoveFigure(to, false));
                    }
                }
                else
                {
                    Debug.Log("At border");
                    p.From.Attack(target);
                }
            }
        }
        
    }
}

public class SpellParams
{
    public Cardgame_FigureController From { get; set; }
    public Cardgame_FieldController[] To { get; set; }
    public Cardgame_CombatCard Card { get; set; }
}
