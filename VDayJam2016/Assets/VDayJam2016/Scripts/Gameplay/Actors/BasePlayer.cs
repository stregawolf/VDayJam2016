using UnityEngine;
using System.Collections;

public class BasePlayer : BaseActor {
    public float mSwingRadius = 1.0f;
    public int mMeleeDamage = 1;
    public GameObject mProjectilePrefab;
    public float mThrowVelocity = 20.0f;

    public DialogText mDialogText;

    public enum EquipedWeaponType
    {
        Melee,
        Ranged,
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
            Stop();
            mHp = 0;
            StartCoroutine(Flicker(OnDeath));
        }
        else
        {
            float rand = (Random.value);
            SoundManager.Instance.PlaySfx((rand>.5)? SoundManager.Instance.sfx_playerhurt1: SoundManager.Instance.sfx_playerhurt2);
            TriggerAnimation("Hurt");
            StartCoroutine(Flicker());
            LoseHearts(GlobalData.NumHearts);
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
                SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_player_atk_melee);
                
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
                SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_player_atk_ranged);
                
                Vector3 startPos = transform.position + transform.up * 0.5f + transform.forward * 0.5f;
                FireProjectile(this, mProjectilePrefab, startPos, transform.forward, mThrowVelocity, Random.rotation);
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
}
