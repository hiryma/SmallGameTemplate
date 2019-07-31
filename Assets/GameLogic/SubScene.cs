using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SubScene : MonoBehaviour
{
	public virtual IEnumerator CoLoad(GameState gameState)
	{
		yield break;
	}

	public virtual void ManualStart(GameState gameState)
	{
	}

	public virtual void Dispose()
	{
	}

	public string Name
	{
		get
		{
			return GetType().Name;
		}
	}

	public virtual Kayac.DebugUiSubMenu CreateSubSceneMenu()
	{
		return null;
	}

	// 次のシーンを返したら、そのシーンのロードが完了し次第Destroyされる。
	// 続いている間はnextSceneNameはnullにせよ
	public virtual void ManualUpdate(out string nextSceneName, float deltaTime)
	{
		nextSceneName = null;
	}
}
