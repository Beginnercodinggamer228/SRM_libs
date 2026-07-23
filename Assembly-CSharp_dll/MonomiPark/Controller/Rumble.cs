using System;
using System.Collections.Generic;

namespace MonomiPark.Controller
{
	// Token: 0x02000A33 RID: 2611
	public abstract class Rumble
	{
		// Token: 0x0600461E RID: 17950 RVA: 0x000CE406 File Offset: 0x000CC606
		public Rumble(Rumble.Motor motor)
		{
			this.motor = motor;
		}

		// Token: 0x0600461F RID: 17951 RVA: 0x000CE415 File Offset: 0x000CC615
		public Rumble.Motor GetMotor()
		{
			return this.motor;
		}

		// Token: 0x06004620 RID: 17952
		public abstract int CurrentPower();

		// Token: 0x06004621 RID: 17953
		public abstract bool IsFinished();

		// Token: 0x040033E9 RID: 13289
		public static IEqualityComparer<Rumble.Motor> motorComparer = new Rumble.MotorComparer();

		// Token: 0x040033EA RID: 13290
		private Rumble.Motor motor;

		// Token: 0x02000A34 RID: 2612
		public enum Motor
		{
			// Token: 0x040033EC RID: 13292
			LARGE,
			// Token: 0x040033ED RID: 13293
			SMALL,
			// Token: 0x040033EE RID: 13294
			LEFT,
			// Token: 0x040033EF RID: 13295
			RIGHT
		}

		// Token: 0x02000A35 RID: 2613
		public class MotorComparer : IEqualityComparer<Rumble.Motor>
		{
			// Token: 0x06004623 RID: 17955 RVA: 0x00017781 File Offset: 0x00015981
			public bool Equals(Rumble.Motor motor1, Rumble.Motor motor2)
			{
				return motor1 == motor2;
			}

			// Token: 0x06004624 RID: 17956 RVA: 0x00017787 File Offset: 0x00015987
			public int GetHashCode(Rumble.Motor motor)
			{
				return (int)motor;
			}
		}
	}
}
