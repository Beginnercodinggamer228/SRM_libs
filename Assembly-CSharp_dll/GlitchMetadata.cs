using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Util.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020004E2 RID: 1250
public class GlitchMetadata : ScriptableObject
{
	// Token: 0x06001A2F RID: 6703 RVA: 0x00065DE4 File Offset: 0x00063FE4
	public bool MaybeDamageExposure(GameObject source)
	{
		if (source == null)
		{
			return false;
		}
		Identifiable.Id id = Identifiable.GetId(source);
		if (id - Identifiable.Id.GLITCH_TARR_SLIME > 1)
		{
			return false;
		}
		PlayerState playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		int? ammoIdx = playerState.Ammo.GetAmmoIdx(Identifiable.Id.GLITCH_SLIME);
		if (ammoIdx == null)
		{
			return false;
		}
		int count = Mathf.Min(Mathf.FloorToInt(this.damageLossExposure.spawnCount.GetRandom()), playerState.Ammo.GetSlotCount(ammoIdx.Value));
		SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<WeaponVacuum>().OnDamageExposure(this.damageLossExposure, count);
		SRSingleton<AmmoSlotUI>.Instance.SpawnAndPlayFX(this.damageLossAmmoFX, ammoIdx.Value, count);
		playerState.Ammo.Decrement(ammoIdx.Value, count);
		TimeDirector timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		playerState.nextAmmoLossDamageTime = timeDirector.HoursFromNow(this.damageLossCooldown * 0.016666668f);
		SRSingleton<SceneContext>.Instance.TutorialDirector.MaybeShowPopup(TutorialDirector.Id.SLIMULATIONS_DAMAGE);
		return true;
	}

