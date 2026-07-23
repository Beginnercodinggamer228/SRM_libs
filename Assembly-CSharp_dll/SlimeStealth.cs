using System;
using UnityEngine;

// Token: 0x02000493 RID: 1171
public class SlimeStealth : RegisteredActorBehaviour, RegistryUpdateable, SpawnListener
{
	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x06001850 RID: 6224 RVA: 0x0005E240 File Offset: 0x0005C440
	public bool IsStealthed
	{
		get
		{
			return this.currentOpacity < 1f;
		}
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x0005E250 File Offset: 0x0005C450
	public void Awake()
	{
		this.stealthController = new MaterialStealthController(base.gameObject);
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		this.vacuumable = base.GetComponent<Vacuumable>();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		if (this.slimeAppearanceApplicator.Appearance != null)
		{
			this.UpdateMaterialStealthController();
		}
		if (this.slimeAppearanceApplicator != null)
		{
			this.slimeAppearanceApplicator.OnAppearanceChanged += delegate(SlimeAppearance appearance)
			{
				this.UpdateMaterialStealthController();
			};
		}
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x0005E2D0 File Offset: 0x0005C4D0
	public void RegistryUpdate()
	{
		this.UpdateStealthOpacity();
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x0005E2D8 File Offset: 0x0005C4D8
	public void DidSpawn()
	{
		this.currentOpacity = 0f;
		this.initStealthUntil = Time.time + 5f;
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x0005E2F8 File Offset: 0x0005C4F8
	public void SetStealth(bool stealth)
	{
		this.targetOpacity = (stealth ? 0f : 1f);
		this.slimeAudio.Play(stealth ? this.slimeAudio.slimeSounds.cloakCue : this.slimeAudio.slimeSounds.decloakCue);
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x0005E34A File Offset: 0x0005C54A
	private void SetOpacity(float opacity)
	{
		this.stealthController.SetOpacity(opacity);
		this.lastOpacity = opacity;
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x0005E35F File Offset: 0x0005C55F
	private void UpdateMaterialStealthController()
	{
		this.stealthController.UpdateMaterials(base.gameObject);
		this.lastOpacity = 1f;
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x0005E380 File Offset: 0x0005C580
	private void UpdateStealthOpacity()
	{
		float num = (Time.time < this.initStealthUntil) ? 0f : this.targetOpacity;
		if (num > this.currentOpacity)
		{
			this.currentOpacity = Mathf.Min(num, this.currentOpacity + 2f * Time.deltaTime);
		}
		else if (this.targetOpacity < this.currentOpacity)
		{
			this.currentOpacity = Mathf.Max(num, this.currentOpacity - 2f * Time.deltaTime);
		}
		float num2 = this.vacuumable.isHeld() ? 1f : this.currentOpacity;
		if (Math.Abs(num2 - this.lastOpacity) > 0.001f)
		{
			this.SetOpacity(num2);
		}
	}

	// Token: 0x040017E4 RID: 6116
	private const float STEALTH_INIT_TIME = 5f;

	// Token: 0x040017E5 RID: 6117
	private const float OPACITY_CHANGE_PER_SEC = 2f;

	// Token: 0x040017E6 RID: 6118
	private const float STEALTH_OPACITY = 0f;

	// Token: 0x040017E7 RID: 6119
	private const float OPACITY_CHANGE_TOLERANCE = 0.001f;

	// Token: 0x040017E8 RID: 6120
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x040017E9 RID: 6121
	private Vacuumable vacuumable;

	// Token: 0x040017EA RID: 6122
	private SlimeAudio slimeAudio;

	// Token: 0x040017EB RID: 6123
	private MaterialStealthController stealthController;

	// Token: 0x040017EC RID: 6124
	private float initStealthUntil;

	// Token: 0x040017ED RID: 6125
	private float currentOpacity = 1f;

	// Token: 0x040017EE RID: 6126
	private float targetOpacity = 1f;

	// Token: 0x040017EF RID: 6127
	private float lastOpacity = 1f;
}
