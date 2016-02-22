using UnityEngine;
using System.Collections;

public class EnemyPlayer : BaseEnemy {
    public float mSwingRadius = 1.0f;
    public int mMeleeDamage = 1;
    public GameObject mProjectilePrefab;
    public float mThrowVelocity = 20.0f;

    public GameObject mMeleeWeaponModel;
    public GameObject mRangedWeaponModel;

    public System.Action mSwingMidwayCallback;
    public System.Action mSwingCompleteCallback;

    public enum EquipedWeaponType
    {
        Melee,
        Ranged,
    }
    public EquipedWeaponType mEquipedWeaponType = EquipedWeaponType.Melee;

    protected void Start()
    {
        SetEquipedWeaponType(mEquipedWeaponType);
    }

    public virtual void SetEquipedWeaponType(EquipedWeaponType weaponType)
    {
        mEquipedWeaponType = weaponType;
        mMeleeWeaponModel.SetActive(false);
        mRangedWeaponModel.SetActive(false);

        switch (mEquipedWeaponType)
        {
            case EquipedWeaponType.Melee:
                mSwingMidwayCallback = OnMeleeSwingMidway;
                mSwingCompleteCallback = null;
                mMeleeWeaponModel.SetActive(true);
                break;
            case EquipedWeaponType.Ranged:
                mSwingMidwayCallback = OnRangedSwingMidWay;
                mSwingCompleteCallback = OnRangedSwingComplete;
                mRangedWeaponModel.SetActive(true);
                break;
        }
    }

    public override void Attack(Vector3 target)
    {
        Stop();
        LookAtPoint(target);
        SwingWeapon();
    }

    public virtual void SwingWeapon()
    {
        TriggerAnimation("WeaponSwing");
    }

    public virtual void OnWeaponSwingMidway()
    {
        if(mSwingMidwayCallback != null)
        {
            mSwingMidwayCallback();
        }
    }

    public void OnMeleeSwingMidway()
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_player_atk_melee);

        Collider[] colliders = Physics.OverlapSphere(mModel.transform.position, mSwingRadius);
        for (int i = 0, n = colliders.Length; i < n; ++i)
        {
            BasePlayer player = colliders[i].GetComponentInParent<BasePlayer>();

            if (player != null && player != this)
            {
                Vector3 dirToEnemy = player.transform.position - transform.position;
                dirToEnemy.Normalize();
                if (Vector3.Dot(transform.forward, dirToEnemy) > 0.25f)
                {
                    player.KnockBack(dirToEnemy * mMeleeDamage / 2.0f);
                    player.TakeDamage(mMeleeDamage);
                }
            }
        }
    }

    public virtual void OnRangedSwingMidWay()
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_player_atk_ranged);

        Vector3 startPos = transform.position + transform.up * 0.5f + transform.forward * 0.5f;
        FireProjectile(this, mProjectilePrefab, startPos, transform.forward, mThrowVelocity, Quaternion.identity);
        mRangedWeaponModel.SetActive(false);
    }

    public virtual void OnRangedSwingComplete()
    {
        mRangedWeaponModel.SetActive(true);
    }

    public virtual void OnWeaponSwingComplete()
    {
        if (mSwingCompleteCallback != null)
        {
            mSwingCompleteCallback();
        }
    }

    
}
