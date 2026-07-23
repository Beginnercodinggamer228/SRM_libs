using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020001AF RID: 431
[RequireComponent(typeof(DroneStation))]
public class DroneStationBattery : SRBehaviour, LiquidConsumer, GadgetModel.Participant
{
	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000910 RID: 2320 RVA: 0x00028E30 File Offset: 0x00027030
	// (remove) Token: 0x06000911 RID: 2321 RVA: 0x00028E68 File Offset: 0x00027068
	public event DroneStationBattery.OnReset onReset;

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x06000912 RID: 2322 RVA: 0x00028EA0 File Offset: 0x000270A0
	// (remove) Token: 0x06000913 RID: 2323 RVA: 0x00028ED8 File Offset: 0x000270D8
	public event DroneStationBattery.OnHasAnyChanged onHasAnyChanged;

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000914 RID: 2324 RVA: 0x00028F0D File Offset: 0x0002710D
	// (set) Token: 0x06000915 RID: 2325 RVA: 0x00028F15 File Offset: 0x00027115
	public DroneStation station { get; private set; }

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000916 RID: 2326 RVA: 0x00028F1E File Offset: 0x0002711E
	public double Time
	{
		get
		{
			return this.droneModel.batteryDepleteTime;
		}
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000917 RID: 2327 RVA: 0x00028F2B File Offset: 0x0002712B
	// (set) Token: 0x06000918 RID: 2328 RVA: 0x00028F3D File Offset: 0x0002713D
	private float percentage
	{
		get
		{
			return this.meter.localScale.y;
		}
		set
		{
			this.meter.localScale = new Vector3(1f, value, 1f);
		}
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00028F5A File Offset: 0x0002715A
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.station = base.GetComponent<DroneStation>();
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00028F78 File Offset: 0x00027178
	public void InitModel(GadgetModel droneModel)
	{
		this.Reset((DroneModel)droneModel);
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00028F86 File Offset: 0x00027186
	public void SetModel(GadgetModel droneModel)
	{
		this.droneModel = (DroneModel)droneModel;
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00028F94 File Offset: 0x00027194
	public void AddLiquid(Identifiable.Id id, float units)
	{
		if (this.percentage < 1f && this.timeDirector.HasReached(this.fxCooldownTime))
		{
			SECTR_AudioSystem.Play(this.station.gadget.metadata.onBatteryFilledCue, base.transform.position, false);
			this.fxCooldownTime = this.timeDirector.HoursFromNow(0.050000004f);
			if (this.station.gadget.metadata.onBatteryFilledFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.station.gadget.metadata.onBatteryFilledFX, base.gameObject);
			}
		}
		this.Reset(this.droneModel);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0002904C File Offset: 0x0002724C
	public void Update()
	{
		this.percentage = Mathf.Clamp01((float)((this.Time - this.timeDirector.WorldTime()) / 100800.0));
		bool? flag = this.previousHasAny;
		bool flag2 = this.HasAny();
		if (!(flag.GetValueOrDefault() == flag2 & flag != null))
		{
			this.previousHasAny = new bool?(this.HasAny());
			if (this.onHasAnyChanged != null)
			{
				this.onHasAnyChanged();
			}
		}
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x000290C7 File Offset: 0x000272C7
	public bool HasAny()
	{
		return this.percentage > 0f;
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x000290D6 File Offset: 0x000272D6
	private void Reset(DroneModel droneModel)
	{
		this.percentage = 1f;
		droneModel.batteryDepleteTime = this.timeDirector.HoursFromNow(28f);
		if (this.onReset != null)
		{
			this.onReset();
		}
	}

	// Token: 0x040007A8 RID: 1960
	[Tooltip("Battery meter transform.")]
	public Transform meter;

	// Token: 0x040007AC RID: 1964
	private const float DURATION_HOURS = 28f;

	// Token: 0x040007AD RID: 1965
	private const float DURATION_SECONDS = 100800f;

	// Token: 0x040007AE RID: 1966
	private TimeDirector timeDirector;

	// Token: 0x040007AF RID: 1967
	private bool? previousHasAny;

	// Token: 0x040007B0 RID: 1968
	private double fxCooldownTime;

	// Token: 0x040007B1 RID: 1969
	private DroneModel droneModel;

	// Token: 0x020001B0 RID: 432
	// (Invoke) Token: 0x06000922 RID: 2338
	public delegate void OnReset();

	// Token: 0x020001B1 RID: 433
	// (Invoke) Token: 0x06000926 RID: 2342
	public delegate void OnHasAnyChanged();
}
