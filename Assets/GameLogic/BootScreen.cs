using UnityEngine;

public class BootScreen : MonoBehaviour
{
	[SerializeField] MeshRenderer logo;
	Material copyMaterial;
	float hideTimer;

	public void ManualStart(Camera camera)
	{
		hideTimer = -float.MaxValue;
		copyMaterial = new Material(logo.sharedMaterial);
		copyMaterial.name = copyMaterial.name + "(Copy)";
		logo.sharedMaterial = copyMaterial;
		logo.sortingOrder = 10000;
		logo.enabled = true;

		var fov = camera.fieldOfView;
		var tanY = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
		var aspect = camera.aspect;
		var imageAspect = (750f / 1334f);
		if (aspect >= imageAspect) // 画面が9:16より横長。縦合わせ。
		{
			var ox = (aspect - imageAspect) * 0.5f / imageAspect;
			copyMaterial.mainTextureOffset = new Vector2(-ox, 0f);
			copyMaterial.mainTextureScale = new Vector2(aspect / imageAspect, 1f);
		}
		else // 縦長。横合わせ
		{
			var fullV = imageAspect / aspect;
			var oy = (fullV - 1f) * 0.5f;
			copyMaterial.mainTextureOffset = new Vector2(0f, oy);
			copyMaterial.mainTextureScale = new Vector2(1f, aspect / imageAspect);
		}
		logo.transform.localScale = new Vector3(
			tanY * 2f * aspect,
			tanY * 2f, 1f);
	}

	public void Hide()
	{
		if (hideTimer < 0)
		{
			hideTimer = 0f;
		}
	}

	public void ManualUpdate(float deltaTime)
	{
		if (hideTimer >= 0f)
		{
			hideTimer += deltaTime;
			var alpha = 1f - hideTimer;
			if (alpha <= 0f)
			{
				logo.enabled = false;
			}
			else
			{
				copyMaterial.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
			}
		}
	}
}
