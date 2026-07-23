using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020004BF RID: 1215
[CreateAssetMenu(menuName = "Slimes/Slime Appearance Director")]
public class SlimeAppearanceDirector : ScriptableObject, AppearancesModel.Participant
{
	// Token: 0x14000019 RID: 25
	// (add) Token: 0x0600196E RID: 6510 RVA: 0x00062F40 File Offset: 0x00061140
	// (remove) Token: 0x0600196F RID: 6511 RVA: 0x00062F78 File Offset: 0x00061178
	public event SlimeAppearanceDirector.OnSlimeAppearanceChangedDelegate onSlimeAppearanceChanged = delegate(SlimeDefinition <p0>, SlimeAppearance <p1>)
	{
	};

	// Token: 0x06001970 RID: 6512 RVA: 0x00062FB0 File Offset: 0x000611B0
	public void OnEnable()
	{
		foreach (SlimeDefinition slimeDefinition in this.SlimeDefinitions.Slimes)
		{
			foreach (SlimeAppearance appearance in slimeDefinition.Appearances)
			{
				this.RegisterDependentAppearances(slimeDefinition, appearance);
			}
		}
		this.RefreshDefaultChosenSlimes();
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x00063024 File Offset: 0x00061224
	public void RegisterDependentAppearances(SlimeDefinition definition, SlimeAppearance appearance)
	{
		if (appearance == null)
		{
			Log.Error("Found an unassigned appearance in a slime definition.", new object[]
			{
				"SlimeDefinition",
				definition.name
			});
			return;
		}
		foreach (SlimeAppearance slimeAppearance in appearance.DependentAppearances)
		{
			if (slimeAppearance == null)
			{
				Log.Error("Found an unassigned dependent appearance in a slime appearance.", new object[]
				{
					"SlimeAppearance",
					appearance
				});
			}
			else
			{
				List<SlimeAppearanceDirector.AppearanceDefinitionPair> list;
				if (!this.appearancesByDependentAppearance.TryGetValue(slimeAppearance, out list))
				{
					list = new List<SlimeAppearanceDirector.AppearanceDefinitionPair>();
					this.appearancesByDependentAppearance.Add(slimeAppearance, list);
				}
				list.Add(new SlimeAppearanceDirector.AppearanceDefinitionPair
				{
					appearance = appearance,
					definition = definition
				});
			}
		}
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x000630D8 File Offset: 0x000612D8
	public void RefreshDefaultChosenSlimes()
	{
		foreach (SlimeDefinition slimeDefinition in this.SlimeDefinitions.Slimes)
		{
			SlimeAppearance slimeAppearance = slimeDefinition.Appearances.First<SlimeAppearance>();
			this.UnlockAppearance(slimeDefinition, slimeAppearance);
			this.UpdateChosenSlimeAppearance(slimeDefinition, slimeAppearance);
		}
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x0006311F File Offset: 0x0006131F
	public SlimeAppearance GetChosenSlimeAppearance(Identifiable.Id id)
	{
		return this.GetChosenSlimeAppearance(this.SlimeDefinitions.GetSlimeByIdentifiableId(id));
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x00063133 File Offset: 0x00061333
	public SlimeAppearance GetChosenSlimeAppearance(SlimeDefinition slimeDefinition)
	{
		return this.appearanceSelections.GetSelectedAppearance(slimeDefinition);
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x00063144 File Offset: 0x00061344
	public void UpdateChosenSlimeAppearance(SlimeDefinition definition, SlimeAppearance newChosenAppearance)
	{
		this.SetChosenSlimeAppearance(definition, newChosenAppearance);
		List<SlimeAppearanceDirector.AppearanceDefinitionPair> list;
		if (this.appearancesByDependentAppearance.TryGetValue(newChosenAppearance, out list))
		{
			foreach (SlimeAppearanceDirector.AppearanceDefinitionPair appearanceDefinitionPair in list)
			{
				if (this.AreDependentAppearancesChosen(appearanceDefinitionPair.appearance))
				{
					this.SetChosenSlimeAppearance(appearanceDefinitionPair.definition, appearanceDefinitionPair.appearance);
				}
			}
		}
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x000631C4 File Offset: 0x000613C4
	public Sprite GetCurrentSlimeIcon(Identifiable.Id slimeId)
	{
		Sprite icon = this.GetChosenSlimeAppearance(slimeId).Icon;
		if (!(icon != null))
		{
			return this.missingIcon;
		}
		return icon;
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x000631EF File Offset: 0x000613EF
	private bool AreDependentAppearancesChosen(SlimeAppearance appearance)
	{
		return appearance.DependentAppearances.All((SlimeAppearance a) => this.appearanceSelections.GetAllSelectedAppearances().Contains(a));
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x00063208 File Offset: 0x00061408
	private void SetChosenSlimeAppearance(SlimeDefinition slimeDefinition, SlimeAppearance newAppearance)
	{
		this.appearanceSelections.SelectAppearanceForSlime(slimeDefinition, newAppearance);
		this.onSlimeAppearanceChanged(slimeDefinition, newAppearance);
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x00063224 File Offset: 0x00061424
	public void UnlockAppearance(SlimeDefinition slimeDefinition, SlimeAppearance appearance)
	{
		this.appearanceSelections.UnlockAppearanceForSlime(slimeDefinition, appearance);
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x00063233 File Offset: 0x00061433
	public void LockAppearance(SlimeDefinition slimeDefinition, SlimeAppearance appearance)
	{
		this.appearanceSelections.LockAppearanceForSlime(slimeDefinition, appearance);
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x00063242 File Offset: 0x00061442
	public bool IsAppearanceUnlocked(SlimeDefinition slimeDefinition, SlimeAppearance appearance)
	{
		return this.appearanceSelections.GetUnlockedAppearances(slimeDefinition).Contains(appearance);
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x00063256 File Offset: 0x00061456
	public IEnumerable<SlimeAppearance> GetUnlockedAppearances(SlimeDefinition slimeDefinition)
	{
		return this.appearanceSelections.GetUnlockedAppearances(slimeDefinition);
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x00063264 File Offset: 0x00061464
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterAppearances(this);
		this.RefreshDefaultChosenSlimes();
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x0006327C File Offset: 0x0006147C
	public void InitModel(AppearancesModel model)
	{
		foreach (SlimeDefinition slimeDefinition in from slime in this.SlimeDefinitions.Slimes
		where !slime.IsLargo
		select slime)
		{
			SlimeAppearance appearanceForSet = slimeDefinition.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC);
			if (appearanceForSet == null)
			{
				throw new Exception("No classic appearance available for slime " + slimeDefinition.Name);
			}
			model.AppearanceSelections.UnlockAppearanceForSlime(slimeDefinition, appearanceForSet);
			model.AppearanceSelections.SelectAppearanceForSlime(slimeDefinition, appearanceForSet);
		}
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x0006332C File Offset: 0x0006152C
	public void SetModel(AppearancesModel model)
	{
		this.appearanceSelections = model.AppearanceSelections;
		foreach (Identifiable.Id id in model.unlocks.Keys.ToList<Identifiable.Id>())
		{
			SlimeDefinition slime = this.SlimeDefinitions.GetSlimeByIdentifiableId(id);
			foreach (SlimeAppearance appearance in (from saveSet in model.unlocks[id]
			select slime.GetAppearanceForSet(saveSet)).ToList<SlimeAppearance>())
			{
				this.UnlockAppearance(slime, appearance);
			}
		}
		foreach (Identifiable.Id id2 in model.selections.Keys.ToList<Identifiable.Id>())
		{
			SlimeDefinition slimeByIdentifiableId = this.SlimeDefinitions.GetSlimeByIdentifiableId(id2);
			this.UpdateChosenSlimeAppearance(slimeByIdentifiableId, slimeByIdentifiableId.GetAppearanceForSet(model.selections[id2]));
		}
	}

	// Token: 0x04001915 RID: 6421
	public SlimeDefinitions SlimeDefinitions;

	// Token: 0x04001916 RID: 6422
	[Tooltip("The icon to show for slime appearances that don't have an icon defined.")]
	public Sprite missingIcon;

	// Token: 0x04001917 RID: 6423
	[Tooltip("SlimeAppearancePopupUI prefab reference.")]
	public GameObject appearancePopupUI;

	// Token: 0x04001918 RID: 6424
	[Tooltip("The default animator controller to use for slimes.")]
	public RuntimeAnimatorController defaultAnimatorController;

	// Token: 0x0400191A RID: 6426
	private readonly Dictionary<SlimeAppearance, List<SlimeAppearanceDirector.AppearanceDefinitionPair>> appearancesByDependentAppearance = new Dictionary<SlimeAppearance, List<SlimeAppearanceDirector.AppearanceDefinitionPair>>(SlimeAppearance.DefaultComparer);

	// Token: 0x0400191B RID: 6427
	private AppearanceSelections appearanceSelections = new AppearanceSelections();

	// Token: 0x020004C0 RID: 1216
	// (Invoke) Token: 0x06001983 RID: 6531
	public delegate void OnSlimeAppearanceChangedDelegate(SlimeDefinition slime, SlimeAppearance appearance);

	// Token: 0x020004C1 RID: 1217
	private class AppearanceDefinitionPair
	{
		// Token: 0x0400191C RID: 6428
		public SlimeAppearance appearance;

		// Token: 0x0400191D RID: 6429
		public SlimeDefinition definition;
	}
}
