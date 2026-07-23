using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200043B RID: 1083
public class RubberEffect : MonoBehaviour
{
	// Token: 0x0600167C RID: 5756 RVA: 0x00057418 File Offset: 0x00055618
	private void Start()
	{
		this.CheckPreset();
		this.invMass = 1f / this.mass;
		this.vacuumable = base.GetComponentInParent<Vacuumable>();
		MeshFilter meshFilter = (MeshFilter)base.GetComponent(typeof(MeshFilter));
		this.originalMesh = meshFilter.sharedMesh;
		this.workingMesh = UnityEngine.Object.Instantiate<Mesh>(meshFilter.sharedMesh);
		meshFilter.sharedMesh = this.workingMesh;
		List<int> list = new List<int>();
		Color[] colors = this.originalMesh.colors;
		Vector3[] vertices = this.originalMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			if (colors[i].grayscale != 1f)
			{
				list.Add(i);
			}
		}
		this.verts = new RubberEffect.VertexRubber[list.Count];
		for (int j = 0; j < list.Count; j++)
		{
			int num = list[j];
			this.verts[j] = new RubberEffect.VertexRubber(base.transform.TransformPoint(vertices[num]), this);
			this.verts[j].colorIntensity = (1f - colors[num].grayscale) * this.effectIntensity;
			this.verts[j].indexId = num;
		}
		this.workingMeshVectors = this.originalMesh.vertices;
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x0005756F File Offset: 0x0005576F
	public void OnEnable()
	{
		this.wasVisible = false;
	}

