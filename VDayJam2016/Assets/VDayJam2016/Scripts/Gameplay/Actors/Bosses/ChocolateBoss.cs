using UnityEngine;
using System.Collections.Generic;

public class ChocolateBoss : EnemyController
{
    public GameObject mSpawnVFXPrefab;
    public GameObject[] mPhase1MinionPrefabs;
    public GameObject[] mPhase2MinionPrefabs;
    public GameObject[] mPhase3MinionPrefabs;
    public float mSpreadAngle = 15.0f;
    public float mSpawnRate = 5.0f;
    public float mMovementDelayTime = 1.0f;
    public int mMaxNumMinions = 25;

    public Color mDamagedColor = Color.yellow;
    public Color mNearDeathColor = Color.red;

    protected float mSpawnTimer = 0.0f;
    protected float mMovementDelayTimer = 0.0f;

    protected List<EnemyController> mMinions = new List<EnemyController>();
    protected Dictionary<GameObject, List<EnemyController>> mMinionPool = new Dictionary<GameObject, List<EnemyController>>();

    protected override void Awake()
    {
        base.Awake();
        mSpawnTimer = mSpawnRate;
    }

    public override void UpdateBehavior(BasePlayer player)
    {
        if(mEnemy.Hp <= 0 && mMinions.Count > 0)
        {
            for (int i = 0, n = mMinions.Count; i < n; ++i)
            {
                if(mMinions[i].mEnemy.Hp > 0)
                {
                    mMinions[i].mEnemy.TakeDamage(100);
                }
            }
        }

        base.UpdateBehavior(player);
    }

    protected override void HandleBossBehaviour(BasePlayer player)
    {
        base.HandleBossBehaviour(player);

        Vector3 dirToPlayer = player.transform.position - transform.position;
        float sqrdistToPlayer = dirToPlayer.sqrMagnitude;

        if (mEnemy.Hp < mEnemy.mMaxHp * 0.175f)
        {
            mEnemy.SetColor(mNearDeathColor);
        }
        else if (mEnemy.Hp < mEnemy.mMaxHp * 0.66f)
        {
            mEnemy.SetColor(mDamagedColor);
        }

        UpdateMinions(player);

        mSpawnTimer -= Time.deltaTime;
        mMovementDelayTimer -= Time.deltaTime;
        if (mSpawnTimer <= 0.0f)
        {
            mEnemy.Stop();
            if (mEnemy.Hp > mEnemy.mMaxHp * 0.66f)
            {
                mSpawnTimer = mSpawnRate;
                SpawnMinion(PickRandomPrefab(mPhase1MinionPrefabs), transform.position + transform.right * 2, player);
                SpawnMinion(PickRandomPrefab(mPhase1MinionPrefabs), transform.position - transform.right * 2, player);
            }
            else if (mEnemy.Hp > mEnemy.mMaxHp * 0.175f)
            {
                mSpawnTimer = mSpawnRate * 0.75f;
                SpawnMinion(PickRandomPrefab(mPhase2MinionPrefabs), transform.position + transform.right * 2, player);
                SpawnMinion(PickRandomPrefab(mPhase2MinionPrefabs), transform.position - transform.right * 2, player);
                SpawnMinion(PickRandomPrefab(mPhase2MinionPrefabs), transform.position + transform.right * 4 + transform.forward * 2, player);
                SpawnMinion(PickRandomPrefab(mPhase2MinionPrefabs), transform.position - transform.right * 4 + transform.forward * 2, player);
            }
            else
            {
                mSpawnTimer = mSpawnRate * 0.5f;
                for(int i = 0; i < 5; ++i)
                {
                    Vector3 pos;
                    if(GetRandomSpawnPos(player, out pos))
                    {
                        SpawnMinion(PickRandomPrefab(mPhase3MinionPrefabs), pos, player);
                    }
                }

            }
            mMovementDelayTimer = mMovementDelayTime;
        }
        else if (sqrdistToPlayer <= mVisionRadiusSquared && mMovementDelayTimer <= 0.0f)
        {
            dirToPlayer.Normalize();
            mEnemy.MoveDir(dirToPlayer);
        }
    }

    public bool GetRandomSpawnPos(BasePlayer player, out Vector3 pos)
    {
        pos = Vector3.zero;
        BaseGrid<DungeonCell> dungeonGrid = GameManager.Instance.mDungeon.Grid;
        int attempts = 0;
        while(attempts < 50)
        {
            pos.Set(Random.Range(1, dungeonGrid.Width), 0, Random.Range(1, dungeonGrid.Height));
            if(GameManager.Instance.mDungeon.GetCell(pos).mTileType == DungeonCell.TileType.Ground 
                && Vector3.Distance(pos, player.transform.position) > 3)
            {
                return true;
            }
            attempts++;
        }
        return false;
    }

    public GameObject PickRandomPrefab(GameObject[] prefabs)
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    public void SpawnMinion(GameObject minionPrefab, Vector3 position, BasePlayer player)
    {
        SpawnMinion(minionPrefab, position, Quaternion.LookRotation((player.transform.position - position).normalized));
    }

    public void SpawnMinion(GameObject minionPrefab, Vector3 position, Quaternion rotation)
    {
        if(GameManager.Instance.mDungeon.GetCell(position).mTileType != DungeonCell.TileType.Ground)
        {
            return;
        }

        // try to reuse an existing dead minion
        List<EnemyController> minionPool;
        if (!mMinionPool.TryGetValue(minionPrefab, out minionPool) || minionPool == null)
        {
            minionPool = new List<EnemyController>();
            mMinionPool[minionPrefab] = minionPool;
        }

        for (int i = 0, n = minionPool.Count; i < n; ++i)
        {
            BaseEnemy existingMinion = minionPool[i].mEnemy;
            if (existingMinion.Hp <= 0 && !existingMinion.IsFlickering)
            {
                Instantiate(mSpawnVFXPrefab, position, Quaternion.identity);
                existingMinion.TeleportTo(position);
                existingMinion.transform.rotation = rotation;
                existingMinion.Revive();
                return;
            }
        }

        if(minionPool.Count < mMaxNumMinions)
        {
            Instantiate(mSpawnVFXPrefab, position, Quaternion.identity);
            GameObject minionObj = GameManager.Instance.SpawnPrefab(minionPrefab, position, rotation);
            EnemyController minion = minionObj.GetComponent<EnemyController>();
            minion.Init();
            minionPool.Add(minion);
            mMinions.Add(minion);
        }
    }

    protected void UpdateMinions(BasePlayer player)
    {
        for (int i = 0, n = mMinions.Count; i < n; ++i)
        {
            mMinions[i].UpdateBehavior(player);
        }
    }

    protected void OnDestroy()
    {
        for(int i = 0, n = mMinions.Count; i < n; ++i)
        {
            Destroy(mMinions[i].gameObject);
        }
    }
}
