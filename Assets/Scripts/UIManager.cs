using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class UIManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Image _displayImage;
	[SerializeField] private TMP_Text _captionText;
	[SerializeField] private TMP_Text _counterText;
	[SerializeField] private Button _nextButton;
	[SerializeField] private Button _prevButton;

	[Inject] private SolarActivityGallery _gallery;

	private void Start()
	{
		_nextButton.onClick.AddListener(OnNextClicked);
		_prevButton.onClick.AddListener(OnPrevClicked);
		UpdateUI();
	}

	/// <summary>
	/// Обновление UI
	/// </summary>
	public void UpdateUI()
	{
		var texture = _gallery.GetCurrentTexture();

		if (texture != null)
		{
			_displayImage.sprite = Sprite.Create(
				texture,
				new Rect(0, 0, texture.width, texture.height),
				new Vector2(0.5f, 0.5f)
			);
			_captionText.text = _gallery.GetCurrentImageName();
			_counterText.text = $"{_gallery.CurrentIndex + 1}/{_gallery.ImageCount}";
		}
		else
		{
			ClearUI();
		}
	}

	/// <summary>
	/// Если нет фотографий
	/// </summary>
	private void ClearUI()
	{
		_displayImage.sprite = null;
		_captionText.text = "No images";
		_counterText.text = "0/0";
	}

	/// <summary>
	/// Событие переключение на следующую картинку
	/// </summary>
	private void OnNextClicked()
	{
		_gallery.ShowNextImage();
		UpdateUI();
	}

	/// <summary>
	/// Событие переключение на предыдущую картинку
	/// </summary>
	private void OnPrevClicked()
	{
		_gallery.ShowPreviousImage();
		UpdateUI();
	}
}