using System;

namespace MonomiPark.Controller
{
	// Token: 0x02000A30 RID: 2608
	public class EmptyRumbleHandler : RumbleHandler
	{
		// Token: 0x06004614 RID: 17940 RVA: 0x00003296 File Offset: 0x00001496
		public void AddRumble(Rumble rumble)
		{
		}

		// Token: 0x06004615 RID: 17941 RVA: 0x000CE3A4 File Offset: 0x000CC5A4
		public void DisableRumble()
		{
			this.enabled = false;
		}

		// Token: 0x06004616 RID: 17942 RVA: 0x000CE3AD File Offset: 0x000CC5AD
		public void EnableRumble()
		{
			this.enabled = true;
		}

		// Token: 0x06004617 RID: 17943 RVA: 0x000CE3B6 File Offset: 0x000CC5B6
		public bool IsRumbleEnabled()
		{
			return this.enabled;
		}

		// Token: 0x040033E6 RID: 13286
		private bool enabled;
	}
}
