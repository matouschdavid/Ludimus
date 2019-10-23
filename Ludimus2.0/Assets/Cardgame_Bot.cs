using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cardgame_Bot : Cardgame_Player
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Turn(List<Cardgame_FigureController> otherFigures)
    {
        CurrentMana = Figures.Count;
        playerUI.UpdateUI();
        bool cantAttack = false;
        bool cantMove = false;
        var lastFigures = new List<Cardgame_FigureController>();
        var tmp = Figures;
        var combinations = GetAllCombos<Cardgame_Card>(Hand.Where(c => c.ManaCost <= CurrentMana).ToList()).Where(c => c.Sum(card => card.ManaCost) <= CurrentMana).OrderByDescending(c => c.Sum(card => card.ManaCost)).ThenByDescending(c => c.Sum(card => (card as Cardgame_CombatCard).Dmg)).ThenByDescending(c => c.Count).ToList();
        var hand = combinations.FirstOrDefault();
        hand.ForEach(c => Debug.Log(c));
        if (hand != null)
        {
            do
            {
                tmp = Figures.Where(figure => !lastFigures.Contains(figure)).ToList();
                tmp = tmp.OrderBy(f => otherFigures.Where(of => of != null).Min(of => GetMovesToPoint(of.GetField(), f.GetField(), false))).ToList();
                Debug.Log(tmp.Count);
                var closestFigure = tmp.FirstOrDefault();
                Debug.Log(closestFigure);
                Cardgame_FigureController closestEnemy = null;
                otherFigures = otherFigures.Where(f => f != null).OrderBy(f => Vector3.Distance(f.transform.position, closestFigure.transform.position)).ThenBy(f => f.Health + f.Block).ToList();
                closestEnemy = otherFigures.FirstOrDefault();
                if (closestEnemy == null || closestFigure == null)
                {
                    controller.EndTurn();
                    yield break;
                }
                (bool, Cardgame_Card, Cardgame_FigureController) result;

                do
                {
                    controller.FieldList.ForEach(f => f.indicator.ResetIndicator());
                    result = CanUseSpell(otherFigures.ToArray(), closestFigure, hand);
                    if (result.Item1)
                    {
                        var spell = result.Item2 as Cardgame_SpellCard;
                        Cardgame_FieldController[] fields = null;

                        Func<Cardgame_FieldController, bool> predicate = f => f.indicator.figureOnField != null && f.indicator.figureOnField.player == this;
                        if (spell.TargetType == TargetType.Enemy)
                            predicate = f => f.indicator.figureOnField != null && f.indicator.figureOnField.player != this;
                        else if (spell.TargetType == TargetType.All)
                            predicate = f => true;
                        controller.FieldList.Where(predicate).ToList().ForEach(field =>
                        {
                            field.indicator.ActivateIndicator(true);
                        });
                        yield return new WaitForSeconds(2);

                        if (spell.DamageType == DamageType.Every)
                        {
                            if (spell.TargetType == TargetType.Self)
                            {
                                fields = Figures.Select(f => f.GetField()).ToArray();
                            }
                            else if (spell.TargetType == TargetType.Enemy)
                            {
                                fields = otherFigures.Select(f => f.GetField()).ToArray();
                            }
                        }
                        else
                        {
                            fields = new Cardgame_FieldController[] { result.Item3.GetField() };
                        }
                        yield return StartCoroutine(controller.DoDmgTo(fields, spell.Message, null, spell));
                        hand.Remove(spell);
                    }
                } while (result.Item1);
                var res = IsInAttackRange(closestEnemy, closestFigure,hand);

                var enemyField = closestEnemy.GetField();
                var ownField = closestFigure.GetField();
                cantAttack = !res.Item1;
                if (res.Item1)
                {
                    if (closestEnemy != null)
                    {
                        Debug.Log("Attack");
                        closestFigure.AddCard(res.Item2, ownField, false);
                        hand.Remove(res.Item2);
                        yield return new WaitForSeconds(2);
                        if (closestEnemy != null)
                        {
                            yield return StartCoroutine(AttackEnemy(enemyField, ownField));
                        }
                    }
                    else
                    {
                        Debug.Log("No enemy to attack");
                    }
                }
                else
                {
                    if (closestEnemy != null)
                    {
                        Debug.Log("Wants to move");

                        if (closestFigure.Range > 0)
                        {
                            while (controller.spellController.InSpell) ;
                            var range = closestFigure.Range;
                            MoveWith(closestFigure, ownField, true);
                            var rightFields = GetFields(0, range, ownField).Where(field => controller.FieldList.Where(ff => ff.indicator.Available).Contains(field)).ToList();
                            var upFields = GetFields(90, range, ownField).Where(field => controller.FieldList.Where(ff => ff.indicator.Available).Contains(field)).ToList();
                            var leftFields = GetFields(180, range, ownField).Where(field => controller.FieldList.Where(ff => ff.indicator.Available).Contains(field)).ToList();
                            var downFields = GetFields(270, range, ownField).Where(field => controller.FieldList.Where(ff => ff.indicator.Available).Contains(field)).ToList();
                            var temp = rightFields.Concat(upFields).Concat(leftFields).Concat(downFields).ToList();

                            var possibleMoves = temp.OrderBy(field => GetMovesToPoint(field, enemyField, false)).ToList();
                            if (!hand.Exists(c => c.CardType == Cardgame_CardType.Combat))
                            {
                                possibleMoves.Reverse();
                            }

                            if (possibleMoves.Count != 0)
                            {
                                Debug.Log("Found best move to " + possibleMoves[0].name);


                                yield return new WaitForSeconds(2);
                                yield return StartCoroutine(MoveFigure(possibleMoves[0], true));
                                Debug.Log("Moved figure");
                            }
                            else
                                cantMove = true;
                        }
                        else
                        {
                            cantMove = true;
                        }
                    }
                    else
                    {
                        Debug.Log("No enemy to move to");
                    }
                }

                cantAttack = !IsInAttackRange(closestEnemy, closestFigure, hand).Item1;
                if (cantAttack && cantMove)
                    lastFigures.Add(closestFigure);

                Debug.Log(lastFigures.Count + " " + Figures.Count);
            } while (lastFigures.Count != Figures.Count);
        }
        Hand = hand;
        controller.EndTurn();
    }

    private (bool, Cardgame_Card, Cardgame_FigureController) CanUseSpell(Cardgame_FigureController[] enemies, Cardgame_FigureController closestFigure, List<Cardgame_Card> hand)
    {
        
        foreach (var card in hand)
        {
            Debug.Log("Check: " + card);
            if(card.CardType == Cardgame_CardType.Spell)
            {
                var spell = card as Cardgame_SpellCard;
                if(spell.TargetType == TargetType.Self)
                {
                    if(spell.DamageType == DamageType.OnlyOne)
                    {
                        return (true, card, closestFigure);
                    }else if(spell.DamageType == DamageType.Every)
                    {
                        return (true, card, closestFigure);
                    }
                }else if(spell.TargetType == TargetType.Enemy)
                {
                    if (spell.DamageType == DamageType.OnlyOne)
                    {
                        return (true, card, enemies.Where(e => e != null).OrderBy(e => e.Health).FirstOrDefault());
                    }
                    else if (spell.DamageType == DamageType.Every)
                    {
                        return (true, card, enemies.FirstOrDefault());
                    }
                }else if(spell.TargetType == TargetType.All)
                {
                    if(spell.DamageType == DamageType.OnlyOne)
                    {
                        return (true, card, enemies.FirstOrDefault());
                    }
                }
            }
        }
        return (false, null, null);
    }

    private (bool, Cardgame_Card) IsInAttackRange(Cardgame_FigureController enemy, Cardgame_FigureController self, List<Cardgame_Card> hand)
    {
        if (enemy == null)
            return (false, null);
        var enemyField = enemy.GetField();
        var selfField = self.GetField();

        foreach (var card in hand)
        {
            int range = card.Range;
            if(card.CardType == Cardgame_CardType.Combat)
            {
                Debug.Log("Check card " + card.Name);
                var fieldsToTarget = GetFields(GetAngle(enemyField.transform.position, self.transform.position), range, selfField);
                if (fieldsToTarget.Contains(enemyField))
                {
                    Debug.Log("Uses card");
                    return (true, card);
                }
                else
                {
                    Debug.Log("Cant use card");
                }
            }
            
        }
        return (false, null);
        
    }

    public List<List<T>> GetAllCombos<T>(List<T> list)
    {
        List<List<T>> result = new List<List<T>>();
        // head
        result.Add(new List<T>());
        result.Last().Add(list[0]);
        if (list.Count == 1)
            return result;
        // tail
        List<List<T>> tailCombos = GetAllCombos(list.Skip(1).ToList());
        tailCombos.ForEach(combo =>
        {
            result.Add(new List<T>(combo));
            combo.Add(list[0]);
            result.Add(new List<T>(combo));
        });
        return result;
    }
}
