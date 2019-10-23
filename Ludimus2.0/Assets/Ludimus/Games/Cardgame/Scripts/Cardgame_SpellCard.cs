using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cardgame/Spellcard", order = 1)]
public class Cardgame_SpellCard : Cardgame_CombatCard
{
    public TargetType TargetType;
}

public enum TargetType
{
    Enemy,
    Self,
    All
}
