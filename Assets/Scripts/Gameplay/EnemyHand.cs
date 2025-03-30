using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHand : Hand
{
    [SerializeField] private List<SO_GameMove> availableMoves;
    
    [field: NonSerialized] public override bool IsPlayerHand { get; protected set; } = false;

    public override SO_GameMove GetSelectedMove()
    {
        MyMove = availableMoves[Random.Range(0, availableMoves.Count)];
        anim.SetInteger(MoveType, MyMove.GetMoveAnimatorIndex());
        
        bMoveSelected = true;

        return MyMove;
    }
}