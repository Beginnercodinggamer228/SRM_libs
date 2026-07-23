using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200032E RID: 814
public class RanchDirector : MonoBehaviour, RanchModel.Participant
{
	// Token: 0x06001125 RID: 4389 RVA: 0x00044CB0 File Offset: 0x00042EB0
	public void Awake()
	{
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.paletteDict = this.palettes.ToDictionary((RanchDirector.PaletteEntry e) => e.palette, (RanchDirector.PaletteEntry e) => e);
		this.recolorMats[RanchDirector.PaletteType.VALLEY] = new Material[this.ranchMats.Length + this.houseMats.Length];
		this.recolorMats[RanchDirector.PaletteType.RANCH_TECH] = new Material[this.ranchMats.Length];
		this.recolorMats[RanchDirector.PaletteType.OGDEN_TECH] = new Material[this.ranchMats.Length];
		this.recolorMats[RanchDirector.PaletteType.MOCHI_TECH] = new Material[this.ranchMats.Length];
		this.recolorMats[RanchDirector.PaletteType.VIKTOR_TECH] = new Material[this.ranchMats.Length];
		for (int i = 0; i < this.ranchMats.Length; i++)
		{
			Material material = this.ranchMats[i];
			this.standardMatDict[material] = (this.recolorMats[RanchDirector.PaletteType.RANCH_TECH][i] = new Material(material));
			this.ogdenMatDict[material] = (this.recolorMats[RanchDirector.PaletteType.OGDEN_TECH][i] = new Material(material));
			this.mochiMatDict[material] = (this.recolorMats[RanchDirector.PaletteType.MOCHI_TECH][i] = new Material(material));
			this.valleyMatDict[material] = (this.recolorMats[RanchDirector.PaletteType.VALLEY][i] = new Material(material));
			this.viktorMatDict[material] = (this.recolorMats[RanchDirector.PaletteType.VIKTOR_TECH][i] = new Material(material));
		}
		this.recolorMats[RanchDirector.PaletteType.HOUSE] = new Material[this.houseMats.Length];
		this.recolorMats[RanchDirector.PaletteType.OGDEN_HOUSE] = new Material[this.houseMats.Length];
		this.recolorMats[RanchDirector.PaletteType.MOCHI_HOUSE] = new Material[this.houseMats.Length];
		this.recolorMats[RanchDirector.PaletteType.VIKTOR_HOUSE] = new Material[this.houseMats.Length];
		for (int j = 0; j < this.houseMats.Length; j++)
		{
			Material material2 = this.houseMats[j];
			this.standardMatDict[material2] = (this.recolorMats[RanchDirector.PaletteType.HOUSE][j] = new Material(material2));
			this.ogdenMatDict[material2] = (this.recolorMats[RanchDirector.PaletteType.OGDEN_HOUSE][j] = new Material(material2));
			this.mochiMatDict[material2] = (this.recolorMats[RanchDirector.PaletteType.MOCHI_HOUSE][j] = new Material(material2));
			this.valleyMatDict[material2] = (this.recolorMats[RanchDirector.PaletteType.VALLEY][j + this.ranchMats.Length] = new Material(material2));
			this.viktorMatDict[material2] = (this.recolorMats[RanchDirector.PaletteType.VIKTOR_HOUSE][j] = new Material(material2));
		}
		this.recolorMats[RanchDirector.PaletteType.VAC] = new Material[this.vacMats.Length];
		for (int k = 0; k < this.vacMats.Length; k++)
		{
			Material material3 = this.vacMats[k];
			Material material4 = new Material(material3);
			this.recolorMats[RanchDirector.PaletteType.VAC][k] = material4;
			this.standardMatDict[material3] = material4;
		}
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x00045022 File Offset: 0x00043222
	public void InitModel(RanchModel model)
	{
		this.ResetDefaults(model);
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0004502B File Offset: 0x0004322B
	public void SetModel(RanchModel model)
	{
		this.model = model;
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x00045034 File Offset: 0x00043234
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterRanch(this);
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x00045048 File Offset: 0x00043248
	public void OnDestroy()
	{
		foreach (Material instance in this.standardMatDict.Values)
		{
			Destroyer.Destroy(instance, "RanchDirector.OnDestroy(1)");
		}
		foreach (Material instance2 in this.ogdenMatDict.Values)
		{
			Destroyer.Destroy(instance2, "RanchDirector.OnDestroy(2)");
		}
		foreach (Material instance3 in this.mochiMatDict.Values)
		{
			Destroyer.Destroy(instance3, "RanchDirector.OnDestroy(3)");
		}
		foreach (Material instance4 in this.valleyMatDict.Values)
		{
			Destroyer.Destroy(instance4, "RanchDirector.OnDestroy(4)");
		}
		foreach (Material instance5 in this.viktorMatDict.Values)
		{
			Destroyer.Destroy(instance5, "RanchDirector.OnDestroy(5)");
		}
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x000451C8 File Offset: 0x000433C8
	public void RegisterPalette(RanchDirector.PaletteEntry entry)
	{
		this.paletteDict[entry.palette] = entry;
		this.orderedPalettes = null;
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x000451E3 File Offset: 0x000433E3
	public bool IsPartnerUnlocked()
	{
		return this.progressDir.HasProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER_UNLOCK);
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x000451F8 File Offset: 0x000433F8
	private void ResetDefaults(RanchModel model)
	{
		model.SelectPalette(RanchDirector.PaletteType.HOUSE, RanchDirector.Palette.DEFAULT);
		model.SelectPalette(RanchDirector.PaletteType.RANCH_TECH, RanchDirector.Palette.DEFAULT);
		model.SelectPalette(RanchDirector.PaletteType.OGDEN_HOUSE, RanchDirector.Palette.OGDEN);
		model.SelectPalette(RanchDirector.PaletteType.OGDEN_TECH, RanchDirector.Palette.OGDEN);
		model.SelectPalette(RanchDirector.PaletteType.MOCHI_HOUSE, RanchDirector.Palette.MOCHI);
		model.SelectPalette(RanchDirector.PaletteType.MOCHI_TECH, RanchDirector.Palette.MOCHI);
		model.SelectPalette(RanchDirector.PaletteType.VAC, RanchDirector.Palette.DEFAULT);
		model.SelectPalette(RanchDirector.PaletteType.VALLEY, RanchDirector.Palette.MOCHI);
		model.SelectPalette(RanchDirector.PaletteType.VIKTOR_TECH, RanchDirector.Palette.VIKTOR);
		model.SelectPalette(RanchDirector.PaletteType.VIKTOR_HOUSE, RanchDirector.Palette.VIKTOR);
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x00045274 File Offset: 0x00043474
	public void SetColorsForPalette(RanchDirector.PaletteType type, RanchDirector.Palette palette)
	{
		RanchDirector.PaletteEntry entry = this.paletteDict[palette];
		foreach (Material mat in this.GetRecolorMaterials(type))
		{
			this.SetColors(mat, entry);
		}
		if (type == RanchDirector.PaletteType.VAC)
		{
			foreach (Material mat2 in this.dynamicVacRecolorMats)
			{
				this.SetColors(mat2, entry);
			}
		}
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x00045300 File Offset: 0x00043500
	private void SetColors(Material mat, RanchDirector.PaletteEntry entry)
	{
		mat.SetColor("_Color00", entry.redDark);
		mat.SetColor("_Color01", entry.redLight);
		mat.SetColor("_Color10", entry.greenDark);
		mat.SetColor("_Color11", entry.greenLight);
		mat.SetColor("_Color20", entry.blueDark);
		mat.SetColor("_Color21", entry.blueLight);
		mat.SetColor("_Color30", entry.blackDark);
		mat.SetColor("_Color31", entry.blackLight);
		mat.SetColor("_Color40", entry.magentaDark);
		mat.SetColor("_Color41", entry.magentaLight);
		mat.SetColor("_Color50", entry.yellowDark);
		mat.SetColor("_Color51", entry.yellowLight);
		mat.SetColor("_Color60", entry.cyanDark);
		mat.SetColor("_Color61", entry.cyanLight);
		mat.SetColor("_Color70", entry.whiteDark);
		mat.SetColor("_Color71", entry.whiteLight);
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x0004541D File Offset: 0x0004361D
	private Material[] GetRecolorMaterials(RanchDirector.PaletteType type)
	{
		return this.recolorMats[type];
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0004542C File Offset: 0x0004362C
	private Dictionary<Material, Material> GetZoneDict(ZoneDirector.Zone zone)
	{
		switch (zone)
		{
		case ZoneDirector.Zone.WILDS:
		case ZoneDirector.Zone.OGDEN_RANCH:
			return this.ogdenMatDict;
		case ZoneDirector.Zone.VALLEY:
			return this.valleyMatDict;
		case ZoneDirector.Zone.MOCHI_RANCH:
			return this.mochiMatDict;
		case ZoneDirector.Zone.SLIMULATIONS:
		case ZoneDirector.Zone.VIKTOR_LAB:
			return this.viktorMatDict;
		default:
			return this.standardMatDict;
		}
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x0004547E File Offset: 0x0004367E
	public Material GetRecolorMaterial(Material mat, ZoneDirector.Zone zone)
	{
		if (this.GetZoneDict(zone).ContainsKey(mat))
		{
			return this.GetZoneDict(zone)[mat];
		}
		return null;
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x000454A0 File Offset: 0x000436A0
	public void RegisterVacRecolorMat(Material mat)
	{
		this.dynamicVacRecolorMats.Add(mat);
		if (base.enabled && this.model != null)
		{
			this.SetColors(mat, this.paletteDict[this.model.selectedPalettes[RanchDirector.PaletteType.VAC]]);
		}
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x000454EC File Offset: 0x000436EC
	public void UnregisterVacRecolorMat(Material mat)
	{
		this.dynamicVacRecolorMats.Remove(mat);
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x000454FB File Offset: 0x000436FB
	public bool IsSelectedPalette(RanchDirector.Palette palette, RanchDirector.PaletteType paletteType)
	{
		return this.model.selectedPalettes[paletteType] == palette;
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x00045514 File Offset: 0x00043714
	public bool HasPalette(RanchDirector.Palette palette)
	{
		RanchDirector.PaletteEntry paletteEntry = this.paletteDict[palette];
		return this.progressDir.GetProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER) >= paletteEntry.requiresPartnerLevel && (paletteEntry.requiresProgressCount <= 0 || this.progressDir.GetProgress(paletteEntry.requiresProgressType) >= paletteEntry.requiresProgressCount);
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0004556F File Offset: 0x0004376F
	public List<RanchDirector.PaletteEntry> GetOrderedPalettes()
	{
		if (this.orderedPalettes == null)
		{
			this.orderedPalettes = this.paletteDict.Values.ToList<RanchDirector.PaletteEntry>();
			this.orderedPalettes.Sort(RanchDirector.Comparer.DEFAULT);
		}
		return this.orderedPalettes;
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x000455A5 File Offset: 0x000437A5
	public void NoteSelected(RanchDirector.PaletteType type, RanchDirector.Palette palette)
	{
		this.SetColorsForPalette(type, palette);
	}

	// Token: 0x0400101F RID: 4127
	public Material[] ranchMats;

	// Token: 0x04001020 RID: 4128
	public Material[] houseMats;

	// Token: 0x04001021 RID: 4129
	public Material[] vacMats;

	// Token: 0x04001022 RID: 4130
	private const float PARTNER_UNLOCK_TIME = 259200f;

	// Token: 0x04001023 RID: 4131
	public RanchDirector.PaletteEntry[] palettes;

	// Token: 0x04001024 RID: 4132
	private Dictionary<RanchDirector.Palette, RanchDirector.PaletteEntry> paletteDict = new Dictionary<RanchDirector.Palette, RanchDirector.PaletteEntry>();

	// Token: 0x04001025 RID: 4133
	private Dictionary<Material, Material> standardMatDict = new Dictionary<Material, Material>();

	// Token: 0x04001026 RID: 4134
	private Dictionary<Material, Material> ogdenMatDict = new Dictionary<Material, Material>();

	// Token: 0x04001027 RID: 4135
	private Dictionary<Material, Material> mochiMatDict = new Dictionary<Material, Material>();

	// Token: 0x04001028 RID: 4136
	private Dictionary<Material, Material> valleyMatDict = new Dictionary<Material, Material>();

	// Token: 0x04001029 RID: 4137
	private Dictionary<Material, Material> viktorMatDict = new Dictionary<Material, Material>();

	// Token: 0x0400102A RID: 4138
	private List<RanchDirector.PaletteEntry> orderedPalettes;

	// Token: 0x0400102B RID: 4139
	private Dictionary<RanchDirector.PaletteType, Material[]> recolorMats = new Dictionary<RanchDirector.PaletteType, Material[]>();

	// Token: 0x0400102C RID: 4140
	private List<Material> dynamicVacRecolorMats = new List<Material>();

	// Token: 0x0400102D RID: 4141
	private ProgressDirector progressDir;

	// Token: 0x0400102E RID: 4142
	private RanchModel model;

	// Token: 0x0400102F RID: 4143
	public static string PARTNER_MAIL_KEY = "partner_rewards";

	// Token: 0x0200032F RID: 815
	public enum PaletteType
	{
		// Token: 0x04001031 RID: 4145
		RANCH_TECH,
		// Token: 0x04001032 RID: 4146
		HOUSE,
		// Token: 0x04001033 RID: 4147
		VAC,
		// Token: 0x04001034 RID: 4148
		OGDEN_TECH,
		// Token: 0x04001035 RID: 4149
		OGDEN_HOUSE,
		// Token: 0x04001036 RID: 4150
		MOCHI_TECH,
		// Token: 0x04001037 RID: 4151
		MOCHI_HOUSE,
		// Token: 0x04001038 RID: 4152
		VALLEY,
		// Token: 0x04001039 RID: 4153
		VIKTOR_TECH,
		// Token: 0x0400103A RID: 4154
		VIKTOR_HOUSE
	}

	// Token: 0x02000330 RID: 816
	public enum Palette
	{
		// Token: 0x0400103C RID: 4156
		DEFAULT,
		// Token: 0x0400103D RID: 4157
		PALETTE01,
		// Token: 0x0400103E RID: 4158
		PALETTE02,
		// Token: 0x0400103F RID: 4159
		PALETTE03,
		// Token: 0x04001040 RID: 4160
		PALETTE04,
		// Token: 0x04001041 RID: 4161
		PALETTE05,
		// Token: 0x04001042 RID: 4162
		PALETTE06,
		// Token: 0x04001043 RID: 4163
		PALETTE07,
		// Token: 0x04001044 RID: 4164
		PALETTE08,
		// Token: 0x04001045 RID: 4165
		PALETTE09,
		// Token: 0x04001046 RID: 4166
		PALETTE10,
		// Token: 0x04001047 RID: 4167
		PALETTE11,
		// Token: 0x04001048 RID: 4168
		PALETTE12,
		// Token: 0x04001049 RID: 4169
		PALETTE13,
		// Token: 0x0400104A RID: 4170
		PALETTE14,
		// Token: 0x0400104B RID: 4171
		PALETTE15,
		// Token: 0x0400104C RID: 4172
		PALETTE16,
		// Token: 0x0400104D RID: 4173
		PALETTE17,
		// Token: 0x0400104E RID: 4174
		PALETTE18,
		// Token: 0x0400104F RID: 4175
		PALETTE19,
		// Token: 0x04001050 RID: 4176
		PALETTE20,
		// Token: 0x04001051 RID: 4177
		PALETTE21,
		// Token: 0x04001052 RID: 4178
		PALETTE22,
		// Token: 0x04001053 RID: 4179
		PALETTE23,
		// Token: 0x04001054 RID: 4180
		PALETTE24,
		// Token: 0x04001055 RID: 4181
		PALETTE25,
		// Token: 0x04001056 RID: 4182
		PALETTE26,
		// Token: 0x04001057 RID: 4183
		PALETTE27,
		// Token: 0x04001058 RID: 4184
		PALETTE28,
		// Token: 0x04001059 RID: 4185
		PALETTE29,
		// Token: 0x0400105A RID: 4186
		PALETTE30,
		// Token: 0x0400105B RID: 4187
		OGDEN = 1000,
		// Token: 0x0400105C RID: 4188
		MOCHI,
		// Token: 0x0400105D RID: 4189
		VIKTOR
	}

	// Token: 0x02000331 RID: 817
	[Serializable]
	public class PaletteEntry
	{
		// Token: 0x0400105E RID: 4190
		public RanchDirector.Palette palette;

		// Token: 0x0400105F RID: 4191
		public Sprite icon;

		// Token: 0x04001060 RID: 4192
		public int order;

		// Token: 0x04001061 RID: 4193
		public int requiresPartnerLevel;

		// Token: 0x04001062 RID: 4194
		public ProgressDirector.ProgressType requiresProgressType = ProgressDirector.ProgressType.NONE;

		// Token: 0x04001063 RID: 4195
		public int requiresProgressCount;

		// Token: 0x04001064 RID: 4196
		public Color redDark;

		// Token: 0x04001065 RID: 4197
		public Color redLight;

		// Token: 0x04001066 RID: 4198
		public Color greenDark;

		// Token: 0x04001067 RID: 4199
		public Color greenLight;

		// Token: 0x04001068 RID: 4200
		public Color blueDark;

		// Token: 0x04001069 RID: 4201
		public Color blueLight;

		// Token: 0x0400106A RID: 4202
		public Color blackDark;

		// Token: 0x0400106B RID: 4203
		public Color blackLight;

		// Token: 0x0400106C RID: 4204
		public Color magentaDark;

		// Token: 0x0400106D RID: 4205
		public Color magentaLight;

		// Token: 0x0400106E RID: 4206
		public Color yellowDark;

		// Token: 0x0400106F RID: 4207
		public Color yellowLight;

		// Token: 0x04001070 RID: 4208
		public Color cyanDark;

		// Token: 0x04001071 RID: 4209
		public Color cyanLight;

		// Token: 0x04001072 RID: 4210
		public Color whiteDark;

		// Token: 0x04001073 RID: 4211
		public Color whiteLight;
	}

	// Token: 0x02000332 RID: 818
	private class Comparer : SRComparer<RanchDirector.PaletteEntry>
	{
		// Token: 0x04001074 RID: 4212
		public static Comparer<RanchDirector.PaletteEntry> DEFAULT = from p in new RanchDirector.Comparer()
		orderby p.palette == RanchDirector.Palette.DEFAULT descending
		orderby p.requiresPartnerLevel > 0 descending
		orderby p.requiresPartnerLevel
		orderby p.order
		select p;
	}
}
