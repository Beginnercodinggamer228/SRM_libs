using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000090 RID: 144
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Audio/SECTR Spline Source")]
public class SECTR_SplineSource : SECTR_PointSource
{
	// Token: 0x06000300 RID: 768 RVA: 0x00013236 File Offset: 0x00011436
	private void Awake()
	{
		this._SetupSpline();
	}

	// Token: 0x06000301 RID: 769 RVA: 0x0001323E File Offset: 0x0001143E
	protected override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00013246 File Offset: 0x00011446
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00013250 File Offset: 0x00011450
	private void Update()
	{
		if (this.instance && this.nodes.Count > 0)
		{
			Vector3 vector = this._GetClosestPointOnSpline(SECTR_AudioSystem.Listener.position);
			vector = base.transform.worldToLocalMatrix.MultiplyPoint3x4(vector);
			this.instance.LocalPosition = vector;
		}
	}

	// Token: 0x06000304 RID: 772 RVA: 0x000132AC File Offset: 0x000114AC
	private void _SetupSpline()
	{
		this.nodes.Clear();
		int count = this.SplinePoints.Count;
		if (count >= 2)
		{
			float num = this.Closed ? (1f / (float)count) : (1f / (float)(count - 1));
			int i;
			for (i = 0; i < count; i++)
			{
				Transform transform = this.SplinePoints[i];
				if (transform)
				{
					this.nodes.Add(new SECTR_SplineSource.SplineNode(transform.position, transform.rotation, num * (float)i, new Vector2(0f, 1f)));
				}
			}
			if (this.Closed && this.nodes.Count > 0)
			{
				float t = num * (float)i;
				this.nodes.Add(new SECTR_SplineSource.SplineNode(this.nodes[0]));
				this.nodes[this.nodes.Count - 1].T = t;
				Vector3 normalized = (this.nodes[1].Point - this.nodes[0].Point).normalized;
				Vector3 normalized2 = (this.nodes[this.nodes.Count - 2].Point - this.nodes[this.nodes.Count - 1].Point).normalized;
				float magnitude = (this.nodes[1].Point - this.nodes[0].Point).magnitude;
				float magnitude2 = (this.nodes[this.nodes.Count - 2].Point - this.nodes[this.nodes.Count - 1].Point).magnitude;
				SECTR_SplineSource.SplineNode splineNode = new SECTR_SplineSource.SplineNode(this.nodes[0]);
				splineNode.Point = this.nodes[0].Point + normalized2 * magnitude;
				SECTR_SplineSource.SplineNode splineNode2 = new SECTR_SplineSource.SplineNode(this.nodes[this.nodes.Count - 1]);
				splineNode2.Point = this.nodes[0].Point + normalized * magnitude2;
				this.nodes.Insert(0, splineNode);
				this.nodes.Add(splineNode2);
			}
			int count2 = this.nodes.Count;
			for (int j = 1; j < count2; j++)
			{
				SECTR_SplineSource.SplineNode splineNode3 = this.nodes[j];
				SECTR_SplineSource.SplineNode splineNode4 = this.nodes[j - 1];
				if (Quaternion.Dot(splineNode3.Rot, splineNode4.Rot) < 0f)
				{
					splineNode3.Rot.x = -splineNode3.Rot.x;
					splineNode3.Rot.y = -splineNode3.Rot.y;
					splineNode3.Rot.z = -splineNode3.Rot.z;
					splineNode3.Rot.w = -splineNode3.Rot.w;
				}
			}
			if (count2 > 0 && !this.Closed)
			{
				this.nodes.Insert(0, this.nodes[0]);
				this.nodes.Add(this.nodes[this.nodes.Count - 1]);
			}
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00013640 File Offset: 0x00011840
	private Vector3 _GetClosestPointOnSpline(Vector3 point)
	{
		Vector3 result = point;
		float num = float.MaxValue;
		int num2 = 20;
		for (int i = 0; i < num2; i++)
		{
			float timeParam = (float)i / (float)num2;
			Vector3 vector = this._GetHermiteAtT(timeParam);
			float num3 = Vector3.SqrMagnitude(point - vector);
			if (num3 < num)
			{
				num = num3;
				result = vector;
			}
		}
		return result;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00013690 File Offset: 0x00011890
	private Vector3 _GetHermiteAtT(float timeParam)
	{
		int count = this.nodes.Count;
		if (timeParam >= this.nodes[count - 2].T)
		{
			return this.nodes[count - 2].Point;
		}
		int num = 1;
		while (num < count - 2 && this.nodes[num].T <= timeParam)
		{
			num++;
		}
		int num2 = num - 1;
		float num3 = (timeParam - this.nodes[num2].T) / (this.nodes[num2 + 1].T - this.nodes[num2].T);
		num3 = this._Ease(num3, this.nodes[num2].EaseIO.x, this.nodes[num2].EaseIO.y);
		float num4 = num3 * num3;
		float num5 = num4 * num3;
		Vector3 point = this.nodes[num2 - 1].Point;
		Vector3 point2 = this.nodes[num2].Point;
		Vector3 point3 = this.nodes[num2 + 1].Point;
		Vector3 point4 = this.nodes[num2 + 2].Point;
		float d = 0.5f;
		Vector3 a = d * (point3 - point);
		Vector3 a2 = d * (point4 - point2);
		float d2 = 2f * num5 - 3f * num4 + 1f;
		float d3 = -2f * num5 + 3f * num4;
		float d4 = num5 - 2f * num4 + num3;
		float d5 = num5 - num4;
		return d2 * point2 + d3 * point3 + d4 * a + d5 * a2;
	}

	// Token: 0x06000307 RID: 775 RVA: 0x0001385C File Offset: 0x00011A5C
	private float _Ease(float t, float k1, float k2)
	{
		float num = k1 * 2f / 3.1415927f + k2 - k1 + (1f - k2) * 2f / 3.1415927f;
		float num2;
		if (t < k1)
		{
			num2 = k1 * 0.63661975f * (Mathf.Sin(t / k1 * 3.1415927f * 0.5f - 1.5707964f) + 1f);
		}
		else if (t < k2)
		{
			num2 = 2f * k1 / 3.1415927f + t - k1;
		}
		else
		{
			num2 = 2f * k1 / 3.1415927f + k2 - k1 + (1f - k2) * 0.63661975f * Mathf.Sin((t - k2) / (1f - k2) * 3.1415927f / 2f);
		}
		return num2 / num;
	}

	// Token: 0x04000324 RID: 804
	private List<SECTR_SplineSource.SplineNode> nodes = new List<SECTR_SplineSource.SplineNode>(8);

	// Token: 0x04000325 RID: 805
	[SECTR_ToolTip("Array of scene objects to use as control points for the spline")]
	public List<Transform> SplinePoints = new List<Transform>();

	// Token: 0x04000326 RID: 806
	[SECTR_ToolTip("Determines if the spline is open or closed (i.e. a loop).")]
	public bool Closed;

	// Token: 0x02000091 RID: 145
	private class SplineNode
	{
		// Token: 0x06000309 RID: 777 RVA: 0x00013935 File Offset: 0x00011B35
		public SplineNode(Vector3 p, Quaternion q, float t, Vector2 io)
		{
			this.Point = p;
			this.Rot = q;
			this.T = t;
			this.EaseIO = io;
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0001395A File Offset: 0x00011B5A
		public SplineNode(SECTR_SplineSource.SplineNode o)
		{
			this.Point = o.Point;
			this.Rot = o.Rot;
			this.T = o.T;
			this.EaseIO = o.EaseIO;
		}

		// Token: 0x04000327 RID: 807
		public Vector3 Point;

		// Token: 0x04000328 RID: 808
		public Quaternion Rot;

		// Token: 0x04000329 RID: 809
		public float T;

		// Token: 0x0400032A RID: 810
		public Vector2 EaseIO;
	}
}
