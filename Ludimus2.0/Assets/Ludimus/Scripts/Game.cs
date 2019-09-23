using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public string Gamename;
    public int MinPlayers;
    public int MaxPlayers;
    public int MinAge;
    public int MaxAge;
    public string StoragePath;

    public Game(string gamename, int minPlayers, int maxPlayers, int minAge, int maxAge, string storagePath)
    {
        Gamename = gamename;
        MinPlayers = minPlayers;
        MaxPlayers = maxPlayers;
        MinAge = minAge;
        MaxAge = maxAge;
        StoragePath = storagePath;
    }
}
