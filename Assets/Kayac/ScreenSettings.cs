using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Kayac
{
	public class ScreenSettings
	{
		class Preset
		{
			public Preset(string name, int width, int height, int marginLeft, int marginRight, int marginTop, int marginBottom)
			{
				this.name = name;
				this.width = width;
				this.height = height;
				this.marginLeft = marginLeft;
				this.marginRight = marginRight;
				this.marginTop = marginTop;
				this.marginBottom = marginBottom;
			}
			public string name;
			public int width;
			public int height;
			public int marginLeft;
			public int marginRight;
			public int marginTop;
			public int marginBottom;
		}
		public static ScreenSettings instance;
		public static ScreenSettings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ScreenSettings();
				}
				return instance;
			}
		}
		public Rect SafeArea { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public float Dpi { get; private set; }
		// スクリーン上の実際のRectを返す
		public Rect ActualScreenRect
		{
			get
			{
				var ret = new Rect();
				var realAspect = (float)Screen.width / (float)Screen.height;
				var requestAspect = (float)Width / (float)Height;
				if (realAspect > requestAspect) // 縦合わせ、横スペース
				{
					ret.y = 0f;
					ret.height = (float)Screen.height;

					ret.width = (float)Screen.width * (requestAspect / realAspect);
					ret.x = ((float)Screen.width - ret.width) * 0.5f;
				}
				else
				{
					ret.x = 0f;
					ret.width = (float)Screen.width;

					ret.height = (float)Screen.height * (realAspect / requestAspect);
					ret.y = ((float)Screen.height - ret.height) * 0.5f;
				}
				return ret;
			}
		}

		// non-public
		ScreenSettings()
		{
			SafeArea = Screen.safeArea;
			Width = Screen.width;
			Height = Screen.height;
			Dpi = Screen.dpi;
			TryLoad();
		}

		public static void Load(
			out int width,
			out int height,
			out int marginLeft,
			out int marginRight,
			out int marginTop,
			out int marginBottom)
		{
			width = PlayerPrefs.GetInt("Kayac/ScreenSettingsWindow.Width");
			height = PlayerPrefs.GetInt("Kayac/ScreenSettingsWindow.Height");
			marginLeft = PlayerPrefs.GetInt("Kayac/ScreenSettingsWindow.MarginLeft");
			marginRight = PlayerPrefs.GetInt("Kayac/ScreenSettingsWindow.MarginRight");
			marginTop = PlayerPrefs.GetInt("Kayac/ScreenSettingsWindow.MarginTop");
			marginBottom = PlayerPrefs.GetInt("Kayac/ScreenSettingsWindow.MarginBottom");
		}

		public static void Save(
			int width,
			int height,
			int marginLeft,
			int marginRight,
			int marginTop,
			int marginBottom)
		{
			PlayerPrefs.SetInt("Kayac/ScreenSettingsWindow.Width", width);
			PlayerPrefs.SetInt("Kayac/ScreenSettingsWindow.Height", height);
			PlayerPrefs.SetInt("Kayac/ScreenSettingsWindow.MarginLeft", marginLeft);
			PlayerPrefs.SetInt("Kayac/ScreenSettingsWindow.MarginRight", marginRight);
			PlayerPrefs.SetInt("Kayac/ScreenSettingsWindow.MarginTop", marginTop);
			PlayerPrefs.SetInt("Kayac/ScreenSettingsWindow.MarginBottom", marginBottom);
		}

		public static void SaveDefault()
		{
			Save(0, 0, 0, 0, 0, 0);
		}

		void TryLoad()
		{
			int width, height, marginLeft, marginRight, marginTop, marginBottom;
			Load(out width, out height, out marginLeft, out marginRight, out marginTop, out marginBottom);
			if ((width <= 0) || (height <= 0))
			{
				return;
			}
			Width = width;
			Height = height;
			var w = (float)Width;
			var h = (float)Height;
			SafeArea = new Rect(
				(float)marginLeft,
				(float)marginTop,
				w - (float)marginLeft - (float)marginRight,
				h - (float)marginTop - (float)marginBottom);

			// メインカメラのviewportをいじる
			var mainCamera = Camera.main;
			var realAspect = (float)Screen.width / (float)Screen.height;
			var requestAspect = w / h;
			if (realAspect > requestAspect) // 縦合わせ、横スペース
			{
				var innerW = requestAspect / realAspect;
				mainCamera.rect = new Rect(
					(1f - innerW) * 0.5f,
					0f,
					innerW,
					1f);
			}
			else // 横合わせ、縦スペース
			{
				var innerH = realAspect / requestAspect;
				mainCamera.rect = new Rect(
					0f,
					(1f - innerH) * 0.5f,
					1f,
					innerH);
			}
		}

		public bool HasMargin
		{
			get
			{
				return (Width != Screen.width) || (Height != Screen.height);
			}
		}

#if UNITY_EDITOR

		public class ScreenSettingsWindow : EditorWindow
		{
			[MenuItem("Kayac/ScreenSettings")]
			static void Create()
			{
				GetWindow<ScreenSettingsWindow>(typeof(ScreenSettingsWindow).Name);
			}

			List<Preset> presets;
			void InitializePresets()
			{
				presets = new List<Preset>();
				presets.Add(new Preset("Reset", 0, 0, 0, 0, 0, 0));

				presets.Add(new Preset("iPhoneX Portrait", 1125, 2436, 0, 0, 132, 102));
				presets.Add(new Preset("iPhoneXr Portrait", 828, 1792, 0, 0, 88, 68));
				presets.Add(new Preset("iPhoneXs Max Portrait", 1242, 2688, 0, 0, 132, 102));
				presets.Add(new Preset("iPad Pro 12.9'' Portrait", 2048, 2732, 0, 0, 0, 0));
				presets.Add(new Preset("16:9 Portrait", 1080, 1920, 0, 0, 0, 0));
				presets.Add(new Preset("4:3 Portrait", 1536, 2048, 0, 0, 0, 0));

				presets.Add(new Preset("iPhoneX Landscape", 2436, 1125, 132, 132, 0, 63));
				presets.Add(new Preset("iPhoneXr Landscape", 1792, 828, 88, 88, 0, 42));
				presets.Add(new Preset("iPhoneXs Max Landscape", 2688, 1242, 132, 132, 0, 63));
				presets.Add(new Preset("iPad Pro 12.9'' Landscape", 2732, 2048, 0, 0, 0, 0));
				presets.Add(new Preset("16:9 Landscape", 1920, 1080, 0, 0, 0, 0));
				presets.Add(new Preset("4:3 Landscape", 2048, 1536, 0, 0, 0, 0));
			}

			void OnGUI()
			{
				if (presets == null)
				{
					InitializePresets();
				}
				bool changed = false;
				int width, height, marginLeft, marginRight, marginTop, marginBottom;
				Load(out width, out height, out marginLeft, out marginRight, out marginTop, out marginBottom);

				var newWidth = EditorGUILayout.IntField("Width", width);
				var newHeight = EditorGUILayout.IntField("Height", height);
				var newMarginLeft = EditorGUILayout.IntField("MarginLeft", marginLeft);
				var newMarginRight = EditorGUILayout.IntField("MarginRight", marginRight);
				var newMarginTop = EditorGUILayout.IntField("MarginTop", marginTop);
				var newMarginBottom = EditorGUILayout.IntField("MarginBottom", marginBottom);
				if ((newWidth != width)
					|| (newHeight != height)
					|| (newMarginLeft != marginLeft)
					|| (newMarginRight != marginRight)
					|| (newMarginTop != marginTop)
					|| (newMarginBottom != marginBottom))
				{
					width = newWidth;
					height = newHeight;
					marginLeft = newMarginLeft;
					marginRight = newMarginRight;
					marginTop = newMarginTop;
					marginBottom = newMarginBottom;
					changed = true;
				}

				foreach (var preset in presets)
				{
					if (GUILayout.Button(preset.name))
					{
						changed = true;
						width = preset.width;
						height = preset.height;
						marginLeft = preset.marginLeft;
						marginRight = preset.marginRight;
						marginTop = preset.marginTop;
						marginBottom = preset.marginBottom;
					}
				}

				if (changed)
				{
					ScreenSettings.Save(width, height, marginLeft, marginRight, marginTop, marginBottom);
				}
			}
		}
#endif
	}

}