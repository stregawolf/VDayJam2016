using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using System;

public static class GlobalData
{
    public static SelectedCharacter sSelectedCharacter = SelectedCharacter.None;

    public static int TotalHearts { get { return sNumBankedHearts + sNumHearts; } }
    private static int sNumBankedHearts = 0;
    public static int NumBankedHearts
    {
        get { return sNumBankedHearts; }
        set { sNumBankedHearts = value; Signal.Dispatch(SignalType.BankedHeartAmountChanged); }
    }

    private static int sNumHearts = 1;
    public static int NumHearts
    {
        get { return sNumHearts; }
        set { sNumHearts = value; Signal.Dispatch(SignalType.HeartAmountChanged); }
    }

    private static int sCurrentFloor = 1;
    public static int CurrentFloor
    {
        get { return sCurrentFloor; }
        set { sCurrentFloor = value; Signal.Dispatch(SignalType.OnFloorChanged); }
    }

    private static int sNumAmmo = 10;
    public static int NumAmmo
    {
        get { return sNumAmmo; }
        set { sNumAmmo = Mathf.Clamp(value, 0, sMaxAmmo); Signal.Dispatch(SignalType.AmmoAmountChanged); }
    }
    private static int sMaxAmmo = 10;
    public static int MaxAmmo
    {
        get { return sMaxAmmo; }
        set { sMaxAmmo = value; Signal.Dispatch(SignalType.AmmoAmountChanged); }
    }

    public enum ItemId
    {
        Translator = 0,
        BankKey,
        GiftRing,
        GiftTech,
        GiftFlowers,
        GiftChocolate,
        NumItems,
    }
    private static HashSet<ItemId> sCollectedItems = new HashSet<ItemId>();
    public static void SetCollectedItem(ItemId id)
    {
        sCollectedItems.Add(id);
        switch (id)
        {
            case ItemId.GiftFlowers:
                ActiveBoss = BossId.Flower;
                break;
            case ItemId.GiftChocolate:
                ActiveBoss = BossId.Chocolate;
                break;
            case ItemId.GiftRing:
                ActiveBoss = BossId.Imposter;
                break;
        }
    }

    public static bool ItemCollected(ItemId id)
    {
        return sCollectedItems.Contains(id);
    }

    public enum BossId
    {
        None = 0,
        Flower,
        Chocolate,
        Imposter,
        NumBosses,
    }
    private static HashSet<BossId> sBossesDefeated = new HashSet<BossId>();
    public static void SetBossDefeated(BossId id)
    {
        if(id != BossId.None)
        {
            sBossesDefeated.Add(id);
            Analytics.CustomEvent(string.Format("BossDefeated-{0}", id.ToString()), new Dictionary<string, object> {
            { "CurrentFloor", sCurrentFloor},
            { "NumHearts", TotalHearts }});
        }

        ActiveBoss = BossId.None;
    }
    public static bool BossDefeated(BossId id)
    {
        if(id == BossId.None)
        {
            return true;
        }
        
        return sBossesDefeated.Contains(id);
    }
    
    private static BossId sActiveBoss = BossId.None;
    public static BossId ActiveBoss
    {
        get { return sActiveBoss; }
        set
        {
            sActiveBoss = value;
            if(sActiveBoss != BossId.None)
            {
                Analytics.CustomEvent(string.Format("BossUnlocked-{0}", sActiveBoss.ToString()), new Dictionary<string, object> {
                { "CurrentFloor", sCurrentFloor},
                { "NumHearts", TotalHearts }});
            }
            Signal.Dispatch(SignalType.OnFloorChanged);
        }
    }

    private static bool sGameLoaded = false;
    public static bool GameLoaded { get { return sGameLoaded; } }

    public static void ResetData()
    {
        sNumBankedHearts = 0;
        sNumHearts = 1;
        sCurrentFloor = 1;
        sNumAmmo = 10;
        sMaxAmmo = 10;

        sCollectedItems.Clear();
        sBossesDefeated.Clear();
        sActiveBoss = BossId.None;

        sGameLoaded = false;
    }

    public static bool SaveExists()
    {
        return PlayerPrefs.GetInt("SaveExists", 0) > 0;
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("PlayerCharacter", (int)sSelectedCharacter);

        PlayerPrefs.SetInt("NumBankedHearts", sNumBankedHearts);
        PlayerPrefs.SetInt("NumHearts", sNumHearts);
        PlayerPrefs.SetInt("CurrentFloor", sCurrentFloor);
        PlayerPrefs.SetInt("NumAmmo", sNumAmmo);
        PlayerPrefs.SetInt("MaxAmmo", sMaxAmmo);

        foreach(var item in sCollectedItems)
        {
            PlayerPrefs.SetInt(string.Format("Item_{0}", item.ToString()), 1);
        }

        foreach(var boss in sBossesDefeated)
        {
            PlayerPrefs.SetInt(string.Format("Boss_{0}", boss.ToString()), 1);
        }

        PlayerPrefs.SetInt("ActiveBoss", (int)sActiveBoss);

        PlayerPrefs.SetInt("SaveExists", 1);
    }

    public static void Load()
    {
        ResetData();

        sSelectedCharacter = (SelectedCharacter)PlayerPrefs.GetInt("PlayerCharacter", (int)sSelectedCharacter);

        sNumBankedHearts = PlayerPrefs.GetInt("NumBankedHearts", sNumBankedHearts);
        sNumHearts = PlayerPrefs.GetInt("NumHearts", sNumHearts);
        sCurrentFloor = PlayerPrefs.GetInt("CurrentFloor", sCurrentFloor);
        sNumAmmo = PlayerPrefs.GetInt("NumAmmo", sNumAmmo);
        sMaxAmmo = PlayerPrefs.GetInt("MaxAmmo", sMaxAmmo);

        for(int i = 0, n = (int)ItemId.NumItems; i < n; ++i)
        {
            ItemId itemId = (ItemId)i;
            if(PlayerPrefs.GetInt(string.Format("Item_{0}", itemId.ToString()), 0) > 0)
            {
                sCollectedItems.Add(itemId);
            }
        }

        for (int i = 0, n = (int)BossId.NumBosses; i < n; ++i)
        {
            BossId bossId = (BossId)i;
            if(PlayerPrefs.GetInt(string.Format("Boss_{0}", bossId.ToString()), 0) > 0)
            {
                sBossesDefeated.Add(bossId);
            }
        }

        sActiveBoss = (BossId)PlayerPrefs.GetInt("ActiveBoss", (int)sActiveBoss);

        sGameLoaded = true;
    }
}

public enum SelectedCharacter
{
    None = 0,
    Rose,
    Vu,
}