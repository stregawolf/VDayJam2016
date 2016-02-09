using UnityEngine;
using System.Collections;

public class BaseProjectile : MonoBehaviour {

    public int mDamagePower;
    public float mLifetime = 2.0f;

    public Rigidbody mRigidbody;
    public Collider mCollider;

    protected BaseActor mOwner;

    protected void Awake()
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

    public virtual void Throw(BaseActor owner, Vector3 startPos, Vector3 dir, float speed)
    {
        mOwner = owner;
        mRigidbody.position = startPos;
        mRigidbody.AddTorque(Random.onUnitSphere * 10.0f);
        mRigidbody.AddForce(dir * speed, ForceMode.VelocityChange);
        if(owner != null && owner.mCollider != null && mCollider != null)
        {
            Physics.IgnoreCollision(owner.mCollider, mCollider);
        }
        Destroy(gameObject, mLifetime);
    }

    public void OnCollisionEnter(Collision c)
    {
        BaseActor hitActor = c.gameObject.GetComponentInParent<BaseActor>();
        if(hitActor != null && hitActor != mOwner)
        {
            hitActor.TakeDamage(mDamagePower);
            hitActor.KnockBack(mRigidbody.velocity.normalized * mDamagePower / 2.0f);
            Destroy(gameObject);
        }
    }
}
