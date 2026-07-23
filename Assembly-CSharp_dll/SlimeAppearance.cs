using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200043F RID: 1087
[CreateAssetMenu(menuName = "Slimes/Slime Appearance")]
public class SlimeAppearance : ScriptableObject
{
	// Token: 0x06001692 RID: 5778 RVA: 0x00057C12 File Offset: 0x00055E12
	public void MaybeShowPopupUI()
	{
		if (this.SaveSet == SlimeAppearance.AppearanceSaveSet.SECRET_STYLE)
		{
			PopupDirector popupDirector = SRSingleton<SceneContext>.Instance.PopupDirector;
			popupDirector.QueueForPopup(new SlimeAppearancePopupUI.PopupCreator(this));
			popupDirector.MaybePopupNext();
		}
	}

	// Token: 0x06001693 RID: 5779 RVA: 0x00057C38 File Offset: 0x00055E38
	public static SlimeAppearance CombineAppearances(SlimeAppearance appearance1, SlimeAppearance appearance2)
	{
		SlimeAppearance slimeAppearance = ScriptableObject.CreateInstance<SlimeAppearance>();
		slimeAppearance.Face = appearance1.Face;
		HashSet<SlimeAppearanceStructure> hashSet = new HashSet<SlimeAppearanceStructure>(new SlimeAppearance.SlimeAppearanceStructureComparer());
		foreach (SlimeAppearanceStructure item in appearance1.Structures)
		{
			hashSet.Add(item);
		}
		foreach (SlimeAppearanceStructure item2 in appearance2.Structures)
		{
			if (!hashSet.Contains(item2))
			{
				hashSet.Add(item2);
			}
		}
		slimeAppearance.Structures = (from s in hashSet
		select new SlimeAppearanceStructure(s)).ToArray<SlimeAppearanceStructure>();
		slimeAppearance.DependentAppearances = new SlimeAppearance[]
		{
			appearance1,
			appearance2
		};
		slimeAppearance.GlintAppearance = (appearance1.GlintAppearance ?? appearance2.GlintAppearance);
		slimeAppearance.TornadoAppearance = (appearance1.TornadoAppearance ?? appearance2.TornadoAppearance);
		slimeAppearance.VineAppearance = (appearance1.VineAppearance ?? appearance2.VineAppearance);
		slimeAppearance.CrystalAppearance = (appearance1.CrystalAppearance ?? appearance2.CrystalAppearance);
		slimeAppearance.ExplosionAppearance = (appearance1.ExplosionAppearance ?? appearance2.ExplosionAppearance);
		return slimeAppearance;
	}

	// Token: 0x040015B8 RID: 5560
	public string NameXlateKey;

	// Token: 0x040015B9 RID: 5561
	public Sprite Icon;

	// Token: 0x040015BA RID: 5562
	public RuntimeAnimatorController AnimatorOverride;

	// Token: 0x040015BB RID: 5563
	public SlimeAppearanceStructure[] Structures;

	// Token: 0x040015BC RID: 5564
	public SlimeFace Face;

	// Token: 0x040015BD RID: 5565
	public SlimeAppearance QubitAppearance;

	// Token: 0x040015BE RID: 5566
	public SlimeAppearance ShockedAppearance;

	// Token: 0x040015BF RID: 5567
	public SlimeAppearance[] DependentAppearances;

	// Token: 0x040015C0 RID: 5568
	public SlimeAppearance.Palette ColorPalette;

	// Token: 0x040015C1 RID: 5569
	public GlintAppearance GlintAppearance;

	// Token: 0x040015C2 RID: 5570
	public TornadoAppearance TornadoAppearance;

	// Token: 0x040015C3 RID: 5571
	public VineAppearance VineAppearance;

	// Token: 0x040015C4 RID: 5572
	public CrystalAppearance CrystalAppearance;

	// Token: 0x040015C5 RID: 5573
	public ExplosionAppearance ExplosionAppearance;

	// Token: 0x040015C6 RID: 5574
	public DeathAppearance DeathAppearance;

	// Token: 0x040015C7 RID: 5575
	public SlimeAppearance.AppearanceSaveSet SaveSet;

	// Token: 0x040015C8 RID: 5576
	public static SlimeAppearanceEqualityComparer DefaultComparer = new SlimeAppearanceEqualityComparer();

