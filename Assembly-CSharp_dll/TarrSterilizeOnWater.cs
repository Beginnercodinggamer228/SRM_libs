using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020004AB RID: 1195
public class TarrSterilizeOnWater : MonoBehaviour, LiquidConsumer
{
	// Token: 0x060018ED RID: 6381 RVA: 0x000610F0 File Offset: 0x0005F2F0
	public virtual void Awake()
	{
		this.destroyer = base.GetComponent<DestroyAfterTime>();
		this.slimeEat = base.GetComponent<SlimeEat>();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		base.GetComponent<SlimeAppearanceApplicator>().OnAppearanceChanged += this.CollectFlashRenderers;
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x0006113C File Offset: 0x0005F33C
	public virtual void Start()
	{
		this.CollectFlashRenderers(null);
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x00061145 File Offset: 0x0005F345
	private void CollectFlashRenderers(SlimeAppearance newAppearance)
	{
		this.flashRenderers = (from marker in base.GetComponentsInChildren<TarrFlashMarker>()
		select marker.GetComponent<Renderer>()).ToList<Renderer>();
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x0006117C File Offset: 0x0005F37C
	public void OnTriggerEnter(Collider col)
	{
		LiquidSource component = col.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId))
		{
			this.inWaterCount++;
		}
		if (col.GetComponent<Oasis>() != null)
		{
			this.inWaterCount++;
		}
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x000611D0 File Offset: 0x0005F3D0
	public void OnTriggerExit(Collider col)
	{
		LiquidSource component = col.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId))
		{
			this.inWaterCount--;
		}
		if (col.GetComponent<Oasis>() != null)
		{
			this.inWaterCount--;
		}
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x00061224 File Offset: 0x0005F424
	public void Update()
	{
		if (this.inWaterCount > 0 && this.timeDir.HasReached(this.nextInWaterCheck))
		{
			this.AddLiquid(Identifiable.Id.WATER_LIQUID, 1f);
			this.nextInWaterCheck = this.timeDir.HoursFromNow(0.016666668f);
		}
		if (this.flashFactor > 0f && this.sterileMat != null)
		{
			this.flashFactor = Mathf.Max(0f, this.flashFactor - Time.deltaTime * 2f);
			this.sterileMat.SetFloat("_HitFlash", this.flashFactor);
		}
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x000612C4 File Offset: 0x0005F4C4
	public void AddLiquid(Identifiable.Id liquidId, float units)
	{
		if (Identifiable.IsLiquid(liquidId))
		{
			if (!this.sterilized)
			{
				this.sterilized = true;
				this.SetSterilized();
				this.destroyer.MultiplyRemainingHours(this.timeRemainingFactor);
			}
			this.FlashHit();
			float num = (float)((liquidId == Identifiable.Id.MAGIC_WATER_LIQUID) ? 10 : 1);
			this.destroyer.AdvanceHours(this.hoursPerHit * units * num);
		}
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x00061326 File Offset: 0x0005F526
	public void OnDestroy()
	{
		if (this.sterileMat != null)
		{
			Destroyer.Destroy(this.sterileMat, "TarrSterilizeOnWater.OnDestroy");
		}
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x00061346 File Offset: 0x0005F546
	private void FlashHit()
	{
		this.flashFactor = 1f;
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x00061354 File Offset: 0x0005F554
	private void SetSterilized()
	{
		this.slimeEat.chanceToSkipProduce = 1f;
		if (this.sterileMat != null)
		{
			Log.Warning("Already have a sterile material", Array.Empty<object>());
		}
		Material material = null;
		foreach (Renderer renderer in this.flashRenderers)
		{
			if (material == null)
			{
				material = renderer.material;
				this.sterileMat = material;
				material.SetTexture(TarrSterilizeOnWater.ColorRampPropertyId, this.sterileRampTex);
			}
			renderer.material = material;
		}
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x00061400 File Offset: 0x0005F600
	public void FromSerializable(bool sterilized)
	{
		this.sterilized = sterilized;
		if (sterilized)
		{
			this.SetSterilized();
		}
	}

	// Token: 0x060018F8 RID: 6392 RVA: 0x00061412 File Offset: 0x0005F612
	public void ToSerializable(out bool sterilized)
	{
		sterilized = this.sterilized;
	}

	// Token: 0x040018A4 RID: 6308
	private static readonly int ColorRampPropertyId = Shader.PropertyToID("_ColorRamp");

	// Token: 0x040018A5 RID: 6309
	public float timeRemainingFactor = 0.5f;

	// Token: 0x040018A6 RID: 6310
	public float hoursPerHit = 0.1f;

	// Token: 0x040018A7 RID: 6311
	public Texture sterileRampTex;

	// Token: 0x040018A8 RID: 6312
	protected bool sterilized;

	// Token: 0x040018A9 RID: 6313
	protected DestroyAfterTime destroyer;

	// Token: 0x040018AA RID: 6314
	private SlimeEat slimeEat;

	// Token: 0x040018AB RID: 6315
	private Material sterileMat;

	// Token: 0x040018AC RID: 6316
	protected TimeDirector timeDir;

	// Token: 0x040018AD RID: 6317
	private int inWaterCount;

	// Token: 0x040018AE RID: 6318
	private double nextInWaterCheck;

	// Token: 0x040018AF RID: 6319
	private float flashFactor;

	// Token: 0x040018B0 RID: 6320
	private const float TIME_BETWEEN_CHECKS = 0.016666668f;

	// Token: 0x040018B1 RID: 6321
	private const float FLASH_DECAY_PER_SEC = 2f;

	// Token: 0x040018B2 RID: 6322
	private List<Renderer> flashRenderers;
}
