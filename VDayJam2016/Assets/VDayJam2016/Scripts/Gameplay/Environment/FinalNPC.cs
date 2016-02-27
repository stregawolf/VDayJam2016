using UnityEngine;
using System.Collections;

public class FinalNPC : MonoBehaviour
{
    public string mSceneToLoad = "";

    protected void OnCollisionEnter(Collision c)
    {
        BasePlayer player = c.collider.GetComponentInParent<BasePlayer>();
        if (player != null)
        {
            GameManager.Instance.GoToScene(mSceneToLoad);
        }
    }
}
