using UnityEngine;
using System.Collections;

public class BaseActor : MonoBehaviour {

    public float mSpeed = 5.0f;

    public Animator mAnimator;
    public Rigidbody mRigidbody;

    protected Vector3 mMoveDir;

    protected virtual void Awake()
    {
        if (mRigidbody == null)
        {
            mRigidbody = GetComponent<Rigidbody>();
        }

        if(mAnimator == null)
        {
            mAnimator = GetComponent<Animator>();
        }
    }

    public virtual void TeleportTo(Vector3 pos)
    {
        transform.position = pos;
    }

    public virtual void MoveDir(Vector3 dir)
    {
        mMoveDir = dir.normalized;
        mAnimator.SetBool("IsWalking", dir.magnitude > 0);
    }

    public virtual void LookAtPoint(Vector3 pos)
    {
        LookDir(pos - transform.position);
    }

    public virtual void LookDir(Vector3 dir)
    {
        dir.y = transform.position.y;
        dir.Normalize();
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    protected virtual void Update()
    {
        transform.position += mMoveDir * mSpeed * Time.deltaTime;
    }
}
