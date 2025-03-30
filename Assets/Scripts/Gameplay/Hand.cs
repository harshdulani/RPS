using System;
using UnityEngine;

public abstract class Hand : MonoBehaviour
{
	[field: NonSerialized] public virtual bool IsPlayerHand { get; protected set; } = false;
	
	protected SO_GameMove MyMove = null;
	protected bool bMovePlayed = false; 
	
	protected Animator anim;
	
	protected static readonly int StartPreRound = Animator.StringToHash("startPreRound");
	protected static readonly int MoveType = Animator.StringToHash("moveType");

	private void OnEnable()
	{
		GameEvents.Singleton.RoundStart += OnUIReady;
	}

	private void OnDisable()
	{
		GameEvents.Singleton.RoundStart -= OnUIReady;
	}

	private void Awake()
	{
		anim = GetComponent<Animator>();
	}

	// May be null if move is not played (on player)/ assigned (on enemy) yet
	public virtual SO_GameMove GetPlayedMove()
	{
		return MyMove;
	}
	
	private void OnUIReady()
	{
		anim.SetTrigger(StartPreRound);
		
		// start timer 5s
		
	}
}
