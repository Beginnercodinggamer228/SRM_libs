using System;
using UnityEngine;

// Token: 0x02000446 RID: 1094
[Serializable]
public class SlimeAppearanceStructure
{
	// Token: 0x060016A1 RID: 5793 RVA: 0x00057E88 File Offset: 0x00056088
	public SlimeAppearanceStructure(SlimeAppearanceStructure slimeAppearanceStructure)
	{
		this.DefaultMaterials = new Material[slimeAppearanceStructure.DefaultMaterials.Length];
		Array.Copy(slimeAppearanceStructure.DefaultMaterials, this.DefaultMaterials, this.DefaultMaterials.Length);
		this.Element = slimeAppearanceStructure.Element;
		this.ElementMaterials = new SlimeAppearanceMaterials[slimeAppearanceStructure.ElementMaterials.Length];
		Array.Copy(slimeAppearanceStructure.ElementMaterials, this.ElementMaterials, this.ElementMaterials.Length);
		this.SupportsFaces = slimeAppearanceStructure.SupportsFaces;
		this.FaceRules = new SlimeFaceRules[slimeAppearanceStructure.FaceRules.Length];
		Array.Copy(slimeAppearanceStructure.FaceRules, this.FaceRules, this.FaceRules.Length);
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x00057F37 File Offset: 0x00056137
	public bool ElementMaterialCountIsValid()
	{
		return !(this.Element != null) || this.ElementMaterials.Length == this.Element.Prefabs.Length;
	}

	// Token: 0x040015E7 RID: 5607
	public Material[] DefaultMaterials;

	// Token: 0x040015E8 RID: 5608
	public SlimeAppearanceElement Element;

	// Token: 0x040015E9 RID: 5609
	public SlimeAppearanceMaterials[] ElementMaterials;

	// Token: 0x040015EA RID: 5610
	public bool SupportsFaces;

	// Token: 0x040015EB RID: 5611
	public SlimeFaceRules[] FaceRules;
}
