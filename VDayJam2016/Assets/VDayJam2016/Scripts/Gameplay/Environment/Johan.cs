using UnityEngine;
using System.Collections;

public class Johan : MonoBehaviour {
    public DialogText mDialogText;
    public GameObject mHeartProjectilePrefab;
    private bool mHeartGiven = false;

    protected void Awake()
    {
        if(mDialogText == null)
        {
            mDialogText = GetComponentInChildren<DialogText>();
        }
    }

    protected void OnCollisionEnter(Collision c)
    {
        BasePlayer player = c.collider.GetComponentInParent<BasePlayer>();
        if (player != null)
        {
            player.mDialogText.Show("Hi Johan");
            mDialogText.Show("Cool");
            if(!mHeartGiven)
            {
                Vector3 dir = transform.position - player.transform.position;
                dir.Normalize();
                Vector3 startPos = transform.position + transform.up * 0.5f + dir;
                GameObject projectileObj = GameManager.Instance.SpawnPrefab(mHeartProjectilePrefab, startPos, Quaternion.identity);
                HeartProjectile projectile = projectileObj.GetComponent<HeartProjectile>();
                projectile.Throw(null, startPos, dir, 5);
                projectile.mHeartValue = 1;

                mHeartGiven = true;
            }
        }
    }
}
