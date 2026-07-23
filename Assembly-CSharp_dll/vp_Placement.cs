using System;
using UnityEngine;

// Token: 0x0200085B RID: 2139
public class vp_Placement
{
	// Token: 0x06002D27 RID: 11559 RVA: 0x000AB7E4 File Offset: 0x000A99E4
	public static bool AdjustPosition(vp_Placement p, float physicsRadius, int attempts = 1000)
	{
		attempts--;
		if (attempts > 0)
		{
			if (p.IsObstructed(physicsRadius))
			{
				Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
				p.Position.x = p.Position.x + insideUnitSphere.x;
				p.Position.z = p.Position.z + insideUnitSphere.z;
				vp_Placement.AdjustPosition(p, physicsRadius, attempts);
			}
			return true;
		}
		Debug.LogWarning("(vp_Placement.AdjustPosition) Failed to find valid placement.");
		return false;
	}

	// Token: 0x06002D28 RID: 11560 RVA: 0x000AB84B File Offset: 0x000A9A4B
	public virtual bool IsObstructed(float physicsRadius = 1f)
	{
		return Physics.CheckSphere(this.Position, physicsRadius, 270532864);
	}

	// Token: 0x06002D29 RID: 11561 RVA: 0x000AB864 File Offset: 0x000A9A64
	public static void SnapToGround(vp_Placement p, float radius, float snapDistance)
	{
		if (snapDistance == 0f)
		{
			return;
		}
		RaycastHit raycastHit;
		Physics.SphereCast(new Ray(p.Position + Vector3.up * snapDistance, Vector3.down), radius, out raycastHit, snapDistance * 2f, -675375893);
		if (raycastHit.collider != null)
		{
			p.Position.y = raycastHit.point.y + 0.05f;
		}
	}

	// Token: 0x04002B35 RID: 11061
	public Vector3 Position = Vector3.zero;

	// Token: 0x04002B36 RID: 11062
	public Quaternion Rotation = Quaternion.identity;
}
