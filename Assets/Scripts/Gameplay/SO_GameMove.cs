using System;
using System.Collections.Generic;
using ToonyColorsPro.ShaderGenerator;
using UnityEngine;

public enum EMoveType
{
	None,
	Rock,
	Paper,
	Scissors,
	Lizard,
	Spock
}

[Serializable]
public struct WinCondition
{
	[field: SerializeField] public string WinString { get; private set; }
	[field: SerializeField] public EMoveType WinAgainst{ get; private set; }
}

[CreateAssetMenu(fileName = "GameMove", menuName = "ScriptableObjects/GameMove", order = 1)]
public class SO_GameMove : ScriptableObject
{
	public EMoveType moveType = EMoveType.None;

	public List<WinCondition> winConditions;

	/*
	 * 0 = None
	 * 1 = Rock
	 * 2 = Paper
	 * 3 = Scissor
	 * 4 = Lizard
	 * 5 = Spock
	 */
	public int GetMoveAnimatorIndex() => (int)moveType;

	// returns 1 if player won the move, -1 if enemy, 0 if draw
	public static int CalculateMoveResult(SO_GameMove player, SO_GameMove enemy, out string exclamation)
	{
		if (player.moveType == enemy.moveType)
		{
			exclamation = "Draw";
			return 0;
		}
		
		foreach (var condition in player.winConditions)
		{
			if (condition.WinAgainst != enemy.moveType) continue;
			
			exclamation = player.moveType + " " + condition.WinString + " " + enemy.moveType + "!";
			return 1;
		}
		
		foreach (var condition in enemy.winConditions)
		{
			if (condition.WinAgainst != player.moveType) continue;
			
			exclamation = enemy.moveType + " " + condition.WinString + " " + player.moveType + "!";
			return -1;
		}
		
		Debug.LogWarning("Execution shouldn't be here.");
		exclamation = "error";
		return 0;
	}
}
