using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020004D3 RID: 1235
public class GlitchBreadcrumbNetwork : PathingNetwork
{
	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060019D6 RID: 6614 RVA: 0x00064F48 File Offset: 0x00063148
	public override Pather Pather
	{
		get
		{
			return this.pather;
		}
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x00064F50 File Offset: 0x00063150
	public void Update()
	{
		List<PathingNetworkNode> list = null;
		if (this.exitDestination != null && this.exitDestination.IsLinkActive())
		{
			Vector3 position = SRSingleton<SceneContext>.Instance.Player.transform.position;
			Vector3 position2 = this.exitDestination.transform.position;
			list = this.Pather.GeneratePathNodes(position, position2);
		}
		if (list == null != (this.activeBreadcrumbs == null) || (list != null && list[0] != this.activeBreadcrumbs[0]))
		{
			this.OnBreadcrumbsChanged(list);
		}
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x00064FE1 File Offset: 0x000631E1
	public void OnDisable()
	{
		this.OnBreadcrumbsChanged(null);
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x00064FEC File Offset: 0x000631EC
	public void OnGlitchRegionLoaded()
	{
		foreach (GlitchTeleportDestination glitchTeleportDestination in SRSingleton<GlitchRegionHelper>.Instance.destinations)
		{
			glitchTeleportDestination.onExitTeleporterBecameActive += this.OnExitTeleporterBecameActive;
		}
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x00065048 File Offset: 0x00063248
	private void OnExitTeleporterBecameActive(GlitchTeleportDestination destination)
	{
		this.exitDestination = destination;
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x00065054 File Offset: 0x00063254
	private void OnBreadcrumbsChanged(List<PathingNetworkNode> breadcrumbs)
	{
		if (this.activeBreadcrumbs != null)
		{
			this.activeBreadcrumbs.ForEach(delegate(GlitchBreadcrumbNetworkNode b)
			{
				b.Deactivate();
			});
			this.activeBreadcrumbs = null;
		}
		if (breadcrumbs != null && breadcrumbs.Any<PathingNetworkNode>())
		{
			this.activeBreadcrumbs = breadcrumbs.Cast<GlitchBreadcrumbNetworkNode>().ToList<GlitchBreadcrumbNetworkNode>();
			for (int i = 0; i < this.activeBreadcrumbs.Count; i++)
			{
				this.activeBreadcrumbs[i].Activate((i + 1 >= this.activeBreadcrumbs.Count) ? this.exitDestination.transform.position : this.activeBreadcrumbs[i + 1].position);
			}
		}
	}

	// Token: 0x0400197D RID: 6525
	private GlitchBreadcrumbNetworkPather pather = new GlitchBreadcrumbNetworkPather();

	// Token: 0x0400197E RID: 6526
	private List<GlitchBreadcrumbNetworkNode> activeBreadcrumbs;

	// Token: 0x0400197F RID: 6527
	private GlitchTeleportDestination exitDestination;
}
