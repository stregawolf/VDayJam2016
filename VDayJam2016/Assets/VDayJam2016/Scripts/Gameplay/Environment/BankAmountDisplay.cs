using UnityEngine;
using System.Collections;

public class BankAmountDisplay : MonoBehaviour {
    public TextMesh mText;

	protected void Awake()
    {
        OnBankAmountChanged();
        Signal.Register(SignalType.BankedHeartAmountChanged, OnBankAmountChanged);
    }

    protected void OnDestroy()
    {
        Signal.Unregister(SignalType.BankedHeartAmountChanged, OnBankAmountChanged);
    }

    public void OnBankAmountChanged()
    {
        mText.text = string.Format("Banked Hearts:\n{0}", GlobalData.NumBankedHearts);
    }
}
