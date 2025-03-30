using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VerticalTimer : MonoBehaviour
{
	[SerializeField, Range(1f, 5f)] private float duration;
	[SerializeField] private Vector2 widthRange;
	private Image _image;
	private Color _timerColor;
	bool bSelectionSubscribed = false;
	
	private void OnEnable()
	{
		GameEvents.Singleton.PlayerMoveSelected += OnPlayerMoveSelected;
		bSelectionSubscribed = true;
	}
	private void OnDisable()
	{
		if (bSelectionSubscribed)
		{
			GameEvents.Singleton.PlayerMoveSelected -= OnPlayerMoveSelected;
			bSelectionSubscribed = false;
		}

		DOTween.Kill(this);
	}

	public void SetColor(Color timerColor)
	{
		_timerColor = timerColor;
	}

	public void StartTimer()
	{
		if (!_image)
			_image = GetComponent<Image>();
		
		_image.color = _timerColor;
		_image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthRange.x);
		
		var seq = DOTween.Sequence();
		seq.SetId(this);
		seq.Insert(0f, _image.DOColor(_timerColor, duration).SetEase(Ease.Linear));
		
		seq.Insert(0f, _image.DOFillAmount(0f, duration).SetEase(Ease.Linear));
		seq.Insert(0f,DOTween.To(() => _image.rectTransform.rect.width,
		                         x => _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x),
		                         widthRange.y, 
		                         duration).SetEase(Ease.Linear));
	}
	private void EndTimer()
	{
		DOTween.Kill(this);
		DOTween.To(() => _image.rectTransform.rect.width,
		                         x => _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x),
		                         0, 
		                         0.5f).SetEase(Ease.InBack).SetDelay(0.5f);
		//_image.fillAmount = 0f;
	}
	
	private void OnPlayerMoveSelected(SO_GameMove move)
	{
		// if player selects move before timer runs out
		EndTimer();
		
		GameEvents.Singleton.PlayerMoveSelected -= OnPlayerMoveSelected;
		bSelectionSubscribed = false;
	}
}
