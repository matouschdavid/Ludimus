using UnityEngine;
public enum Cardgame_MovementType
{
    Straight,
    Diagonal
}

public enum Cardgame_CardType
{
    Movement,
    Combat
}



public class Cardgame_Card : ScriptableObject
{
    public string Name;
    public string Description;
    public int Range;
    public Sprite Picture;
    public Cardgame_CardType CardType;
}
