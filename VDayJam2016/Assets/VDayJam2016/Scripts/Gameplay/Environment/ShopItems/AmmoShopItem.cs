using UnityEngine;
using System.Collections;

public class AmmoShopItem : BaseShopItem {
    public GameObject mVuAmmoModel;
    public GameObject mRoseAmmoModel;

    public int mAmmoAmount = 10;

    protected override void Awake()
    {
        base.Awake();

        if(GlobalData.sSelectedCharacter == SelectedCharacter.Rose)
        {
            mVuAmmoModel.SetActive(false);
        }
        else
        {
            mRoseAmmoModel.SetActive(false);
        }
    }

    public override bool OnPurchase()
    {
        if(GlobalData.NumAmmo < GlobalData.MaxAmmo)
        {
            GlobalData.NumAmmo += mAmmoAmount;
            return true;
        }
        return false;
    }
}
