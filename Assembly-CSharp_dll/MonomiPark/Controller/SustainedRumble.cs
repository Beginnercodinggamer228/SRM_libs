using System;

namespace MonomiPark.Controller
{
	// Token: 0x02000A37 RID: 2615
	public class SustainedRumble : Rumble
	{
		// Token: 0x0600462A RID: 17962 RVA: 0x000CE429 File Offset: 0x000CC629
		public SustainedRumble(Rumble.Motor motor, int power) : base(motor)
		{
			this.power = power;
		}

		// Token: 0x0600462B RID: 17963 RVA: 0x000CE439 File Offset: 0x000CC639
		public override int CurrentPower()
		{
			return this.power;
		}

		// Token: 0x0600462C RID: 17964 RVA: 0x000CE441 File Offset: 0x000CC641
		public void UpdatePower(int power)
		{
			this.power = power;
		}

		// Token: 0x0600462D RID: 17965 RVA: 0x000CE44A File Offset: 0x000CC64A
		public void FinishRumble()
		{
			this.isFinished = true;
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x000CE453 File Offset: 0x000CC653
		public override bool IsFinished()
		{
			return this.isFinished;
		}

		// Token: 0x040033F0 RID: 13296
		private int power;

		// Token: 0x040033F1 RID: 13297
		private bool isFinished;
	}
}
