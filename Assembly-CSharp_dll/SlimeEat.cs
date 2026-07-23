using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000469 RID: 1129
public class SlimeEat : CollidableActorBehaviour, Collidable, ActorModel.Participant
{
	// Token: 0x0600173A RID: 5946 RVA: 0x0005A0BF File Offset: 0x000582BF
	public static void ClearClaimedFood()
	{
		SlimeEat.claimedFood.Clear();
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x0005A0CC File Offset: 0x000582CC
	public List<SlimeDiet.EatMapEntry> GetEatMapById(Identifiable.Id id)
	{
		List<SlimeDiet.EatMapEntry> list = new List<SlimeDiet.EatMapEntry>();
		this.slimeDefinition.Diet.AddEatMapEntries(id, list);
		return list;
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x0005A0F4 File Offset: 0x000582F4
	static SlimeEat()
	{
		SlimeEat.foodGroupIds[SlimeEat.FoodGroup.VEGGIES] = new List<Identifiable.Id>(Identifiable.VEGGIE_CLASS).ToArray();
		SlimeEat.foodGroupIds[SlimeEat.FoodGroup.FRUIT] = new List<Identifiable.Id>(Identifiable.FRUIT_CLASS).ToArray();
		SlimeEat.foodGroupIds[SlimeEat.FoodGroup.MEAT] = new List<Identifiable.Id>(Identifiable.MEAT_CLASS).ToArray();
		List<Identifiable.Id> list = new List<Identifiable.Id>();
		foreach (object obj in Enum.GetValues(typeof(Identifiable.Id)))
		{
			Identifiable.Id id = (Identifiable.Id)obj;
			if (Identifiable.IsSlime(id) && !Identifiable.IsTarr(id) && id != Identifiable.Id.GOLD_SLIME && id != Identifiable.Id.LUCKY_SLIME)
			{
				list.Add(id);
			}
		}
		SlimeEat.foodGroupIds[SlimeEat.FoodGroup.NONTARRGOLD_SLIMES] = list.ToArray();
		List<Identifiable.Id> list2 = new List<Identifiable.Id>();
		foreach (object obj2 in Enum.GetValues(typeof(Identifiable.Id)))
		{
			Identifiable.Id id2 = (Identifiable.Id)obj2;
			if (Identifiable.IsPlort(id2) && id2 != Identifiable.Id.PUDDLE_PLORT && id2 != Identifiable.Id.GOLD_PLORT && id2 != Identifiable.Id.FIRE_PLORT)
			{
				list2.Add(id2);
			}
		}
		SlimeEat.foodGroupIds[SlimeEat.FoodGroup.PLORTS] = list2.ToArray();
		SlimeEat.foodGroupIds[SlimeEat.FoodGroup.GINGER] = new Identifiable.Id[]
		{
			Identifiable.Id.GINGER_VEGGIE
		};
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x0005A2D0 File Offset: 0x000584D0
	public static Identifiable.Id[] GetFoodGroupIds(SlimeEat.FoodGroup group)
	{
		return SlimeEat.foodGroupIds[group];
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x0005A2E0 File Offset: 0x000584E0
	public override void Awake()
	{
		base.Awake();
		this.chomper = base.GetComponent<Chomper>();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.faceAnim = base.GetComponent<SlimeFaceAnimator>();
		this.regionMember = base.GetComponent<RegionMember>();
		this.bodyAnim = base.GetComponentInChildren<Animator>();
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.modDir = SRSingleton<SceneContext>.Instance.ModDirector;
		this.tentacleGrapple = base.GetComponent<TentacleGrapple>();
		this.animDigestingId = Animator.StringToHash("Digesting");
		this.appearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(ActorModel actorModel)
	{
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x0005A383 File Offset: 0x00058583
	public void SetModel(ActorModel actorModel)
	{
		this.slimeModel = (actorModel as SlimeModel);
		this.InitFood();
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x0005A397 File Offset: 0x00058597
	public void InitFood()
	{
		this.CalculateAllEats();
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x0005A3A0 File Offset: 0x000585A0
	private void CalculateAllEats()
	{
		List<SlimeDiet.EatMapEntry> eatMap = this.slimeDefinition.Diet.EatMap;
		this.allEats = new Dictionary<Identifiable.Id, DriveCalculator>(Identifiable.idComparer);
		PlayerState x = (SRSingleton<SceneContext>.Instance == null) ? null : SRSingleton<SceneContext>.Instance.PlayerState;
		for (int i = 0; i < eatMap.Count; i++)
		{
			if (!(x != null) || !SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().preventHostiles || !Identifiable.IsTarr(eatMap[i].becomesId))
			{
				float num = (eatMap[i].eats == Identifiable.Id.HONEY_PLORT) ? 0.5f : 0f;
				this.allEats[eatMap[i].eats] = new DriveCalculator(eatMap[i].driver, eatMap[i].extraDrive + num, eatMap[i].minDrive);
			}
		}
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x0005A491 File Offset: 0x00058691
	public override void Start()
	{
		base.Start();
		this.ResetEatClock();
		if (!Identifiable.IsTarr(Identifiable.GetId(base.gameObject)) && this.regionMember.IsInRegion(RegionRegistry.RegionSetId.SLIMULATIONS))
		{
			this.isEatingEnabled = false;
		}
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x0005A4C6 File Offset: 0x000586C6
	public void ProcessCollisionEnter(Collision col)
	{
		this.MaybeSpinAndChomp(col.gameObject, false);
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x0005A4D8 File Offset: 0x000586D8
	public bool MaybeSpinAndChomp(GameObject obj, bool ignoreEmotions)
	{
		if (this.isEatingEnabled && this.chomper.CanChomp())
		{
			Identifiable.Id id = this.ExtractOtherId(obj);
			if (this.allEats.ContainsKey(id) && Identifiable.IsEdible(obj) && (ignoreEmotions || this.slimeModel.isFeral || (this.emotions != null && this.allEats[id].Drive(this.emotions, id) >= this.minDriveToEat)) && (this.tentacleGrapple == null || this.tentacleGrapple.IsGrappling(obj)))
			{
				base.transform.LookAt(obj.transform);
				this.chomper.StartChomp(obj, id, false, true, this.onStartChomp, new Chomper.OnChompCompleteDelegate(this.FinishChomp));
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x0005A5B4 File Offset: 0x000587B4
	public bool MaybeChomp(GameObject obj)
	{
		if (this.isEatingEnabled && this.chomper.CanChomp())
		{
			Identifiable.Id id = this.ExtractOtherId(obj);
			if (this.allEats.ContainsKey(id) && Identifiable.IsEdible(obj) && (this.slimeModel.isFeral || (this.emotions != null && this.allEats[id].Drive(this.emotions, id) >= this.minDriveToEat)))
			{
				this.chomper.StartChomp(obj, id, false, false, this.onStartChomp, new Chomper.OnChompCompleteDelegate(this.FinishChomp));
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x0005A667 File Offset: 0x00058867
	public void CancelChomp(GameObject obj)
	{
		this.chomper.CancelChomp(obj);
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x0005A678 File Offset: 0x00058878
	private Identifiable.Id ExtractOtherId(GameObject other)
	{
		int instanceID = other.GetInstanceID();
		Identifiable.Id result;
		if (SlimeEat.recentIds.contains(instanceID))
		{
			Identifiable identifiable = SlimeEat.recentIds.get(instanceID);
			result = ((identifiable == null) ? Identifiable.Id.NONE : identifiable.id);
		}
		else
		{
			Identifiable component = other.GetComponent<Identifiable>();
			SlimeEat.recentIds.put(instanceID, component);
			result = ((component == null) ? Identifiable.Id.NONE : component.id);
		}
		return result;
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x0005A6E4 File Offset: 0x000588E4
	private void FinishChomp(GameObject chomping, Identifiable.Id chompingId, bool whileHeld, bool wasLaunched)
	{
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.chompCue);
		if (chomping == null || SlimeEat.claimedFood.Contains(chomping))
		{
			return;
		}
		SlimeEat.claimedFood.Add(chomping);
		this.faceAnim.SetTrigger("triggerChompClosed");
		for (int i = 0; i < this.slimeDefinition.Diet.EatMap.Count; i++)
		{
			SlimeDiet.EatMapEntry eatMapEntry = this.slimeDefinition.Diet.EatMap[i];
			if (eatMapEntry.eats == chompingId)
			{
				if (eatMapEntry.producesId != Identifiable.Id.NONE)
				{
					this.EatAndProduce(chomping, eatMapEntry, false, false, false);
				}
				else if (eatMapEntry.becomesId != Identifiable.Id.NONE)
				{
					this.EatAndTransform(chomping, eatMapEntry, false);
				}
				else
				{
					this.DoDamage(chomping, false);
				}
				this.OnEat(eatMapEntry.driver, chompingId, wasLaunched, eatMapEntry.isFavorite);
			}
		}
		if (this.onFinishChompSuccess != null)
		{
			this.onFinishChompSuccess(chomping);
		}
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x0005A7E0 File Offset: 0x000589E0
	private void EatAndTransform(GameObject target, SlimeDiet.EatMapEntry em, bool immediateMode)
	{
		if (!immediateMode)
		{
			SRBehaviour.SpawnAndPlayFX(this.TransformFX, base.transform.position, base.transform.rotation);
		}
		if (this.DoDamage(target, immediateMode))
		{
			SlimeEmotions component = base.GetComponent<SlimeEmotions>();
			Destroyer.DestroyActor(base.gameObject, "SlimeEat.EatAndTransform", false);
			GameObject gameObject = SRBehaviour.InstantiateActor(this.lookupDir.GetPrefab(em.becomesId), this.regionMember.setId, base.transform.position, base.transform.rotation, false);
			SlimeEmotions component2 = gameObject.GetComponent<SlimeEmotions>();
			if (component2 != null)
			{
				component2.SetAll(component);
			}
			gameObject.transform.DOScale(gameObject.transform.localScale, 0.5f).From(base.gameObject.transform.localScale, true).SetEase(Ease.OutElastic);
			OnTransformed component3 = gameObject.GetComponent<OnTransformed>();
			if (component3 != null)
			{
				component3.OnTransformed();
			}
		}
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x0005A8D0 File Offset: 0x00058AD0
	private void EatAndProduce(GameObject target, SlimeDiet.EatMapEntry em, bool immediateMode, bool skipDelays, bool skipProduction)
	{
		this.bodyAnim.SetBool(this.animDigestingId, true);
		if (!immediateMode)
		{
			if (em.isFavorite)
			{
				SRBehaviour.SpawnAndPlayFX(this.EatFavoriteFX, target.transform.position, target.transform.rotation);
			}
			else
			{
				SRBehaviour.SpawnAndPlayFX(this.EatFX, target.transform.position, target.transform.rotation);
			}
		}
		if (this.DoDamage(target, immediateMode))
		{
			float delay = 2f;
			int num = em.NumToProduce();
			if (immediateMode)
			{
				delay = 0f;
				num = 1;
			}
			if (target != null)
			{
				Destroyer.DestroyActor(target, "SlimeEat.EatAndProduce", false);
			}
			if (!skipProduction && (this.chanceToSkipProduce <= 0f || !Randoms.SHARED.GetProbability(this.chanceToSkipProduce)))
			{
				GameObject prefab = this.lookupDir.GetPrefab(em.producesId);
				if (Randoms.SHARED.GetProbability(this.modDir.ChanceRandomPlort()) && Identifiable.IsPlort(em.producesId))
				{
					prefab = this.lookupDir.GetPrefab(Randoms.SHARED.Pick<Identifiable.Id>(Identifiable.PLORT_CLASS, Identifiable.Id.NONE));
				}
				if (em.producesId == Identifiable.Id.GOLD_PLORT && num >= 3)
				{
					SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.GOLD_SLIME_TRIPLE_PLORT, 1);
				}
				if (skipDelays)
				{
					this.Produce(num, prefab);
					return;
				}
				base.StartCoroutine(this.ProduceAfterDelay(num, prefab, delay));
				return;
			}
			else
			{
				if (skipDelays)
				{
					this.Digest();
					return;
				}
				base.StartCoroutine(this.DigestOnlyAfterDelay(delay));
			}
		}
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x0005AA4C File Offset: 0x00058C4C
	public void EatImmediate(GameObject target, Identifiable.Id eatId, List<Identifiable.Id> produceIds, List<Identifiable.Id> alreadyCollectedIds, bool skipDelays)
	{
		if (target != null)
		{
			if (SlimeEat.claimedFood.Contains(target))
			{
				return;
			}
			SlimeEat.claimedFood.Add(target);
		}
		List<SlimeDiet.EatMapEntry> eatMap = this.slimeDefinition.Diet.EatMap;
		SlimeDiet.EatMapEntry eatMapEntry = null;
		foreach (Identifiable.Id id in produceIds)
		{
			for (int i = 0; i < eatMap.Count; i++)
			{
				if (eatMap[i].eats == eatId && eatMap[i].producesId == id)
				{
					eatMapEntry = eatMap[i];
					bool skipProduction = alreadyCollectedIds.Remove(id);
					this.EatAndProduce(target, eatMapEntry, true, skipDelays, skipProduction);
				}
			}
		}
		if (eatMapEntry != null)
		{
			this.OnEat(eatMapEntry.driver, eatId, false, eatMapEntry.isFavorite);
		}
		SlimeDiet.EatMapEntry eatMapEntry2 = null;
		for (int j = 0; j < eatMap.Count; j++)
		{
			if (eatMap[j].eats == eatId && eatMap[j].becomesId != Identifiable.Id.NONE)
			{
				eatMapEntry2 = eatMap[j];
			}
		}
		if (eatMapEntry2 != null)
		{
			this.EatAndTransform(target, eatMapEntry2, true);
			this.OnEat(eatMapEntry2.driver, eatId, false, eatMapEntry2.isFavorite);
		}
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x0005AB98 File Offset: 0x00058D98
	public List<Identifiable.Id> GetProducedIds(Identifiable.Id foodId, List<Identifiable.Id> producedIdList)
	{
		List<SlimeDiet.EatMapEntry> eatMap = this.slimeDefinition.Diet.EatMap;
		producedIdList.Clear();
		for (int i = 0; i < eatMap.Count; i++)
		{
			if (eatMap[i].eats == foodId && eatMap[i].producesId != Identifiable.Id.NONE)
			{
				producedIdList.Add(eatMap[i].producesId);
				if (eatMap[i].isFavorite)
				{
					producedIdList.Add(eatMap[i].producesId);
				}
			}
		}
		return producedIdList;
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x0005AC1D File Offset: 0x00058E1D
	private IEnumerator DigestOnlyAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (base.gameObject != null)
		{
			this.Digest();
		}
		yield break;
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x0005AC33 File Offset: 0x00058E33
	private void Digest()
	{
		this.bodyAnim.SetBool(this.animDigestingId, false);
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x0005AC47 File Offset: 0x00058E47
	private IEnumerator ProduceAfterDelay(int count, GameObject produces, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (base.gameObject != null)
		{
			this.Produce(count, produces);
		}
		yield break;
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x0005AC6C File Offset: 0x00058E6C
	private void Produce(int count, GameObject produces)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3 position = base.transform.TransformPoint(SlimeEat.LOCAL_PRODUCE_LOC);
			Vector3 velocity = base.transform.TransformVector(SlimeEat.LOCAL_PRODUCE_VEL);
			if (this.ProduceFX != null)
			{
				RecolorSlimeMaterial[] componentsInChildren = SRBehaviour.SpawnAndPlayFX(this.ProduceFX, position, base.transform.rotation).GetComponentsInChildren<RecolorSlimeMaterial>();
				if (componentsInChildren != null && componentsInChildren.Length != 0)
				{
					SlimeAppearance.Palette appearancePalette = this.appearanceApplicator.GetAppearancePalette();
					RecolorSlimeMaterial[] array = componentsInChildren;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].SetColors(appearancePalette.Top, appearancePalette.Middle, appearancePalette.Bottom);
					}
				}
			}
			GameObject gameObject = SRBehaviour.InstantiateActor(produces, this.regionMember.setId, position, base.transform.rotation, false);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.velocity = velocity;
			}
			PlortInvulnerability component2 = gameObject.GetComponent<PlortInvulnerability>();
			if (component2 != null)
			{
				component2.GoInvulnerable();
			}
			gameObject.transform.DOScale(gameObject.transform.localScale, 0.5f).From(new Vector3(0.01f, 0.01f, 0.01f), true).SetEase(Ease.Linear);
		}
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.plortCue);
		this.bodyAnim.SetBool(this.animDigestingId, false);
		if (this.onProducePlortsComplete != null)
		{
			this.onProducePlortsComplete();
		}
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x0005ADF4 File Offset: 0x00058FF4
	private void OnEat(SlimeEmotions.Emotion driver, Identifiable.Id otherId, bool eatingLaunchedFood, bool isFavorite)
	{
		this.ResetEatClock();
		this.emotions.Adjust(driver, -this.drivePerEat);
		this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, isFavorite ? (-this.agitationPerFavEat) : (-this.agitationPerEat));
		if (this.onEat != null)
		{
			this.onEat(otherId);
		}
		if (Identifiable.IsAnimal(otherId) && CellDirector.IsOnRanch(this.regionMember))
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.CHICKENS_FED_SLIMES, 1);
		}
		if (isFavorite && CellDirector.IsOnRanch(this.regionMember))
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.FED_FAVORITE, 1);
		}
		if (eatingLaunchedFood)
		{
			SlimeSubbehaviourPlexer component = base.GetComponent<SlimeSubbehaviourPlexer>();
			if (component != null && !component.IsGrounded())
			{
				SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.FED_AIRBORNE, 1);
			}
		}
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x0005AEC5 File Offset: 0x000590C5
	public bool IsChomping()
	{
		return this.chomper.IsChomping();
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x0005AED2 File Offset: 0x000590D2
	public void ResetEatClock()
	{
		this.chomper.ResetEatClock();
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x0005AEDF File Offset: 0x000590DF
	public Dictionary<Identifiable.Id, DriveCalculator> GetAllEats()
	{
		return new Dictionary<Identifiable.Id, DriveCalculator>(this.allEats, Identifiable.idComparer);
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x0005AEF1 File Offset: 0x000590F1
	public bool DoesEat(GameObject gameObject)
	{
		return this.DoesEat(this.ExtractOtherId(gameObject)) && Identifiable.IsEdible(gameObject);
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x0005AF0A File Offset: 0x0005910A
	public bool DoesEat(Identifiable.Id id)
	{
		return this.allEats.ContainsKey(id);
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x0005AF18 File Offset: 0x00059118
	public bool WillNowEat(Identifiable.Id id)
	{
		return this.allEats.ContainsKey(id) && (this.slimeModel.isFeral || (this.emotions != null && this.allEats[id].Drive(this.emotions, id) > this.minDriveToEat));
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x0005AF74 File Offset: 0x00059174
	public bool WantsToEat()
	{
		return this.slimeModel.isFeral || (!(this.emotions == null) && this.allEats.Count >= 1 && this.emotions.GetCurr(SlimeEmotions.Emotion.HUNGER) > this.minDriveToEat);
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x0005AFC4 File Offset: 0x000591C4
	private bool DoDamage(GameObject other, bool immediateMode)
	{
		if (other == null)
		{
			return true;
		}
		if (!immediateMode)
		{
			this.slimeAudio.Play(this.slimeAudio.slimeSounds.gulpCue);
		}
		Damageable interfaceComponent = other.GetInterfaceComponent<Damageable>();
		if (interfaceComponent == null)
		{
			if (!immediateMode)
			{
				this.PlayOnDeathAudio(other);
			}
			Destroyer.DestroyActor(other, "SlimeEat.DoDamage#1", false);
			return true;
		}
		if (interfaceComponent.Damage(this.damagePerAttack, base.gameObject))
		{
			DeathHandler.Kill(other, DeathHandler.Source.SLIME_ATTACK, base.gameObject, "SlimeEat.DoDamage#2");
			if (!immediateMode)
			{
				this.PlayOnDeathAudio(other);
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x0005B050 File Offset: 0x00059250
	private void PlayOnDeathAudio(GameObject other)
	{
		SlimeAudio componentInChildren = other.GetComponentInChildren<SlimeAudio>();
		if (componentInChildren != null && componentInChildren.slimeSounds.voiceDamageCue != null)
		{
			SECTR_AudioSystem.Play(componentInChildren.slimeSounds.voiceDamageCue, other.transform.position, false);
		}
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x0005B09D File Offset: 0x0005929D
	public IEnumerable<Identifiable.Id> GetProducedIds()
	{
		return this.slimeDefinition.Diet.Produces.AsEnumerable<Identifiable.Id>();
	}

	// Token: 0x04001663 RID: 5731
	public SlimeDefinition slimeDefinition;

	// Token: 0x04001664 RID: 5732
	public SlimeEat.OnEatDelegate onEat;

	// Token: 0x04001665 RID: 5733
	public Chomper.OnChompStartDelegate onStartChomp;

	// Token: 0x04001666 RID: 5734
	public SlimeEat.OnFinishChompSuccessDelegate onFinishChompSuccess;

	// Token: 0x04001667 RID: 5735
	public SlimeEat.OnProducePlortsCompleteDelegate onProducePlortsComplete;

	// Token: 0x04001668 RID: 5736
	public float chanceToSkipProduce;

	// Token: 0x04001669 RID: 5737
	public const float WIND_UP_TIME = 1f;

	// Token: 0x0400166A RID: 5738
	public const float WIND_UP_TIME_QUICK = 0.25f;

	// Token: 0x0400166B RID: 5739
	public const float DIGEST_TIME = 2f;

	// Token: 0x0400166C RID: 5740
	public int damagePerAttack = 20;

	// Token: 0x0400166D RID: 5741
	public GameObject EatFX;

	// Token: 0x0400166E RID: 5742
	public GameObject EatFavoriteFX;

	// Token: 0x0400166F RID: 5743
	public GameObject TransformFX;

	// Token: 0x04001670 RID: 5744
	public GameObject ProduceFX;

	// Token: 0x04001671 RID: 5745
	[Header("Food Groups")]
	[Tooltip("Types of food to ignore even if in the food groups.")]
	public Identifiable.Id[] foodGroupsExceptions;

	// Token: 0x04001672 RID: 5746
	[Tooltip("Standard set of objects produced by anything covered by the food groups.")]
	public Identifiable.Id[] foodGroupsProduceId;

	// Token: 0x04001673 RID: 5747
	[Tooltip("Standard object to become when eating anything covered by the food groups.")]
	public Identifiable.Id foodGroupsBecomesId;

	// Token: 0x04001674 RID: 5748
	[Tooltip("Standard driver to use for anything covered by the food groups.")]
	public SlimeEmotions.Emotion foodGroupsDriver;

	// Token: 0x04001675 RID: 5749
	[Tooltip("Standard extra drive to use for anything covered by the food groups.")]
	public float foodGroupsExtraDrive;

	// Token: 0x04001676 RID: 5750
	[Tooltip("Standard minimum drive to use for anything covered by the food groups.")]
	public float foodGroupsMinDrive;

	// Token: 0x04001677 RID: 5751
	[Space(10f)]
	public float minDriveToEat = 0.333f;

	// Token: 0x04001678 RID: 5752
	public float drivePerEat = 0.333f;

	// Token: 0x04001679 RID: 5753
	public float agitationPerEat = 0.15f;

	// Token: 0x0400167A RID: 5754
	public float agitationPerFavEat = 0.3f;

	// Token: 0x0400167B RID: 5755
	private Dictionary<Identifiable.Id, DriveCalculator> allEats = new Dictionary<Identifiable.Id, DriveCalculator>(Identifiable.idComparer);

	// Token: 0x0400167C RID: 5756
	private SlimeEmotions emotions;

	// Token: 0x0400167D RID: 5757
	private bool isEatingEnabled = true;

	// Token: 0x0400167E RID: 5758
	private SlimeAudio slimeAudio;

	// Token: 0x0400167F RID: 5759
	private RegionMember regionMember;

	// Token: 0x04001680 RID: 5760
	private Chomper chomper;

	// Token: 0x04001681 RID: 5761
	private SlimeFaceAnimator faceAnim;

	// Token: 0x04001682 RID: 5762
	private Animator bodyAnim;

	// Token: 0x04001683 RID: 5763
	private TentacleGrapple tentacleGrapple;

	// Token: 0x04001684 RID: 5764
	private LookupDirector lookupDir;

	// Token: 0x04001685 RID: 5765
	private ModDirector modDir;

	// Token: 0x04001686 RID: 5766
	private static HashSet<GameObject> claimedFood = new HashSet<GameObject>();

	// Token: 0x04001687 RID: 5767
	private static readonly Vector3 LOCAL_PRODUCE_LOC = new Vector3(0f, 0.5f, 0f);

	// Token: 0x04001688 RID: 5768
	private static readonly Vector3 LOCAL_PRODUCE_VEL = new Vector3(0f, 1f, 0f);

	// Token: 0x04001689 RID: 5769
	private const float TRANSFORM_SCALE_UP_TIME = 0.5f;

	// Token: 0x0400168A RID: 5770
	private const float PRODUCE_SCALE_UP_TIME = 0.5f;

	// Token: 0x0400168B RID: 5771
	private const float FERAL_EXTRA_DRIVE = 0f;

	// Token: 0x0400168C RID: 5772
	private const float HONEY_PLORT_EXTRA_DRIVE = 0.5f;

	// Token: 0x0400168D RID: 5773
	private static Dictionary<SlimeEat.FoodGroup, Identifiable.Id[]> foodGroupIds = new Dictionary<SlimeEat.FoodGroup, Identifiable.Id[]>();

	// Token: 0x0400168E RID: 5774
	private int animDigestingId;

	// Token: 0x0400168F RID: 5775
	private SlimeModel slimeModel;

	// Token: 0x04001690 RID: 5776
	private SlimeAppearanceApplicator appearanceApplicator;

	// Token: 0x04001691 RID: 5777
	private static LRUCache<int, Identifiable> recentIds = new LRUCache<int, Identifiable>(200);

	// Token: 0x0200046A RID: 1130
	// (Invoke) Token: 0x06001760 RID: 5984
	public delegate void OnEatDelegate(Identifiable.Id id);

	// Token: 0x0200046B RID: 1131
	// (Invoke) Token: 0x06001764 RID: 5988
	public delegate void OnFinishChompSuccessDelegate(GameObject gameObject);

	// Token: 0x0200046C RID: 1132
	// (Invoke) Token: 0x06001768 RID: 5992
	public delegate void OnProducePlortsCompleteDelegate();

	// Token: 0x0200046D RID: 1133
	public enum FoodGroup
	{
		// Token: 0x04001693 RID: 5779
		FRUIT,
		// Token: 0x04001694 RID: 5780
		VEGGIES,
		// Token: 0x04001695 RID: 5781
		MEAT,
		// Token: 0x04001696 RID: 5782
		NONTARRGOLD_SLIMES,
		// Token: 0x04001697 RID: 5783
		PLORTS,
		// Token: 0x04001698 RID: 5784
		GINGER
	}
}
