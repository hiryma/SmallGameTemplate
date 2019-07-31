using System.IO;
using System.Collections.Specialized;
using System.Collections;
using UnityEngine;
using Kayac;

public class DebugService
{
	public DebugUiLogWindow Log{ get; private set; }
	public delegate void OverrideFileChangedCallback(string path);
	public event OverrideFileChangedCallback OnOverrideFileChanged;

	DebugUiButton tapModeButton;
	DebugUiButton resoButton;
	DebugSlack slack;
	string slackKey = "任意の鍵";
	string slackEncodedToken = "slackApiTokenをDebugSlack.EncryptXorでエンコした文字列";

	public DebugService(
		World world,
		int serverPort,
		Camera camera,
		Shader textShader,
		Shader texturedShader,
		Font font,
		Sprite tapMark,
		string debugServerIndexHtml)
	{
		var decodedToken = DebugSlack.DecryptXor(slackKey, slackEncodedToken);
		if (decodedToken != null)
		{
			slack = new DebugSlack(decodedToken, "テキトーなチャンネル名");
		}
		// ログできるだけ速く欲しいので、こいつのコンストラクトだけ先にやる
		Log = new DebugUiLogWindow(12f, 380f, 700f, borderEnabled: true, captureUnityLog: true);
		Log.enabled = false;

		this.world = world;
		this.debugServerIndexHtml = debugServerIndexHtml;
		server = new DebugServer(serverPort, "/assets/", OnFileChanged);

		uiManager = DebugUiManager.Create(
			camera,
			textShader,
			texturedShader,
			font,
			432,
			768,
			1f,
			8192);
		uiManager.safeAreaVisualizationEnabled = true;
		var frameTimeGauge = new FrameTimeGauge(100f, 15f, null);
		uiManager.Add(frameTimeGauge, 0f, 0f, DebugUi.AlignX.Right, DebugUi.AlignY.Bottom);
		menu = new DebugUiMenu(100f, 40f, DebugUi.Direction.Down, "DebugMenu");
		uiManager.Add(menu);
		menu.AddItem("LogWindow", () =>
		{
			Log.enabled = !Log.enabled;
		});
		uiManager.Add(Log, 0, 40f, DebugUi.AlignX.Center, DebugUi.AlignY.Top);

		var dataSubMenu = new DebugUiSubMenu("Data", 100f, 40f, DebugUi.Direction.Down);
		menu.AddSubMenu(dataSubMenu, DebugUi.Direction.Right);
		dataSubMenu.AddItem("SendJsons", () =>
		{
			world.StartCoroutine(CoSendJsons());
		});
		dataSubMenu.AddItem("Reload\nGlobalParams", () =>
		{
			world.ReloadGlobalParams();
		});

		var debugSubMenu = new DebugUiSubMenu("Debug", 100f, 40f, DebugUi.Direction.Down);
		menu.AddSubMenu(debugSubMenu, DebugUi.Direction.Right);

		debugSubMenu.AddItem("HideDebug", () =>
		{
			ToggleUiEnabled();
		});

		debugSubMenu.AddItem("AutoTap", () =>
		{
			tapper.enabled = !tapper.enabled;
		});

		var resoSubMenu = new DebugUiSubMenu("Resolution", 100f, 40f, DebugUi.Direction.Down);
		menu.AddSubMenu(resoSubMenu, DebugUi.Direction.Right);
		var text = string.Format("{0}x{1}", Screen.width, Screen.height);
		resoButton = resoSubMenu.AddItem(text, () =>
		{
			var w = Screen.width * 70 / 100;
			var h = Screen.height * 70 / 100;
			if (w < (16 * 9))
			{
				w = 720;
				h = 1280;
			}
			Screen.SetResolution(w, h, false, 60);
			resoButton.text = w.ToString() + "x" + h.ToString();
		});
		resoSubMenu.AddItem("Aspect\n4:3", () =>
		{
			ScreenSettings.Save(1536, 2048, 0, 0, 0, 0);
		});
		resoSubMenu.AddItem("Aspect\n16:9", () =>
		{
			ScreenSettings.Save(1080, 1920, 0, 0, 0, 0);
		});
		resoSubMenu.AddItem("Aspect\niPhoneX", () =>
		{
			ScreenSettings.Save(1242, 2688, 0, 0, 132, 102);
		});
		resoSubMenu.AddItem("Aspect\nDefault", () =>
		{
			ScreenSettings.SaveDefault();
		});

		menu.AddItem(DebugServerUtil.GetLanIpAddress(), () =>
		{
			var url = string.Format("http://{0}:{1}/", DebugServerUtil.GetLanIpAddress(), serverPort);
			Application.OpenURL(url);
		});
		uiManager.enabled = false; // 初期状態は無効

		server.RegisterRequestCallback("/", OnWebRequestRoot);
		server.RegisterRequestCallback("/api/upload-file", OnWebRequestUploadFile);
		server.RegisterRequestCallback("/api/delete-file", OnWebRequestDeleteFile);
		server.RegisterRequestCallback("/api/delete-all-file", OnWebRequestDeleteAllFile);
		server.RegisterRequestCallback("/api/toggle-debug", OnWebRequestToggleDebug);

		var go = new GameObject("DebugTapper");
		tapper = go.AddComponent<DefaultDebugTapper>();
		tapper.ManualStart(8, tapMark);
		tapper.enabled = false;
	}

