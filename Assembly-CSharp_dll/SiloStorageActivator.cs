using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000344 RID: 836
public class SiloStorageActivator : MonoBehaviour, TechActivator, LandPlotModel.Participant
{
	// Token: 0x0600118A RID: 4490 RVA: 0x00046792 File Offset: 0x00044992
	public void Awake()
	{
		this.buttonAnimator = base.GetComponentInParent<Animator>();
		this.buttonAnimation = Animator.StringToHash("ButtonPressed");
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(LandPlotModel model)
	{
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x000467B0 File Offset: 0x000449B0
	public void SetModel(LandPlotModel model)
	{
		this.landPlotModel = model;
		this.OnActiveSlotChanged();
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x000467BF File Offset: 0x000449BF
	public void OnEnable()
	{
		if (this.landPlotModel != null)
		{
			this.OnActiveSlotChanged();
		}
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x000467D0 File Offset: 0x000449D0
	public void Activate()
	{
		if (this.nextActivationTime < Time.time)
		{
			this.nextActivationTime = Time.time + 0.4f;
			this.landPlotModel.siloStorageIndices[this.activatorIdx] = (this.landPlotModel.siloStorageIndices[this.activatorIdx] + 1) % this.siloSlotUIs.Count;
			SECTR_AudioSystem.Play(this.onPressButtonCue, base.transform.position, false);
			this.buttonAnimator.SetTrigger(this.buttonAnimation);
			this.OnActiveSlotChanged();
		}
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x00025E60 File Offset: 0x00024060
	public GameObject GetCustomGuiPrefab()
	{
		return null;
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0004685C File Offset: 0x00044A5C
	private void OnActiveSlotChanged()
	{
		int num = this.landPlotModel.siloStorageIndices[this.activatorIdx];
		this.siloCatcher.slotIdx = this.siloSlotUIs[num].slotIdx;
		if (this.siloSlotAnimator.gameObject.activeInHierarchy)
		{
			this.siloSlotAnimator.SetInteger("slot", num);
		}
	}

	// Token: 0x040010C2 RID: 4290
	[Tooltip("SiloCatcher script to update on slot changed.")]
	public SiloCatcher siloCatcher;

	// Token: 0x040010C3 RID: 4291
	[Tooltip("SiloSlotUI scripts to update on slot changed.")]
	public List<SiloSlotUI> siloSlotUIs;

	// Token: 0x040010C4 RID: 4292
	[Tooltip("Animator to control the active silo slot.")]
	public Animator siloSlotAnimator;

	// Token: 0x040010C5 RID: 4293
	[Tooltip("SFX played when the button is pressed. (optional)")]
	public SECTR_AudioCue onPressButtonCue;

	// Token: 0x040010C6 RID: 4294
	[Tooltip("Where we fall in the activator order")]
	public int activatorIdx;

	// Token: 0x040010C7 RID: 4295
	private Animator buttonAnimator;

	// Token: 0x040010C8 RID: 4296
	private int buttonAnimation;

	// Token: 0x040010C9 RID: 4297
	private const float TIME_BETWEEN_ACTIVATIONS = 0.4f;

	// Token: 0x040010CA RID: 4298
	private float nextActivationTime;

	// Token: 0x040010CB RID: 4299
	private LandPlotModel landPlotModel;
}
