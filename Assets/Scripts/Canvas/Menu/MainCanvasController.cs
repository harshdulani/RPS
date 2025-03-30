using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour
{
	[SerializeField] private Image blackBG;
	[SerializeField, Header("(PreGame) UI to disable when starting to play")] private Image playButtonImage;
	[SerializeField] private TextMeshProUGUI playButtonLabel;
	[SerializeField, Range(0.25f, 2f)] private float playBeginFadeDuration = 1f;

	[SerializeField, Header("(MidGame) Radial Move Selection Menu")] private GameObject radialMoveMenu;
	// graphics that get enabled first
	[SerializeField] private List<Graphic> radialMoveMenuGraphics1;
	[SerializeField, Range(0.25f, 2f)] private float radialFadeDuration1 = 1f;
	
	// graphics that get enabled later
	[SerializeField] private List<Graphic> radialMoveMenuGraphics2;
	[SerializeField, Range(0.25f, 2f)] private float radialFadeDuration2 = 1f;

	[SerializeField] private RectTransform radialMoveMenuBackground;
	[SerializeField] private float radialMenuHidePosY = -315f;
	[SerializeField] private TextMeshProUGUI playedMoveLabel;
	
	[SerializeField, Header("(PostGame) UI to show after move is over")] private Image retryButtonImage;
	[SerializeField] private TextMeshProUGUI exclamationText;
	[SerializeField, Range(0.25f, 2f)] private float exclamationAppearDuration = 1f;
	[SerializeField, Range(0.25f, 2f)] private float exclamationFadeDuration = 1f;
	[SerializeField, Range(0.25f, 2f)] private float exclamationWaitDuration = 1f;
	private float _initRetryButtonPosY;

	private void OnEnable()
	{
		GameEvents.Singleton.MovePlayed += OnMovePlayed;
		GameEvents.Singleton.MoveEnded += OnMoveEnd;
	}
	private void OnDisable()
	{
		GameEvents.Singleton.MovePlayed -= OnMovePlayed;
		GameEvents.Singleton.MoveEnded -= OnMoveEnd;

		// kill any running tweens - currently only tagged sequences
		// if there are tweens that run individually, might need to Tween.SetId(this); for them too
		DOTween.Kill(this);
	}
	
	private void Awake()
	{
		_initRetryButtonPosY = retryButtonImage.rectTransform.anchoredPosition.y;
	}
	
	public void OnPlayButtonPressed()
	{
		if (AudioManager.instance)
		{
			AudioManager.instance.Play("ButtonPress");
		}
		float currentTimerDuration = 0f;

		Sequence sequence = DOTween.Sequence();
		sequence.SetId(this);
		
		// fade bg & fade button + its label to clear
		sequence.Insert(currentTimerDuration, blackBG.DOFade(0f, playBeginFadeDuration).OnComplete(() => blackBG.gameObject.SetActive(false)));
		sequence.Insert(currentTimerDuration, playButtonImage.DOFade(0f, playBeginFadeDuration).OnComplete(() => playButtonImage.gameObject.SetActive(false)));
		sequence.Insert(currentTimerDuration, playButtonImage.DOFade(0f, playBeginFadeDuration).OnComplete(() => playButtonImage.gameObject.SetActive(false)));
		sequence.Insert(currentTimerDuration, playButtonLabel.DOFade(0f, playBeginFadeDuration));
		currentTimerDuration += playBeginFadeDuration;
		
		sequence.AppendCallback(() =>
		                        {
			                        radialMoveMenu.SetActive(true);
			                        foreach (var graphic in radialMoveMenuGraphics1)
			                        {
				                        var graphicColor = graphic.color;
				                        graphic.color = new Color(graphicColor.r, graphicColor.g, graphicColor.b, 0f);
				                        graphic.transform.localScale *= 0f;
			                        }
			                        foreach (var graphic in radialMoveMenuGraphics2)
			                        {
				                        var graphicColor = graphic.color;
				                        graphic.color = new Color(graphicColor.r, graphicColor.g, graphicColor.b, 0f);
				                        graphic.transform.localScale *= 0f;
			                        }
		                        });
		
		sequence.AppendCallback(() => GameEvents.Singleton.AnnounceRoundStart());
		
		foreach (var graphic in radialMoveMenuGraphics1)
		{
			sequence.Insert(currentTimerDuration, graphic.DOFade(1f, radialFadeDuration1));
			sequence.Insert(currentTimerDuration, graphic.transform.DOScale(1f, radialFadeDuration1).SetEase(Ease.OutElastic));
		}
		currentTimerDuration += radialFadeDuration1 * 0.5f;
		foreach (var graphic in radialMoveMenuGraphics2)
		{
			sequence.Insert(currentTimerDuration, graphic.DOFade(1f, radialFadeDuration2));
			sequence.Insert(currentTimerDuration, graphic.transform.DOScale(1f, radialFadeDuration2).SetEase(Ease.OutElastic));
		}
	}

	public void RetryButtonPressed()
	{
		if (AudioManager.instance)
		{
			AudioManager.instance.Play("ButtonPress");
		}
		UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}

	#region Event Reactions
	private void OnMovePlayed(SO_GameMove move)
	{
		foreach (var graphic in radialMoveMenuGraphics2)
		{
			graphic.DOFade(0f, radialFadeDuration2).OnComplete(() => graphic.gameObject.SetActive(false));
			graphic.transform.DOScale(0f, radialFadeDuration2).SetEase(Ease.InElastic);
		}

		playedMoveLabel.text = move.moveType.ToString();
		radialMoveMenuBackground.DOAnchorPosY(radialMenuHidePosY, 1f);
	}

	private void OnMoveEnd(int winResult, string exclamation)
	{
		exclamationText.gameObject.SetActive(true);
		exclamationText.transform.localScale = Vector3.zero;
		exclamationText.text = exclamation;
		
		retryButtonImage.gameObject.SetActive(true);
		retryButtonImage.rectTransform.anchoredPosition = new Vector2(0f, 150f); // this will throw it off-screen since it is anchored from the top
		
		var seq = DOTween.Sequence();
		seq.SetId(this);
		
		seq.Insert(0f, retryButtonImage.rectTransform.DOAnchorPosY(_initRetryButtonPosY, 0.5f).SetEase(Ease.OutElastic));
		
		seq.Insert(0f, exclamationText.transform.DOScale(Vector3.one, exclamationAppearDuration).SetEase(Ease.OutCirc));
		var seqLength = exclamationAppearDuration;
		
		seq.AppendInterval(exclamationWaitDuration);
		seqLength += exclamationWaitDuration;

		seq.Insert(seqLength, exclamationText.transform.DOScale(Vector3.zero, exclamationFadeDuration));
		seq.Insert(seqLength, exclamationText.DOFade(0f, exclamationFadeDuration));
		seqLength += exclamationFadeDuration;
		
		seq.AppendCallback(() =>
		                   {
			                   exclamationText.transform.localScale = Vector3.zero;
			                   exclamationText.text = winResult switch
			                                          {
				                                          -1 => // player defeat
					                                          "You Lose!",
				                                          0 => // draw
					                                          "Draw!",
				                                          1 => // player victory
					                                          "You Win!",
				                                          _ => "" // default case
			                                          };
		                   });
		
		seq.Append(exclamationText.transform.DOScale(Vector3.one, exclamationFadeDuration));
		seq.Insert(seqLength, exclamationText.DOFade(1f, exclamationFadeDuration));
		seqLength += exclamationFadeDuration;
	}
	#endregion
}
