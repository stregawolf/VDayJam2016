using UnityEngine;
using System.Collections;

public class BaseCollectable : MonoBehaviour {

    public virtual void OnTriggerEnter(Collider c)
    {
        OnCollect(c.gameObject);
    }

    public virtual void OnCollect(GameObject collector)
    {
        Destroy(gameObject);
    }
}
