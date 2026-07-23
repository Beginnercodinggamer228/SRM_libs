using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class LookupDirector : SRBehaviour
{
	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000BDA RID: 3034 RVA: 0x0003181E File Offset: 0x0002FA1E
	public IEnumerable<GameObject> PlotPrefabs
	{
		get
		{
			return this.plotPrefabs;
		}
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000BDB RID: 3035 RVA: 0x00031826 File Offset: 0x0002FA26
	public IEnumerable<GadgetDefinition> GadgetDefinitions
	{
		get
		{
			return this.gadgetDefinitions.Concat(this.gadgetDefinitionsDynamic);
		}
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000BDC RID: 3036 RVA: 0x00031839 File Offset: 0x0002FA39
	public IEnumerable<VacItemDefinition> VacItemDefinitions
	{
		get
		{
			return this.vacItemDefinitions.Concat(this.vacItemDefinitionsDynamic);
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000BDD RID: 3037 RVA: 0x0003184C File Offset: 0x0002FA4C
	public IEnumerable<GameObject> GordoEntries
	{
		get
		{
			return this.gordoEntries;
		}
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00031854 File Offset: 0x0002FA54
	public void Awake()
	{
		this.identifiablePrefabDict.Clear();
		foreach (GameObject gameObject in this.identifiablePrefabs)
		{
			if (!(gameObject == null))
			{
				Identifiable component = gameObject.GetComponent<Identifiable>();
				if (this.identifiablePrefabDict.ContainsKey(component.id))
				{
					Log.Error("LookupDirector Duplicate Identifiable ID: " + component.id, Array.Empty<object>());
				}
				this.identifiablePrefabDict[component.id] = gameObject;
			}
		}
		this.plotPrefabDict.Clear();
		foreach (GameObject gameObject2 in this.plotPrefabs)
		{
			if (!(gameObject2 == null))
			{
				LandPlot component2 = gameObject2.GetComponent<LandPlot>();
				if (this.plotPrefabDict.ContainsKey(component2.typeId))
				{
					Log.Error("LookupDirector Duplicate Plot ID: " + component2.typeId, Array.Empty<object>());
				}
				this.plotPrefabDict[component2.typeId] = gameObject2;
			}
		}
		this.resourcePrefabDict.Clear();
		foreach (GameObject gameObject3 in this.resourceSpawnerPrefabs)
		{
			if (!(gameObject3 == null))
			{
				SpawnResource component3 = gameObject3.GetComponent<SpawnResource>();
				if (this.resourcePrefabDict.ContainsKey(component3.id))
				{
					Log.Error("LookupDirector Duplicate Resource ID: " + component3.id, Array.Empty<object>());
				}
				this.resourcePrefabDict[component3.id] = gameObject3;
			}
		}
		this.gadgetDefinitionDict.Clear();
		foreach (GadgetDefinition gadgetDefinition in this.gadgetDefinitions)
		{
			if (!(gadgetDefinition.prefab == null))
			{
				Gadget component4 = gadgetDefinition.prefab.GetComponent<Gadget>();
				if (this.gadgetDefinitionDict.ContainsKey(component4.id))
				{
					Log.Error("LookupDirector Duplicate Gadget ID: " + component4.id, Array.Empty<object>());
				}
				if (gadgetDefinition.id != component4.id)
				{
					Log.Error("LookupDirector Mismatch Gadget.", new object[]
					{
						"entryId",
						gadgetDefinition.id,
						"gadgetId",
						component4.id
					});
				}
				this.gadgetDefinitionDict[component4.id] = gadgetDefinition;
			}
		}
		this.vacItemDict.Clear();
		foreach (VacItemDefinition vacItemDefinition in this.vacItemDefinitions)
		{
			if (this.vacItemDict.ContainsKey(vacItemDefinition.Id))
			{
				Log.Error("LookupDirector Duplicate Vac Item Definition ID: " + vacItemDefinition.Id, Array.Empty<object>());
			}
			this.vacItemDict[vacItemDefinition.Id] = vacItemDefinition;
		}
		this.liquidDict.Clear();
		foreach (LiquidDefinition liquidDefinition in this.liquidDefinitions)
		{
			if (this.liquidDict.ContainsKey(liquidDefinition.Id))
			{
				Log.Error("LookupDirector Duplicate Liquid ID: " + liquidDefinition.Id, Array.Empty<object>());
			}
			this.liquidDict[liquidDefinition.Id] = liquidDefinition;
		}
		this.upgradeDefinitionDict.Clear();
		foreach (UpgradeDefinition upgradeDefinition in this.upgradeDefinitions)
		{
			if (this.upgradeDefinitionDict.ContainsKey(upgradeDefinition.Upgrade))
			{
				Log.Error("LookupDirector Duplicate Upgrade ID: " + upgradeDefinition.Upgrade, Array.Empty<object>());
			}
			this.upgradeDefinitionDict[upgradeDefinition.Upgrade] = upgradeDefinition;
		}
		this.gordoDict.Clear();
		foreach (GameObject gameObject4 in this.gordoEntries)
		{
			GordoIdentifiable component5 = gameObject4.GetComponent<GordoIdentifiable>();
			if (this.gordoDict.ContainsKey(component5.id))
			{
				Log.Error("LookupDirector Duplicate Gordo ID: " + component5.id, Array.Empty<object>());
			}
			else
			{
				this.gordoDict.Add(component5.id, gameObject4);
			}
		}
		this.toyDict.Clear();
		foreach (ToyDefinition toyDefinition in this.toyDefinitions)
		{
			this.toyDict.Add(toyDefinition.ToyId, toyDefinition);
		}
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x00031DC8 File Offset: 0x0002FFC8
	public GameObject GetPrefab(Identifiable.Id id)
	{
		if (!this.identifiablePrefabDict.ContainsKey(id))
		{
			Log.Error(string.Concat(new object[]
			{
				"Missing prefab: ",
				id,
				" hasIdsCount: ",
				this.identifiablePrefabDict.Count
			}), Array.Empty<object>());
			return null;
		}
		if (this.identifiablePrefabDict[id] == null)
		{
			Log.Error("No prefab wired up for identifiable", new object[]
			{
				"id",
				id
			});
			return null;
		}
		return this.identifiablePrefabDict[id];
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x00031E69 File Offset: 0x00030069
	public GameObject GetPlotPrefab(LandPlot.Id id)
	{
		return this.plotPrefabDict[id];
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x00031E78 File Offset: 0x00030078
	public GameObject GetResourcePrefab(SpawnResource.Id id)
	{
		GameObject result;
		try
		{
			result = this.resourcePrefabDict[id];
		}
		catch (KeyNotFoundException innerException)
		{
			throw new KeyNotFoundException(string.Format("Failed to find spawn resource entry: {0}", id), innerException);
		}
		return result;
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x00031EC0 File Offset: 0x000300C0
	public GadgetDefinition GetGadgetDefinition(Gadget.Id id)
	{
		GadgetDefinition result;
		try
		{
			result = this.gadgetDefinitionDict[id];
		}
		catch (KeyNotFoundException innerException)
		{
			throw new KeyNotFoundException(string.Format("Failed to find gadget definition: {0}", id), innerException);
		}
		return result;
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x00031F08 File Offset: 0x00030108
	public bool HasGadgetDefinition(Gadget.Id id)
	{
		return this.gadgetDefinitionDict.ContainsKey(id);
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x00031F18 File Offset: 0x00030118
	public void RegisterFashion(GameObject prefab, VacItemDefinition vac, GadgetDefinition gadget)
	{
		this.identifiablePrefabDict[vac.Id] = prefab;
		this.vacItemDict[vac.Id] = vac;
		this.vacItemDefinitionsDynamic.RemoveAll((VacItemDefinition e) => e.Id == vac.Id);
		this.vacItemDefinitionsDynamic.Add(vac);
		this.gadgetDefinitionDict[gadget.id] = gadget;
		this.gadgetDefinitionsDynamic.RemoveAll((GadgetDefinition e) => e.id == gadget.id);
		this.gadgetDefinitionsDynamic.Add(gadget);
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x00031FDA File Offset: 0x000301DA
	public UpgradeDefinition GetUpgradeDefinition(PlayerState.Upgrade upgrade)
	{
		return this.upgradeDefinitionDict[upgrade];
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x00031FE8 File Offset: 0x000301E8
	public ToyDefinition GetToyDefinition(Identifiable.Id id)
	{
		return this.toyDict[id];
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x00031FF6 File Offset: 0x000301F6
	public void RegisterToy(ToyDefinition entry, GameObject prefab)
	{
		this.identifiablePrefabDict[entry.ToyId] = prefab;
		this.toyDict[entry.ToyId] = entry;
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x0003201C File Offset: 0x0003021C
	public Color GetColor(Identifiable.Id id)
	{
		VacItemDefinition vacItemDefinition;
		if (this.vacItemDict.TryGetValue(id, out vacItemDefinition))
		{
			return vacItemDefinition.Color;
		}
		return Color.clear;
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00032048 File Offset: 0x00030248
	public Sprite GetIcon(Identifiable.Id id)
	{
		try
		{
			if (Identifiable.IsSlime(id))
			{
				return this.slimeAppearanceDirector.GetCurrentSlimeIcon(id);
			}
			if (id != Identifiable.Id.NONE)
			{
				return this.vacItemDict[id].Icon;
			}
		}
		catch (KeyNotFoundException ex)
		{
			Log.Error("Failed to find Identifiable Id when looking up icon.", new object[]
			{
				"Id",
				id.ToString(),
				"Exception",
				ex
			});
		}
		return null;
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x000320D0 File Offset: 0x000302D0
	public GameObject GetLiquidIncomingFX(Identifiable.Id id)
	{
		if (this.liquidDict.ContainsKey(id))
		{
			return this.liquidDict[id].InFx;
		}
		return null;
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x000320F3 File Offset: 0x000302F3
	public GameObject GetLiquidVacFailFX(Identifiable.Id id)
	{
		if (this.liquidDict.ContainsKey(id))
		{
			return this.liquidDict[id].VacFailFx;
		}
		return null;
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00032116 File Offset: 0x00030316
	public GameObject GetGordo(Identifiable.Id id)
	{
		if (this.gordoDict.ContainsKey(id))
		{
			return this.gordoDict[id];
		}
		return null;
	}

	// Token: 0x04000ABE RID: 2750
	[SerializeField]
	private SlimeAppearanceDirector slimeAppearanceDirector;

	// Token: 0x04000ABF RID: 2751
	[SerializeField]
	private PrefabList identifiablePrefabs;

	// Token: 0x04000AC0 RID: 2752
	[SerializeField]
	private PrefabList plotPrefabs;

	// Token: 0x04000AC1 RID: 2753
	[SerializeField]
	private PrefabList resourceSpawnerPrefabs;

	// Token: 0x04000AC2 RID: 2754
	[SerializeField]
	private GadgetDefinitionList gadgetDefinitions;

	// Token: 0x04000AC3 RID: 2755
	[SerializeField]
	private VacItemDefinitionList vacItemDefinitions;

	// Token: 0x04000AC4 RID: 2756
	[SerializeField]
	private LiquidDefinitionList liquidDefinitions;

	// Token: 0x04000AC5 RID: 2757
	[SerializeField]
	private UpgradeDefinitionList upgradeDefinitions;

	// Token: 0x04000AC6 RID: 2758
	[SerializeField]
	private PrefabList gordoEntries;

	// Token: 0x04000AC7 RID: 2759
	[SerializeField]
	private ToyDefinitionList toyDefinitions;

	// Token: 0x04000AC8 RID: 2760
	private readonly List<GadgetDefinition> gadgetDefinitionsDynamic = new List<GadgetDefinition>();

	// Token: 0x04000AC9 RID: 2761
	private readonly List<VacItemDefinition> vacItemDefinitionsDynamic = new List<VacItemDefinition>();

	// Token: 0x04000ACA RID: 2762
	private Dictionary<Identifiable.Id, GameObject> identifiablePrefabDict = new Dictionary<Identifiable.Id, GameObject>(Identifiable.idComparer);

	// Token: 0x04000ACB RID: 2763
	private Dictionary<LandPlot.Id, GameObject> plotPrefabDict = new Dictionary<LandPlot.Id, GameObject>(LandPlot.idComparer);

	// Token: 0x04000ACC RID: 2764
	private Dictionary<SpawnResource.Id, GameObject> resourcePrefabDict = new Dictionary<SpawnResource.Id, GameObject>(SpawnResource.idComparer);

	// Token: 0x04000ACD RID: 2765
	private Dictionary<Gadget.Id, GadgetDefinition> gadgetDefinitionDict = new Dictionary<Gadget.Id, GadgetDefinition>(Gadget.idComparer);

	// Token: 0x04000ACE RID: 2766
	private Dictionary<Identifiable.Id, VacItemDefinition> vacItemDict = new Dictionary<Identifiable.Id, VacItemDefinition>(Identifiable.idComparer);

	// Token: 0x04000ACF RID: 2767
	private Dictionary<Identifiable.Id, LiquidDefinition> liquidDict = new Dictionary<Identifiable.Id, LiquidDefinition>(Identifiable.idComparer);

	// Token: 0x04000AD0 RID: 2768
	private Dictionary<PlayerState.Upgrade, UpgradeDefinition> upgradeDefinitionDict = new Dictionary<PlayerState.Upgrade, UpgradeDefinition>(PlayerState.upgradeComparer);

	// Token: 0x04000AD1 RID: 2769
	private Dictionary<Identifiable.Id, GameObject> gordoDict = new Dictionary<Identifiable.Id, GameObject>(Identifiable.idComparer);

	// Token: 0x04000AD2 RID: 2770
	private Dictionary<Identifiable.Id, ToyDefinition> toyDict = new Dictionary<Identifiable.Id, ToyDefinition>(Identifiable.idComparer);
}
