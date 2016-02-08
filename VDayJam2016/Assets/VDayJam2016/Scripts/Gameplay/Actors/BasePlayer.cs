using UnityEngine;
using System.Collections;

public class BasePlayer : BaseActor {
    public float mSwingRadius = 1.0f;
    public int mMeleeDamage = 1;

    public virtual void SwingWeapon()
    {
        TriggerAnimation("WeaponSwing");
    }

    public virtual void OnWeaponHit()
    {
        Collider[] colliders = Physics.OverlapSphere(mModel.transform.position, mSwingRadius);
        for (int i = 0, n = colliders.Length; i < n; ++i)
        {
            BaseEnemy enemy = colliders[i].GetComponentInParent<BaseEnemy>();
            
            if (enemy != null && enemy != this)
            {
                Vector3 dirToEnemy = enemy.transform.position - transform.position;
                dirToEnemy.Normalize();
                if(Vector3.Dot(transform.forward, dirToEnemy) > 0.0f)
                {
                    enemy.KnockBack(dirToEnemy * mMeleeDamage / 2.0f);
                    enemy.TakeDamage(mMeleeDamage);
                }
            }
        }
    }
}
