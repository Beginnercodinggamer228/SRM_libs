using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AB RID: 171
[AddComponentMenu("")]
public class SECTR_ChunkRef : MonoBehaviour
{
	// Token: 0x060003F0 RID: 1008 RVA: 0x00018304 File Offset: 0x00016504
	public static SECTR_ChunkRef FindChunkRef(string chunkName)
	{
		int count = SECTR_ChunkRef.allChunkRefs.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_ChunkRef sectr_ChunkRef = SECTR_ChunkRef.allChunkRefs[i];
			if (sectr_ChunkRef.name == chunkName)
			{
				return sectr_ChunkRef;
			}
		}
		return null;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x00018345 File Offset: 0x00016545
	private void OnEnable()
	{
		SECTR_ChunkRef.allChunkRefs.Add(this);
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x00018352 File Offset: 0x00016552
	private void OnDisable()
	{
		SECTR_ChunkRef.allChunkRefs.Remove(this);
	}

	// Token: 0x040003E6 RID: 998
	private static List<SECTR_ChunkRef> allChunkRefs = new List<SECTR_ChunkRef>();

	// Token: 0x040003E7 RID: 999
	public Transform RealSector;

	// Token: 0x040003E8 RID: 1000
	public bool Recentered;
}
