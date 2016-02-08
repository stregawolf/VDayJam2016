using UnityEngine;
using System.Collections;

public class FollowCamera : BaseCamera {
    public float mLerpSpeed = 2.0f;
    public Vector3 mOffset = Vector3.zero;
    public Transform mTarget;
    public bool mUseMouseLookAhead = true;

    [Range(0.0f,1.0f)]
    public float mMouseLookAheadFactor = 0.25f;

    protected Plane mGroundPlane;

    protected void Awake()
    {
        mGroundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public void Init(Transform target)
    {
        mTarget = target;
        transform.position = new Vector3(mTarget.position.x, transform.position.y, mTarget.position.z) + mOffset;
    }

    public override void UpdatePosition()
    {
        if(mTarget == null)
        {
            return;
        }

        Vector3 destPos = new Vector3(mTarget.position.x, transform.position.y, mTarget.position.z) + mOffset;
        if(mUseMouseLookAhead)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            if(mGroundPlane.Raycast(mouseRay, out dist))
            {
                Vector3 mousePos = mouseRay.GetPoint(dist);
                mousePos.y = destPos.y;
                destPos = Vector3.Lerp(destPos, mousePos, mMouseLookAheadFactor);
            }
        }
        transform.position = Vector3.Lerp(transform.position, destPos, Time.deltaTime * mLerpSpeed);
    }
}
