using UnityEngine;
using System.Collections;

public static class GlobalData
{
    public static SelectedCharacter mSelectedCharacter = SelectedCharacter.None;
}

public enum SelectedCharacter
{
    None,
    Rose,
    Vu,
}