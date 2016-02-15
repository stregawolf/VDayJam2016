using UnityEngine;
using System.Collections.Generic;

public class Shop : MonoBehaviour {
    public GameObject[] mAlwaysAvailableItems;
    public GameObject[] mRandomItemPool;

    public Transform[] mShopItemSlots;

    protected void Start()
    {
        SpawnShopItems();
    }

    public void SpawnShopItems()
    {
        int numAlwaysAvailableItems = mAlwaysAvailableItems.Length;
        List<GameObject> randomItemPool = new List<GameObject>(mRandomItemPool);

        for (int i = 0, n = mShopItemSlots.Length; i < n; ++i)
        {
            if(i < numAlwaysAvailableItems)
            {
                SetShopItem(mAlwaysAvailableItems[i], i);
            }
            else
            {
                if(randomItemPool.Count > 0)
                {
                    GameObject randomItem = randomItemPool[Random.Range(0, randomItemPool.Count)];
                    randomItemPool.Remove(randomItem);
                    SetShopItem(randomItem, i);
                }
            }
        }
    }

    public void SetShopItem(GameObject prefab, int slotIndex)
    {
        Transform slot = mShopItemSlots[slotIndex];
        GameObject obj = Instantiate(prefab, slot.position, slot.rotation) as GameObject;
        obj.transform.SetParent(slot);
    }
}
