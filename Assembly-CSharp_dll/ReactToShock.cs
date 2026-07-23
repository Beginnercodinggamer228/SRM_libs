using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000430 RID: 1072
public class ReactToShock : SRBehaviour
{
	// Token: 0x06001655 RID: 5717 RVA: 0x00056670 File Offset: 0x00054870
	public void Awake()
	{
		this.damagePlayer = base.GetComponent<DamagePlayerOnTouch>();
		this.regionMember = base.GetComponent<RegionMember>();
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		this.slimeAppearanceApplicator.OnAppearanceChanged += this.UpdateAppearances;
		if (this.slimeAppearanceApplicator.Appearance != null)
		{
			this.UpdateAppearances(this.slimeAppearanceApplicator.Appearance);
		}
		this.plortObj = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(this.plortId);
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x00056708 File Offset: 0x00054908
	public void Update()
	{
		if (this.timeDirector.HasReached(this.nextReactionTime))
		{
			if (this.cooldownFXInstance != null)
			{
				SRBehaviour.RecycleAndStopFX(this.cooldownFXInstance);
				this.cooldownFXInstance = null;
			}
			this.damagePlayer.SetBlocked(true);
		}
		if (this.checkAppearance && this.timeDirector.HasReached(this.nextReactionTime))
		{
			this.slimeAppearanceApplicator.Appearance = this.normalAppearance;
			this.slimeAppearanceApplicator.ApplyAppearance();
			this.checkAppearance = false;
		}
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x00056794 File Offset: 0x00054994
	public void DoShock(Identifiable.Id id)
	{
		switch (id)
		{
		case Identifiable.Id.VALLEY_AMMO_1:
			this.MaybeCreatePlorts(1);
			return;
		case Identifiable.Id.VALLEY_AMMO_2:
			this.MaybeCreatePlorts(2, ReactToShock.PlortSounds.SUCCESS);
			return;
		case Identifiable.Id.VALLEY_AMMO_3:
			break;
		case Identifiable.Id.VALLEY_AMMO_4:
			this.MaybeCreatePlorts(1, ReactToShock.PlortSounds.SUCCESS);
			if (this.electricField == null)
			{
				this.electricField = UnityEngine.Object.Instantiate<GameObject>(this.electricFieldPrefab);
				this.electricField.transform.SetParent(base.transform, false);
			}
			this.electricField.GetComponent<QuicksilverElectricField>().ResetDeathTime();
			break;
		default:
			return;
		}
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x00056820 File Offset: 0x00054A20
	public bool MaybeCreatePlorts(int count)
	{
		ReactToShock.PlortSounds mask = ReactToShock.PlortSounds.SUCCESS | ReactToShock.PlortSounds.FAILURE;
		return this.MaybeCreatePlorts(count, mask);
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x00056838 File Offset: 0x00054A38
	public bool MaybeCreatePlorts(int count, ReactToShock.PlortSounds mask)
	{
		if (this.timeDirector.HasReached(this.nextReactionTime))
		{
			for (int i = 0; i < count; i++)
			{
				Vector3 position = base.transform.TransformPoint(ReactToShock.LOCAL_PRODUCE_LOC);
				SRBehaviour.InstantiateActor(this.plortObj, this.regionMember.setId, position, base.transform.rotation, false);
				if (this.produceFX != null)
				{
					RecolorSlimeMaterial[] componentsInChildren = SRBehaviour.SpawnAndPlayFX(this.produceFX, position, base.transform.rotation).GetComponentsInChildren<RecolorSlimeMaterial>();
					if (componentsInChildren != null && componentsInChildren.Length != 0)
					{
						SlimeAppearance.Palette appearancePalette = this.slimeAppearanceApplicator.GetAppearancePalette();
						RecolorSlimeMaterial[] array = componentsInChildren;
						for (int j = 0; j < array.Length; j++)
						{
							array[j].SetColors(appearancePalette.Top, appearancePalette.Middle, appearancePalette.Bottom);
						}
					}
				}
			}
			if (this.cooldownFX != null)
			{
				if (this.cooldownFXInstance != null)
				{
					SRBehaviour.RecycleAndStopFX(this.cooldownFXInstance);
				}
				this.cooldownFXInstance = SRBehaviour.SpawnAndPlayFX(this.cooldownFX, base.gameObject);
			}
			this.damagePlayer.SetBlocked(false);
			this.slimeAppearanceApplicator.Appearance = this.shockedAppearance;
			this.slimeAppearanceApplicator.ApplyAppearance();
			this.checkAppearance = true;
			this.PlaySFX(this.onHitSuccessCue, ReactToShock.PlortSounds.SUCCESS, mask);
			this.nextReactionTime = this.timeDirector.HoursFromNow(this.cooldownHours);
			return true;
		}
		this.PlaySFX(this.onHitFailureCue, ReactToShock.PlortSounds.FAILURE, mask);
		return false;
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x000569B8 File Offset: 0x00054BB8
	private void UpdateAppearances(SlimeAppearance baseAppearance)
	{
		if (baseAppearance == this.normalAppearance || baseAppearance == this.shockedAppearance)
		{
			return;
		}
		this.normalAppearance = baseAppearance;
		this.shockedAppearance = baseAppearance.ShockedAppearance;
		if (this.checkAppearance)
		{
			this.slimeAppearanceApplicator.Appearance = this.shockedAppearance;
			this.slimeAppearanceApplicator.ApplyAppearance();
		}
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x00056A19 File Offset: 0x00054C19
	private bool PlaySFX(SECTR_AudioCue cue, ReactToShock.PlortSounds expected, ReactToShock.PlortSounds mask)
	{
		if ((mask & expected) != ReactToShock.PlortSounds.NONE)
		{
			SECTR_AudioSystem.Play(cue, base.transform.position, false);
			return true;
		}
		return false;
	}

	// Token: 0x04001543 RID: 5443
	public Identifiable.Id plortId;

	// Token: 0x04001544 RID: 5444
	public GameObject produceFX;

	// Token: 0x04001545 RID: 5445
	[Tooltip("Duration, in game hours, to cooldown before a reaction is available again.")]
	public float cooldownHours;

	// Token: 0x04001546 RID: 5446
	[Tooltip("FX to show while the cooldown is active. (optional)")]
	public GameObject cooldownFX;

	// Token: 0x04001547 RID: 5447
	[Tooltip("Prefab to instantiate an electric field.")]
	public GameObject electricFieldPrefab;

	// Token: 0x04001548 RID: 5448
	private GameObject electricField;

	// Token: 0x04001549 RID: 5449
	private DamagePlayerOnTouch damagePlayer;

	// Token: 0x0400154A RID: 5450
	[Tooltip("SFX played when the slime is hit and successfully produces a plort.")]
	public SECTR_AudioCue onHitSuccessCue;

	// Token: 0x0400154B RID: 5451
	[Tooltip("SFX played when the slime is hit and fails to produce a plort.")]
	public SECTR_AudioCue onHitFailureCue;

	// Token: 0x0400154C RID: 5452
	private double nextReactionTime;

	// Token: 0x0400154D RID: 5453
	private GameObject cooldownFXInstance;

	// Token: 0x0400154E RID: 5454
	private bool checkAppearance;

	// Token: 0x0400154F RID: 5455
	private RegionMember regionMember;

	// Token: 0x04001550 RID: 5456
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x04001551 RID: 5457
	private GameObject plortObj;

	// Token: 0x04001552 RID: 5458
	private TimeDirector timeDirector;

	// Token: 0x04001553 RID: 5459
	private SlimeAppearance normalAppearance;

	// Token: 0x04001554 RID: 5460
	private SlimeAppearance shockedAppearance;

	// Token: 0x04001555 RID: 5461
	private static Vector3 LOCAL_PRODUCE_LOC = new Vector3(0f, 0.5f, 0f);

	// Token: 0x02000431 RID: 1073
	[Flags]
	public enum PlortSounds
	{
		// Token: 0x04001557 RID: 5463
		NONE = 0,
		// Token: 0x04001558 RID: 5464
		SUCCESS = 1,
		// Token: 0x04001559 RID: 5465
		FAILURE = 2
	}
}
