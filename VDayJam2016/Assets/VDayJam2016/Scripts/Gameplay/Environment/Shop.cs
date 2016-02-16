using UnityEngine;
using System.Collections.Generic;

public class Shop : MonoBehaviour {
    public GameObject[] mSpecialItemPool;
    public GameObject[] mGiftItemPool;
    
    public Transform mSpecialItemSlot;
    public Transform mGiftItemSlot;

    public DialogText mDialogText;

    public string[] mRandomDialog;
    public string[] mRandomTranslatedDialog;

    protected void Awake()
    {
        if(mDialogText == null)
        {
            mDialogText = GetComponentInChildren<DialogText>();
        }
    }

    protected void Start()
    {
        SpawnShopItems();

        if (GlobalData.ItemCollected(GlobalData.ItemId.Translator))
        {
            mDialogText.Show("Welcome to my shop!", 10.0f);
        }
        else
        {
            mDialogText.Show("Mew Mew Meow!", 10.0f);
        }
    }

    public void SpawnShopItems()
    {
        // set special item
        List<GameObject> availableSpecialItems = new List<GameObject>();
        for (int i = 0, n = mSpecialItemPool.Length; i < n; ++i)
        {
            BaseShopItem shopItem = mSpecialItemPool[i].GetComponent<BaseShopItem>();
            if (shopItem != null && shopItem.CanBePurchased())
            {
                availableSpecialItems.Add(mSpecialItemPool[i]);
            }
        }

        if(availableSpecialItems.Count > 0)
        {
            SetShopItem(availableSpecialItems[Random.Range(0, availableSpecialItems.Count)], mSpecialItemSlot);
        }

        // set gift item
        for (int i = 0, n = mGiftItemPool.Length; i < n; ++i)
        {
            BaseShopItem shopItem = mGiftItemPool[i].GetComponent<BaseShopItem>();
            if (shopItem != null && shopItem.CanBePurchased())
            {
                SetShopItem(mGiftItemPool[i], mGiftItemSlot);
                break;
            }
        }

        /*
        int numAlwaysAvailableItems = mAlwaysAvailableItems.Length;
        List<GameObject> availableSpecialItems = new List<GameObject>();
        for(int i = 0, n = mSpecialItemPool.Length; i < n; ++i)
        {
            BaseShopItem shopItem = mSpecialItemPool[i].GetComponent<BaseShopItem>();
            if(shopItem != null && shopItem.CanBePurchased())
            {
                availableSpecialItems.Add(mSpecialItemPool[i]);
            }
        }

        for (int i = 0, n = mShopItemSlots.Length; i < n; ++i)
        {
            if(i < numAlwaysAvailableItems)
            {
                SetShopItem(mAlwaysAvailableItems[i], i);
            }
            else
            {
                if(availableSpecialItems.Count > 0)
                {
                    GameObject randomItem = availableSpecialItems[Random.Range(0, availableSpecialItems.Count)];
                    availableSpecialItems.Remove(randomItem);
                    SetShopItem(randomItem, i);
                }
                else
                {
                    break;
                }
            }
        }
        */
    }

    public void SetShopItem(GameObject prefab, Transform slot)
    {
        GameObject obj = Instantiate(prefab, slot.position, slot.rotation) as GameObject;
        obj.transform.SetParent(slot);
    }

    protected void OnCollisionEnter(Collision c)
    {
        BasePlayer player = c.collider.GetComponentInParent<BasePlayer>();
        if(player != null)
        {
            if (GlobalData.ItemCollected(GlobalData.ItemId.Translator))
            {
                mDialogText.Show(mRandomTranslatedDialog[Random.Range(0, mRandomTranslatedDialog.Length)]);
            }
            else
            {
                mDialogText.Show(mRandomDialog[Random.Range(0, mRandomDialog.Length)]);
            }
        }
    }
}
