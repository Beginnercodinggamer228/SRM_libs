using System;
using UnityEngine;

// Token: 0x0200034A RID: 842
public class VacAccelerationHelper
{
	// Token: 0x060011AB RID: 4523 RVA: 0x00046D5F File Offset: 0x00044F5F
	public static VacAccelerationHelper CreateInput()
	{
		return new VacAccelerationHelper(1f, 3f, 0.5f, 1f);
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x00046D7A File Offset: 0x00044F7A
	public static VacAccelerationHelper CreateOutput()
	{
		return new VacAccelerationHelper(1f, 1.75f, 0.5f, 1f);
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x00046D95 File Offset: 0x00044F95
	public VacAccelerationHelper(float minFactor, float maxFactor, float speed, float duration)
	{
		this.minFactor = minFactor;
		this.maxFactor = maxFactor;
		this.speed = speed;
		this.duration = duration;
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x060011AE RID: 4526 RVA: 0x00046DBA File Offset: 0x00044FBA
	public float Factor
	{
		get
		{
			if (Time.time < this.timeEnd)
			{
				return Math.Min(this.minFactor + (Time.time - this.timeBegin) * this.speed, this.maxFactor);
			}
			return this.minFactor;
		}
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x00046DF5 File Offset: 0x00044FF5
	public void OnTriggered()
	{
		this.timeBegin = ((Time.time >= this.timeEnd) ? Time.time : this.timeBegin);
		this.timeEnd = Time.time + this.duration;
	}

	// Token: 0x040010E9 RID: 4329
	public readonly float minFactor;

	// Token: 0x040010EA RID: 4330
	public readonly float maxFactor;

	// Token: 0x040010EB RID: 4331
	public readonly float speed;

	// Token: 0x040010EC RID: 4332
	public readonly float duration;

	// Token: 0x040010ED RID: 4333
	private float timeBegin;

	// Token: 0x040010EE RID: 4334
	private float timeEnd;
}
