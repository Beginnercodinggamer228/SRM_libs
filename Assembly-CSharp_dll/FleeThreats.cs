using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020003D9 RID: 985
public class FleeThreats : SlimeSubbehaviour, RegistryFixedUpdateable
{
	// Token: 0x06001477 RID: 5239 RVA: 0x0004F29B File Offset: 0x0004D49B
	public override void Awake()
	{
		base.Awake();
		this.member = base.GetComponent<RegionMember>();
		this.totemLinker = base.GetComponentInChildren<TotemLinker>();
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x0004BA2F File Offset: 0x00049C2F
	public override void Start()
	{
		base.Start();
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x0004F2BB File Offset: 0x0004D4BB
	public override float Relevancy(bool isGrounded)
	{
		this.threat = this.FindNearestThreat();
		if (this.threat != null)
		{
			return this.emotions.GetCurr(this.driver);
		}
		return 0f;
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x0004F2E8 File Offset: 0x0004D4E8
	public void RegistryFixedUpdate()
	{
		if (this.threat != null && this.threat.gameObject != null && this.threat.gameObject.activeSelf)
		{
			float magnitude = (SlimeSubbehaviour.GetGotoPos(this.threat.gameObject) - base.transform.position).magnitude;
			this.emotions.Adjust(this.driver, this.fearProfile.DistToFearAdjust(this.threat.id, magnitude));
			return;
		}
		if (this.threat != null && this.threat.gameObject == null)
		{
			this.threat = null;
		}
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x0004F398 File Offset: 0x0004D598
	public override void Selected()
	{
		SlimeFaceAnimator component = base.GetComponent<SlimeFaceAnimator>();
		if (component != null)
		{
			component.SetTrigger("triggerAlarm");
		}
		if (this.totemLinker != null)
		{
			this.totemLinker.DisableToteming();
		}
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x0004F3DC File Offset: 0x0004D5DC
	public override void Action()
	{
		if (this.threat != null && this.threat.gameObject != null && base.IsGrounded() && Time.fixedTime >= this.nextLeap)
		{
			Vector3 vector = -(SlimeSubbehaviour.GetGotoPos(this.threat.gameObject) - base.transform.position).normalized;
			this.RotateTowards(vector);
			if (this.grapplers.Count <= 0)
			{
				float curr = this.emotions.GetCurr(this.driver);
				if (base.IsBlocked(null, 0, false))
				{
					this.slimeBody.AddForce((vector + Vector3.up * 5f).normalized * (3f * curr * this.maxJump * this.slimeBody.mass), ForceMode.Impulse);
				}
				else
				{
					this.slimeBody.AddForce((vector + Vector3.up).normalized * (curr * this.maxJump * this.slimeBody.mass), ForceMode.Impulse);
				}
				this.nextLeap = Time.fixedTime + 0.5f;
			}
		}
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x0004F51C File Offset: 0x0004D71C
	private void RotateTowards(Vector3 dirToTarget)
	{
		Vector3 angularVelocity = this.slimeBody.angularVelocity;
		Vector3 a = Vector3.Cross(Quaternion.AngleAxis(angularVelocity.magnitude * 57.29578f * this.facingStability / this.facingSpeed, angularVelocity) * base.transform.forward, dirToTarget);
		this.slimeBody.AddTorque(a * (this.facingSpeed * this.facingSpeed));
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x0004F58C File Offset: 0x0004D78C
	private FleeThreats.Threat FindNearestThreat()
	{
		FleeThreats.Threat threat = null;
		Vector3 position = base.transform.position;
		float curr = this.emotions.GetCurr(this.driver);
		foreach (Identifiable.Id id in this.fearProfile.GetThreateningIdentifiables())
		{
			float searchRadius = this.fearProfile.GetSearchRadius(id, curr);
			float num = searchRadius * searchRadius;
			this.nearbyGameObjects.Clear();
			CellDirector.Get(id, this.member, this.nearbyGameObjects);
			for (int i = 0; i < this.nearbyGameObjects.Count; i++)
			{
				GameObject gameObject = this.nearbyGameObjects[i];
				if (gameObject.activeInHierarchy && (id != Identifiable.Id.FIRE_COLUMN || this.FireColumnIsActiveThreat(gameObject)))
				{
					float sqrMagnitude = (SlimeSubbehaviour.GetGotoPos(gameObject) - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						if (threat == null)
						{
							threat = new FleeThreats.Threat();
						}
						threat.id = id;
						threat.gameObject = gameObject;
						num = sqrMagnitude;
					}
				}
			}
		}
		this.nearbyGameObjects.Clear();
		return threat;
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x0004F6BC File Offset: 0x0004D8BC
	private bool FireColumnIsActiveThreat(GameObject potentialThreatObject)
	{
		FireColumn componentInParent = potentialThreatObject.GetComponentInParent<FireColumn>();
		return componentInParent != null && componentInParent.IsFireActive();
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0004F6E1 File Offset: 0x0004D8E1
	public void AddGrappler(TentacleHook hook)
	{
		this.grapplers.Add(hook);
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x0004F6F0 File Offset: 0x0004D8F0
	public void RemoveGrappler(TentacleHook hook)
	{
		this.grapplers.Remove(hook);
	}

	// Token: 0x0400133D RID: 4925
	public FearProfile fearProfile;

	// Token: 0x0400133E RID: 4926
	public SlimeEmotions.Emotion driver = SlimeEmotions.Emotion.FEAR;

	// Token: 0x0400133F RID: 4927
	public float maxJump = 2f;

	// Token: 0x04001340 RID: 4928
	public float facingStability = 0.2f;

	// Token: 0x04001341 RID: 4929
	public float facingSpeed = 1f;

	// Token: 0x04001342 RID: 4930
	private FleeThreats.Threat threat;

	// Token: 0x04001343 RID: 4931
	private RegionMember member;

	// Token: 0x04001344 RID: 4932
	private TotemLinker totemLinker;

	// Token: 0x04001345 RID: 4933
	private HashSet<TentacleHook> grapplers = new HashSet<TentacleHook>();

	// Token: 0x04001346 RID: 4934
	private float nextLeap;

	// Token: 0x04001347 RID: 4935
	private const float LEAP_COOLDOWN = 0.5f;

	// Token: 0x04001348 RID: 4936
	private List<GameObject> nearbyGameObjects = new List<GameObject>();

	// Token: 0x020003DA RID: 986
	private class Threat
	{
		// Token: 0x04001349 RID: 4937
		public Identifiable.Id id;

		// Token: 0x0400134A RID: 4938
		public GameObject gameObject;
	}
}
