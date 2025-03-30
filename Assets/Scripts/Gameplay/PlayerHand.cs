using System;
using UnityEngine;

public class PlayerHand : Hand
{
	[field: NonSerialized] public override bool IsPlayerHand { get; protected set; } = true;
    
    [SerializeField] private EnemyHand enemyHand;
    
    public void PlayMove(SO_GameMove move)
    {
        if (bMovePlayed) return;
		
        bMovePlayed = true;
        GameEvents.Singleton.AnnounceMovePlayed(move);
		
        anim.SetInteger(MoveType, move.GetMoveAnimatorIndex());
		
        SO_GameMove enemyMove = enemyHand.GetPlayedMove();
        int moveResult = SO_GameMove.CalculateMoveResult(move, enemyMove, out string exclamation);
		
        GameEvents.Singleton.AnnounceMoveEnded(moveResult, exclamation);
        
        Debug.Log("MoveResult: " + moveResult + ", " + exclamation + " ");
        
        // x secs delay
        // announce victory/ defeat
    }
}
