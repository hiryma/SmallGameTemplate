using UnityEngine;

namespace Kayac
{
	public class ResolutionConverter : MonoBehaviour
	{
		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			// Copy the source Render Texture to the destination,
			// applying the material along the way.
			Graphics.Blit(src, dest, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}
/*
		void OnPostRender()
		{
			var camera = gameObject.GetComponent<Camera>();
			if (camera.tag != "MainCamera")
			{
				Debug.LogError("ResolutionConverter need MainCamera.");
				return;
			}
			var rt = camera.targetTexture;
			if (rt != null)
			{
				var rt2 = camera.targetTexture;
				Graphics.SetRenderTarget(null);
				GL.Clear(true, true, new Color(1f, 1f, 0f, 0f), 1f);
				Graphics.Blit(rt, null, new Vector2(1f, 1f), new Vector2(0f, 0f));
			}
		}
*/
	}
}
