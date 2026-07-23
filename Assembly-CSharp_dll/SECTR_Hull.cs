using System;
using UnityEngine;

// Token: 0x02000098 RID: 152
public abstract class SECTR_Hull : MonoBehaviour
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x0600032E RID: 814 RVA: 0x00014C48 File Offset: 0x00012E48
	public Vector3[] VertsCW
	{
		get
		{
			this.ComputeVerts();
			return this.vertsCW;
		}
	}

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600032F RID: 815 RVA: 0x00014C56 File Offset: 0x00012E56
	public Vector3[] VertsCCW
	{
		get
		{
			this.ComputeVerts();
			return this.vertsCCW;
		}
	}

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000330 RID: 816 RVA: 0x00014C64 File Offset: 0x00012E64
	public Vector3 Normal
	{
		get
		{
			this.ComputeVerts();
			return base.transform.rotation * this.meshNormal;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000331 RID: 817 RVA: 0x00014C82 File Offset: 0x00012E82
	public Vector3 ReverseNormal
	{
		get
		{
			this.ComputeVerts();
			return base.transform.rotation * -this.meshNormal;
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x06000332 RID: 818 RVA: 0x00014CA8 File Offset: 0x00012EA8
	public Vector3 Center
	{
		get
		{
			this.ComputeVerts();
			return base.transform.localToWorldMatrix.MultiplyPoint3x4(this.meshCentroid);
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000333 RID: 819 RVA: 0x00014CD4 File Offset: 0x00012ED4
	public Plane HullPlane
	{
		get
		{
			this.ComputeVerts();
			return new Plane(this.Normal, this.Center);
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000334 RID: 820 RVA: 0x00014CED File Offset: 0x00012EED
	public Plane ReverseHullPlane
	{
		get
		{
			this.ComputeVerts();
			return new Plane(this.ReverseNormal, this.Center);
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000335 RID: 821 RVA: 0x00014D08 File Offset: 0x00012F08
	public Bounds BoundingBox
	{
		get
		{
			Bounds result = new Bounds(base.transform.position, Vector3.zero);
			if (this.HullMesh)
			{
				this.ComputeVerts();
				if (this.vertsCW != null)
				{
					Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
					int num = this.vertsCW.Length;
					for (int i = 0; i < num; i++)
					{
						result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.vertsCW[i]));
					}
				}
			}
			return result;
		}
	}

	// Token: 0x06000336 RID: 822 RVA: 0x00014D84 File Offset: 0x00012F84
	public bool IsPointInHull(Vector3 p, float distanceTolerance)
	{
		this.ComputeVerts();
		Vector3 a = base.transform.worldToLocalMatrix.MultiplyPoint3x4(p);
		Vector3 b = a - Vector3.Dot(a - this.meshCentroid, this.meshNormal) * this.meshNormal;
		if (this.vertsCW != null && Vector3.SqrMagnitude(a - b) < distanceTolerance * distanceTolerance)
		{
			float num = 6.2831855f;
			int num2 = this.vertsCW.Length;
			for (int i = 0; i < num2; i++)
			{
				Vector3 lhs = this.vertsCW[i] - b;
				Vector3 rhs = this.vertsCW[(i + 1) % num2] - b;
				float magnitude = lhs.magnitude;
				float magnitude2 = rhs.magnitude;
				float num3 = magnitude * magnitude2;
				if (num3 < 0.001f)
				{
					return true;
				}
				float f = Vector3.Dot(lhs, rhs) / num3;
				num -= Mathf.Acos(f);
			}
			return Mathf.Abs(num) < 0.001f;
		}
		return false;
	}

	// Token: 0x06000337 RID: 823 RVA: 0x00014E8C File Offset: 0x0001308C
	protected void ComputeVerts()
	{
		if (this.HullMesh != this.previousMesh)
		{
			if (this.HullMesh)
			{
				int vertexCount = this.HullMesh.vertexCount;
				this.vertsCW = new Vector3[vertexCount];
				this.vertsCCW = new Vector3[vertexCount];
				this.meshCentroid = Vector3.zero;
				for (int i = 0; i < vertexCount; i++)
				{
					Vector3 vector = this.HullMesh.vertices[i];
					this.vertsCW[i] = vector;
					this.meshCentroid += vector;
				}
				this.meshCentroid /= (float)this.HullMesh.vertexCount;
				this.meshNormal = Vector3.zero;
				int num = this.HullMesh.normals.Length;
				for (int j = 0; j < num; j++)
				{
					this.meshNormal += this.HullMesh.normals[j];
				}
				this.meshNormal /= (float)this.HullMesh.normals.Length;
				this.meshNormal.Normalize();
				bool flag = true;
				for (int k = 0; k < vertexCount; k++)
				{
					Vector3 a2 = this.vertsCW[k];
					Vector3 vector2 = a2 - Vector3.Dot(a2 - this.meshCentroid, this.meshNormal) * this.meshNormal;
					flag = (flag && Vector3.SqrMagnitude(a2 - vector2) < 0.001f);
					this.vertsCW[k] = vector2;
				}
				if (!flag)
				{
					Debug.LogWarning("Occluder mesh of " + base.name + " is not planar!");
				}
				Array.Sort<Vector3>(this.vertsCW, (Vector3 a, Vector3 b) => SECTR_Geometry.CompareVectorsCW(a, b, this.meshCentroid, this.meshNormal));
				if (!SECTR_Geometry.IsPolygonConvex(this.vertsCW))
				{
					Debug.LogWarning("Occluder mesh of " + base.name + " is not convex!");
				}
				this.vertsCCW = this.vertsCW;
				Array.Reverse((SECTR_Geometry.CompareVectorsCW(this.vertsCW[0], this.vertsCW[0], this.meshCentroid, this.meshNormal) >= 0) ? this.vertsCCW : this.vertsCW);
			}
			else
			{
				this.meshNormal = Vector3.zero;
				this.meshCentroid = Vector3.zero;
				this.vertsCW = null;
				this.vertsCCW = null;
			}
			this.previousMesh = this.HullMesh;
		}
	}

	// Token: 0x0400034C RID: 844
	private Mesh previousMesh;

	// Token: 0x0400034D RID: 845
	private Vector3[] vertsCW;

	// Token: 0x0400034E RID: 846
	private Vector3[] vertsCCW;

	// Token: 0x0400034F RID: 847
	private Vector3 meshCentroid = Vector3.zero;

	// Token: 0x04000350 RID: 848
	protected Vector3 meshNormal = Vector3.forward;

	// Token: 0x04000351 RID: 849
	[SECTR_ToolTip("Convex, planar mesh that defines the portal shape.")]
	public Mesh HullMesh;
}
