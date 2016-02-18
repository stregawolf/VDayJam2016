using UnityEngine;
using System;
using System.Collections.Generic;

public enum SignalType
{
    GameStart, // game officially started
    GameEnd, // game is over win state
    LevelStart, // dungeon finished loading
    LevelComplete, // player reached goal
    StartNextLevel,
    RestartLevel,
    HeartAmountChanged,
    AmmoAmountChanged,
    WeaponChanged,
    PlayerDeath,
    Unused,
    HeartBankUnlocked,
    OnFloorChanged,
    BossDefeated,
    FinalBossLevelCompleted,
    BankedHeartAmountChanged,
}

public class Signal
{
    private static Dictionary<SignalType, List<Action>> sSignalMapping = new Dictionary<SignalType, List<Action>>();

    public static void Register(SignalType signalType, Action callback)
    {
        List<Action> callbacks;
        if(!sSignalMapping.TryGetValue(signalType, out callbacks) || callbacks == null)
        {
            callbacks = new List<Action>();
            sSignalMapping[signalType] = callbacks;
        }
        if (!callbacks.Contains(callback))
        {
            callbacks.Add(callback);
        }
    }

    public static void Unregister(SignalType signalType, Action callback)
    {
        List<Action> callbacks;
        if (sSignalMapping.TryGetValue(signalType, out callbacks) && callbacks != null)
        {
            callbacks.Remove(callback);
        }
    }

    public static void Dispatch(SignalType signalType)
    {
        List<Action> callbacks;
        if (sSignalMapping.TryGetValue(signalType, out callbacks) && callbacks != null)
        {
            for (int i = 0, n = callbacks.Count; i < n; ++i)
            {
                callbacks[i]();
            }
        }
    }
}