using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public BasePlayer mPlayer;
    protected Plane mGroundPlane;

    public enum EquipedWeaponType
    {
        Melee,
        Ranged,
        Support,
    }
    protected EquipedWeaponType mEquipedWeaponType = EquipedWeaponType.Melee;

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
        mPlayer.MoveDir(moveDir, Mathf.Clamp01(moveDir.magnitude));

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (mGroundPlane.Raycast(mouseRay, out dist))
        {
            Vector3 hit = mouseRay.GetPoint(dist);
            mPlayer.LookAtPoint(hit);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            mEquipedWeaponType = EquipedWeaponType.Melee;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mEquipedWeaponType = EquipedWeaponType.Ranged;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            mEquipedWeaponType = EquipedWeaponType.Support;
        }

            if (Input.GetMouseButtonDown(0))
        {
            switch(mEquipedWeaponType)
            {
                case EquipedWeaponType.Melee:
                    mPlayer.SwingWeapon();
                    break;
                case EquipedWeaponType.Ranged:
                    mPlayer.ThrowProjectile();
                    break;
                case EquipedWeaponType.Support:
                    mPlayer.UseSupport();
                    break;
            }
        }
    }
}
