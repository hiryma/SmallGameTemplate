using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kayac;

public class Target : MonoBehaviour
{
	[SerializeField] PhysicsMaterial2D material;

	Line line;
	EdgeCollider2D collider;
	int life;

	public bool Alive{ get{return life > 0;}}

	public void ManualStart(TargetData data)
	{
		life = data.life;
		line = new Line(new Color32(255, 128, 128, 255), data.points.Length, loop: true);
		for (int i = 0; i < data.points.Length; i++)
		{
			line.SetPoint(i, data.points[i].x, data.points[i].y);
		}
		BuildCollider();
	}

	void BuildCollider()
	{
		var collider = gameObject.AddComponent<EdgeCollider2D>();
		collider.sharedMaterial = material;
		collider.points = line.points;
		collider.
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log("Hit! Life=" + life);
		life--;
		if (life <= 0)
		{
			gameObject.SetActive(false);
		}
	}

	public void DrawDebug(DebugPrimitiveRenderer2D renderer)
	{
		if (Alive)
		{
			line.DrawDebug(renderer);
		}
	}
}
