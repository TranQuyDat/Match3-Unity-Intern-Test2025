using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{    
    public int BoardSizeX = 7;
    public int BoardSizeY = 1;

    public int BottomCellSizeX = 6;

    public int MatchesMin = 3;

    public int LevelMoves = 16;

    public float LevelTime = 30f;

    public float TimeForHint = 5f;
}
