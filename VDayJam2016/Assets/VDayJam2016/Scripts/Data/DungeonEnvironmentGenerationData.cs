using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DungeonEnvironmentGenerationData", menuName = "Data/DungeonEnvironmentGenerationData")]
public class DungeonEnvironmentGenerationData : ScriptableObject
{
    [Header("Total Size")]
    public Vector2i mTotalDimensions = new Vector2i(51, 51);

    [Header("Initial Room")]
    public bool mRandomizeInitialRoomPosition = true;
    public Vector2i mInitialRoomPosition = new Vector2i(25, 3);
    public Vector2i mInitialRoomSize = new Vector2i(3, 3);

    [Header("Generation Data")]
    public bool mUseSeed = false;
    public int mSeed = 0;
    public Vector2i mMinRoomDimensions = new Vector2i(3, 3);
    public Vector2i mMaxRoomDimensions = new Vector2i(11, 11);

    public int mMaxCorridorLength = 4;

    
}
