using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
	[SerializeField] Rigidbody2D rigidbody;

	LauncherParams parameters;
	float position;
	float velocity;
	Vector2 initialPosition;
	bool drawing;

	public void ManualStart(LauncherParams parameters)
	{
		this.parameters = parameters;
		rigidbody.isKinematic = true;
		position = 0f;
		velocity = 0f;
		initialPosition = new Vector2(parameters.x, parameters.y);
		rigidbody.position = initialPosition;
	}

	public Vector2 InitialPosition{ get{ return initialPosition;}}

	public void Draw()
	{
		drawing = true;
	}

	public void Release()
	{
		drawing = false;
	}

	void FixedUpdate()
	{
		if (parameters == null)
		{
			return;
		}
		var dt = Time.fixedDeltaTime;
		var accel = ((initialPosition.y - position) * parameters.spring);
		accel -= (velocity * parameters.damper);
		if (drawing)
		{
			accel -= parameters.drawAccel;
		}
		velocity += accel * dt;
		position += velocity * dt;
		var pos = initialPosition;
		pos.y = position;
		rigidbody.position = pos;
		rigidbody.velocity = new Vector2(0f, velocity);
	}
}
