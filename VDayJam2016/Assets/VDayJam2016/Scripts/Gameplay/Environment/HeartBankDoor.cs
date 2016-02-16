using UnityEngine;
using System.Collections;

public class HeartBankDoor : MonoBehaviour {
    public DialogText mDialogText;

    protected void Awake()
    {
        if(GlobalData.ItemCollected(GlobalData.ItemId.BankKey))
        {
            gameObject.SetActive(false);
        }
        else
        {
            Signal.Register(SignalType.HeartBankUnlocked, OnHeartBankUnlocked);
        }
    }

    protected void OnDestroy()
    {
        Signal.Unregister(SignalType.HeartBankUnlocked, OnHeartBankUnlocked);
    }

    protected void OnCollisionEnter(Collision c)
    {
        BasePlayer player = c.collider.GetComponent<BasePlayer>();
        if(player != null)
        {
            mDialogText.Show("Heart Bank is locked...");
        }
    }

    protected void OnHeartBankUnlocked()
    {
        StartCoroutine(HandleUnlock());
    }

    protected IEnumerator HandleUnlock()
    {
        float unlockTimer = 1.0f;
        while(unlockTimer > 0.0f)
        {
            unlockTimer -= Time.deltaTime;
            transform.localScale = Vector3.one * Mathf.Clamp01(unlockTimer);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
