using UnityEngine;
using System.Collections;

public class BaseEnemy : BaseActor {
    public float mTurnSpeed = 270.0f;

    public float mHitRadius = 0.5f;
    public int mAttackPower = 1;

    public int mMinHeartsValue = 1;
    public int mMaxHeartsValue = 10;

    public GameObject mProjectilePrefab;
    public float mThrowSpeed = 10.0f;

    public bool mIsBoss = false;

    public GameObject mDeathVFX;
    
    public AudioClip hurtSFX;

    public virtual void Attack(Vector3 target)
    {
        if(IsPlayingAnimation())
        {
            return;
        }

        Stop();

        Vector3 dir = target - transform.position;
        dir.y = transform.position.y;
        dir.Normalize();
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        if (mProjectilePrefab != null)
        {
            Vector3 startPos = transform.position + transform.up * 0.5f + transform.forward * 0.5f;
            FireProjectile(this, mProjectilePrefab, startPos, transform.forward, mThrowSpeed, Quaternion.identity);
        }
        else
        {
            TriggerAnimation("EnemyAttack");
        }
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
                player.KnockBack(knockback * mAttackPower);
                player.TakeDamage(mAttackPower);
            }
        }
    }

    public override void LookDir(Vector3 dir)
    {
        dir.y = transform.position.y;
        dir.Normalize();
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), mTurnSpeed*Time.deltaTime);
    }

    public override void TakeDamage(int amount)
    {
        Stop();
        base.TakeDamage(amount);
        SoundManager.Instance.PlaySfx(hurtSFX);
        if(mDeathVFX != null && mHp <= 0)
        {
            Instantiate(mDeathVFX, transform.position, Quaternion.identity);
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_monster_pop);
        int heartValue = Random.Range(mMinHeartsValue, mMaxHeartsValue + 1);
        if(heartValue > 0)
        {
            HeartProjectile heart = DropHeart(heartValue);
            if (mIsBoss)
            {
                heart.transform.localScale *= 2;
            }
        }
        

        if(mIsBoss)
        {
            Signal.Dispatch(SignalType.BossDefeated);
        }
    }

    public virtual void OnCollisionEnter(Collision c)
    {
        if(mHp <= 0)
        {
            return;
        }

        BasePlayer player = c.collider.GetComponentInParent<BasePlayer>();
        if(player != null)
        {
            Vector3 knockback = player.transform.position - transform.position;
            knockback.Normalize();
            player.KnockBack(knockback * mAttackPower);
            player.TakeDamage(mAttackPower);
        }
    }
}
