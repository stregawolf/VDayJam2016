﻿using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
    public BaseEnemy mEnemy;
    public float mMinDistance = 0.0f;
    public float mVisionRadius = 5.0f;
    public float mAttackRadius = 1.0f;
    public bool mStopWhileFlickering = true;
    protected float mMinDistanceSquared;
    protected float mVisionRadiusSquared;
    protected float mAttackRadiusSquared;
    protected Vector3 mOriginalPos;

    public float mInitialResponseDelay = 0.0f;
    public float mAttackPauseTime = 0.25f;
    protected float mResponseDelayTimer = 0.0f;
    public float mDefaultTimeBetweenAttacks = 1.0f;
    protected float mAttackDelayTimer = 0.0f;

    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Flee,
        Boss,
    }

    public EnemyState mCurrentState = EnemyState.Idle;

    protected virtual void Awake()
    {
        if(mEnemy == null)
        {
            mEnemy = GetComponent<BaseEnemy>();
        }
    }

    [ContextMenu("Init")]
    public virtual void Init()
    {
        mResponseDelayTimer = mInitialResponseDelay;
        mMinDistanceSquared = mMinDistance * mMinDistance;
        mVisionRadiusSquared = mVisionRadius * mVisionRadius;
        mAttackRadiusSquared = mAttackRadius * mAttackRadius;
        mOriginalPos = transform.position;
    }

    public virtual void UpdateBehavior(BasePlayer player)
    {
        if((mStopWhileFlickering && mEnemy.IsFlickering) || mEnemy.Hp <= 0)
        {
            return;
        }

        if(mResponseDelayTimer > 0)
        {
            mResponseDelayTimer -= Time.deltaTime;
            return;
        }
        mAttackDelayTimer -= Time.deltaTime;
        switch (mCurrentState)
        {
            case EnemyState.Idle:
                HandleIdleState(player);
                break;
            case EnemyState.Chase:
                HandleChaseState(player);
                break;
            case EnemyState.Attack:
                HandleAttackState(player);
                break;
            case EnemyState.Flee:
                HandleFleeState(player);
                break;
            case EnemyState.Boss:
                HandleBossBehaviour(player);
                break;
        }
    }

    protected virtual void HandleBossBehaviour(BasePlayer player)
    {

    }

    protected virtual void HandleFleeState(BasePlayer player)
    {
        Vector3 dirToPlayer = player.transform.position - transform.position;
        float sqrdistToPlayer = dirToPlayer.sqrMagnitude;
        if (sqrdistToPlayer <= mAttackRadiusSquared && mAttackDelayTimer <= 0.0f)
        {
            mEnemy.Attack(player.transform.position);
            mCurrentState = EnemyState.Attack;
            mResponseDelayTimer = mAttackPauseTime;
            mAttackDelayTimer = mDefaultTimeBetweenAttacks;
        }
        else if (sqrdistToPlayer <= mVisionRadiusSquared)
        {
            dirToPlayer.Normalize();
            mEnemy.MoveDir(dirToPlayer*-1);
        }
        else
        {
            mEnemy.Stop();
            mCurrentState = EnemyState.Idle;
        }
    }

    protected virtual void HandleAttackState(BasePlayer player)
    {
        if(!mEnemy.IsPlayingAnimation())
        {
            mCurrentState = EnemyState.Idle;
        }
    }

    protected virtual void HandleChaseState(BasePlayer player)
    {
        Vector3 dirToPlayer = player.transform.position - transform.position;
        float sqrdistToPlayer = dirToPlayer.sqrMagnitude;
        if(sqrdistToPlayer <= mAttackRadiusSquared && mAttackDelayTimer <= 0.0f)
        {
            mEnemy.Attack(player.transform.position);
            mCurrentState = EnemyState.Attack;
            mResponseDelayTimer = mAttackPauseTime;
            mAttackDelayTimer = mDefaultTimeBetweenAttacks;
        }
        else if(sqrdistToPlayer <= mMinDistanceSquared)
        {
            dirToPlayer.Normalize();
            mEnemy.MoveDir(-dirToPlayer);
        }
        else if (sqrdistToPlayer <= mVisionRadiusSquared)
        {
            if(sqrdistToPlayer > mAttackRadiusSquared)
            {
                dirToPlayer.Normalize();
                mEnemy.MoveDir(dirToPlayer);
            }
            else
            {
                mEnemy.LookAtPoint(player.transform.position);
            }
        }
        else
        {
            mEnemy.Stop();
            mCurrentState = EnemyState.Idle;
        }
    }

    [Header("Idle State Parameters")]
    public float mMinWanderTime = 1.0f;
    public float mMaxWanderTime = 3.0f;
    public bool mbRunsFromPlayer = false;

    protected float mWanderTimer = 0.0f;
    protected float mWanderSpeed = 0.0f;
    protected Vector3 mWanderDir = Vector3.zero;

    protected virtual void HandleIdleState(BasePlayer player)
    {
        mWanderTimer -= Time.deltaTime;
        if (mWanderTimer <= 0.0f)
        {
            mWanderTimer = Random.Range(mMinWanderTime, mMaxWanderTime);
            mWanderDir = Random.onUnitSphere;
            mWanderDir.y = 0;
            mWanderSpeed = mWanderDir.magnitude/2.0f;
            mWanderDir.Normalize();
        }
        else
        {
            mEnemy.MoveDir(mWanderDir, mWanderSpeed);
        }

        if (Vector3.SqrMagnitude(player.transform.position - transform.position) <= mVisionRadiusSquared)
        {
            //Debug.Log("Player detected");
            if (mbRunsFromPlayer)
            {
                mCurrentState = EnemyState.Flee;
            }
            else
            {
                //Debug.Log("Chasing");
                mCurrentState = EnemyState.Chase;
            }
        }
    }
}
