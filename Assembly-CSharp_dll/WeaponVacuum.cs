using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Util.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020002E4 RID: 740
public class WeaponVacuum : SRBehaviour, PlayerModel.Participant
{
	// Token: 0x06000FC9 RID: 4041 RVA: 0x0003E418 File Offset: 0x0003C618
	public void Awake()
	{
		this.vacAudio = base.GetComponent<SECTR_PointSource>();
		this.vacAudioHandler = new WeaponVacuum.VacAudioHandler(this);
		this.regionRegistry = SRSingleton<SceneContext>.Instance.RegionRegistry;
		this.pediaDir = SRSingleton<SceneContext>.Instance.PediaDirector;
		this.tutDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.optionsDir = SRSingleton<GameContext>.Instance.OptionsDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.playerEvents = base.GetComponentInParent<vp_FPPlayerEventHandler>();
		this.member = base.GetComponentInParent<RegionMember>();
		this.tracker = this.vacRegion.GetComponent<TrackCollisions>();
		this.animActiveId = Animator.StringToHash("active");
		this.animHoldingId = Animator.StringToHash("holding");
		this.animVacModeId = Animator.StringToHash("vacMode");
		this.animSprintingId = Animator.StringToHash("sprinting");
		this.animSwitchSlotsId = Animator.StringToHash("switchSlots");
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x0003E526 File Offset: 0x0003C726
	public void Start()
	{
		this.player = SRSingleton<SceneContext>.Instance.PlayerState;
		this.nextShot = Time.fixedTime;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPlayerParticipant(this);
		this.cameraHeld.SetActive(false);
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(PlayerModel model)
	{
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x0003E55F File Offset: 0x0003C75F
	public void SetModel(PlayerModel model)
	{
		this.playerModel = model;
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x00003296 File Offset: 0x00001496
	public void RegionSetChanged(RegionRegistry.RegionSetId previous, RegionRegistry.RegionSetId current)
	{
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x00003296 File Offset: 0x00001496
	public void TransformChanged(Vector3 pos, Quaternion rot)
	{
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x00003296 File Offset: 0x00001496
	public void RegisteredPotentialAmmoChanged(Dictionary<PlayerState.AmmoMode, List<GameObject>> registeredPotentialAmmo)
	{
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00003296 File Offset: 0x00001496
	public void KeyAdded()
	{
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x0003E568 File Offset: 0x0003C768
	public void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		HashSet<GameObject> inVac = this.tracker.CurrColliders();
		this.UpdateHud(inVac);
		this.UpdateSlotForInputs();
		this.UpdateVacModeForInputs();
		SRSingleton<SceneContext>.Instance.PlayerState.InGadgetMode = (this.vacMode == WeaponVacuum.VacMode.GADGET);
		if (SRInput.Actions.attack.WasPressed || SRInput.Actions.vac.WasPressed || SRInput.Actions.burst.WasPressed)
		{
			this.launchedHeld = false;
		}
		float num = 1f;
		if (Time.fixedTime >= this.nextShot && !this.launchedHeld && this.vacMode == WeaponVacuum.VacMode.SHOOT)
		{
			this.Expel(inVac);
			num = this.GetShootSpeedFactor(inVac);
			this.nextShot = Time.fixedTime + this.shootCooldown / num;
		}
		if (this.vacAnimator != null)
		{
			this.vacAnimator.speed = num;
		}
		if (!this.launchedHeld && this.vacMode == WeaponVacuum.VacMode.VAC)
		{
			this.vacAudioHandler.SetActive(true);
			this.vacFX.SetActive(this.held == null);
			this.siloActivator.enabled = (this.held == null);
			if (this.held != null)
			{
				this.UpdateHeld(inVac);
			}
			else
			{
				this.Consume(inVac);
			}
		}
		else
		{
			this.ClearVac();
		}
		this.UpdateVacAnimators();
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x0003E6CC File Offset: 0x0003C8CC
	private float GetShootSpeedFactor(HashSet<GameObject> inVac)
	{
		float num = 1f;
		foreach (GameObject gameObject in inVac)
		{
			VacShootAccelerator component = gameObject.GetComponent<VacShootAccelerator>();
			if (component != null)
			{
				num = Math.Max(num, component.GetVacShootSpeedFactor());
			}
		}
		return num;
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x0003E730 File Offset: 0x0003C930
	private void UpdateVacAnimators()
	{
		bool flag = this.playerModel.hasAirBurst && SRInput.Actions.burst.WasPressed;
		bool flag2 = this.vacMode == WeaponVacuum.VacMode.SHOOT || this.vacMode == WeaponVacuum.VacMode.VAC || flag;
		bool value = this.vacMode == WeaponVacuum.VacMode.VAC;
		if (this.vacAnimator == null)
		{
			this.vacAnimator = base.GetComponentInChildren<Animator>();
			this.vacColorAnimator = base.GetComponentInChildren<VacColorAnimator>();
		}
		this.vacAnimator.SetBool(this.animActiveId, flag2);
		this.vacAnimator.SetBool(this.animVacModeId, value);
		this.vacAnimator.SetBool(this.animHoldingId, this.held != null);
		this.vacColorAnimator.SetVacActive(flag2);
		this.vacColorAnimator.SetVacMode(value);
		if (flag)
		{
			this.AirBurst();
		}
		this.vacAnimator.SetBool(this.animSprintingId, this.playerEvents.Run.Active);
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x0003E828 File Offset: 0x0003CA28
	private void ClearVac()
	{
		this.ClearLiquids();
		this.vacAudioHandler.SetActive(false);
		this.vacFX.SetActive(false);
		this.siloActivator.enabled = false;
		foreach (Joint joint in this.joints)
		{
			if (joint != null && joint.connectedBody != null)
			{
				Vacuumable component = joint.connectedBody.GetComponent<Vacuumable>();
				if (component != null)
				{
					component.release();
				}
			}
		}
		if (this.held != null)
		{
			Vacuumable component2 = this.held.GetComponent<Vacuumable>();
			if (component2 != null)
			{
				component2.release();
			}
			this.lockJoint.connectedBody = null;
			Identifiable component3 = this.held.GetComponent<Identifiable>();
			this.held = null;
			this.SetHeldRad(0f);
			if (component3 != null && Identifiable.IsTarr(component3.id))
			{
				int val = (int)Math.Floor((this.timeDir.WorldTime() - this.heldStartTime) * 0.01666666753590107);
				this.achieveDir.MaybeUpdateMaxStat(AchievementsDirector.IntStat.EXTENDED_TARR_HOLD, val);
			}
			this.heldStartTime = double.NaN;
		}
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x0003E984 File Offset: 0x0003CB84
	private void SetHeldRad(float rad)
	{
		this.heldRad = rad;
		Vector3 localPosition = this.lockJoint.transform.localPosition;
		if (rad == 0f)
		{
			this.lockJoint.transform.localPosition = new Vector3(localPosition.x, 1f, localPosition.z);
			return;
		}
		this.lockJoint.transform.localPosition = new Vector3(localPosition.x, rad, localPosition.z);
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x0003E9FC File Offset: 0x0003CBFC
	private void UpdateHeld(HashSet<GameObject> inVac)
	{
		Rigidbody component = this.held.GetComponent<Rigidbody>();
		if (this.lockJoint.connectedBody != component)
		{
			this.held.transform.position = this.lockJoint.transform.position;
			this.lockJoint.connectedBody = component;
			this.lockJoint.anchor = Vector3.zero;
			this.lockJoint.connectedAnchor = Vector3.zero;
		}
		foreach (GameObject gameObject in inVac)
		{
			if (gameObject != this.held)
			{
				Vacuumable component2 = gameObject.GetComponent<Vacuumable>();
				if (component2 != null)
				{
					component2.release();
				}
			}
		}
		this.ClearLiquids();
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x0003EAD8 File Offset: 0x0003CCD8
	public void LateUpdate()
	{
		if (this.held != null)
		{
			this.held.transform.position = this.lockJoint.transform.position;
			this.held.transform.LookAt(this.heldFaceTowards.transform, this.heldFaceTowards.transform.up);
		}
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x0003EB3E File Offset: 0x0003CD3E
	public bool InVacMode()
	{
		return this.vacMode == WeaponVacuum.VacMode.VAC;
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x0003EB49 File Offset: 0x0003CD49
	private void PlayTransientAudio(SECTR_AudioCue cue)
	{
		SECTR_AudioSystem.Play(cue, base.transform.position, false);
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x0003EB60 File Offset: 0x0003CD60
	private void UpdateSlotForInputs()
	{
		if (SRInput.Actions.slot1.WasPressed)
		{
			if (this.player.Ammo.SetAmmoSlot(0))
			{
				this.PlayTransientAudio(this.vacAmmoSelectCue);
				this.vacAnimator.SetTrigger(this.animSwitchSlotsId);
				return;
			}
		}
		else if (SRInput.Actions.slot2.WasPressed)
		{
			if (this.player.Ammo.SetAmmoSlot(1))
			{
				this.PlayTransientAudio(this.vacAmmoSelectCue);
				this.vacAnimator.SetTrigger(this.animSwitchSlotsId);
				return;
			}
		}
		else if (SRInput.Actions.slot3.WasPressed)
		{
			if (this.player.Ammo.SetAmmoSlot(2))
			{
				this.PlayTransientAudio(this.vacAmmoSelectCue);
				this.vacAnimator.SetTrigger(this.animSwitchSlotsId);
				return;
			}
		}
		else if (SRInput.Actions.slot4.WasPressed)
		{
			if (this.player.Ammo.SetAmmoSlot(3))
			{
				this.PlayTransientAudio(this.vacAmmoSelectCue);
				this.vacAnimator.SetTrigger(this.animSwitchSlotsId);
				return;
			}
		}
		else if (SRInput.Actions.slot5.WasPressed)
		{
			if (this.player.Ammo.SetAmmoSlot(4))
			{
				this.PlayTransientAudio(this.vacAmmoSelectCue);
				this.vacAnimator.SetTrigger(this.animSwitchSlotsId);
				return;
			}
		}
		else
		{
			if (SRInput.Actions.prevSlot.WasPressed)
			{
				this.player.Ammo.PrevAmmoSlot();
				this.PlayTransientAudio(this.vacAmmoSelectCue);
				this.vacAnimator.SetTrigger(this.animSwitchSlotsId);
				return;
			}
			if (SRInput.Actions.nextSlot.WasPressed)
			{
				this.player.Ammo.NextAmmoSlot();
				this.PlayTransientAudio(this.vacAmmoSelectCue);
				this.vacAnimator.SetTrigger(this.animSwitchSlotsId);
			}
		}
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x0003ED44 File Offset: 0x0003CF44
	private void UpdateVacModeForInputs()
	{
		if (SRInput.Actions.toggleGadgetMode.WasReleased && this.progressDir.HasProgress(ProgressDirector.ProgressType.UNLOCK_LAB))
		{
			if (this.vacMode == WeaponVacuum.VacMode.GADGET)
			{
				this.vacMode = WeaponVacuum.VacMode.NONE;
				SRSingleton<Overlay>.Instance.SetEnableGadgetMode(false);
				this.PlayTransientAudio(this.gadgetModeOnCue);
			}
			else
			{
				this.vacMode = WeaponVacuum.VacMode.GADGET;
				SRSingleton<Overlay>.Instance.SetEnableGadgetMode(true);
				this.tutDir.OnGadgetModeActivated();
				this.PlayTransientAudio(this.gadgetModeOffCue);
			}
		}
		switch (this.vacMode)
		{
		case WeaponVacuum.VacMode.NONE:
			if (SRInput.Actions.vac.WasPressed)
			{
				this.vacMode = WeaponVacuum.VacMode.VAC;
				return;
			}
			if (SRInput.Actions.attack.WasPressed)
			{
				this.vacMode = WeaponVacuum.VacMode.SHOOT;
				return;
			}
			break;
		case WeaponVacuum.VacMode.SHOOT:
			if (SRInput.Actions.vac.WasPressed)
			{
				this.vacMode = WeaponVacuum.VacMode.VAC;
				return;
			}
			if (!SRInput.Actions.attack.IsPressed)
			{
				this.vacMode = WeaponVacuum.VacMode.NONE;
			}
			break;
		case WeaponVacuum.VacMode.VAC:
			if (SRInput.Actions.attack.WasPressed)
			{
				this.vacMode = WeaponVacuum.VacMode.SHOOT;
				return;
			}
			if ((this.optionsDir.vacLockOnHold && this.held != null) ? SRInput.Actions.vac.WasPressed : (!SRInput.Actions.vac.IsPressed))
			{
				this.vacMode = WeaponVacuum.VacMode.NONE;
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x0003EEA3 File Offset: 0x0003D0A3
	public void EnterWater()
	{
		this.inWaterCount++;
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x0003EEB3 File Offset: 0x0003D0B3
	public void ExitWater()
	{
		this.inWaterCount--;
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x0003EEC3 File Offset: 0x0003D0C3
	public bool IsHolding()
	{
		return this.held != null;
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x0003EED1 File Offset: 0x0003D0D1
	public Identifiable.Id HeldIdent()
	{
		if (this.held != null)
		{
			return this.held.GetComponent<Identifiable>().id;
		}
		return Identifiable.Id.NONE;
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x0003EEF4 File Offset: 0x0003D0F4
	public void DropAllVacced()
	{
		foreach (Joint joint in this.joints)
		{
			if (joint != null && joint.connectedBody != null)
			{
				Vacuumable component = joint.connectedBody.GetComponent<Vacuumable>();
				if (component != null)
				{
					component.release();
				}
			}
		}
		if (this.held != null)
		{
			Vacuumable component2 = this.held.GetComponent<Vacuumable>();
			if (component2 != null)
			{
				component2.release();
			}
			this.lockJoint.connectedBody = null;
			this.held = null;
			this.SetHeldRad(0f);
			this.heldStartTime = double.NaN;
		}
		this.vacAudioHandler.SetActive(false);
		if (this.vacAnimator == null)
		{
			this.vacAnimator = base.GetComponentInChildren<Animator>();
			this.vacColorAnimator = base.GetComponentInChildren<VacColorAnimator>();
		}
		this.vacAnimator.SetBool(this.animActiveId, false);
		this.vacAnimator.SetBool(this.animVacModeId, false);
		this.vacAnimator.SetBool(this.animHoldingId, false);
		this.vacColorAnimator.SetVacActive(false);
		this.vacColorAnimator.SetVacMode(false);
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x0003F048 File Offset: 0x0003D248
	private void ClearLiquids()
	{
		foreach (WeaponVacuum.IncomingLiquid incomingLiquid in this.liquidDict.Values)
		{
			Destroyer.Destroy(incomingLiquid.fx, "WeaponVacuum.ClearLiquids");
		}
		this.liquidDict.Clear();
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x0003F0B4 File Offset: 0x0003D2B4
	private void AirBurst()
	{
		if ((float)this.player.GetCurrEnergy() < this.staminaPerBurst)
		{
			this.PlayTransientAudio(this.vacBurstNoEnergyCue);
			return;
		}
		AnalyticsUtil.CustomEvent("PulseWaveUsed", null, true);
		this.PlayTransientAudio(this.vacBurstCue);
		Vector3 position = this.vacOrigin.transform.position;
		foreach (Collider collider in Physics.OverlapSphere(position, this.airBurstRadius))
		{
			if (collider && collider.GetComponent<Rigidbody>() != null && collider.gameObject != base.gameObject)
			{
				Identifiable component = collider.gameObject.GetComponent<Identifiable>();
				if (component != null && Identifiable.IsSlime(component.id) && Vector3.Dot(this.vacOrigin.transform.up, collider.gameObject.transform.position - this.vacOrigin.transform.position) > 0f)
				{
					Rigidbody component2 = collider.GetComponent<Rigidbody>();
					PhysicsUtil.SoftExplosionForce(this.airBurstPower * component2.mass, position, this.airBurstRadius, component2);
				}
			}
		}
		this.player.SpendEnergy(this.staminaPerBurst);
		if (this.airBurstFX != null)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.airBurstFX, Vector3.zero, Quaternion.identity).transform.SetParent(this.vacOrigin.transform, false);
		}
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x0003F236 File Offset: 0x0003D436
	private void Expel(HashSet<GameObject> inVac)
	{
		if (this.held != null)
		{
			this.ExpelHeld();
			return;
		}
		if (this.player.Ammo.HasSelectedAmmo())
		{
			this.ExpelAmmo(inVac);
			return;
		}
		this.PlayTransientAudio(this.vacShootEmptyCue);
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x0003F274 File Offset: 0x0003D474
	private void ExpelHeld()
	{
		vp_FPController componentInParent = base.GetComponentInParent<vp_FPController>();
		Ray ray = new Ray(this.vacOrigin.transform.position, this.vacOrigin.transform.up);
		Vector3 vector = ray.origin;
		Vector3 velocity = ray.direction * this.ejectSpeed + componentInParent.Velocity;
		vector = this.EnsureNotShootingIntoRock(vector, ray, this.heldRad, ref velocity);
		this.held.transform.position = vector;
		this.held.GetComponent<Rigidbody>().velocity = velocity;
		Vacuumable component = this.held.GetComponent<Vacuumable>();
		if (component != null)
		{
			component.release();
			component.Launch(Vacuumable.LaunchSource.PLAYER);
			SlimeEat component2 = this.held.GetComponent<SlimeEat>();
			if (component2 != null)
			{
				component2.CancelChomp(SRSingleton<SceneContext>.Instance.Player);
			}
		}
		DamagePlayerOnTouch component3 = this.held.GetComponent<DamagePlayerOnTouch>();
		if (component3 != null)
		{
			component3.ResetDamageAmnesty();
		}
		this.lockJoint.connectedBody = null;
		Identifiable component4 = this.held.GetComponent<Identifiable>();
		this.held = null;
		this.SetHeldRad(0f);
		if (component4 != null && Identifiable.IsTarr(component4.id))
		{
			int val = (int)Math.Floor((this.timeDir.WorldTime() - this.heldStartTime) * 0.01666666753590107);
			this.achieveDir.MaybeUpdateMaxStat(AchievementsDirector.IntStat.EXTENDED_TARR_HOLD, val);
		}
		this.heldStartTime = double.NaN;
		this.launchedHeld = true;
		this.ShootEffect();
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x0003F408 File Offset: 0x0003D608
	private void ExpelAmmo(HashSet<GameObject> inVac)
	{
		GameObject selectedStored = this.player.Ammo.GetSelectedStored();
		Identifiable component = selectedStored.GetComponent<Identifiable>();
		this.Expel(selectedStored, false);
		this.player.Ammo.DecrementSelectedAmmo(1);
		if (component != null)
		{
			this.tutDir.OnShoot(component.id);
		}
		this.ShootEffect();
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x0003F466 File Offset: 0x0003D666
	private float GetSpeed(Identifiable.Id id)
	{
		if (id - Identifiable.Id.VALLEY_AMMO_1 <= 3)
		{
			return this.ejectSpeed * 3f;
		}
		return this.ejectSpeed;
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x0003F488 File Offset: 0x0003D688
	public void Expel(GameObject toExpel, bool ignoreEmotions = false)
	{
		vp_FPController componentInParent = base.GetComponentInParent<vp_FPController>();
		Ray ray = new Ray(this.vacOrigin.transform.position, this.vacOrigin.transform.up);
		float num = PhysicsUtil.RadiusOfObject(toExpel);
		float num2 = (ray.direction.y >= 0f) ? 0f : (-0.5f * ray.direction.y);
		Vector3 vector = ray.origin + ray.direction * (num * 0.2f + num2);
		Vector3 velocity = ray.direction * this.GetSpeed(this.player.Ammo.GetSelectedId()) + componentInParent.Velocity;
		vector = this.EnsureNotShootingIntoRock(vector, ray, num, ref velocity);
		GameObject gameObject = SRBehaviour.InstantiateActor(toExpel, this.regionRegistry.GetCurrentRegionSetId(), vector, Quaternion.identity, false);
		gameObject.transform.LookAt(base.transform);
		PhysicsUtil.RestoreFreezeRotationConstraints(gameObject);
		SlimeEmotions component = gameObject.GetComponent<SlimeEmotions>();
		if (component != null && this.player.Ammo.GetSelectedId() != Identifiable.Id.NONE && !ignoreEmotions)
		{
			component.SetAll(this.player.Ammo.GetSelectedEmotions());
		}
		gameObject.GetComponent<Rigidbody>().velocity = velocity;
		gameObject.transform.DOScale(gameObject.transform.localScale, 0.1f).From(gameObject.transform.localScale * 0.2f, true).SetEase(Ease.Linear);
		gameObject.GetComponent<Vacuumable>().Launch(Vacuumable.LaunchSource.PLAYER);
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x0003F628 File Offset: 0x0003D828
	public void OnDamageExposure(GlitchMetadata.ExposureMetadata metadata, int count)
	{
		vp_FPController controller = base.GetComponentInParent<vp_FPController>();
		GameObject prefab = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(Identifiable.Id.GLITCH_SLIME);
		float radius = PhysicsUtil.RadiusOfObject(prefab);
		GlitchMetadata.ExposureMetadata metadata2 = metadata;
		GameObject gameObject = null;
		int? count2 = new int?(count);
		metadata2.OnExposed(gameObject, new Vector3?(base.transform.position), delegate(out Vector3 position, out Vector3 velocity)
		{
			Ray ray = new Ray(this.vacOrigin.transform.position, this.vacOrigin.transform.up);
			float num = (ray.direction.y >= 0f) ? 0f : (-0.5f * ray.direction.y);
			velocity = ray.direction * metadata.velocity + controller.Velocity;
			velocity = Quaternion.Euler(metadata.velocityRotationX.GetRandom(), metadata.velocityRotationY.GetRandom(), 0f) * velocity;
			position = ray.origin + ray.direction * (radius * 0.2f + num);
			position = this.EnsureNotShootingIntoRock(position, ray, radius, ref velocity);
		}, count2, null, delegate(GameObject instance)
		{
			instance.GetComponent<GlitchSlimeFlee>().DisableExposureChance();
			this.vacAnimator.SetTrigger(this.animSwitchSlotsId);
			this.ShootEffect();
		});
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x0003F6B4 File Offset: 0x0003D8B4
	private Vector3 EnsureNotShootingIntoRock(Vector3 startPos, Ray ray, float objRad, ref Vector3 vel)
	{
		float d = 0.5f;
		Ray ray2 = new Ray(ray.origin - ray.direction * d, ray.direction);
		float magnitude = (startPos - ray2.origin).magnitude;
		int layerMask = 270572033;
		RaycastHit raycastHit;
		Physics.Raycast(ray2, out raycastHit, magnitude, layerMask, QueryTriggerInteraction.Ignore);
		if (raycastHit.collider != null)
		{
			startPos = raycastHit.point - ray.direction * objRad;
			vel -= Vector3.Project(vel, raycastHit.normal);
		}
		return startPos;
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x0003F76A File Offset: 0x0003D96A
	private void ShootEffect()
	{
		if (this.shootFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.shootFX, this.vacOrigin, Vector3.zero, Quaternion.identity);
		}
		this.PlayTransientAudio(this.vacShootCue);
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x0003F7A2 File Offset: 0x0003D9A2
	private void CaptureEffect()
	{
		if (this.captureFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.captureFX, this.vacOrigin, Vector3.zero, Quaternion.identity);
		}
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x0003F7CE File Offset: 0x0003D9CE
	private void CaptureFailedEffect()
	{
		if (this.captureFailedFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.captureFailedFX, this.vacOrigin, Vector3.zero, Quaternion.identity);
		}
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x0003F7FC File Offset: 0x0003D9FC
	private void Consume(HashSet<GameObject> inVac)
	{
		this.ConsumeExistingJointed();
		this.lastWeaponPos = new Vector3?(this.vacOrigin.transform.position);
		List<LiquidSource> currLiquids = new List<LiquidSource>();
		UnityWorkarounds.SafeRemoveAllNulls<Vacuumable>(this.animatingConsume);
		int num = 0;
		foreach (GameObject gameObj in inVac)
		{
			this.ConsumeVacItem(gameObj, ref num, ref currLiquids);
		}
		if (num > 0 && !CellDirector.IsOnRanch(this.member))
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.MaybeUpdateMaxStat(AchievementsDirector.IntStat.SLIMES_IN_VAC, num);
		}
		this.ConsumeLiquids(currLiquids);
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x0003F8AC File Offset: 0x0003DAAC
	private void ConsumeLiquids(List<LiquidSource> currLiquids)
	{
		List<LiquidSource> list = new List<LiquidSource>();
		foreach (LiquidSource item in this.liquidDict.Keys)
		{
			if (!currLiquids.Contains(item))
			{
				list.Add(item);
			}
		}
		foreach (LiquidSource key in list)
		{
			Destroyer.Destroy(this.liquidDict[key].fx, "WeaponVacuum.ConsumeLiquids");
			this.liquidDict.Remove(key);
		}
		foreach (LiquidSource liquidSource in currLiquids)
		{
			if (!this.liquidDict.ContainsKey(liquidSource))
			{
				GameObject gameObject = SRBehaviour.SpawnAndPlayFX(SRSingleton<GameContext>.Instance.LookupDirector.GetLiquidIncomingFX(liquidSource.liquidId));
				gameObject.transform.SetParent(this.vacOrigin.transform, false);
				this.liquidDict[liquidSource] = new WeaponVacuum.IncomingLiquid(liquidSource.liquidId, Time.time + 0.25f, gameObject);
			}
			else if (Time.time >= this.liquidDict[liquidSource].nextUnitTick)
			{
				bool flag = this.ConsumeLiquid(liquidSource);
				this.liquidDict[liquidSource].nextUnitTick += 0.25f;
				if (flag)
				{
					liquidSource.ConsumeLiquid();
					this.CaptureEffect();
				}
				else
				{
					SRBehaviour.SpawnAndPlayFX(SRSingleton<GameContext>.Instance.LookupDirector.GetLiquidVacFailFX(liquidSource.liquidId)).transform.SetParent(this.vacOrigin.transform, false);
				}
			}
		}
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x0003FAC8 File Offset: 0x0003DCC8
	private bool ConsumeLiquid(LiquidSource source)
	{
		if (this.player.Ammo.MaybeAddToSlot(source.liquidId, null))
		{
			return true;
		}
		if (source.ReplacesExistingLiquidAmmo())
		{
			for (int i = 0; i < this.player.Ammo.GetUsableSlotCount(); i++)
			{
				Identifiable.Id slotName = this.player.Ammo.GetSlotName(i);
				if (slotName != source.liquidId && Identifiable.IsLiquid(slotName))
				{
					return this.player.Ammo.Replace(i, source.liquidId);
				}
			}
		}
		return false;
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x0003FB50 File Offset: 0x0003DD50
	private void ConsumeExistingJointed()
	{
		this.joints.RemoveAll((Joint joint) => joint == null);
		foreach (Joint joint2 in this.joints)
		{
			SpringJoint springJoint = (SpringJoint)joint2;
			if (!(springJoint.connectedBody != null) || !springJoint.connectedBody.isKinematic)
			{
				float magnitude = springJoint.transform.localPosition.magnitude;
				float num = 0f;
				if (this.lastWeaponPos != null)
				{
					num = (springJoint.transform.position - this.lastWeaponPos.Value).magnitude - magnitude;
				}
				float t = magnitude / this.maxVacDist;
				float num2 = Mathf.Lerp(this.maxJointSpeed, this.minJointSpeed, t);
				float spring = Mathf.Lerp(this.maxSpringStrength, this.minSpringStrength, t);
				float num3 = Mathf.Max(0f, magnitude - num2 * Time.deltaTime - num);
				if (magnitude > 0f)
				{
					springJoint.transform.localPosition = num3 / magnitude * springJoint.transform.localPosition;
				}
				springJoint.spring = spring;
			}
		}
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x0003FCC8 File Offset: 0x0003DEC8
	private void ConsumeVacItem(GameObject gameObj, ref int slimesInVac, ref List<LiquidSource> currLiquids)
	{
		Vacuumable component = gameObj.GetComponent<Vacuumable>();
		Identifiable component2 = gameObj.GetComponent<Identifiable>();
		LiquidSource component3 = gameObj.GetComponent<LiquidSource>();
		if (component2 != null && Identifiable.IsSlime(component2.id))
		{
			slimesInVac++;
		}
		if (component != null && component.enabled && (component2 == null || !Identifiable.IsLiquid(component2.id)) && !this.animatingConsume.Contains(component))
		{
			Vector3 direction = gameObj.transform.position - this.vacOrigin.transform.position;
			Ray ray = new Ray(this.vacOrigin.transform.position, direction);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, this.maxVacDist, -536887557))
			{
				if (raycastHit.rigidbody != null && raycastHit.rigidbody.gameObject == gameObj)
				{
					if (component.GetDestroyOnVac())
					{
						SRBehaviour.SpawnAndPlayFX(this.destroyOnVacFX, gameObj.transform.position, gameObj.transform.rotation);
						if (component2 == null)
						{
							Destroyer.Destroy(gameObj, "WeaponVacuum.ConsumeVacItem#1");
						}
						else
						{
							Destroyer.DestroyActor(gameObj, "WeaponVacuum.ConsumeVacItem#2", false);
						}
					}
					else if (component.canCapture() && (!this.slimeFilter || component2 == null || !Identifiable.IsSlime(component2.id)))
					{
						Rigidbody component4 = component.GetComponent<Rigidbody>();
						if (component.isCaptive() && component.IsTornadoed())
						{
							component.release();
						}
						if (!component.isCaptive())
						{
							if (component4.isKinematic)
							{
								component.Pending = true;
							}
							else
							{
								component.capture(this.CreateJoint(new Ray(this.vacOrigin.transform.position, this.vacOrigin.transform.rotation * Vector3.up), component));
							}
						}
						if (!component4.isKinematic && component2 != null && (Identifiable.IsAnimal(component2.id) || Identifiable.IsSlime(component2.id)))
						{
							this.RotateTowards(gameObj, this.heldFaceTowards.transform.position - gameObj.transform.position);
						}
					}
				}
				if (component2 != null && component.isCaptive() && Vector3.Distance(gameObj.transform.position, ray.origin) < this.captureDist)
				{
					if (component2.id != Identifiable.Id.NONE && component.enabled && component.size == Vacuumable.Size.NORMAL)
					{
						if (component != null)
						{
							base.StartCoroutine(this.StartConsuming(component, component2.id));
						}
					}
					else if (this.held == null && component.enabled && component.canCapture())
					{
						this.held = gameObj;
						this.SetHeldRad(PhysicsUtil.RadiusOfObject(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(component2.id)));
						component.hold();
						if (Identifiable.IsLargo(component2.id))
						{
							this.tutDir.MaybeShowPopup(TutorialDirector.Id.LARGO);
							this.pediaDir.MaybeShowPopup(PediaDirector.Id.LARGO_SLIME);
						}
						SlimeFeral component5 = gameObj.GetComponent<SlimeFeral>();
						if (component5 != null && component5.IsFeral())
						{
							this.pediaDir.MaybeShowPopup(PediaDirector.Id.FERAL_SLIME);
						}
						this.heldStartTime = this.timeDir.WorldTime();
						SlimeEat component6 = this.held.GetComponent<SlimeEat>();
						if (component6 != null)
						{
							component6.ResetEatClock();
						}
						TentacleGrapple component7 = this.held.GetComponent<TentacleGrapple>();
						if (component7 != null)
						{
							component7.Release();
						}
						GroundVine component8 = this.held.GetComponent<GroundVine>();
						if (component8 != null)
						{
							component8.Release();
						}
						this.pediaDir.MaybeShowPopup(component2.id);
						this.PlayTransientAudio(this.vacHeldCue);
						SRSingleton<SceneContext>.Instance.Player.GetComponent<ScreenShaker>().ShakeFrontImpact(1f);
					}
				}
			}
		}
		if (component3 != null && component3.Available())
		{
			if (this.playerEvents.Underwater.Active)
			{
				currLiquids.Add(component3);
				return;
			}
			float num = 1.5f;
			Vector3 up = this.vacOrigin.transform.up;
			Vector3 origin = this.vacOrigin.transform.position - up.normalized * num;
			float maxDistance = this.maxVacDist + num;
			Ray ray2 = new Ray(origin, up);
			float num2 = float.PositiveInfinity;
			float num3 = float.NaN;
			foreach (RaycastHit raycastHit2 in Physics.RaycastAll(ray2, maxDistance, -536887557, QueryTriggerInteraction.Collide))
			{
				if (raycastHit2.collider.gameObject == gameObj)
				{
					num3 = raycastHit2.distance;
				}
				else if (!raycastHit2.collider.isTrigger)
				{
					num2 = Math.Min(num2, raycastHit2.distance);
				}
			}
			if (num3 <= num2)
			{
				currLiquids.Add(component3);
			}
		}
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x000401D3 File Offset: 0x0003E3D3
	private IEnumerator StartConsuming(Vacuumable vacuumable, Identifiable.Id id)
	{
		vacuumable.StartConsumeScale();
		WeaponVacuum.MoveTowards moveTowards = vacuumable.gameObject.AddComponent<WeaponVacuum.MoveTowards>();
		moveTowards.dest = this.vacOrigin.transform;
		this.animatingConsume.Add(vacuumable);
		yield return new WaitForSeconds(0.2f);
		Destroyer.Destroy(moveTowards, "WeaponVacuum.StartConsuming");
		if (vacuumable != null)
		{
			if (vacuumable.TryConsume())
			{
				this.CaptureEffect();
				this.pediaDir.MaybeShowPopup(id);
				this.tutDir.OnVac(id);
				if (Identifiable.IsPlort(id) && !CellDirector.IsOnRanch(this.member))
				{
					this.achieveDir.AddToStat(AchievementsDirector.IntStat.DAY_COLLECT_PLORTS, 1);
				}
			}
			else
			{
				vacuumable.release();
				this.CaptureFailedEffect();
				this.animatingConsume.Remove(vacuumable);
				vacuumable.ReverseConsumeScale();
			}
		}
		yield break;
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x000401F0 File Offset: 0x0003E3F0
	public void ForceJoint(Vacuumable vacuumable)
	{
		vacuumable.capture(this.CreateJoint(new Ray(this.vacOrigin.transform.position, this.vacOrigin.transform.rotation * Vector3.up), vacuumable));
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x00040230 File Offset: 0x0003E430
	private Joint CreateJoint(Ray alongRay, Vacuumable vacuumable)
	{
		Vector3 position = vacuumable.transform.position;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.vacJointPrefab);
		gameObject.transform.position = position;
		gameObject.transform.SetParent(this.vacOrigin.transform, true);
		SpringJoint component = gameObject.GetComponent<SpringJoint>();
		component.spring = this.minSpringStrength;
		this.joints.Add(component);
		return component;
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x00040298 File Offset: 0x0003E498
	private Vector3 ClosestPointOnRay(Ray ray, Vector3 pt)
	{
		Vector3 rhs = pt - ray.origin;
		float d = Vector3.Dot(ray.direction, rhs);
		return ray.origin + ray.direction * d;
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x000402DA File Offset: 0x0003E4DA
	private void UpdateHud(HashSet<GameObject> inVac)
	{
		this.UpdateCrosshair(inVac);
		this.UpdateTargetingInfo();
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x000402EC File Offset: 0x0003E4EC
	private void UpdateCrosshair(HashSet<GameObject> inVac)
	{
		bool pointedAtVaccable = false;
		foreach (GameObject gameObject in inVac)
		{
			if (gameObject == null)
			{
				Debug.Log("Null gameobj, skipping: " + ((gameObject == null) ? "null" : gameObject.name));
				if (Application.isEditor)
				{
				}
			}
			else
			{
				try
				{
					Vacuumable vacuumable;
					if (!WeaponVacuum.recentIds.contains(gameObject.GetInstanceID()))
					{
						vacuumable = gameObject.GetComponent<Vacuumable>();
						WeaponVacuum.recentIds.put(gameObject.GetInstanceID(), vacuumable);
					}
					else
					{
						vacuumable = WeaponVacuum.recentIds.get(gameObject.GetInstanceID());
					}
					if (vacuumable != null && vacuumable.enabled)
					{
						pointedAtVaccable = true;
						break;
					}
				}
				catch (Exception ex)
				{
					Debug.Log("Got an e, skipping: " + ((gameObject == null) ? "null" : gameObject.name) + " msg: " + ex.Message);
					bool isEditor = Application.isEditor;
				}
			}
		}
		this.player.PointedAtVaccable = pointedAtVaccable;
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x00040420 File Offset: 0x0003E620
	private void UpdateTargetingInfo()
	{
		Ray ray = new Ray(this.vacOrigin.transform.position, this.vacOrigin.transform.up);
		this.player.Targeting = null;
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, this.maxVacDist, -536887557))
		{
			this.player.Targeting = raycastHit.collider.gameObject;
		}
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x00040489 File Offset: 0x0003E689
	public bool InGadgetMode()
	{
		return this.vacMode == WeaponVacuum.VacMode.GADGET;
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x00040494 File Offset: 0x0003E694
	private void RotateTowards(GameObject gameObj, Vector3 dirToTarget)
	{
		Rigidbody component = gameObj.GetComponent<Rigidbody>();
		Vector3 a = Vector3.Cross(Quaternion.AngleAxis(component.angularVelocity.magnitude * 57.29578f / this.facingSpeed, component.angularVelocity) * component.transform.forward, dirToTarget);
		component.AddTorque(a * this.facingSpeed * this.facingSpeed);
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x00040504 File Offset: 0x0003E704
	public void FixedUpdate()
	{
		if (this.held != null)
		{
			float num = this.heldRad;
			float num2 = num * 2f + 0.1f;
			Ray ray = new Ray(this.lockJoint.transform.position - num2 * this.lockJoint.transform.up, this.lockJoint.transform.up);
			float num3 = num2;
			int layerMask = 270567937;
			RaycastHit raycastHit;
			if (Physics.SphereCast(ray, num, out raycastHit, num3, layerMask, QueryTriggerInteraction.Ignore))
			{
				vp_FPController componentInParent = base.GetComponentInParent<vp_FPController>();
				float d = (num3 - raycastHit.distance) / num;
				Vector3 force = Vector3.Project(this.vacOrigin.transform.up, raycastHit.normal) * -0.2f * d * d;
				componentInParent.AddDepenetrationForce(force);
			}
		}
	}

	// Token: 0x04000E89 RID: 3721
	public float ejectSpeed = 25f;

	// Token: 0x04000E8A RID: 3722
	public float shootCooldown = 0.24f;

	// Token: 0x04000E8B RID: 3723
	public GameObject vacRegion;

	// Token: 0x04000E8C RID: 3724
	public GameObject vacOrigin;

	// Token: 0x04000E8D RID: 3725
	public GameObject vacJointPrefab;

	// Token: 0x04000E8E RID: 3726
	public Joint lockJoint;

	// Token: 0x04000E8F RID: 3727
	public GameObject vacFX;

	// Token: 0x04000E90 RID: 3728
	public float maxVacDist = 20f;

	// Token: 0x04000E91 RID: 3729
	public float captureDist = 1f;

	// Token: 0x04000E92 RID: 3730
	public float minJointSpeed = 3f;

	// Token: 0x04000E93 RID: 3731
	public float maxJointSpeed = 6f;

	// Token: 0x04000E94 RID: 3732
	public float minSpringStrength = 5f;

	// Token: 0x04000E95 RID: 3733
	public float maxSpringStrength = 20f;

	// Token: 0x04000E96 RID: 3734
	public float airBurstPower = 1200f;

	// Token: 0x04000E97 RID: 3735
	public float airBurstRadius = 20f;

	// Token: 0x04000E98 RID: 3736
	public GameObject airBurstFX;

	// Token: 0x04000E99 RID: 3737
	public float staminaPerBurst = 20f;

	// Token: 0x04000E9A RID: 3738
	public GameObject shootFX;

	// Token: 0x04000E9B RID: 3739
	public GameObject captureFX;

	// Token: 0x04000E9C RID: 3740
	public GameObject captureFailedFX;

	// Token: 0x04000E9D RID: 3741
	public GameObject heldFaceTowards;

	// Token: 0x04000E9E RID: 3742
	public SECTR_AudioCue vacStartCue;

	// Token: 0x04000E9F RID: 3743
	public SECTR_AudioCue vacRunCue;

	// Token: 0x04000EA0 RID: 3744
	public SECTR_AudioCue vacEndCue;

	// Token: 0x04000EA1 RID: 3745
	public SECTR_AudioCue vacShootCue;

	// Token: 0x04000EA2 RID: 3746
	public SECTR_AudioCue vacShootEmptyCue;

	// Token: 0x04000EA3 RID: 3747
	public SECTR_AudioCue vacAmmoSelectCue;

	// Token: 0x04000EA4 RID: 3748
	public SECTR_AudioCue vacBurstCue;

	// Token: 0x04000EA5 RID: 3749
	public SECTR_AudioCue vacBurstNoEnergyCue;

	// Token: 0x04000EA6 RID: 3750
	public SECTR_AudioCue vacHeldCue;

	// Token: 0x04000EA7 RID: 3751
	public SECTR_AudioCue gadgetModeOnCue;

	// Token: 0x04000EA8 RID: 3752
	public SECTR_AudioCue gadgetModeOffCue;

	// Token: 0x04000EA9 RID: 3753
	public SiloActivator siloActivator;

	// Token: 0x04000EAA RID: 3754
	public GameObject destroyOnVacFX;

	// Token: 0x04000EAB RID: 3755
	[Tooltip("Reference to the HeldCamera GameObject.")]
	public GameObject cameraHeld;

	// Token: 0x04000EAC RID: 3756
	private float nextShot;

	// Token: 0x04000EAD RID: 3757
	private PlayerState player;

	// Token: 0x04000EAE RID: 3758
	private GameObject held;

	// Token: 0x04000EAF RID: 3759
	private double heldStartTime;

	// Token: 0x04000EB0 RID: 3760
	private float heldRad;

	// Token: 0x04000EB1 RID: 3761
	private bool launchedHeld;

	// Token: 0x04000EB2 RID: 3762
	private SECTR_PointSource vacAudio;

	// Token: 0x04000EB3 RID: 3763
	private List<Joint> joints = new List<Joint>();

	// Token: 0x04000EB4 RID: 3764
	private WeaponVacuum.VacAudioHandler vacAudioHandler;

	// Token: 0x04000EB5 RID: 3765
	private Animator vacAnimator;

	// Token: 0x04000EB6 RID: 3766
	private VacColorAnimator vacColorAnimator;

	// Token: 0x04000EB7 RID: 3767
	private Vector3? lastWeaponPos;

	// Token: 0x04000EB8 RID: 3768
	private RegionRegistry regionRegistry;

	// Token: 0x04000EB9 RID: 3769
	private PediaDirector pediaDir;

	// Token: 0x04000EBA RID: 3770
	private TutorialDirector tutDir;

	// Token: 0x04000EBB RID: 3771
	private AchievementsDirector achieveDir;

	// Token: 0x04000EBC RID: 3772
	private TimeDirector timeDir;

	// Token: 0x04000EBD RID: 3773
	private OptionsDirector optionsDir;

	// Token: 0x04000EBE RID: 3774
	private ProgressDirector progressDir;

	// Token: 0x04000EBF RID: 3775
	private int inWaterCount;

	// Token: 0x04000EC0 RID: 3776
	private Dictionary<LiquidSource, WeaponVacuum.IncomingLiquid> liquidDict = new Dictionary<LiquidSource, WeaponVacuum.IncomingLiquid>();

	// Token: 0x04000EC1 RID: 3777
	private vp_FPPlayerEventHandler playerEvents;

	// Token: 0x04000EC2 RID: 3778
	private bool slimeFilter;

	// Token: 0x04000EC3 RID: 3779
	private HashSet<Vacuumable> animatingConsume = new HashSet<Vacuumable>();

	// Token: 0x04000EC4 RID: 3780
	private TrackCollisions tracker;

	// Token: 0x04000EC5 RID: 3781
	private PlayerModel playerModel;

	// Token: 0x04000EC6 RID: 3782
	private RegionMember member;

	// Token: 0x04000EC7 RID: 3783
	private WeaponVacuum.VacMode vacMode;

	// Token: 0x04000EC8 RID: 3784
	private const int VAC_RAY_MASK = -536887557;

	// Token: 0x04000EC9 RID: 3785
	private int animActiveId;

	// Token: 0x04000ECA RID: 3786
	private int animHoldingId;

	// Token: 0x04000ECB RID: 3787
	private int animVacModeId;

	// Token: 0x04000ECC RID: 3788
	private int animSprintingId;

	// Token: 0x04000ECD RID: 3789
	private int animSwitchSlotsId;

	// Token: 0x04000ECE RID: 3790
	private const float LIQUID_UNIT_TIME = 0.25f;

	// Token: 0x04000ECF RID: 3791
	private const float SHOOT_SCALE_UP_TIME = 0.1f;

	// Token: 0x04000ED0 RID: 3792
	private const float SHOOT_SCALE_FACTOR = 0.2f;

	// Token: 0x04000ED1 RID: 3793
	private const float CONSUME_SCALE_DOWN_TIME = 0.2f;

	// Token: 0x04000ED2 RID: 3794
	private const float CONSUME_SCALE_FACTOR = 0.1f;

	// Token: 0x04000ED3 RID: 3795
	private const float HOLD_SCREEN_SHAKE = 1f;

	// Token: 0x04000ED4 RID: 3796
	private const float VALLEY_EJECT_SPEED_FACTOR = 3f;

	// Token: 0x04000ED5 RID: 3797
	private static LRUCache<int, Vacuumable> recentIds = new LRUCache<int, Vacuumable>(1000);

	// Token: 0x04000ED6 RID: 3798
	public float facingSpeed = 10f;

	// Token: 0x020002E5 RID: 741
	private class IncomingLiquid
	{
		// Token: 0x06000FFE RID: 4094 RVA: 0x000406A9 File Offset: 0x0003E8A9
		public IncomingLiquid(Identifiable.Id id, float nextUnitTick, GameObject fx)
		{
			this.id = id;
			this.nextUnitTick = nextUnitTick;
			this.fx = fx;
		}

		// Token: 0x04000ED7 RID: 3799
		public Identifiable.Id id;

		// Token: 0x04000ED8 RID: 3800
		public float nextUnitTick;

		// Token: 0x04000ED9 RID: 3801
		public GameObject fx;
	}

	// Token: 0x020002E6 RID: 742
	private enum VacMode
	{
		// Token: 0x04000EDB RID: 3803
		NONE,
		// Token: 0x04000EDC RID: 3804
		SHOOT,
		// Token: 0x04000EDD RID: 3805
		VAC,
		// Token: 0x04000EDE RID: 3806
		GADGET
	}

	// Token: 0x020002E7 RID: 743
	private class VacAudioHandler
	{
		// Token: 0x06000FFF RID: 4095 RVA: 0x000406C6 File Offset: 0x0003E8C6
		public VacAudioHandler(WeaponVacuum vac)
		{
			this.vac = vac;
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x000406D8 File Offset: 0x0003E8D8
		public void SetActive(bool active)
		{
			if (active && !this.active)
			{
				this.vac.vacAudio.Cue = this.vac.vacStartCue;
				this.vac.vacAudio.Play();
				this.vac.vacAudio.Cue = this.vac.vacRunCue;
				this.vac.vacAudio.Play();
			}
			else if (!active && this.active)
			{
				this.vac.vacAudio.Cue = this.vac.vacEndCue;
				this.vac.vacAudio.Play();
			}
			this.active = active;
		}

		// Token: 0x04000EDF RID: 3807
		private WeaponVacuum vac;

		// Token: 0x04000EE0 RID: 3808
		private bool active;
	}

	// Token: 0x020002E8 RID: 744
	private class MoveTowards : SRBehaviour
	{
		// Token: 0x06001001 RID: 4097 RVA: 0x00040785 File Offset: 0x0003E985
		public void Awake()
		{
			this.endTime = Time.fixedTime + 0.2f;
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x00040798 File Offset: 0x0003E998
		public void FixedUpdate()
		{
			float fixedTime = Time.fixedTime;
			float num = this.endTime - fixedTime;
			float t = (num <= Time.fixedDeltaTime) ? 1f : (Time.fixedDeltaTime / num);
			base.transform.position = Vector3.Lerp(base.transform.position, this.dest.position, t);
		}

		// Token: 0x04000EE1 RID: 3809
		public Transform dest;

		// Token: 0x04000EE2 RID: 3810
		private float endTime;
	}
}
