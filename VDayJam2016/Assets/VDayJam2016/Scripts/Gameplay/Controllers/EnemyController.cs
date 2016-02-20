using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
    public BaseEnemy mEnemy;
    public float mVisionRadius = 5.0f;
    public float mAttackRadius = 1.0f;
    public bool mStopWhileFlickering = true;
    protected float mVisionRadiusSquared;
    protected float mAttackRadiusSquared;
    protected Vector3 mOriginalPos;


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

        switch(mCurrentState)
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
        if (sqrdistToPlayer <= mAttackRadiusSquared)
        {
            mEnemy.transform.rotation = Quaternion.LookRotation(dirToPlayer.normalized);
            mEnemy.Attack(player.transform.position);
            mCurrentState = EnemyState.Attack;
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
        if(sqrdistToPlayer <= mAttackRadiusSquared)
        {
            mEnemy.Attack(player.transform.position);
            mCurrentState = EnemyState.Attack;
        }
        else if (sqrdistToPlayer <= mVisionRadiusSquared)
        {
            dirToPlayer.Normalize();
            mEnemy.MoveDir(dirToPlayer);
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
            Debug.Log("Player detected");
            if (mbRunsFromPlayer)
            {
                mCurrentState = EnemyState.Flee;
            }
            else
            {
                Debug.Log("Chasing");
                mCurrentState = EnemyState.Chase;
            }
        }
    }
}
