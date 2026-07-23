using System;
using System.Collections;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006FA RID: 1786
public class Extractor : SRBehaviour, GadgetInteractor, GadgetModel.Participant
{
	// Token: 0x06002535 RID: 9525 RVA: 0x0008ED68 File Offset: 0x0008CF68
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.anim = base.GetComponent<Animator>();
		this.zoneDirector = base.GetComponentInParent<ZoneDirector>();
		this.extractedAnimId = Animator.StringToHash("extracted");
		this.ejectAnimId = Animator.StringToHash("eject");
		this.readyToDespawnAnimId = Animator.StringToHash("readyToDespawn");
		this.skipDeployAnimId = Animator.StringToHash("skipDeploy");
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x0008EE08 File Offset: 0x0008D008
	public void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		}
	}

	// Token: 0x06002537 RID: 9527 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(GadgetModel model)
	{
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x0008EE34 File Offset: 0x0008D034
	public void SetModel(GadgetModel model)
	{
		this.model = (ExtractorModel)model;
		if (this.model.cyclesRemaining == 0 && this.model.cycleEndTime == 0.0)
		{
			this.model.cyclesRemaining = this.cycles;
			this.StartNewCycleOrDestroy();
			return;
		}
		this.anim.SetTrigger(this.skipDeployAnimId);
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x0008EE99 File Offset: 0x0008D099
	public void OnBundlesAvailable(MessageDirector msgDir)
	{
		this.uiBundle = msgDir.GetBundle("ui");
	}

	// Token: 0x0600253A RID: 9530 RVA: 0x0008EEAC File Offset: 0x0008D0AC
	public void Start()
	{
		foreach (Extractor.ProduceEntry produceEntry in this.produces)
		{
			if (!produceEntry.restrictZone || this.IsInZone(produceEntry.zone))
			{
				this.spawnWeights[produceEntry.id] = produceEntry.weight;
				this.spawnFX[produceEntry.id] = produceEntry.spawnFX;
			}
		}
	}

	// Token: 0x0600253B RID: 9531 RVA: 0x0008EF16 File Offset: 0x0008D116
	public void OnDisable()
	{
		if (this.isPendingDestroy)
		{
			this.DestroyFromGadgetSite(true);
		}
	}

	// Token: 0x0600253C RID: 9532 RVA: 0x0008EF28 File Offset: 0x0008D128
	public void Update()
	{
		if (this.model.queuedToProduce > 0)
		{
			if (this.timeDir.HasReached(this.model.nextProduceTime))
			{
				this.anim.SetTrigger(this.ejectAnimId);
				this.SpawnItem();
				this.model.queuedToProduce--;
				if (this.model.queuedToProduce <= 0)
				{
					this.model.nextProduceTime = 0.0;
					this.StartNewCycleOrDestroy();
				}
				else
				{
					this.model.nextProduceTime = this.timeDir.HoursFromNow(0.004166667f);
				}
			}
		}
		else if (this.timeDir.HasReached(this.model.cycleEndTime) && !this.anim.GetBool(this.extractedAnimId))
		{
			this.anim.SetBool(this.extractedAnimId, true);
		}
		if (this.model.cycleEndTime == double.PositiveInfinity || this.timeDir.HasReached(this.model.cycleEndTime))
		{
			this.countdownText.text = this.uiBundle.Get("m.ready");
			return;
		}
		this.countdownText.text = this.timeDir.FormatTime((int)(this.timeDir.HoursUntil(this.model.cycleEndTime) * 60.0));
	}

	// Token: 0x0600253D RID: 9533 RVA: 0x0008F090 File Offset: 0x0008D290
	public void OnInteract()
	{
		if (this.timeDir.HasReached(this.model.cycleEndTime))
		{
			this.model.queuedToProduce = Randoms.SHARED.GetInRange(this.produceMin, this.produceMax + 1);
			this.model.nextProduceTime = this.timeDir.WorldTime();
			this.model.cycleEndTime = double.PositiveInfinity;
		}
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x0008F102 File Offset: 0x0008D302
	public bool CanInteract()
	{
		return this.timeDir.HasReached(this.model.cycleEndTime);
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x0008F11C File Offset: 0x0008D31C
	private void StartNewCycleOrDestroy()
	{
		this.anim.SetBool(this.extractedAnimId, false);
		if (this.model.cyclesRemaining <= 0)
		{
			base.StartCoroutine(this.AnimAndDestroy());
			return;
		}
		if (!this.infiniteCycles)
		{
			this.model.cyclesRemaining--;
		}
		this.model.cycleEndTime = this.timeDir.HoursFromNow(this.hoursPerCycle);
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x0008F18E File Offset: 0x0008D38E
	private IEnumerator AnimAndDestroy()
	{
		this.isPendingDestroy = true;
		this.anim.SetBool(this.readyToDespawnAnimId, true);
		yield return new WaitForSeconds(1.5f);
		this.isPendingDestroy = false;
		this.DestroyFromGadgetSite(false);
		yield break;
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x0008F1A0 File Offset: 0x0008D3A0
	private void DestroyFromGadgetSite(bool includeInactive)
	{
		GadgetSite componentInParent = base.GetComponentInParent<GadgetSite>(includeInactive);
		if (componentInParent != null)
		{
			componentInParent.DestroyAttached();
		}
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x0008F1C4 File Offset: 0x0008D3C4
	private void SpawnItem()
	{
		Identifiable.Id id = Randoms.SHARED.Pick<Identifiable.Id>(this.spawnWeights, Identifiable.Id.NONE);
		if (id != Identifiable.Id.NONE)
		{
			Rigidbody component = SRBehaviour.InstantiateActor(this.lookupDir.GetPrefab(id), this.zoneDirector.regionSetId, this.spawnPoint.position, this.spawnPoint.rotation, false).GetComponent<Rigidbody>();
			if (component != null)
			{
				float num = Identifiable.IsEcho(id) ? 20f : 1f;
				component.AddForce(this.spawnPoint.forward * 50f + new Vector3(Randoms.SHARED.GetInRange(-num, num), Randoms.SHARED.GetInRange(-num, num), Randoms.SHARED.GetInRange(-num, num)));
			}
			GameObject gameObject = this.spawnFX.Get(id);
			if (gameObject != null)
			{
				SRBehaviour.SpawnAndPlayFX(gameObject, this.spawnPoint.position, this.spawnPoint.rotation);
			}
		}
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x0008F2BF File Offset: 0x0008D4BF
	private bool IsInZone(ZoneDirector.Zone zone)
	{
		return this.zoneDirector.zone == zone;
	}

	// Token: 0x04002412 RID: 9234
	[Tooltip("All the items the extractor can produce and their weights")]
	public Extractor.ProduceEntry[] produces;

	// Token: 0x04002413 RID: 9235
	public int cycles = 1;

	// Token: 0x04002414 RID: 9236
	public bool infiniteCycles;

	// Token: 0x04002415 RID: 9237
	public int produceMin = 4;

	// Token: 0x04002416 RID: 9238
	public int produceMax = 8;

	// Token: 0x04002417 RID: 9239
	public float hoursPerCycle = 22f;

	// Token: 0x04002418 RID: 9240
	public Transform spawnPoint;

	// Token: 0x04002419 RID: 9241
	public Text countdownText;

	// Token: 0x0400241A RID: 9242
	private TimeDirector timeDir;

	// Token: 0x0400241B RID: 9243
	private LookupDirector lookupDir;

	// Token: 0x0400241C RID: 9244
	private ZoneDirector zoneDirector;

	// Token: 0x0400241D RID: 9245
	private Dictionary<Identifiable.Id, float> spawnWeights = new Dictionary<Identifiable.Id, float>(Identifiable.idComparer);

	// Token: 0x0400241E RID: 9246
	private Dictionary<Identifiable.Id, GameObject> spawnFX = new Dictionary<Identifiable.Id, GameObject>(Identifiable.idComparer);

	// Token: 0x0400241F RID: 9247
	private Animator anim;

	// Token: 0x04002420 RID: 9248
	private int extractedAnimId;

	// Token: 0x04002421 RID: 9249
	private int ejectAnimId;

	// Token: 0x04002422 RID: 9250
	private int readyToDespawnAnimId;

	// Token: 0x04002423 RID: 9251
	private int skipDeployAnimId;

	// Token: 0x04002424 RID: 9252
	private bool isPendingDestroy;

	// Token: 0x04002425 RID: 9253
	private MessageBundle uiBundle;

	// Token: 0x04002426 RID: 9254
	private ExtractorModel model;

	// Token: 0x04002427 RID: 9255
	private const float SPAWN_EJECT_FORCE = 50f;

	// Token: 0x04002428 RID: 9256
	private const float PRODUCE_GAP = 0.25f;

	// Token: 0x04002429 RID: 9257
	private const float DESTROY_DELAY = 1.5f;

	// Token: 0x020006FB RID: 1787
	[Serializable]
	public class ProduceEntry
	{
		// Token: 0x0400242A RID: 9258
		public Identifiable.Id id;

		// Token: 0x0400242B RID: 9259
		public float weight;

		// Token: 0x0400242C RID: 9260
		public bool restrictZone;

		// Token: 0x0400242D RID: 9261
		public ZoneDirector.Zone zone;

		// Token: 0x0400242E RID: 9262
		public GameObject spawnFX;
	}
}
