using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
    public SignalType mSignalType = SignalType.LevelComplete;

    public void OnTriggerEnter(Collider c)
    {
        Signal.Dispatch(mSignalType);
    }
}
