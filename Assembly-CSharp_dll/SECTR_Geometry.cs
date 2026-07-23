using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000095 RID: 149
public static class SECTR_Geometry
{
	// Token: 0x0600031C RID: 796 RVA: 0x00013D28 File Offset: 0x00011F28
	public static Bounds ComputeBounds(Light light)
	{
		Bounds result;
		if (light)
		{
			switch (light.type)
			{
			case LightType.Spot:
			{
				Vector3 position = light.transform.position;
				result = new Bounds(position, Vector3.zero);
				Vector3 up = light.transform.up;
				Vector3 right = light.transform.right;
				Vector3 vector = position + light.transform.forward * light.range;
				float num = Mathf.Tan(light.spotAngle * 0.5f * 0.017453292f) * light.range;
				result.Encapsulate(vector);
				Vector3 a = vector + up * num;
				Vector3 a2 = vector + up * -num;
				Vector3 b = right * num;
				Vector3 b2 = right * -num;
				result.Encapsulate(a + b);
				result.Encapsulate(a + b2);
				result.Encapsulate(a2 + b);
				result.Encapsulate(a2 + b2);
				return result;
			}
			case LightType.Point:
			{
				float num2 = light.range * 2f;
				result = new Bounds(light.transform.position, new Vector3(num2, num2, num2));
				return result;
			}
			}
			result = new Bounds(light.transform.position, new Vector3(0.01f, 0.01f, 0.01f));
		}
		else
		{
			result = new Bounds(light.transform.position, new Vector3(0.01f, 0.01f, 0.01f));
		}
		return result;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x00013ED0 File Offset: 0x000120D0
	public static Bounds ComputeBounds(Terrain terrain)
	{
		if (terrain)
		{
			Vector3 vector = (terrain.terrainData != null) ? terrain.terrainData.size : Vector3.zero;
			Vector3 position = terrain.transform.position;
			return new Bounds(new Vector3(position.x + vector.x * 0.5f, position.y + vector.y * 0.5f, position.z + vector.z * 0.5f), vector);
		}
		return default(Bounds);
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00013F60 File Offset: 0x00012160
	public static bool FrustumIntersectsBounds(Bounds bounds, List<Plane> frustum, int inMask, out int outMask)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		outMask = 0;
		int num = 0;
		for (int i = 1; i <= inMask; i += i)
		{
			if ((i & inMask) != 0)
			{
				Plane plane = frustum[num];
				float num2 = center.x * plane.normal.x + center.y * plane.normal.y + center.z * plane.normal.z + plane.distance;
				float num3 = extents.x * Mathf.Abs(plane.normal.x) + extents.y * Mathf.Abs(plane.normal.y) + extents.z * Mathf.Abs(plane.normal.z);
				if (num2 + num3 < 0f)
				{
					return false;
				}
				outMask |= i;
			}
			num++;
		}
		return true;
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0001404C File Offset: 0x0001224C
	public static bool FrustumContainsBounds(Bounds bounds, List<Plane> frustum)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		int count = frustum.Count;
		for (int i = 0; i < count; i++)
		{
			Plane plane = frustum[i];
			float num = center.x * plane.normal.x + center.y * plane.normal.y + center.z * plane.normal.z + plane.distance;
			float num2 = extents.x * Mathf.Abs(plane.normal.x) + extents.y * Mathf.Abs(plane.normal.y) + extents.z * Mathf.Abs(plane.normal.z);
			if (num + num2 < 0f || num - num2 < 0f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00014137 File Offset: 0x00012337
	public static bool BoundsContainsBounds(Bounds container, Bounds contained)
	{
		return container.Contains(contained.min) && container.Contains(contained.max);
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00014159 File Offset: 0x00012359
	public static bool BoundsIntersectsSphere(Bounds bounds, Vector3 sphereCenter, float sphereRadius)
	{
		return Vector3.SqrMagnitude(Vector3.Min(Vector3.Max(sphereCenter, bounds.min), bounds.max) - sphereCenter) <= sphereRadius * sphereRadius;
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00014188 File Offset: 0x00012388
	public static Bounds ProjectBounds(Bounds bounds, Vector3 projection)
	{
		Vector3 point = bounds.min + projection;
		Vector3 point2 = bounds.max + projection;
		bounds.Encapsulate(point);
		bounds.Encapsulate(point2);
		return bounds;
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000141C4 File Offset: 0x000123C4
	public static bool IsPointInFrontOfPlane(Vector3 position, Vector3 center, Vector3 normal)
	{
		Vector3 normalized = (position - center).normalized;
		return Vector3.Dot(normal, normalized) > 0f;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x000141F0 File Offset: 0x000123F0
	public static bool IsPolygonConvex(Vector3[] verts)
	{
		int num = verts.Length;
		if (num < 3)
		{
			return false;
		}
		float num2 = (float)(num - 2) * 3.1415927f;
		for (int i = 0; i < num; i++)
		{
			Vector3 a = verts[i];
			Vector3 b = verts[(i + 1) % num];
			Vector3 a2 = verts[(i + 2) % num];
			Vector3 lhs = a - b;
			lhs.Normalize();
			Vector3 rhs = a2 - b;
			rhs.Normalize();
			num2 -= Mathf.Acos(Vector3.Dot(lhs, rhs));
		}
		return Mathf.Abs(num2) < 0.001f;
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00014280 File Offset: 0x00012480
	public static int CompareVectorsCW(Vector3 a, Vector3 b, Vector3 centroid, Vector3 normal)
	{
		Vector3 vector = Vector3.Cross(a - centroid, b - centroid);
		float magnitude = vector.magnitude;
		if (magnitude <= 0.001f)
		{
			return 0;
		}
		vector /= magnitude;
		if (Vector3.Dot(normal, vector) <= 0f)
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x0400033F RID: 831
	public const float kVERTEX_EPSILON = 0.001f;

	// Token: 0x04000340 RID: 832
	public const float kBOUNDS_CHEAT = 0.01f;
}
