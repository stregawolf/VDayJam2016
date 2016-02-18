using UnityEngine;
using System.Collections;

public class HeartCollectable : BaseCollectable {

    public override void OnCollect(GameObject collector)
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_pickup2);
        GlobalData.NumHearts++;
        base.OnCollect(collector);
    }
}
