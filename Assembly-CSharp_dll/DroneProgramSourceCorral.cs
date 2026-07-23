using System;

// Token: 0x0200018F RID: 399
public class DroneProgramSourceCorral : DroneProgramSourceLandPlot
{
	// Token: 0x06000880 RID: 2176 RVA: 0x0002764B File Offset: 0x0002584B
	protected override LandPlot.Id GetLandPlotID()
	{
		return LandPlot.Id.CORRAL;
	}
}
