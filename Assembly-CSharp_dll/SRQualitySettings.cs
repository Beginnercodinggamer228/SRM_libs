using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000371 RID: 881
public static class SRQualitySettings
{
	// Token: 0x0600123F RID: 4671 RVA: 0x000489AC File Offset: 0x00046BAC
	static SRQualitySettings()
	{
		SRQualitySettings.defaults[SRQualitySettings.Level.LOWEST] = new SRQualitySettings.Settings(SRQualitySettings.LightingLevel.LOW, SRQualitySettings.TextureLevel.LOW, SRQualitySettings.AntialiasingMode.NONE, SRQualitySettings.ShadowsLevel.NONE, SRQualitySettings.ParticlesLevel.LOW, SRQualitySettings.ModelDetailLevel.LOW, SRQualitySettings.WaterDetailLevel.LOW, false, false);
		SRQualitySettings.defaults[SRQualitySettings.Level.LOW] = new SRQualitySettings.Settings(SRQualitySettings.LightingLevel.MEDIUM, SRQualitySettings.TextureLevel.LOW, SRQualitySettings.AntialiasingMode.NONE, SRQualitySettings.ShadowsLevel.LOW, SRQualitySettings.ParticlesLevel.LOW, SRQualitySettings.ModelDetailLevel.MEDIUM, SRQualitySettings.WaterDetailLevel.LOW, false, false);
		SRQualitySettings.defaults[SRQualitySettings.Level.DEFAULT] = new SRQualitySettings.Settings(SRQualitySettings.LightingLevel.HIGH, SRQualitySettings.TextureLevel.MEDIUM, SRQualitySettings.AntialiasingMode.NONE, SRQualitySettings.ShadowsLevel.MEDIUM, SRQualitySettings.ParticlesLevel.MEDIUM, SRQualitySettings.ModelDetailLevel.MEDIUM, SRQualitySettings.WaterDetailLevel.MEDIUM, false, true);
		SRQualitySettings.defaults[SRQualitySettings.Level.HIGH] = new SRQualitySettings.Settings(SRQualitySettings.LightingLevel.HIGHEST, SRQualitySettings.TextureLevel.HIGH, SRQualitySettings.AntialiasingMode.MULTISAMPLING_2X, SRQualitySettings.ShadowsLevel.MEDIUM, SRQualitySettings.ParticlesLevel.HIGH, SRQualitySettings.ModelDetailLevel.HIGH, SRQualitySettings.WaterDetailLevel.HIGH, true, true);
		SRQualitySettings.defaults[SRQualitySettings.Level.VERY_HIGH] = new SRQualitySettings.Settings(SRQualitySettings.LightingLevel.HIGHEST, SRQualitySettings.TextureLevel.HIGH, SRQualitySettings.AntialiasingMode.MULTISAMPLING_8X, SRQualitySettings.ShadowsLevel.HIGH, SRQualitySettings.ParticlesLevel.HIGH, SRQualitySettings.ModelDetailLevel.HIGH, SRQualitySettings.WaterDetailLevel.HIGH, true, true);
		SRQualitySettings.SetToDefaultLevels();
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x00048A58 File Offset: 0x00046C58
	private static void SetToDefaultLevels()
	{
		SRQualitySettings.CurrentLevel = SRQualitySettings.Level.DEFAULT;
	}

	// Token: 0x06001241 RID: 4673 RVA: 0x00048A60 File Offset: 0x00046C60
	public static void ResetProfile()
	{
		SRQualitySettings.SetToDefaultLevels();
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00048A67 File Offset: 0x00046C67
	public static void ForceLowQuality()
	{
		Log.Debug("Forcing Low Quality", Array.Empty<object>());
		SRQualitySettings.CurrentLevel = SRQualitySettings.Level.LOWEST;
	}

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06001243 RID: 4675 RVA: 0x00048A7E File Offset: 0x00046C7E
	// (set) Token: 0x06001244 RID: 4676 RVA: 0x00048A85 File Offset: 0x00046C85
	public static SRQualitySettings.Level CurrentLevel
	{
		get
		{
			return SRQualitySettings.level;
		}
		set
		{
			SRQualitySettings.level = value;
			SRQualitySettings.SetDefaults(SRQualitySettings.level);
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06001245 RID: 4677 RVA: 0x00048A97 File Offset: 0x00046C97
	// (set) Token: 0x06001246 RID: 4678 RVA: 0x00048AA3 File Offset: 0x00046CA3
	public static SRQualitySettings.LightingLevel Lighting
	{
		get
		{
			return SRQualitySettings.currSettings.lighting;
		}
		set
		{
			if (SRQualitySettings.currSettings.lighting != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.lighting = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06001247 RID: 4679 RVA: 0x00048ACC File Offset: 0x00046CCC
	// (set) Token: 0x06001248 RID: 4680 RVA: 0x00048AD8 File Offset: 0x00046CD8
	public static SRQualitySettings.TextureLevel Textures
	{
		get
		{
			return SRQualitySettings.currSettings.textures;
		}
		set
		{
			if (SRQualitySettings.currSettings.textures != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.textures = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06001249 RID: 4681 RVA: 0x00048B01 File Offset: 0x00046D01
	// (set) Token: 0x0600124A RID: 4682 RVA: 0x00048B0D File Offset: 0x00046D0D
	public static SRQualitySettings.AntialiasingMode Antialiasing
	{
		get
		{
			return SRQualitySettings.currSettings.antialiasing;
		}
		set
		{
			if (SRQualitySettings.currSettings.antialiasing != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.antialiasing = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x0600124B RID: 4683 RVA: 0x00048B36 File Offset: 0x00046D36
	// (set) Token: 0x0600124C RID: 4684 RVA: 0x00048B42 File Offset: 0x00046D42
	public static SRQualitySettings.ParticlesLevel Particles
	{
		get
		{
			return SRQualitySettings.currSettings.particles;
		}
		set
		{
			if (SRQualitySettings.currSettings.particles != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.particles = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x0600124D RID: 4685 RVA: 0x00048B6B File Offset: 0x00046D6B
	// (set) Token: 0x0600124E RID: 4686 RVA: 0x00048B77 File Offset: 0x00046D77
	public static SRQualitySettings.ShadowsLevel Shadows
	{
		get
		{
			return SRQualitySettings.currSettings.shadows;
		}
		set
		{
			if (SRQualitySettings.currSettings.shadows != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.shadows = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x0600124F RID: 4687 RVA: 0x00048BA0 File Offset: 0x00046DA0
	// (set) Token: 0x06001250 RID: 4688 RVA: 0x00048BAC File Offset: 0x00046DAC
	public static SRQualitySettings.ModelDetailLevel ModelDetail
	{
		get
		{
			return SRQualitySettings.currSettings.modelDetail;
		}
		set
		{
			if (SRQualitySettings.currSettings.modelDetail != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.modelDetail = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06001251 RID: 4689 RVA: 0x00048BD5 File Offset: 0x00046DD5
	// (set) Token: 0x06001252 RID: 4690 RVA: 0x00048BE1 File Offset: 0x00046DE1
	public static SRQualitySettings.WaterDetailLevel WaterDetail
	{
		get
		{
			return SRQualitySettings.currSettings.waterDetail;
		}
		set
		{
			if (SRQualitySettings.currSettings.waterDetail != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.waterDetail = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x06001253 RID: 4691 RVA: 0x00048C0A File Offset: 0x00046E0A
	// (set) Token: 0x06001254 RID: 4692 RVA: 0x00048C16 File Offset: 0x00046E16
	public static bool AmbientOcclusion
	{
		get
		{
			return SRQualitySettings.currSettings.ambientOcclusion;
		}
		set
		{
			if (SRQualitySettings.currSettings.ambientOcclusion != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.ambientOcclusion = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06001255 RID: 4693 RVA: 0x00048C3F File Offset: 0x00046E3F
	// (set) Token: 0x06001256 RID: 4694 RVA: 0x00048C4B File Offset: 0x00046E4B
	public static bool Bloom
	{
		get
		{
			return SRQualitySettings.currSettings.bloom;
		}
		set
		{
			if (SRQualitySettings.currSettings.bloom != value)
			{
				SRQualitySettings.CurrentLevel = SRQualitySettings.Level.CUSTOM;
			}
			SRQualitySettings.currSettings.bloom = value;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x00048C74 File Offset: 0x00046E74
	public static DepthTextureMode GetDepthTextureMode()
	{
		DepthTextureMode depthTextureMode = DepthTextureMode.Depth;
		if (SRQualitySettings.currSettings.ambientOcclusion)
		{
			depthTextureMode |= DepthTextureMode.DepthNormals;
		}
		return depthTextureMode;
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x00048C94 File Offset: 0x00046E94
	private static void SetLighting(SRQualitySettings.LightingLevel level)
	{
		switch (level)
		{
		case SRQualitySettings.LightingLevel.LOW:
			QualitySettings.pixelLightCount = 0;
			return;
		case SRQualitySettings.LightingLevel.MEDIUM:
			QualitySettings.pixelLightCount = 1;
			return;
		case SRQualitySettings.LightingLevel.HIGH:
			QualitySettings.pixelLightCount = 2;
			return;
		case SRQualitySettings.LightingLevel.HIGHEST:
			QualitySettings.pixelLightCount = 4;
			return;
		default:
			Log.Warning("Unknown level: " + level, Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x00048CF0 File Offset: 0x00046EF0
	private static void SetTextures(SRQualitySettings.TextureLevel level)
	{
		switch (level)
		{
		case SRQualitySettings.TextureLevel.LOW:
			QualitySettings.masterTextureLimit = 1;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			return;
		case SRQualitySettings.TextureLevel.MEDIUM:
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
			return;
		case SRQualitySettings.TextureLevel.HIGH:
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			return;
		default:
			Log.Warning("Unknown level: " + level, Array.Empty<object>());
			return;
		}
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x00048D54 File Offset: 0x00046F54
	private static void SetAntialiasing(SRQualitySettings.AntialiasingMode mode)
	{
		switch (mode)
		{
		case SRQualitySettings.AntialiasingMode.NONE:
			QualitySettings.antiAliasing = 0;
			return;
		case SRQualitySettings.AntialiasingMode.MULTISAMPLING_2X:
			QualitySettings.antiAliasing = 2;
			return;
		case SRQualitySettings.AntialiasingMode.MULTISAMPLING_4X:
			QualitySettings.antiAliasing = 4;
			return;
		case SRQualitySettings.AntialiasingMode.MULTISAMPLING_8X:
			QualitySettings.antiAliasing = 8;
			return;
		default:
			Log.Warning("Unknown mode: " + mode, Array.Empty<object>());
			return;
		}
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x00048DB0 File Offset: 0x00046FB0
	private static void SetShadows(SRQualitySettings.ShadowsLevel level)
	{
		switch (level)
		{
		case SRQualitySettings.ShadowsLevel.NONE:
			QualitySettings.shadowProjection = ShadowProjection.CloseFit;
			QualitySettings.shadowDistance = 15f;
			QualitySettings.shadowNearPlaneOffset = 2f;
			QualitySettings.shadowCascades = 0;
			return;
		case SRQualitySettings.ShadowsLevel.LOW:
			QualitySettings.shadowProjection = ShadowProjection.CloseFit;
			QualitySettings.shadowDistance = 20f;
			QualitySettings.shadowNearPlaneOffset = 2f;
			QualitySettings.shadowCascades = 0;
			return;
		case SRQualitySettings.ShadowsLevel.MEDIUM:
			QualitySettings.shadowProjection = ShadowProjection.CloseFit;
			QualitySettings.shadowDistance = 100f;
			QualitySettings.shadowNearPlaneOffset = 2f;
			QualitySettings.shadowCascades = 2;
			QualitySettings.shadowCascade2Split = 0.333f;
			return;
		case SRQualitySettings.ShadowsLevel.HIGH:
			QualitySettings.shadowProjection = ShadowProjection.StableFit;
			QualitySettings.shadowDistance = 150f;
			QualitySettings.shadowNearPlaneOffset = 2f;
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowCascade4Split = new Vector3(0.067f, 0.2f, 0.467f);
			return;
		default:
			Log.Warning("Unknown level: " + level, Array.Empty<object>());
			return;
		}
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x00048E9C File Offset: 0x0004709C
	private static void SetParticles(SRQualitySettings.ParticlesLevel level)
	{
		switch (level)
		{
		case SRQualitySettings.ParticlesLevel.LOW:
			QualitySettings.particleRaycastBudget = 16;
			return;
		case SRQualitySettings.ParticlesLevel.MEDIUM:
			QualitySettings.particleRaycastBudget = 256;
			return;
		case SRQualitySettings.ParticlesLevel.HIGH:
			QualitySettings.particleRaycastBudget = 1024;
			return;
		default:
			Log.Warning("Unknown level: " + level, Array.Empty<object>());
			return;
		}
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x00048EF8 File Offset: 0x000470F8
	private static void SetModelDetail(SRQualitySettings.ModelDetailLevel level)
	{
		switch (level)
		{
		case SRQualitySettings.ModelDetailLevel.LOW:
			QualitySettings.skinWeights = SkinWeights.TwoBones;
			QualitySettings.lodBias = 0.5f;
			QualitySettings.maximumLODLevel = 1;
			return;
		case SRQualitySettings.ModelDetailLevel.MEDIUM:
			QualitySettings.skinWeights = SkinWeights.FourBones;
			QualitySettings.lodBias = 1f;
			QualitySettings.maximumLODLevel = 0;
			return;
		case SRQualitySettings.ModelDetailLevel.HIGH:
			QualitySettings.skinWeights = SkinWeights.FourBones;
			QualitySettings.lodBias = 2f;
			QualitySettings.maximumLODLevel = 0;
			return;
		default:
			Log.Warning("Unknown level: " + level, Array.Empty<object>());
			return;
		}
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x00048F78 File Offset: 0x00047178
	private static void SetWaterDetail(SRQualitySettings.WaterDetailLevel level)
	{
		switch (level)
		{
		case SRQualitySettings.WaterDetailLevel.LOW:
			Shader.globalMaximumLOD = 400;
			return;
		case SRQualitySettings.WaterDetailLevel.MEDIUM:
			Shader.globalMaximumLOD = 700;
			return;
		case SRQualitySettings.WaterDetailLevel.HIGH:
			Shader.globalMaximumLOD = 900;
			return;
		default:
			Log.Warning("Unknown level: " + level, Array.Empty<object>());
			return;
		}
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x00048FD4 File Offset: 0x000471D4
	private static void SetDefaults(SRQualitySettings.Level level)
	{
		if (SRQualitySettings.defaults.ContainsKey(level))
		{
			SRQualitySettings.Settings settings = SRQualitySettings.defaults[level];
			SRQualitySettings.currSettings.lighting = settings.lighting;
			SRQualitySettings.currSettings.textures = settings.textures;
			SRQualitySettings.currSettings.particles = settings.particles;
			SRQualitySettings.currSettings.shadows = settings.shadows;
			SRQualitySettings.currSettings.modelDetail = settings.modelDetail;
			SRQualitySettings.currSettings.waterDetail = settings.waterDetail;
			SRQualitySettings.currSettings.ambientOcclusion = settings.ambientOcclusion;
			SRQualitySettings.currSettings.bloom = settings.bloom;
			SRQualitySettings.currSettings.antialiasing = settings.antialiasing;
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x00049094 File Offset: 0x00047294
	private static void UpdateFromLevels()
	{
		SRQualitySettings.UpdateBaseQualityLevel();
		SRQualitySettings.SetLighting(SRQualitySettings.currSettings.lighting);
		SRQualitySettings.SetTextures(SRQualitySettings.currSettings.textures);
		SRQualitySettings.SetParticles(SRQualitySettings.currSettings.particles);
		SRQualitySettings.SetShadows(SRQualitySettings.currSettings.shadows);
		SRQualitySettings.SetModelDetail(SRQualitySettings.currSettings.modelDetail);
		SRQualitySettings.SetAntialiasing(SRQualitySettings.currSettings.antialiasing);
		SRQualitySettings.SetWaterDetail(SRQualitySettings.currSettings.waterDetail);
		SRQualitySettings.UpdateVsync();
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x00049114 File Offset: 0x00047314
	private static void UpdateVsync()
	{
		SRSingleton<GameContext>.Instance.OptionsDirector.UpdateVsync();
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x00049128 File Offset: 0x00047328
	private static void UpdateBaseQualityLevel()
	{
		bool flag = SRQualitySettings.currSettings.particles == SRQualitySettings.ParticlesLevel.HIGH;
		SRQualitySettings.ShadowsLevel shadows = SRQualitySettings.currSettings.shadows;
		string text = Enum.GetName(typeof(SRQualitySettings.ShadowsLevel), shadows).ToLowerInvariant() + "Shadows_" + (flag ? "softParticles" : "hardParticles");
		string[] names = QualitySettings.names;
		bool flag2 = false;
		for (int i = 0; i < names.Length; i++)
		{
			if (names[i] == text)
			{
				QualitySettings.SetQualityLevel(i, true);
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			Log.Warning("Did not find quality level: " + text, Array.Empty<object>());
		}
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x000491CF File Offset: 0x000473CF
	public static void Pull(out SRQualitySettings.Settings settings, out SRQualitySettings.Level overallLevel)
	{
		settings = new SRQualitySettings.Settings(SRQualitySettings.currSettings);
		overallLevel = SRQualitySettings.CurrentLevel;
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x000491E4 File Offset: 0x000473E4
	public static void Push(SRQualitySettings.Settings settings, SRQualitySettings.Level overallLevel)
	{
		SRQualitySettings.CurrentLevel = overallLevel;
		if (settings != null)
		{
			SRQualitySettings.currSettings = new SRQualitySettings.Settings(settings);
			SRQualitySettings.UpdateFromLevels();
		}
	}

	// Token: 0x0400116F RID: 4463
	private static SRQualitySettings.Settings currSettings = new SRQualitySettings.Settings(SRQualitySettings.LightingLevel.LOW, SRQualitySettings.TextureLevel.LOW, SRQualitySettings.AntialiasingMode.NONE, SRQualitySettings.ShadowsLevel.NONE, SRQualitySettings.ParticlesLevel.LOW, SRQualitySettings.ModelDetailLevel.LOW, SRQualitySettings.WaterDetailLevel.LOW, false, false);

	// Token: 0x04001170 RID: 4464
	private static Dictionary<SRQualitySettings.Level, SRQualitySettings.Settings> defaults = new Dictionary<SRQualitySettings.Level, SRQualitySettings.Settings>();

	// Token: 0x04001171 RID: 4465
	private static SRQualitySettings.Level level;

	// Token: 0x02000372 RID: 882
	public enum Level
	{
		// Token: 0x04001173 RID: 4467
		LOWEST,
		// Token: 0x04001174 RID: 4468
		LOW,
		// Token: 0x04001175 RID: 4469
		DEFAULT,
		// Token: 0x04001176 RID: 4470
		HIGH,
		// Token: 0x04001177 RID: 4471
		VERY_HIGH,
		// Token: 0x04001178 RID: 4472
		CUSTOM = 1000
	}

	// Token: 0x02000373 RID: 883
	public enum LightingLevel
	{
		// Token: 0x0400117A RID: 4474
		LOW,
		// Token: 0x0400117B RID: 4475
		MEDIUM,
		// Token: 0x0400117C RID: 4476
		HIGH,
		// Token: 0x0400117D RID: 4477
		HIGHEST
	}

	// Token: 0x02000374 RID: 884
	public enum TextureLevel
	{
		// Token: 0x0400117F RID: 4479
		LOW,
		// Token: 0x04001180 RID: 4480
		MEDIUM,
		// Token: 0x04001181 RID: 4481
		HIGH
	}

	// Token: 0x02000375 RID: 885
	public enum AntialiasingMode
	{
		// Token: 0x04001183 RID: 4483
		NONE,
		// Token: 0x04001184 RID: 4484
		MULTISAMPLING_2X,
		// Token: 0x04001185 RID: 4485
		MULTISAMPLING_4X,
		// Token: 0x04001186 RID: 4486
		MULTISAMPLING_8X
	}

	// Token: 0x02000376 RID: 886
	public enum ParticlesLevel
	{
		// Token: 0x04001188 RID: 4488
		LOW,
		// Token: 0x04001189 RID: 4489
		MEDIUM,
		// Token: 0x0400118A RID: 4490
		HIGH
	}

	// Token: 0x02000377 RID: 887
	public enum ShadowsLevel
	{
		// Token: 0x0400118C RID: 4492
		NONE,
		// Token: 0x0400118D RID: 4493
		LOW,
		// Token: 0x0400118E RID: 4494
		MEDIUM,
		// Token: 0x0400118F RID: 4495
		HIGH
	}

	// Token: 0x02000378 RID: 888
	public enum ModelDetailLevel
	{
		// Token: 0x04001191 RID: 4497
		LOW,
		// Token: 0x04001192 RID: 4498
		MEDIUM,
		// Token: 0x04001193 RID: 4499
		HIGH
	}

	// Token: 0x02000379 RID: 889
	public enum WaterDetailLevel
	{
		// Token: 0x04001195 RID: 4501
		LOW,
		// Token: 0x04001196 RID: 4502
		MEDIUM,
		// Token: 0x04001197 RID: 4503
		HIGH
	}

	// Token: 0x0200037A RID: 890
	[Serializable]
	public class Settings
	{
		// Token: 0x06001265 RID: 4709 RVA: 0x00049200 File Offset: 0x00047400
		public Settings(SRQualitySettings.LightingLevel lighting, SRQualitySettings.TextureLevel textures, SRQualitySettings.AntialiasingMode antialiasing, SRQualitySettings.ShadowsLevel shadows, SRQualitySettings.ParticlesLevel particles, SRQualitySettings.ModelDetailLevel modelDetail, SRQualitySettings.WaterDetailLevel waterDetail, bool ambientOcclusion, bool bloom)
		{
			this.lighting = lighting;
			this.textures = textures;
			this.antialiasing = antialiasing;
			this.shadows = shadows;
			this.particles = particles;
			this.modelDetail = modelDetail;
			this.waterDetail = waterDetail;
			this.ambientOcclusion = ambientOcclusion;
			this.bloom = bloom;
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x00049258 File Offset: 0x00047458
		public Settings(SRQualitySettings.Settings other)
		{
			this.lighting = other.lighting;
			this.textures = other.textures;
			this.antialiasing = other.antialiasing;
			this.shadows = other.shadows;
			this.particles = other.particles;
			this.modelDetail = other.modelDetail;
			this.waterDetail = other.waterDetail;
			this.ambientOcclusion = other.ambientOcclusion;
			this.bloom = other.bloom;
		}

		// Token: 0x04001198 RID: 4504
		public SRQualitySettings.LightingLevel lighting;

		// Token: 0x04001199 RID: 4505
		public SRQualitySettings.TextureLevel textures;

		// Token: 0x0400119A RID: 4506
		public SRQualitySettings.AntialiasingMode antialiasing;

		// Token: 0x0400119B RID: 4507
		public SRQualitySettings.ShadowsLevel shadows;

		// Token: 0x0400119C RID: 4508
		public SRQualitySettings.ParticlesLevel particles;

		// Token: 0x0400119D RID: 4509
		public SRQualitySettings.ModelDetailLevel modelDetail;

		// Token: 0x0400119E RID: 4510
		public SRQualitySettings.WaterDetailLevel waterDetail;

		// Token: 0x0400119F RID: 4511
		public bool ambientOcclusion;

		// Token: 0x040011A0 RID: 4512
		public bool bloom;
	}
}
