using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartCounter : MonoBehaviour {
    public Text mCountLabel;
    
    private void Awake()
    {
        OnNumHeartsChanged();
        Signal.Register(SignalType.HeartAmountChanged, OnNumHeartsChanged);
    }

    private void OnDestroy()
    {
        Signal.Unregister(SignalType.HeartAmountChanged, OnNumHeartsChanged);
    }

    public void OnNumHeartsChanged()
    {
        SetDisplayAmount(GlobalData.NumHearts);
    }

    public void SetDisplayAmount(int amount)
    {
        mCountLabel.text = amount.ToString();
    }
}
