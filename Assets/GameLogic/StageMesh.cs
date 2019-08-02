using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kayac;

public class StageMesh : MonoBehaviour
{
	[SerializeField] PhysicsMaterial2D material;

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
		var line = new Line(new Color32(0, 255, 0, 255), data.points.Length, loop: false);
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
