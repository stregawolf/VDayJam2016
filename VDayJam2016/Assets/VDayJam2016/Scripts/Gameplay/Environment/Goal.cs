using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
    public void OnTriggerEnter(Collider c)
    {
        GlobalData.sCurrentFloor++;
        Signal.Dispatch(SignalType.LevelComplete);
    }
}
