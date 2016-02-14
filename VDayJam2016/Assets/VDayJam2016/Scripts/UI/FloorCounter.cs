using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloorCounter : MonoBehaviour {
    public Text mCountLabel;

    private void Awake()
    {
        Signal.Register(SignalType.LevelStart, OnFloorChanged);
    }

    private void OnDestroy()
    {
        Signal.Unregister(SignalType.LevelStart, OnFloorChanged);
    }

    public void OnFloorChanged()
    {
        SetDisplayAmount(GlobalData.sCurrentFloor);
    }

    public void SetDisplayAmount(int amount)
    {
        mCountLabel.text = string.Format("Floor {0}", amount);
    }
}
