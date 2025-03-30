using System;
using DG.Tweening;
using UnityEngine;

public class PlayerHand : Hand
{
	[field: NonSerialized] public override bool IsPlayerHand { get; protected set; } = true;
    
    [SerializeField] private EnemyHand enemyHand;
    
    public void SelectPlayerMove(SO_GameMove move)
    {
        if (bMoveSelected) return;
        
        if (AudioManager.instance)
	        AudioManager.instance.Play("ButtonPress");
        
        bMoveSelected = true;
        MyMove = move;
        
        GameEvents.Singleton.AnnouncePlayerMoveSelected(move);
		
        anim.SetInteger(MoveType, move.GetMoveAnimatorIndex());
    }

    // When the wind up animations for playing the round are over
    public void OnDoneWithRoundAnimations()
    {
	    var playerMove = GetSelectedMove();
	    var enemyMove = enemyHand.GetSelectedMove();
	    if (!playerMove)
	    {
		    transform.DOLocalMoveY(-20f, 1f).SetEase(Ease.InBack);
		    GameEvents.Singleton.AnnounceMoveEnded(-1, "<color=#FF4044>You didn't select a move!</color>");
		    return;
	    }
	    int moveResult = SO_GameMove.EvaluateMoveResult(GetSelectedMove(),
	                                                     enemyHand.GetSelectedMove(),
	                                                     out string exclamation);
		
	    GameEvents.Singleton.AnnounceMoveEnded(moveResult, exclamation);
        
	    Debug.Log("MoveResult: " + moveResult + ", " + exclamation + " ");
    }
}
