using UnityEngine;
using System.Collections;

public class FollowCamera : BaseCamera {
    public float mLerpSpeed = 2.0f;
    public Vector3 mOffset = Vector3.zero;
    public Transform mTarget;

    public void Init(Transform target)
    {
        mTarget = target;
    }

    public override void UpdatePosition()
    {
        if(mTarget == null)
        {
            return;
        }

        Vector3 destPos = new Vector3(mTarget.position.x, transform.position.y, mTarget.position.z) + mOffset;
        transform.position = Vector3.Lerp(transform.position, destPos, Time.deltaTime * mLerpSpeed);
    }
}
