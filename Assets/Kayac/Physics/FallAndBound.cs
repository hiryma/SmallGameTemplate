namespace Kayac
{
	public struct FallAndBound
	{
		public void Init(
			float position,
			float velocity,
			float gravity,
			float damping,
			float bounciness)
		{
			eqMotion.Init(position, velocity);
			this.gravity = gravity;
			this.damping = damping;
			this.bounciness = bounciness;
		}

		public void Update(float deltaTime)
		{
			var accel = -gravity;
			accel -= eqMotion.velocity * damping;
			var prevPosition = eqMotion.position;
			eqMotion.Update(deltaTime, accel);
			if ((prevPosition >= 0f) && (eqMotion.position < 0f))
			{
				eqMotion.velocity *= -bounciness;
				eqMotion.position = 0f;
//				eqMotion.position = -eqMotion.position * bounciness;
			}
		}
		public float Position { get { return eqMotion.position; } }
		EqMotion1 eqMotion;
		public float gravity;
		public float damping;
		public float bounciness;
	}
}