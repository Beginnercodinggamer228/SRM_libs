using System;
using UnityEngine;

// Token: 0x02000437 RID: 1079
public class RubberBoneEffect : MonoBehaviour
{
	// Token: 0x06001672 RID: 5746 RVA: 0x00056DC8 File Offset: 0x00054FC8
	public void Reset()
	{
		for (int i = 0; i < this.verts.Length; i++)
		{
			this.verts[i].Reset();
		}
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x00056DF8 File Offset: 0x00054FF8
	private void Start()
	{
		this.ownBody = base.GetComponentInParent<Rigidbody>();
		this.ownRenderer = ((this.skinRenderer != null) ? this.skinRenderer : base.GetComponentInParent<Renderer>());
		this.CheckPreset();
		this.invMass = 1f / this.mass;
		this.vacuumable = base.GetComponentInParent<Vacuumable>();
		this.verts = new RubberBoneEffect.VertexRubber[this.bones.Length];
		for (int i = 0; i < this.bones.Length; i++)
		{
			this.verts[i] = new RubberBoneEffect.VertexRubber(this.bones[i], this);
		}
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x00056E93 File Offset: 0x00055093
	public void OnEnable()
	{
		this.wasSleeping = true;
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x00056E9C File Offset: 0x0005509C
	private void LateUpdate()
	{
		bool flag = (this.ownBody != null && this.ownBody.IsSleeping()) || (this.ownRenderer != null && !this.ownRenderer.isVisible);
		if (flag)
		{
			if (!this.sleeping)
			{
				RubberBoneEffect.VertexRubber[] array = this.verts;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Reset();
				}
			}
			this.sleeping = true;
		}
		else
		{
			if (this.wasSleeping || base.transform.position != this.lastWorldPosition || base.transform.rotation != this.lastWorldRotation)
			{
				foreach (RubberBoneEffect.VertexRubber vertexRubber in this.verts)
				{
					if (this.wasSleeping)
					{
						vertexRubber.Reset();
					}
				}
				this.sleeping = false;
			}
			if (!this.sleeping)
			{
				float heldFactor = (this.vacuumable != null && this.vacuumable.isHeld()) ? this.vacHeldFactor : 1f;
				float num = (this.unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * 60f;
				float timeAdjDamping = Mathf.Pow(this.damping, num);
				RubberBoneEffect.VertexRubber[] array = this.verts;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].LateUpdate(num, timeAdjDamping, heldFactor);
				}
				this.lastWorldPosition = base.transform.position;
				this.lastWorldRotation = base.transform.rotation;
			}
		}
		this.wasSleeping = flag;
	}

	// Token: 0x06001676 RID: 5750 RVA: 0x00057030 File Offset: 0x00055230
	private void CheckPreset()
	{
		switch (this.Presets)
		{
		case RubberBoneEffect.RubberType.RubberDuck:
			this.gravity = 0f;
			this.mass = 2f;
			this.stiffness = 0.5f;
			this.damping = 0.85f;
			this.effectIntensity = 1f;
			return;
		case RubberBoneEffect.RubberType.HardRubber:
			this.gravity = 0f;
			this.mass = 8f;
			this.stiffness = 0.5f;
			this.damping = 0.9f;
			this.effectIntensity = 0.5f;
			return;
		case RubberBoneEffect.RubberType.Jelly:
			this.gravity = 0f;
			this.mass = 1f;
			this.stiffness = 0.95f;
			this.damping = 0.95f;
			this.effectIntensity = 1f;
			return;
		case RubberBoneEffect.RubberType.SoftLatex:
			this.gravity = 1f;
			this.mass = 0.9f;
			this.stiffness = 0.3f;
			this.damping = 0.25f;
			this.effectIntensity = 1f;
			return;
		case RubberBoneEffect.RubberType.Slime:
			this.gravity = 0.2f;
			this.mass = 6f;
			this.stiffness = 1f;
			this.damping = 0.75f;
			this.effectIntensity = 1f;
			return;
		case RubberBoneEffect.RubberType.SlimeTarr:
			this.gravity = 0.2f;
			this.mass = 8f;
			this.stiffness = 1f;
			this.damping = 0.85f;
			this.effectIntensity = 1f;
			return;
		default:
			return;
		}
	}

	// Token: 0x0400156F RID: 5487
	public RubberBoneEffect.RubberType Presets;

	// Token: 0x04001570 RID: 5488
	public float effectIntensity = 1f;

	// Token: 0x04001571 RID: 5489
	public float gravity;

	// Token: 0x04001572 RID: 5490
	public float damping = 0.7f;

	// Token: 0x04001573 RID: 5491
	public float mass = 1f;

	// Token: 0x04001574 RID: 5492
	public float stiffness = 0.2f;

	// Token: 0x04001575 RID: 5493
	[Tooltip("All the bones we want to manipulate and how much to manipulate them.")]
	public RubberBoneEffect.BoneEntry[] bones;

	// Token: 0x04001576 RID: 5494
	public SkinnedMeshRenderer skinRenderer;

	// Token: 0x04001577 RID: 5495
	[Tooltip("If the effect should ignore time scaling.")]
	public bool unscaledTime;

	// Token: 0x04001578 RID: 5496
	private float invMass;

	// Token: 0x04001579 RID: 5497
	private float vacHeldFactor = 0.1f;

	// Token: 0x0400157A RID: 5498
	private RubberBoneEffect.VertexRubber[] verts;

	// Token: 0x0400157B RID: 5499
	private bool sleeping = true;

	// Token: 0x0400157C RID: 5500
	private Vector3 lastWorldPosition;

	// Token: 0x0400157D RID: 5501
	private Quaternion lastWorldRotation;

	// Token: 0x0400157E RID: 5502
	private bool wasSleeping;

	// Token: 0x0400157F RID: 5503
	private Vacuumable vacuumable;

	// Token: 0x04001580 RID: 5504
	private Rigidbody ownBody;

	// Token: 0x04001581 RID: 5505
	private Renderer ownRenderer;

	// Token: 0x02000438 RID: 1080
	public enum RubberType
	{
		// Token: 0x04001583 RID: 5507
		Custom,
		// Token: 0x04001584 RID: 5508
		RubberDuck,
		// Token: 0x04001585 RID: 5509
		HardRubber,
		// Token: 0x04001586 RID: 5510
		Jelly,
		// Token: 0x04001587 RID: 5511
		SoftLatex,
		// Token: 0x04001588 RID: 5512
		Slime,
		// Token: 0x04001589 RID: 5513
		SlimeTarr
	}

	// Token: 0x02000439 RID: 1081
	[Serializable]
	public class BoneEntry
	{
		// Token: 0x0400158A RID: 5514
		public Transform trans;

		// Token: 0x0400158B RID: 5515
		public float intensity;
	}

	// Token: 0x0200043A RID: 1082
	internal class VertexRubber
	{
		// Token: 0x06001679 RID: 5753 RVA: 0x00057208 File Offset: 0x00055408
		public VertexRubber(RubberBoneEffect.BoneEntry bone, RubberBoneEffect effect)
		{
			this.bone = bone;
			this.effect = effect;
			this.pos = bone.trans.position;
			this.rootPos = bone.trans.localPosition;
			this.lastLocalPos = this.rootPos;
			this.intensity = bone.intensity * effect.effectIntensity;
			this.Reset();
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x00057270 File Offset: 0x00055470
		public void Reset()
		{
			Vector3 vector = this.rootPos + Vector3.down * ((this.effect.gravity * 5f - this.effect.damping) * 0.75f);
			Vector3 vector2 = this.effect.transform.TransformPoint(vector);
			this.lastLocalPos = vector;
			this.pos = vector2;
			this.force = Vector3.zero;
			this.vel = Vector3.zero;
			this.bone.trans.localPosition = vector;
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x00057300 File Offset: 0x00055500
		public void LateUpdate(float timeFactor, float timeAdjDamping, float heldFactor)
		{
			Vector3 a = this.effect.transform.TransformPoint(this.rootPos) - this.pos;
			this.force = a * (this.effect.stiffness * Mathf.Min(1f, a.magnitude));
			this.force.y = this.force.y - this.effect.gravity * 0.1f;
			this.vel = timeAdjDamping * (this.vel + this.force * (timeFactor * this.effect.invMass));
			this.pos += this.vel * timeFactor;
			Vector3 b = this.effect.transform.InverseTransformPoint(this.pos);
			this.bone.trans.localPosition = Vector3.Lerp(this.rootPos, b, this.intensity * heldFactor);
			this.lastLocalPos = this.bone.trans.localPosition;
		}

		// Token: 0x0400158C RID: 5516
		private RubberBoneEffect effect;

		// Token: 0x0400158D RID: 5517
		public Vector3 pos;

		// Token: 0x0400158E RID: 5518
		public Vector3 vel;

		// Token: 0x0400158F RID: 5519
		public Vector3 force;

		// Token: 0x04001590 RID: 5520
		public Vector3 rootPos;

		// Token: 0x04001591 RID: 5521
		public float intensity;

		// Token: 0x04001592 RID: 5522
		public Vector3 lastLocalPos;

		// Token: 0x04001593 RID: 5523
		private RubberBoneEffect.BoneEntry bone;
	}
}
