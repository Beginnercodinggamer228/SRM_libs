using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonomiPark.Controller
{
	// Token: 0x02000A2F RID: 2607
	public abstract class BaseRumbleHandler : MonoBehaviour, RumbleHandler
	{
		// Token: 0x06004609 RID: 17929 RVA: 0x000CE152 File Offset: 0x000CC352
		public void AddRumble(Rumble rumble)
		{
			this.rumbles[rumble.GetMotor()].Add(rumble);
		}

		// Token: 0x0600460A RID: 17930 RVA: 0x000CE16C File Offset: 0x000CC36C
		private void Awake()
		{
			this.rumbles[Rumble.Motor.LARGE] = new List<Rumble>();
			this.rumbles[Rumble.Motor.SMALL] = new List<Rumble>();
			this.rumbles[Rumble.Motor.LEFT] = new List<Rumble>();
			this.rumbles[Rumble.Motor.RIGHT] = new List<Rumble>();
		}

		// Token: 0x0600460B RID: 17931 RVA: 0x000CE1BD File Offset: 0x000CC3BD
		private void Update()
		{
			this.AggregateRumbles();
			this.ApplyRumblePower();
			this.CleanupRumbles();
		}

		// Token: 0x0600460C RID: 17932 RVA: 0x000CE1D4 File Offset: 0x000CC3D4
		private void AggregateRumbles()
		{
			this.aggregateRumblePower[Rumble.Motor.RIGHT] = 0;
			this.aggregateRumblePower[Rumble.Motor.LEFT] = 0;
			this.aggregateRumblePower[Rumble.Motor.SMALL] = 0;
			this.aggregateRumblePower[Rumble.Motor.LARGE] = 0;
			foreach (KeyValuePair<Rumble.Motor, List<Rumble>> keyValuePair in this.rumbles)
			{
				foreach (Rumble rumble in keyValuePair.Value)
				{
					this.ApplyRumble(rumble);
				}
			}
		}

		// Token: 0x0600460D RID: 17933 RVA: 0x000CE298 File Offset: 0x000CC498
		private void ApplyRumble(Rumble rumble)
		{
			if (rumble.IsFinished())
			{
				this.toRemove.Add(rumble);
				return;
			}
			Dictionary<Rumble.Motor, int> dictionary = this.aggregateRumblePower;
			Rumble.Motor motor = rumble.GetMotor();
			dictionary[motor] += rumble.CurrentPower();
		}

		// Token: 0x0600460E RID: 17934 RVA: 0x000CE2E0 File Offset: 0x000CC4E0
		private void CleanupRumbles()
		{
			foreach (Rumble rumble in this.toRemove)
			{
				this.rumbles[rumble.GetMotor()].Remove(rumble);
			}
			this.toRemove.Clear();
		}

		// Token: 0x0600460F RID: 17935
		protected abstract void ApplyRumblePower();

		// Token: 0x06004610 RID: 17936 RVA: 0x000CE350 File Offset: 0x000CC550
		public void EnableRumble()
		{
			this.rumbleEnabled = true;
		}

		// Token: 0x06004611 RID: 17937 RVA: 0x000CE359 File Offset: 0x000CC559
		public void DisableRumble()
		{
			this.rumbleEnabled = false;
		}

		// Token: 0x06004612 RID: 17938 RVA: 0x000CE362 File Offset: 0x000CC562
		public bool IsRumbleEnabled()
		{
			return this.rumbleEnabled;
		}

		// Token: 0x040033E2 RID: 13282
		protected bool rumbleEnabled = true;

		// Token: 0x040033E3 RID: 13283
		protected Dictionary<Rumble.Motor, List<Rumble>> rumbles = new Dictionary<Rumble.Motor, List<Rumble>>(Rumble.motorComparer);

		// Token: 0x040033E4 RID: 13284
		protected Dictionary<Rumble.Motor, int> aggregateRumblePower = new Dictionary<Rumble.Motor, int>(Rumble.motorComparer);

		// Token: 0x040033E5 RID: 13285
		protected List<Rumble> toRemove = new List<Rumble>();
	}
}
