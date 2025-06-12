using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum StateUI
    {
        None,
        CombatLogExtended,
        Pause,
        Victory,
        Graveyard,
        Mulligan,
        Tutorial
    }
    
    public static StateUI CurrentStateUI = StateUI.None;
}
