using UnityEngine;
using System.Collections;

public class SpinProjectile : BaseProjectile {

    public Transform mModel;
    public float mRotationSpeed = 360.0f;

    public float mRampUpTime = 0.5f;
    protected float mRampUpTimer = 0.0f;

    protected void Update()
    {
        if(mRampUpTime > 0)
        {
            mRampUpTimer += Time.deltaTime;
            mModel.Rotate(Vector3.up * mRotationSpeed*Mathf.Clamp01(mRampUpTimer/mRampUpTime) * Time.deltaTime);
        }
        else
        {
            mModel.Rotate(Vector3.up * mRotationSpeed * Time.deltaTime);
        }
    }
}
