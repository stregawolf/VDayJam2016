﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public const float kActionDelayTime = 0.1f;

    public BasePlayer mPlayer;
    protected Plane mGroundPlane;
    protected float mActionDelayTimer = 0.0f;

    public enum QueuedActionType
    {
        None,
        ChangeToMelee,
        ChangeToRange,
    }

    protected QueuedActionType mQueuedAction = QueuedActionType.None;

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
            mQueuedAction = QueuedActionType.ChangeToMelee;
            mActionDelayTimer = kActionDelayTime;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mQueuedAction = QueuedActionType.ChangeToRange;
            mActionDelayTimer = kActionDelayTime;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if(mPlayer.mEquipedWeaponType == BasePlayer.EquipedWeaponType.Melee)
            {
                mQueuedAction = QueuedActionType.ChangeToRange;
            }
            else
            {
                mQueuedAction = QueuedActionType.ChangeToMelee;
            }
            mActionDelayTimer = kActionDelayTime;
        }
        /*
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mPlayer.mDialogText.Show("TEST test test\nTest test");
        }
        */
        if (Input.GetMouseButtonDown(0))
        {
            mPlayer.Attack();
        }

        if(mPlayer.IsPlayingAnimation())
        {
            return;
        }

        if(mActionDelayTimer <= 0.0f)
        {
            return;
        }

        mActionDelayTimer -= Time.deltaTime;
        if (mActionDelayTimer <= 0.0f)
        {
            switch (mQueuedAction)
            {
                case QueuedActionType.ChangeToMelee:
                    mPlayer.SetEquipedWeaponType(BasePlayer.EquipedWeaponType.Melee);
                    break;
                case QueuedActionType.ChangeToRange:
                    mPlayer.SetEquipedWeaponType(BasePlayer.EquipedWeaponType.Ranged);
                    break;
            }
            mQueuedAction = QueuedActionType.None;
        }
        

    }
}
