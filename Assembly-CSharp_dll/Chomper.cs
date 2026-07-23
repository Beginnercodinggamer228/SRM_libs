using System;
using System.Linq;
using UnityEngine;

// Token: 0x020003A7 RID: 935
public class Chomper : SRBehaviour
{
	// Token: 0x06001386 RID: 4998 RVA: 0x0004BF8A File Offset: 0x0004A18A
	public void Awake()
	{
		this.faceAnim = base.GetComponent<SlimeFaceAnimator>();
		this.bodyAnim = base.GetComponentInChildren<Animator>();
		this.animBiteId = Animator.StringToHash("Bite");
		this.animQuickBiteId = Animator.StringToHash("QuickBite");
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x0004BFC4 File Offset: 0x0004A1C4
	public void BiteComplete()
	{
		if (this.metadata != null)
		{
			Chomper.Metadata metadata = this.metadata;
			if (metadata != null)
			{
				metadata.onComplete(this.metadata.gameObject, this.metadata.id, this.metadata.isHeld, this.metadata.isLaunched);
			}
			this.DestroyJoints();
			this.metadata = null;
		}
		this.ResetEatClock();
		this.bodyAnim.SetBool(this.animBiteId, false);
		this.bodyAnim.SetBool(this.animQuickBiteId, false);
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x0004C052 File Offset: 0x0004A252
	public bool IsChomping()
	{
		return this.metadata != null;
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x0004C05D File Offset: 0x0004A25D
	private void OnJointBreak(float breakForce)
	{
		if (this.metadata != null && this.metadata.joint != null && this.metadata.joint.connectedBody == null)
		{
			this.ForceCancelChomp();
		}
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x0004C098 File Offset: 0x0004A298
	public bool CanChomp()
	{
		return Time.fixedTime >= this.nextChompTime && !this.IsChomping();
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x0004C0B4 File Offset: 0x0004A2B4
	public bool CancelChomp(GameObject obj)
	{
		if (this.metadata == null)
		{
			return false;
		}
		if (this.metadata.gameObject != obj)
		{
			return false;
		}
		if (this.metadata.isQuickChomp)
		{
			return false;
		}
		this.ForceCancelChomp();
		return true;
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x0004C0EB File Offset: 0x0004A2EB
	private void ForceCancelChomp()
	{
		this.DestroyJoints();
		this.metadata = null;
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x0004C0FC File Offset: 0x0004A2FC
	private void DestroyJoints()
	{
		if (this.metadata != null && this.metadata.gameObject != null)
		{
			foreach (SafeJointReference safeJointReference in base.GetComponents<SafeJointReference>().Concat(this.metadata.gameObject.GetComponents<SafeJointReference>()))
			{
				safeJointReference.DestroyJoint();
			}
		}
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x0004C178 File Offset: 0x0004A378
	public void StartChomp(GameObject other, Identifiable.Id otherId, bool whileHeld, bool quick, Chomper.OnChompStartDelegate onChompStart, Chomper.OnChompCompleteDelegate onChompComplete)
	{
		if (onChompStart != null)
		{
			onChompStart();
		}
		this.metadata = new Chomper.Metadata
		{
			onComplete = onChompComplete,
			gameObject = other,
			isQuickChomp = quick,
			id = otherId,
			isHeld = whileHeld,
			isLaunched = (LayerMask.NameToLayer("Launched") == other.layer)
		};
		this.faceAnim.SetTrigger((otherId == Identifiable.Id.PLAYER) ? "triggerAttackTelegraph" : (quick ? "triggerChompOpenQuick" : "triggerChompOpen"));
		this.bodyAnim.SetBool(quick ? this.animQuickBiteId : this.animBiteId, true);
		if (quick)
		{
			foreach (SafeJointReference safeJointReference in base.GetComponents<SafeJointReference>().Concat(other.GetComponents<SafeJointReference>()))
			{
				safeJointReference.DestroyJoint();
			}
			this.metadata.joint = SlimeUtil.AttachToMouth(base.gameObject, other);
		}
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x0004C280 File Offset: 0x0004A480
	public void ResetEatClock()
	{
		this.nextChompTime = Time.fixedTime + this.timePerAttack;
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x0004C294 File Offset: 0x0004A494
	public void OnDisable()
	{
		this.ForceCancelChomp();
	}

	// Token: 0x0400124A RID: 4682
	[Tooltip("Time per attack.")]
	public float timePerAttack = 3f;

	// Token: 0x0400124B RID: 4683
	private float nextChompTime;

	// Token: 0x0400124C RID: 4684
	private SlimeFaceAnimator faceAnim;

	// Token: 0x0400124D RID: 4685
	private Animator bodyAnim;

	// Token: 0x0400124E RID: 4686
	private int animQuickBiteId;

	// Token: 0x0400124F RID: 4687
	private int animBiteId;

	// Token: 0x04001250 RID: 4688
	private Chomper.Metadata metadata;

	// Token: 0x020003A8 RID: 936
	// (Invoke) Token: 0x06001393 RID: 5011
	public delegate void OnChompStartDelegate();

	// Token: 0x020003A9 RID: 937
	// (Invoke) Token: 0x06001397 RID: 5015
	public delegate void OnChompCompleteDelegate(GameObject chomped, Identifiable.Id chompedId, bool whileHeld, bool wasLaunched);

	// Token: 0x020003AA RID: 938
	private class Metadata
	{
		// Token: 0x04001251 RID: 4689
		public Chomper.OnChompCompleteDelegate onComplete;

		// Token: 0x04001252 RID: 4690
		public GameObject gameObject;

		// Token: 0x04001253 RID: 4691
		public Identifiable.Id id;

		// Token: 0x04001254 RID: 4692
		public bool isHeld;

		// Token: 0x04001255 RID: 4693
		public bool isLaunched;

		// Token: 0x04001256 RID: 4694
		public bool isQuickChomp;

		// Token: 0x04001257 RID: 4695
		public FixedJoint joint;
	}
}
