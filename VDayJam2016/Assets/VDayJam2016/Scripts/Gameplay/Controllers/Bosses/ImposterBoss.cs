using UnityEngine;
using System.Collections.Generic;

public class ImposterBoss : EnemyController {
    public float mRangedRadius = 6.0f;

    public float mActionRate = 5.0f;
    public float mMovementDelayTime = 1.0f;

    public float mSpreadAngle = 15.0f;
    public float mProjectileSpeed = 10.0f;

    public GameObject mSpawnVFXPrefab;
    public GameObject mMinionPrefab;
    public int mMaxNumMinions = 10;

    protected float mActionTimer = 0.0f;
    protected float mMovementDelayTimer = 0.0f;

    protected EnemyPlayer mEnemyPlayer;
    protected float mRangedAttackRadiusSquared;

    protected List<EnemyController> mMinions = new List<EnemyController>();
    protected Dictionary<GameObject, List<EnemyController>> mMinionPool = new Dictionary<GameObject, List<EnemyController>>();

    protected bool mbShouldChase = true;
    protected Vector3 mStartingPos;

    public enum Phase
    {
        Melee,
        Ranged,
        Clones,
        Laser,
    }
    public Phase mCurrentPhase = Phase.Melee;

    protected override void Awake()
    {
        base.Awake();

        mEnemyPlayer = mEnemy as EnemyPlayer;
        mRangedAttackRadiusSquared = mRangedRadius * mRangedRadius;
        mActionTimer = mActionRate;
    }

    public override void Init()
    {
        base.Init();
        mStartingPos = transform.position;
    }

