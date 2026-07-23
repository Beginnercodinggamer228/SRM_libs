using System;
using UnityEngine;

namespace MonomiPark.Controller
{
	// Token: 0x02000A32 RID: 2610
	public class PulseRumble : Rumble
	{
		// Token: 0x0600461B RID: 17947 RVA: 0x000CE3CF File Offset: 0x000CC5CF
		public PulseRumble(Rumble.Motor motor, int maxPower, float duration) : base(motor)
		{
			this.stopTime = Time.time + duration;
			this.maxPower = maxPower;
		}

		// Token: 0x0600461C RID: 17948 RVA: 0x000CE3EC File Offset: 0x000CC5EC
		public override int CurrentPower()
		{
			return this.maxPower;
		}

		// Token: 0x0600461D RID: 17949 RVA: 0x000CE3F4 File Offset: 0x000CC5F4
		public override bool IsFinished()
		{
			return Time.time >= this.stopTime;
		}

		// Token: 0x040033E7 RID: 13287
		private int maxPower;

		// Token: 0x040033E8 RID: 13288
		private float stopTime;
	}
}
