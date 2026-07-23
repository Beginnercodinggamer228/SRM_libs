using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B5 RID: 181
[RequireComponent(typeof(SECTR_Member))]
[AddComponentMenu("SECTR/Stream/SECTR Neighbor Loader")]
public class SECTR_NeighborLoader : SECTR_Loader
{
	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000423 RID: 1059 RVA: 0x000193B0 File Offset: 0x000175B0
	public override bool Loaded
	{
		get
		{
			bool flag = true;
			int count = this.currentSectors.Count;
			int num = 0;
			while (num < count && flag)
			{
				SECTR_Sector sectr_Sector = this.currentSectors[num];
				if (sectr_Sector.Frozen)
				{
					SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
					if (component && !component.IsLoaded())
					{
						flag = false;
						break;
					}
				}
				num++;
			}
			return flag;
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0001940F File Offset: 0x0001760F
	private void OnEnable()
	{
		this.cachedMember = base.GetComponent<SECTR_Member>();
		this.cachedMember.Changed += this._MembershipChanged;
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00019434 File Offset: 0x00017634
	private void OnDisable()
	{
		this.cachedMember.Changed -= this._MembershipChanged;
		if (this.currentSectors.Count > 0)
		{
			this._MembershipChanged(this.currentSectors, null);
		}
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x00019468 File Offset: 0x00017668
	private void Start()
	{
		base.LockSelf(true);
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00019471 File Offset: 0x00017671
	private void Update()
	{
		if (this.locked && this.Loaded)
		{
			base.LockSelf(false);
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x0001948C File Offset: 0x0001768C
	private void _MembershipChanged(List<SECTR_Sector> left, List<SECTR_Sector> joined)
	{
		if (joined != null)
		{
			int count = joined.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_Sector sectr_Sector = joined[i];
				if (sectr_Sector && !this.currentSectors.Contains(sectr_Sector))
				{
					SECTR_Graph.BreadthWalk(ref this.neighbors, sectr_Sector, (SECTR_Portal.PortalFlags)0, this.MaxDepth);
					int count2 = this.neighbors.Count;
					for (int j = 0; j < count2; j++)
					{
						SECTR_Chunk component = this.neighbors[j].Sector.GetComponent<SECTR_Chunk>();
						if (component)
						{
							component.AddReference();
						}
					}
					this.currentSectors.Add(sectr_Sector);
				}
			}
		}
		if (left != null)
		{
			int count3 = left.Count;
			for (int k = 0; k < count3; k++)
			{
				SECTR_Sector sectr_Sector2 = left[k];
				if (sectr_Sector2 && this.currentSectors.Contains(sectr_Sector2))
				{
					SECTR_Graph.BreadthWalk(ref this.neighbors, sectr_Sector2, (SECTR_Portal.PortalFlags)0, this.MaxDepth);
					int count4 = this.neighbors.Count;
					for (int l = 0; l < count4; l++)
					{
						SECTR_Chunk component2 = this.neighbors[l].Sector.GetComponent<SECTR_Chunk>();
						if (component2)
						{
							component2.RemoveReference();
						}
					}
					this.currentSectors.Remove(sectr_Sector2);
				}
			}
		}
	}

	// Token: 0x04000412 RID: 1042
	private SECTR_Member cachedMember;

	// Token: 0x04000413 RID: 1043
	private List<SECTR_Sector> currentSectors = new List<SECTR_Sector>(4);

	// Token: 0x04000414 RID: 1044
	private List<SECTR_Graph.Node> neighbors = new List<SECTR_Graph.Node>(8);

	// Token: 0x04000415 RID: 1045
	[SECTR_ToolTip("Determines how far out to load neighbor sectors from the current sector. Depth of 0 means only the current Sector.")]
	public int MaxDepth = 1;
}
