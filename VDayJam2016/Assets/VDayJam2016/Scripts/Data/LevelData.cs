using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    [Multiline]
    public string mData;
    public Vector2i mPlayerCell;
    public Vector2i mGoalCell;
    public SignalType mGoalSignalType = SignalType.StartNextLevel;
    public bool mbGoalActiveAtStart = true;

    [System.Serializable]
    public class SpawnData
    {
        public Vector2i mCell;
        public float mYRotation = 0;

        public enum SpawnType
        {
            Special,
            Collectable,
            Enemy,
        }
        public SpawnType mSpawnType = SpawnType.Special;
        public GameObject mPrefab;
    }
    public SpawnData[] mSpawnDatas;
}