using System.Collections;
using UnityEngine;
using Kayac;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
	[SerializeField] Camera mainCamera;
	[SerializeField] BootScreen bootScreen;
	[SerializeField] bool cameraControllerDisabled;
	[SerializeField] StageMesh stageMesh;
	[SerializeField] Ball ball;
	[SerializeField] Launcher launcher;
	[SerializeField] Flipper flipperPrefab;

	[SerializeField, Header("Debug")] Shader debugTextShader;
	[SerializeField] Shader debugTexturedShader;
	[SerializeField] Font debugFont;
	[SerializeField] MeshRenderer debugMeshRenderer;
	[SerializeField] MeshFilter debugMeshFilter;

	public Launcher Launcher{get{return launcher;}}
	public Ball Ball{get{return ball;}}
	public BootScreen BootScreen{get{return bootScreen;}}
	public Camera Camera { get { return mainCamera; } }
	public GlobalParams GlobalParams { get; private set; }
	const string globalParamsPath = "global_params.json";
	DebugPrimitiveRenderer2D debugRenderer;
	Flipper[] flippers;

	public IEnumerator CoLoad()
	{
		CameraController = new CameraController(mainCamera);

		yield return CoLoadGlobalParams();
		if (GlobalParams == null)
		{
			Debug.LogError("グローバルパラメタJSONが読めない!");
			yield break;
		}
		SetDefaultCamera(convergeNow: true);
	}

	public void ReloadGlobalParams()
	{
		StartCoroutine(CoLoadGlobalParams());
	}

	IEnumerator CoLoadGlobalParams()
	{
		var ret = new CoroutineReturnValue<string>();
		yield return DebugServerUtil.CoLoad(ret, globalParamsPath);
		if (ret.Exception != null)
		{
			Debug.LogException(ret.Exception);
			yield break;
		}
		OnGlobalParamsChange(ret.Value);
	}

	public void OnOverrideFileChanged(string path)
	{
		// 全ファイル削除か、GlobalParamであればリロード
		if ((path == null) || (path == globalParamsPath))
		{
			StartCoroutine(CoLoadGlobalParams());
		}
	}

	void OnGlobalParamsChange(string json)
	{
		var newParams = UnityEngine.JsonUtility.FromJson<GlobalParams>(json);
		if (newParams == null)
		{
			Debug.LogError("global_params.json : deserialization failed.\n" + json);
		}
		else
		{
			GlobalParams = newParams;
			ApplyGlobalParams();
		}
	}

	public void ManualStart()
	{
		debugRenderer = new DebugPrimitiveRenderer2D(
			debugTextShader,
			debugTexturedShader,
			debugFont,
			debugMeshRenderer,
			debugMeshFilter);
		debugMeshRenderer.enabled = true;
		launcher.ManualStart(GlobalParams.launcher);
		stageMesh.ManualStart(GlobalParams.commonShape);

		bootScreen.ManualStart(mainCamera);
	}

	public void StartStage(StageData stageData)
	{
		if (flippers != null)
		{
			foreach (var flipper in flippers)
			{
				Destroy(flipper.gameObject);
			}
		}
		flippers = new Flipper[stageData.flippers.Length];
		for (int i = 0; i < stageData.flippers.Length; i++)
		{
			flippers[i] = Instantiate(flipperPrefab, transform, false);
			flippers[i].ManualStart(GlobalParams, stageData.flippers[i]);
		}
	}

	public void Flip()
	{
		foreach (var flipper in flippers)
		{
			flipper.Flip();
		}
	}

	public void ApplyGlobalParams()
	{
		Physics2D.gravity = new Vector2(0f, -GlobalParams.gravity);
		SetDefaultCamera(convergeNow: true);
	}

	public void SetDefaultCamera(bool convergeNow)
	{
		CameraController.PositionGoal = new Vector3(0f, 50f, -130f);
		CameraController.TargetGoal = new Vector3(0f, 50f, 0f);
		CameraController.Stiffness = 4f;
		if (convergeNow)
		{
			CameraController.Converge();
		}
	}

	public void ShakeCamera(float magnitude, float decline)
	{
		CameraController.ShakeDecline = decline;
		CameraController.Shake(magnitude);
	}

	public CameraController CameraController { get; private set; }

	// タイトルや結果でも行う更新処理
	public void ManualUpdate(float deltaTime)
	{
		if (flippers != null)
		{
			foreach (var flipper in flippers)
			{
				flipper.ManualUpdate(deltaTime);
			}
		}
		stageMesh.ManualUpdate(deltaTime);
		DrawDebug();

		bootScreen.ManualUpdate(deltaTime);
		if (!cameraControllerDisabled)
		{
			CameraController.ManualUpdate(deltaTime);
		}
	}

	void DrawDebug()
	{
		stageMesh.DrawDebug(debugRenderer);
		debugRenderer.UpdateMesh();
	}

	void Awake()
	{
	}
}
