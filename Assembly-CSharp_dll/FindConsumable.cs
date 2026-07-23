using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020003D5 RID: 981
public abstract class FindConsumable : SlimeSubbehaviour
{
	// Token: 0x0600145A RID: 5210 RVA: 0x0004EAD3 File Offset: 0x0004CCD3
	public override void Awake()
	{
		base.Awake();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.member = base.GetComponent<RegionMember>();
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.startTime = Time.time;
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x0004EB0E File Offset: 0x0004CD0E
	public override void Start()
	{
		base.Start();
		this.UpdateSearchIds();
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x0004EB1C File Offset: 0x0004CD1C
	public void UpdateSearchIds()
	{
		this.searchIds = this.GetSearchIds();
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x0004EB2A File Offset: 0x0004CD2A
	protected virtual Dictionary<Identifiable.Id, DriveCalculator> GetSearchIds()
	{
		return base.GetComponent<SlimeEat>().GetAllEats();
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x0004EB37 File Offset: 0x0004CD37
	protected void RotateTowards(Vector3 dirToTarget)
	{
		base.RotateTowards(dirToTarget, this.facingSpeed, this.facingStability);
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x0004EB4C File Offset: 0x0004CD4C
	protected GameObject FindNearestConsumable(IList<GameObjectActorModelIdentifiableIndex.Entry> gameObjects, out float drive)
	{
		GameObject result = null;
		float num = 1f / (this.maxSearchRad * this.maxSearchRad);
		drive = 0f;
		float num2 = this.minDist * this.minDist;
		Vector3 position = base.transform.position;
		for (int i = 0; i < gameObjects.Count; i++)
		{
			GameObjectActorModelIdentifiableIndex.Entry entry = gameObjects[i];
			GameObject gameObject = entry.GameObject;
			DriveCalculator driveCalculator;
			if (gameObject != base.gameObject && this.searchIds.TryGetValue(entry.Id, out driveCalculator) && Identifiable.IsEdible(gameObject))
			{
				float sqrMagnitude = (SlimeSubbehaviour.GetGotoPos(gameObject) - position).sqrMagnitude;
				if (sqrMagnitude >= num2)
				{
					float num3 = this.searchIds[entry.Id].Drive(this.emotions, entry.Id);
					float num4 = num3 / sqrMagnitude;
					if (num4 > num)
					{
						result = gameObject;
						num = num4;
						drive = Mathf.Clamp(num3, 0f, 1f);
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x0004EC58 File Offset: 0x0004CE58
	protected GameObject FindNearestConsumable(out float drive)
	{
		FindConsumable.localStatic_entries.Clear();
		CellDirector.Get(this.searchIds.Keys.AsEnumerable<Identifiable.Id>(), this.member, FindConsumable.localStatic_entries);
		return this.FindNearestConsumable(FindConsumable.localStatic_entries, out drive);
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x0004EC90 File Offset: 0x0004CE90
	protected GameObject FindNearestConsumableOld(out float drive)
	{
		GameObject result = null;
		float num = 1f / (this.maxSearchRad * this.maxSearchRad);
		drive = 0f;
		float num2 = this.minDist * this.minDist;
		foreach (KeyValuePair<Identifiable.Id, DriveCalculator> keyValuePair in this.searchIds)
		{
			this.nearbyGameObjects.Clear();
			CellDirector.Get(keyValuePair.Key, this.member, this.nearbyGameObjects);
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.nearbyGameObjects.Count; i++)
			{
				GameObject gameObject = this.nearbyGameObjects[i];
				if (gameObject != base.gameObject && Identifiable.IsEdible(gameObject))
				{
					float sqrMagnitude = (SlimeSubbehaviour.GetGotoPos(gameObject) - position).sqrMagnitude;
					if (sqrMagnitude >= num2)
					{
						float num3 = keyValuePair.Value.Drive(this.emotions, keyValuePair.Key);
						float num4 = num3 / sqrMagnitude;
						if (num4 > num)
						{
							result = gameObject;
							num = num4;
							drive = Mathf.Clamp(num3, 0f, 1f);
						}
					}
				}
			}
		}
		this.nearbyGameObjects.Clear();
		return result;
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x0004EDF0 File Offset: 0x0004CFF0
	protected void MoveTowards(Vector3 targetPos, bool shouldJump, ref float nextJumpAvail, float jumpStrength)
	{
		if (base.IsGrounded())
		{
			Vector3 vector = targetPos - base.transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			Vector3 normalized = vector.normalized;
			this.RotateTowards(normalized);
			if (shouldJump)
			{
				if (Time.fixedTime >= nextJumpAvail)
				{
					float d = Mathf.Min(1f, Mathf.Sqrt(sqrMagnitude) / 30f);
					this.slimeBody.AddForce((normalized * d + Vector3.up).normalized * jumpStrength * this.slimeBody.mass, ForceMode.Impulse);
					this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
					this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
					nextJumpAvail = Time.time + 1f;
					return;
				}
			}
			else
			{
				if (sqrMagnitude <= 9f)
				{
					this.slimeBody.AddForce(normalized * (480f * this.pursuitSpeedFactor * this.slimeBody.mass * Time.fixedDeltaTime));
					return;
				}
				float num = this.ScootCycleSpeed();
				this.slimeBody.AddForce(normalized * (150f * this.slimeBody.mass * this.pursuitSpeedFactor * Time.fixedDeltaTime * num));
				Vector3 position = base.transform.position + Vector3.down * (0.5f * base.transform.localScale.y);
				this.slimeBody.AddForceAtPosition(normalized * (270f * this.slimeBody.mass * Time.fixedDeltaTime * num), position);
			}
		}
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x0004EFAC File Offset: 0x0004D1AC
	protected float ScootCycleSpeed()
	{
		return Mathf.Sin((Time.time - this.startTime) * 6.2831855f) + 1f;
	}

	// Token: 0x04001326 RID: 4902
	public float minSearchRad = 5f;

	// Token: 0x04001327 RID: 4903
	public float maxSearchRad = 30f;

	// Token: 0x04001328 RID: 4904
	public float facingStability = 1f;

	// Token: 0x04001329 RID: 4905
	public float facingSpeed = 5f;

	// Token: 0x0400132A RID: 4906
	public float pursuitSpeedFactor = 1f;

	// Token: 0x0400132B RID: 4907
	public float minDist;

	// Token: 0x0400132C RID: 4908
	protected Dictionary<Identifiable.Id, DriveCalculator> searchIds;

	// Token: 0x0400132D RID: 4909
	protected SlimeAudio slimeAudio;

	// Token: 0x0400132E RID: 4910
	protected RegionMember member;

	// Token: 0x0400132F RID: 4911
	protected LookupDirector lookupDir;

	// Token: 0x04001330 RID: 4912
	protected float startTime;

	// Token: 0x04001331 RID: 4913
	private const float SCOOT_CYCLE_TIME = 1f;

	// Token: 0x04001332 RID: 4914
	private const float SCOOT_CYCLE_FACTOR = 6.2831855f;

	// Token: 0x04001333 RID: 4915
	private List<GameObject> nearbyGameObjects = new List<GameObject>();

	// Token: 0x04001334 RID: 4916
	private static List<GameObjectActorModelIdentifiableIndex.Entry> localStatic_entries = new List<GameObjectActorModelIdentifiableIndex.Entry>();
}
