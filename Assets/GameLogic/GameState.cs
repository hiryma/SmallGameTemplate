public class GameState
{
	public World World { get; private set; }
	public int StageIndex { get; set; }
	public int ClearScore { get; set; } // 0ならクリアしてない
	public int LoopCount { get; set; } // 周回数
	public bool OnceShutterOpened { get; set; } // 一度でもシャッターを開けた
	public bool DebugToggleFired { get; set; }

	public GameState(World world)
	{
		World = world;
		StageIndex = 1;
	}
}
