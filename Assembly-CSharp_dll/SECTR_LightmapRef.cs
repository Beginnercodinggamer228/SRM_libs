using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AF RID: 175
[AddComponentMenu("")]
public class SECTR_LightmapRef : MonoBehaviour
{
	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000410 RID: 1040 RVA: 0x00018ADD File Offset: 0x00016CDD
	public List<SECTR_LightmapRef.RefData> LightmapRefs
	{
		get
		{
			return this.lightmapRefs;
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00018AE8 File Offset: 0x00016CE8
	public static void InitRefCounts()
	{
		int num = LightmapSettings.lightmaps.Length;
		if (SECTR_LightmapRef.globalLightmapRefCount == null || SECTR_LightmapRef.globalLightmapRefCount.Length != num)
		{
			SECTR_LightmapRef.globalLightmapRefCount = new int[num];
		}
		for (int i = 0; i < num; i++)
		{
			LightmapData lightmapData = LightmapSettings.lightmaps[i];
			SECTR_LightmapRef.globalLightmapRefCount[i] = ((lightmapData.lightmapColor || lightmapData.lightmapDir) ? 1 : 0);
		}
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00018B54 File Offset: 0x00016D54
	private void Start()
	{
		if ((!Application.isEditor || Application.isPlaying) && SECTR_LightmapRef.globalLightmapRefCount != null)
		{
			int num = LightmapSettings.lightmaps.Length;
			int count = this.lightmapRefs.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_LightmapRef.RefData refData = this.lightmapRefs[i];
				if (refData.index >= 0 && refData.index < SECTR_LightmapRef.globalLightmapRefCount.Length)
				{
					if (SECTR_LightmapRef.globalLightmapRefCount[refData.index] == 0)
					{
						LightmapData lightmapData = new LightmapData();
						lightmapData.lightmapDir = refData.NearLightmap;
						lightmapData.lightmapColor = refData.FarLightmap;
						LightmapData[] array = new LightmapData[num];
						for (int j = 0; j < num; j++)
						{
							if (refData.index == j)
							{
								array[j] = lightmapData;
							}
							else
							{
								array[j] = LightmapSettings.lightmaps[j];
							}
						}
						LightmapSettings.lightmaps = array;
					}
					SECTR_LightmapRef.globalLightmapRefCount[refData.index]++;
				}
			}
		}
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00018C50 File Offset: 0x00016E50
	private void OnDestroy()
	{
		if ((!Application.isEditor || Application.isPlaying) && SECTR_LightmapRef.globalLightmapRefCount != null)
		{
			int num = LightmapSettings.lightmaps.Length;
			int count = this.lightmapRefs.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_LightmapRef.RefData refData = this.lightmapRefs[i];
				if (refData.index >= 0 && refData.index < SECTR_LightmapRef.globalLightmapRefCount.Length)
				{
					SECTR_LightmapRef.globalLightmapRefCount[refData.index]--;
					if (SECTR_LightmapRef.globalLightmapRefCount[refData.index] == 0)
					{
						LightmapData[] array = new LightmapData[num];
						for (int j = 0; j < num; j++)
						{
							if (refData.index == j)
							{
								array[j] = new LightmapData();
							}
							else
							{
								array[j] = LightmapSettings.lightmaps[j];
							}
						}
						LightmapSettings.lightmaps = array;
					}
				}
			}
		}
	}

	// Token: 0x040003F8 RID: 1016
	[SerializeField]
	[HideInInspector]
	private List<SECTR_LightmapRef.RefData> lightmapRefs = new List<SECTR_LightmapRef.RefData>();

	// Token: 0x040003F9 RID: 1017
	private static int[] globalLightmapRefCount;

	// Token: 0x020000B0 RID: 176
	[Serializable]
	public class RefData
	{
		// Token: 0x040003FA RID: 1018
		public Texture2D FarLightmap;

		// Token: 0x040003FB RID: 1019
		public Texture2D NearLightmap;

		// Token: 0x040003FC RID: 1020
		public int index = -1;
	}
}
