using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class CorralRegion : SRBehaviour
{
	// Token: 0x0600104E RID: 4174 RVA: 0x00041520 File Offset: 0x0003F720
	public void Awake()
	{
		CorralRegion.allCorrals.Add(this);
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x0004152D File Offset: 0x0003F72D
	public void OnDestroy()
	{
		CorralRegion.allCorrals.Remove(this);
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x0004153C File Offset: 0x0003F73C
	public static bool IsWithin(Vector3 pos)
	{
		using (List<CorralRegion>.Enumerator enumerator = CorralRegion.allCorrals.GetEnumerator())
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

	// Token: 0x04000F1B RID: 3867
	private static List<CorralRegion> allCorrals = new List<CorralRegion>();
}
