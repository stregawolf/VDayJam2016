﻿using UnityEngine;
using System.Collections;

public class BaseProjectile : MonoBehaviour {

    public int mDamagePower;
    public float mLifetime = 2.0f;
    public float mMovementDelayTime = 0.0f;
    public bool mBounceOffWalls = true;

    public Rigidbody mRigidbody;
    public Collider mCollider;


    protected BaseActor mOwner;
    protected Vector3 mLastPos;
    protected virtual void Awake()
    {
        if(mRigidbody == null)
        {
            mRigidbody = GetComponentInChildren<Rigidbody>();
        }

        if(mCollider == null)
        {
            mCollider = GetComponentInChildren<Collider>();
        }
    }

    protected void FixedUpdate()
    {
        mMovementDelayTime -= Time.deltaTime;

        if (mMovementDelayTime > 0.0f || GameManager.Instance.mDungeon.GetCell(mRigidbody.position).mTileType == DungeonCell.TileType.Wall)
        {
            mRigidbody.position = mLastPos;
        }

        Vector3 dest = mRigidbody.position + mRigidbody.velocity * Time.fixedDeltaTime;
        if (GameManager.Instance.mDungeon.GetCell(dest).mTileType == DungeonCell.TileType.Wall)
        {
            if(mBounceOffWalls)
            {
                Vector2i currCell = GameManager.Instance.mDungeon.WorldToCellPos(mRigidbody.position);
                Vector2i destCell = GameManager.Instance.mDungeon.WorldToCellPos(dest);
                Vector3 vel = mRigidbody.velocity;
                if (destCell.mX - currCell.mX != 0)
                {
                    vel.x *= -0.5f;
                }
                if (destCell.mY - currCell.mY != 0)
                {
                    vel.z *= -0.5f;
                }
                vel.y *= 0.5f;
                mRigidbody.velocity = vel;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        mLastPos = mRigidbody.position;
    }

    public virtual void Throw(BaseActor owner, Vector3 startPos, Vector3 dir, float speed, bool addTorque = true)
    {
        mOwner = owner;
        mRigidbody.position = startPos;
        mLastPos = startPos;
        if (addTorque)
        {
            mRigidbody.AddTorque(Random.onUnitSphere * 10.0f);
        }
        mRigidbody.AddForce(dir * speed, ForceMode.VelocityChange);
        if(owner != null && owner.mCollider != null && mCollider != null)
        {
            Physics.IgnoreCollision(owner.mCollider, mCollider);
        }

        StartAutoDestruction();
    }

    protected virtual void StartAutoDestruction()
    {
        Destroy(gameObject, mLifetime);
    }

    public virtual void OnCollisionEnter(Collision c)
    {
        BaseActor hitActor = c.gameObject.GetComponentInParent<BaseActor>();
        if(hitActor != null && hitActor != mOwner)
        {
            hitActor.TakeDamage(mDamagePower);
            //hitActor.KnockBack(mRigidbody.velocity.normalized * mDamagePower *0.15f);
            Destroy(gameObject);
        }
    }
}
