using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class SwipeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField] private float _swipeThreshold = 50f; // Минимальная дистанция свайпа

	[Inject] private SolarActivityGallery _gallery;
	private Vector2 _startTouchPosition;

	// Начало свайпа
	public void OnBeginDrag(PointerEventData eventData)
	{
		// Фиксируем начало свайпа
		if (eventData.pointerPressRaycast.gameObject == gameObject)
			_startTouchPosition = eventData.position;
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.pointerPressRaycast.gameObject != gameObject)
			return;

		Vector2 endTouchPosition = eventData.position;
		float swipeDistance = endTouchPosition.x - _startTouchPosition.x;

		if (Mathf.Abs(swipeDistance) > _swipeThreshold)
		{
			if (swipeDistance > 0)
				_gallery.ShowPreviousImage(); // Свайп вправо → назад
			else
				_gallery.ShowNextImage(); // Свайп влево → вперед
		}
	}
}