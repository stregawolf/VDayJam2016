using UnityEngine;
using System.Collections;

public class BaseShopItem : MonoBehaviour {
    public TextMesh mCostText;
    public bool mAllowMultiplePurchase = false;
    public int mCost = 10;

    public string mPurchaseSuccessText = "Item purchased!";
    public string mPurchaseFailedText = "Could not purchase item";

    protected virtual void Awake()
    {
        mCostText.text = mCost.ToString();
    }

    public virtual void OnCollisionEnter(Collision c)
    {
        BasePlayer player = c.collider.GetComponentInParent<BasePlayer>();
        if(player == null)
        {
            return;
        }

        if (GlobalData.NumHearts >= mCost)
        {
            // can afford
            if(OnPurchase())
            {
                // successful purchase
                GlobalData.NumHearts -= mCost;
                if (player.mDialogText != null)
                {
                    player.mDialogText.Show(mPurchaseSuccessText);
                }

                if (!mAllowMultiplePurchase)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // failed purchase
                if (player.mDialogText != null)
                {
                    player.mDialogText.Show(mPurchaseFailedText);
                }
            }
        }
        else
        {
            // not enough
            if (player.mDialogText != null)
            {
                player.mDialogText.Show("Not enough heart");
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
