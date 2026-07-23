using System;
using UnityEngine;

// Token: 0x020003CC RID: 972
public class FadeAndDestroySplat : MonoBehaviour
{
	// Token: 0x06001437 RID: 5175 RVA: 0x0004E1D8 File Offset: 0x0004C3D8
	public void Awake()
	{
		this.projector = base.GetComponentInChildren<Projector>();
		this.mat = this.GetMaterial();
		float num = this.TimeForParticlesLevel(SRQualitySettings.Particles);
		this.fadeStartTime = Time.time + num;
		this.fadeEndTime = this.fadeStartTime + this.fadeTime;
		this.fadeInStartTime = Time.time;
		this.fadeInEndTime = this.fadeInStartTime + this.fadeInTime;
		this.invFadeTime = 1f / this.fadeTime;
		if (this.textures.Length != 0)
		{
			this.mat.SetTexture("_DecalTex", Randoms.SHARED.Pick<Texture2D>(this.textures));
			this.mat.SetFloat("_Alpha", 0f);
		}
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x0004E298 File Offset: 0x0004C498
	private float TimeForParticlesLevel(SRQualitySettings.ParticlesLevel level)
	{
		switch (level)
		{
		case SRQualitySettings.ParticlesLevel.LOW:
			return this.minQualTimeBeforeFade;
		case SRQualitySettings.ParticlesLevel.MEDIUM:
			return (this.timeBeforeFade + this.minQualTimeBeforeFade) * 0.5f;
		case SRQualitySettings.ParticlesLevel.HIGH:
			return this.timeBeforeFade;
		default:
			Log.Warning("Unknown particles level: " + level, Array.Empty<object>());
			return this.minQualTimeBeforeFade;
		}
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x0004E2FC File Offset: 0x0004C4FC
	public void Update()
	{
		float time = Time.time;
		if (this.mat == null)
		{
			Log.Error("Updating splat for destroyed material.", Array.Empty<object>());
			SentrySdk.CaptureMessage("Attempting to update splat with destroyed material!");
			SRSingleton<SceneContext>.Instance.fxPool.Recycle(base.gameObject);
			return;
		}
		if (this.mat.shader == null)
		{
			Log.Error("Updating splat for material with destroyed shader.", Array.Empty<object>());
			SentrySdk.CaptureMessage("Attempting to update splat with destroyed shader!");
			SRSingleton<SceneContext>.Instance.fxPool.Recycle(base.gameObject);
			return;
		}
		if (time <= this.fadeInEndTime)
		{
			float value = Mathf.Lerp(0f, 1f, (time - this.fadeInStartTime) / this.fadeInTime);
			this.mat.SetFloat("_Alpha", value);
		}
		else if (time <= this.fadeStartTime)
		{
			float value = 1f;
			this.mat.SetFloat("_Alpha", value);
		}
		if (time >= this.fadeEndTime)
		{
			SRSingleton<SceneContext>.Instance.fxPool.Recycle(base.gameObject);
			return;
		}
		if (time > this.fadeStartTime)
		{
			float value = 1f - (time - this.fadeStartTime) * this.invFadeTime;
			this.mat.SetFloat("_Alpha", value);
			if (!this.hasBegunFade)
			{
				SECTR_AudioSystem.Play(this.onFadeBeginCue, base.transform.position, false);
				this.hasBegunFade = true;
			}
		}
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x0004E468 File Offset: 0x0004C668
	protected Material GetMaterial()
	{
		Material material = UnityEngine.Object.Instantiate<Material>(this.projector.material);
		this.projector.material = material;
		return material;
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x0004E493 File Offset: 0x0004C693
	public void SetScale(float scale)
	{
		this.projector.orthographicSize = scale * 0.5f;
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x0004E4A7 File Offset: 0x0004C6A7
	public void SetColors(Color topColor, Color midColor, Color btmColor)
	{
		this.mat.SetColor("_TopColor", topColor);
		this.mat.SetColor("_MiddleColor", midColor);
		this.mat.SetColor("_BottomColor", btmColor);
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x0004E4DC File Offset: 0x0004C6DC
	public void OnDestroy()
	{
		Destroyer.Destroy(this.mat, "FadeAndDestroySplat.OnDestroy");
	}

	// Token: 0x040012EF RID: 4847
	public float timeBeforeFade = 5f;

	// Token: 0x040012F0 RID: 4848
	public float minQualTimeBeforeFade = 1f;

	// Token: 0x040012F1 RID: 4849
	public float fadeTime = 1f;

	// Token: 0x040012F2 RID: 4850
	private float fadeInTime = 0.125f;

	// Token: 0x040012F3 RID: 4851
	public Texture2D[] textures;

	// Token: 0x040012F4 RID: 4852
	public GameObject splatFX;

	// Token: 0x040012F5 RID: 4853
	[Tooltip("SFX played when the fade begins.")]
	public SECTR_AudioCue onFadeBeginCue;

	// Token: 0x040012F6 RID: 4854
	private bool hasBegunFade;

	// Token: 0x040012F7 RID: 4855
	private float fadeStartTime;

	// Token: 0x040012F8 RID: 4856
	private float fadeEndTime;

	// Token: 0x040012F9 RID: 4857
	private float invFadeTime;

	// Token: 0x040012FA RID: 4858
	private float fadeInStartTime;

	// Token: 0x040012FB RID: 4859
	private float fadeInEndTime;

	// Token: 0x040012FC RID: 4860
	private Projector projector;

	// Token: 0x040012FD RID: 4861
	private const float BASE_ORTHO_SIZE = 0.5f;

	// Token: 0x040012FE RID: 4862
	private Material mat;
}
