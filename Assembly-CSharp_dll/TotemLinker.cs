using System;
using UnityEngine;

// Token: 0x020004B1 RID: 1201
public class TotemLinker : RegisteredActorBehaviour, RegistryFixedUpdateable, RegistryUpdateable
{
	// Token: 0x0600191A RID: 6426 RVA: 0x00061E09 File Offset: 0x00060009
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.emotions = base.GetComponentInParent<SlimeEmotions>();
		this.vacuumable = base.GetComponentInParent<Vacuumable>();
		this.body = base.GetComponentInParent<Rigidbody>();
	}

	// Token: 0x0600191B RID: 6427 RVA: 0x00061E3F File Offset: 0x0006003F
	public override void Start()
	{
		base.Start();
		this.totemActiveTime = Time.time + 1f;
		this.antiGrav = Physics.gravity * (-1f + this.gravFactorWhileTotemed);
		this.initted = true;
	}

	// Token: 0x0600191C RID: 6428 RVA: 0x00061E7B File Offset: 0x0006007B
	public Rigidbody JointBody()
	{
		return this.body;
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x00061E84 File Offset: 0x00060084
	public void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger)
		{
			return;
		}
		TotemLinker componentInChildren = col.GetComponentInChildren<TotemLinker>();
		if (componentInChildren != null && componentInChildren != this && this.CanLink() && componentInChildren.CanBeLinked() && !componentInChildren.IndirectlyLinks(this, 20))
		{
			this.RelinkTo(componentInChildren);
		}
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x00061ED8 File Offset: 0x000600D8
	private bool IndirectlyLinks(TotemLinker checkLink, int checkSteps)
	{
		if (this.linkTo == null)
		{
			return false;
		}
		if (this.linkTo == checkLink)
		{
			return true;
		}
		if (checkSteps == 0)
		{
			Log.Warning("Failed to complete check for circular totem link.", Array.Empty<object>());
			return false;
		}
		return this.linkTo.IndirectlyLinks(checkLink, checkSteps - 1);
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x00061F28 File Offset: 0x00060128
	public void DisableToteming()
	{
		this.BreakLink();
		this.SetStackReceptive(false);
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x00061F37 File Offset: 0x00060137
	public void EnableToteming()
	{
		if (!this.stackReceptive)
		{
			this.rethinkReceptivenessTime = this.timeDir.WorldTime();
		}
	}

	// Token: 0x06001921 RID: 6433 RVA: 0x00061F54 File Offset: 0x00060154
	protected void RelinkTo(TotemLinker totem)
	{
		if (this.joint != null)
		{
			Destroyer.Destroy(this.joint, "TotemLinker.RelinkTo");
			this.joint = null;
		}
		this.linkTo = totem;
		if (totem != null)
		{
			totem.LinkFrom(this);
			totem.SetStackReceptive(true);
			SpringJoint springJoint = this.JointBody().gameObject.AddComponent<SpringJoint>();
			totem.JointBody().MovePosition(base.transform.position);
			springJoint.autoConfigureConnectedAnchor = false;
			SafeJointReference.AttachSafely(totem.JointBody().gameObject, springJoint, true);
			springJoint.connectedAnchor = new Vector3(0f, -0.4f, 0f);
			springJoint.anchor = new Vector3(0f, 0.4f, 0f);
			springJoint.spring = 200f;
			springJoint.breakForce = 100f;
			this.joint = springJoint;
		}
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x0006203C File Offset: 0x0006023C
	public void RegistryUpdate()
	{
		if (this.linkTo != null && (this.joint == null || !this.Linkable() || !this.stackReceptive))
		{
			this.BreakLink();
		}
		if (!this.totemActive && Time.time >= this.totemActiveTime)
		{
			this.totemActive = true;
		}
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x00062098 File Offset: 0x00060298
	public void UpdateEvenWhenInactive()
	{
		if (!this.initted)
		{
			return;
		}
		if (this.timeDir.HasReached(this.rethinkReceptivenessTime))
		{
			this.SetStackReceptive(Randoms.SHARED.GetProbability(this.receptivenessProb));
		}
		bool flag = this.CanLink();
		if (base.gameObject.activeSelf != flag)
		{
			base.gameObject.SetActive(flag);
		}
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x000620F8 File Offset: 0x000602F8
	public void RegistryFixedUpdate()
	{
		if (this.linkedFrom != null)
		{
			this.body.AddForce(this.antiGrav, ForceMode.Acceleration);
		}
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x0006211A File Offset: 0x0006031A
	public bool IsLinkedFrom()
	{
		return this.linkedFrom != null;
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x00062128 File Offset: 0x00060328
	public void SetStackReceptive(bool receptive)
	{
		if (this.stackReceptive != receptive)
		{
			this.stackReceptive = receptive;
		}
		this.rethinkReceptivenessTime = this.timeDir.HoursFromNowOrStart(Randoms.SHARED.GetInRange(this.rethinkReceptivenessMin, this.rethinkReceptivenessMax));
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x00062164 File Offset: 0x00060364
	private void BreakLink()
	{
		if (this.linkedFrom != null && this.linkedFrom != this.linkTo)
		{
			this.linkedFrom.RelinkTo(this.linkTo);
		}
		else if (this.linkTo != null)
		{
			this.linkTo.LinkFrom(null);
		}
		this.linkTo = null;
		this.linkedFrom = null;
		if (this.joint != null)
		{
			Destroyer.Destroy(this.joint, "TotemLinker.BreakLink");
			this.joint = null;
		}
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x000621F2 File Offset: 0x000603F2
	public bool CanLink()
	{
		return this.stackReceptive && this.linkTo == null && this.Linkable();
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x00062212 File Offset: 0x00060412
	public bool CanBeLinked()
	{
		return this.linkedFrom == null && this.Linkable();
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x0006222A File Offset: 0x0006042A
	public void LinkFrom(TotemLinker from)
	{
		this.linkedFrom = from;
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x00062234 File Offset: 0x00060434
	private bool Linkable()
	{
		return this.initted && (!(this.vacuumable == null) || !(this.emotions == null)) && (!(this.vacuumable != null) || !this.vacuumable.isCaptive()) && (!(this.emotions != null) || this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) <= 0.5f);
	}

	// Token: 0x040018DF RID: 6367
	[Tooltip("Probability that a totem linker will be receptive to linking another.")]
	public float receptivenessProb = 0.25f;

	// Token: 0x040018E0 RID: 6368
	[Tooltip("Minimum game hours between rethinking whether we're receptive.")]
	public float rethinkReceptivenessMin = 6f;

	// Token: 0x040018E1 RID: 6369
	[Tooltip("Maximum game hours between rethinking whether we're receptive.")]
	public float rethinkReceptivenessMax = 12f;

	// Token: 0x040018E2 RID: 6370
	[Tooltip("How much to allow gravity to do its thing on slimes while totemed.")]
	public float gravFactorWhileTotemed = 0.5f;

	// Token: 0x040018E3 RID: 6371
	private TotemLinker linkTo;

	// Token: 0x040018E4 RID: 6372
	private TotemLinker linkedFrom;

	// Token: 0x040018E5 RID: 6373
	private Joint joint;

	// Token: 0x040018E6 RID: 6374
	private SlimeEmotions emotions;

	// Token: 0x040018E7 RID: 6375
	private Vacuumable vacuumable;

	// Token: 0x040018E8 RID: 6376
	private Rigidbody body;

	// Token: 0x040018E9 RID: 6377
	private TimeDirector timeDir;

	// Token: 0x040018EA RID: 6378
	private Vector3 antiGrav;

	// Token: 0x040018EB RID: 6379
	private float totemActiveTime;

	// Token: 0x040018EC RID: 6380
	private bool totemActive;

	// Token: 0x040018ED RID: 6381
	private double rethinkReceptivenessTime;

	// Token: 0x040018EE RID: 6382
	private bool stackReceptive;

	// Token: 0x040018EF RID: 6383
	private bool initted;

	// Token: 0x040018F0 RID: 6384
	private const float STACK_DIST = 0.8f;

	// Token: 0x040018F1 RID: 6385
	private const float HALF_STACK_DIST = 0.4f;

	// Token: 0x040018F2 RID: 6386
	private const float AGITATION_BREAK = 0.5f;

	// Token: 0x040018F3 RID: 6387
	private const float DELAY = 1f;

	// Token: 0x040018F4 RID: 6388
	private const int CIRCULAR_LINK_STEPS = 20;
}
