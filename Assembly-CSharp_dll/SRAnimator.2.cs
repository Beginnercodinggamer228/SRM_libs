using System;

// Token: 0x02000365 RID: 869
public abstract class SRAnimator<T> : SRAnimator
{
	// Token: 0x17000190 RID: 400
	// (get) Token: 0x06001201 RID: 4609 RVA: 0x00047A04 File Offset: 0x00045C04
	// (set) Token: 0x06001202 RID: 4610 RVA: 0x00047A0C File Offset: 0x00045C0C
	public T parent { get; private set; }

	// Token: 0x06001203 RID: 4611 RVA: 0x00047A15 File Offset: 0x00045C15
	public override void Awake()
	{
		base.Awake();
		this.parent = base.GetComponentInParent<T>();
	}
}
