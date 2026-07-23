using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020004E1 RID: 1249
public class GlitchLiquidSource : LiquidSource
{
	// Token: 0x06001A26 RID: 6694 RVA: 0x00065CC5 File Offset: 0x00063EC5
	public override void Awake()
	{
		base.Awake();
		if (Application.isPlaying)
		{
			this.tutorialDirector = SRSingleton<SceneContext>.Instance.TutorialDirector;
		}
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x00065CE4 File Offset: 0x00063EE4
	protected override void InitModel(LiquidSourceModel model)
	{
		base.InitModel(model);
		model.unitsFilled = 1f;
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x00065CF8 File Offset: 0x00063EF8
	protected override void SetModel(LiquidSourceModel model)
	{
		base.SetModel(model);
		this.OnAvailabilityChanged();
	}

	// Token: 0x06001A29 RID: 6697 RVA: 0x00065D08 File Offset: 0x00063F08
	private void OnAvailabilityChanged()
	{
		bool active = this.Available();
		GameObject[] array = this.onAvailableFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(active);
		}
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x00065D3A File Offset: 0x00063F3A
	public override bool Available()
	{
		return base.Available() && this.model.unitsFilled > 0f;
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x00065D58 File Offset: 0x00063F58
	public override void ConsumeLiquid()
	{
		base.ConsumeLiquid();
		this.model.unitsFilled = 0f;
		this.tutorialDirector.MaybeShowPopup(TutorialDirector.Id.SLIMULATIONS_DEBUG_SPRAY);
		SRBehaviour.SpawnAndPlayFX(this.onConsumeFX, base.transform.position, Quaternion.identity);
		SECTR_AudioSystem.Play(this.onConsumeCue, base.transform.position, false);
		this.OnAvailabilityChanged();
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public override bool ReplacesExistingLiquidAmmo()
	{
		return true;
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x00065DC2 File Offset: 0x00063FC2
	public void ResetLiquidState()
	{
		this.model.unitsFilled = 1f;
		this.OnAvailabilityChanged();
	}

	// Token: 0x040019AC RID: 6572
	[Tooltip("FX objects that should be activated/deactivated based off the current state.")]
	public GameObject[] onAvailableFX;

	// Token: 0x040019AD RID: 6573
	[Tooltip("SFX played when the station is consumed. (once, non-looping)")]
	public SECTR_AudioCue onConsumeCue;

	// Token: 0x040019AE RID: 6574
	[Tooltip("FX spawned when the station is consumed.")]
	public GameObject onConsumeFX;

	// Token: 0x040019AF RID: 6575
	private TutorialDirector tutorialDirector;
}
