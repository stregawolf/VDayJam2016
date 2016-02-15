using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaseWeaponDisplay : MonoBehaviour {
    public GameObject mRoseWeapon;
    public GameObject mVuWeapon;

    public RawImage mBgImage;
    public Color mSelectedColor = Color.green;

    public BasePlayer.EquipedWeaponType mWeaponType = BasePlayer.EquipedWeaponType.Melee;

    protected virtual void Awake()
    {
        if(GlobalData.sSelectedCharacter == SelectedCharacter.Rose)
        {
            mVuWeapon.SetActive(false);
        }
        else
        {
            mRoseWeapon.SetActive(false);
        }

        Signal.Register(SignalType.WeaponChanged, OnWeaponChanged);
    }

    protected virtual void OnDestroy()
    {
        Signal.Unregister(SignalType.WeaponChanged, OnWeaponChanged);
    }

    protected virtual void OnWeaponChanged()
    {
        SetSelected(GameManager.Instance.Player1.mPlayer.mEquipedWeaponType == mWeaponType);
    }

    public virtual void SetSelected(bool isSelected)
    {
        if(isSelected)
        {
            mBgImage.color = mSelectedColor;
        }
        else
        {
            mBgImage.color = Color.white;
        }
    }
}
