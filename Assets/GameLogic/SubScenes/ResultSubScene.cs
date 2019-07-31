using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResultSubScene : SubScene, IPointerClickHandler
{
	[SerializeField] Canvas canvas;

	void Awake()
	{
		canvas.enabled = false;
	}

	public override void ManualStart(GameState gameState)
	{
		canvas.enabled = true;
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.planeDistance = 2f;
		canvas.worldCamera = gameState.World.CameraController.Camera;

		gameState.StageIndex++;
		// 雑に循環させる
		if (gameState.StageIndex > gameState.World.GlobalParams.stageCount)
		{
			gameState.StageIndex = 1; // TODO: エンディング出すの?
			gameState.LoopCount++; // 周回数増える
		}
	}

	public override void ManualUpdate(out string nextSceneName, float deltaTime)
	{
		animationEnd = true;
		nextSceneName = null;
		if (clicked && animationEnd)
		{
			nextSceneName = "Game";
		}
	}

	public void OnPointerClick(PointerEventData data)
	{
		clicked = true;
	}

	// non public ----------------
	bool clicked;
	bool animationEnd;
}
