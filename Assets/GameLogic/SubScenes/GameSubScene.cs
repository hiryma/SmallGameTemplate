using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kayac;


public class GameSubScene : SubScene
{
	[SerializeField] Canvas canvas;
	[SerializeField] RectTransformScaler scaler;

	public override IEnumerator CoLoad(GameState gameState)
	{
		this.gameState = gameState;
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.planeDistance = 2f;
		canvas.worldCamera = gameState.World.CameraController.Camera;

		// ステージデータ読み込み
		var ret = new CoroutineReturnValue<string>();
		var path = string.Format("StageData/stage_{0}.json", gameState.StageIndex);
		yield return DebugServerUtil.CoLoad(ret, path);
		if (ret.Exception != null)
		{
			Debug.LogException(ret.Exception);
			yield break;
		}
		var newStageData = UnityEngine.JsonUtility.FromJson<StageData>(ret.Value);
		if (newStageData != null)
		{
			stageData = newStageData;
		}
		else
		{
			Debug.LogError("json deserialization error.");
		}
	}

	public override Kayac.DebugUiSubMenu CreateSubSceneMenu()
	{
		var ret = new DebugUiSubMenu("Game[" + gameState.StageIndex + "]", 100f, 40f, DebugUi.Direction.Down);
		ret.AddItem("Clear", () =>
		{
		});
		ret.AddItem("GameOver", () =>
		{
		});
		return ret;
	}

	public override void ManualStart(GameState gameState)
	{
		scaler.Apply();
		Reset();
	}

	void Reset()
	{
		var param = gameState.World.GlobalParams;
		time = 0f;
		gameState.World.SetDefaultCamera(convergeNow: false);
	}

	public override void Dispose()
	{
	}

	public override void ManualUpdate(out string nextSceneName, float deltaTime)
	{
		nextSceneName = null;
		// 遷移判定
		var world = gameState.World;
		if (toGameover)
		{
			nextSceneName = "Title";
		}
		else if (Input.GetKeyDown(KeyCode.Q))
		{
			nextSceneName = "Result";
		}
		time += deltaTime;
	}

	// non public ------------------------
	GameState gameState;
	StageData stageData;
	float time;
	bool toGameover;

	void Awake()
	{
	}
}
