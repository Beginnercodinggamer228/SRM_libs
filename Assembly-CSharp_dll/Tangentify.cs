using System;
using UnityEngine;

// Token: 0x02000520 RID: 1312
public class Tangentify
{
	// Token: 0x06001B6A RID: 7018 RVA: 0x00068DE0 File Offset: 0x00066FE0
	public static void AddTangents(Mesh mesh)
	{
		int num = mesh.triangles.Length / 3;
		int num2 = mesh.vertices.Length;
		Vector3[] array = new Vector3[num2];
		Vector3[] array2 = new Vector3[num2];
		Vector4[] array3 = new Vector4[num2];
		for (long num3 = 0L; num3 < (long)num; num3 += 3L)
		{
			long num4 = (long)mesh.triangles[(int)(checked((IntPtr)num3))];
			long num5 = (long)mesh.triangles[(int)(checked((IntPtr)(unchecked(num3 + 1L))))];
			long num6 = (long)mesh.triangles[(int)(checked((IntPtr)(unchecked(num3 + 2L))))];
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector2 vector4;
			Vector2 vector5;
			Vector2 vector6;
			checked
			{
				vector = mesh.vertices[(int)((IntPtr)num4)];
				vector2 = mesh.vertices[(int)((IntPtr)num5)];
				vector3 = mesh.vertices[(int)((IntPtr)num6)];
				vector4 = mesh.uv[(int)((IntPtr)num4)];
				vector5 = mesh.uv[(int)((IntPtr)num5)];
				vector6 = mesh.uv[(int)((IntPtr)num6)];
			}
			float num7 = vector2.x - vector.x;
			float num8 = vector3.x - vector.x;
			float num9 = vector2.y - vector.y;
			float num10 = vector3.y - vector.y;
			float num11 = vector2.z - vector.z;
			float num12 = vector3.z - vector.z;
			float num13 = vector5.x - vector4.x;
			float num14 = vector6.x - vector4.x;
			float num15 = vector5.y - vector4.y;
			float num16 = vector6.y - vector4.y;
			float num17 = 1f / (num13 * num16 - num14 * num15);
			Vector3 b = new Vector3((num16 * num7 - num15 * num8) * num17, (num16 * num9 - num15 * num10) * num17, (num16 * num11 - num15 * num12) * num17);
			Vector3 b2 = new Vector3((num13 * num8 - num14 * num7) * num17, (num13 * num10 - num14 * num9) * num17, (num13 * num12 - num14 * num11) * num17);
			checked
			{
				array[(int)((IntPtr)num4)] += b;
				array[(int)((IntPtr)num5)] += b;
				array[(int)((IntPtr)num6)] += b;
				array2[(int)((IntPtr)num4)] += b2;
				array2[(int)((IntPtr)num5)] += b2;
				array2[(int)((IntPtr)num6)] += b2;
			}
		}
		for (long num18 = 0L; num18 < (long)num2; num18 += 1L)
		{
			checked
			{
				Vector3 vector7 = mesh.normals[(int)((IntPtr)num18)];
				Vector3 vector8 = array[(int)((IntPtr)num18)];
				Vector3 normalized = (vector8 - vector7 * Vector3.Dot(vector7, vector8)).normalized;
				array3[(int)((IntPtr)num18)] = new Vector4(normalized.x, normalized.y, normalized.z);
				array3[(int)((IntPtr)num18)].w = ((Vector3.Dot(Vector3.Cross(vector7, vector8), array2[(int)((IntPtr)num18)]) < 0f) ? -1f : 1f);
			}
		}
		mesh.tangents = array3;
	}
}
