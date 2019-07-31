using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootSubScene : SubScene
{
	public override void ManualUpdate(out string nextSceneName, float deltaTime)
	{
		nextSceneName = "Title"; // 今のところやることないのですぐタイトルへ
	}
}
