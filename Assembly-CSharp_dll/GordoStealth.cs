using System;
using UnityEngine;

// Token: 0x020003F6 RID: 1014
public class GordoStealth : MonoBehaviour
{
	// Token: 0x0600152E RID: 5422 RVA: 0x00052441 File Offset: 0x00050641
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.materialStealthController = new MaterialStealthController(base.gameObject);
		this.SetStealth(true);
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x0005246B File Offset: 0x0005066B
	public void OnTriggerEnter(Collider col)
	{
		if (col.GetComponentInParent<TrackCollisions>() != null)
		{
			this.vacTriggerCount++;
			if (this.vacTriggerCount == 1)
			{
				this.SetStealth(false);
			}
		}
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x00052499 File Offset: 0x00050699
	public void OnTriggerExit(Collider col)
	{
		if (col.GetComponentInParent<TrackCollisions>() != null)
		{
			this.vacTriggerCount--;
			if (this.vacTriggerCount == 0)
			{
				this.goStealthAt = this.timeDir.HoursFromNow(0.083333336f);
			}
		}
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x000524D5 File Offset: 0x000506D5
	public void Update()
	{
		if (!this.stealth && this.timeDir.HasReached(this.goStealthAt))
		{
			this.SetStealth(true);
		}
		this.UpdateStealthOpacity();
	}

	// Token: 0x06001532 RID: 5426 RVA: 0x00052500 File Offset: 0x00050700
	private void UpdateStealthOpacity()
	{
		if (this.tgtOpacity > this.currOpacity)
		{
			this.currOpacity = Mathf.Min(this.tgtOpacity, this.currOpacity + 2f * Time.deltaTime);
		}
		else if (this.tgtOpacity < this.currOpacity)
		{
			this.currOpacity = Mathf.Max(this.tgtOpacity, this.currOpacity - 2f * Time.deltaTime);
		}
		this.materialStealthController.SetOpacity(this.currOpacity);
	}

	// Token: 0x06001533 RID: 5427 RVA: 0x00052582 File Offset: 0x00050782
	private void SetStealth(bool stealth)
	{
		this.stealth = stealth;
		this.tgtOpacity = (stealth ? 0f : 1f);
		this.goStealthAt = double.PositiveInfinity;
	}

	// Token: 0x04001402 RID: 5122
	private int vacTriggerCount;

	// Token: 0x04001403 RID: 5123
	private bool stealth;

	// Token: 0x04001404 RID: 5124
	private double goStealthAt = double.PositiveInfinity;

	// Token: 0x04001405 RID: 5125
	private float tgtOpacity;

	// Token: 0x04001406 RID: 5126
	private float currOpacity;

	// Token: 0x04001407 RID: 5127
	private TimeDirector timeDir;

	// Token: 0x04001408 RID: 5128
	private MaterialStealthController materialStealthController;

	// Token: 0x04001409 RID: 5129
	private const float OPACITY_CHANGE_PER_SEC = 2f;

	// Token: 0x0400140A RID: 5130
	private const float STEALTH_OPACITY = 0f;

	// Token: 0x0400140B RID: 5131
	private const float STEALTH_DELAY_HRS = 0.083333336f;
}
