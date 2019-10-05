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

    public IEnumerator Turn(List<Cardgame_Card> hand, List<Cardgame_FigureController> otherFigures)
    {
        Figures = Figures.OrderBy(f => otherFigures.Min(of => GetMovesToPoint(of.GetField(), f.GetField(), Cardgame_MovementType.Straight))).ToList();
        foreach (var figure in Figures)
        {
            bool canAttack = false;
            bool endNextTurn = false;
            var list = otherFigures.Where(f => f != null).OrderBy(f => Vector3.Distance(f.transform.position, figure.transform.position)).ThenBy(f => f.Health + f.Block).ToList();
            foreach (var f in list)
            {
                bool skip = false;
                do
                {
                    if (f == null)
                    {
                        skip = true;
                    }
                    else
                    {
                        var res = IsInAttackRange(f, figure, hand);
                        canAttack = res.Item1;
                        if (res.Item1)
                        {
                            if (f != null)
                            {
                                Debug.Log("Attack");
                                var movementType = GetAngle(f.transform.position, figure.transform.position) % 90 == 0 ? Cardgame_MovementType.Straight : Cardgame_MovementType.Diagonal;
                                figure.AddCard(res.Item2, figure.GetField(), movementType);
                                hand.Remove(res.Item2);
                                yield return new WaitForSeconds(2);
                                if(f != null)
                                    AttackEnemy(f.GetField(), figure.GetField());
                            }
                        }
                        else
                        {
                            if (f == null)
                            {
                                skip = true;
                                break;
                            }
                            else
                            {
                                Debug.Log("Wants to move");
                                var movementCard = hand.FirstOrDefault(c => c.CardType == Cardgame_CardType.Movement) as Cardgame_MovementCard;
                                if (movementCard != null)
                                {
                                    figure.AddCard(movementCard, figure.GetField(), figure.MovementType);
                                    hand.Remove(movementCard);
                                    Debug.Log("Has movementcard");
                                    var rightFields = GetFields(0, (int)(movementCard.Range * figure.SpeedMul), figure.GetField()).Where(field => controller.FieldList.Where(ff => ff.indicator.Available).Contains(field)).ToList();
                                    var upFields = GetFields(90, (int)(movementCard.Range * figure.SpeedMul), figure.GetField()).Where(field => controller.FieldList.Where(ff => ff.indicator.Available).Contains(field)).ToList();
                                    var leftFields = GetFields(180, (int)(movementCard.Range * figure.SpeedMul), figure.GetField()).Where(field => controller.FieldList.Where(ff => ff.indicator.Available).Contains(field)).ToList();
                                    var downFields = GetFields(270, (int)(movementCard.Range * figure.SpeedMul), figure.GetField()).Where(field => controller.FieldList.Where(ff => ff.indicator.Available).Contains(field)).ToList();
                                    var tmp = rightFields.Concat(upFields).Concat(leftFields).Concat(downFields).ToList();

                                    var possibleMoves = tmp.OrderBy(field => GetMovesToPoint(field, f.GetField(), Cardgame_MovementType.Straight)).ToList();
                                    if (!hand.Exists(c => c.CardType == Cardgame_CardType.Combat))
                                    {
                                        endNextTurn = true;
                                        possibleMoves.Reverse();
                                    }

                                    if (possibleMoves.Count != 0)
                                    {
                                        Debug.Log("Found best move to " + possibleMoves[0].name);
                                        
                                        
                                        yield return new WaitForSeconds(2);
                                        MoveFigure(possibleMoves[0]);
                                        Debug.Log("Moved figure");
                                        if (endNextTurn)
                                        {
                                            controller.EndTurn();
                                            yield break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                } while ((hand.Exists(c => c.CardType == Cardgame_CardType.Movement) || canAttack) && !skip);
            }
            
        }
        controller.EndTurn();
    }

    private (bool, Cardgame_Card) IsInAttackRange(Cardgame_FigureController enemy, Cardgame_FigureController self, List<Cardgame_Card> hand)
    {
        if (enemy == null)
            return (false, null);
        var enemyField = enemy.GetField();
        var selfField = self.GetField();
        foreach (var card in hand)
        {
            int range = 0;
            if(card.CardType == Cardgame_CardType.Combat)
            {
                range = (card as Cardgame_CombatCard).Range;
                var fieldsToTarget = GetFields(GetAngle(enemyField.transform.position, self.transform.position), range, selfField);
                if (fieldsToTarget.Contains(enemyField))
                {
                    return (true, card);
                }
            }
            
        }
        return (false, null);
        
    }
}
