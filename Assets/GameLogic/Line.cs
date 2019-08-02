using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kayac;

public class Line
{
	public Line(Color32 color, int count, bool loop)
	{
		this.color = color;
		this.loop = loop;
		points = new Vector2[count];
	}

	public void SetPoint(int index, float x, float y)
	{
		points[index] = new Vector2(x, y);
	}

	public void DrawDebug(DebugPrimitiveRenderer2D renderer)
	{
		renderer.color = color;
		renderer.AddLine1px(points[0].x, points[0].y, points[1].x, points[1].y);
		for (int i = 2; i < points.Length; i++)
		{
			renderer.ContinueLine1px(points[i].x, points[i].y);
		}
		if (loop)
		{
			renderer.ContinueLine1px(points[0].x, points[0].y);
		}
	}
	public Vector2[] points;
	public Color32 color;
	bool loop;
}
