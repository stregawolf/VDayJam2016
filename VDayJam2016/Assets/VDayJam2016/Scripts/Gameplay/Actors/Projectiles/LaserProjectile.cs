using UnityEngine;
using System.Collections;

public class LaserProjectile : SpinProjectile {
    public GameObject mDamagePrefab;
    public float mSpawnTime = 0.25f;

    protected float mSpawnTimer = 0.0f;

    protected override void Update()
    {
        base.Update();

        mSpawnTimer -= Time.deltaTime;
        if(mSpawnTimer <= 0)
        {
            mSpawnTimer = mSpawnTime;
            Instantiate(mDamagePrefab, transform.position, Quaternion.identity);
        }
    }
}
