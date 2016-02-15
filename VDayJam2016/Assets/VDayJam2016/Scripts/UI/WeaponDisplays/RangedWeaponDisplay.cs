using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RangedWeaponDisplay : BaseWeaponDisplay
{
    public Text mCountLabel;
    public Color mOutOfAmmoColor = Color.red;

    protected override void Awake()
    {
        base.Awake();
        Signal.Register(SignalType.AmmoAmountChanged, OnNumAmmoChanged);
    }

    protected void Start()
    {
        OnNumAmmoChanged();
    } 

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Signal.Unregister(SignalType.AmmoAmountChanged, OnNumAmmoChanged);
    }

    public void OnNumAmmoChanged()
    {
        SetDisplayAmount(GlobalData.NumAmmo, GlobalData.MaxAmmo);
        OnWeaponChanged();
    }

    public void SetDisplayAmount(int amount, int max)
    {
        mCountLabel.text = string.Format("{0}/{1}", amount, max);
    }

    public override void SetSelected(bool isSelected)
    {
        if(isSelected)
        {
            if (GlobalData.NumAmmo > 0)
            {
                base.SetSelected(isSelected);
            }
            else
            {
                mBgImage.color = mOutOfAmmoColor;
            }
        }
        else
        {
            base.SetSelected(isSelected);
        }

        mCountLabel.color = (GlobalData.NumAmmo > 0) ? Color.white : mOutOfAmmoColor;
    }
}
