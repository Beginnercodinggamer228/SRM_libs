using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Token: 0x020003FD RID: 1021
public class GroundVine : FindConsumable
{
	// Token: 0x06001552 RID: 5458 RVA: 0x00052CB8 File Offset: 0x00050EB8
	public override void Awake()
	{
		base.Awake();
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		this.slimeAppearanceApplicator.OnAppearanceChanged += this.UpdateVineAppearance;
		if (this.slimeAppearanceApplicator.Appearance != null)
		{
			this.UpdateVineAppearance(this.slimeAppearanceApplicator.Appearance);
		}
		this.nextVineTime = Time.time + this.cooldown;
		this.groundMask = 268439553;
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x00052D2F File Offset: 0x00050F2F
	public override void Start()
	{
		base.Start();
		this.playerObj = SRSingleton<SceneContext>.Instance.Player;
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x00052D48 File Offset: 0x00050F48
	public override float Relevancy(bool isGrounded)
	{
		if (Time.time < this.nextVineTime || base.IsCaptive())
		{
			return 0f;
		}
		if (!isGrounded)
		{
			return 0f;
		}
		float num;
		this.target = base.FindNearestConsumable(out num);
		if (this.target == null || this.target == this.playerObj || GroundVine.allGrabbed.Contains(this.target))
		{
			return 0f;
		}
		return num * num * 0.95f;
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x00052DCC File Offset: 0x00050FCC
	public override void Action()
	{
		if (this.phase == GroundVine.Phase.IDLE && this.activeVine == null && this.target != null)
		{
			if (!this.MaybeGrapple(this.target))
			{
				return;
			}
		}
		else if (this.target == null || this.activeVine == null)
		{
			this.Release();
			return;
		}
		if (Time.time >= this.phaseEndTime)
		{
			if (this.phase == GroundVine.Phase.GRAB_GROW)
			{
				float num = 1f;
				float num2 = 0.75f * num;
				SECTR_AudioSystem.Play(this.vineDownCue, this.activeVine.transform.position, false);
				TweenUtil.ScaleOut(this.activeVine, num2, Ease.InQuint);
				this.phase = GroundVine.Phase.GRAB_SHRINK;
				this.phaseEndTime = Time.time + num2;
				return;
			}
			if (this.phase == GroundVine.Phase.GRAB_SHRINK)
			{
				RaycastHit raycastHit;
				Physics.Raycast(base.transform.position + base.transform.forward * (PhysicsUtil.RadiusOfObject(base.gameObject) + 0.5f), Vector3.down, out raycastHit, 2f, this.groundMask);
				if (raycastHit.collider == null)
				{
					this.Release();
					return;
				}
				SRBehaviour.SpawnAndPlayFX(this.vineExitFX, this.activeVine.transform.position, Quaternion.identity);
				this.activeVine.transform.position = raycastHit.point;
				this.target.transform.position = raycastHit.point;
				SRBehaviour.SpawnAndPlayFX(this.vineEnterFX, this.activeVine.transform.position, Quaternion.identity);
				SECTR_AudioSystem.Play(this.vineUpCue, this.activeVine.transform.position, false);
				float num3 = Randoms.SHARED.GetInRange(2f, 2.5f) / 4f;
				float num4 = 0.75f * num3;
				this.activeVine.transform.localScale = new Vector3(1f, 1f, num3);
				TweenUtil.ScaleIn(this.activeVine, num4, Ease.InOutCubic);
				TweenUtil.ScaleIn(this.target, 0.25f, Ease.Linear);
				this.phase = GroundVine.Phase.EAT_GROW;
				this.phaseEndTime = Time.time + num4;
				this.EnableTargetCollider(true);
				return;
			}
			else
			{
				if (this.phase == GroundVine.Phase.EAT_GROW)
				{
					float num5 = 0.25f;
					TweenUtil.ScaleOut(this.activeVine, num5, Ease.InQuint);
					this.phase = GroundVine.Phase.EAT_SHRINK;
					this.phaseEndTime = Time.time + num5;
					SECTR_AudioSystem.Play(this.vineDownCue, this.activeVine.transform.position, false);
					Destroyer.Destroy(this.activeVine.GetComponentInChildren<Joint>(), "GroundVine.Action#1");
					return;
				}
				if (this.phase == GroundVine.Phase.EAT_SHRINK)
				{
					SRBehaviour.SpawnAndPlayFX(this.vineExitFX, this.activeVine.transform.position, Quaternion.identity);
					this.Release();
				}
			}
		}
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x000530AA File Offset: 0x000512AA
	public override void Selected()
	{
		if (this.target != null)
		{
			this.MaybeGrapple(this.target);
		}
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x000530C7 File Offset: 0x000512C7
	public override void Deselected()
	{
		base.Deselected();
		this.nextVineTime = Time.time + this.cooldown;
		this.Release();
	}

	// Token: 0x06001558 RID: 5464 RVA: 0x000530E7 File Offset: 0x000512E7
	public override bool CanRethink()
	{
		return this.phase == GroundVine.Phase.IDLE;
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x000530F2 File Offset: 0x000512F2
	private void UpdateVineAppearance(SlimeAppearance appearance)
	{
		this.vinePrefab = appearance.VineAppearance.vinePrefab;
		this.vineEnterFX = appearance.VineAppearance.vineEnterFx;
		this.vineExitFX = appearance.VineAppearance.vineExitFx;
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x00053128 File Offset: 0x00051328
	private bool MaybeGrapple(GameObject target)
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(target.transform.position, Vector3.down, out raycastHit, 0.5f, this.groundMask))
		{
			return false;
		}
		if (!GroundVine.allGrabbed.Add(target))
		{
			return false;
		}
		float num = (target.transform.position.y - raycastHit.point.y + Randoms.SHARED.GetInRange(3f, 4f)) / 4f;
		float num2 = 0.75f * num;
		this.activeVine = SRBehaviour.InstantiateDynamic(this.vinePrefab, raycastHit.point, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), false);
		this.activeVine.transform.localScale = new Vector3(1f, 1f, num);
		TweenUtil.ScaleIn(this.activeVine, num2, Ease.InOutCubic);
		SRBehaviour.SpawnAndPlayFX(this.vineEnterFX, this.activeVine.transform.position, Quaternion.identity);
		SECTR_AudioSystem.Play(this.vineUpCue, this.activeVine.transform.position, false);
		this.phase = GroundVine.Phase.GRAB_GROW;
		this.phaseEndTime = Time.time + num2;
		Joint componentInChildren = this.activeVine.GetComponentInChildren<Joint>();
		target.transform.position = componentInChildren.transform.position;
		SafeJointReference.AttachSafely(target, componentInChildren, true);
		componentInChildren.connectedAnchor = Vector3.zero;
		this.EnableTargetCollider(false);
		return true;
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x0005329C File Offset: 0x0005149C
	public void Release()
	{
		Destroyer.Destroy(this.activeVine, "GroundVine.Release");
		if (this.phase >= GroundVine.Phase.GRAB_GROW)
		{
			GroundVine.allGrabbed.Remove(this.target);
		}
		this.EnableTargetCollider(true);
		this.target = null;
		this.activeVine = null;
		this.phase = GroundVine.Phase.IDLE;
		this.phaseEndTime = float.PositiveInfinity;
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x000532FA File Offset: 0x000514FA
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.Release();
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x00053308 File Offset: 0x00051508
	public void EnableTargetCollider(bool toEnable)
	{
		if (this.target != null)
		{
			foreach (Collider collider in this.target.GetComponents<Collider>())
			{
				if (!collider.isTrigger)
				{
					collider.enabled = toEnable;
				}
			}
		}
	}

	// Token: 0x0400143D RID: 5181
	[Tooltip("Time between vines, in seconds.")]
	public float cooldown = 2f;

	// Token: 0x0400143E RID: 5182
	[Tooltip("The audio cue for the vine coming up out of the ground.")]
	public SECTR_AudioCue vineUpCue;

	// Token: 0x0400143F RID: 5183
	[Tooltip("The audio cue for the vine going down back into the ground.")]
	public SECTR_AudioCue vineDownCue;

	// Token: 0x04001440 RID: 5184
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x04001441 RID: 5185
	private GameObject vinePrefab;

	// Token: 0x04001442 RID: 5186
	private GameObject vineEnterFX;

	// Token: 0x04001443 RID: 5187
	private GameObject vineExitFX;

	// Token: 0x04001444 RID: 5188
	private GameObject target;

	// Token: 0x04001445 RID: 5189
	private GameObject activeVine;

	// Token: 0x04001446 RID: 5190
	private float nextVineTime;

	// Token: 0x04001447 RID: 5191
	private GameObject playerObj;

	// Token: 0x04001448 RID: 5192
	private int groundMask;

	// Token: 0x04001449 RID: 5193
	private GroundVine.Phase phase;

	// Token: 0x0400144A RID: 5194
	private float phaseEndTime = float.PositiveInfinity;

	// Token: 0x0400144B RID: 5195
	private static HashSet<GameObject> allGrabbed = new HashSet<GameObject>();

	// Token: 0x0400144C RID: 5196
	private const float TARGET_SCALE_TIME = 0.25f;

	// Token: 0x0400144D RID: 5197
	private const float FULL_VINE_SCALE_TIME = 0.75f;

	// Token: 0x0400144E RID: 5198
	private const float FULL_VINE_HEIGHT = 4f;

	// Token: 0x0400144F RID: 5199
	private const float MIN_EAT_HEIGHT = 2f;

	// Token: 0x04001450 RID: 5200
	private const float MAX_EAT_HEIGHT = 2.5f;

	// Token: 0x04001451 RID: 5201
	private const float RELEASE_SCALE_TIME = 0.5f;

	// Token: 0x04001452 RID: 5202
	private const float MAX_HEIGHT = 0.5f;

	// Token: 0x04001453 RID: 5203
	private const float MIN_EXTRA_HEIGHT = 3f;

	// Token: 0x04001454 RID: 5204
	private const float MAX_EXTRA_HEIGHT = 4f;

	// Token: 0x020003FE RID: 1022
	private enum Phase
	{
		// Token: 0x04001456 RID: 5206
		IDLE,
		// Token: 0x04001457 RID: 5207
		GRAB_GROW,
		// Token: 0x04001458 RID: 5208
		GRAB_SHRINK,
		// Token: 0x04001459 RID: 5209
		EAT_GROW,
		// Token: 0x0400145A RID: 5210
		EAT_SHRINK
	}
}
