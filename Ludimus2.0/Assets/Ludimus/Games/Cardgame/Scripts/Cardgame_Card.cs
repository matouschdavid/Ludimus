using UnityEngine;
public enum Cardgame_CardType
{
    Combat,
    Spell
}

public class Cardgame_Card : ScriptableObject
{
    
    public string Name;
    public string Description;
    public int Range;
    public Sprite Picture;
    public Cardgame_CardType CardType;
    public GameObject Other;
    public int ManaCost;
}
