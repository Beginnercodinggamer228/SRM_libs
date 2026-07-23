using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class DronePather : Pather
{
	// Token: 0x0600077D RID: 1917 RVA: 0x000257CF File Offset: 0x000239CF
	public DronePather(float maxConnDist)
	{
		this.sqrMaxConnectionDist = maxConnDist * maxConnDist;
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x000257E0 File Offset: 0x000239E0
	private static bool PathIsBlocked(Vector3 start, Vector3 end)
	{
		Vector3 vector = end - start;
		RaycastHit raycastHit;
		return Physics.SphereCast(start, 0.5f, vector.normalized, out raycastHit, vector.magnitude, -537968901);
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00025818 File Offset: 0x00023A18
	protected override bool PathPredicate(Vector3 start, Vector3 end)
	{
		return (start - end).sqrMagnitude <= this.sqrMaxConnectionDist && !DronePather.PathIsBlocked(start, end);
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00025848 File Offset: 0x00023A48
	protected override bool NearestAccessibleNodePredicate(Vector3 start, Vector3 end)
	{
		return this.PathPredicate(start, end);
	}

	// Token: 0x040006E0 RID: 1760
	private float sqrMaxConnectionDist;
}
