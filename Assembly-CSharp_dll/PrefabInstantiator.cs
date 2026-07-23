using System;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020002ED RID: 749
public interface PrefabInstantiator
{
	// Token: 0x06001015 RID: 4117
	GameObject InstantiateActor(long actorId, Identifiable.Id id, RegionRegistry.RegionSetId regionSetId, Vector3 pos, Vector3 rot, GameModel gameModel);

	// Token: 0x06001016 RID: 4118
	GameObject InstantiateGadget(Gadget.Id id, GadgetSiteModel site, GameModel gameModel);

	// Token: 0x06001017 RID: 4119
	void InstantiatePlot(LandPlot.Id id, LandPlotModel plotModel, bool expectingPush);
}