	// Token: 0x0600167E RID: 5758 RVA: 0x00057578 File Offset: 0x00055778
	private void LateUpdate()
	{
		if (!base.GetComponent<Renderer>().isVisible)
		{
			if (!this.sleeping)
			{
				this.workingMesh.vertices = this.originalMesh.vertices;
				this.workingMesh.RecalculateBounds();
				this.sleeping = true;
				RubberEffect.VertexRubber[] array = this.verts;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].vertSleeping = true;
				}
			}
		}
		else
		{
			if (!this.wasVisible || base.transform.position != this.lastWorldPosition || base.transform.rotation != this.lastWorldRotation)
			{
				foreach (RubberEffect.VertexRubber vertexRubber in this.verts)
				{
					if (vertexRubber.vertSleeping || !this.wasVisible)
					{
						vertexRubber.Reset();
					}
				}
				this.sleeping = false;
			}
			if (!this.sleeping)
			{
				float num = this.vacuumable.isHeld() ? this.vacHeldFactor : 1f;
				this.workingMeshVectors = this.originalMesh.vertices;
				int num2 = 0;
				float num3 = Time.deltaTime / 0.016667f;
				float timeAdjDamping = Mathf.Pow(this.damping, num3);
				foreach (RubberEffect.VertexRubber vertexRubber2 in this.verts)
				{
					if (vertexRubber2.vertSleeping)
					{
						num2++;
					}
					else
					{
						Vector3 target = base.transform.TransformPoint(this.workingMeshVectors[vertexRubber2.indexId]);
						vertexRubber2.Update(target, num3, timeAdjDamping);
					}
					Vector3 b = base.transform.InverseTransformPoint(vertexRubber2.pos);
					this.workingMeshVectors[vertexRubber2.indexId] = Vector3.Lerp(this.workingMeshVectors[vertexRubber2.indexId], b, vertexRubber2.colorIntensity * num);
					if (vertexRubber2.vertSleeping)
					{
						num2++;
					}
				}
				this.workingMesh.vertices = this.workingMeshVectors;
				this.workingMesh.RecalculateBounds();
				if (base.transform.position == this.lastWorldPosition && base.transform.rotation == this.lastWorldRotation)
				{
					if (num2 == this.verts.Length)
					{
						this.sleeping = true;
					}
				}
				else
				{
					this.lastWorldPosition = base.transform.position;
					this.lastWorldRotation = base.transform.rotation;
				}
			}
		}
		this.wasVisible = base.GetComponent<Renderer>().isVisible;
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x000577F4 File Offset: 0x000559F4
	private void CheckPreset()
	{
		switch (this.Presets)
		{
		case RubberEffect.RubberType.RubberDuck:
			this.gravity = 0f;
			this.mass = 2f;
			this.stiffness = 0.5f;
			this.damping = 0.85f;
			this.effectIntensity = 1f;
			return;
		case RubberEffect.RubberType.HardRubber:
			this.gravity = 0f;
			this.mass = 8f;
			this.stiffness = 0.5f;
			this.damping = 0.9f;
			this.effectIntensity = 0.5f;
			return;
		case RubberEffect.RubberType.Jelly:
			this.gravity = 0f;
			this.mass = 1f;
			this.stiffness = 0.95f;
			this.damping = 0.95f;
			this.effectIntensity = 1f;
			return;
		case RubberEffect.RubberType.SoftLatex:
			this.gravity = 1f;
			this.mass = 0.9f;
			this.stiffness = 0.3f;
			this.damping = 0.25f;
			this.effectIntensity = 1f;
			return;
		case RubberEffect.RubberType.Slime:
			this.gravity = 0.9f;
			this.mass = 6f;
			this.stiffness = 0.333f;
			this.damping = 0.85f;
			this.effectIntensity = 1f;
			return;
		case RubberEffect.RubberType.SlimeTarr:
			this.gravity = 0.9f;
			this.mass = 8f;
			this.stiffness = 0.333f;
			this.damping = 0.85f;
			this.effectIntensity = 1f;
			return;
		default:
			return;
		}
	}

	// Token: 0x04001594 RID: 5524
	public RubberEffect.RubberType Presets;

	// Token: 0x04001595 RID: 5525
	public float effectIntensity = 1f;

	// Token: 0x04001596 RID: 5526
	public float gravity;

	// Token: 0x04001597 RID: 5527
	public float damping = 0.7f;

	// Token: 0x04001598 RID: 5528
	public float mass = 1f;

	// Token: 0x04001599 RID: 5529
	public float stiffness = 0.2f;

	// Token: 0x0400159A RID: 5530
	private float invMass;

	// Token: 0x0400159B RID: 5531
	private float vacHeldFactor = 0.33f;

	// Token: 0x0400159C RID: 5532
	private Mesh workingMesh;

	// Token: 0x0400159D RID: 5533
	private Mesh originalMesh;

	// Token: 0x0400159E RID: 5534
	private RubberEffect.VertexRubber[] verts;

	// Token: 0x0400159F RID: 5535
	private Vector3[] workingMeshVectors;

	// Token: 0x040015A0 RID: 5536
	private bool sleeping = true;

	// Token: 0x040015A1 RID: 5537
	private Vector3 lastWorldPosition;

	// Token: 0x040015A2 RID: 5538
	private Quaternion lastWorldRotation;

	// Token: 0x040015A3 RID: 5539
	private bool wasVisible;

	// Token: 0x040015A4 RID: 5540
	private Vacuumable vacuumable;

	// Token: 0x0200043C RID: 1084
	public enum RubberType
	{
		// Token: 0x040015A6 RID: 5542
		Custom,
		// Token: 0x040015A7 RID: 5543
		RubberDuck,
		// Token: 0x040015A8 RID: 5544
		HardRubber,
		// Token: 0x040015A9 RID: 5545
		Jelly,
		// Token: 0x040015AA RID: 5546
		SoftLatex,
		// Token: 0x040015AB RID: 5547
		Slime,
		// Token: 0x040015AC RID: 5548
		SlimeTarr
	}

	// Token: 0x0200043D RID: 1085
	internal class VertexRubber
	{
		// Token: 0x06001681 RID: 5761 RVA: 0x000579C9 File Offset: 0x00055BC9
		public VertexRubber(Vector3 target, RubberEffect effect)
		{
			this.effect = effect;
			this.pos = target;
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x000579E0 File Offset: 0x00055BE0
		public void Reset()
		{
			Vector3 vector = this.effect.transform.TransformPoint(this.effect.originalMesh.vertices[this.indexId]);
			this.lastVel = Vector3.zero;
			this.lastForce = Vector3.zero;
			this.lastPos = vector;
			this.pos = vector;
			this.force = Vector3.zero;
			this.vel = Vector3.zero;
			this.vertSleeping = false;
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x00057A5C File Offset: 0x00055C5C
		public void Update(Vector3 target, float timeFactor, float timeAdjDamping)
		{
			if (!this.vertSleeping)
			{
				this.force = (target - this.pos) * this.effect.stiffness;
				this.force.y = this.force.y - this.effect.gravity * 0.1f;
				this.vel = timeAdjDamping * (this.vel + this.force * (timeFactor * this.effect.invMass));
				this.pos += this.vel * timeFactor;
				if (this.pos == this.lastPos && this.vel == this.lastVel && this.force == this.lastForce)
				{
					this.vertSleeping = true;
					return;
				}
				this.lastPos = this.pos;
				this.lastVel = this.vel;
				this.lastForce = this.force;
			}
		}

		// Token: 0x040015AD RID: 5549
		public int indexId;

		// Token: 0x040015AE RID: 5550
		private RubberEffect effect;

		// Token: 0x040015AF RID: 5551
		public Vector3 pos;

		// Token: 0x040015B0 RID: 5552
		public Vector3 vel;

		// Token: 0x040015B1 RID: 5553
		public Vector3 force;

		// Token: 0x040015B2 RID: 5554
		public Vector3 lastPos;

		// Token: 0x040015B3 RID: 5555
		public Vector3 lastVel;

		// Token: 0x040015B4 RID: 5556
		public Vector3 lastForce;

		// Token: 0x040015B5 RID: 5557
		public bool vertSleeping;

		// Token: 0x040015B6 RID: 5558
		public float colorIntensity;
	}
}
