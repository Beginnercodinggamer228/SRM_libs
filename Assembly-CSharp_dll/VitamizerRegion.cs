using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200034C RID: 844
public class VitamizerRegion : SRBehaviour
{
	// Token: 0x060011B1 RID: 4529 RVA: 0x00046E29 File Offset: 0x00045029
	public void Awake()
	{
		VitamizerRegion.allVitamizers.Add(this);
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x00046E36 File Offset: 0x00045036
	public void OnDestroy()
	{
		VitamizerRegion.allVitamizers.Remove(this);
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x00046E44 File Offset: 0x00045044
	public static bool IsWithin(Vector3 pos)
	{
		using (List<VitamizerRegion>.Enumerator enumerator = VitamizerRegion.allVitamizers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetComponent<Collider>().bounds.Contains(pos))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x040010EF RID: 4335
	public const float TWINS_PROB = 0.5f;

	// Token: 0x040010F0 RID: 4336
	private static List<VitamizerRegion> allVitamizers = new List<VitamizerRegion>();
}
