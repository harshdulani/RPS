using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Separate singleton for all game flow events
    // so that any class who cares about these can just know about this class
    // and not whichever class advances the game flow - decoupling
    public static GameEvents Singleton;

    #region Events
    public Action RoundStart;
    public Action<SO_GameMove> PlayerMoveSelected;
    public Action<int, string> MoveEnded;

    #endregion
    
    private void Awake()
    {
        if (!Singleton) Singleton = this;
        else Destroy(gameObject);
    }
    
    #region Event Invocations
    public void AnnounceRoundStart()
    {
        RoundStart?.Invoke();
    }

    public void AnnouncePlayerMoveSelected(SO_GameMove move)
    {
        PlayerMoveSelected?.Invoke(move);
    }

    public void AnnounceMoveEnded(int moveResult, string exclamation)
    {
        MoveEnded?.Invoke(moveResult, exclamation);
    }
    
    #endregion
}
