using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F9 RID: 761
public class CoopRegion : SRBehaviour
{
	// Token: 0x06001046 RID: 4166 RVA: 0x00041467 File Offset: 0x0003F667
	public void Awake()
	{
		CoopRegion.allCoops.Add(this);
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00041474 File Offset: 0x0003F674
	public void OnDestroy()
	{
		CoopRegion.allCoops.Remove(this);
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00041482 File Offset: 0x0003F682
	public void SetDeluxe()
	{
		this.isDeluxe = true;
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x0004148B File Offset: 0x0003F68B
	public static bool IsWithin(Vector3 pos)
	{
		return CoopRegion.IsWithin(pos, false);
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x00041494 File Offset: 0x0003F694
	public static bool IsWithinDeluxe(Vector3 pos)
	{
		return CoopRegion.IsWithin(pos, true);
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x000414A0 File Offset: 0x0003F6A0
	private static bool IsWithin(Vector3 pos, bool mustBeDeluxe)
	{
		bool flag = false;
		foreach (CoopRegion coopRegion in CoopRegion.allCoops)
		{
			if (coopRegion.GetComponent<Collider>().bounds.Contains(pos))
			{
				flag |= (!mustBeDeluxe || coopRegion.isDeluxe);
			}
		}
		return flag;
	}

	// Token: 0x04000F19 RID: 3865
	private static List<CoopRegion> allCoops = new List<CoopRegion>();

	// Token: 0x04000F1A RID: 3866
	private bool isDeluxe;
}
