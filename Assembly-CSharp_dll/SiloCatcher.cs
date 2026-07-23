using System;
using System.Collections.Generic;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class SiloCatcher : SRBehaviour, VacShootAccelerator
{
	// Token: 0x06001168 RID: 4456 RVA: 0x00046068 File Offset: 0x00044268
	public void Awake()
	{
		if (this.type.HasInput())
		{
			this.audioSource = base.GetRequiredComponent<SECTR_AudioSource>();
		}
		if (this.type.HasOutput())
		{
			this.region = base.GetRequiredComponentInParent<Region>(false);
		}
		switch (this.type)
		{
		case SiloCatcher.Type.SILO_DEFAULT:
		case SiloCatcher.Type.SILO_OUTPUT_ONLY:
			this.storageSilo = base.GetRequiredComponentInParent<SiloStorage>(false);
			return;
		case SiloCatcher.Type.REFINERY:
			this.droneNetwork = base.GetComponentInParent<DroneNetwork>();
			if (this.droneNetwork != null)
			{
				this.droneNetwork.Register(this);
				return;
			}
			break;
		case SiloCatcher.Type.DECORIZER:
			this.storageDecorizer = base.GetRequiredComponentInParent<DecorizerStorage>(false);
			return;
		case SiloCatcher.Type.VIKTOR_STORAGE:
			this.storageGlitch = base.GetRequiredComponentInParent<GlitchStorage>(false);
			break;
		default:
			return;
		}
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x0004611A File Offset: 0x0004431A
	public void Start()
	{
		if (this.type.HasOutput())
		{
			this.vac = SRSingleton<SceneContext>.Instance.Player.GetRequiredComponentInChildren(false);
		}
		this.hasStarted = true;
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x00046146 File Offset: 0x00044346
	public void OnDestroy()
	{
		if (this.droneNetwork != null)
		{
			this.droneNetwork.Deregister(this);
			this.droneNetwork = null;
		}
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x0004616C File Offset: 0x0004436C
	public void OnTriggerEnter(Collider collider)
	{
		if (!this.hasStarted || !base.isActiveAndEnabled)
		{
			return;
		}
		if (collider.isTrigger)
		{
			return;
		}
		if (!this.type.HasInput())
		{
			return;
		}
		Identifiable.Id id = Identifiable.GetId(collider.gameObject);
		if (id == Identifiable.Id.NONE)
		{
			return;
		}
		Vacuumable component = collider.gameObject.GetComponent<Vacuumable>();
		if (component == null || component.isCaptive())
		{
			return;
		}
		if (!this.collectedThisFrame.Add(collider.gameObject))
		{
			return;
		}
		if (!this.Insert(id))
		{
			return;
		}
		DestroyOnTouching component2 = collider.gameObject.GetComponent<DestroyOnTouching>();
		if (component2 != null)
		{
			component2.NoteDestroying();
		}
		Destroyer.DestroyActor(collider.gameObject, "BaseCatcher.OnTriggerEnter", false);
		SRBehaviour.SpawnAndPlayFX(this.storeFX, collider.gameObject.transform.position, collider.gameObject.transform.rotation);
		this.audioSource.Play();
		this.accelerationInput.OnTriggered();
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x0004625B File Offset: 0x0004445B
	public void LateUpdate()
	{
		this.collectedThisFrame.Clear();
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x00046268 File Offset: 0x00044468
	private bool Insert(Identifiable.Id id)
	{
		switch (this.type)
		{
		case SiloCatcher.Type.SILO_DEFAULT:
			return this.storageSilo.MaybeAddIdentifiable(id, this.slotIdx, 1, false);
		case SiloCatcher.Type.REFINERY:
			return SRSingleton<SceneContext>.Instance.GadgetDirector.AddToRefinery(id);
		case SiloCatcher.Type.DECORIZER:
			return this.storageDecorizer.Add(id);
		case SiloCatcher.Type.VIKTOR_STORAGE:
			return this.storageGlitch.Add(id);
		}
		throw new ArgumentException(this.type.ToString());
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x000462EE File Offset: 0x000444EE
	public float GetVacShootSpeedFactor()
	{
		return this.accelerationInput.Factor;
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x000462FC File Offset: 0x000444FC
	public void OnTriggerStay(Collider collider)
	{
		if (!this.hasStarted || !base.isActiveAndEnabled)
		{
			return;
		}
		if (!this.type.HasOutput())
		{
			return;
		}
		if (Time.time < this.nextEject)
		{
			return;
		}
		SiloActivator componentInParent = collider.gameObject.GetComponentInParent<SiloActivator>();
		if (componentInParent == null || !componentInParent.enabled)
		{
			return;
		}
		Vector3 normalized = (collider.gameObject.transform.position - base.transform.position).normalized;
		if (Mathf.Abs(Vector3.Angle(base.transform.forward, normalized)) > 45f)
		{
			return;
		}
		Identifiable.Id id;
		if (!this.Remove(out id))
		{
			return;
		}
		Vacuumable component = SRBehaviour.InstantiateActor(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(id), this.region.setId, base.transform.position + normalized * 1.2f, base.transform.rotation, false).GetComponent<Vacuumable>();
		this.vac.ForceJoint(component);
		this.nextEject = Time.time + 0.25f / this.accelerationOutput.Factor;
		this.accelerationOutput.OnTriggered();
	}

	// Token: 0x06001170 RID: 4464 RVA: 0x00046428 File Offset: 0x00044628
	private bool Remove(out Identifiable.Id id)
	{
		switch (this.type)
		{
		case SiloCatcher.Type.SILO_DEFAULT:
		case SiloCatcher.Type.SILO_OUTPUT_ONLY:
		{
			Ammo relevantAmmo = this.storageSilo.GetRelevantAmmo();
			relevantAmmo.SetAmmoSlot(this.slotIdx);
			id = relevantAmmo.GetSelectedId();
			if (id == Identifiable.Id.NONE)
			{
				return false;
			}
			relevantAmmo.DecrementSelectedAmmo(1);
			return true;
		}
		case SiloCatcher.Type.DECORIZER:
			return this.storageDecorizer.Remove(out id);
		case SiloCatcher.Type.VIKTOR_STORAGE:
			return this.storageGlitch.Remove(out id);
		}
		throw new ArgumentException(this.type.ToString());
	}

	// Token: 0x0400109B RID: 4251
	[Tooltip("Input/output type.")]
	public SiloCatcher.Type type;

	// Token: 0x0400109C RID: 4252
	[Tooltip("FX played on successful input.")]
	public GameObject storeFX;

	// Token: 0x0400109D RID: 4253
	[Tooltip("Index in SiloStorage to input/output the object. (SILO_DEFAULT, SILO_OUTPUT_ONLY")]
	public int slotIdx;

	// Token: 0x0400109E RID: 4254
	private bool hasStarted;

	// Token: 0x0400109F RID: 4255
	private HashSet<GameObject> collectedThisFrame = new HashSet<GameObject>();

	// Token: 0x040010A0 RID: 4256
	private SiloStorage storageSilo;

	// Token: 0x040010A1 RID: 4257
	private DroneNetwork droneNetwork;

	// Token: 0x040010A2 RID: 4258
	private SECTR_AudioSource audioSource;

	// Token: 0x040010A3 RID: 4259
	private DecorizerStorage storageDecorizer;

	// Token: 0x040010A4 RID: 4260
	private GlitchStorage storageGlitch;

	// Token: 0x040010A5 RID: 4261
	private VacAccelerationHelper accelerationInput = VacAccelerationHelper.CreateInput();

	// Token: 0x040010A6 RID: 4262
	private const float EJECT_RATE = 0.25f;

	// Token: 0x040010A7 RID: 4263
	private const float EJECT_DIST = 1.2f;

	// Token: 0x040010A8 RID: 4264
	private const float MAX_ANGLE_DEGS = 45f;

	// Token: 0x040010A9 RID: 4265
	private float nextEject;

	// Token: 0x040010AA RID: 4266
	private WeaponVacuum vac;

	// Token: 0x040010AB RID: 4267
	private Region region;

	// Token: 0x040010AC RID: 4268
	private VacAccelerationHelper accelerationOutput = VacAccelerationHelper.CreateOutput();

	// Token: 0x0200033E RID: 830
	public enum Type
	{
		// Token: 0x040010AE RID: 4270
		SILO_DEFAULT,
		// Token: 0x040010AF RID: 4271
		SILO_OUTPUT_ONLY,
		// Token: 0x040010B0 RID: 4272
		REFINERY,
		// Token: 0x040010B1 RID: 4273
		DECORIZER,
		// Token: 0x040010B2 RID: 4274
		VIKTOR_STORAGE
	}
}
