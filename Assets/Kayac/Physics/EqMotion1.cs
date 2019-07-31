using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kayac
{
	public struct EqMotion1
	{
		public void Init(float position, float velocity)
		{
			this.position = position;
			this.velocity =velocity;
		}
		public void Update(float deltaTime, float accel)
		{
			velocity += accel * deltaTime;
			position += velocity * deltaTime;
		}
		public float position;
		public float velocity;
	}

}