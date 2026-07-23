using System;
using System.Collections.Generic;

// Token: 0x020001A6 RID: 422
public class DroneProgramSourcePond : DroneProgramSourceLandPlot
{
	// Token: 0x060008E5 RID: 2277 RVA: 0x00028A3E File Offset: 0x00026C3E
	protected override LandPlot.Id GetLandPlotID()
	{
		return LandPlot.Id.POND;
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0002841F File Offset: 0x0002661F
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations(Identifiable source)
	{
		return base.GetTargetOrientations_Gather(source.gameObject, new DroneSubbehaviour.GatherConfig
		{
			distanceVertical = 1.25f
		});
	}
}
