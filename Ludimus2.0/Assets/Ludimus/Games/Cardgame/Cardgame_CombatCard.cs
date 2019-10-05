using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cardgame/CombatCard", order = 1)]
public class Cardgame_CombatCard : Cardgame_Card
{
    public int Dmg;
    public int Block;
    public GameObject Ability;
    public string Message;
    public DamageType DamageType;
}

public enum DamageType
{
    AllInLine,
    OnlyOne,
    AllAround,
    Self
}
