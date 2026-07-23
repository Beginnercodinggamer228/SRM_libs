using System;
using System.Collections.Generic;

// Token: 0x02000197 RID: 407
public class DroneProgramSourceFreeRange : DroneProgramSourceDynamic
{
	// Token: 0x060008AC RID: 2220 RVA: 0x000283F0 File Offset: 0x000265F0
	protected override GardenDroneSubnetwork GetSubnetwork()
	{
		DroneNetwork.LandPlotMetadata containing = this.drone.network.GetContaining(this.source);
		if (containing == null)
		{
			return null;
		}
		return containing.subnetwork;
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0002841F File Offset: 0x0002661F
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations(Identifiable source)
	{
		return base.GetTargetOrientations_Gather(source.gameObject, new DroneSubbehaviour.GatherConfig
		{
			distanceVertical = 1.25f
		});
	}
}
