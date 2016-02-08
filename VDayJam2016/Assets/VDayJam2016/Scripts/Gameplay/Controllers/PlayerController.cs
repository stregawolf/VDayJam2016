using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public BasePlayer mPlayer;
    protected Plane mGroundPlane;

    protected void Awake()
    {
        if(mPlayer == null)
        {
            mPlayer = GetComponent<BasePlayer>();
        }

        mGroundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    public void UpdatePlayerControls()
    {
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //Debug.Log(moveDir);
        mPlayer.MoveDir(moveDir, moveDir.magnitude);

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (mGroundPlane.Raycast(mouseRay, out dist))
        {
            Vector3 hit = mouseRay.GetPoint(dist);
            mPlayer.LookAtPoint(hit);
        }
    }
}
