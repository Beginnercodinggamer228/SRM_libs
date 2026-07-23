using System;
using UnityEngine;

// Token: 0x020002AE RID: 686
public class Overlay : SRSingleton<Overlay>
{
	// Token: 0x06000E91 RID: 3729 RVA: 0x0003AC84 File Offset: 0x00038E84
	public void Update()
	{
		if (this.activeRadFX != null)
		{
			Color color = this.activeRadMat.color;
			float num = color.a;
			if (this.tgtRadAlpha > num)
			{
				num = Mathf.Min(this.tgtRadAlpha, num + 2f * Time.deltaTime);
			}
			else if (this.tgtRadAlpha < num)
			{
				num = Mathf.Max(this.tgtRadAlpha, num - 2f * Time.deltaTime);
			}
			color.a = num;
			this.activeRadMat.color = color;
			if (num <= 0f)
			{
				Destroyer.Destroy(this.activeRadFX, "Overlay.Update#1");
				Destroyer.Destroy(this.activeRadMat, "Overlay.Update#2");
				this.activeRadFX = null;
			}
		}
		if (this.activeFirestormFX != null)
		{
			Color color2 = this.activeFirestormMat.color;
			float num2 = color2.a;
			if (this.tgtFirestormAlpha > num2)
			{
				num2 = Mathf.Min(this.tgtFirestormAlpha, num2 + 2f * Time.deltaTime);
			}
			else if (this.tgtFirestormAlpha < num2)
			{
				num2 = Mathf.Max(this.tgtFirestormAlpha, num2 - 2f * Time.deltaTime);
			}
			color2.a = num2;
			this.activeFirestormMat.color = color2;
			if (num2 <= 0f)
			{
				Destroyer.Destroy(this.activeFirestormFX, "Overlay.Update#3");
				Destroyer.Destroy(this.activeFirestormMat, "Overlay.Update#4");
				this.activeFirestormFX = null;
			}
		}
		if (this.activeGadgetFX != null)
		{
			Color color3 = this.activeGadgetMat.color;
			float num3 = color3.a;
			if (this.tgtGadgetAlpha > num3)
			{
				num3 = Mathf.Min(this.tgtGadgetAlpha, num3 + 2f * Time.deltaTime);
			}
			else if (this.tgtGadgetAlpha < num3)
			{
				num3 = Mathf.Max(this.tgtGadgetAlpha, num3 - 2f * Time.deltaTime);
			}
			color3.a = num3;
			this.activeGadgetMat.color = color3;
			if (num3 <= 0f)
			{
				Destroyer.Destroy(this.activeGadgetFX, "Overlay.Update#5");
				Destroyer.Destroy(this.activeGadgetMat, "Overlay.Update#6");
				this.activeGadgetFX = null;
			}
		}
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x0003AEA4 File Offset: 0x000390A4
	public void PlayTeleport()
	{
		this.Play(this.teleportFX);
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x0003AEB3 File Offset: 0x000390B3
	public void PlayDamage()
	{
		this.Play(this.damageFX);
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x0003AEC2 File Offset: 0x000390C2
	public void PlayChomp()
	{
		this.Play(this.chompFX);
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x0003AED4 File Offset: 0x000390D4
	public void SetEnableRad(bool enabled)
	{
		this.tgtRadAlpha = (enabled ? 1f : 0f);
		if (enabled && this.activeRadFX == null)
		{
			this.activeRadFX = this.Play(this.radFX);
			this.activeRadMat = this.activeRadFX.GetComponent<Renderer>().material;
			Color color = this.activeRadMat.color;
			color.a = 0f;
			this.activeRadMat.color = color;
		}
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0003AF54 File Offset: 0x00039154
	public void SetEnableFirestorm(bool enabled)
	{
		this.tgtFirestormAlpha = (enabled ? 1f : 0f);
		if (enabled && this.activeFirestormFX == null)
		{
			this.activeFirestormFX = this.Play(this.firestormFX);
			this.activeFirestormMat = this.activeFirestormFX.GetComponent<Renderer>().material;
			Color color = this.activeFirestormMat.color;
			color.a = 0f;
			this.activeFirestormMat.color = color;
		}
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x0003AFD4 File Offset: 0x000391D4
	public void SetEnableGadgetMode(bool enabled)
	{
		this.tgtGadgetAlpha = (enabled ? 1f : 0f);
		if (enabled && this.activeGadgetFX == null)
		{
			this.activeGadgetFX = this.Play(this.gadgetFX);
			this.activeGadgetMat = this.activeGadgetFX.GetComponent<Renderer>().material;
			Color color = this.activeGadgetMat.color;
			color.a = 0f;
			this.activeGadgetMat.color = color;
		}
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x0003B053 File Offset: 0x00039253
	public GameObject Play(GameObject fxOrig)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(fxOrig, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = base.transform;
		SRBehaviour.PlayFX(gameObject);
		return gameObject;
	}

	// Token: 0x04000DA2 RID: 3490
	public GameObject teleportFX;

	// Token: 0x04000DA3 RID: 3491
	public GameObject damageFX;

	// Token: 0x04000DA4 RID: 3492
	public GameObject chompFX;

	// Token: 0x04000DA5 RID: 3493
	public GameObject radFX;

	// Token: 0x04000DA6 RID: 3494
	public GameObject firestormFX;

	// Token: 0x04000DA7 RID: 3495
	public GameObject gadgetFX;

	// Token: 0x04000DA8 RID: 3496
	[Tooltip("FX played while the dash pad is active.")]
	public GameObject dashPadFX;

	// Token: 0x04000DA9 RID: 3497
	private GameObject activeRadFX;

	// Token: 0x04000DAA RID: 3498
	private Material activeRadMat;

	// Token: 0x04000DAB RID: 3499
	private float tgtRadAlpha;

	// Token: 0x04000DAC RID: 3500
	private GameObject activeFirestormFX;

	// Token: 0x04000DAD RID: 3501
	private Material activeFirestormMat;

	// Token: 0x04000DAE RID: 3502
	private float tgtFirestormAlpha;

	// Token: 0x04000DAF RID: 3503
	private GameObject activeGadgetFX;

	// Token: 0x04000DB0 RID: 3504
	private Material activeGadgetMat;

	// Token: 0x04000DB1 RID: 3505
	private float tgtGadgetAlpha;

	// Token: 0x04000DB2 RID: 3506
	private const float RAD_ALPHA_DELTA = 2f;

	// Token: 0x04000DB3 RID: 3507
	private const float FIRESTORM_ALPHA_DELTA = 2f;

	// Token: 0x04000DB4 RID: 3508
	private const float GADGET_ALPHA_DELTA = 2f;
}
