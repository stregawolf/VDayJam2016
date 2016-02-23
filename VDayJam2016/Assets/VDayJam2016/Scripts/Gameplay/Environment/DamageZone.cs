using UnityEngine;
using System.Collections;

public class DamageZone : MonoBehaviour {
    public int mDamageAmount = 1;

    public void OnTriggerStay(Collider c)
    {
        BasePlayer player = c.GetComponentInParent<BasePlayer>();
        if(player != null)
        {
            player.TakeDamage(mDamageAmount);
            player.KnockBack((player.transform.position - transform.position).normalized);
        }
    }
}
