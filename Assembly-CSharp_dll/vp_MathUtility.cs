using System;
using UnityEngine;

// Token: 0x0200083D RID: 2109
public static class vp_MathUtility
{
	// Token: 0x06002C37 RID: 11319 RVA: 0x000A6E61 File Offset: 0x000A5061
	public static float NaNSafeFloat(float value, float prevValue = 0f)
	{
		value = (double.IsNaN((double)value) ? prevValue : value);
		return value;
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x000A6E74 File Offset: 0x000A5074
	public static Vector2 NaNSafeVector2(Vector2 vector, Vector2 prevVector = default(Vector2))
	{
		vector.x = (double.IsNaN((double)vector.x) ? prevVector.x : vector.x);
		vector.y = (double.IsNaN((double)vector.y) ? prevVector.y : vector.y);
		return vector;
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x000A6EC8 File Offset: 0x000A50C8
	public static Vector3 NaNSafeVector3(Vector3 vector, Vector3 prevVector = default(Vector3))
	{
		vector.x = (double.IsNaN((double)vector.x) ? prevVector.x : vector.x);
		vector.y = (double.IsNaN((double)vector.y) ? prevVector.y : vector.y);
		vector.z = (double.IsNaN((double)vector.z) ? prevVector.z : vector.z);
		return vector;
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x000A6F40 File Offset: 0x000A5140
	public static Quaternion NaNSafeQuaternion(Quaternion quaternion, Quaternion prevQuaternion = default(Quaternion))
	{
		quaternion.x = (double.IsNaN((double)quaternion.x) ? prevQuaternion.x : quaternion.x);
		quaternion.y = (double.IsNaN((double)quaternion.y) ? prevQuaternion.y : quaternion.y);
		quaternion.z = (double.IsNaN((double)quaternion.z) ? prevQuaternion.z : quaternion.z);
		quaternion.w = (double.IsNaN((double)quaternion.w) ? prevQuaternion.w : quaternion.w);
		return quaternion;
	}

	// Token: 0x06002C3B RID: 11323 RVA: 0x000A6FDC File Offset: 0x000A51DC
	public static Vector3 SnapToZero(Vector3 value, float epsilon = 0.0001f)
	{
		value.x = ((Mathf.Abs(value.x) < epsilon) ? 0f : value.x);
		value.y = ((Mathf.Abs(value.y) < epsilon) ? 0f : value.y);
		value.z = ((Mathf.Abs(value.z) < epsilon) ? 0f : value.z);
		return value;
	}

	// Token: 0x06002C3C RID: 11324 RVA: 0x000A7050 File Offset: 0x000A5250
	public static float SnapToZero(float value, float epsilon = 0.0001f)
	{
		value = ((Mathf.Abs(value) < epsilon) ? 0f : value);
		return value;
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x000A7066 File Offset: 0x000A5266
	public static float ReduceDecimals(float value, float factor = 1000f)
	{
		return Mathf.Round(value * factor) / factor;
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x000A7072 File Offset: 0x000A5272
	public static bool IsOdd(int val)
	{
		return val % 2 != 0;
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x000A707A File Offset: 0x000A527A
	public static float Sinus(float rate, float amp, float offset = 0f)
	{
		return Mathf.Cos((Time.time + offset) * rate) * amp;
	}
}