    public override void UpdateBehavior(BasePlayer player)
    {
        if (mEnemy.Hp <= 0 && mMinions.Count > 0)
        {
            for (int i = 0, n = mMinions.Count; i < n; ++i)
            {
                if (mMinions[i].mEnemy.Hp > 0)
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

        UpdateMinions(player);

        mActionTimer -= Time.deltaTime;
        mMovementDelayTimer -= Time.deltaTime;

        if(mActionTimer <= 0.0f)
        {
            switch(mCurrentPhase)
            {
                case Phase.Melee:
                    HandleMeleePhaseAction(player, sqrdistToPlayer, dirToPlayer);
                    break;
                case Phase.Ranged:
                    HandleRangedPhaseAction(player, sqrdistToPlayer, dirToPlayer);
                    break;
                case Phase.Clones:
                    HandleClonesPhaseAction(player, sqrdistToPlayer, dirToPlayer);
                    break;
                case Phase.Laser:
                    HandleLaserPhaseAction(player, sqrdistToPlayer, dirToPlayer);
                    break;
            }
        }
        else if(mMovementDelayTimer <= 0.0f)
        {
            float maxDist = 0.0f;
            float minDist = 0.0f;
            switch (mCurrentPhase)
            {
                case Phase.Melee:
                    if (mEnemy.Hp < mEnemy.mMaxHp * 0.75f)
                    {
                        mEnemyPlayer.SetEquipedWeaponType(EnemyPlayer.EquipedWeaponType.Ranged);
                        mCurrentPhase = Phase.Ranged;
                        return;
                    }
                    maxDist = mAttackRadiusSquared * 3f;
                    minDist = player.mSwingRadius * player.mSwingRadius + mAttackRadiusSquared * 2;
                    HandleAvoidMeleeMovement(player, sqrdistToPlayer, dirToPlayer, maxDist, minDist);
                    break;
                case Phase.Ranged:
                    if (mEnemy.Hp < mEnemy.mMaxHp * 0.5f)
                    {
                        mEnemyPlayer.SetEquipedWeaponType(EnemyPlayer.EquipedWeaponType.Melee);
                        mCurrentPhase = Phase.Clones;
                        return;
                    }
                    maxDist = mRangedAttackRadiusSquared*0.75f;
                    minDist = player.mSwingRadius * player.mSwingRadius + mRangedAttackRadiusSquared*0.5f;
                    HandleAvoidMeleeMovement(player, sqrdistToPlayer, dirToPlayer, maxDist, minDist);
                    break;
                case Phase.Clones:
                    if (mEnemy.Hp < mEnemy.mMaxHp * 0.25f)
                    {
                        mCurrentPhase = Phase.Laser;
                        return;
                    }
                    maxDist = mAttackRadiusSquared * 3f;
                    minDist = player.mSwingRadius * player.mSwingRadius + mAttackRadiusSquared * 2;
                    HandleAvoidMeleeMovement(player, sqrdistToPlayer, dirToPlayer, maxDist, minDist);
                    break;
                case Phase.Laser:
                    maxDist = mAttackRadiusSquared * 3f;
                    minDist = player.mSwingRadius * player.mSwingRadius + mAttackRadiusSquared * 2;
                    HandleAvoidMeleeMovement(player, sqrdistToPlayer, dirToPlayer, maxDist, minDist);
                    break;
            }
            
        }
    }

    protected void HandleAvoidMeleeMovement(BasePlayer player, float sqrdistToPlayer, Vector3 dirToPlayer, float maxDist, float minDist)
    {
        mEnemy.LookAtPoint(player.transform.position);
        if (sqrdistToPlayer > maxDist)
        {
            mbShouldChase = true;
        }
        else if (sqrdistToPlayer < minDist)
        {
            mbShouldChase = false;
        }

        if (mbShouldChase)
        {
            mEnemy.MoveDir(dirToPlayer);
        }
        else
        {
            mEnemy.MoveDir(-dirToPlayer);
        }
    }

    protected int mAttackCount = 0;
    protected Vector3 mLastAttackDir;
    protected bool mAttackStarted = false;
    public const float mAttackDelayTime = 0.25f;

    protected void HandleMeleePhaseAction(BasePlayer player, float sqrdistToPlayer, Vector3 dirToPlayer)
    {
        if(mAttackStarted)
        {
            mAttackDelayTimer -= Time.deltaTime;
            if (mAttackDelayTimer > 0.0f)
            {
                mEnemy.LookDir(mLastAttackDir);
                mEnemy.MoveDir(mLastAttackDir, 2f);
            }
            else
            {
                mEnemy.LookDir(mLastAttackDir);
                mEnemy.Attack(player.transform.position);
                mAttackCount++;
                mAttackDelayTimer = mAttackDelayTime;
                if (mAttackCount >= 3)
                {
                    mAttackStarted = false;
                    mActionTimer = mActionRate;
                    mMovementDelayTimer = mMovementDelayTime;
                }
            }
        }
        else
        {
            if (sqrdistToPlayer > mAttackRadiusSquared * 2.5f)
            {
                mEnemy.LookAtPoint(player.transform.position);
                mEnemy.MoveDir(dirToPlayer, 1.75f);
            }
            else
            {
                mEnemyPlayer.mSwingMidwayCallback = mEnemyPlayer.OnMeleeSwingMidway;
                mAttackCount = 0;
                mAttackStarted = true;
                mLastAttackDir = dirToPlayer;
                mAttackDelayTimer = mAttackDelayTime;
            }
        }
    }

    public const float mRangedAttackDelayTime = 0.75f;
    protected void HandleRangedPhaseAction(BasePlayer player, float sqrdistToPlayer, Vector3 dirToPlayer)
    {
        if (mAttackStarted)
        {
            mAttackDelayTimer -= Time.deltaTime;
            if(mAttackDelayTimer > 0.0f)
            {
                mEnemy.LookDir(dirToPlayer);
            }
            else
            {
                mAttackCount++;
                mEnemy.LookDir(mLastAttackDir);
                mEnemy.Attack(player.transform.position);
                mAttackDelayTimer = mRangedAttackDelayTime;
                if (mAttackCount >= 3)
                {
                    mAttackStarted = false;
                    mActionTimer = mActionRate;
                    mMovementDelayTimer = mMovementDelayTime * 2;
                }
            }
        }
        else
        {
            if (sqrdistToPlayer > mRangedAttackRadiusSquared)
            {
                mEnemy.LookAtPoint(player.transform.position);
                mEnemy.MoveDir(dirToPlayer, 1.75f);
            }
            else
            {
                mEnemy.Stop();
                mEnemyPlayer.mSwingMidwayCallback = OnRangedPhaseMidSwing;
                mAttackCount = 0;
                mAttackStarted = true;
                mLastAttackDir = dirToPlayer;
                mAttackDelayTimer = mAttackDelayTime;
            }
        }
    }

    protected void OnRangedPhaseMidSwing()
    {
        FireProjectiles(mEnemyPlayer.mProjectilePrefab, mAttackCount + 2, mSpreadAngle, mProjectileSpeed);
    }

    protected bool mbSpawnedClonesLastTime = false;
    protected void HandleClonesPhaseAction(BasePlayer player, float sqrdistToPlayer, Vector3 dirToPlayer)
    {
        if (mAttackStarted)
        {
            mAttackDelayTimer -= Time.deltaTime;
            if (mAttackDelayTimer > 0.0f)
            {
                mEnemy.LookDir(mLastAttackDir);
                if (mMovementDelayTimer <= 0.0f)
                {
                    mEnemy.MoveDir(mLastAttackDir, 2f);
                }
            }
            else
            {
                mEnemy.LookDir(mLastAttackDir);
                mEnemy.Attack(player.transform.position);
                mAttackCount++;
                mAttackDelayTimer = mAttackDelayTime;
                if (mAttackCount >= 3)
                {
                    mAttackStarted = false;
                    mActionTimer = mActionRate;
                    mMovementDelayTimer = mMovementDelayTime;
                }
            }
        }
        else
        {
            if (sqrdistToPlayer > mAttackRadiusSquared * 2.5f)
            {
                mEnemy.LookAtPoint(player.transform.position);
                mEnemy.MoveDir(dirToPlayer, 1.75f);
            }
            else
            {
                if(mbSpawnedClonesLastTime)
                {
                    mEnemyPlayer.mSwingMidwayCallback = mEnemyPlayer.OnMeleeSwingMidway;
                    mAttackCount = 0;
                    mAttackStarted = true;
                    mLastAttackDir = dirToPlayer;
                    mAttackDelayTimer = mAttackDelayTime;
                    mbSpawnedClonesLastTime = false;
                }
                else
                {
                    mEnemy.Stop();
                    Instantiate(mSpawnVFXPrefab, transform.position, Quaternion.identity);
                    EnemyController[] minions = new EnemyController[3];
                    minions[0] = this;
                    minions[1] = SpawnMinion(mMinionPrefab, transform.position + transform.right * 3, player);
                    minions[2] = SpawnMinion(mMinionPrefab, transform.position - transform.right * 3, player);

                    EnemyController minionToSwitchWith = minions[Random.Range(0, 3)];
                    if (minionToSwitchWith != null)
                    {
                        Vector3 switchPos = minionToSwitchWith.transform.position;
                        minionToSwitchWith.mEnemy.TeleportTo(transform.position);
                        mEnemy.TeleportTo(switchPos);

                        dirToPlayer = player.transform.position - transform.position;
                    }

                    mEnemyPlayer.mSwingMidwayCallback = mEnemyPlayer.OnMeleeSwingMidway;
                    mAttackCount = 0;
                    mAttackStarted = true;
                    mLastAttackDir = dirToPlayer;
                    mMovementDelayTimer = mMovementDelayTime * 0.5f;
                    mAttackDelayTimer = mAttackDelayTime;
                    mbSpawnedClonesLastTime = true;
                }
                
            }
        }
    }

    public GameObject mLaserPrefab;
    public float mLaserFireTime = 3.0f;
    public float mLaserRotationRate = 90.0f;
    public float mLaserProjectileInterval = 0.5f;
    public float mDelayAfterLaser = 4.0f;
    protected float mLaserProjectileInteralTimer = 0.0f;
    protected float mLaserFireTimer = 0.0f;
    protected bool mbLaserStarted = false;
    protected bool mbLaseredLastTime = false;
    protected void HandleLaserPhaseAction(BasePlayer player, float sqrdistToPlayer, Vector3 dirToPlayer)
    {
        if(mbLaserStarted)
        {
            Vector3 dirToStartPos = mStartingPos - transform.position;
            float sqrDist = dirToStartPos.sqrMagnitude;
            if(sqrDist > 0.1f)
            {
                mEnemy.LookAtPoint(mStartingPos);
                mEnemy.MoveDir(dirToStartPos.normalized, 2.0f);
            }
            else
            {
                // spin and fire laser 
                mEnemy.Stop();
                mEnemy.transform.Rotate(Vector3.up * mLaserRotationRate * Time.deltaTime);

                mLaserProjectileInteralTimer -= Time.deltaTime;
                if(mLaserProjectileInteralTimer <= 0.0)
                {
                    FireProjectiles(mLaserPrefab, 6, 60.0f, mProjectileSpeed*0.5f);
                    mLaserProjectileInteralTimer = mLaserProjectileInterval;
                }


                mLaserFireTimer -= Time.deltaTime;
                if(mLaserFireTimer <= 0.0f)
                {
                    mbLaserStarted = false;
                    mMovementDelayTimer = mDelayAfterLaser;
                    mActionTimer = mMovementDelayTimer;
                }
            }
        }
        else if (mAttackStarted)
        {
            mAttackDelayTimer -= Time.deltaTime;
            if (mAttackDelayTimer > 0.0f)
            {
                mEnemy.LookDir(mLastAttackDir);
                if (mMovementDelayTimer <= 0.0f)
                {
                    mEnemy.MoveDir(mLastAttackDir, 2f);
                }
            }
            else
            {
                mEnemy.LookDir(mLastAttackDir);
                mEnemy.Attack(player.transform.position);
                mAttackCount++;
                mAttackDelayTimer = mAttackDelayTime;
                if (mAttackCount >= 3)
                {
                    mAttackStarted = false;
                    mActionTimer = mActionRate;
                    mMovementDelayTimer = mMovementDelayTime;
                }
            }
        }
        else
        {
            if (sqrdistToPlayer > mAttackRadiusSquared * 2.5f)
            {
                mEnemy.LookAtPoint(player.transform.position);
                mEnemy.MoveDir(dirToPlayer, 1.75f);
            }
            else
            {
                if (mbLaseredLastTime)
                {
                    mEnemy.Stop();
                    Instantiate(mSpawnVFXPrefab, transform.position, Quaternion.identity);
                    EnemyController[] minions = new EnemyController[3];
                    minions[0] = this;
                    minions[1] = SpawnMinion(mMinionPrefab, transform.position + transform.right * 3, player);
                    minions[2] = SpawnMinion(mMinionPrefab, transform.position - transform.right * 3, player);

                    EnemyController minionToSwitchWith = minions[Random.Range(0, 3)];
                    if (minionToSwitchWith != null)
                    {
                        Vector3 switchPos = minionToSwitchWith.transform.position;
                        minionToSwitchWith.mEnemy.TeleportTo(transform.position);
                        mEnemy.TeleportTo(switchPos);

                        dirToPlayer = player.transform.position - transform.position;
                    }

                    mEnemyPlayer.mSwingMidwayCallback = mEnemyPlayer.OnMeleeSwingMidway;
                    mAttackCount = 0;
                    mAttackStarted = true;
                    mLastAttackDir = dirToPlayer;
                    mMovementDelayTimer = mMovementDelayTime * 0.5f;
                    mAttackDelayTimer = mAttackDelayTime;
                    mbLaseredLastTime = false;
                }
                else
                {
                    mbLaserStarted = true;
                    mbLaseredLastTime = true;
                    mLaserFireTimer = mLaserFireTime;
                }

            }
        }
    }

    protected void FireProjectiles(GameObject prefab, int numProjectiles, float spreadAngle, float speed)
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_player_atk_ranged);

        Vector3 startPos = transform.position + transform.up * 0.5f;
        Vector3 rotation = transform.eulerAngles;
        rotation.y -= numProjectiles / 2.0f * spreadAngle;
        for (int i = 0; i < numProjectiles; ++i)
        {
            Vector3 dir = Quaternion.Euler(rotation) * Vector3.forward;
            mEnemy.FireProjectile(mEnemy, prefab, startPos + dir, dir, speed, Quaternion.identity);

            rotation.y += spreadAngle;
        }
    }

    public EnemyController SpawnMinion(GameObject minionPrefab, Vector3 position, BasePlayer player)
    {
        return SpawnMinion(minionPrefab, position, Quaternion.LookRotation((player.transform.position - position).normalized));
    }

    public EnemyController SpawnMinion(GameObject minionPrefab, Vector3 position, Quaternion rotation)
    {
        if (GameManager.Instance.mDungeon.GetCell(position).mTileType != DungeonCell.TileType.Ground)
        {
            return null;
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
                minionPool[i].Init();
                existingMinion.TeleportTo(position);
                existingMinion.transform.rotation = rotation;
                existingMinion.Revive();
                return minionPool[i];
            }
        }

        if (minionPool.Count < mMaxNumMinions)
        {
            Instantiate(mSpawnVFXPrefab, position, Quaternion.identity);
            GameObject minionObj = GameManager.Instance.SpawnPrefab(minionPrefab, position, rotation);
            EnemyController minion = minionObj.GetComponent<EnemyController>();
            minion.Init();
            minionPool.Add(minion);
            mMinions.Add(minion);
            return minion;
        }
        return null;
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
        for (int i = 0, n = mMinions.Count; i < n; ++i)
        {
            Destroy(mMinions[i].gameObject);
        }
    }
}
