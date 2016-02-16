using UnityEngine;
using System.Collections;

public class BasePlayer : BaseActor {
    public float mSwingRadius = 1.0f;
    public int mMeleeDamage = 1;
    public GameObject mProjectilePrefab;
    public float mThrowVelocity = 20.0f;

    public GameObject mHeartProjectilePrefab;

    public DialogText mDialogText;

    public enum EquipedWeaponType
    {
        Melee,
        Ranged,
        Support,
    }
    public EquipedWeaponType mEquipedWeaponType = EquipedWeaponType.Melee;

    public GameObject mMeleeWeaponModel;
    public GameObject mRangedWeaponModel;

    protected override void Awake()
    {
        base.Awake();
        if(mDialogText == null)
        {
            mDialogText = GetComponentInChildren<DialogText>();
        }
    }

    protected void Start()
    {
        SetEquipedWeaponType(mEquipedWeaponType);
    }

    public override void TakeDamage(int amount)
    {
        if (IsFlickering)
        {
            return;
        }

        if(GlobalData.NumHearts <= 0)
        {
            StartCoroutine(Flicker(OnDeath));
        }
        else
        {
            TriggerAnimation("Hurt");
            StartCoroutine(Flicker());

            Vector3 startPos = transform.position + transform.up * 0.5f;
            int numHearts = GlobalData.NumHearts;
            while(numHearts > 0)
            {
                int value = 1;
                float scale = 1.0f;
                if(numHearts > 375)
                {
                    value = 100;
                    scale = 2.0f;
                }
                else if(numHearts > 125)
                {
                    value = 50;
                    scale = 1.75f;
                }
                else if(numHearts > 100)
                {
                    value = 25;
                    scale = 1.5f;
                }
                else if(numHearts > 10)
                {
                    value = 10;
                    scale = 1.25f;
                }

                Vector3 dir = Random.onUnitSphere;
                dir.y = 0;
                dir.Normalize();
                Vector3 spawnPos = startPos + dir * 0.5f;
                GameObject projectileObj = GameManager.Instance.SpawnPrefab(mHeartProjectilePrefab, spawnPos, Quaternion.identity);
                HeartProjectile projectile = projectileObj.GetComponent<HeartProjectile>();
                projectile.mHeartValue = value;
                projectile.transform.localScale *= scale;
                projectile.Throw(null, spawnPos, dir, Random.Range(3.0f, 10.0f), false);

                numHearts-= value;
            }

            GlobalData.NumHearts = 0;
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        Signal.Dispatch(SignalType.PlayerDeath);
    }

    public virtual void SetEquipedWeaponType(EquipedWeaponType weaponType)
    {
        mEquipedWeaponType = weaponType;
        mMeleeWeaponModel.SetActive(false);
        mRangedWeaponModel.SetActive(false);

        switch(mEquipedWeaponType)
        {
            case EquipedWeaponType.Melee:
                mMeleeWeaponModel.SetActive(true);
                break;
            case EquipedWeaponType.Ranged:
                mRangedWeaponModel.SetActive(GlobalData.NumAmmo > 0);
                break;
        }

        Signal.Dispatch(SignalType.WeaponChanged);
    }
    
    public virtual void Attack()
    {
        switch (mEquipedWeaponType)
        {
            case EquipedWeaponType.Melee:
                SwingWeapon();
                break;
            case EquipedWeaponType.Ranged:
                ThrowProjectile();
                break;
            case EquipedWeaponType.Support:
                UseSupport();
                break;
        }
    }

    public virtual void SwingWeapon()
    {
        TriggerAnimation("WeaponSwing");
    }

    public virtual void OnWeaponSwingMidway()
    {
        switch(mEquipedWeaponType)
        {
            case EquipedWeaponType.Melee:
                Collider[] colliders = Physics.OverlapSphere(mModel.transform.position, mSwingRadius);
                for (int i = 0, n = colliders.Length; i < n; ++i)
                {
                    BaseEnemy enemy = colliders[i].GetComponentInParent<BaseEnemy>();

                    if (enemy != null && enemy != this)
                    {
                        Vector3 dirToEnemy = enemy.transform.position - transform.position;
                        dirToEnemy.Normalize();
                        if (Vector3.Dot(transform.forward, dirToEnemy) > 0.0f)
                        {
                            enemy.KnockBack(dirToEnemy * mMeleeDamage / 2.0f);
                            enemy.TakeDamage(mMeleeDamage);
                        }
                    }
                }
                break;
            case EquipedWeaponType.Ranged:

                Vector3 startPos = transform.position + transform.up * 0.5f + transform.forward * 0.5f;
                GameObject projectileObj = GameManager.Instance.SpawnPrefab(mProjectilePrefab, startPos, Random.rotation);
                BaseProjectile projectile = projectileObj.GetComponent<BaseProjectile>();
                projectile.Throw(this, startPos, transform.forward, mThrowVelocity);

                mRangedWeaponModel.SetActive(false);
                break;
        }
    }

    public virtual void OnWeaponSwingComplete()
    {
        switch (mEquipedWeaponType)
        {
            case EquipedWeaponType.Ranged:
                mRangedWeaponModel.SetActive(GlobalData.NumAmmo > 0);
                break;
        }
    }

    public void ThrowProjectile()
    {
        if(GlobalData.NumAmmo <= 0)
        {
            return;
        }

        mRangedWeaponModel.SetActive(true);
        TriggerAnimation("WeaponSwing");
        GlobalData.NumAmmo--;
    }

    public void UseSupport()
    {

    }
}
