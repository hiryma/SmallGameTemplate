//#if DEVELOPMENT_BUILD || UNITY_EDITOR // 無条件Onでいいよね?
#define DEBUG_ENABLED
//#endif
using System.Collections;
using UnityEngine;
using Kayac;

public class Main : MonoBehaviour
{
	[SerializeField] Camera mainCamera;
	[SerializeField] BootSubScene bootSubScenePrefab;
	[SerializeField] World world;
	[SerializeField] Graphics bootingScreen; //後でsplashに変える
	[SerializeField, Header("Debug")] Shader debugTextShader;
	[SerializeField] Shader debugTexturedShader;
	[SerializeField] Font debugFont;
	[SerializeField] TextAsset debugServerIndexHtml;
	[SerializeField] MeshRenderer debugMeshRenderer;
	[SerializeField] MeshFilter debugMeshFilter;
	[SerializeField] Sprite debugTapMark;


	GameState gameState;
	SubScene subScene;
	SubScene nextSubScene;
	string loadSubSceneName;
	Coroutine loadSubSceneCoroutine;
	AsyncOperation bgSceneLoading;

#if DEBUG_ENABLED
	DebugService debugService;
	DebugPrimitiveRenderer3D debugRenderer;
#endif

	IEnumerator Start()
	{
 		Application.targetFrameRate = 60; // 60欲しい!

		// アスペクト比いじられてるようならカメラ追加してクリアする
		if (ScreenSettings.Instance.HasMargin)
		{
			var go = new GameObject("DebugClearCamera");
			go.transform.SetParent(gameObject.transform, false);
			var camera = go.AddComponent<Camera>();
			camera.depth = -float.MaxValue;
			camera.cullingMask = 0; // 何も描画しない
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.backgroundColor = Color.black;
		}

		yield return world.CoLoad();

		world.ManualStart();
#if DEBUG_ENABLED
		debugService = new DebugService(
			world,
			8080,
			mainCamera,
			debugTextShader,
			debugTexturedShader,
			debugFont,
			debugTapMark,
			debugServerIndexHtml.text);
		debugRenderer = new DebugPrimitiveRenderer3D(
			debugTextShader,
			debugTexturedShader,
			debugFont,
			mainCamera,
			debugMeshRenderer,
			debugMeshFilter);
		// 上書き検出
		debugService.OnOverrideFileChanged += world.OnOverrideFileChanged;
#endif
		gameState = new GameState(world);
		subScene = Instantiate(bootSubScenePrefab, gameObject.transform, false);

		// カメラ初期設定
		world.SetDefaultCamera(convergeNow: true);
	}

	void Update()
	{
		if (subScene == null) // 初期化済んでない
		{
			return;
		}
		// デバグコマンド
		if (gameState.DebugToggleFired)
		{
			gameState.DebugToggleFired = false;
			debugService.ToggleUiEnabled();
		}
		// ロード完了してるなら
		if ((nextSubScene != null) && (loadSubSceneCoroutine == null))
		{
			Debug.Log("SubSceneDispose " + subScene.name + " -> " + nextSubScene.name);
			// 現シーン破棄
			subScene.Dispose();
			Destroy(subScene.gameObject);
			// 新シーン開始
			subScene = nextSubScene;
#if DEBUG_ENABLED
			debugService.ReplaceSubSceneMenu(subScene);
#endif
			// 開始
			subScene.ManualStart(gameState);
			nextSubScene = null;
			loadSubSceneName = null;
		}

		float dt = Time.deltaTime;
		// シーン更新
		string nextSceneName = null;
		subScene.ManualUpdate(out nextSceneName, dt);
		// 次に行けと言われれば、
		if ((loadSubSceneName == null) && (nextSceneName != null))
		{
			loadSubSceneName = nextSceneName;
			loadSubSceneCoroutine = StartCoroutine(CoLoadSubScene());
		}

		// グローバルオブジェクト類更新
		world.ManualUpdate(dt);
#if DEBUG_ENABLED
		debugService.ManualUpdate(dt);
		debugRenderer.UpdateMesh();
#endif
	}

	IEnumerator CoLoadSubScene()
	{
		var fileName = "SubScenes/" + loadSubSceneName + "SubScene";
		var req = Resources.LoadAsync<SubScene>(fileName);
		yield return req;
		Debug.Assert(req.asset != null, "SubScene Load Failed: " + fileName);
		if (req.asset != null)
		{
			var prefab = req.asset as SubScene;
			var next = Instantiate(prefab, gameObject.transform, false);
			yield return next.CoLoad(gameState);
			nextSubScene = next; // ロード完了
			loadSubSceneCoroutine = null;
		}
	}
}
