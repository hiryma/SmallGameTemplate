using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kayac;

public class StageMesh : MonoBehaviour
{
	[SerializeField] PhysicsMaterial2D material;
	class Line
	{
		public Line(Color32 color, int count)
		{
			this.color = color;
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
		}
		public Vector2[] points;
		public Color32 color;
	}
	List<Line> lines;
	EdgeCollider2D[] colliders;

	public void ManualStart(ShapeData shape)
	{
		ReadShape(shape);
		BuildColliders();
	}

	void ReadShape(ShapeData data)
	{
		lines = new List<Line>();
		for (int i = 0; i < data.lines.Length; i++)
		{
			lines.Add(ReadLine(data.lines[i]));
		}
	}

	Line ReadLine(LineData data)
	{
		var line = new Line(new Color32(0, 255, 0, 255), data.points.Length);
		for (int i = 0; i < data.points.Length; i++)
		{
			line.SetPoint(i, data.points[i].x, data.points[i].y);
		}
		return line;
	}

	public void ManualUpdate(float deltaTime)
	{

	}

	void BuildColliders()
	{
		if (colliders != null)
		{
			foreach (var collider in colliders)
			{
				Destroy(collider.gameObject);
			}
		}
		colliders = new EdgeCollider2D[lines.Count];
		foreach (var line in lines)
		{
			BuildCollider(line);
		}
	}

	void BuildCollider(Line line)
	{
		var go = new GameObject("Colider");
		go.transform.SetParent(transform, false);
		var collider = go.AddComponent<EdgeCollider2D>();
		collider.sharedMaterial = material;
		collider.points = line.points;
	}

	public void DrawDebug(DebugPrimitiveRenderer2D renderer)
	{
		foreach (var line in lines)
		{
			line.DrawDebug(renderer);
		}
	}
}
