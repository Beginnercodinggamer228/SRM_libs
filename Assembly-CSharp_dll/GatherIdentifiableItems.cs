using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003DC RID: 988
public class GatherIdentifiableItems : FindConsumable
{
	// Token: 0x0600148C RID: 5260 RVA: 0x0004FB4F File Offset: 0x0004DD4F
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x0004FB58 File Offset: 0x0004DD58
	public override float Relevancy(bool isGrounded)
	{
		this.Release();
		if (Time.time >= this.disallowSelectionUntil)
		{
			float num;
			this.target = base.FindNearestConsumable(out num);
		}
		if (!(this.target == null))
		{
			return Randoms.SHARED.GetInRange(0.3f, 0.5f);
		}
		return 0f;
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x0004FBAE File Offset: 0x0004DDAE
	public override void Selected()
	{
		this.giveUpOnGatherTime = Time.time + 10f;
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x0004FBC4 File Offset: 0x0004DDC4
	public override void Action()
	{
		if (this.target == null || (this.joint == null && this.isAttached) || Time.time > this.giveUpOnGatherTime || this.vacuumable.isHeld())
		{
			this.Release();
			return;
		}
		if (this.joint == null)
		{
			Vector3 gotoPos = SlimeSubbehaviour.GetGotoPos(this.target);
			bool flag = base.IsBlocked(this.target, 0, false);
			base.MoveTowards(gotoPos, flag, ref this.nextLeapAvail, this.maxJump);
			if ((gotoPos - base.transform.position).sqrMagnitude <= 1f * base.transform.localScale.z * base.transform.localScale.z)
			{
				if (!flag)
				{
					Vector3? vector = this.gatherToPos = this.GetGatherToPosition();
					if (vector != null && GatherIdentifiableItems.allGathering.Add(this.target))
					{
						this.joint = SlimeUtil.AttachToMouth(base.gameObject, this.target);
						this.giveUpOnGatherTime = Time.time + 10f;
						this.isAttached = true;
						this.slimeAudio.Play(this.slimeAudio.slimeSounds.gatherCue);
						return;
					}
				}
				this.Release();
				return;
			}
		}
		else
		{
			if ((this.gatherToPos.Value - base.transform.position).sqrMagnitude <= 9f)
			{
				this.Release();
				return;
			}
			Rigidbody component = base.GetComponent<Rigidbody>();
			base.MoveTowards(this.gatherToPos.Value, true, ref this.nextLeapAvail, this.maxJump * (this.target.GetComponent<Rigidbody>().mass + component.mass) / component.mass);
			base.RotateTowards(this.gatherToPos.Value - base.transform.position);
		}
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x0004FDB4 File Offset: 0x0004DFB4
	private Vector3? GetGatherToPosition()
	{
		Identifiable component = this.target.GetComponent<Identifiable>();
		GameObject gameObject = (component == null) ? null : this.FindItemNotOfType(component.id, this.maxSearchRad);
		if (!(gameObject == null))
		{
			return new Vector3?(SlimeSubbehaviour.GetGotoPos(gameObject));
		}
		return null;
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x0004FE0A File Offset: 0x0004E00A
	private void Release()
	{
		Destroyer.Destroy(this.joint, "GatherIdentifiableItems.Release");
		this.joint = null;
		if (this.isAttached)
		{
			GatherIdentifiableItems.allGathering.Remove(this.target);
		}
		this.target = null;
		this.isAttached = false;
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x0004FE4A File Offset: 0x0004E04A
	public override bool CanRethink()
	{
		return !this.isAttached;
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x0004FE55 File Offset: 0x0004E055
	public override void Deselected()
	{
		base.Deselected();
		this.Release();
		this.disallowSelectionUntil = Time.time + this.pauseBetweenGathers;
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x0004FE75 File Offset: 0x0004E075
	public override void OnDisable()
	{
		base.OnDisable();
		this.Release();
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x0004FE84 File Offset: 0x0004E084
	protected override Dictionary<Identifiable.Id, DriveCalculator> GetSearchIds()
	{
		Dictionary<Identifiable.Id, DriveCalculator> dictionary = new Dictionary<Identifiable.Id, DriveCalculator>(Identifiable.idComparer);
		GatherIdentifiableItems.ItemClass[] array = this.itemClasses;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Identifiable.Id key in GatherIdentifiableItems.GetSearchIds(array[i]))
			{
				dictionary[key] = new DriveCalculator(SlimeEmotions.Emotion.NONE, 0f, 0f);
			}
		}
		return dictionary;
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x0004FF08 File Offset: 0x0004E108
	private static ICollection<Identifiable.Id> GetSearchIds(GatherIdentifiableItems.ItemClass itemClass)
	{
		if (itemClass == GatherIdentifiableItems.ItemClass.FRUIT)
		{
			return Identifiable.FRUIT_CLASS;
		}
		if (itemClass != GatherIdentifiableItems.ItemClass.VEGGIE)
		{
			return new HashSet<Identifiable.Id>();
		}
		return Identifiable.VEGGIE_CLASS;
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x0004FF24 File Offset: 0x0004E124
	protected GameObject FindItemNotOfType(Identifiable.Id ineligibleId, float maxDist)
	{
		float num = maxDist * maxDist;
		List<GameObject> list = new List<GameObject>();
		foreach (KeyValuePair<Identifiable.Id, DriveCalculator> keyValuePair in this.searchIds)
		{
			if (keyValuePair.Key != ineligibleId)
			{
				this.nearbyGameObjectsLst.Clear();
				CellDirector.Get(keyValuePair.Key, this.member, this.nearbyGameObjectsLst);
				Vector3 position = base.transform.position;
				float num2 = this.minGatherDist * this.minGatherDist;
				for (int i = 0; i < this.nearbyGameObjectsLst.Count; i++)
				{
					GameObject gameObject = this.nearbyGameObjectsLst[i];
					if (Identifiable.IsEdible(gameObject))
					{
						float sqrMagnitude = (SlimeSubbehaviour.GetGotoPos(gameObject) - position).sqrMagnitude;
						if (sqrMagnitude <= num && sqrMagnitude >= num2)
						{
							list.Add(gameObject);
						}
					}
				}
			}
		}
		this.nearbyGameObjectsLst.Clear();
		return Randoms.SHARED.Pick<GameObject>(list, null);
	}

	// Token: 0x0400135A RID: 4954
	[Tooltip("The item types we will gather.")]
	public GatherIdentifiableItems.ItemClass[] itemClasses;

	// Token: 0x0400135B RID: 4955
	public float maxJump = 6f;

	// Token: 0x0400135C RID: 4956
	public float pauseBetweenGathers = 10f;

	// Token: 0x0400135D RID: 4957
	public float minGatherDist = 5f;

	// Token: 0x0400135E RID: 4958
	private GameObject target;

	// Token: 0x0400135F RID: 4959
	private Vector3? gatherToPos;

	// Token: 0x04001360 RID: 4960
	private FixedJoint joint;

	// Token: 0x04001361 RID: 4961
	private float nextLeapAvail;

	// Token: 0x04001362 RID: 4962
	private float giveUpOnGatherTime;

	// Token: 0x04001363 RID: 4963
	private float disallowSelectionUntil;

	// Token: 0x04001364 RID: 4964
	private bool isAttached;

	// Token: 0x04001365 RID: 4965
	private static HashSet<GameObject> allGathering = new HashSet<GameObject>();

	// Token: 0x04001366 RID: 4966
	private const float GIVE_UP_GATHER_TIME = 10f;

	// Token: 0x04001367 RID: 4967
	private const float CLOSE_ENOUGH = 1f;

	// Token: 0x04001368 RID: 4968
	private const float CLOSE_ENOUGH_SQR = 1f;

	// Token: 0x04001369 RID: 4969
	private const float GATHER_RAD = 3f;

	// Token: 0x0400136A RID: 4970
	private const float GATHER_RAD_SQR = 9f;

	// Token: 0x0400136B RID: 4971
	private List<GameObject> nearbyGameObjectsLst = new List<GameObject>();

	// Token: 0x020003DD RID: 989
	public enum ItemClass
	{
		// Token: 0x0400136D RID: 4973
		FRUIT,
		// Token: 0x0400136E RID: 4974
		VEGGIE
	}
}
