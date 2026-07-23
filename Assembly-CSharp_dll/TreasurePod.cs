using System;
using System.Collections;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020007A4 RID: 1956
public class TreasurePod : IdHandler, TreasurePodModel.Participant
{
	// Token: 0x060028F0 RID: 10480 RVA: 0x0009AC00 File Offset: 0x00098E00
	public void Awake()
	{
		this.cellDirector = base.GetComponentInParent<CellDirector>();
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPod(base.id, base.gameObject);
		this.animOpenId = Animator.StringToHash("Open");
		this.animOpenImmediateId = Animator.StringToHash("OpenImmediate");
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.gadgetDir = SRSingleton<SceneContext>.Instance.GadgetDirector;
		this.slimeAppearanceDirector = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector;
	}

	// Token: 0x060028F1 RID: 10481 RVA: 0x0009AC84 File Offset: 0x00098E84
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterPod(base.id);
		}
	}

	// Token: 0x060028F2 RID: 10482 RVA: 0x0009ACA8 File Offset: 0x00098EA8
	public void OnEnable()
	{
		this.forceUpdate = true;
		this.nextUpdateImmediate = true;
		base.StartCoroutine(this.OnEnableCoroutine());
	}

	// Token: 0x060028F3 RID: 10483 RVA: 0x0009ACC5 File Offset: 0x00098EC5
	private IEnumerator OnEnableCoroutine()
	{
		yield return new WaitForSeconds(TreasurePod.OPEN_DELAY);
		yield return base.StartCoroutine(this.SpawnQueuedPrizeObjs());
		yield break;
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(TreasurePodModel model)
	{
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x0009ACD4 File Offset: 0x00098ED4
	public void SetModel(TreasurePodModel model)
	{
		this.model = model;
		this.UpdateImmediate(model.state);
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x060028F6 RID: 10486 RVA: 0x0009ACE9 File Offset: 0x00098EE9
	// (set) Token: 0x060028F7 RID: 10487 RVA: 0x0009ACF6 File Offset: 0x00098EF6
	public TreasurePod.State CurrState
	{
		get
		{
			return this.model.state;
		}
		set
		{
			this.model.state = value;
			this.forceUpdate = true;
		}
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x0009AD0B File Offset: 0x00098F0B
	public ZoneDirector.Zone GetZoneId()
	{
		if (this.cellDirector != null)
		{
			return this.cellDirector.GetZoneId();
		}
		return ZoneDirector.Zone.NONE;
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x0009AD28 File Offset: 0x00098F28
	private void UpdateImmediate(TreasurePod.State state)
	{
		this.nextUpdateImmediate = true;
		this.CurrState = state;
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x0009AD38 File Offset: 0x00098F38
	public void Update()
	{
		if (this.forceUpdate)
		{
			this.forceUpdate = false;
			if (this.nextUpdateImmediate)
			{
				if (this.model.state == TreasurePod.State.OPEN && this.model.spawnQueue.Count == 0)
				{
					base.GetComponentInChildren<Animator>().SetTrigger(this.animOpenImmediateId);
				}
				this.nextUpdateImmediate = false;
			}
			base.GetComponentInChildren<Animator>().SetBool(this.animOpenId, this.model.state == TreasurePod.State.OPEN);
		}
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x0009ADB3 File Offset: 0x00098FB3
	protected override string IdPrefix()
	{
		return "pod";
	}

	// Token: 0x060028FC RID: 10492 RVA: 0x0009ADBA File Offset: 0x00098FBA
	public bool HasKey()
	{
		return !this.needsUpgrade || this.playerState.HasUpgrade(this.requiredUpgrade);
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x0009ADD7 File Offset: 0x00098FD7
	public bool HasAnyKey()
	{
		return this.playerState.HasUpgrade(PlayerState.Upgrade.TREASURE_CRACKER_1) || this.playerState.HasUpgrade(PlayerState.Upgrade.TREASURE_CRACKER_2) || this.playerState.HasUpgrade(PlayerState.Upgrade.TREASURE_CRACKER_3) || this.playerState.HasUpgrade(PlayerState.Upgrade.TREASURE_CRACKER_4);
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x0009AE18 File Offset: 0x00099018
	public void Activate()
	{
		if (this.HasKey())
		{
			this.CurrState = TreasurePod.State.OPEN;
			if (this.openCue != null)
			{
				SECTR_AudioSystem.Play(this.openCue, base.transform.position, false);
			}
			if (this.openFX != null)
			{
				SRBehaviour.InstantiateDynamic(this.openFX, base.transform.position, base.transform.rotation, false);
			}
			base.StartCoroutine((SRSingleton<SceneContext>.Instance.GameModel.currGameMode == PlayerState.GameMode.TIME_LIMIT_V2) ? this.AwardPrizesRushMode() : this.AwardPrizesDefault());
			AnalyticsUtil.CustomEvent("PodOpened", new Dictionary<string, object>
			{
				{
					"id",
					base.id
				}
			}, true);
		}
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x0009AED6 File Offset: 0x000990D6
	private IEnumerator AwardPrizesDefault()
	{
		if (this.blueprint != Gadget.Id.NONE)
		{
			this.gadgetDir.AddBlueprint(this.blueprint);
		}
		if (this.unlockedSlimeAppearance != null)
		{
			this.slimeAppearanceDirector.UnlockAppearance(this.unlockedSlimeAppearanceDefinition, this.unlockedSlimeAppearance);
		}
		if (this.spawnObjs != null && this.spawnObjs.Length != 0)
		{
			GameObject[] array = this.spawnObjs;
			for (int i = 0; i < array.Length; i++)
			{
				Identifiable.Id id = Identifiable.GetId(array[i]);
				this.model.spawnQueue.Enqueue(id);
			}
		}
		yield return new WaitForSeconds(TreasurePod.OPEN_DELAY);
		if (this.afterOpenFX != null)
		{
			SRBehaviour.InstantiateDynamic(this.afterOpenFX, base.transform.position, base.transform.rotation, false);
		}
		if (this.blueprint != Gadget.Id.NONE)
		{
			SRSingleton<SceneContext>.Instance.PopupDirector.QueueForPopup(new TreasurePod.BlueprintPopupCreator(this.blueprint));
			SRSingleton<SceneContext>.Instance.PopupDirector.MaybePopupNext();
			yield return new WaitForSeconds(TreasurePod.ITEM_GAP_DELAY);
		}
		if (this.unlockedSlimeAppearance != null)
		{
			this.slimeAppearanceDirector.UpdateChosenSlimeAppearance(this.unlockedSlimeAppearanceDefinition, this.unlockedSlimeAppearance);
			this.unlockedSlimeAppearance.MaybeShowPopupUI();
		}
		yield return base.StartCoroutine(this.SpawnQueuedPrizeObjs());
		yield break;
	}

	// Token: 0x06002900 RID: 10496 RVA: 0x0009AEE5 File Offset: 0x000990E5
	private IEnumerator AwardPrizesRushMode()
	{
		yield return new WaitForSeconds(TreasurePod.OPEN_DELAY);
		if (this.onCoinsFX != null)
		{
			SRBehaviour.InstantiateDynamic(this.onCoinsFX).transform.position = base.transform.TransformPoint(TreasurePod.EJECT_OFF);
		}
		SECTR_AudioSystem.Play(this.onCoinsCue, base.transform.position, false);
		SRBehaviour.InstantiateDynamic(this.coins, base.transform.position, Quaternion.identity, false);
		yield break;
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x0009AEF4 File Offset: 0x000990F4
	private IEnumerator SpawnQueuedPrizeObjs()
	{
		yield return new WaitForSeconds(TreasurePod.ITEM_GAP_DELAY);
		Vector3 ejectPos = base.transform.TransformPoint(TreasurePod.EJECT_OFF);
		while (this.model.spawnQueue.Count > 0)
		{
			Rigidbody component = SRBehaviour.InstantiateActor(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(this.model.spawnQueue.Dequeue()), this.cellDirector.region.setId, ejectPos, base.transform.rotation, false).GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddForce(base.transform.TransformDirection(TreasurePod.EJECT_VEL) + Randoms.SHARED.GetInRange(-0.1f, 0.1f) * base.transform.right, ForceMode.VelocityChange);
			}
			if (this.spawnObjCue != null)
			{
				SECTR_AudioSystem.Play(this.spawnObjCue, ejectPos, false);
			}
			yield return new WaitForSeconds(TreasurePod.ITEM_GAP_DELAY);
		}
		yield break;
	}

	// Token: 0x04002866 RID: 10342
	public bool needsUpgrade = true;

	// Token: 0x04002867 RID: 10343
	public PlayerState.Upgrade requiredUpgrade;

	// Token: 0x04002868 RID: 10344
	public SECTR_AudioCue openCue;

	// Token: 0x04002869 RID: 10345
	[Tooltip("The FX that will be played as soon as the treasure pod is unlocked.")]
	public GameObject openFX;

	// Token: 0x0400286A RID: 10346
	[Tooltip("The FX that will be played after the treasure pod opening animation has finished.")]
	public GameObject afterOpenFX;

	// Token: 0x0400286B RID: 10347
	public Gadget.Id blueprint;

	// Token: 0x0400286C RID: 10348
	public GameObject[] spawnObjs;

	// Token: 0x0400286D RID: 10349
	public SECTR_AudioCue spawnObjCue;

	// Token: 0x0400286E RID: 10350
	[Tooltip("The SlimeDefinition for the appearance that will be unlocked.")]
	public SlimeDefinition unlockedSlimeAppearanceDefinition;

	// Token: 0x0400286F RID: 10351
	[Tooltip("The SlimeAppearance that will be unlocked.")]
	public SlimeAppearance unlockedSlimeAppearance;

	// Token: 0x04002870 RID: 10352
	[Tooltip("Coins prefab awarded by the treasure pod. (TIME_LIMIT_V2)")]
	public GameObject coins;

	// Token: 0x04002871 RID: 10353
	[Tooltip("FX played when coins are awarded. (TIME_LIMIT_V2, optional)")]
	public GameObject onCoinsFX;

	// Token: 0x04002872 RID: 10354
	[Tooltip("SFX played when coins are awarded. (TIME_LIMIT_V2, optional)")]
	public SECTR_AudioCue onCoinsCue;

	// Token: 0x04002873 RID: 10355
	private CellDirector cellDirector;

	// Token: 0x04002874 RID: 10356
	private bool nextUpdateImmediate;

	// Token: 0x04002875 RID: 10357
	private bool forceUpdate;

	// Token: 0x04002876 RID: 10358
	private int animOpenId;

	// Token: 0x04002877 RID: 10359
	private int animOpenImmediateId;

	// Token: 0x04002878 RID: 10360
	private PlayerState playerState;

	// Token: 0x04002879 RID: 10361
	private GadgetDirector gadgetDir;

	// Token: 0x0400287A RID: 10362
	private SlimeAppearanceDirector slimeAppearanceDirector;

	// Token: 0x0400287B RID: 10363
	private TreasurePodModel model;

	// Token: 0x0400287C RID: 10364
	public static float ITEM_GAP_DELAY = 0.8f;

	// Token: 0x0400287D RID: 10365
	public static float OPEN_DELAY = 3.3f;

	// Token: 0x0400287E RID: 10366
	private static Vector3 EJECT_VEL = new Vector3(0f, 5f, 2.5f);

	// Token: 0x0400287F RID: 10367
	private static Vector3 EJECT_OFF = new Vector3(0f, 1.75f, 1f);

	// Token: 0x020007A5 RID: 1957
	public enum State
	{
		// Token: 0x04002881 RID: 10369
		LOCKED,
		// Token: 0x04002882 RID: 10370
		OPEN
	}

	// Token: 0x020007A6 RID: 1958
	private class BlueprintPopupCreator : PopupDirector.PopupCreator
	{
		// Token: 0x06002904 RID: 10500 RVA: 0x0009AF67 File Offset: 0x00099167
		public BlueprintPopupCreator(Gadget.Id id)
		{
			this.id = id;
		}

		// Token: 0x06002905 RID: 10501 RVA: 0x0009AF76 File Offset: 0x00099176
		public override void Create()
		{
			BlueprintPopupUI.CreateBlueprintPopup(SRSingleton<GameContext>.Instance.LookupDirector.GetGadgetDefinition(this.id));
		}

		// Token: 0x06002906 RID: 10502 RVA: 0x0009AF93 File Offset: 0x00099193
		public override bool Equals(object other)
		{
			return other is TreasurePod.BlueprintPopupCreator && ((TreasurePod.BlueprintPopupCreator)other).id == this.id;
		}

		// Token: 0x06002907 RID: 10503 RVA: 0x0009AFB2 File Offset: 0x000991B2
		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		// Token: 0x06002908 RID: 10504 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public override bool ShouldClear()
		{
			return false;
		}

		// Token: 0x04002883 RID: 10371
		private Gadget.Id id;
	}
}
