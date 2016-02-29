using UnityEngine;
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
            if (mIsDeposit)
            {
                if(GlobalData.NumHearts >= mTransferAmount)
                {
                    SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_deposit, .5f);
                    GlobalData.NumBankedHearts += mTransferAmount;
                    GlobalData.NumHearts -= mTransferAmount;
                    mDialogText.Show(string.Format("{0} heart deposited!", mTransferAmount));
                    Instantiate(mSuccessVFX, transform.position, Quaternion.identity);
                    
                    GlobalData.Save();
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
                    SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_withdraw, .5f);
                    GlobalData.NumBankedHearts -= mTransferAmount;
                    GlobalData.NumHearts += mTransferAmount;
                    mDialogText.Show(string.Format("{0} heart Withdrawn!", mTransferAmount),3);
                    Instantiate(mSuccessVFX, player.transform.position, Quaternion.identity);
                    GlobalData.Save();
                }
                else
                {
                    mDialogText.Show("No heart to withdraw");
                }
            }
        }
    }
}
