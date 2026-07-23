using System;

// Token: 0x020001D3 RID: 467
public class GardenDroneSubnetwork : PathingNetwork
{
	// Token: 0x17000132 RID: 306
	// (get) Token: 0x060009D9 RID: 2521 RVA: 0x0002B909 File Offset: 0x00029B09
	public override Pather Pather
	{
		get
		{
			return this.pather;
		}
	}

	// Token: 0x0400082E RID: 2094
	private const float MAX_CONNECTION_DIST = 10f;

	// Token: 0x0400082F RID: 2095
	private DronePather pather = new DronePather(10f);
}
