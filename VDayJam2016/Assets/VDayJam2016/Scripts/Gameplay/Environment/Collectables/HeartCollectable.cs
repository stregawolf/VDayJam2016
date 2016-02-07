using UnityEngine;
using System.Collections;

public class HeartCollectable : BaseCollectable {

    public override void OnCollect(GameObject collector)
    {
        Signal.Dispatch(SignalType.HeartCollected);
        base.OnCollect(collector);
    }
}
