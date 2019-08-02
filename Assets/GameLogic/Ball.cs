using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	[SerializeField] Rigidbody2D rigidBody;
	[SerializeField] GameObject modelObject;

	public float Radius{ get{return 2f;}}
	public Vector2 Position{ get {return rigidBody.position;} }

	public void Reset(Vector2 position)
	{
		rigidBody.position = position;
		rigidBody.velocity = Vector2.zero;
	}

	public void AddForce(Vector2 force)
	{
		rigidBody.AddForce(force);
	}

	public void SetVisibility(bool visible)
	{
		modelObject.SetActive(visible);
	}

	void Awake()
	{
		SetVisibility(false);
	}
}
