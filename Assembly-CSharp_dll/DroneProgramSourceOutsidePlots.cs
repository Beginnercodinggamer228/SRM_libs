using System;

// Token: 0x020001A2 RID: 418
public class DroneProgramSourceOutsidePlots : DroneProgramSourceDynamic
{
	// Token: 0x060008DB RID: 2267 RVA: 0x00028983 File Offset: 0x00026B83
	protected override bool SourcePredicate(DroneNetwork.LandPlotMetadata metadata, Identifiable source)
	{
		return metadata == null && base.SourcePredicate(metadata, source);
	}
}
