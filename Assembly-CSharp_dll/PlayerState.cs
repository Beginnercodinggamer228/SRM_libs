using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020002C0 RID: 704
public class PlayerState : SRBehaviour, PlayerModel.Participant
{
	// Token: 0x1400000F RID: 15
	// (add) Token: 0x06000EDD RID: 3805 RVA: 0x0003BD10 File Offset: 0x00039F10
	// (remove) Token: 0x06000EDE RID: 3806 RVA: 0x0003BD48 File Offset: 0x00039F48
	public event PlayerState.OnEndGame onEndGame = delegate()
	{
	};

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000EDF RID: 3807 RVA: 0x0003BD80 File Offset: 0x00039F80
	// (remove) Token: 0x06000EE0 RID: 3808 RVA: 0x0003BDB8 File Offset: 0x00039FB8
	public event PlayerState.OnEndGameTimeChanged onEndGameTimeChanged = delegate()
	{
	};

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000EE1 RID: 3809 RVA: 0x0003BDED File Offset: 0x00039FED
	// (set) Token: 0x06000EE2 RID: 3810 RVA: 0x0003BDF5 File Offset: 0x00039FF5
	public bool PointedAtVaccable { get; set; }

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x0003BDFE File Offset: 0x00039FFE
	// (set) Token: 0x06000EE4 RID: 3812 RVA: 0x0003BE06 File Offset: 0x0003A006
	public GameObject Targeting { get; set; }

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000EE5 RID: 3813 RVA: 0x0003BE0F File Offset: 0x0003A00F
	// (set) Token: 0x06000EE6 RID: 3814 RVA: 0x0003BE17 File Offset: 0x0003A017
	public bool InGadgetMode { get; set; }

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000EE7 RID: 3815 RVA: 0x0003BE20 File Offset: 0x0003A020
	public Ammo Ammo
	{
		get
		{
			return this.ammoDict[this.ammoMode];
		}
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x0003BE33 File Offset: 0x0003A033
	// (set) Token: 0x06000EE9 RID: 3817 RVA: 0x0003BE3B File Offset: 0x0003A03B
	public double nextAmmoLossDamageTime { get; set; }

	// Token: 0x06000EEA RID: 3818 RVA: 0x0003BE44 File Offset: 0x0003A044
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.mailDir = SRSingleton<SceneContext>.Instance.MailDirector;
		this.metadataDirector = SRSingleton<SceneContext>.Instance.MetadataDirector;
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0003BEA4 File Offset: 0x0003A0A4
	public void InitModel(PlayerModel model)
	{
		this.Reset(model);
		model.ammoDict[PlayerState.AmmoMode.DEFAULT] = new AmmoModel();
		model.ammoDict[PlayerState.AmmoMode.NIMBLE_VALLEY] = new AmmoModel();
		this.ammoDict[PlayerState.AmmoMode.DEFAULT].InitModel(model.ammoDict[PlayerState.AmmoMode.DEFAULT]);
		this.ammoDict[PlayerState.AmmoMode.NIMBLE_VALLEY].InitModel(model.ammoDict[PlayerState.AmmoMode.NIMBLE_VALLEY]);
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0003BF14 File Offset: 0x0003A114
	public void SetModel(PlayerModel model)
	{
		this.model = model;
		this.RegisteredPotentialAmmoChanged(this.model.registeredPotentialAmmo);
		this.ammoDict[PlayerState.AmmoMode.DEFAULT].SetModel(model.ammoDict[PlayerState.AmmoMode.DEFAULT]);
		this.ammoDict[PlayerState.AmmoMode.NIMBLE_VALLEY].SetModel(model.ammoDict[PlayerState.AmmoMode.NIMBLE_VALLEY]);
		this.CheckAllUpgradeLockers();
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0003BF7C File Offset: 0x0003A17C
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> registeredPotentialAmmo)
	{
		if (this.ammoDict != null && registeredPotentialAmmo != null)
		{
			using (Dictionary<PlayerState.AmmoMode, List<GameObject>>.Enumerator enumerator = registeredPotentialAmmo.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<PlayerState.AmmoMode, List<GameObject>> pair = enumerator.Current;
					pair.Value.ForEach(delegate(GameObject p)
					{
						this.ammoDict[pair.Key].RegisterPotentialAmmo(p);
					});
				}
			}
		}
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x0003BFFC File Offset: 0x0003A1FC
	public void KeyAdded()
	{
		SRSingleton<SceneContext>.Instance.PediaDirector.MaybeShowPopup(PediaDirector.Id.KEYS);
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x00003296 File Offset: 0x00001496
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x00003296 File Offset: 0x00001496
	public void TransformChanged(Vector3 pos, Quaternion rot)
	{
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x0003B1A9 File Offset: 0x000393A9
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x0003C012 File Offset: 0x0003A212
	public HashSet<Identifiable.Id> GetPotentialAmmo()
	{
		return new HashSet<Identifiable.Id>(from go in this.potentialAmmo
		select Identifiable.GetId(go), Identifiable.idComparer);
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x0003C048 File Offset: 0x0003A248
	private void Reset(PlayerModel model)
	{
		model.Reset(SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings());
		Dictionary<PlayerState.AmmoMode, Ammo> dictionary = new Dictionary<PlayerState.AmmoMode, Ammo>(PlayerState.AmmoModeComparer.Instance);
		dictionary.Add(PlayerState.AmmoMode.DEFAULT, new Ammo(this.GetPotentialAmmo(), 5, 4, PlayerState.PLAYER_AMMO_PREDS, new Func<Identifiable.Id, int, int>(this.GetMaxAmmo_Default)));
		Dictionary<PlayerState.AmmoMode, Ammo> dictionary2 = dictionary;
		PlayerState.AmmoMode key = PlayerState.AmmoMode.NIMBLE_VALLEY;
		HashSet<Identifiable.Id> hashSet = new HashSet<Identifiable.Id>(Identifiable.idComparer);
		hashSet.Add(Identifiable.Id.QUICKSILVER_PLORT);
		hashSet.Add(Identifiable.Id.VALLEY_AMMO_1);
		hashSet.Add(Identifiable.Id.VALLEY_AMMO_2);
		hashSet.Add(Identifiable.Id.VALLEY_AMMO_3);
		hashSet.Add(Identifiable.Id.VALLEY_AMMO_4);
		int numSlots = 3;
		int usableSlots = 3;
		Predicate<Identifiable.Id>[] array = new Predicate<Identifiable.Id>[3];
		array[0] = ((Identifiable.Id id) => id == Identifiable.Id.QUICKSILVER_PLORT);
		array[1] = ((Identifiable.Id id) => id == Identifiable.Id.VALLEY_AMMO_1);
		array[2] = ((Identifiable.Id id) => id == Identifiable.Id.VALLEY_AMMO_2 || id == Identifiable.Id.VALLEY_AMMO_3 || id == Identifiable.Id.VALLEY_AMMO_4);
		dictionary2.Add(key, new Ammo(hashSet, numSlots, usableSlots, array, new Func<Identifiable.Id, int, int>(this.GetMaxAmmo_NimbleValley)));
		this.ammoDict = dictionary;
		this.SetAmmoMode(PlayerState.AmmoMode.DEFAULT);
		this.InitUpgradeLocks(model);
		model.upgrades.Clear();
		this.InitZoneMaps(model);
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x0003C190 File Offset: 0x0003A390
	private int GetMaxAmmo_Default(Identifiable.Id id, int index)
	{
		if (id == Identifiable.Id.GLITCH_DEBUG_SPRAY_LIQUID)
		{
			return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.debugSprayMaxAmmo;
		}
		if (id == Identifiable.Id.GLITCH_SLIME || id == Identifiable.Id.GLITCH_BUG_REPORT)
		{
			return SRSingleton<SceneContext>.Instance.MetadataDirector.Glitch.slimeMaxAmmo;
		}
		return this.model.maxAmmo;
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x0003C1EA File Offset: 0x0003A3EA
	private int GetMaxAmmo_NimbleValley(Identifiable.Id id, int index)
	{
		switch (index)
		{
		case 0:
			return 250;
		case 1:
			return 100;
		case 2:
			return 3;
		default:
			throw new ArgumentException();
		}
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x0003C210 File Offset: 0x0003A410
	private void InitZoneMaps(PlayerModel model)
	{
		model.unlockedZoneMaps.Clear();
		model.unlockedZoneMaps.Add(ZoneDirector.Zone.RANCH);
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x0003C22A File Offset: 0x0003A42A
	public void UnlockMap(ZoneDirector.Zone zone)
	{
		this.model.unlockedZoneMaps.Add(zone);
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x0003C23E File Offset: 0x0003A43E
	public void LockAllMaps()
	{
		this.model.unlockedZoneMaps.Clear();
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0003C250 File Offset: 0x0003A450
	public void UnlockAllMaps()
	{
		this.model.unlockedZoneMaps.Add(ZoneDirector.Zone.MOSS);
		this.model.unlockedZoneMaps.Add(ZoneDirector.Zone.DESERT);
		this.model.unlockedZoneMaps.Add(ZoneDirector.Zone.QUARRY);
		this.model.unlockedZoneMaps.Add(ZoneDirector.Zone.REEF);
		this.model.unlockedZoneMaps.Add(ZoneDirector.Zone.RUINS);
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x0003C2B7 File Offset: 0x0003A4B7
	public bool HasUnlockedMap(ZoneDirector.Zone zone)
	{
		return this.model.unlockedZoneMaps.Contains(zone);
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x0003C2CC File Offset: 0x0003A4CC
	private void InitUpgradeLocks(PlayerModel model)
	{
		model.availUpgrades.Clear();
		model.availUpgrades.Add(PlayerState.Upgrade.HEALTH_1);
		model.availUpgrades.Add(PlayerState.Upgrade.ENERGY_1);
		model.availUpgrades.Add(PlayerState.Upgrade.AMMO_1);
		model.availUpgrades.Add(PlayerState.Upgrade.JETPACK);
		model.availUpgrades.Add(PlayerState.Upgrade.LIQUID_SLOT);
		model.upgradeLocks.Clear();
		model.upgradeLocks[PlayerState.Upgrade.RUN_EFFICIENCY] = this.CreateBasicLock(null, null, 48f);
		model.upgradeLocks[PlayerState.Upgrade.AIR_BURST] = this.CreateBasicLock(null, null, 72f);
		model.upgradeLocks[PlayerState.Upgrade.JETPACK_EFFICIENCY] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.JETPACK), null, 120f);
		model.upgradeLocks[PlayerState.Upgrade.HEALTH_2] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.HEALTH_1), null, 48f);
		model.upgradeLocks[PlayerState.Upgrade.HEALTH_3] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.HEALTH_2), null, 72f);
		model.upgradeLocks[PlayerState.Upgrade.ENERGY_2] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.ENERGY_1), null, 48f);
		model.upgradeLocks[PlayerState.Upgrade.ENERGY_3] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.ENERGY_2), null, 72f);
		model.upgradeLocks[PlayerState.Upgrade.AMMO_2] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.AMMO_1), null, 48f);
		model.upgradeLocks[PlayerState.Upgrade.AMMO_3] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.AMMO_2), null, 72f);
		model.upgradeLocks[PlayerState.Upgrade.TREASURE_CRACKER_1] = this.CreateBasicLock(null, () => this.achieveDir.GetGameIntStat(AchievementsDirector.GameIntStat.FABRICATED_GADGETS) >= 1, 5f);
		model.upgradeLocks[PlayerState.Upgrade.TREASURE_CRACKER_2] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.TREASURE_CRACKER_1), () => this.achieveDir.GetGameIntStat(AchievementsDirector.GameIntStat.FABRICATED_GADGETS) >= 20, 1f);
		model.upgradeLocks[PlayerState.Upgrade.TREASURE_CRACKER_3] = this.CreateBasicLock(new PlayerState.Upgrade?(PlayerState.Upgrade.TREASURE_CRACKER_2), () => this.achieveDir.GetGameIntStat(AchievementsDirector.GameIntStat.FABRICATED_GADGETS) >= 50, 1f);
		model.upgradeLocks[PlayerState.Upgrade.SPARE_KEY] = this.CreateBasicLock(null, () => this.mailDir.HasReadMail(new MailDirector.Mail(MailDirector.Type.PERSONAL, "casey_11")), 3f);
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0003C4F8 File Offset: 0x0003A6F8
	private void CheckAllUpgradeLockers()
	{
		foreach (KeyValuePair<PlayerState.Upgrade, PlayerState.UpgradeLocker> keyValuePair in this.model.upgradeLocks)
		{
			if (keyValuePair.Value.CheckUnlockCondition())
			{
				keyValuePair.Value.Unlock();
			}
		}
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x0003C564 File Offset: 0x0003A764
	private PlayerState.UpgradeLocker CreateBasicLock(PlayerState.Upgrade? waitForUpgrade, PlayerState.UnlockCondition extraCondition, float delayHrs)
	{
		return new PlayerState.UpgradeLocker(this, () => (waitForUpgrade == null || this.HasUpgrade(waitForUpgrade.Value)) && (extraCondition == null || extraCondition()), delayHrs);
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x0003C5A0 File Offset: 0x0003A7A0
	public double? GetEndGameTimeRemaining()
	{
		if (this.model.endGameTime != null)
		{
			double num = this.timeDir.WorldTime();
			double num2 = this.model.endGameTime.Value - num;
			return new double?((num2 > 0.0) ? num2 : 0.0);
		}
		return null;
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0003C605 File Offset: 0x0003A805
	public double? GetEndGameTime()
	{
		return this.model.endGameTime;
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x0003C612 File Offset: 0x0003A812
	public void SetEndGameTime(double time)
	{
		this.model.endGameTime = new double?(time);
		this.onEndGameTimeChanged();
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x0003C630 File Offset: 0x0003A830
	public bool IsGameOver()
	{
		return this.model.endGameTime != null && this.timeDir.HasReached(this.model.endGameTime.Value);
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x0003C664 File Offset: 0x0003A864
	public void Update()
	{
		this.model.Recover();
		List<PlayerState.Upgrade> list = new List<PlayerState.Upgrade>();
		foreach (KeyValuePair<PlayerState.Upgrade, PlayerState.UpgradeLocker> keyValuePair in this.model.upgradeLocks)
		{
			if (keyValuePair.Value.ReachedUnlockTime())
			{
				list.Add(keyValuePair.Key);
				if (!this.HasUpgrade(keyValuePair.Key) && !this.model.availUpgrades.Contains(keyValuePair.Key))
				{
					this.model.availUpgrades.Add(keyValuePair.Key);
					this.popupDir.QueueForPopup(new PlayerState.AvailUpgradePopupCreator(keyValuePair.Key));
					this.popupDir.MaybePopupNext();
				}
			}
		}
		foreach (PlayerState.Upgrade key in list)
		{
			this.model.upgradeLocks.Remove(key);
		}
		if (this.gameState == PlayerState.GameState.DEFAULT && this.IsGameOver())
		{
			this.gameState = PlayerState.GameState.GAME_OVER;
			this.onEndGame();
			UnityEngine.Object.Instantiate<GameObject>(SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().endGameUIPrefab);
		}
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x0003C7D0 File Offset: 0x0003A9D0
	public int GetCurrEnergy()
	{
		return Mathf.FloorToInt(this.model.currEnergy);
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x0003C7E2 File Offset: 0x0003A9E2
	public int GetCurrHealth()
	{
		return Mathf.FloorToInt(this.model.currHealth);
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0003C7F4 File Offset: 0x0003A9F4
	public int GetMaxHealth()
	{
		return this.model.maxHealth;
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x0003C801 File Offset: 0x0003AA01
	public int GetMaxEnergy()
	{
		return this.model.maxEnergy;
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x0003C80E File Offset: 0x0003AA0E
	public int GetCurrRad()
	{
		return Mathf.CeilToInt(Mathf.Min(this.model.currRads, (float)this.model.maxRads));
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x0003C831 File Offset: 0x0003AA31
	public PlayerState.AmmoMode GetAmmoMode()
	{
		return this.ammoMode;
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x0003C839 File Offset: 0x0003AA39
	public IEnumerable<KeyValuePair<PlayerState.AmmoMode, Ammo>> GetAmmoDict()
	{
		return this.ammoDict;
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x0003C841 File Offset: 0x0003AA41
	public Ammo GetAmmo(PlayerState.AmmoMode mode)
	{
		return this.ammoDict[mode];
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x0003C84F File Offset: 0x0003AA4F
	public void SetEnergy(int energy)
	{
		this.model.SetEnergy((float)energy);
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x0003C85E File Offset: 0x0003AA5E
	public void SetRad(int rad)
	{
		this.model.SetRad((float)rad);
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x0003C86D File Offset: 0x0003AA6D
	public void SetHealth(int health)
	{
		this.model.SetHealth((float)health);
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x0003C87C File Offset: 0x0003AA7C
	public void SetAmmoMode(PlayerState.AmmoMode mode)
	{
		if (this.ammoMode != mode)
		{
			this.ammoMode = mode;
			if (this.onAmmoModeChanged != null)
			{
				this.onAmmoModeChanged(mode);
			}
		}
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x0003C8A2 File Offset: 0x0003AAA2
	public int AddRads(float rads)
	{
		return this.model.AddRads(rads);
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x0003C8B0 File Offset: 0x0003AAB0
	public void RemoveRads(float rads)
	{
		this.model.currRads -= rads;
		this.model.radRecoverAfter = this.timeDir.WorldTime();
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x0003C8DB File Offset: 0x0003AADB
	public bool CanBeDamaged()
	{
		return !SRSingleton<SceneContext>.Instance.TimeDirector.IsFastForwarding() && SRInput.Instance.GetInputMode() == SRInput.InputMode.DEFAULT;
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x0003C900 File Offset: 0x0003AB00
	public bool Damage(int healthLoss, GameObject source)
	{
		if (!this.CanBeDamaged())
		{
			return false;
		}
		this.model.LoseHealth((float)healthLoss);
		if (this.timeDir.HasReached(this.nextAmmoLossDamageTime))
		{
			this.metadataDirector.Glitch.MaybeDamageExposure(source);
		}
		if (this.model.currHealth <= 0f)
		{
			this.model.currHealth = 0f;
			this.model.healthBurstAfter = double.PositiveInfinity;
			return true;
		}
		return false;
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x0003C984 File Offset: 0x0003AB84
	public void Heal(int healthGain)
	{
		this.model.currHealth = Mathf.Clamp(this.model.currHealth + (float)healthGain, 0f, (float)this.model.maxHealth);
		this.model.healthBurstAfter = this.timeDir.WorldTime();
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x0003C9D6 File Offset: 0x0003ABD6
	public void SpendEnergy(float energy)
	{
		this.model.SpendEnergy(energy);
		if (this.GetCurrEnergy() <= 0)
		{
			SECTR_AudioSystem.Play(this.onEnergyDepletedCue, base.transform.position, false);
		}
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x0003CA08 File Offset: 0x0003AC08
	public void AddCurrency(int adjust, PlayerState.CoinsType coinsType = PlayerState.CoinsType.NORM)
	{
		this.model.currency += adjust;
		this.model.currencyEverCollected += adjust;
		if (adjust > 0)
		{
			this.achieveDir.AddToStat(AchievementsDirector.IntStat.DAY_CURRENCY, adjust);
			this.achieveDir.AddToStat(AchievementsDirector.IntStat.CURRENCY, adjust);
		}
		SRSingleton<PopupElementsUI>.Instance.CreateCoinsPopup(adjust, coinsType);
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x0003CA65 File Offset: 0x0003AC65
	public void AddCurrencyDisplayDelta(int adjust)
	{
		this.model.currencyDisplayDelta += adjust;
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x0003CA7A File Offset: 0x0003AC7A
	public void SetCurrencyDisplay(int? currencyDisplay)
	{
		this.model.currencyDisplayOverride = currencyDisplay;
	}

	// Token: 0x06000F18 RID: 3864 RVA: 0x0003CA88 File Offset: 0x0003AC88
	public int GetDisplayedCurrency()
	{
		if (this.model.currencyDisplayOverride != null)
		{
			return this.model.currencyDisplayOverride.Value;
		}
		return this.model.currency + this.model.currencyDisplayDelta;
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x0003CAC4 File Offset: 0x0003ACC4
	public void SpendCurrency(int adjust, bool forcedLoss = false)
	{
		if (this.model.currency < adjust)
		{
			throw new ArgumentException("Attempting to spend more currency than we have.");
		}
		this.model.currency -= adjust;
		if (!forcedLoss)
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.GameIntStat.CURRENCY_SPENT, adjust);
		}
		SRSingleton<PopupElementsUI>.Instance.CreateCoinsPopup(-adjust, PlayerState.CoinsType.NORM);
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x0003CB1E File Offset: 0x0003AD1E
	public int GetCurrency()
	{
		return this.model.currency;
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x0003CB2B File Offset: 0x0003AD2B
	public void AddKey()
	{
		this.model.AddKey();
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x0003CB38 File Offset: 0x0003AD38
	public bool SpendKey()
	{
		if (this.model.keys >= 1)
		{
			this.model.keys--;
			return true;
		}
		return false;
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x0003CB5E File Offset: 0x0003AD5E
	public int GetKeys()
	{
		return this.model.keys;
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0003CB6B File Offset: 0x0003AD6B
	public void OnGameIntStatChanged(AchievementsDirector.GameIntStat stat, int val)
	{
		if (stat == AchievementsDirector.GameIntStat.FABRICATED_GADGETS)
		{
			this.CheckAllUpgradeLockers();
		}
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x0003CB77 File Offset: 0x0003AD77
	public void OnMailRead()
	{
		this.CheckAllUpgradeLockers();
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x0003CB80 File Offset: 0x0003AD80
	public void AddUpgrade(PlayerState.Upgrade upgrade, bool isFirstTime = false)
	{
		if (!this.model.upgrades.Contains(upgrade))
		{
			this.model.upgrades.Add(upgrade);
			this.model.ApplyUpgrade(upgrade, isFirstTime);
			this.CheckAllUpgradeLockers();
			if (upgrade == PlayerState.Upgrade.LIQUID_SLOT)
			{
				SRSingleton<SceneContext>.Instance.TutorialDirector.OnLiquidSlotGained();
			}
		}
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x0003CBD8 File Offset: 0x0003ADD8
	public bool HasOrCanGetUpgrade(PlayerState.Upgrade upgrade)
	{
		return this.HasUpgrade(upgrade) || this.CanGetUpgrade(upgrade);
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0003CBEC File Offset: 0x0003ADEC
	public bool HasUpgrade(PlayerState.Upgrade upgrade)
	{
		return this.model.upgrades.Contains(upgrade);
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0003CBFF File Offset: 0x0003ADFF
	public bool CanGetUpgrade(PlayerState.Upgrade upgrade)
	{
		return !this.HasUpgrade(upgrade) && this.model.availUpgrades.Contains(upgrade);
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x0003CC1D File Offset: 0x0003AE1D
	public void OnEnteredZone(ZoneDirector.Zone zone)
	{
		this.ammoDict[PlayerState.AmmoMode.NIMBLE_VALLEY].Clear(delegate(int ii)
		{
			Identifiable.Id slotName = this.ammoDict[PlayerState.AmmoMode.NIMBLE_VALLEY].GetSlotName(ii);
			return slotName - Identifiable.Id.VALLEY_AMMO_1 <= 3;
		});
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x00003296 File Offset: 0x00001496
	public void OnExitedZone(ZoneDirector.Zone zone)
	{
	}

	// Token: 0x04000DF1 RID: 3569
	private PlayerState.GameState gameState;

	// Token: 0x04000DF3 RID: 3571
	public GameObject[] potentialAmmo;

	// Token: 0x04000DF4 RID: 3572
	private PlayerModel model;

	// Token: 0x04000DF5 RID: 3573
	[Tooltip("SFX played each time the player's energy is depleted. (optional)")]
	public SECTR_AudioCue onEnergyDepletedCue;

	// Token: 0x04000DF9 RID: 3577
	public PlayerState.OnAmmoModeChanged onAmmoModeChanged;

	// Token: 0x04000DFA RID: 3578
	private Dictionary<PlayerState.AmmoMode, Ammo> ammoDict;

	// Token: 0x04000DFB RID: 3579
	private PlayerState.AmmoMode ammoMode;

	// Token: 0x04000DFC RID: 3580
	public static PlayerState.UpgradeComparer upgradeComparer = new PlayerState.UpgradeComparer();

	// Token: 0x04000DFD RID: 3581
	private TimeDirector timeDir;

	// Token: 0x04000DFE RID: 3582
	private PopupDirector popupDir;

	// Token: 0x04000DFF RID: 3583
	private AchievementsDirector achieveDir;

	// Token: 0x04000E00 RID: 3584
	private MailDirector mailDir;

	// Token: 0x04000E01 RID: 3585
	private MetadataDirector metadataDirector;

	// Token: 0x04000E03 RID: 3587
	private static readonly Predicate<Identifiable.Id> NO_LIQUID = (Identifiable.Id id) => !Identifiable.IsLiquid(id);

	// Token: 0x04000E04 RID: 3588
	private static readonly Predicate<Identifiable.Id> ONLY_LIQUID = (Identifiable.Id id) => Identifiable.IsLiquid(id);

	// Token: 0x04000E05 RID: 3589
	public static readonly Predicate<Identifiable.Id>[] PLAYER_AMMO_PREDS = new Predicate<Identifiable.Id>[]
	{
		PlayerState.NO_LIQUID,
		PlayerState.NO_LIQUID,
		PlayerState.NO_LIQUID,
		PlayerState.NO_LIQUID,
		PlayerState.ONLY_LIQUID
	};

	// Token: 0x020002C1 RID: 705
	// (Invoke) Token: 0x06000F2E RID: 3886
	public delegate bool UnlockCondition();

	// Token: 0x020002C2 RID: 706
	public enum CoinsType
	{
		// Token: 0x04000E07 RID: 3591
		NONE = -1,
		// Token: 0x04000E08 RID: 3592
		NORM,
		// Token: 0x04000E09 RID: 3593
		MOCHI,
		// Token: 0x04000E0A RID: 3594
		DRONE
	}

	// Token: 0x020002C3 RID: 707
	public class UpgradeLockData
	{
		// Token: 0x06000F31 RID: 3889 RVA: 0x0003CD95 File Offset: 0x0003AF95
		public UpgradeLockData(bool timedLock, double lockedUntil)
		{
			this.timedLock = timedLock;
			this.lockedUntil = lockedUntil;
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x000053FC File Offset: 0x000035FC
		public UpgradeLockData()
		{
		}

		// Token: 0x04000E0B RID: 3595
		public bool timedLock;

		// Token: 0x04000E0C RID: 3596
		public double lockedUntil;
	}

	// Token: 0x020002C4 RID: 708
	public class UpgradeLocker
	{
		// Token: 0x06000F33 RID: 3891 RVA: 0x0003CDAB File Offset: 0x0003AFAB
		public UpgradeLocker(PlayerState playerState, PlayerState.UnlockCondition unlockCondition, float unlockDelayHrs)
		{
			this.playerState = playerState;
			this.unlockCondition = unlockCondition;
			this.unlockDelayHrs = unlockDelayHrs;
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0003CDC8 File Offset: 0x0003AFC8
		public bool CheckUnlockCondition()
		{
			return !this.timedLock && this.unlockCondition();
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0003CDDF File Offset: 0x0003AFDF
		public bool ReachedUnlockTime()
		{
			return this.timedLock && this.playerState.timeDir.HasReached(this.lockedUntil);
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0003CE01 File Offset: 0x0003B001
		public void Unlock()
		{
			this.timedLock = true;
			this.lockedUntil = this.playerState.timeDir.HoursFromNow(this.unlockDelayHrs);
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0003CE26 File Offset: 0x0003B026
		public void Push(PlayerState.UpgradeLockData data)
		{
			this.timedLock = data.timedLock;
			this.lockedUntil = data.lockedUntil;
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0003CE40 File Offset: 0x0003B040
		public void Pull(out PlayerState.UpgradeLockData data)
		{
			data = new PlayerState.UpgradeLockData(this.timedLock, this.lockedUntil);
		}

		// Token: 0x04000E0D RID: 3597
		private PlayerState playerState;

		// Token: 0x04000E0E RID: 3598
		private PlayerState.UnlockCondition unlockCondition;

		// Token: 0x04000E0F RID: 3599
		private float unlockDelayHrs;

		// Token: 0x04000E10 RID: 3600
		private bool timedLock;

		// Token: 0x04000E11 RID: 3601
		private double lockedUntil;
	}

	// Token: 0x020002C5 RID: 709
	// (Invoke) Token: 0x06000F3A RID: 3898
	public delegate void OnEndGame();

	// Token: 0x020002C6 RID: 710
	private enum GameState
	{
		// Token: 0x04000E13 RID: 3603
		DEFAULT,
		// Token: 0x04000E14 RID: 3604
		GAME_OVER
	}

	// Token: 0x020002C7 RID: 711
	// (Invoke) Token: 0x06000F3E RID: 3902
	public delegate void OnEndGameTimeChanged();

	// Token: 0x020002C8 RID: 712
	public enum AmmoMode
	{
		// Token: 0x04000E16 RID: 3606
		DEFAULT,
		// Token: 0x04000E17 RID: 3607
		NIMBLE_VALLEY
	}

	// Token: 0x020002C9 RID: 713
	public class AmmoModeComparer : IEqualityComparer<PlayerState.AmmoMode>
	{
		// Token: 0x06000F41 RID: 3905 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(PlayerState.AmmoMode a, PlayerState.AmmoMode b)
		{
			return a == b;
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(PlayerState.AmmoMode a)
		{
			return (int)a;
		}

		// Token: 0x04000E18 RID: 3608
		public static PlayerState.AmmoModeComparer Instance = new PlayerState.AmmoModeComparer();
	}

	// Token: 0x020002CA RID: 714
	// (Invoke) Token: 0x06000F46 RID: 3910
	public delegate void OnAmmoModeChanged(PlayerState.AmmoMode mode);

	// Token: 0x020002CB RID: 715
	public enum Upgrade
	{
		// Token: 0x04000E1A RID: 3610
		HEALTH_1,
		// Token: 0x04000E1B RID: 3611
		HEALTH_2,
		// Token: 0x04000E1C RID: 3612
		HEALTH_3,
		// Token: 0x04000E1D RID: 3613
		ENERGY_1,
		// Token: 0x04000E1E RID: 3614
		ENERGY_2,
		// Token: 0x04000E1F RID: 3615
		ENERGY_3,
		// Token: 0x04000E20 RID: 3616
		AMMO_1,
		// Token: 0x04000E21 RID: 3617
		AMMO_2,
		// Token: 0x04000E22 RID: 3618
		AMMO_3,
		// Token: 0x04000E23 RID: 3619
		JETPACK,
		// Token: 0x04000E24 RID: 3620
		JETPACK_EFFICIENCY,
		// Token: 0x04000E25 RID: 3621
		AIR_BURST,
		// Token: 0x04000E26 RID: 3622
		RUN_EFFICIENCY,
		// Token: 0x04000E27 RID: 3623
		LIQUID_SLOT,
		// Token: 0x04000E28 RID: 3624
		AMMO_4,
		// Token: 0x04000E29 RID: 3625
		HEALTH_4,
		// Token: 0x04000E2A RID: 3626
		RUN_EFFICIENCY_2,
		// Token: 0x04000E2B RID: 3627
		GOLDEN_SURESHOT,
		// Token: 0x04000E2C RID: 3628
		SPARE_KEY,
		// Token: 0x04000E2D RID: 3629
		TREASURE_CRACKER_1 = 100,
		// Token: 0x04000E2E RID: 3630
		TREASURE_CRACKER_2,
		// Token: 0x04000E2F RID: 3631
		TREASURE_CRACKER_3,
		// Token: 0x04000E30 RID: 3632
		TREASURE_CRACKER_4
	}

	// Token: 0x020002CC RID: 716
	public class UpgradeComparer : IEqualityComparer<PlayerState.Upgrade>
	{
		// Token: 0x06000F49 RID: 3913 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(PlayerState.Upgrade x, PlayerState.Upgrade y)
		{
			return x == y;
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(PlayerState.Upgrade obj)
		{
			return (int)obj;
		}
	}

	// Token: 0x020002CD RID: 717
	public enum GameMode
	{
		// Token: 0x04000E32 RID: 3634
		CLASSIC,
		// Token: 0x04000E33 RID: 3635
		TIME_LIMIT,
		// Token: 0x04000E34 RID: 3636
		CASUAL,
		// Token: 0x04000E35 RID: 3637
		TIME_LIMIT_V2
	}

	// Token: 0x020002CE RID: 718
	public class GameModeComparer : IEqualityComparer<PlayerState.GameMode>
	{
		// Token: 0x06000F4C RID: 3916 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(PlayerState.GameMode a, PlayerState.GameMode b)
		{
			return a == b;
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(PlayerState.GameMode a)
		{
			return (int)a;
		}

		// Token: 0x04000E36 RID: 3638
		public static PlayerState.GameModeComparer Instance = new PlayerState.GameModeComparer();
	}

	// Token: 0x020002CF RID: 719
	private class AvailUpgradePopupCreator : PopupDirector.PopupCreator
	{
		// Token: 0x06000F50 RID: 3920 RVA: 0x0003CE6D File Offset: 0x0003B06D
		public AvailUpgradePopupCreator(PlayerState.Upgrade id)
		{
			this.id = id;
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x0003CE7C File Offset: 0x0003B07C
		public override void Create()
		{
			AvailUpgradePopupUI.CreateAvailUpgradePopup(this.id);
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x0003CE8A File Offset: 0x0003B08A
		public override bool Equals(object other)
		{
			return other is PlayerState.AvailUpgradePopupCreator && ((PlayerState.AvailUpgradePopupCreator)other).id == this.id;
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0003CEA9 File Offset: 0x0003B0A9
		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		// Token: 0x06000F54 RID: 3924 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public override bool ShouldClear()
		{
			return false;
		}

		// Token: 0x04000E37 RID: 3639
		private PlayerState.Upgrade id;
	}
}
