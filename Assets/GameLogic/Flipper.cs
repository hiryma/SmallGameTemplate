using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
	[SerializeField] HingeJoint2D joint;
	[SerializeField] Rigidbody2D rigidbody;
	[SerializeField] BoxCollider2D collider;
	[SerializeField] GameObject modelObject;

	float motorTimer;
	GlobalParams globalParams;

	public void ManualStart(GlobalParams globalParams, FlipperParams parameters)
	{
		this.globalParams = globalParams;
		transform.localPosition = new Vector3(parameters.x, parameters.y, 0f);
		transform.localRotation = Quaternion.Euler(0f, 0f, parameters.rotation);

		joint.useLimits = true;
		var limits = new JointAngleLimits2D();
		limits.min = parameters.angleMin;
		limits.max = parameters.angleMax;
		var xOffset = parameters.length * 0.5f;
		if (parameters.isLeft)
		{
			limits.min = -parameters.angleMin;
			limits.max = -parameters.angleMax;
		}
		else
		{
			xOffset = -xOffset;
			limits.min = parameters.angleMin;
			limits.max = parameters.angleMax;
		}
		joint.limits = limits;
		collider.offset = new Vector2(xOffset, 0f);
		collider.size = new Vector2(parameters.length, parameters.thickness);
		modelObject.transform.localPosition = new Vector3(xOffset, 0f, 0f);
		modelObject.transform.localScale = new Vector3(parameters.length, parameters.thickness, 1f);
		var motor = new JointMotor2D();
		motor.maxMotorTorque = float.MaxValue;
		motor.motorSpeed = globalParams.flipperMotorSpeed;
		motor.motorSpeed *= parameters.isLeft ? -1f : 1f;
		joint.motor = motor;
		joint.useMotor = false;
	}

	public void Flip()
	{
		joint.useMotor = true;
		motorTimer = 0f;
	}

	public void ManualUpdate(float deltaTime)
	{
		if (motorTimer >= globalParams.flipperDuration)
		{
			joint.useMotor = false;
		}
		motorTimer += deltaTime;
	}
}
