using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class PathingNetworkNode : SRBehaviour
{
	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000CC3 RID: 3267 RVA: 0x00034947 File Offset: 0x00032B47
	public Vector3 position
	{
		get
		{
			return this.nodeLoc.position;
		}
	}

	// Token: 0x04000B92 RID: 2962
	[Tooltip("List of other nodes connected to this node.")]
	public List<PathingNetworkNode> connections;

	// Token: 0x04000B93 RID: 2963
	[Tooltip("Location transform.")]
	public Transform nodeLoc;
}
