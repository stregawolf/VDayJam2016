using UnityEngine;
using System.Collections;

public class Bobble : MonoBehaviour {
    public float mBobbleSpeed = 1.0f;
    public float mBobbleHeight = 50.0f;
    public float mT = 0.0f;
    protected Vector3 mStartPos;

    void Start()
    {
        mStartPos = transform.localPosition;
    }

	// Update is called once per frame
	void Update () {
        mT += Time.deltaTime * mBobbleSpeed;
        transform.localPosition = mStartPos + Vector3.up * mBobbleHeight * Mathf.Cos(mT);
	}
}
