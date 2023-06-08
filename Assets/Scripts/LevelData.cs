using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public int ID;

    [Header("Level props")]
    public Color BackgroundColor;
    public SpriteRenderer LowerBackground;
    public SpriteRenderer UpperBackground;
    public SpriteRenderer Soil;
    public Wall Wall;

    [Header("Level Settings")]
    public float LevelSpeed;
    public float BackGroundSpeed;
    public float DistanceBetweenWalls;
    public float MaxVerticalDistanceBetweenGaps;
    public float GapSize;
}
