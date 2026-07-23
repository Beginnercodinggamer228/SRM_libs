using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007E3 RID: 2019
[RequireComponent(typeof(MeshFilter))]
public class SoftNormalsToVertexColor : MonoBehaviour
{
	// Token: 0x06002A47 RID: 10823 RVA: 0x0009E662 File Offset: 0x0009C862
	private void OnDrawGizmos()
	{
		if (this.generateNow)
		{
			this.generateNow = false;
			this.TryGenerate();
		}
	}

	// Token: 0x06002A48 RID: 10824 RVA: 0x0009E679 File Offset: 0x0009C879
	private void Awake()
	{
		if (this.generateOnAwake)
		{
			this.TryGenerate();
		}
	}

	// Token: 0x06002A49 RID: 10825 RVA: 0x0009E68C File Offset: 0x0009C88C
	private void TryGenerate()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component == null)
		{
			Debug.LogError("MeshFilter missing on the vertex color generator", base.gameObject);
			return;
		}
		if (component.sharedMesh == null)
		{
			Debug.LogError("Assign a mesh to the MeshFilter before generating vertex colors", base.gameObject);
			return;
		}
		this.Generate(component.sharedMesh);
		Debug.Log("Vertex colors generated", base.gameObject);
	}

	// Token: 0x06002A4A RID: 10826 RVA: 0x0009E6F8 File Offset: 0x0009C8F8
	private void Generate(Mesh m)
	{
		Vector3[] normals = m.normals;
		Vector3[] vertices = m.vertices;
		Color[] array = new Color[normals.Length];
		List<List<int>> list = new List<List<int>>();
		for (int i = 0; i < vertices.Length; i++)
		{
			bool flag = false;
			foreach (List<int> list2 in list)
			{
				if (vertices[list2[0]] == vertices[i])
				{
					list2.Add(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(new List<int>
				{
					i
				});
			}
		}
		foreach (List<int> list3 in list)
		{
			Vector3 vector = Vector3.zero;
			foreach (int num in list3)
			{
				vector += normals[num];
			}
			vector.Normalize();
			if (this.method == SoftNormalsToVertexColor.Method.AngularDeviation)
			{
				float num2 = 0f;
				foreach (int num3 in list3)
				{
					num2 += Vector3.Dot(normals[num3], vector);
				}
				num2 /= (float)list3.Count;
				float num4 = Mathf.Acos(num2) * 57.29578f;
				float num5 = 180f - num4 - 90f;
				float d = 0.5f / Mathf.Sin(num5 * 0.017453292f);
				vector *= d;
			}
			foreach (int num6 in list3)
			{
				array[num6] = new Color(vector.x, vector.y, vector.z);
			}
		}
		m.colors = array;
	}

	// Token: 0x04002958 RID: 10584
	public SoftNormalsToVertexColor.Method method = SoftNormalsToVertexColor.Method.AngularDeviation;

	// Token: 0x04002959 RID: 10585
	public bool generateOnAwake;

	// Token: 0x0400295A RID: 10586
	public bool generateNow;

	// Token: 0x020007E4 RID: 2020
	public enum Method
	{
		// Token: 0x0400295C RID: 10588
		Simple,
		// Token: 0x0400295D RID: 10589
		AngularDeviation
	}
}
