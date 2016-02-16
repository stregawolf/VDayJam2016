using UnityEngine;
using System.Collections;

public class SpecialIdShopItem : BaseShopItem {
    public GlobalData.ItemId mItemId;
    public GlobalData.BossId mBossRequirement = GlobalData.BossId.None;
    public SignalType mOnPurchaseSignal = SignalType.Unused;

    public override bool CanBePurchased()
    {
        return !GlobalData.ItemCollected(mItemId) && GlobalData.BossDefeated(mBossRequirement);
    }

    public override bool OnPurchase()
    {
        GlobalData.SetCollectedItem(mItemId);

        if(mOnPurchaseSignal != SignalType.Unused)
        {
            Signal.Dispatch(mOnPurchaseSignal);
        }

        return base.OnPurchase();
    }
}
