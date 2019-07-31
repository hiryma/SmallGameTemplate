#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Kayac
{
	/*
	https://github.com/BenoitFreslon/Vibration
	のラッパー。
	無効化/有効化機能と、
	機種依存関数の除去を行っている。
	元の奴はEditorでもjavaを読もうとして死るので、そこのケアもしている
	*/
	public class Vibrator
	{
		public static Vibrator Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Vibrator();
				}
				return instance;
			}
		}
		public bool Enabled{ get; private set; }
		public int AndroidWeakDuration{ get; set; }
		public int AndroidStrongDuration{ get; set; }
		public bool IosOldVibratorReplacementEnabled { get; set; }

		// 300ms揺らす。Handheld.Vibrateと同等
		public void VibrateDefault()
		{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
			Vibration.Vibrate();
#else
			VibrateUnityApi();
#endif
		}

		public void VibrateWeak()
		{
#if !UNITY_EDITOR
#if UNITY_ANDROID
			Vibration.Vibrate(AndroidWeakDuration);
#elif UNITY_IOS
			if (!iosHasOldVibrator)
			{
				Vibration.VibratePop();
			}
			else if (IosOldVibratorReplacementEnabled)
			{
UnityEngine.Debug.Log("iOS OldVibration replacement.");
				Vibration.VibrateOldShort();
			}
#endif
#endif
		}

		public void VibrateStrong()
		{
#if !UNITY_EDITOR
#if UNITY_ANDROID
			Vibration.Vibrate(AndroidStrongDuration);
#elif UNITY_IOS
			if (!iosHasOldVibrator)
			{
				Vibration.VibratePeek();
			}
			else if (IosOldVibratorReplacementEnabled)
			{
UnityEngine.Debug.Log("iOS OldVibration replacement.");
				Vibration.VibrateOldShort();
			}
#endif
#endif
		}

		public void VibrateUnityApi()
		{
			if (UnityEngine.SystemInfo.supportsVibration)
			{
#if !UNITY_WEBGL
				UnityEngine.Handheld.Vibrate();
#endif
			}
		}

		// non public -------------
		static Vibrator instance;
		long[] androidWeak3Pattern = new long[6];
		bool iosHasOldVibrator;

		Vibrator()
		{
			Enabled = true;
			// これら3つは様子を見て調整する。 TODO: 実機で実行してみないとわからない
			AndroidWeakDuration = 50;
			AndroidStrongDuration = 400;
#if UNITY_IOS
			IosOldVibratorReplacementEnabled = true;
			var gen = Device.generation;
			iosHasOldVibrator = (gen == DeviceGeneration.iPhone)
				|| (gen == DeviceGeneration.iPhone3G)
				|| (gen == DeviceGeneration.iPhone3GS)
				|| (gen == DeviceGeneration.iPhone4)
				|| (gen == DeviceGeneration.iPhone4S)
				|| (gen == DeviceGeneration.iPhone5)
				|| (gen == DeviceGeneration.iPhone5C)
				|| (gen == DeviceGeneration.iPhone5S)
				|| (gen == DeviceGeneration.iPhone6)
				|| (gen == DeviceGeneration.iPhone6Plus)
				|| (gen == DeviceGeneration.iPhoneSE1Gen);
			UnityEngine.Debug.Log("iOS Vibration: Old=" + iosHasOldVibrator);
#endif
		}
	}
}