using System;
using System.Linq;
using UnityEngine;

// Token: 0x020001F3 RID: 499
public class GadgetPortableSlimeBaitAttractor : Attractor
{
	// Token: 0x06000A71 RID: 2673 RVA: 0x0002D27C File Offset: 0x0002B47C
	public void Awake()
	{
		Gadget.Id id = base.GetComponentInParent<Gadget>().id;
		if (id == Gadget.Id.PORTABLE_SLIME_BAIT_FRUIT)
		{
			this.bait = Identifiable.FRUIT_CLASS.First<Identifiable.Id>();
			return;
		}
		if (id == Gadget.Id.PORTABLE_SLIME_BAIT_VEGGIE)
		{
			this.bait = Identifiable.VEGGIE_CLASS.First<Identifiable.Id>();
			return;
		}
		if (id != Gadget.Id.PORTABLE_SLIME_BAIT_MEAT)
		{
			Log.Error("Failed to get bait type for GadgetPortableSlimeBaitAttractor.", new object[]
			{
				"gadget",
				base.GetComponentInParent<Gadget>().id
			});
			return;
		}
		this.bait = Identifiable.MEAT_CLASS.First<Identifiable.Id>();
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0002D310 File Offset: 0x0002B510
	public override float AweFactor(GameObject slime)
	{
		SlimeEat component = slime.GetComponent<SlimeEat>();
		return (float)((component != null && component.enabled && component.DoesEat(this.bait)) ? 1 : 0);
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public override bool CauseMoveTowards()
	{
		return true;
	}

	// Token: 0x0400088C RID: 2188
	private Identifiable.Id bait;
}
