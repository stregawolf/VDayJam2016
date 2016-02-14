using UnityEngine;
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

    public static void ResetData()
    {
        sNumBankedHearts = 0;
        sNumHearts = 1;
        sCurrentFloor = 1;
    }
}

public enum SelectedCharacter
{
    None,
    Rose,
    Vu,
}