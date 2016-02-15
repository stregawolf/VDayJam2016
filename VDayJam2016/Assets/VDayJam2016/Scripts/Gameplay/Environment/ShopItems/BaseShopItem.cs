using UnityEngine;
using System.Collections;

public class BaseShopItem : BaseCollectable {
    public TextMesh mCostText;
    public bool mAllowMultiplePurchase = false;
    public int mCost = 10;

    protected virtual void Awake()
    {
        mCostText.text = mCost.ToString();
    }

    public override void OnCollect(GameObject collector)
    {
        if(GlobalData.NumHearts >= mCost)
        {
            if(OnPurchase())
            {
                GlobalData.NumHearts -= mCost;
                if (!mAllowMultiplePurchase)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public virtual bool OnPurchase()
    {
        return true;
    }

    public virtual bool CanBePurchased()
    {
        return true;
    }
}