	IEnumerator CoSendJsons()
	{
		yield return CoSendTextFile("global_params.json");
		for (int i = 0; i < 100; i++) // とりあえず仮で大量に
		{
			var path = string.Format("StageData/stage_{0}.json", (i + 1));
			yield return CoSendTextFile(path);
		}
	}

	IEnumerator CoSendTextFile(string path)
	{
		if (slack == null)
		{
			yield break;
		}
		var ret = new CoroutineReturnValue<string>();
		yield return DebugServerUtil.CoLoad(ret, path);
		if (ret.Exception == null)
		{
			yield return slack.CoPostSnippet(ret.Value, null, null, path);
		}
	}

	public void ReplaceSubSceneMenu(SubScene newSubScene)
	{
		if (subSceneMenu != null)
		{
			menu.RemoveSubMenu(subSceneMenu);
			subSceneMenu = null;
		}

		subSceneMenu = newSubScene.CreateSubSceneMenu();
		if (subSceneMenu != null)
		{
			menu.AddSubMenu(
				subSceneMenu, DebugUiMenu.Direction.Right);
		}
	}

	public void ManualUpdate(float deltaTime)
	{
		frameCount = Time.frameCount;
		server.ManualUpdate();
		uiManager.ManualUpdate(deltaTime);
	}

	public void OnFileChanged(string path)
	{
		OnOverrideFileChanged(path);
	}

	// non public --------------
	World world;
	DebugServer server;
	DebugPrimitiveRenderer2D primitiveRenderer;
	DebugUiManager uiManager;
	DebugUiMenu menu;
	DebugUiSubMenu subSceneMenu;
	int frameCount; // 別スレッドから呼べないのでManualUpdateで更新
	string debugServerIndexHtml;
	DebugTapper tapper;


	[System.Serializable]
	class UploadFileArg
	{
		public string path;
		public string contentBase64;
	}

	void OnWebRequestRoot(
		out string outputHtml,
		NameValueCollection queryString,
		Stream bodyData)
	{
		outputHtml = debugServerIndexHtml;
	}

	void OnWebRequestUploadFile(
		out string outputHtml,
		NameValueCollection queryString,
		Stream bodyData)
	{
		outputHtml = null;
		if (bodyData == null)
		{
			outputHtml = "中身が空.";
			return;
		}
		var path = queryString["path"];
		if (string.IsNullOrEmpty(path))
		{
			outputHtml = "アップロードしたファイルのパスが空.";
			return;
		}
		DebugServerUtil.SaveOverride(path, bodyData);
		OnOverrideFileChanged(path);
	}

	void OnWebRequestDeleteFile(
		out string outputHtml,
		NameValueCollection queryString,
		Stream bodyData)
	{
		outputHtml = null;
		var path = queryString["path"];
		if (string.IsNullOrEmpty(path))
		{
			outputHtml = "アップロードしたファイルのパスが空.";
			return;
		}
		DebugServerUtil.DeleteOverride(path);
		OnOverrideFileChanged(path); // ファイルを消した後通知
	}

	void OnWebRequestDeleteAllFile(
		out string outputHtml,
		NameValueCollection queryString,
		Stream bodyData)
	{
		outputHtml = null;
		DebugServerUtil.DeleteAllOverride();
		OnOverrideFileChanged(null); // ファイルを消した後通知
	}

	void OnWebRequestToggleDebug(
		out string outputHtml,
		NameValueCollection queryString,
		Stream bodyData)
	{
		outputHtml = null;
		ToggleUiEnabled();
	}

	public void ToggleUiEnabled()
	{
		uiManager.enabled = !(uiManager.enabled);
	}
}
