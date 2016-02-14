using UnityEngine;
using System.Collections;

public class HeartCollectable : BaseCollectable {

    public override void OnCollect(GameObject collector)
    {
        GlobalData.NumHearts++;
        base.OnCollect(collector);
    }
}
