using System;
using System.Collections.Generic;

// Token: 0x02000412 RID: 1042
public class PhaseSite : IdHandler
{
	// Token: 0x060015C2 RID: 5570 RVA: 0x000548C7 File Offset: 0x00052AC7
	public void Awake()
	{
		PhaseSite.allSites.Add(this);
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x000548D4 File Offset: 0x00052AD4
	public void OnDestroy()
	{
		PhaseSite.allSites.Remove(this);
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x000548E2 File Offset: 0x00052AE2
	protected override string IdPrefix()
	{
		return "phaseSite";
	}

	// Token: 0x040014BB RID: 5307
	public static List<PhaseSite> allSites = new List<PhaseSite>();

	// Token: 0x040014BC RID: 5308
	public PhaseableObject phaseableObject;
}
