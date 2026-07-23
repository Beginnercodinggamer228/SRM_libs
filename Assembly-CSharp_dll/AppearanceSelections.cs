using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000389 RID: 905
public class AppearanceSelections
{
	// Token: 0x14000012 RID: 18
	// (add) Token: 0x060012D8 RID: 4824 RVA: 0x00049D00 File Offset: 0x00047F00
	// (remove) Token: 0x060012D9 RID: 4825 RVA: 0x00049D38 File Offset: 0x00047F38
	public event AppearanceSelections.OnAppearanceUnlockedForSlimeDelegate onAppearanceUnlocked = delegate(SlimeDefinition <p0>, SlimeAppearance <p1>)
	{
	};

	// Token: 0x14000013 RID: 19
	// (add) Token: 0x060012DA RID: 4826 RVA: 0x00049D70 File Offset: 0x00047F70
	// (remove) Token: 0x060012DB RID: 4827 RVA: 0x00049DA8 File Offset: 0x00047FA8
	public event AppearanceSelections.OnAppearanceSelectedForSlimeDelegate onAppearanceSelected = delegate(SlimeDefinition <p0>, SlimeAppearance <p1>)
	{
	};

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x060012DC RID: 4828 RVA: 0x00049DE0 File Offset: 0x00047FE0
	// (remove) Token: 0x060012DD RID: 4829 RVA: 0x00049E18 File Offset: 0x00048018
	public event AppearanceSelections.OnAppearanceLockedForSlimeDelegate onAppearanceLocked = delegate(SlimeDefinition <p0>, SlimeAppearance <p1>)
	{
	};

	// Token: 0x060012DE RID: 4830 RVA: 0x00049E50 File Offset: 0x00048050
	public void UnlockAppearanceForSlime(SlimeDefinition slime, SlimeAppearance appearance)
	{
		if (!slime.Appearances.Contains(appearance))
		{
			Log.Error(string.Format("Trying to unlock appearance {0} not attached to a slime definition {1}.", appearance.name, slime.Name), Array.Empty<object>());
			return;
		}
		this.GetOrCreateUnlockSetForSlime(slime).Add(appearance);
		this.onAppearanceUnlocked(slime, appearance);
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x00049EA8 File Offset: 0x000480A8
	public void LockAppearanceForSlime(SlimeDefinition slime, SlimeAppearance appearance)
	{
		HashSet<SlimeAppearance> hashSet;
		if (this.unlocks.TryGetValue(slime.IdentifiableId, out hashSet))
		{
			hashSet.Remove(appearance);
		}
		this.onAppearanceLocked(slime, appearance);
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x00049EE0 File Offset: 0x000480E0
	public void SelectAppearanceForSlime(SlimeDefinition slime, SlimeAppearance appearance)
	{
		SlimeAppearance selectedAppearance = this.GetSelectedAppearance(slime);
		if (selectedAppearance != null)
		{
			this.allSelectedAppearances.Remove(selectedAppearance);
		}
		this.allSelectedAppearances.Add(appearance);
		this.selections[slime.IdentifiableId] = appearance;
		this.onAppearanceSelected(slime, appearance);
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x00049F38 File Offset: 0x00048138
	public SlimeAppearance GetSelectedAppearance(SlimeDefinition slime)
	{
		SlimeAppearance result;
		if (!this.selections.TryGetValue(slime.IdentifiableId, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x00049F5D File Offset: 0x0004815D
	public List<SlimeAppearance> GetUnlockedAppearances(SlimeDefinition slime)
	{
		return this.GetOrCreateUnlockSetForSlime(slime).ToList<SlimeAppearance>();
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x00049F6B File Offset: 0x0004816B
	public HashSet<SlimeAppearance> GetAllSelectedAppearances()
	{
		return this.allSelectedAppearances;
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x00049F74 File Offset: 0x00048174
	private HashSet<SlimeAppearance> GetOrCreateUnlockSetForSlime(SlimeDefinition slime)
	{
		HashSet<SlimeAppearance> hashSet;
		if (!this.unlocks.TryGetValue(slime.IdentifiableId, out hashSet))
		{
			hashSet = new HashSet<SlimeAppearance>();
			this.unlocks[slime.IdentifiableId] = hashSet;
		}
		return hashSet;
	}

	// Token: 0x040011D2 RID: 4562
	private readonly Dictionary<Identifiable.Id, HashSet<SlimeAppearance>> unlocks = new Dictionary<Identifiable.Id, HashSet<SlimeAppearance>>();

	// Token: 0x040011D3 RID: 4563
	private readonly Dictionary<Identifiable.Id, SlimeAppearance> selections = new Dictionary<Identifiable.Id, SlimeAppearance>();

	// Token: 0x040011D4 RID: 4564
	private readonly HashSet<SlimeAppearance> allSelectedAppearances = new HashSet<SlimeAppearance>(SlimeAppearanceEqualityComparer.Default);

	// Token: 0x0200038A RID: 906
	// (Invoke) Token: 0x060012E7 RID: 4839
	public delegate void OnAppearanceUnlockedForSlimeDelegate(SlimeDefinition slime, SlimeAppearance appearance);

	// Token: 0x0200038B RID: 907
	// (Invoke) Token: 0x060012EB RID: 4843
	public delegate void OnAppearanceSelectedForSlimeDelegate(SlimeDefinition slime, SlimeAppearance appearance);

	// Token: 0x0200038C RID: 908
	// (Invoke) Token: 0x060012EF RID: 4847
	public delegate void OnAppearanceLockedForSlimeDelegate(SlimeDefinition slime, SlimeAppearance appearance);
}
