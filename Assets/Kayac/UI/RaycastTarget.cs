using UnityEngine;
using UnityEngine.UI;

public class RaycastTarget : Graphic
{
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
