using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Zenject;




#if UNITY_EDITOR
using UnityEditor;
#endif

public class SolarActivityGallery : MonoBehaviour
{
	[System.Serializable]
	public class ImageData
	{
		public string name;

		// Текстура подгружается при необходимости
		[System.NonSerialized]
		public Texture2D texture;
		[HideInInspector]
		public string path;
	}

	public SolarActivityGallery()
    {
    }

    [Header("Image Settings")]
	[SerializeField] private List<ImageData> _images = new List<ImageData>();

	[HideInInspector]
	public int CurrentIndex = 0;
	[HideInInspector]
	public int ImageCount => _images.Count;

	private Dictionary<string, Texture2D> _loadedTextures = new Dictionary<string, Texture2D>();

	private const int MAX_CACHED_TEXTURES = 5;
	private Queue<string> _textureCacheQueue = new Queue<string>();

#if UNITY_EDITOR
	[CustomEditor(typeof(SolarActivityGallery))]
	public class SolarActivityGalleryEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			SolarActivityGallery gallery = (SolarActivityGallery)target;
			if (GUILayout.Button("Загрузить изображения"))
			{
				SelectMultipleFiles(gallery);
			}

			if (GUILayout.Button("Загрузить папку"))
			{
				SelectFolder(gallery);
			}

			if (GUILayout.Button("Очистить все"))
			{
				ClearImages(gallery);
			}
		}


		/// <summary>
		/// Выбрать несколько файлов
		/// </summary>
		/// <param name="gallery"></param>
		private void SelectMultipleFiles(SolarActivityGallery gallery)
		{
			//// Открываем диалог с мультивыбором
			string path = EditorUtility.OpenFilePanelWithFilters("Выберите изображения", "", new string[] { "Images", "png,jpg,jpeg" });

			if (!string.IsNullOrEmpty(path))
			{
				AddImage(gallery, path);
			}
		}

		/// <summary>
		/// Выбрать папку для выбора папки
		/// </summary>
		/// <param name="gallery"></param>
		private void SelectFolder(SolarActivityGallery gallery)
		{
			//// Открываем диалог с выбором папки
			string folderPath = EditorUtility.OpenFolderPanel("Выберите папку", "", "");

			if (!string.IsNullOrEmpty(folderPath))
			{
				string[] extensions = { "*.jpg", "*.png", "*.jpeg" };
				foreach (string ext in extensions)
				{
					foreach (string filePath in Directory.GetFiles(folderPath, ext))
					{
						AddImage(gallery, filePath);
					}
				}
			}
		}

		/// <summary>
		/// Почистить список изображений
		/// </summary>
		/// <param name="gallery"></param>
		private void ClearImages(SolarActivityGallery gallery)
		{
			gallery._images.Clear();
		}
		/// <summary>
		/// Загрузить картинку
		/// </summary>
		/// <param name="path">Путь к картинке</param>
		public void AddImage(SolarActivityGallery gallery, string path)
		{
			if (!File.Exists(path)) return;

			gallery._images.Add(new ImageData
			{
				name = Path.GetFileName(path),
				path = path,
				texture = null
			});
		}
	}

#endif


	/// <summary>
	/// Вернуть текущую текстуру
	/// </summary>
	/// <returns>Текущая текстура</returns>
	public Texture2D GetCurrentTexture()
	{
		if (_images.Count == 0 || CurrentIndex < 0 || CurrentIndex >= _images.Count)
			return null;

		var image = _images[CurrentIndex];
		if (image.texture == null)
		{
			image.texture = LoadTexture(image.path);
			ManageTextureCache(image.path);
		}
		return image.texture;
	}

	/// <summary>
	/// Название картинки
	/// </summary>
	/// <returns></returns>
	public string GetCurrentImageName() =>
		_images.Count > 0 ? _images[CurrentIndex].name : string.Empty;

	/// <summary>
	/// Показать следующую картинку
	/// </summary>
	public void ShowNextImage()
	{
		if (_images.Count == 0) return;
		CurrentIndex = (CurrentIndex + 1) % _images.Count;
	}

	/// <summary>
	/// Показать предыдущую картинку
	/// </summary>
	public void ShowPreviousImage()
	{
		if (_images.Count == 0) return;
		CurrentIndex = (CurrentIndex - 1 + _images.Count) % _images.Count;
	}

	/// <summary>
	/// Загрузить текстуру
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	private Texture2D LoadTexture(string path)
	{
		if (!File.Exists(path)) return null;

		if (!_loadedTextures.TryGetValue(path, out var texture))
		{
			try
			{
				byte[] fileData = File.ReadAllBytes(path);
				texture = new Texture2D(2, 2);
				texture.LoadImage(fileData);
				_loadedTextures[path] = texture;
			}
			catch (System.Exception e)
			{
				Debug.LogError($"Ошибка загрузки {path}: {e.Message}");
				return null;
			}
		}
		return texture;
	}

	/// <summary>
	/// Кэшировать текстуру
	/// </summary>
	/// <param name="newTexturePath">Путь к картинке</param>
	private void ManageTextureCache(string newTexturePath)
	{
		if (_loadedTextures.Count >= MAX_CACHED_TEXTURES)
		{
			var oldest = _textureCacheQueue.Dequeue();
			Destroy(_loadedTextures[oldest]);
			_loadedTextures.Remove(oldest);
		}
		_textureCacheQueue.Enqueue(newTexturePath);
	}

	/// <summary>
	/// Очистка всех картинок
	/// </summary>
	public void ClearAll()
	{
		_images.Clear();
		ClearTextureCache();
		CurrentIndex = 0;
	}

	/// <summary>
	/// Очистить кэш
	/// </summary>
	private void ClearTextureCache()
	{
		foreach (var texture in _loadedTextures.Values)
		{
			Destroy(texture);
		}
		_loadedTextures.Clear();
		_textureCacheQueue.Clear();
		Resources.UnloadUnusedAssets();
	}

	private void OnDisable()
	{
		ClearAll();
	}
}
