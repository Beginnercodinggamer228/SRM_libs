using System;
using UnityEngine;

// Token: 0x020002DE RID: 734
public class VacColorAnimator : MonoBehaviour
{
	// Token: 0x06000FAC RID: 4012 RVA: 0x0003DD20 File Offset: 0x0003BF20
	public void Awake()
	{
		this.vacSpiralMat = this.spiralRenderer.material;
		this.vacDialMat = this.dialRenderer.material;
		this.ranchDir = SRSingleton<SceneContext>.Instance.RanchDirector;
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.slimeAppearanceDirector = SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector;
		this.ranchDir.RegisterVacRecolorMat(this.vacSpiralMat);
		this.ranchDir.RegisterVacRecolorMat(this.vacDialMat);
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x0003DDB4 File Offset: 0x0003BFB4
	public void OnDestroy()
	{
		this.ranchDir.UnregisterVacRecolorMat(this.vacSpiralMat);
		this.ranchDir.UnregisterVacRecolorMat(this.vacDialMat);
		Destroyer.Destroy(this.vacSpiralMat, "VacColorAnimator.OnDestroy#1");
		Destroyer.Destroy(this.vacDialMat, "VacColorAnimator.OnDestroy#2");
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0003DE03 File Offset: 0x0003C003
	public void SetVacActive(bool isActive)
	{
		this.isActive = isActive;
		this.UpdateColorTarget();
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x0003DE12 File Offset: 0x0003C012
	public void SetVacMode(bool inVacMode)
	{
		this.inVacMode = inVacMode;
		this.UpdateColorTarget();
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x0003DE24 File Offset: 0x0003C024
	public void Update()
	{
		if (this.colorVal > this.colorTarget)
		{
			this.colorVal = Mathf.Max(this.colorTarget, this.colorVal - Time.deltaTime * 2f);
		}
		else if (this.colorVal < this.colorTarget)
		{
			this.colorVal = Mathf.Min(this.colorTarget, this.colorVal + Time.deltaTime * 2f);
		}
		float selectedFullness = this.playerState.Ammo.GetSelectedFullness();
		Color value = Color.black;
		GameObject selectedStored = this.playerState.Ammo.GetSelectedStored();
		if (selectedStored != null)
		{
			Identifiable component = selectedStored.GetComponent<Identifiable>();
			if (component != null)
			{
				value = this.GetCurrentColor(component.id);
			}
		}
		this.vacSpiralMat.SetFloat(VacColorAnimator.PROPERTY_SPIRAL_COLOR, this.colorVal);
		this.vacDialMat.SetFloat(VacColorAnimator.PROPERTY_SPIRAL_COLOR, this.colorVal);
		this.vacDialMat.SetFloat(VacColorAnimator.PROPERTY_AMMO_FULLNESS, selectedFullness);
		this.vacDialMat.SetColor(VacColorAnimator.PROPERTY_AMMO_COLOR, value);
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x0003DF31 File Offset: 0x0003C131
	private void UpdateColorTarget()
	{
		this.colorTarget = (this.isActive ? (this.inVacMode ? 0f : 1f) : 0.5f);
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0003DF5C File Offset: 0x0003C15C
	private Color GetCurrentColor(Identifiable.Id id)
	{
		if (Identifiable.IsSlime(id))
		{
			return this.slimeAppearanceDirector.GetChosenSlimeAppearance(id).ColorPalette.Ammo;
		}
		if (id != Identifiable.Id.NONE)
		{
			return this.lookupDir.GetColor(id);
		}
		return Color.clear;
	}

	// Token: 0x04000E6A RID: 3690
	public Renderer spiralRenderer;

	// Token: 0x04000E6B RID: 3691
	public Renderer dialRenderer;

	// Token: 0x04000E6C RID: 3692
	private bool isActive;

	// Token: 0x04000E6D RID: 3693
	private bool inVacMode;

	// Token: 0x04000E6E RID: 3694
	private float colorTarget = 0.5f;

	// Token: 0x04000E6F RID: 3695
	private float colorVal = 0.5f;

	// Token: 0x04000E70 RID: 3696
	private Material vacSpiralMat;

	// Token: 0x04000E71 RID: 3697
	private Material vacDialMat;

	// Token: 0x04000E72 RID: 3698
	private PlayerState playerState;

	// Token: 0x04000E73 RID: 3699
	private LookupDirector lookupDir;

	// Token: 0x04000E74 RID: 3700
	private RanchDirector ranchDir;

	// Token: 0x04000E75 RID: 3701
	public SlimeAppearanceDirector slimeAppearanceDirector;

	// Token: 0x04000E76 RID: 3702
	private static readonly int PROPERTY_SPIRAL_COLOR = Shader.PropertyToID("_SpiralColor");

	// Token: 0x04000E77 RID: 3703
	private static readonly int PROPERTY_AMMO_FULLNESS = Shader.PropertyToID("_AmmoFullness");

	// Token: 0x04000E78 RID: 3704
	private static readonly int PROPERTY_AMMO_COLOR = Shader.PropertyToID("_AmmoColor");

	// Token: 0x04000E79 RID: 3705
	private const float SECS_TO_TRANSITION = 0.5f;

	// Token: 0x04000E7A RID: 3706
	private const float TRANS_PER_SEC = 2f;
}
