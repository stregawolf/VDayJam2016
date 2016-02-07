using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
    public void OnTriggerEnter(Collider c)
    {
        Signal.Dispatch(SignalType.LevelComplete);
    }
}