	// Token: 0x06001A30 RID: 6704 RVA: 0x00065EE0 File Offset: 0x000640E0
	public GlitchMetadata.ExposureMetadata GetDittoExposureMetadata(Identifiable.Id id)
	{
		if (this.dittoOverridesDictionary.ContainsKey(id))
		{
			return this.dittoOverridesDictionary[id].exposure;
		}
		if (Identifiable.LARGO_CLASS.Contains(id))
		{
			return this.dittoLargo.exposure;
		}
		if (!Identifiable.SLIME_CLASS.Contains(id))
		{
			return null;
		}
		return this.dittoStandard.exposure;
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x00065F40 File Offset: 0x00064140
	public float GetDittoProbability(Identifiable.Id id, GlitchSlimeSpawner spawner)
	{
		float? num = this.dittoOverridesDictionary.ContainsKey(id) ? new float?(this.dittoOverridesDictionary[id].probability) : (Identifiable.LARGO_CLASS.Contains(id) ? new float?(spawner.probablityLargo.GetOrDefault(this.dittoLargo.probability)) : (Identifiable.SLIME_CLASS.Contains(id) ? new float?(spawner.probablityStandard.GetOrDefault(this.dittoStandard.probability)) : null));
		if (num == null)
		{
			return 0f;
		}
		return num.GetValueOrDefault();
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x00065FE8 File Offset: 0x000641E8
	public void OnEnable()
	{
		if (this.dittoOverrides != null)
		{
			this.dittoOverridesDictionary = this.dittoOverrides.ToDictionary((GlitchMetadata.DittoMetadata_DictEntry e) => e.id, (GlitchMetadata.DittoMetadata_DictEntry e) => e, Identifiable.idComparer);
		}
	}

	// Token: 0x040019B0 RID: 6576
	[Header("Damage Loss Exposure")]
	[Tooltip("Exposure metadata applied when the player receives damage.")]
	public GlitchMetadata.ExposureMetadata damageLossExposure;

	// Token: 0x040019B1 RID: 6577
	[Tooltip("Time after exposing slimes on player damage before it can occur again. (in-game minutes)")]
	public float damageLossCooldown;

	// Token: 0x040019B2 RID: 6578
	[Tooltip("FX spawned on the AmmoSlotUI when slimes are removed.")]
	public GameObject damageLossAmmoFX;

	// Token: 0x040019B3 RID: 6579
	[Header("Glitch Teleporters")]
	[Tooltip("Time before the teleporter out of the SLIMULATIONS is activated. (in-game hours, random range)")]
	public Vector2 teleportActivationDelay;

	// Token: 0x040019B4 RID: 6580
	[Tooltip("Prefab containing the 'exit teleporter is now active' HUD prefab.")]
	public GameObject teleportHudPrefab;

	// Token: 0x040019B5 RID: 6581
	[Tooltip("Maximum time to show the 'exit teleporter is now active' HUD image. (seconds)")]
	public float teleportHudLifetime;

	// Token: 0x040019B6 RID: 6582
	[Header("Debug Spray/Station")]
	[Tooltip("Max ammo override capacity for Identifiable.Id.GLITCH_DEBUG_SPRAY_LIQUID.")]
	public int debugSprayMaxAmmo;

	// Token: 0x040019B7 RID: 6583
	[Tooltip("Amount of ammo acquired by vaccing a debug spray station.")]
	public int debugSprayAmmoPerStation;

	// Token: 0x040019B8 RID: 6584
	[Header("Ditto Largo/Slime")]
	[Tooltip("Default ditto metadata used by standard slimes.")]
	public GlitchMetadata.DittoMetadata dittoStandard;

	// Token: 0x040019B9 RID: 6585
	[Tooltip("Default ditto metadata used by largos.")]
	public GlitchMetadata.DittoMetadata dittoLargo;

	// Token: 0x040019BA RID: 6586
	[Tooltip("Dictionary of custom ditto metadata overrides by id.")]
	public List<GlitchMetadata.DittoMetadata_DictEntry> dittoOverrides;

	// Token: 0x040019BB RID: 6587
	public Dictionary<Identifiable.Id, GlitchMetadata.DittoMetadata> dittoOverridesDictionary;

	// Token: 0x040019BC RID: 6588
	[Header("Glitch Imposto")]
	[Tooltip("Exposure metadata applied to the imposto exposures.")]
	public GlitchMetadata.ExposureMetadata impostoExposure;

	// Token: 0x040019BD RID: 6589
	[Tooltip("Time before an imposto is deactivated when the player isn't looking. (in-game minutes)")]
	public float impostoDeactivateTime;

	// Token: 0x040019BE RID: 6590
	[Tooltip("Squared distance between the imposto and the player to be considered in detection range.")]
	public float impostoDetectionRange;

	// Token: 0x040019BF RID: 6591
	[Tooltip("Time before the imposto is deactivated after coming back into detection range. (seconds)")]
	public float impostoFailedExposedDelayTime;

	// Token: 0x040019C0 RID: 6592
	[Tooltip("Time that each imposto will cooldown before it is made available again. (in-game hours)")]
	public float impostoCooldownTime;

	// Token: 0x040019C1 RID: 6593
	[Tooltip("Minimum amount of time that a GlitchImpostoDirector must be hibernated before it resets impostos. (in-game hours)")]
	public float impostoMinHibernationTime;

	// Token: 0x040019C2 RID: 6594
	[Tooltip("Time between the next flicker animation is played. (in-game minutes, random range)")]
	public Vector2 impostoFlickerCooldownTime;

	// Token: 0x040019C3 RID: 6595
	[Tooltip("Flicker animation speed. (random range)")]
	public Vector2 impostoFlickerSpeed;

	// Token: 0x040019C4 RID: 6596
	[Tooltip("Flicker animation radius. (random range)")]
	public Vector2 impostoFlickerRadius;

	// Token: 0x040019C5 RID: 6597
	[Tooltip("Flicker animation number of points to move to.")]
	public int impostoFlickerPoints;

	// Token: 0x040019C6 RID: 6598
	[Tooltip("Flicker FX. (optional)")]
	public GameObject impostoFlickerFX;

	// Token: 0x040019C7 RID: 6599
	[Tooltip("Flicker SFX cue. (optional)")]
	public SECTR_AudioCue impostoFlickerCue;

	// Token: 0x040019C8 RID: 6600
	[Header("Glitch Slime")]
	[Tooltip("Time the glitch slime is alive. (in-game minutes, random range)")]
	public Vector2 slimeLifetime;

	// Token: 0x040019C9 RID: 6601
	[Tooltip("Exposure metadata applied to the glitch slime split.")]
	public GlitchMetadata.ExposureMetadata slimeExposure;

	// Token: 0x040019CA RID: 6602
	[Tooltip("Base percentage chance that the glitch slime will split.")]
	public float slimeBaseExposureChance;

	// Token: 0x040019CB RID: 6603
	[Tooltip("Percentage degradation of the glitch slime split chance each time a split occurs.")]
	public float slimeExposureChanceDegradation;

	// Token: 0x040019CC RID: 6604
	[Tooltip("Time before the glitch slime begins fleeing. (seconds)")]
	public float slimeFleeDelay;

	// Token: 0x040019CD RID: 6605
	[Tooltip("Max ammo capacity override for Identifiable.Id.GLITCH_SLIME.")]
	public int slimeMaxAmmo;

	// Token: 0x040019CE RID: 6606
	[Header("Glitch Tarr Slime")]
	[Tooltip("Lifetime override used by tarr in SLIMULATIONS. (in-game minutes, random range)")]
	public Vector2 tarrLifetime;

	// Token: 0x040019CF RID: 6607
	[Tooltip("Base percentage chance that the tarr slime will split into multiple tarr slimes on timed death.")]
	public float tarrBaseMultiplyChance;

	// Token: 0x040019D0 RID: 6608
	[Tooltip("Percentage degradation that the tarr slime will split into multiple tarr slimes on timed death.")]
	public float tarrMultiplyChanceDegradation;

	// Token: 0x040019D1 RID: 6609
	[Tooltip("Number of tarr slimes the tarr slime will multiply into on timed death.")]
	public Vector2 tarrMultiplyCount;

	// Token: 0x040019D2 RID: 6610
	[Header("Glitch Tarr Spawner")]
	[Tooltip("Time between checks of the tarr spawner should spawn more tarr. (in-game minutes)")]
	public float tarrSpawnerThrottleTime;

	// Token: 0x040019D3 RID: 6611
	[Header("Glitch Tarr Node")]
	[Tooltip("Base delay before more tarr spawners are activated. (in-game hours)")]
	public float tarrNodeActivationDelay;

	// Token: 0x040019D4 RID: 6612
	[Tooltip("Delay per spawner before the next tarr spawner is activated. (in-game hours)")]
	public float tarrNodeActivationDelayPerNode;

	// Token: 0x040019D5 RID: 6613
	[Tooltip("Minimum distance the tarr spawner must be from the entrance teleporter to be potentially activated immediately.")]
	public float tarrNodeMinActivationDistanceSquared;

	// Token: 0x040019D6 RID: 6614
	[Tooltip("Speed that tarr nodes are scaled in on activation.")]
	public float tarrNodeScaleInSpeed;

	// Token: 0x040019D7 RID: 6615
	[Tooltip("Delay added to tarr nodes that spawn on the player before damage is applied. (in-game minutes)")]
	public float tarrNodeSpawnDamagePreventionTime;

	// Token: 0x040019D8 RID: 6616
	[Tooltip("Additional multiplier applied to the tarr node music min/max distance.")]
	public float tarrNodeMusicDistanceMultiplier;

	// Token: 0x040019D9 RID: 6617
	[Header("Audio - Music")]
	[Tooltip("Music associated with Zone.SLIMULATIONS.")]
	public MusicDirector.Music.Zone.Default musicSlimulation;

	// Token: 0x040019DA RID: 6618
	[Tooltip("Music associated with Zone.VIKTOR_LAB.")]
	public MusicDirector.Music.Zone.Default musicViktorLab;

	// Token: 0x040019DB RID: 6619
	[Header("Ambiance")]
	[Tooltip("Fixed time-of-day in the AmbianceDirector.")]
	[Range(0f, 1f)]
	public float ambianceTimeOfDay;

	// Token: 0x040019DC RID: 6620
	[Header("Glitch Terminal Animation")]
	[Tooltip("FX prefab used during the player's transition to/from the SLIMULATIONS.")]
	public GameObject animationFX;

	// Token: 0x040019DD RID: 6621
	[Tooltip("Material to update the sea renderer with on teleportation in SLIMULATIONS region.")]
	public Material animationSeaMaterial;

	// Token: 0x040019DE RID: 6622
	[Tooltip("SFX cue to play when the terminal enters BOOT_UP state. (3D)")]
	public SECTR_AudioCue animationOnTerminalBootupCue;

	// Token: 0x040019DF RID: 6623
	[Tooltip("SFX cue to play when the terminal enters IDLE state. (3D, Looping)")]
	public SECTR_AudioCue animationOnTerminalIdleCue;

	// Token: 0x040019E0 RID: 6624
	[Tooltip("SFX cue to play when the player teleports into the SLIMULATIONS region. (2D)")]
	public SECTR_AudioCue animationOnTeleportInCue;

	// Token: 0x040019E1 RID: 6625
	[Tooltip("SFX cue to play when the player teleports out of the SLIMULATIONS region. (2D)")]
	public SECTR_AudioCue animationOnTeleportOutCue;

	// Token: 0x040019E2 RID: 6626
	[Header("Glitch Terminal Activator")]
	[Tooltip("GUI prefab to show when the activator is inactive due to progress.")]
	public GameObject activatorGuiProgress;

	// Token: 0x040019E3 RID: 6627
	[Tooltip("GUI prefab to show when the activator is inactive due to ammo.")]
	public GameObject activatorGuiAmmo;

	// Token: 0x040019E4 RID: 6628
	[Tooltip("GUI prefab to show when the activator is activate not but activate-able.")]
	public GameObject activatorGuiPreActive;

	// Token: 0x020004E3 RID: 1251
	[Serializable]
	public class ExposureMetadata
	{
		// Token: 0x06001A34 RID: 6708 RVA: 0x00066054 File Offset: 0x00064254
		public void OnExposed(GameObject gameObject = null, Vector3? origin = null, GlitchMetadata.ExposureMetadata.GetPositionAndVelocity getPositionAndVelocity = null, int? count = null, GameObject source = null, GlitchMetadata.ExposureMetadata.OnInstantiated onInstantiated = null)
		{
			GlitchMetadata.ExposureMetadata.OnInstantiated onInstantiated2;
			if (onInstantiated == null)
			{
				onInstantiated2 = delegate(GameObject go)
				{
				};
			}
			else
			{
				onInstantiated2 = onInstantiated;
			}
			onInstantiated = onInstantiated2;
			count = new int?((count != null) ? count.Value : Mathf.FloorToInt(this.spawnCount.GetRandom()));
			source = ((source != null) ? source : SRSingleton<SceneContext>.Instance.Player);
			origin = new Vector3?((origin != null) ? origin.Value : gameObject.transform.position);
			if (getPositionAndVelocity == null)
			{
				Vector3 sourceForward = source.transform.forward;
				getPositionAndVelocity = delegate(out Vector3 position, out Vector3 velocity)
				{
					Vector3 normalized = (Quaternion.Euler(this.velocityRotationX.GetRandom(), this.velocityRotationY.GetRandom(), 0f) * sourceForward).normalized;
					position = origin.Value + UnityEngine.Random.insideUnitSphere * this.spawnRadius;
					velocity = normalized * this.velocity;
				};
			}
			SRSingleton<SceneContext>.Instance.StartCoroutine(this.OnExposed_Coroutine(gameObject, getPositionAndVelocity, count.Value, source.transform.position, onInstantiated));
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x00066167 File Offset: 0x00064367
		private IEnumerator OnExposed_Coroutine(GameObject gameObject, GlitchMetadata.ExposureMetadata.GetPositionAndVelocity getPositionAndVelocity, int count, Vector3 sourcePosition, GlitchMetadata.ExposureMetadata.OnInstantiated onInstantiated)
		{
			LookupDirector lookupDirector = SRSingleton<GameContext>.Instance.LookupDirector;
			GameObject prefab = lookupDirector.GetPrefab(Identifiable.Id.GLITCH_SLIME);
			int num;
			for (int ii = 0; ii < count; ii = num)
			{
				Vector3 position;
				Vector3 vector;
				getPositionAndVelocity(out position, out vector);
				GameObject gameObject2 = SRBehaviour.InstantiateActor(prefab, RegionRegistry.RegionSetId.SLIMULATIONS, position, Quaternion.identity, false);
				gameObject2.transform.LookAt(sourcePosition);
				PhysicsUtil.RestoreFreezeRotationConstraints(gameObject2);
				gameObject2.GetComponent<Rigidbody>().velocity = vector;
				gameObject2.GetComponent<Vacuumable>().PreventCaptureFor(this.capturePreventionTime);
				if (this.capturePreventionFX != null)
				{
					Destroyer.Destroy(SRBehaviour.SpawnAndPlayFX(this.capturePreventionFX, gameObject2), this.capturePreventionTime, "GlitchMetadata.OnExposed", false, false);
				}
				float fromValue = gameObject2.transform.localScale.x * 0.2f;
				gameObject2.transform.DOScale(gameObject2.transform.localScale, 0.2f).From(fromValue, true).SetEase(Ease.Linear);
				onInstantiated(gameObject2);
				this.SpawnAndPlayFX(gameObject);
				yield return new WaitForSeconds(0.02f);
				num = ii + 1;
			}
			yield break;
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0006619B File Offset: 0x0006439B
		public void OnFailedExposed(GameObject gameObject)
		{
			this.SpawnAndPlayFX(gameObject);
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x000661A4 File Offset: 0x000643A4
		private void SpawnAndPlayFX(GameObject gameObject)
		{
			if (gameObject == null)
			{
				return;
			}
			if (this.onExposedFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.onExposedFX, gameObject.transform.position, Quaternion.identity);
			}
			SECTR_AudioSystem.Play(this.onExposedSFX, gameObject.transform.position, false);
		}

		// Token: 0x040019E5 RID: 6629
		[Tooltip("Random range of number of glitch slimes to spawn.")]
		public Vector2 spawnCount;

		// Token: 0x040019E6 RID: 6630
		[Tooltip("Additional offset applied to the spawn position as a random position in a sphere.")]
		public float spawnRadius;

		// Token: 0x040019E7 RID: 6631
		[Tooltip("Speed to spawn the glitch slime with.")]
		public float velocity;

		// Token: 0x040019E8 RID: 6632
		[Tooltip("Rotation applied to the velocity vector around the X axis. (random range)")]
		public Vector2 velocityRotationX;

		// Token: 0x040019E9 RID: 6633
		[Tooltip("Rotation applied to the velocity vector around the Y axis. (random range)")]
		public Vector2 velocityRotationY;

		// Token: 0x040019EA RID: 6634
		[Tooltip("Time after spawn that the glitch slimes will not be capturable. (seconds)")]
		public float capturePreventionTime;

		// Token: 0x040019EB RID: 6635
		[Tooltip("FX played on the glitch slime when it is not capturable. (optional)")]
		public GameObject capturePreventionFX;

		// Token: 0x040019EC RID: 6636
		[Tooltip("FX played when the exposure occurs. (optional)")]
		public GameObject onExposedFX;

		// Token: 0x040019ED RID: 6637
		[Tooltip("SFX played when the exposure occurs. (optional)")]
		public SECTR_AudioCue onExposedSFX;

		// Token: 0x020004E4 RID: 1252
		// (Invoke) Token: 0x06001A3A RID: 6714
		public delegate void OnInstantiated(GameObject instance);

		// Token: 0x020004E5 RID: 1253
		// (Invoke) Token: 0x06001A3E RID: 6718
		public delegate void GetPositionAndVelocity(out Vector3 position, out Vector3 velocity);
	}

	// Token: 0x020004EA RID: 1258
	[Serializable]
	public class DittoMetadata
	{
		// Token: 0x040019FE RID: 6654
		[Tooltip("Probability that the spawned slime will become a ditto.")]
		public float probability;

		// Token: 0x040019FF RID: 6655
		[Tooltip("Exposure metadata when this ditto is exposed.")]
		public GlitchMetadata.ExposureMetadata exposure;
	}

	// Token: 0x020004EB RID: 1259
	[Serializable]
	public class DittoMetadata_DictEntry : GlitchMetadata.DittoMetadata
	{
		// Token: 0x04001A00 RID: 6656
		[Tooltip("Identifiable id to override the ditto metadata for.")]
		public Identifiable.Id id;
	}
}
