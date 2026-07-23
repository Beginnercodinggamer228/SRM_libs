using System;
using System.Collections;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using Noise;
using UnityEngine;

// Token: 0x020003E9 RID: 1001
public class GordoEat : IdHandler, GadgetModel.Participant, GordoModel.Participant
{
	// Token: 0x060014DE RID: 5342 RVA: 0x000513CC File Offset: 0x0004F5CC
	public void Awake()
	{
		this.rewards = base.GetComponent<GordoRewardsBase>();
		this.initScale = base.transform.localScale.x;
		this.vibrateInitScale = this.toVibrate.localScale.x;
		this.cellDirector = base.GetComponentInParent<CellDirector>();
		if (!string.IsNullOrEmpty(base.id))
		{
			SRSingleton<SceneContext>.Instance.GameModel.RegisterGordo(base.id, base.gameObject);
		}
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x00051448 File Offset: 0x0004F648
	public void Start()
	{
		this.allEats.AddRange(this.slimeDefinition.Diet.GetDietIdentifiableIds());
		int eatenCount = this.GetEatenCount();
		if (eatenCount != -1 && eatenCount >= this.GetTargetCount())
		{
			this.ImmediateReachedTarget();
		}
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x0005148A File Offset: 0x0004F68A
	public ZoneDirector.Zone GetZoneId()
	{
		if (this.cellDirector != null)
		{
			return this.cellDirector.GetZoneId();
		}
		return ZoneDirector.Zone.NONE;
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x000514A7 File Offset: 0x0004F6A7
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null && !string.IsNullOrEmpty(base.id))
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterGordo(base.id);
		}
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x000514D8 File Offset: 0x0004F6D8
	public bool HasPopped()
	{
		return this.GetEatenCount() == -1;
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x000514E3 File Offset: 0x0004F6E3
	public void InitModel(GadgetModel model)
	{
		((SnareModel)model).gordoTargetCount = this.targetCount;
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x000514F6 File Offset: 0x0004F6F6
	public void SetModel(GadgetModel model)
	{
		this.snareModel = (SnareModel)model;
		if (this.snareModel.gordoEatenCount == -1)
		{
			this.rewards.SetupActiveRewards();
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x00051529 File Offset: 0x0004F729
	public void InitModel(GordoModel model)
	{
		model.targetCount = this.targetCount;
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x00051537 File Offset: 0x0004F737
	public virtual void SetModel(GordoModel model)
	{
		this.gordoModel = model;
		if (this.gordoModel.gordoEatenCount == -1)
		{
			this.rewards.SetupActiveRewards();
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x00051565 File Offset: 0x0004F765
	public void OnResetEatenCount()
	{
		base.GetComponent<GordoFaceAnimator>().SetDefaultState();
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x00051572 File Offset: 0x0004F772
	public int GetEatenCount()
	{
		if (this.gordoModel != null)
		{
			return this.gordoModel.gordoEatenCount;
		}
		if (this.snareModel != null)
		{
			return this.snareModel.gordoEatenCount;
		}
		return 0;
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x0005159D File Offset: 0x0004F79D
	public int GetTargetCount()
	{
		if (this.gordoModel != null)
		{
			return this.gordoModel.targetCount;
		}
		if (this.snareModel != null)
		{
			return this.snareModel.gordoTargetCount;
		}
		return 0;
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x000515C8 File Offset: 0x0004F7C8
	protected void SetEatenCount(int eatenCount)
	{
		if (this.gordoModel != null)
		{
			this.gordoModel.gordoEatenCount = eatenCount;
			return;
		}
		if (this.snareModel != null)
		{
			this.snareModel.gordoEatenCount = eatenCount;
		}
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x000515F4 File Offset: 0x0004F7F4
	public bool CanEat()
	{
		int eatenCount = this.GetEatenCount();
		return eatenCount != -1 && eatenCount < this.GetTargetCount();
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x00051618 File Offset: 0x0004F818
	public bool MaybeEat(Collider col)
	{
		if (!this.CanEat())
		{
			return false;
		}
		Identifiable component = col.GetComponent<Identifiable>();
		if (component != null && this.allEats.Contains(component.id) && !this.eating.Contains(col.gameObject))
		{
			List<SlimeDiet.EatMapEntry> eatMap = this.slimeDefinition.Diet.EatMap;
			for (int i = 0; i < eatMap.Count; i++)
			{
				SlimeDiet.EatMapEntry eatMapEntry = eatMap[i];
				if (eatMapEntry.eats == component.id)
				{
					this.DoEat(col.gameObject);
					this.SetEatenCount(this.GetEatenCount() + eatMapEntry.NumToProduce());
					if (eatMapEntry.isFavorite)
					{
						SRBehaviour.SpawnAndPlayFX(this.EatFavoriteFX, col.gameObject.transform.position, col.gameObject.transform.rotation);
					}
					if (this.GetEatenCount() >= this.GetTargetCount())
					{
						base.StartCoroutine(this.ReachedTarget());
					}
					return true;
				}
			}
			this.SetEatenCount(this.gordoModel.gordoEatenCount);
		}
		return false;
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x00051730 File Offset: 0x0004F930
	public void Update()
	{
		float num = Noise.PerlinNoise(0.0, 0f, Time.time, 0.1f, this.vibrationFactor, 2f);
		float percentageFed = this.GetPercentageFed();
		float num2 = Mathf.Lerp(this.initScale, this.initScale * this.growthFactor, percentageFed);
		float num3 = 0.7f;
		base.transform.localScale = new Vector3(num2, num2, num2);
		float num4 = this.vibrateInitScale * ((percentageFed <= num3) ? 1f : (1f + num * (percentageFed - num3) / (1f - num3)));
		this.toVibrate.localScale = new Vector3(num4, num4, num4);
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x000517DD File Offset: 0x0004F9DD
	public void LateUpdate()
	{
		this.eating.Clear();
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x000517EC File Offset: 0x0004F9EC
	public float GetPercentageFed()
	{
		int eatenCount = this.GetEatenCount();
		int num = this.GetTargetCount();
		return (float)((eatenCount == -1) ? num : eatenCount) / (float)num;
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x00051814 File Offset: 0x0004FA14
	private void DoEat(GameObject obj)
	{
		if (this.eatFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.eatFX, obj.transform.position, obj.transform.localRotation);
		}
		if (this.eatCue != null)
		{
			SECTR_AudioSystem.Play(this.eatCue, obj.transform.position, false);
		}
		Destroyer.DestroyActor(obj, "GordoEat.DoEat", false);
		this.eating.Add(obj);
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x00051890 File Offset: 0x0004FA90
	private void ImmediateReachedTarget()
	{
		this.rewards.GiveRewards();
		base.gameObject.SetActive(false);
		this.SetEatenCount(-1);
		SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.BURST_GORDOS, 1);
		AnalyticsUtil.CustomEvent("GordoBurst", new Dictionary<string, object>
		{
			{
				"type",
				base.name
			}
		}, true);
		SRSingleton<SceneContext>.Instance.PediaDirector.MaybeShowPopup(this.GetPediaId());
		GordoSnare componentInParent = base.GetComponentInParent<GordoSnare>();
		if (componentInParent != null)
		{
			componentInParent.Destroy();
			Destroyer.Destroy(base.gameObject, 0f, "GordoEat.ImmediateReachedTarget", true, false);
		}
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x00051930 File Offset: 0x0004FB30
	protected virtual PediaDirector.Id GetPediaId()
	{
		return PediaDirector.Id.GORDO_SLIME;
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x00051937 File Offset: 0x0004FB37
	private IEnumerator ReachedTarget()
	{
		this.WillStartBurst();
		base.GetComponent<GordoFaceAnimator>().SetTrigger("Strain");
		SECTR_AudioSystem.Play(this.strainCue, base.transform.position, false);
		yield return new WaitForSeconds(2f);
		SECTR_AudioSystem.Play(this.burstCue, base.transform.position, false);
		if (this.destroyFX != null)
		{
			GameObject gameObject = SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position + Vector3.up * 2f, base.transform.rotation);
			Identifiable component = base.gameObject.GetComponent<Identifiable>();
			Color[] colors = SlimeUtil.GetColors(base.gameObject, (component != null) ? component.id : Identifiable.Id.NONE, true);
			RecolorSlimeMaterial[] componentsInChildren = gameObject.GetComponentsInChildren<RecolorSlimeMaterial>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetColors(colors[0], colors[1], colors[2]);
			}
		}
		this.DidCompleteBurst();
		this.ImmediateReachedTarget();
		yield break;
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x00051946 File Offset: 0x0004FB46
	public bool DropsKey()
	{
		return this.rewards.HasKeyReward();
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void WillStartBurst()
	{
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void DidCompleteBurst()
	{
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x00051953 File Offset: 0x0004FB53
	public string GetDirectFoodGroupsMsg()
	{
		return this.slimeDefinition.Diet.GetDirectFoodGroupsMsg();
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x00051965 File Offset: 0x0004FB65
	protected override string IdPrefix()
	{
		return "gordo";
	}

	// Token: 0x040013BD RID: 5053
	public SlimeDefinition slimeDefinition;

	// Token: 0x040013BE RID: 5054
	public int targetCount = 100;

	// Token: 0x040013BF RID: 5055
	public GameObject eatFX;

	// Token: 0x040013C0 RID: 5056
	public SECTR_AudioCue eatCue;

	// Token: 0x040013C1 RID: 5057
	public GameObject destroyFX;

	// Token: 0x040013C2 RID: 5058
	public float growthFactor = 1.5f;

	// Token: 0x040013C3 RID: 5059
	public float vibrationFactor;

	// Token: 0x040013C4 RID: 5060
	public GameObject slimePrefab;

	// Token: 0x040013C5 RID: 5061
	public GameObject slimeSpawnFXPrefab;

	// Token: 0x040013C6 RID: 5062
	public SECTR_AudioCue strainCue;

	// Token: 0x040013C7 RID: 5063
	public SECTR_AudioCue burstCue;

	// Token: 0x040013C8 RID: 5064
	public GameObject EatFavoriteFX;

	// Token: 0x040013C9 RID: 5065
	public Transform toVibrate;

	// Token: 0x040013CA RID: 5066
	private List<Identifiable.Id> allEats = new List<Identifiable.Id>();

	// Token: 0x040013CB RID: 5067
	protected SnareModel snareModel;

	// Token: 0x040013CC RID: 5068
	protected GordoModel gordoModel;

	// Token: 0x040013CD RID: 5069
	private GordoRewardsBase rewards;

	// Token: 0x040013CE RID: 5070
	private HashSet<GameObject> eating = new HashSet<GameObject>();

	// Token: 0x040013CF RID: 5071
	private CellDirector cellDirector;

	// Token: 0x040013D0 RID: 5072
	private float initScale;

	// Token: 0x040013D1 RID: 5073
	private float vibrateInitScale;

	// Token: 0x040013D2 RID: 5074
	protected const float EXPLODE_DELAY = 2f;

	// Token: 0x040013D3 RID: 5075
	public const int ALREADY_BURST_FLAG = -1;
}
