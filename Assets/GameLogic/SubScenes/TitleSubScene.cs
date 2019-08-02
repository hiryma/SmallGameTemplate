using UnityEngine;
using UnityEngine.EventSystems;

public class TitleSubScene : SubScene, IPointerClickHandler, IDragHandler, IEndDragHandler
{
	[SerializeField] Canvas canvas;
	[SerializeField] CanvasRenderer textRenderer;

	bool clicked;
	World world;
	GameState gameState;
	Kayac.HiddenCommand toggleDebugCommand;
	Kayac.SpringDamper1 alphaAnim;

	void Awake()
	{
		canvas.enabled = false;
	}

	public override void ManualStart(GameState gameState)
	{
		this.gameState = gameState;
		toggleDebugCommand = new Kayac.HiddenCommand("012587436", 3, 3);
		gameState.StageIndex = 1;
		canvas.enabled = true;
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.planeDistance = 2f;
		canvas.worldCamera = gameState.World.CameraController.Camera;
		world = gameState.World;
		alphaAnim.Init(-0.4f, 0f, 16f, 0f);

		// ステージロード判定ガン無視して社ロゴ消す
		gameState.World.BootScreen.Hide();
	}

	public void OnPointerClick(PointerEventData dataUnused)
	{
		clicked = true;
	}

	public void OnDrag(PointerEventData data)
	{
		toggleDebugCommand.OnDrag(data);
	}

	public void OnEndDrag(PointerEventData data)
	{
		if (toggleDebugCommand.OnEndDrag(data))
		{
			gameState.DebugToggleFired = true;
		}
	}

	public override void ManualUpdate(out string nextSubSceneName, float deltaTime)
	{
		// NEXTの明滅
		alphaAnim.Update(deltaTime);
		var alpha = 0.6f + alphaAnim.Position;
		textRenderer.SetAlpha(alpha);

		nextSubSceneName = null;
		if (clicked)
		{
			nextSubSceneName = "Game";
		}
	}
}
