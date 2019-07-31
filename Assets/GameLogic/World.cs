using System.Collections;
using UnityEngine;
using Kayac;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
	[SerializeField] Camera mainCamera;
	[SerializeField] BootScreen bootScreen;
	[SerializeField] bool cameraControllerDisabled;

	public Camera Camera { get { return mainCamera; } }
	public GlobalParams GlobalParams { get; private set; }
	const string globalParamsPath = "global_params.json";

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
		bootScreen.ManualStart(mainCamera);
	}

	public void ApplyGlobalParams()
	{
		SetDefaultCamera(convergeNow: true);
	}

	public void SetDefaultCamera(bool convergeNow)
	{
		CameraController.PositionGoal = new Vector3(0f, 100f, 100f);
		CameraController.TargetGoal = new Vector3(0f, 0f, 0f);
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
		bootScreen.ManualUpdate(deltaTime);
		if (!cameraControllerDisabled)
		{
			CameraController.ManualUpdate(deltaTime);
		}
	}

	void Awake()
	{
	}
}