	// Token: 0x040015C9 RID: 5577
	public static SlimeAppearance.BoneComparer DefaultBoneComparer = new SlimeAppearance.BoneComparer();

	// Token: 0x02000440 RID: 1088
	public enum SlimeBone
	{
		// Token: 0x040015CB RID: 5579
		None,
		// Token: 0x040015CC RID: 5580
		Root,
		// Token: 0x040015CD RID: 5581
		Attachment,
		// Token: 0x040015CE RID: 5582
		Slime,
		// Token: 0x040015CF RID: 5583
		Core,
		// Token: 0x040015D0 RID: 5584
		JiggleBack,
		// Token: 0x040015D1 RID: 5585
		JiggleBottom,
		// Token: 0x040015D2 RID: 5586
		JiggleFront,
		// Token: 0x040015D3 RID: 5587
		JiggleLeft,
		// Token: 0x040015D4 RID: 5588
		JiggleRight,
		// Token: 0x040015D5 RID: 5589
		JiggleTop,
		// Token: 0x040015D6 RID: 5590
		Spinner,
		// Token: 0x040015D7 RID: 5591
		LeftWing,
		// Token: 0x040015D8 RID: 5592
		RightWing
	}

	// Token: 0x02000441 RID: 1089
	public enum AppearanceSaveSet
	{
		// Token: 0x040015DA RID: 5594
		NONE,
		// Token: 0x040015DB RID: 5595
		CLASSIC,
		// Token: 0x040015DC RID: 5596
		SECRET_STYLE
	}

	// Token: 0x02000442 RID: 1090
	public class BoneComparer : IEqualityComparer<SlimeAppearance.SlimeBone>
	{
		// Token: 0x06001696 RID: 5782 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(SlimeAppearance.SlimeBone bone1, SlimeAppearance.SlimeBone bone2)
		{
			return bone1 == bone2;
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(SlimeAppearance.SlimeBone bone)
		{
			return (int)bone;
		}
	}

	// Token: 0x02000443 RID: 1091
	private class SlimeAppearanceStructureComparer : IEqualityComparer<SlimeAppearanceStructure>
	{
		// Token: 0x06001699 RID: 5785 RVA: 0x00057D7B File Offset: 0x00055F7B
		public bool Equals(SlimeAppearanceStructure x, SlimeAppearanceStructure y)
		{
			return x.Element.GetHashCode() == y.Element.GetHashCode();
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x00057D95 File Offset: 0x00055F95
		public int GetHashCode(SlimeAppearanceStructure obj)
		{
			return obj.Element.GetHashCode();
		}
	}

	// Token: 0x02000444 RID: 1092
	[Serializable]
	public struct Palette
	{
		// Token: 0x0600169C RID: 5788 RVA: 0x00057DA4 File Offset: 0x00055FA4
		public static SlimeAppearance.Palette FromMaterial(Material material)
		{
			return new SlimeAppearance.Palette
			{
				Top = material.GetColor(SlimeAppearance.Palette.TopColorPropertyId),
				Middle = material.GetColor(SlimeAppearance.Palette.MiddleColorPropertyId),
				Bottom = material.GetColor(SlimeAppearance.Palette.BottomColorPropertyId),
				Ammo = Color.black
			};
		}

		// Token: 0x040015DD RID: 5597
		private static int TopColorPropertyId = Shader.PropertyToID("_TopColor");

		// Token: 0x040015DE RID: 5598
		private static int MiddleColorPropertyId = Shader.PropertyToID("_MiddleColor");

		// Token: 0x040015DF RID: 5599
		private static int BottomColorPropertyId = Shader.PropertyToID("_BottomColor");

		// Token: 0x040015E0 RID: 5600
		public static SlimeAppearance.Palette Default = new SlimeAppearance.Palette
		{
			Top = Color.grey,
			Middle = Color.grey,
			Bottom = Color.grey,
			Ammo = Color.grey
		};

		// Token: 0x040015E1 RID: 5601
		public Color Top;

		// Token: 0x040015E2 RID: 5602
		public Color Middle;

		// Token: 0x040015E3 RID: 5603
		public Color Bottom;

		// Token: 0x040015E4 RID: 5604
		public Color Ammo;
	}
}
