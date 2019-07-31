namespace Kayac
{
	public struct SpringDamper1
	{
		public void Init(
			float position,
			float velocity,
			float stiffness,
			float damping)
		{
			eqMotion.Init(position, velocity);
			this.stiffness = stiffness;
			this.damping = damping;
		}
		public void Update(float deltaTime)
		{
			var accel = -eqMotion.velocity * damping;
			accel += -eqMotion.position * stiffness;
			eqMotion.Update(deltaTime, accel);
		}

		public float Position { get { return eqMotion.position; } }

		EqMotion1 eqMotion;
		public float stiffness;
		public float damping;
	}
}
