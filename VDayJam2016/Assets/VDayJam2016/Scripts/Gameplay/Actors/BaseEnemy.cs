using UnityEngine;
using System.Collections;

public class BaseEnemy : BaseActor {
    public float mHitRadius = 0.5f;
    public int mAttackPower = 1;

    public virtual void Attack(Vector3 target)
    {
        if(IsPlayingAnimation())
        {
            return;
        }

        Stop();
        LookAtPoint(target);
        TriggerAnimation("EnemyAttack");
    }

    public virtual void OnAttackLanded()
    {
        Collider[] colliders = Physics.OverlapSphere(mModel.transform.position, mHitRadius);
        for(int i = 0, n = colliders.Length; i < n; ++i)
        {
            BasePlayer player = colliders[i].GetComponentInParent<BasePlayer>();
            if(player != null && player != this)
            {
                Vector3 knockback = player.transform.position - transform.position;
                knockback.Normalize();
                player.KnockBack(knockback * mAttackPower / 2.0f);
                player.TakeDamage(mAttackPower);
            }
        }
    }
}
