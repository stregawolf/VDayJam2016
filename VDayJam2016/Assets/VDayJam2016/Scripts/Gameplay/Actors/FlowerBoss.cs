using UnityEngine;
using System.Collections;

public class FlowerBoss : EnemyController {
    public GameObject mFlowerProjectilePrefab;
    public float mSpreadAngle = 15.0f;
    public float mProjectileSpeed = 10.0f;
    public float mFireRate = 5.0f;
    public float mMovementDelayTime = 1.0f;

    protected float mFireTimer = 0.0f;
    protected float mMovementDelayTimer = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        mFireTimer = mFireRate;
    }

    protected override void HandleBossBehaviour(BasePlayer player)
    {
        base.HandleBossBehaviour(player);

        Vector3 dirToPlayer = player.transform.position - transform.position;
        float sqrdistToPlayer = dirToPlayer.sqrMagnitude;

        mFireTimer -= Time.deltaTime;
        mMovementDelayTimer -= Time.deltaTime;
        if (sqrdistToPlayer <= mAttackRadiusSquared && mFireTimer <= 0.0f)
        {
            mEnemy.Stop();
            if (mEnemy.Hp > mEnemy.mMaxHp * 0.66f)
            {
                mFireTimer = mFireRate;
                FireProjectiles(1);
            }
            else if (mEnemy.Hp > mEnemy.mMaxHp * 0.33f)
            {
                mFireTimer = mFireRate * 0.75f;
                FireProjectiles(3);
            }
            else
            {
                mFireTimer = mFireRate * 0.66f;
                FireProjectiles(5);
            }
            mMovementDelayTimer = mMovementDelayTime;
        }
        else if (sqrdistToPlayer <= mVisionRadiusSquared && mMovementDelayTimer <= 0.0f)
        {
            dirToPlayer.Normalize();
            mEnemy.MoveDir(dirToPlayer);
        }
    }

    protected void FireProjectiles(int numProjectiles)
    {
        Vector3 startPos = transform.position + transform.up * 0.5f;
        Vector3 rotation = transform.eulerAngles;
        rotation.y -= numProjectiles / 2 * mSpreadAngle;
        for (int i = 0; i < numProjectiles; ++i)
        {
            Vector3 dir = Quaternion.Euler(rotation) * Vector3.forward;
            mEnemy.FireProjectile(mEnemy, mFlowerProjectilePrefab, startPos + dir, dir, mProjectileSpeed, Quaternion.identity);

            rotation.y += mSpreadAngle;
        }
    }
}
