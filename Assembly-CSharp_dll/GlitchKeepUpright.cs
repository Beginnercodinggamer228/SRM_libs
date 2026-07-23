using System;

// Token: 0x020004E0 RID: 1248
public class GlitchKeepUpright : KeepUpright
{
	// Token: 0x06001A23 RID: 6691 RVA: 0x00065CA1 File Offset: 0x00063EA1
	public void Awake()
	{
		this.vacuumable = base.GetComponent<Vacuumable>();
	}

	// Token: 0x06001A24 RID: 6692 RVA: 0x00065CAF File Offset: 0x00063EAF
	public override void RegistryFixedUpdate()
	{
		if (this.vacuumable.isCaptive())
		{
			return;
		}
		base.RegistryFixedUpdate();
	}

	// Token: 0x040019AB RID: 6571
	private Vacuumable vacuumable;
}
