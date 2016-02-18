using UnityEngine;
using System.Collections;

public class HeartCollectable : BaseCollectable
{
    public GameObject mCollectionVFX;
    public override void OnCollect(GameObject collector)
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_pickup2);
        BasePlayer hitPlayer = collector.GetComponentInParent<BasePlayer>();
        if (hitPlayer != null)
        {
            hitPlayer.mDialogText.Show("+1 Heart", 2.0f);
            GlobalData.NumHearts++;
            Instantiate(mCollectionVFX, transform.position, Quaternion.identity);
            base.OnCollect(collector);
        }
    }
}
