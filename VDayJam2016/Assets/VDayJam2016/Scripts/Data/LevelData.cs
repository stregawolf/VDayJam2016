using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    [Multiline]
    public string mData;
    public Vector2i mPlayerCell;
    public Vector2i mGoalCell;
    public SignalType mGoalSignalType = SignalType.StartNextLevel;
}