using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003BB RID: 955
public class DestroyOnTouching : SRBehaviour
{
	// Token: 0x060013E9 RID: 5097 RVA: 0x0004D1A0 File Offset: 0x0004B3A0
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.plexer = base.gameObject.GetComponent<SlimeSubbehaviourPlexer>();
		this.hasPlexer = (this.plexer != null);
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		if (this.slimeAppearanceApplicator != null)
		{
			this.slimeAppearanceApplicator.OnAppearanceChanged += this.UpdateDestroyFX;
			if (this.slimeAppearanceApplicator.Appearance != null)
			{
				this.UpdateDestroyFX(this.slimeAppearanceApplicator.Appearance);
			}
		}
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x0004D235 File Offset: 0x0004B435
	public void FixedUpdate()
	{
		if (this.timeDir.HasReached(this.destroyAt))
		{
			this.DestroyAndWater();
		}
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x0004D250 File Offset: 0x0004B450
	public void NoteDestroying()
	{
		this.destroying = true;
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x0004D25C File Offset: 0x0004B45C
	private void DestroyAndWater()
	{
		if (this.destroying)
		{
			return;
		}
		this.destroying = true;
		if (this.destroyFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, base.transform.rotation);
		}
		if (this.wateringRadius > 0f)
		{
			SphereOverlapTrigger.CreateGameObject(base.transform.position, this.wateringRadius, delegate(IEnumerable<Collider> colliders)
			{
				HashSet<LiquidConsumer> hashSet = new HashSet<LiquidConsumer>();
				foreach (Collider collider in colliders)
				{
					LiquidConsumer[] array;
					if (collider.isTrigger)
					{
						array = collider.gameObject.GetComponents<LiquidConsumer>();
					}
					else
					{
						array = collider.gameObject.GetComponentsInParent<LiquidConsumer>();
					}
					foreach (LiquidConsumer item in array)
					{
						hashSet.Add(item);
					}
				}
				foreach (LiquidConsumer liquidConsumer in hashSet)
				{
					liquidConsumer.AddLiquid(this.liquidType, this.wateringUnits);
				}
			}, 4);
		}
		Destroyer.DestroyActor(base.gameObject, "DestroyOnTouching.DestroyAndWater", false);
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0004D2EB File Offset: 0x0004B4EB
	public void OnCollisionEnter(Collision col)
	{
		if (this.IsDestructiveContact(col) && this.destructiveContacts.Add(col.gameObject))
		{
			this.UpdateDestroyTime();
		}
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x0004D30F File Offset: 0x0004B50F
	public void OnCollisionExit(Collision col)
	{
		if (this.IsDestructiveContact(col) && this.destructiveContacts.Remove(col.gameObject))
		{
			this.UpdateDestroyTime();
		}
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x0004D334 File Offset: 0x0004B534
	public void OnTriggerEnter(Collider col)
	{
		LiquidSource component = col.gameObject.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId) && this.waterSources.Add(component))
		{
			this.UpdateDestroyTime();
		}
		AshSafetyZone component2 = col.gameObject.GetComponent<AshSafetyZone>();
		if (component2 != null && this.ashSources.Add(component2))
		{
			this.UpdateDestroyTime();
		}
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x0004D3A0 File Offset: 0x0004B5A0
	public void OnTriggerExit(Collider col)
	{
		LiquidSource component = col.gameObject.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId) && this.waterSources.Remove(component))
		{
			this.UpdateDestroyTime();
		}
		AshSafetyZone component2 = col.gameObject.GetComponent<AshSafetyZone>();
		if (component2 != null && this.ashSources.Remove(component2))
		{
			this.UpdateDestroyTime();
		}
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x0004D40C File Offset: 0x0004B60C
	private void UpdateDestroyFX(SlimeAppearance appearance)
	{
		if (appearance.DeathAppearance != null)
		{
			this.destroyFX = appearance.DeathAppearance.deathFX;
		}
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x0004D430 File Offset: 0x0004B630
	private void UpdateDestroyTime()
	{
		this.destructiveContacts.RemoveWhere((GameObject c) => c == null);
		this.waterSources.RemoveWhere((LiquidSource w) => w == null || w.gameObject == null);
		this.ashSources.RemoveWhere((AshSafetyZone a) => a == null || a.gameObject == null);
		bool flag = (this.hasPlexer ? (!this.plexer.IsGrounded() || this.destructiveContacts.Count == 0) : (this.destructiveContacts.Count == 0)) && (this.touchingWaterOkay || this.waterSources.Count <= 0);
		bool flag2 = this.touchingAshOkay && this.ashSources.Count > 0;
		bool flag3 = this.touchingWaterOkay && this.waterSources.Count > 0;
		bool flag4 = !flag && !flag2 && !flag3;
		if (!double.IsPositiveInfinity(this.destroyAt) || !flag4)
		{
			if (!double.IsPositiveInfinity(this.destroyAt) && !flag4)
			{
				this.destroyAt = double.PositiveInfinity;
			}
			return;
		}
		if (this.hoursOfContactAllowed <= 0f)
		{
			base.StartCoroutine(this.DestroyAndWaterAtEndOfFrame());
			return;
		}
		this.destroyAt = this.timeDir.HoursFromNowOrStart(this.hoursOfContactAllowed);
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x0004D5B2 File Offset: 0x0004B7B2
	private IEnumerator DestroyAndWaterAtEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		this.DestroyAndWater();
		yield break;
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x0004D5C1 File Offset: 0x0004B7C1
	public float PctTimeToDestruct()
	{
		return Mathf.Clamp01((float)((this.destroyAt - this.timeDir.WorldTime()) / (double)(3600f * this.hoursOfContactAllowed)));
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x0004D5E9 File Offset: 0x0004B7E9
	private bool IsDestructiveContact(Collision col)
	{
		return (!this.touchingWaterOkay || DestroyOnTouching.IsNonWater(col)) && (!this.touchingAshOkay || DestroyOnTouching.IsNonAsh(col));
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x0004D610 File Offset: 0x0004B810
	private static bool IsNonWater(Collision col)
	{
		LiquidSource component = col.gameObject.GetComponent<LiquidSource>();
		Identifiable component2 = col.gameObject.GetComponent<Identifiable>();
		bool flag = component != null && Identifiable.IsWater(component.liquidId);
		bool flag2 = component2 != null && (component2.id == Identifiable.Id.PUDDLE_PLORT || component2.id == Identifiable.Id.PUDDLE_SLIME || Identifiable.IsWater(component2.id));
		return !flag && !flag2;
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x0004D684 File Offset: 0x0004B884
	private static bool IsNonAsh(Collision col)
	{
		UnityEngine.Object component = col.gameObject.GetComponent<AshSource>();
		Identifiable component2 = col.gameObject.GetComponent<Identifiable>();
		bool flag = component != null;
		bool flag2 = component2 != null && (component2.id == Identifiable.Id.FIRE_PLORT || component2.id == Identifiable.Id.FIRE_SLIME);
		return !flag && !flag2;
	}

	// Token: 0x040012A3 RID: 4771
	[Tooltip("How long, in hours, we can contact one or more non-water objects before poofing.")]
	public float hoursOfContactAllowed;

	// Token: 0x040012A4 RID: 4772
	[Tooltip("When we poof, how large an area is watered.")]
	public float wateringRadius;

	// Token: 0x040012A5 RID: 4773
	[Tooltip("Amount to water each thing in radius when we poof.")]
	public float wateringUnits = 3f;

	// Token: 0x040012A6 RID: 4774
	[Tooltip("The effect to play when we poof.")]
	public GameObject destroyFX;

	// Token: 0x040012A7 RID: 4775
	[Tooltip("Should we destroy only if touching a non-water object?")]
	public bool touchingWaterOkay = true;

	// Token: 0x040012A8 RID: 4776
	[Tooltip("Should we destroy only if touching a non-ash object? Note: Does not include toys.")]
	public bool touchingAshOkay;

	// Token: 0x040012A9 RID: 4777
	[Tooltip("Should we destroy if touching an actor even when in the water. Note: Does not include toys.")]
	public bool reactToActors;

	// Token: 0x040012AA RID: 4778
	[Tooltip("The type of liquid we should spread on destruction.")]
	public Identifiable.Id liquidType = Identifiable.Id.WATER_LIQUID;

	// Token: 0x040012AB RID: 4779
	private double destroyAt = double.PositiveInfinity;

	// Token: 0x040012AC RID: 4780
	private HashSet<GameObject> destructiveContacts = new HashSet<GameObject>();

	// Token: 0x040012AD RID: 4781
	private HashSet<LiquidSource> waterSources = new HashSet<LiquidSource>();

	// Token: 0x040012AE RID: 4782
	private HashSet<AshSafetyZone> ashSources = new HashSet<AshSafetyZone>();

	// Token: 0x040012AF RID: 4783
	private TimeDirector timeDir;

	// Token: 0x040012B0 RID: 4784
	private bool destroying;

	// Token: 0x040012B1 RID: 4785
	private SlimeSubbehaviourPlexer plexer;

	// Token: 0x040012B2 RID: 4786
	private bool hasPlexer;

	// Token: 0x040012B3 RID: 4787
	private SlimeAppearanceApplicator slimeAppearanceApplicator;
}
