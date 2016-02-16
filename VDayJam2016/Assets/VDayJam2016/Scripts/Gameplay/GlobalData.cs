﻿using UnityEngine;
using System.Collections;
using System;

public static class GlobalData
{
    public static SelectedCharacter sSelectedCharacter = SelectedCharacter.None;

    public static int TotalHearts { get { return sNumBankedHearts + sNumHearts; } }
    public static int sNumBankedHearts = 0;
    private static int sNumHearts = 1;
    public static int NumHearts
    {
        get { return sNumHearts; }
        set { sNumHearts = value; Signal.Dispatch(SignalType.HeartAmountChanged); }
    }

    public static int sCurrentFloor = 1;

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

    public static bool sbHasTranslator = false;

    public static void ResetData()
    {
        sNumBankedHearts = 0;
        sNumHearts = 1;
        sCurrentFloor = 1;
        sNumAmmo = 10;
        sMaxAmmo = 10;

        sbHasTranslator = false;
    }
}

public enum SelectedCharacter
{
    None,
    Rose,
    Vu,
}