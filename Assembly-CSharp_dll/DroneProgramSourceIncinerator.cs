using System;
using System.Collections.Generic;

// Token: 0x0200019B RID: 411
public class DroneProgramSourceIncinerator : DroneProgramSourceLandPlot
{
	// Token: 0x060008BA RID: 2234 RVA: 0x00028590 File Offset: 0x00026790
	protected override LandPlot.Id GetLandPlotID()
	{
		return LandPlot.Id.INCINERATOR;
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0002841F File Offset: 0x0002661F
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations(Identifiable source)
	{
		return base.GetTargetOrientations_Gather(source.gameObject, new DroneSubbehaviour.GatherConfig
		{
			distanceVertical = 1.25f
		});
	}
}
