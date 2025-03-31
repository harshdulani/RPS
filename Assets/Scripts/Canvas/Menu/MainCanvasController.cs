using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour
{
	[SerializeField] private Image blackBG;
	[SerializeField] private TextMeshProUGUI fpsText;
	[SerializeField, Header("(PreGame) UI to disable when starting to play")] private Image playButtonImage;
	[SerializeField] private TextMeshProUGUI playButtonLabel;
	[SerializeField, Range(0.25f, 2f)] private float playBeginFadeDuration = 1f;

	[SerializeField, Header("(MidGame) Radial Move Selection Menu")] private GameObject radialMoveMenu;
	[SerializeField] private VerticalTimer timerL, timerR;
	
	// graphics that get enabled first
	[SerializeField] private Image radialMoveMenuBG;
	[SerializeField, Range(0.25f, 2f)] private float radialFadeDuration1 = 1f;
	
	// graphics that get enabled later
	[SerializeField] private List<Graphic> radialMoveMenuGraphics2;
	[SerializeField, Range(0.25f, 2f)] private float radialFadeDuration2 = 1f;
	private bool bRadialMenuHidden = false;

	[SerializeField] private float radialMenuHidePosY = -315f;
	[SerializeField] private TextMeshProUGUI playedMoveLabel;
	
	[SerializeField, Header("(PostGame) UI to show after move is over")] private Image retryButtonImage;
	[SerializeField] private TextMeshProUGUI retryButtonText;
	[SerializeField] private TextMeshProUGUI exclamationText;
	[SerializeField] private TextMeshProUGUI winResultText;
	[SerializeField, Range(0.25f, 2f)] private float exclamationAppearDuration = 1f;
	[SerializeField, Range(0.25f, 2f)] private float exclamationFadeDuration = 1f;
	[SerializeField, Range(0.25f, 2f)] private float exclamationWaitDuration = 1f;
	[SerializeField] public Color winColor;
	[SerializeField] public Color drawColor;
	[SerializeField] public Color loseColor;
	private float _initRetryButtonPosY;
	
	[SerializeField, Header("Score UI")] private TextMeshProUGUI scoreText;
	private int _winResult = -100;
	
	[SerializeField, Header("Best of 3")] private TextMeshProUGUI bestOf3ScoreText;
	
	private void OnEnable()
	{
		GameEvents.Singleton.PlayerMoveSelected += OnPlayerMoveSelected;
		GameEvents.Singleton.MoveEnded += OnMoveEnd;
	}
	private void OnDisable()
	{
		GameEvents.Singleton.PlayerMoveSelected -= OnPlayerMoveSelected;
		GameEvents.Singleton.MoveEnded -= OnMoveEnd;

		// kill any running tweens - currently only tagged sequences
		// if there are tweens that run individually, might need to Tween.SetId(this); for them too
		DOTween.Kill(this);
	}
	
	private void Awake()
	{
		// remove 30fps cap on mobiles
		Application.targetFrameRate = 90;
		UpdateScoreText();
	}
	
	private float _deltaTime = 0.0f;
	private void Update()
	{
		if (fpsText && fpsText.gameObject.activeSelf)
		{
			_deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
			float fps = 1.0f / _deltaTime;
			fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";
		}
	}

	private void UpdateScoreText()
	{
		scoreText.text = "Score: " + ScoreBoard.GetCurrentScore() + "\nBest: " + ScoreBoard.GetBestScore();
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
			                        {
				                        var graphicColor = radialMoveMenuBG.color;
				                        radialMoveMenuBG.color = new Color(graphicColor.r, graphicColor.g, graphicColor.b, 0f);
				                        radialMoveMenuBG.transform.localScale *= 0f;
			                        }
			                        foreach (var graphic in radialMoveMenuGraphics2)
			                        {
				                        var graphicColor = graphic.color;
				                        graphic.color = new Color(graphicColor.r, graphicColor.g, graphicColor.b, 0f);
				                        graphic.transform.localScale *= 0f;
			                        }
			                        
			                        timerL.gameObject.SetActive(true);
			                        timerL.SetColor(loseColor);
			                        timerL.StartTimer();
			                        
			                        timerR.gameObject.SetActive(true);
			                        timerR.SetColor(loseColor);
			                        timerR.StartTimer();

			                        GameEvents.Singleton.AnnounceRoundStart();
		                        });

		sequence.Insert(currentTimerDuration, radialMoveMenuBG.DOFade(1f, radialFadeDuration1));
		sequence.Insert(currentTimerDuration, radialMoveMenuBG.transform.DOScale(1f, radialFadeDuration1).SetEase(Ease.OutElastic));
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
	private void OnPlayerMoveSelected(SO_GameMove move)
	{
		bRadialMenuHidden = true;
		foreach (var graphic in radialMoveMenuGraphics2)
		{
			graphic.DOFade(0f, radialFadeDuration2).OnComplete(() => graphic.gameObject.SetActive(false));
			graphic.transform.DOScale(0f, radialFadeDuration2).SetEase(Ease.InElastic);
		}

		playedMoveLabel.text = move.moveType.ToString();
		radialMoveMenuBG.rectTransform.DOAnchorPosY(radialMenuHidePosY, 1f);
	}

	private void OnMoveEnd(int winResult, string exclamation)
	{
		_winResult = winResult;
		
		if (!bRadialMenuHidden)
		{
			bRadialMenuHidden = true;
			
			radialMoveMenuBG.DOFade(0f, radialFadeDuration1);
			radialMoveMenuBG.transform.DOScale(0f, radialFadeDuration1).SetEase(Ease.OutElastic);
			foreach (var graphic in radialMoveMenuGraphics2)
			{
				graphic.DOFade(0f, radialFadeDuration2);
				graphic.transform.DOScale(0f, radialFadeDuration2).SetEase(Ease.OutElastic);
			}
		}
		exclamationText.gameObject.SetActive(true);
		exclamationText.transform.localScale = Vector3.zero;
		exclamationText.text = exclamation;
		winResultText.gameObject.SetActive(true);
		winResultText.transform.localScale = Vector3.zero;
		winResultText.text = winResult switch
		                     {
			                     -1 => // player defeat
				                     "<color=#" + ColorUtility.ToHtmlStringRGB(loseColor) +">You Lose!</color>",
			                     0 => // draw
				                     "<color=#" + ColorUtility.ToHtmlStringRGB(drawColor) +">Draw!</color>",
			                     1 => // player victory
				                     "<color=#" + ColorUtility.ToHtmlStringRGB(winColor) +">You Win!</color>",
			                     _ => "" // default case
		                     };
		
		ScoreBoard.UpdateScores(_winResult);
		UpdateScoreText();
		
		var seq = DOTween.Sequence();
		seq.SetId(this);
		
		var seqLength = 0f;
		seq.Insert(0f, exclamationText.transform.DOScale(Vector3.one, exclamationAppearDuration).SetEase(Ease.OutCirc));
		seqLength += exclamationAppearDuration;

		seq.AppendInterval(exclamationWaitDuration);
		seqLength += exclamationWaitDuration;
		seq.AppendCallback(() =>
		                   {
			                   retryButtonImage.gameObject.SetActive(true);
			                   {
				                   var color = retryButtonImage.color;
				                   color.a = 0f;
				                   retryButtonImage.color = color;
			                   }
			                   {
				                   var color = retryButtonText.color;
				                   color.a = 0f;
				                   retryButtonText.color = color;
			                   }

			                   if (AudioManager.instance)
			                   {
				                   if (winResult == 1)
					                   AudioManager.instance.Play("Win");
				                   else if (winResult == -1)
					                   AudioManager.instance.Play("Lose");
			                   }
		                   });
		seq.Insert(seqLength, winResultText.rectTransform.DOScale(Vector3.one, exclamationAppearDuration).SetEase(Ease.OutCirc));
		seq.Insert(seqLength, radialMoveMenuBG.rectTransform.DOAnchorPosY(radialMenuHidePosY * 2f, exclamationAppearDuration).SetEase(Ease.OutCirc));
		seq.Insert(seqLength, retryButtonImage.DOFade(1f, exclamationAppearDuration));
		seq.Insert(seqLength, retryButtonText.DOFade(1f, exclamationAppearDuration));
		seqLength += exclamationAppearDuration;

		seq.Insert(seqLength, exclamationText.transform.DOScale(Vector3.zero, exclamationFadeDuration));
		seq.Insert(seqLength, exclamationText.DOFade(0f, exclamationFadeDuration));
		seqLength += exclamationFadeDuration;
	}
	#endregion
}
