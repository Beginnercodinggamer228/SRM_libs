using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200032A RID: 810
public class PlortCollector : SRBehaviour, LandPlotModel.Participant
{
	// Token: 0x06001117 RID: 4375 RVA: 0x00044674 File Offset: 0x00042874
	public void Awake()
	{
		this.region = base.GetComponentInParent<Region>();
		this.storage = base.GetComponentInParent<SiloStorage>();
		this.vacAudio = base.GetComponent<SECTR_AudioSource>();
		this.collectAnim = base.GetComponentInChildren<Animator>();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.animCycloneActiveId = Animator.StringToHash("CycloneActive");
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x000446D1 File Offset: 0x000428D1
	public void InitModel(LandPlotModel model)
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		model.collectorNextTime = this.timeDir.HoursFromNowOrStart(this.collectPeriod);
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x000446FA File Offset: 0x000428FA
	public void SetModel(LandPlotModel model)
	{
		this.model = model;
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x00044704 File Offset: 0x00042904
	public void Update()
	{
		if (this.region.Hibernated)
		{
			return;
		}
		if (this.joints.Count > 0 && this.timeDir.HasReached(this.endCollectAt))
		{
			foreach (PlortCollector.JointReference jointReference in this.joints)
			{
				jointReference.Destroy();
			}
			this.joints.Clear();
		}
		else if (this.timeDir.HasReached(this.model.collectorNextTime))
		{
			this.DoCollection();
		}
		if (this.joints.Count > 0)
		{
			List<PlortCollector.JointReference> list = new List<PlortCollector.JointReference>();
			foreach (PlortCollector.JointReference jointReference2 in this.joints)
			{
				if (jointReference2.joint == null || jointReference2.joint.connectedBody == null)
				{
					list.Add(jointReference2);
				}
				else if (!this.storage.CanAccept(jointReference2.id))
				{
					list.Add(jointReference2);
				}
				else if ((jointReference2.joint.connectedBody.transform.position - this.collectPt.position).sqrMagnitude <= 1f)
				{
					if (this.storage.MaybeAddIdentifiable(jointReference2.id))
					{
						if (this.collectFX != null)
						{
							SRBehaviour.SpawnAndPlayFX(this.collectFX, jointReference2.joint.connectedBody.transform.position, jointReference2.joint.connectedBody.transform.rotation);
						}
						Destroyer.DestroyActor(jointReference2.joint.connectedBody.gameObject, "PlortCollector.Update", false);
					}
					list.Add(jointReference2);
				}
				else
				{
					jointReference2.joint.maxDistance = Mathf.Max(0f, jointReference2.joint.maxDistance - Time.deltaTime * 5f);
				}
			}
			foreach (PlortCollector.JointReference jointReference3 in list)
			{
				this.joints.Remove(jointReference3);
				jointReference3.Destroy();
			}
		}
		bool flag = this.joints.Count > 0 || !this.timeDir.HasReached(this.forceCollectUntil);
		if (this.collectAnim != null)
		{
			this.collectAnim.SetBool(this.animCycloneActiveId, flag);
		}
		if (flag && !this.vacAudio.IsPlaying)
		{
			this.vacAudio.Play();
			return;
		}
		if (!flag && this.vacAudio.IsPlaying)
		{
			this.vacAudio.Stop(false);
		}
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x00044A1C File Offset: 0x00042C1C
	public void StartCollection()
	{
		if (this.joints.Count == 0 && this.timeDir.HasReached(this.forceCollectUntil))
		{
			this.DoCollection();
		}
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x00044A44 File Offset: 0x00042C44
	private void DoCollection()
	{
		this.model.collectorNextTime += (double)(3600f * this.collectPeriod);
		foreach (GameObject gameObject in this.collectionArea.CurrColliders())
		{
			Identifiable component = gameObject.GetComponent<Identifiable>();
			if (component != null && this.storage.CanAccept(component.id))
			{
				Vacuumable component2 = gameObject.GetComponent<Vacuumable>();
				if (component2 != null && !component2.isCaptive())
				{
					GameObject gameObject2 = new GameObject("CollectJoint");
					gameObject2.AddComponent<Rigidbody>().isKinematic = true;
					gameObject2.transform.SetParent(this.collectPt, false);
					gameObject2.transform.localPosition = Vector3.zero;
					SpringJoint springJoint = gameObject2.AddComponent<SpringJoint>();
					springJoint.spring = 1000f;
					springJoint.maxDistance = (gameObject.transform.position - this.collectPt.position).magnitude;
					springJoint.autoConfigureConnectedAnchor = false;
					springJoint.connectedAnchor = Vector3.zero;
					SafeJointReference.AttachSafely(gameObject, springJoint, true);
					springJoint.connectedBody.WakeUp();
					component2.capture(springJoint);
					this.joints.Add(new PlortCollector.JointReference
					{
						vacuumable = component2,
						joint = springJoint,
						id = component.id
					});
				}
			}
		}
		this.forceCollectUntil = this.timeDir.HoursFromNow(0.083333336f);
		this.endCollectAt = this.timeDir.HoursFromNow(0.16666667f);
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x00044C0C File Offset: 0x00042E0C
	public void FastForward(List<Identifiable.Id> produceIds, List<Identifiable.Id> alreadyCollectedIds)
	{
		for (int i = produceIds.Count - 1; i >= 0; i--)
		{
			if (this.storage.MaybeAddIdentifiable(produceIds[i]))
			{
				alreadyCollectedIds.Add(produceIds[i]);
			}
		}
	}

	// Token: 0x04001008 RID: 4104
	[Tooltip("The area within which we collect plorts.")]
	public TrackCollisions collectionArea;

	// Token: 0x04001009 RID: 4105
	[Tooltip("Time between collections in hours.")]
	public float collectPeriod = 1f;

	// Token: 0x0400100A RID: 4106
	[Tooltip("Animator to animate while collecting any plorts.")]
	public Animator collectAnim;

	// Token: 0x0400100B RID: 4107
	[Tooltip("Effect to play on collecting an individual plort.")]
	public GameObject collectFX;

	// Token: 0x0400100C RID: 4108
	[Tooltip("Where to pull the plorts to")]
	public Transform collectPt;

	// Token: 0x0400100D RID: 4109
	private SiloStorage storage;

	// Token: 0x0400100E RID: 4110
	private SECTR_AudioSource vacAudio;

	// Token: 0x0400100F RID: 4111
	private TimeDirector timeDir;

	// Token: 0x04001010 RID: 4112
	private Region region;

	// Token: 0x04001011 RID: 4113
	private List<PlortCollector.JointReference> joints = new List<PlortCollector.JointReference>();

	// Token: 0x04001012 RID: 4114
	private double endCollectAt;

	// Token: 0x04001013 RID: 4115
	private double forceCollectUntil;

	// Token: 0x04001014 RID: 4116
	private const float COLLECT_DIST = 1f;

	// Token: 0x04001015 RID: 4117
	private const float COLLECT_DIST_SQR = 1f;

	// Token: 0x04001016 RID: 4118
	private const float COLLECT_SPEED = 5f;

	// Token: 0x04001017 RID: 4119
	private const float MIN_COLLECT_TIME = 0.083333336f;

	// Token: 0x04001018 RID: 4120
	private const float MAX_COLLECT_TIME = 0.16666667f;

	// Token: 0x04001019 RID: 4121
	private int animCycloneActiveId;

	// Token: 0x0400101A RID: 4122
	private LandPlotModel model;

	// Token: 0x0200032B RID: 811
	private class JointReference
	{
		// Token: 0x0600111F RID: 4383 RVA: 0x00044C6B File Offset: 0x00042E6B
		public void Destroy()
		{
			Destroyer.Destroy(this.joint, "PlortCollector.JointReference.Destroy");
			if (this.vacuumable != null)
			{
				this.vacuumable.release();
				this.vacuumable = null;
			}
		}

		// Token: 0x0400101B RID: 4123
		public Identifiable.Id id;

		// Token: 0x0400101C RID: 4124
		public Vacuumable vacuumable;

		// Token: 0x0400101D RID: 4125
		public SpringJoint joint;
	}
}
