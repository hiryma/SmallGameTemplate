using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kayac;
using UnityEngine.EventSystems;


public class GameSubScene : SubScene, IPointerDownHandler, IPointerUpHandler
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
		gameState.World.StartStage(stageData);
		var ball = gameState.World.Ball;
		var launcher = gameState.World.Launcher;
		var pos = launcher.InitialPosition;
		pos.y += ball.Radius;
pos.x -= 1f;
pos.y += 5f;
		ball.Reset(pos);
		ball.SetVisibility(true);
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
		// ボール落ちたらとりあえずgameover
		var world = gameState.World;
		if (world.Ball.Position.y < -10f)
		{
			toGameover = true;
		}

		// クリア判定
		bool cleared = false;
		if (!world.TargetAlive)
		{
			cleared = true;
		}


		// 遷移判定
		if (toGameover)
		{
			nextSceneName = "Title";
		}
		else if (cleared || Input.GetKeyDown(KeyCode.Q))
		{
			nextSceneName = "Result";
		}
		time += deltaTime;
	}

	public void OnPointerDown(PointerEventData data)
	{
		var world = gameState.World;
		var launcher = world.Launcher;
		launcher.Draw();
		world.Flip();
	}

	public void OnPointerUp(PointerEventData data)
	{
		var launcher = gameState.World.Launcher;
		launcher.Release();
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
