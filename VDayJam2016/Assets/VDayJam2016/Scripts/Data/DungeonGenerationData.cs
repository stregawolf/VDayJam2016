using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "DungeonGenerationData", menuName ="Data/DungeonGenerationData")]
public class DungeonGenerationData: ScriptableObject
{
    public int mNumCollectables = 0;
    public int mNumEnemies = 0;
    public GameObject[] mEnemyPrefabs;

    public DungeonEnvironmentGenerationData mEnvironmentData;
}
