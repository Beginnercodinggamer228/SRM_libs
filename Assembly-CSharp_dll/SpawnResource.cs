using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000499 RID: 1177
public class SpawnResource : SRBehaviour, LiquidConsumer, DestroyAfterTimeListener, DestroyAfterTimeCondition, SpawnResourceModel.Participant
{
	// Token: 0x06001889 RID: 6281 RVA: 0x0005F499 File Offset: 0x0005D699
	public double GetNextSpawnTime()
	{
		return this.model.nextSpawnTime;
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x0005F4A8 File Offset: 0x0005D6A8
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.ambianceDir = SRSingleton<SceneContext>.Instance.AmbianceDirector;
		this.rand = Randoms.SHARED;
		this.totalSpawnsRemaining = this.MaxTotalSpawns;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterResourceSpawner(base.transform.position, this);
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x0005F507 File Offset: 0x0005D707
	public void Start()
	{
		this.landPlot = base.GetComponentInParent<LandPlot>();
		this.region = base.GetComponentInParent<Region>();
		if (this.landPlot != null)
		{
			this.allowSpawningInFastForwarding = true;
			return;
		}
		this.allowSpawningInFastForwarding = false;
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x0005F53E File Offset: 0x0005D73E
	public void InitModel(SpawnResourceModel model)
	{
		model.pos = base.transform.position;
		model.nextSpawnRipens = false;
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x0005F558 File Offset: 0x0005D758
	public void SetModel(SpawnResourceModel model)
	{
		this.model = model;
		if (model.nextSpawnTime == 0.0)
		{
			if (base.GetComponent<SpawnResourceForceFirstRipeness>() != null)
			{
				model.nextSpawnRipens = true;
				return;
			}
			if (this.timeDir.IsAtStart())
			{
				float inRange = this.rand.GetInRange(3f, 9f);
				double nextSpawnTime = this.timeDir.HoursFromNowOrStart(-inRange);
				model.nextSpawnTime = nextSpawnTime;
				return;
			}
			model.nextSpawnTime = this.timeDir.WorldTime();
		}
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x0005F5E0 File Offset: 0x0005D7E0
	public void InitAsReplacement(SpawnResource old)
	{
		this.totalSpawnsRemaining = old.totalSpawnsRemaining;
		if (old.model != null)
		{
			old.model.SetParticipant(this);
			this.SetModel(old.model);
		}
		foreach (Joint joint in old.SpawnJoints)
		{
			if (joint.connectedBody != null)
			{
				Joint joint2 = this.NearestJoint(joint.transform.position, 0.1f);
				if (joint2 != null)
				{
					joint.connectedBody.position = joint2.transform.position;
					joint.connectedBody.GetComponent<ResourceCycle>().Reattach(joint2);
				}
			}
		}
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x0005F688 File Offset: 0x0005D888
	public void Update()
	{
		if (this.region.Hibernated)
		{
			return;
		}
		this.UpdateToTime(this.timeDir.WorldTime(), this.timeDir.DeltaWorldTime());
		if (this.spawnQueue.Count > 0)
		{
			this.Spawn(this.spawnQueue.Dequeue());
		}
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x0005F6E0 File Offset: 0x0005D8E0
	public void UpdateToTime(double worldTime, double deltaWorldTime)
	{
		this.model.storedWater += (float)((double)(this.ambianceDir.PrecipitationRate() - 0.13043478f) * deltaWorldTime * 0.00027777778450399637);
		this.model.storedWater = Mathf.Clamp(this.model.storedWater, 0f, 3f);
		if (this.onWateredFX != null)
		{
			this.onWateredFX.SetActive(this.IsWatered());
		}
		if (this.spawnBlockers > 0)
		{
			return;
		}
		this.model.nextSpawnTime -= (double)this.AdditionalRipenessPerSecond() * deltaWorldTime;
		if (TimeUtil.HasReached(worldTime, this.model.nextSpawnTime))
		{
			float num = (float)((worldTime - this.model.nextSpawnTime) * 0.00027777778450399637);
			if (this.allowSpawningInFastForwarding && num >= 0.25f)
			{
				ResourceCycle resourceToSpawn = this.GetResourceToSpawn();
				float num2 = resourceToSpawn.unripeGameHours + resourceToSpawn.ripeGameHours;
				float num3 = resourceToSpawn.edibleGameHours + resourceToSpawn.rottenGameHours;
				if (this.landPlot != null && this.landPlot.HasUpgrade(LandPlot.Upgrade.MIRACLE_MIX))
				{
					num3 *= 2f;
				}
				while (num >= 0f)
				{
					if (num < num2 + num3)
					{
						this.spawnQueue.Enqueue(new SpawnResource.SpawnRequest
						{
							spawnAtTime = new double?(this.model.nextSpawnTime)
						});
					}
					float num4 = this.rand.GetInRange(this.MinSpawnIntervalGameHours, this.MaxSpawnIntervalGameHours) * (this.IsWatered() ? 0.5f : 1f);
					if (num4 < 1f)
					{
						num4 = 1f;
					}
					this.StepNextSpawnTime(0f, num4);
					num -= num4;
				}
			}
			else if (this.model.nextSpawnRipens)
			{
				this.spawnQueue.Enqueue(new SpawnResource.SpawnRequest
				{
					spawnRipe = true
				});
				this.UpdateNextSpawnTime(0f);
				this.model.nextSpawnRipens = false;
			}
			else
			{
				double num5 = this.model.nextSpawnTime;
				float num6 = this.rand.GetInRange(this.MinSpawnIntervalGameHours, this.MaxSpawnIntervalGameHours) * (this.IsWatered() ? 0.5f : 1f) * 3600f;
				while (num5 + (double)num6 < worldTime)
				{
					num5 += (double)num6;
				}
				this.spawnQueue.Enqueue(new SpawnResource.SpawnRequest
				{
					spawnAtTime = new double?(num5)
				});
				this.UpdateNextSpawnTime(0f);
			}
			if (this.onReachedSpawnTime != null)
			{
				this.onReachedSpawnTime();
			}
		}
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x0005F974 File Offset: 0x0005DB74
	private ResourceCycle GetResourceToSpawn()
	{
		return this.ObjectsToSpawn[0].GetComponent<ResourceCycle>();
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x0005F983 File Offset: 0x0005DB83
	public void OnDestroy()
	{
		this.DropFromJoints();
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterResourceSpawner(base.transform.position);
		}
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x0005F9B2 File Offset: 0x0005DBB2
	public void WillDestroyAfterTime()
	{
		this.DropFromJoints();
		SRSingleton<SceneContext>.Instance.GameModel.UnregisterResourceSpawner(base.transform.position);
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x0005F9D4 File Offset: 0x0005DBD4
	private void DropFromJoints()
	{
		foreach (Joint joint in this.SpawnJoints)
		{
			if (joint.connectedBody != null)
			{
				ResourceCycle component = joint.connectedBody.GetComponent<ResourceCycle>();
				joint.connectedBody.WakeUp();
				if (component != null)
				{
					component.Detach(new ResourceCycle.AdditionalRipeness(this.AdditionalRipenessPerSecond));
				}
				joint.connectedBody = null;
			}
		}
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x0005FA44 File Offset: 0x0005DC44
	public Identifiable.Id GetPrimarySpawnId()
	{
		if (this.BonusObjectsToSpawn != null && this.BonusObjectsToSpawn.Length != 0)
		{
			return this.BonusObjectsToSpawn[0].GetComponent<Identifiable>().id;
		}
		if (this.ObjectsToSpawn == null || this.ObjectsToSpawn.Length < 1)
		{
			return Identifiable.Id.NONE;
		}
		return this.ObjectsToSpawn[0].GetComponent<Identifiable>().id;
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x0005FA9C File Offset: 0x0005DC9C
	public void AddLiquid(Identifiable.Id liquidId, float amount)
	{
		if (Identifiable.IsWater(liquidId))
		{
			this.model.storedWater = Mathf.Min(3f, this.model.storedWater + amount);
		}
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x0005FAC8 File Offset: 0x0005DCC8
	private bool HasNutrientSoil()
	{
		LandPlot componentInParent = base.GetComponentInParent<LandPlot>();
		return componentInParent != null && componentInParent.HasUpgrade(LandPlot.Upgrade.SOIL);
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x0005FAEE File Offset: 0x0005DCEE
	private bool HasSprinkler()
	{
		return this.landPlot != null && this.landPlot.HasUpgrade(LandPlot.Upgrade.SPRINKLER);
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x0005FB0C File Offset: 0x0005DD0C
	private bool IsWatered()
	{
		return this.HasSprinkler() || this.model.storedWater > 0f;
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x0005FB2A File Offset: 0x0005DD2A
	private float AdditionalRipenessPerSecond()
	{
		if (!this.IsWatered())
		{
			return 0f;
		}
		return 0.5f;
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x0005FB40 File Offset: 0x0005DD40
	private IEnumerable<SpawnResource.SpawnMetadata> GetSpawnMetadatas(SpawnResource.SpawnRequest request, double worldTime)
	{
		int num = 0;
		if (this.MaxActiveSpawns != 0)
		{
			this.spawned.RemoveAll((GameObject go) => go == null);
			num = this.spawned.Count;
		}
		ReferenceCount<GameObject> referenceCount = new ReferenceCount<GameObject>();
		if (this.MaxActiveSpawns == 0 || num < this.MaxActiveSpawns)
		{
			int num2 = (int)UnityEngine.Random.Range(this.HasNutrientSoil() ? this.MinNutrientObjectsSpawned : this.MinObjectsSpawned, this.MaxObjectsSpawned);
			for (int i = 0; i < num2; i++)
			{
				GameObject[] array = (this.BonusObjectsToSpawn != null && this.BonusObjectsToSpawn.Length >= 1 && (i < this.minBonusSelections || UnityEngine.Random.value < this.BonusChance)) ? this.BonusObjectsToSpawn : this.ObjectsToSpawn;
				GameObject gameObject = array[this.rand.GetInRange(0, array.Length - 1)];
				ResourceCycle component = gameObject.GetComponent<ResourceCycle>();
				if (request.spawnAtTime == null || !component.WouldProgressToRotten(request.spawnAtTime.Value, worldTime))
				{
					referenceCount.Increment(gameObject);
				}
			}
		}
		return from pair in referenceCount
		select new SpawnResource.SpawnMetadata
		{
			request = request,
			prefab = pair.Key,
			count = pair.Value
		};
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x0005FC8F File Offset: 0x0005DE8F
	private void Spawn(SpawnResource.SpawnRequest request)
	{
		this.Spawn(request, this.timeDir.WorldTime());
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x0005FCA3 File Offset: 0x0005DEA3
	private void Spawn(SpawnResource.SpawnRequest request, double worldTime)
	{
		this.Spawn(this.GetSpawnMetadatas(request, worldTime));
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x0005FCB4 File Offset: 0x0005DEB4
	private void Spawn(IEnumerable<SpawnResource.SpawnMetadata> metadatas)
	{
		List<Joint> list = new List<Joint>();
		foreach (Joint joint in this.SpawnJoints)
		{
			if (joint.connectedBody == null)
			{
				list.Add(joint);
			}
			else if (this.forceDestroyLeftoversOnSpawn)
			{
				Destroyer.DestroyActor(joint.connectedBody.gameObject, "SpawnResource.Spawn#1", false);
				list.Add(joint);
			}
		}
		foreach (SpawnResource.SpawnMetadata spawnMetadata in metadatas)
		{
			for (int j = 0; j < spawnMetadata.count; j++)
			{
				Joint joint2 = this.rand.Pluck<Joint>(list, null);
				if (joint2 == null)
				{
					break;
				}
				Vector3 position = joint2.transform.position;
				Quaternion rotation = joint2.transform.rotation;
				GameObject gameObject = SRBehaviour.InstantiateActor(spawnMetadata.prefab, this.region.setId, position, rotation, false);
				ResourceCycle component = gameObject.GetComponent<ResourceCycle>();
				component.Attach(joint2, new ResourceCycle.AdditionalRipeness(this.AdditionalRipenessPerSecond), null);
				if (this.MaxActiveSpawns != 0)
				{
					this.spawned.Add(gameObject);
				}
				if (spawnMetadata.request.spawnRipe)
				{
					component.ProgressResource(this.timeDir.WorldTime());
				}
				else if (spawnMetadata.request.spawnAtTime != null)
				{
					component.ProgressResource(spawnMetadata.request.spawnAtTime.Value + (double)(component.unripeGameHours * 3600f));
				}
				else
				{
					if (this.SpawnFX != null)
					{
						SRBehaviour.SpawnAndPlayFX(this.SpawnFX, position, Quaternion.identity);
					}
					TweenUtil.ScaleIn(gameObject, 4f, Ease.OutQuad);
				}
			}
			if (this.MaxTotalSpawns != 0)
			{
				this.totalSpawnsRemaining -= spawnMetadata.count;
				if (this.totalSpawnsRemaining <= 0)
				{
					Destroyer.Destroy(base.gameObject, "SpawnResource.Spawn#2");
				}
			}
		}
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x0005FED0 File Offset: 0x0005E0D0
	private void UpdateNextSpawnTime(float preripenedHours)
	{
		this.model.nextSpawnTime = this.timeDir.HoursFromNow(-preripenedHours + this.rand.GetInRange(this.MinSpawnIntervalGameHours, this.MaxSpawnIntervalGameHours));
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x0005FF02 File Offset: 0x0005E102
	private void StepNextSpawnTime(float ripeness, float stepHours)
	{
		this.model.nextSpawnTime = TimeDirector.HoursFromTime(-ripeness + stepHours, this.model.nextSpawnTime);
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x0005FF24 File Offset: 0x0005E124
	public void RefreshSpawnJointObjectPositions()
	{
		foreach (FixedJoint fixedJoint in this.SpawnJoints)
		{
			if (fixedJoint.connectedBody != null)
			{
				fixedJoint.connectedBody.transform.position = fixedJoint.transform.position;
				fixedJoint.connectedBody.transform.rotation = fixedJoint.transform.rotation;
				RegionMember component = fixedJoint.connectedBody.GetComponent<RegionMember>();
				if (component != null)
				{
					component.UpdateRegionMembership(true);
				}
			}
		}
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x0005FFB0 File Offset: 0x0005E1B0
	public Joint PickRipeResourceJoint()
	{
		foreach (Joint joint in this.SpawnJoints)
		{
			if (joint.connectedBody != null && joint.connectedBody.GetComponent<ResourceCycle>().GetState() == ResourceCycle.State.RIPE)
			{
				return joint;
			}
		}
		return null;
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x0005FFFC File Offset: 0x0005E1FC
	public Joint NearestJoint(Vector3 pos, float maxDist)
	{
		float num = maxDist * maxDist;
		Joint result = null;
		foreach (Joint joint in this.SpawnJoints)
		{
			float sqrMagnitude = (joint.transform.position - pos).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = joint;
			}
		}
		return result;
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x00060052 File Offset: 0x0005E252
	private bool AllJointsDisconnected()
	{
		return this.SpawnJoints.All((Joint j) => j.connectedBody == null);
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x0006007E File Offset: 0x0005E27E
	public bool ReadyToDestroy()
	{
		this.model.nextSpawnTime = double.PositiveInfinity;
		return this.AllJointsDisconnected();
	}

	// Token: 0x060018A6 RID: 6310 RVA: 0x0006009A File Offset: 0x0005E29A
	public void RegisterSpawnBlocker()
	{
		this.spawnBlockers++;
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x000600AA File Offset: 0x0005E2AA
	public void DeregisterSpawnBlocker()
	{
		this.spawnBlockers--;
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x000600BA File Offset: 0x0005E2BA
	public void FastForward(double startTime, double endTime)
	{
		this.UpdateToTime(endTime, endTime - startTime);
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x000600C8 File Offset: 0x0005E2C8
	public IEnumerable<DroneFastForwarder.GatherGroup> GetFastForwardGroups(double endTime)
	{
		return (from m in this.spawnQueue.SelectMany((SpawnResource.SpawnRequest r) => this.GetSpawnMetadatas(r, endTime))
		group m by Identifiable.GetId(m.prefab) into g
		select new SpawnResource.GatherGroup(this, g.Key, g.Aggregate(0, (int agg, SpawnResource.SpawnMetadata m) => agg + m.count))).Cast<DroneFastForwarder.GatherGroup>();
	}

	// Token: 0x04001816 RID: 6166
	public static SpawnResource.IdComparer idComparer = new SpawnResource.IdComparer();

	// Token: 0x04001817 RID: 6167
	public SpawnResource.Id id;

	// Token: 0x04001818 RID: 6168
	public GameObject[] ObjectsToSpawn;

	// Token: 0x04001819 RID: 6169
	public GameObject[] BonusObjectsToSpawn;

	// Token: 0x0400181A RID: 6170
	public float MaxObjectsSpawned = 1f;

	// Token: 0x0400181B RID: 6171
	public float MinObjectsSpawned = 1f;

	// Token: 0x0400181C RID: 6172
	public float MinNutrientObjectsSpawned = 1f;

	// Token: 0x0400181D RID: 6173
	public float MinSpawnIntervalGameHours = 18f;

	// Token: 0x0400181E RID: 6174
	public float MaxSpawnIntervalGameHours = 24f;

	// Token: 0x0400181F RID: 6175
	public Joint[] SpawnJoints;

	// Token: 0x04001820 RID: 6176
	public float BonusChance = 0.01f;

	// Token: 0x04001821 RID: 6177
	public int minBonusSelections;

	// Token: 0x04001822 RID: 6178
	public GameObject SpawnFX;

	// Token: 0x04001823 RID: 6179
	public int MaxActiveSpawns;

	// Token: 0x04001824 RID: 6180
	public int MaxTotalSpawns;

	// Token: 0x04001825 RID: 6181
	public float wateringDurationHours = 23f;

	// Token: 0x04001826 RID: 6182
	public bool forceDestroyLeftoversOnSpawn;

	// Token: 0x04001827 RID: 6183
	[Tooltip("GameObject that is shown/hidden based off the watered state. (optional)")]
	public GameObject onWateredFX;

	// Token: 0x04001828 RID: 6184
	public UnityAction onReachedSpawnTime;

	// Token: 0x04001829 RID: 6185
	private bool allowSpawningInFastForwarding;

	// Token: 0x0400182A RID: 6186
	private Queue<SpawnResource.SpawnRequest> spawnQueue = new Queue<SpawnResource.SpawnRequest>();

	// Token: 0x0400182B RID: 6187
	private List<GameObject> spawned = new List<GameObject>();

	// Token: 0x0400182C RID: 6188
	private int totalSpawnsRemaining;

	// Token: 0x0400182D RID: 6189
	private Randoms rand;

	// Token: 0x0400182E RID: 6190
	private TimeDirector timeDir;

	// Token: 0x0400182F RID: 6191
	private AmbianceDirector ambianceDir;

	// Token: 0x04001830 RID: 6192
	private Region region;

	// Token: 0x04001831 RID: 6193
	private LandPlot landPlot;

	// Token: 0x04001832 RID: 6194
	private int spawnBlockers;

	// Token: 0x04001833 RID: 6195
	private SpawnResourceModel model;

	// Token: 0x04001834 RID: 6196
	private const float MAX_WATER_STORED = 3f;

	// Token: 0x04001835 RID: 6197
	private const float WATER_USED_PER_HOUR = 0.13043478f;

	// Token: 0x04001836 RID: 6198
	private const float WATERED_TIME_FACTOR = 0.5f;

	// Token: 0x04001837 RID: 6199
	private const float MIN_BONUS_RIPENESS = 3f;

	// Token: 0x04001838 RID: 6200
	private const float MAX_BONUS_RIPENESS = 9f;

	// Token: 0x04001839 RID: 6201
	private const float SCALE_IN_RESOURCE_TIME = 4f;

	// Token: 0x0400183A RID: 6202
	private const float FAST_FORWARD_MIN_HOURS = 0.25f;

	// Token: 0x0400183B RID: 6203
	private const float MIN_SPAWN_TIME_STEP = 1f;

	// Token: 0x0200049A RID: 1178
	public enum Id
	{
		// Token: 0x0400183D RID: 6205
		NONE,
		// Token: 0x0400183E RID: 6206
		CUBERRY_TREE,
		// Token: 0x0400183F RID: 6207
		MANGO_TREE,
		// Token: 0x04001840 RID: 6208
		CARROT_PATCH,
		// Token: 0x04001841 RID: 6209
		OCAOCA_PATCH,
		// Token: 0x04001842 RID: 6210
		PEAR_TREE,
		// Token: 0x04001843 RID: 6211
		POGO_TREE,
		// Token: 0x04001844 RID: 6212
		PARSNIP_PATCH,
		// Token: 0x04001845 RID: 6213
		BEET_PATCH,
		// Token: 0x04001846 RID: 6214
		ONION_PATCH,
		// Token: 0x04001847 RID: 6215
		LEMON_TREE,
		// Token: 0x04001848 RID: 6216
		GINGER_PATCH,
		// Token: 0x04001849 RID: 6217
		CUBERRY_TREE_DLX,
		// Token: 0x0400184A RID: 6218
		MANGO_TREE_DLX,
		// Token: 0x0400184B RID: 6219
		CARROT_PATCH_DLX,
		// Token: 0x0400184C RID: 6220
		OCAOCA_PATCH_DLX,
		// Token: 0x0400184D RID: 6221
		PEAR_TREE_DLX,
		// Token: 0x0400184E RID: 6222
		POGO_TREE_DLX,
		// Token: 0x0400184F RID: 6223
		PARSNIP_PATCH_DLX,
		// Token: 0x04001850 RID: 6224
		BEET_PATCH_DLX,
		// Token: 0x04001851 RID: 6225
		ONION_PATCH_DLX,
		// Token: 0x04001852 RID: 6226
		LEMON_TREE_DLX,
		// Token: 0x04001853 RID: 6227
		GINGER_PATCH_DLX
	}

	// Token: 0x0200049B RID: 1179
	public class IdComparer : IEqualityComparer<SpawnResource.Id>
	{
		// Token: 0x060018AC RID: 6316 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(SpawnResource.Id id1, SpawnResource.Id id2)
		{
			return id1 == id2;
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(SpawnResource.Id obj)
		{
			return (int)obj;
		}
	}

	// Token: 0x0200049C RID: 1180
	private class SpawnRequest
	{
		// Token: 0x04001854 RID: 6228
		public double? spawnAtTime;

		// Token: 0x04001855 RID: 6229
		public bool spawnRipe;
	}

	// Token: 0x0200049D RID: 1181
	private class SpawnMetadata
	{
		// Token: 0x04001856 RID: 6230
		public SpawnResource.SpawnRequest request;

		// Token: 0x04001857 RID: 6231
		public GameObject prefab;

		// Token: 0x04001858 RID: 6232
		public int count;
	}

	// Token: 0x0200049E RID: 1182
	private class GatherGroup : DroneFastForwarder.GatherGroup
	{
		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060018B1 RID: 6321 RVA: 0x000601BE File Offset: 0x0005E3BE
		public override Identifiable.Id id
		{
			get
			{
				return this.rid;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060018B2 RID: 6322 RVA: 0x000601C6 File Offset: 0x0005E3C6
		public override int count
		{
			get
			{
				return this.rcount;
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060018B3 RID: 6323 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public override bool overflow
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x000601CE File Offset: 0x0005E3CE
		public GatherGroup(SpawnResource resource, Identifiable.Id id, int count)
		{
			this.resource = resource;
			this.rid = id;
			this.rcount = count;
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x000601EC File Offset: 0x0005E3EC
		public override void Decrement(int decrement)
		{
			this.rcount = Mathf.Max(this.rcount - decrement, 0);
			if (this.rcount <= 0 && this.resource.spawnQueue.Any<SpawnResource.SpawnRequest>())
			{
				this.resource.spawnQueue.Dequeue();
			}
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x00060239 File Offset: 0x0005E439
		public override void Dispose()
		{
			this.rcount = 0;
		}

		// Token: 0x04001859 RID: 6233
		private SpawnResource resource;

		// Token: 0x0400185A RID: 6234
		private Identifiable.Id rid;

		// Token: 0x0400185B RID: 6235
		private int rcount;
	}
}
