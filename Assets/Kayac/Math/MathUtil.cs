using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
	public static float Percent()
	{
		return UnityEngine.Random.value * 100f;
	}

	public static Vector3 Min(Vector3 a, Vector3 b)
	{
		Vector3 r;
		r.x = System.Math.Min(a.x, b.x);
		r.y = System.Math.Min(a.y, b.y);
		r.z = System.Math.Min(a.z, b.z);
		return r;
	}

	public static Vector3 Max(Vector3 a, Vector3 b)
	{
		Vector3 r;
		r.x = System.Math.Max(a.x, b.x);
		r.y = System.Math.Max(a.y, b.y);
		r.z = System.Math.Max(a.z, b.z);
		return r;
	}

	public static float InterpolateExp(float current, float goal, float deltaTime, float speed)
	{
		current += (goal - current) * (deltaTime * speed);
		return current;
	}

	public static Vector2 InterpolateExp(Vector2 current, Vector2 goal, float deltaTime, float speed)
	{
		current += (goal - current) * (deltaTime * speed);
		return current;
	}

	public static Vector3 InterpolateExp(Vector3 current, Vector3 goal, float deltaTime, float speed)
	{
		current += (goal - current) * (deltaTime * speed);
		return current;
	}
}
