using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using RichPresence;
using UnityEngine;

// Token: 0x02000380 RID: 896
public class SceneContext : SRSingleton<SceneContext>
{
	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06001277 RID: 4727 RVA: 0x00049461 File Offset: 0x00047661
	// (set) Token: 0x06001278 RID: 4728 RVA: 0x00049469 File Offset: 0x00047669
	public PlayerState PlayerState { get; private set; }

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06001279 RID: 4729 RVA: 0x00049472 File Offset: 0x00047672
	// (set) Token: 0x0600127A RID: 4730 RVA: 0x0004947A File Offset: 0x0004767A
	public EconomyDirector EconomyDirector { get; private set; }

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x0600127B RID: 4731 RVA: 0x00049483 File Offset: 0x00047683
	// (set) Token: 0x0600127C RID: 4732 RVA: 0x0004948B File Offset: 0x0004768B
	public ExchangeDirector ExchangeDirector { get; private set; }

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x0600127D RID: 4733 RVA: 0x00049494 File Offset: 0x00047694
	// (set) Token: 0x0600127E RID: 4734 RVA: 0x0004949C File Offset: 0x0004769C
	public PediaDirector PediaDirector { get; private set; }

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x0600127F RID: 4735 RVA: 0x000494A5 File Offset: 0x000476A5
	// (set) Token: 0x06001280 RID: 4736 RVA: 0x000494AD File Offset: 0x000476AD
	public TutorialDirector TutorialDirector { get; private set; }

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06001281 RID: 4737 RVA: 0x000494B6 File Offset: 0x000476B6
	// (set) Token: 0x06001282 RID: 4738 RVA: 0x000494BE File Offset: 0x000476BE
	public MailDirector MailDirector { get; private set; }

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x06001283 RID: 4739 RVA: 0x000494C7 File Offset: 0x000476C7
	// (set) Token: 0x06001284 RID: 4740 RVA: 0x000494CF File Offset: 0x000476CF
	public ModDirector ModDirector { get; private set; }

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06001285 RID: 4741 RVA: 0x000494D8 File Offset: 0x000476D8
	// (set) Token: 0x06001286 RID: 4742 RVA: 0x000494E0 File Offset: 0x000476E0
	public ProgressDirector ProgressDirector { get; private set; }

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06001287 RID: 4743 RVA: 0x000494E9 File Offset: 0x000476E9
	// (set) Token: 0x06001288 RID: 4744 RVA: 0x000494F1 File Offset: 0x000476F1
	public AchievementsDirector AchievementsDirector { get; private set; }

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06001289 RID: 4745 RVA: 0x000494FA File Offset: 0x000476FA
	// (set) Token: 0x0600128A RID: 4746 RVA: 0x00049502 File Offset: 0x00047702
	public TimeDirector TimeDirector { get; private set; }

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x0600128B RID: 4747 RVA: 0x0004950B File Offset: 0x0004770B
	// (set) Token: 0x0600128C RID: 4748 RVA: 0x00049513 File Offset: 0x00047713
	public AmbianceDirector AmbianceDirector { get; private set; }

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x0600128D RID: 4749 RVA: 0x0004951C File Offset: 0x0004771C
	// (set) Token: 0x0600128E RID: 4750 RVA: 0x00049524 File Offset: 0x00047724
	public GadgetDirector GadgetDirector { get; private set; }

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x0600128F RID: 4751 RVA: 0x0004952D File Offset: 0x0004772D
	// (set) Token: 0x06001290 RID: 4752 RVA: 0x00049535 File Offset: 0x00047735
	public PopupDirector PopupDirector { get; private set; }

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06001291 RID: 4753 RVA: 0x0004953E File Offset: 0x0004773E
	// (set) Token: 0x06001292 RID: 4754 RVA: 0x00049546 File Offset: 0x00047746
	public TeleportNetwork TeleportNetwork { get; set; }

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06001293 RID: 4755 RVA: 0x0004954F File Offset: 0x0004774F
	// (set) Token: 0x06001294 RID: 4756 RVA: 0x00049557 File Offset: 0x00047757
	public SceneParticleDirector SceneParticleDirector { get; set; }

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06001295 RID: 4757 RVA: 0x00049560 File Offset: 0x00047760
	// (set) Token: 0x06001296 RID: 4758 RVA: 0x00049568 File Offset: 0x00047768
	public MetadataDirector MetadataDirector { get; private set; }

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06001297 RID: 4759 RVA: 0x00049571 File Offset: 0x00047771
	// (set) Token: 0x06001298 RID: 4760 RVA: 0x00049579 File Offset: 0x00047779
	public ActorRegistry ActorRegistry { get; private set; }

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06001299 RID: 4761 RVA: 0x00049582 File Offset: 0x00047782
	// (set) Token: 0x0600129A RID: 4762 RVA: 0x0004958A File Offset: 0x0004778A
	public RegionRegistry RegionRegistry { get; private set; }

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x0600129B RID: 4763 RVA: 0x00049593 File Offset: 0x00047793
	// (set) Token: 0x0600129C RID: 4764 RVA: 0x0004959B File Offset: 0x0004779B
	public GameModel GameModel { get; private set; }

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x0600129D RID: 4765 RVA: 0x00025E60 File Offset: 0x00024060
	public SECTRDirector SECTRDirector
	{
		get
		{
			return null;
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x0600129E RID: 4766 RVA: 0x000495A4 File Offset: 0x000477A4
	// (set) Token: 0x0600129F RID: 4767 RVA: 0x000495AC File Offset: 0x000477AC
	public RanchDirector RanchDirector { get; private set; }

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060012A0 RID: 4768 RVA: 0x000495B5 File Offset: 0x000477B5
	// (set) Token: 0x060012A1 RID: 4769 RVA: 0x000495BD File Offset: 0x000477BD
	public HolidayDirector HolidayDirector { get; private set; }

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x060012A2 RID: 4770 RVA: 0x000495C6 File Offset: 0x000477C6
	// (set) Token: 0x060012A3 RID: 4771 RVA: 0x000495CE File Offset: 0x000477CE
	public InstrumentDirector InstrumentDirector { get; private set; }

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x060012A4 RID: 4772 RVA: 0x000495D7 File Offset: 0x000477D7
	// (set) Token: 0x060012A5 RID: 4773 RVA: 0x000495DF File Offset: 0x000477DF
	public GameModeConfig GameModeConfig { get; private set; }

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x060012A6 RID: 4774 RVA: 0x000495E8 File Offset: 0x000477E8
	// (set) Token: 0x060012A7 RID: 4775 RVA: 0x000495F0 File Offset: 0x000477F0
	public GameObject Player
	{
		get
		{
			return this.player;
		}
		set
		{
			this.player = value;
			SRSingleton<GameContext>.Instance.InputDirector.NoteNewPlayer(this.player);
			this.PlayerZoneTracker = ((this.player != null) ? this.player.GetComponent<PlayerZoneTracker>() : null);
		}
	}

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x060012A8 RID: 4776 RVA: 0x00049630 File Offset: 0x00047830
	// (set) Token: 0x060012A9 RID: 4777 RVA: 0x00049638 File Offset: 0x00047838
	public PlayerZoneTracker PlayerZoneTracker { get; private set; }

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x060012AA RID: 4778 RVA: 0x00049644 File Offset: 0x00047844
	// (remove) Token: 0x060012AB RID: 4779 RVA: 0x0004967C File Offset: 0x0004787C
	public event SceneContext.OnSessionEnded_Delegate onSessionEnded;

	// Token: 0x060012AC RID: 4780 RVA: 0x000496B4 File Offset: 0x000478B4
	public override void Awake()
	{
		base.Awake();
		this.PlayerState = base.GetComponent<PlayerState>();
		this.EconomyDirector = base.GetComponent<EconomyDirector>();
		this.ExchangeDirector = base.GetComponent<ExchangeDirector>();
		this.PediaDirector = base.GetComponent<PediaDirector>();
		this.TutorialDirector = base.GetComponent<TutorialDirector>();
		this.MailDirector = base.GetComponent<MailDirector>();
		this.ModDirector = base.GetComponent<ModDirector>();
		this.ProgressDirector = base.GetComponent<ProgressDirector>();
		this.AchievementsDirector = base.GetComponent<AchievementsDirector>();
		this.TimeDirector = base.GetComponent<TimeDirector>();
		this.AmbianceDirector = base.GetComponent<AmbianceDirector>();
		this.GadgetDirector = base.GetComponent<GadgetDirector>();
		this.PopupDirector = base.GetComponent<PopupDirector>();
		this.ActorRegistry = base.GetComponent<ActorRegistry>();
		this.RanchDirector = base.GetComponent<RanchDirector>();
		this.HolidayDirector = base.GetComponent<HolidayDirector>();
		this.InstrumentDirector = base.GetComponent<InstrumentDirector>();
		this.SceneParticleDirector = base.GetComponent<SceneParticleDirector>();
		this.MetadataDirector = base.GetComponent<MetadataDirector>();
		this.RegionRegistry = base.GetComponent<RegionRegistry>();
		this.TeleportNetwork = base.GetComponent<TeleportNetwork>();
		this.GameModel = base.GetComponent<GameModel>();
		this.GameModeConfig = base.GetComponent<GameModeConfig>();
		this.PreloadObjectPoolOnState(this.fxPool, "fxPool", ObjectPoolConfig.StartupPoolMode.Awake);
		this.PreloadObjectPoolOnState(this.appearanceObjectPool, "appearanceObjectPool", ObjectPoolConfig.StartupPoolMode.Awake);
		if (SceneContext.onNextSceneAwake != null)
		{
			SceneContext.onNextSceneAwake(this);
			SceneContext.onNextSceneAwake = null;
		}
		this.onSessionEnded += delegate()
		{
			AnalyticsUtil.CustomEvent("SessionEnded", null, true);
		};
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x0004983C File Offset: 0x00047A3C
	private void PreloadObjectPoolOnState(ObjectPool pool, string poolName, ObjectPoolConfig.StartupPoolMode startupPoolMode)
	{
		if (pool.config.startupPoolMode == startupPoolMode)
		{
			pool.CreateStartupPools();
		}
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x00049854 File Offset: 0x00047A54
	public void Start()
	{
		if (SceneContext.beforeSceneLoaded != null)
		{
			try
			{
				SceneContext.beforeSceneLoaded(this);
			}
			finally
			{
				SceneContext.beforeSceneLoaded = null;
			}
		}
		this.GameModeConfig.InitForLevel();
		this.TimeDirector.InitForLevel();
		this.PediaDirector.InitForLevel();
		this.AchievementsDirector.InitForLevel();
		this.PlayerState.InitForLevel();
		this.EconomyDirector.InitForLevel();
		this.ExchangeDirector.InitForLevel();
		this.TutorialDirector.InitForLevel();
		this.MailDirector.InitForLevel();
		this.ModDirector.InitForLevel();
		this.ProgressDirector.InitForLevel();
		this.AmbianceDirector.InitForLevel();
		this.GadgetDirector.InitForLevel();
		this.PopupDirector.InitForLevel();
		this.SceneParticleDirector.InitForLevel();
		this.HolidayDirector.InitForLevel();
		this.SlimeAppearanceDirector.InitForLevel();
		this.InstrumentDirector.InitForLevel();
		this.RanchDirector.InitForLevel();
		if (this.Player != null)
		{
			this.Player.GetComponent<FirestormActivator>().InitForLevel();
		}
		if (Levels.isMainMenu())
		{
			SRSingleton<GameContext>.Instance.RichPresenceDirector.SetRichPresence(default(MainMenuData));
		}
		this.PreloadObjectPoolOnState(this.fxPool, "fxPool", ObjectPoolConfig.StartupPoolMode.Start);
		this.PreloadObjectPoolOnState(this.appearanceObjectPool, "appearanceObjectPool", ObjectPoolConfig.StartupPoolMode.Start);
		SRSingleton<GameContext>.Instance.DLCDirector.InitForLevel();
		if (!this.GameModel.expectingPush)
		{
			this.NoteGameFullyLoaded();
		}
		SRSingleton<GameContext>.Instance.AutoSaveDirector.OnSceneLoaded();
		if (SceneContext.onSceneLoaded != null)
		{
			try
			{
				SceneContext.onSceneLoaded(this);
			}
			finally
			{
				SceneContext.onSceneLoaded = null;
			}
		}
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x00049A18 File Offset: 0x00047C18
	public void LateUpdate()
	{
		SlimeEat.ClearClaimedFood();
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x00049A1F File Offset: 0x00047C1F
	public void NoteGameFullyLoaded()
	{
		this.ProgressDirector.GameFullyLoaded();
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x00049A2C File Offset: 0x00047C2C
	public void OnApplicationQuit()
	{
		this.OnSessionEnded();
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x00049A34 File Offset: 0x00047C34
	public void OnSessionEnded()
	{
		if (!Levels.isSpecial() && this.onSessionEnded != null)
		{
			this.onSessionEnded();
			this.onSessionEnded = null;
		}
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x00049A57 File Offset: 0x00047C57
	public void Register(WakeUpDestination destination)
	{
		this.wakeUpDestinations[destination.deathRegionSetId] = destination;
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x00049A6B File Offset: 0x00047C6B
	public void Deregister(WakeUpDestination destination)
	{
		this.wakeUpDestinations.Remove(destination.deathRegionSetId);
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x00049A7F File Offset: 0x00047C7F
	public WakeUpDestination GetWakeUpDestination(RegionMember member)
	{
		if (this.wakeUpDestinations.ContainsKey(member.setId))
		{
			return this.wakeUpDestinations[member.setId];
		}
		return this.GetWakeUpDestination();
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x00049AAC File Offset: 0x00047CAC
	public WakeUpDestination GetWakeUpDestination()
	{
		return this.wakeUpDestinations[RegionRegistry.RegionSetId.HOME];
	}

	// Token: 0x040011A4 RID: 4516
	public static SceneContext.SceneLoadDelegate beforeSceneLoaded;

	// Token: 0x040011A5 RID: 4517
	public static SceneContext.SceneLoadDelegate onSceneLoaded;

	// Token: 0x040011A6 RID: 4518
	public static SceneContext.SceneLoadDelegate onNextSceneAwake;

	// Token: 0x040011BE RID: 4542
	private GameObject player;

	// Token: 0x040011C0 RID: 4544
	private Dictionary<RegionRegistry.RegionSetId, WakeUpDestination> wakeUpDestinations = new Dictionary<RegionRegistry.RegionSetId, WakeUpDestination>(RegionRegistry.RegionSetIdComparer.Instance);

	// Token: 0x040011C1 RID: 4545
	public ObjectPool fxPool;

	// Token: 0x040011C2 RID: 4546
	public ObjectPool appearanceObjectPool;

	// Token: 0x040011C3 RID: 4547
	public SlimeAppearanceDirector SlimeAppearanceDirector;

	// Token: 0x02000381 RID: 897
	// (Invoke) Token: 0x060012B9 RID: 4793
	public delegate void SceneLoadDelegate(SceneContext ctx);

	// Token: 0x02000382 RID: 898
	// (Invoke) Token: 0x060012BD RID: 4797
	public delegate void OnSessionEnded_Delegate();
}
