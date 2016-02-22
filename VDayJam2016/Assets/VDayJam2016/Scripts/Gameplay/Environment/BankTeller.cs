﻿using UnityEngine;
using System.Collections;

public class BankTeller : MonoBehaviour {
    public DialogText mDialogText;

    public int mTransferAmount;
    public bool mIsDeposit = true;
    public GameObject mSuccessVFX;

    protected void OnCollisionEnter(Collision c)
    {
        BasePlayer player = c.collider.GetComponentInParent<BasePlayer>();
        if (player != null)
        {
            if(mIsDeposit)
            {
                if(GlobalData.NumHearts >= mTransferAmount)
                {
                    GlobalData.NumBankedHearts += mTransferAmount;
                    GlobalData.NumHearts -= mTransferAmount;
                    mDialogText.Show(string.Format("{0} hearts deposited!", mTransferAmount));
                    Instantiate(mSuccessVFX, transform.position, Quaternion.identity);
                }
                else
                {
                    mDialogText.Show(string.Format("must deposit at least\n{0} hearts", mTransferAmount),3);
                }
            }
            else
            {
                if (GlobalData.NumBankedHearts >= mTransferAmount)
                {
                    GlobalData.NumBankedHearts -= mTransferAmount;
                    GlobalData.NumHearts += mTransferAmount;
                    mDialogText.Show(string.Format("{0} hearts Withdrawn!", mTransferAmount),3);
                    Instantiate(mSuccessVFX, player.transform.position, Quaternion.identity);
                }
                else
                {
                    mDialogText.Show("No hearts to withdraw");
                }
            }
        }
    }
}