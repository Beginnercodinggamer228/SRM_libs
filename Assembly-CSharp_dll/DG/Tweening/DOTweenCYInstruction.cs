using System;
using UnityEngine;

namespace DG.Tweening
{
	// Token: 0x02000C21 RID: 3105
	public static class DOTweenCYInstruction
	{
		// Token: 0x02000C22 RID: 3106
		public class WaitForCompletion : CustomYieldInstruction
		{
			// Token: 0x1700049F RID: 1183
			// (get) Token: 0x060057E6 RID: 22502 RVA: 0x001094EF File Offset: 0x001076EF
			public override bool keepWaiting
			{
				get
				{
					return this.t.active && !this.t.IsComplete();
				}
			}

			// Token: 0x060057E7 RID: 22503 RVA: 0x0010950E File Offset: 0x0010770E
			public WaitForCompletion(Tween tween)
			{
				this.t = tween;
			}

			// Token: 0x04004373 RID: 17267
			private readonly Tween t;
		}

		// Token: 0x02000C23 RID: 3107
		public class WaitForRewind : CustomYieldInstruction
		{
			// Token: 0x170004A0 RID: 1184
			// (get) Token: 0x060057E8 RID: 22504 RVA: 0x00109520 File Offset: 0x00107720
			public override bool keepWaiting
			{
				get
				{
					return this.t.active && (!this.t.playedOnce || this.t.position * (float)(this.t.CompletedLoops() + 1) > 0f);
				}
			}

			// Token: 0x060057E9 RID: 22505 RVA: 0x0010956C File Offset: 0x0010776C
			public WaitForRewind(Tween tween)
			{
				this.t = tween;
			}

			// Token: 0x04004374 RID: 17268
			private readonly Tween t;
		}

		// Token: 0x02000C24 RID: 3108
		public class WaitForKill : CustomYieldInstruction
		{
			// Token: 0x170004A1 RID: 1185
			// (get) Token: 0x060057EA RID: 22506 RVA: 0x0010957B File Offset: 0x0010777B
			public override bool keepWaiting
			{
				get
				{
					return this.t.active;
				}
			}

			// Token: 0x060057EB RID: 22507 RVA: 0x00109588 File Offset: 0x00107788
			public WaitForKill(Tween tween)
			{
				this.t = tween;
			}

			// Token: 0x04004375 RID: 17269
			private readonly Tween t;
		}

		// Token: 0x02000C25 RID: 3109
		public class WaitForElapsedLoops : CustomYieldInstruction
		{
			// Token: 0x170004A2 RID: 1186
			// (get) Token: 0x060057EC RID: 22508 RVA: 0x00109597 File Offset: 0x00107797
			public override bool keepWaiting
			{
				get
				{
					return this.t.active && this.t.CompletedLoops() < this.elapsedLoops;
				}
			}

			// Token: 0x060057ED RID: 22509 RVA: 0x001095BB File Offset: 0x001077BB
			public WaitForElapsedLoops(Tween tween, int elapsedLoops)
			{
				this.t = tween;
				this.elapsedLoops = elapsedLoops;
			}

			// Token: 0x04004376 RID: 17270
			private readonly Tween t;

			// Token: 0x04004377 RID: 17271
			private readonly int elapsedLoops;
		}

		// Token: 0x02000C26 RID: 3110
		public class WaitForPosition : CustomYieldInstruction
		{
			// Token: 0x170004A3 RID: 1187
			// (get) Token: 0x060057EE RID: 22510 RVA: 0x001095D1 File Offset: 0x001077D1
			public override bool keepWaiting
			{
				get
				{
					return this.t.active && this.t.position * (float)(this.t.CompletedLoops() + 1) < this.position;
				}
			}

			// Token: 0x060057EF RID: 22511 RVA: 0x00109604 File Offset: 0x00107804
			public WaitForPosition(Tween tween, float position)
			{
				this.t = tween;
				this.position = position;
			}

			// Token: 0x04004378 RID: 17272
			private readonly Tween t;

			// Token: 0x04004379 RID: 17273
			private readonly float position;
		}

		// Token: 0x02000C27 RID: 3111
		public class WaitForStart : CustomYieldInstruction
		{
			// Token: 0x170004A4 RID: 1188
			// (get) Token: 0x060057F0 RID: 22512 RVA: 0x0010961A File Offset: 0x0010781A
			public override bool keepWaiting
			{
				get
				{
					return this.t.active && !this.t.playedOnce;
				}
			}

			// Token: 0x060057F1 RID: 22513 RVA: 0x00109639 File Offset: 0x00107839
			public WaitForStart(Tween tween)
			{
				this.t = tween;
			}

			// Token: 0x0400437A RID: 17274
			private readonly Tween t;
		}
	}
}
