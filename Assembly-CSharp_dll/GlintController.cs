using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Util.Extensions;
using UnityEngine;

// Token: 0x020003E2 RID: 994
public class GlintController : RegisteredActorBehaviour, RegistryUpdateable, LiquidConsumer
{
	// Token: 0x060014B7 RID: 5303 RVA: 0x00050898 File Offset: 0x0004EA98
	public void Awake()
	{
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		this.slimeAppearanceApplicator.OnAppearanceChanged += this.UpdateGlintAppearance;
		if (this.slimeAppearanceApplicator.Appearance != null)
		{
			this.UpdateGlintAppearance(this.slimeAppearanceApplicator.Appearance);
		}
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x00050908 File Offset: 0x0004EB08
	public override void Start()
	{
		base.Start();
		float curr = this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION);
		this.nextSpawnTime = this.timeDirector.HoursFromNow(GlintController.AdjustTime(0.5f, curr));
		this.Initialize(GlintController.Phase.SUSPENDED, GlintController.AdjustTime(1f, curr), curr);
		this.Initialize(GlintController.Phase.READY, GlintController.AdjustTime(0.5f, curr), curr);
		this.previousUpdateTime = this.timeDirector.WorldTime();
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x0005097C File Offset: 0x0004EB7C
	public override void OnEnable()
	{
		base.OnEnable();
		foreach (GlintController.Glint glint in this.glints)
		{
			if (glint.gameObject != null)
			{
				glint.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x000509E8 File Offset: 0x0004EBE8
	public override void OnDisable()
	{
		base.OnDisable();
		foreach (GlintController.Glint glint in this.glints)
		{
			if (glint.gameObject != null)
			{
				glint.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x00050A54 File Offset: 0x0004EC54
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SRSingleton<SceneContext>.Instance != null)
		{
			this.DestroyAllGlints();
		}
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x00050A6F File Offset: 0x0004EC6F
	public void RegistryUpdate()
	{
		if (this.timeDirector.HasReached(this.nextUpdateTime))
		{
			this.nextUpdateTime = this.timeDirector.HoursFromNow(0.16666667f);
			this.UpdateToTime(this.timeDirector.WorldTime());
		}
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x00050AAB File Offset: 0x0004ECAB
	public void AddLiquid(Identifiable.Id liquidId, float units)
	{
		if (Identifiable.IsWater(liquidId))
		{
			this.DestroyAllGlints();
		}
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x00050ABB File Offset: 0x0004ECBB
	private static float AdjustTime(float hours, float agitation)
	{
		return hours * (1f - 0.8f * agitation);
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x00050ACC File Offset: 0x0004ECCC
	private void UpdateGlintAppearance(SlimeAppearance appearance)
	{
		this.suspendedGlintPrefab = appearance.GlintAppearance.suspendedGlintPrefab;
		this.readyGlintPrefab = appearance.GlintAppearance.readyGlintPrefab;
		this.freeGlintPrefab = appearance.GlintAppearance.freeGlintPrefab;
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x00050B04 File Offset: 0x0004ED04
	private void DestroyAllGlints()
	{
		foreach (GlintController.Glint glint in this.glints)
		{
			if (glint.gameObject != null)
			{
				SRBehaviour.RequestDestroy(glint.gameObject, "GlintController.DestroyAllGlints");
			}
		}
		this.glints.Clear();
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x00050B7C File Offset: 0x0004ED7C
	private void UpdateToTime(double time)
	{
		float curr = this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION);
		if (time - this.previousUpdateTime >= 7200.0)
		{
			this.DestroyAllGlints();
			this.Initialize(GlintController.Phase.SUSPENDED, GlintController.AdjustTime(1f, curr), curr);
			this.Initialize(GlintController.Phase.READY, GlintController.AdjustTime(0.5f, curr), curr);
			this.Initialize(GlintController.Phase.FREE, GlintController.AdjustTime(0.5f, curr), curr);
		}
		else
		{
			this.glints.RemoveAll((GlintController.Glint g) => g.gameObject == null);
			foreach (GlintController.Glint glint in this.glints)
			{
				if (TimeUtil.HasReached(time, glint.phaseTimes[GlintController.Phase.FREE]))
				{
					SRBehaviour.RequestDestroy(glint.gameObject, "GlintDestroyer.UpdateToTime");
					glint.gameObject = null;
				}
				else if (glint.phase < GlintController.Phase.FREE && TimeUtil.HasReached(time, glint.phaseTimes[GlintController.Phase.READY]))
				{
					this.ChangePhase(glint, GlintController.Phase.FREE, time, curr);
				}
				else if (glint.phase < GlintController.Phase.READY && TimeUtil.HasReached(time, glint.phaseTimes[GlintController.Phase.SUSPENDED]))
				{
					this.ChangePhase(glint, GlintController.Phase.READY, time, curr);
				}
			}
			if (TimeUtil.HasReached(time, this.nextSpawnTime))
			{
				this.nextSpawnTime = TimeDirector.HoursFromTime(GlintController.AdjustTime(0.5f, curr), time);
				this.glints.Add(this.ChangePhase(new GlintController.Glint(), GlintController.Phase.SUSPENDED, time, curr));
			}
		}
		this.previousUpdateTime = time;
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x00050D24 File Offset: 0x0004EF24
	private GlintController.Glint ChangePhase(GlintController.Glint instance, GlintController.Phase phase, double currentTime, float agitation)
	{
		instance.phase = phase;
		if (instance.gameObject != null)
		{
			GameObject gameObject = this.Instantiate(phase, instance.gameObject.transform.position, instance.gameObject.transform.rotation);
			SRBehaviour.RequestDestroy(instance.gameObject, "GlintDestroyer.ChangePhase");
			instance.gameObject = gameObject;
		}
		else
		{
			Vector3 vector = UnityEngine.Random.insideUnitSphere * GlintController.SPAWN_RADIUS.Lerp(agitation);
			vector.y = Mathf.Abs(vector.y);
			instance.gameObject = this.Instantiate(phase, base.transform.position + vector, Quaternion.identity);
		}
		instance.phaseTimes = new Dictionary<GlintController.Phase, double>();
		for (GlintController.Phase phase2 = phase; phase2 <= GlintController.Phase.FREE; phase2++)
		{
			currentTime = (instance.phaseTimes[phase2] = TimeDirector.HoursFromTime((phase2 == GlintController.Phase.SUSPENDED) ? GlintController.AdjustTime(1f, agitation) : ((phase2 == GlintController.Phase.READY) ? GlintController.AdjustTime(0.5f, agitation) : 0.5f), currentTime));
		}
		return instance;
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x00050E2C File Offset: 0x0004F02C
	private void Initialize(GlintController.Phase phase, float phaseHours, float agitation)
	{
		int num = Mathf.RoundToInt(phaseHours / GlintController.AdjustTime(0.5f, agitation));
		for (int i = 0; i < num; i++)
		{
			this.glints.Add(this.ChangePhase(new GlintController.Glint(), phase, this.timeDirector.WorldTime() - (double)(Randoms.SHARED.GetFloat(phaseHours) * 3600f), agitation));
		}
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x00050E90 File Offset: 0x0004F090
	private GameObject Instantiate(GlintController.Phase phase, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = SRBehaviour.InstantiatePooledDynamic((phase == GlintController.Phase.SUSPENDED) ? this.suspendedGlintPrefab : ((phase == GlintController.Phase.READY) ? this.readyGlintPrefab : ((phase == GlintController.Phase.FREE) ? this.freeGlintPrefab : null)), position, rotation);
		Recycler component = gameObject.GetComponent<Recycler>();
		component.pool = SRSingleton<SceneContext>.Instance.fxPool;
		component.OnBeforeRecycle = (Recycler.RecycleEvent)Delegate.Combine(component.OnBeforeRecycle, new Recycler.RecycleEvent(this.OnBeforeRecycle));
		return gameObject;
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x00050F00 File Offset: 0x0004F100
	private void OnBeforeRecycle(GameObject gameObject)
	{
		Recycler component = gameObject.GetComponent<Recycler>();
		component.OnBeforeRecycle = (Recycler.RecycleEvent)Delegate.Remove(component.OnBeforeRecycle, new Recycler.RecycleEvent(this.OnBeforeRecycle));
		this.glints.First((GlintController.Glint g) => g.gameObject == gameObject).gameObject = null;
	}

	// Token: 0x04001393 RID: 5011
	private static readonly Vector2 SPAWN_RADIUS = new Vector2(7.5f, 30f);

	// Token: 0x04001394 RID: 5012
	private const float UPDATE_PERIOD = 0.16666667f;

	// Token: 0x04001395 RID: 5013
	private const float TIME_SPAWN = 0.5f;

	// Token: 0x04001396 RID: 5014
	private const float TIME_PHASE_BASE = 1f;

	// Token: 0x04001397 RID: 5015
	private const float TIME_PHASE_READY = 0.5f;

	// Token: 0x04001398 RID: 5016
	private const float TIME_PHASE_FREE = 0.5f;

	// Token: 0x04001399 RID: 5017
	private const float FAST_FORWARD_MIN_SECONDS = 7200f;

	// Token: 0x0400139A RID: 5018
	private GameObject suspendedGlintPrefab;

	// Token: 0x0400139B RID: 5019
	private GameObject readyGlintPrefab;

	// Token: 0x0400139C RID: 5020
	private GameObject freeGlintPrefab;

	// Token: 0x0400139D RID: 5021
	private double nextUpdateTime;

	// Token: 0x0400139E RID: 5022
	private double nextSpawnTime;

	// Token: 0x0400139F RID: 5023
	private double previousUpdateTime;

	// Token: 0x040013A0 RID: 5024
	private TimeDirector timeDirector;

	// Token: 0x040013A1 RID: 5025
	private SlimeEmotions emotions;

	// Token: 0x040013A2 RID: 5026
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x040013A3 RID: 5027
	private List<GlintController.Glint> glints = new List<GlintController.Glint>();

	// Token: 0x020003E3 RID: 995
	private enum Phase
	{
		// Token: 0x040013A5 RID: 5029
		SUSPENDED,
		// Token: 0x040013A6 RID: 5030
		READY,
		// Token: 0x040013A7 RID: 5031
		FREE
	}

	// Token: 0x020003E4 RID: 996
	private class Glint
	{
		// Token: 0x040013A8 RID: 5032
		public GameObject gameObject;

		// Token: 0x040013A9 RID: 5033
		public Dictionary<GlintController.Phase, double> phaseTimes;

		// Token: 0x040013AA RID: 5034
		public GlintController.Phase phase;
	}
}
