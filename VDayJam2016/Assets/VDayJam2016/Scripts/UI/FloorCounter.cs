using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloorCounter : MonoBehaviour {
    public Text mCountLabel;

    private void Awake()
    {
        OnFloorChanged();
        Signal.Register(SignalType.OnFloorChanged, OnFloorChanged);
        Signal.Register(SignalType.FinalBossLevelCompleted, OnFinalBossLevelCompleted);
    }

    private void OnDestroy()
    {
        Signal.Unregister(SignalType.OnFloorChanged, OnFloorChanged);
        Signal.Unregister(SignalType.FinalBossLevelCompleted, OnFinalBossLevelCompleted);
    }

    public void OnFloorChanged()
    {
        SetDisplayAmount(GlobalData.CurrentFloor);
    }

    public void OnFinalBossLevelCompleted()
    {
        mCountLabel.text = "Congratulations!";
    }

    public void SetDisplayAmount(int amount)
    {
        if(GlobalData.ActiveBoss == GlobalData.BossId.None)
        {
            mCountLabel.text = string.Format("Floor {0}", amount);
        }
        else
        {
            mCountLabel.text = string.Format("Boss {0}", GlobalData.ActiveBoss.ToString());
        }
    }
}
