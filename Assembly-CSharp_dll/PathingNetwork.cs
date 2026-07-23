using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000251 RID: 593
public abstract class PathingNetwork : SRBehaviour
{
	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000CBD RID: 3261
	public abstract Pather Pather { get; }

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000CBE RID: 3262 RVA: 0x000348EE File Offset: 0x00032AEE
	public PathingNetworkNode[] Nodes
	{
		get
		{
			if (!(this.nodesParent != null))
			{
				return new PathingNetworkNode[0];
			}
			return this.nodesParent.GetComponentsInChildren<PathingNetworkNode>(true);
		}
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00034911 File Offset: 0x00032B11
	public virtual void Awake()
	{
		this.RecalculateNodeConnections();
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00034919 File Offset: 0x00032B19
	public void RecalculateNodeConnections()
	{
		this.Pather.RecalculateNodeConnections(this.Nodes, this.whitelistConnections, this.blacklistConnections);
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00034938 File Offset: 0x00032B38
	public Queue<Vector3> GeneratePath(Vector3 start, Vector3 end)
	{
		return this.Pather.GeneratePath(start, end);
	}

	// Token: 0x04000B8D RID: 2957
	[Tooltip("GameObject parenting the PathingNetworkNode.")]
	public GameObject nodesParent;

	// Token: 0x04000B8E RID: 2958
	[Tooltip("List of node pairings on the whitelist.")]
	public List<Pather.NodePair> whitelistConnections;

	// Token: 0x04000B8F RID: 2959
	[Tooltip("List of node pairings on the blacklist.")]
	public List<Pather.NodePair> blacklistConnections;

	// Token: 0x04000B90 RID: 2960
	[Tooltip("Enable/disable drawing of the network node gizmos.")]
	public bool drawNodeGizmos;

	// Token: 0x04000B91 RID: 2961
	[Tooltip("Enable/disable drawing of the network connection override gizmos.")]
	public bool drawOverrideGizmos;
}
