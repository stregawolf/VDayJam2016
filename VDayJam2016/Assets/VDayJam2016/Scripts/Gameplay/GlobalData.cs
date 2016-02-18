using UnityEngine;
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
        Translator,
        BankKey,
        GiftRing,
        GiftTech,
        GiftFlowers,
        GiftChocolate,
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
        None,
        Flower,
        Chocolate,
        Imposter,
    }
    private static HashSet<BossId> sBossesDefeated = new HashSet<BossId>();
    public static void SetBossDefeated(BossId id)
    {
        sBossesDefeated.Add(id);
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
        set { sActiveBoss = value; Signal.Dispatch(SignalType.OnFloorChanged); }
    }

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
    }
}

public enum SelectedCharacter
{
    None,
    Rose,
    Vu,
}