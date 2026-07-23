using System;
using UnityEngine;

// Token: 0x020002F3 RID: 755
public class AirNet : MonoBehaviour
{
	// Token: 0x06001023 RID: 4131 RVA: 0x00040CF0 File Offset: 0x0003EEF0
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.netCollider = base.GetComponent<Collider>();
		this.netMaterial = base.GetComponent<Renderer>().material;
		this.dmgPerImpulse = 1f / (float)this.hitForceToDestroy;
		this.recoverFactor = 1f / (this.hoursToRecover * 3600f);
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x00040D55 File Offset: 0x0003EF55
	public void OnDestroy()
	{
		Destroyer.Destroy(this.netMaterial, "AirNet.OnDestroy");
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x00040D68 File Offset: 0x0003EF68
	public void OnCollisionEnter(Collision col)
	{
		if (Identifiable.IsSlime(Identifiable.GetId(col.gameObject)))
		{
			float magnitude = col.impulse.magnitude;
			this.netStrength = Mathf.Max(0f, this.netStrength - magnitude * this.dmgPerImpulse);
			this.recoverStartTime = this.timeDir.HoursFromNow(this.hoursToRecover);
		}
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x00040DCC File Offset: 0x0003EFCC
	public void Update()
	{
		if (this.netStrength < 1f && this.timeDir.HasReached(this.recoverStartTime))
		{
			this.netStrength = Mathf.Clamp((float)((double)this.netStrength + this.timeDir.DeltaWorldTime() * (double)this.recoverFactor), 0.33f, 1f);
		}
		this.netCollider.enabled = (this.netStrength > 0f);
		this.netMaterial.color = this.CurrColor();
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x00040E53 File Offset: 0x0003F053
	private Color CurrColor()
	{
		if (this.netStrength <= 0f)
		{
			return Color.clear;
		}
		return Color.Lerp(this.brokenColor, this.fullColor, this.netStrength);
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x00040E7F File Offset: 0x0003F07F
	public bool IsNetActive()
	{
		return base.gameObject.activeInHierarchy && this.netStrength > 0f;
	}

	// Token: 0x04000EF5 RID: 3829
	public int hitForceToDestroy = 300;

	// Token: 0x04000EF6 RID: 3830
	public float hoursToStartRecovery = 0.1f;

	// Token: 0x04000EF7 RID: 3831
	public float hoursToRecover = 0.1f;

	// Token: 0x04000EF8 RID: 3832
	public Color fullColor = Color.white;

	// Token: 0x04000EF9 RID: 3833
	public Color brokenColor = Color.red;

	// Token: 0x04000EFA RID: 3834
	private TimeDirector timeDir;

	// Token: 0x04000EFB RID: 3835
	private Collider netCollider;

	// Token: 0x04000EFC RID: 3836
	private Material netMaterial;

	// Token: 0x04000EFD RID: 3837
	private float netStrength = 1f;

	// Token: 0x04000EFE RID: 3838
	private double recoverStartTime;

	// Token: 0x04000EFF RID: 3839
	private float dmgPerImpulse;

	// Token: 0x04000F00 RID: 3840
	private float recoverFactor;

	// Token: 0x04000F01 RID: 3841
	private const float NEW_NET_STRENGTH = 0.33f;
}
