using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHand : Hand
{
    [SerializeField] private List<SO_GameMove> availableMoves;
    
    [SerializeField] private bool bHelpPlayer = true;
    [SerializeField, Range(0f, 1f)] private float cheatChance = 0.25f;
    [SerializeField] private PlayerHand playerHand;
    
    [field: NonSerialized] public override bool IsPlayerHand { get; protected set; } = false;

    [SerializeField, Range(0f, 1f)] private float LossStreakBreakChance = 0.6f;
    [SerializeField] private int MaxLossStreak = 3;
    
    private SO_GameMove GetMoveSelection()
    {
        if (!playerHand || !bHelpPlayer) return GetRandomMove();
        
        // help favor the player to win by increasing the low 40% win chance
        bool bRandomCheat = Random.Range(0f, 1f) < cheatChance;
        bool bBreakLossStreak = ScoreBoard.LossStreak > MaxLossStreak && Random.Range(0f, 1f) < LossStreakBreakChance;
        
        if (bRandomCheat || bBreakLossStreak)
        {
            var playerMove = playerHand.GetSelectedMove();
            var lossMove = playerMove.winConditions[Random.Range(0, playerMove.winConditions.Count)];
            foreach (var move in availableMoves)
            {
                if (lossMove.WinAgainst == move.moveType)
                {
                    return move;
                }
            }
        }
        
        return GetRandomMove();
    }

    private SO_GameMove GetRandomMove() => availableMoves[Random.Range(0, availableMoves.Count)];
    
    public override SO_GameMove GetSelectedMove()
    {
        MyMove = GetMoveSelection();
        anim.SetInteger(MoveType, MyMove.GetMoveAnimatorIndex());
        
        bMoveSelected = true;

        return MyMove;
    }
}