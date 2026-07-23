using System;
using System.Collections;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class Identifiable : SRBehaviour, ActorModel.Participant
{
	// Token: 0x06000B2C RID: 2860 RVA: 0x0002F168 File Offset: 0x0002D368
	static Identifiable()
	{
		foreach (object obj in Enum.GetValues(typeof(Identifiable.Id)))
		{
			Identifiable.Id id = (Identifiable.Id)obj;
			string name = Enum.GetName(typeof(Identifiable.Id), id);
			if (name.EndsWith("_VEGGIE"))
			{
				Identifiable.VEGGIE_CLASS.Add(id);
				Identifiable.FOOD_CLASS.Add(id);
				Identifiable.NON_SLIMES_CLASS.Add(id);
			}
			else if (name.EndsWith("_FRUIT"))
			{
				Identifiable.FRUIT_CLASS.Add(id);
				Identifiable.FOOD_CLASS.Add(id);
				Identifiable.NON_SLIMES_CLASS.Add(id);
			}
			else if (name.EndsWith("_TOFU"))
			{
				Identifiable.TOFU_CLASS.Add(id);
				Identifiable.FOOD_CLASS.Add(id);
				Identifiable.NON_SLIMES_CLASS.Add(id);
			}
			else if (name.EndsWith("_SLIME"))
			{
				Identifiable.SLIME_CLASS.Add(id);
			}
			else if (name.EndsWith("_LARGO"))
			{
				Identifiable.LARGO_CLASS.Add(id);
			}
			else if (name.EndsWith("_GORDO"))
			{
				Identifiable.GORDO_CLASS.Add(id);
			}
			else if (name.EndsWith("_PLORT"))
			{
				Identifiable.PLORT_CLASS.Add(id);
				Identifiable.NON_SLIMES_CLASS.Add(id);
			}
			else if (name.EndsWith("HEN") || name.EndsWith("ROOSTER"))
			{
				Identifiable.MEAT_CLASS.Add(id);
				Identifiable.FOOD_CLASS.Add(id);
				Identifiable.NON_SLIMES_CLASS.Add(id);
			}
			else if (id == Identifiable.Id.CHICK || name.EndsWith("_CHICK"))
			{
				Identifiable.NON_SLIMES_CLASS.Add(id);
				Identifiable.CHICK_CLASS.Add(id);
			}
			else if (name.EndsWith("_LIQUID"))
			{
				Identifiable.LIQUID_CLASS.Add(id);
			}
			else if (name.EndsWith("_CRAFT"))
			{
				Identifiable.CRAFT_CLASS.Add(id);
				Identifiable.NON_SLIMES_CLASS.Add(id);
			}
			else if (name.EndsWith("_FASHION"))
			{
				Identifiable.FASHION_CLASS.Add(id);
			}
			else if (name.EndsWith("_ECHO"))
			{
				Identifiable.ECHO_CLASS.Add(id);
			}
			else if (name.StartsWith("ECHO_NOTE_"))
			{
				Identifiable.ECHO_NOTE_CLASS.Add(id);
			}
			else if (name.EndsWith("_ORNAMENT"))
			{
				Identifiable.ORNAMENT_CLASS.Add(id);
			}
			else if (name.EndsWith("_TOY") || id == Identifiable.Id.KOOKADOBA_BALL)
			{
				Identifiable.TOY_CLASS.Add(id);
			}
			if (name.Contains("TANGLE"))
			{
				Identifiable.ALLERGY_FREE_CLASS.Add(id);
			}
		}
		Identifiable.EATERS_CLASS.UnionWith(Identifiable.SLIME_CLASS);
		Identifiable.EATERS_CLASS.UnionWith(Identifiable.LARGO_CLASS);
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000B2D RID: 2861 RVA: 0x0002F82C File Offset: 0x0002DA2C
	// (set) Token: 0x06000B2E RID: 2862 RVA: 0x0002F834 File Offset: 0x0002DA34
	public bool isPhysicsInitialized { get; private set; }

	// Token: 0x06000B2F RID: 2863 RVA: 0x0002F83D File Offset: 0x0002DA3D
	private IEnumerator SetPhysicsInitialized()
	{
		yield return new WaitForFixedUpdate();
		this.isPhysicsInitialized = true;
		yield break;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0002F84C File Offset: 0x0002DA4C
	public void Awake()
	{
		this.member = base.GetComponent<RegionMember>();
		if (this.id == Identifiable.Id.PLAYER)
		{
			SRSingleton<SceneContext>.Instance.Player = base.gameObject;
		}
		if (Identifiable.SCENE_OBJECTS.Contains(this.id))
		{
			SRSingleton<SceneContext>.Instance.GameModel.RegisterStartingActor(base.gameObject, this.GetStartingActorRegion());
		}
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0002F8AC File Offset: 0x0002DAAC
	private RegionRegistry.RegionSetId GetStartingActorRegion()
	{
		Identifiable.Id id = this.id;
		if (id <= Identifiable.Id.SCARECROW)
		{
			if (id == Identifiable.Id.PLAYER)
			{
				return RegionRegistry.RegionSetId.HOME;
			}
			if (id != Identifiable.Id.SCARECROW)
			{
				goto IL_41;
			}
		}
		else
		{
			if (id == Identifiable.Id.FIRE_COLUMN)
			{
				return RegionRegistry.RegionSetId.DESERT;
			}
			if (id != Identifiable.Id.PORTABLE_SCARECROW)
			{
				if (id != Identifiable.Id.GLITCH_TARR_PORTAL)
				{
					goto IL_41;
				}
				return RegionRegistry.RegionSetId.SLIMULATIONS;
			}
		}
		return base.GetComponentInParent<Region>().setId;
		IL_41:
		throw new ArgumentException(string.Format("Failed to get RegionSetId for starting actor. [id={0}]", this.id));
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0002F914 File Offset: 0x0002DB14
	public void OnEnable()
	{
		base.StartCoroutine(this.SetPhysicsInitialized());
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0002F923 File Offset: 0x0002DB23
	public void OnDisable()
	{
		this.isPhysicsInitialized = false;
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0002F92C File Offset: 0x0002DB2C
	public void OnDestroy()
	{
		if (this.model != null)
		{
			CellDirector.UnregisterFromAll(this.member, base.gameObject, this.model);
		}
		this.member.regionsChanged -= this.OnMemberChanged_UpdateRegistration;
		if (this.NotifyOnDestroy != null)
		{
			this.NotifyOnDestroy(this);
		}
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0002F983 File Offset: 0x0002DB83
	private static CellDirector GetDirector(Region region)
	{
		return region.cellDir;
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0002F98B File Offset: 0x0002DB8B
	public static bool IsSlime(Identifiable.Id id)
	{
		return Identifiable.SLIME_CLASS.Contains(id) || Identifiable.LARGO_CLASS.Contains(id);
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0002F9A7 File Offset: 0x0002DBA7
	public static bool IsGordo(Identifiable.Id id)
	{
		return Identifiable.GORDO_CLASS.Contains(id);
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0002F9B4 File Offset: 0x0002DBB4
	public static bool IsLargo(Identifiable.Id id)
	{
		return Identifiable.LARGO_CLASS.Contains(id);
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0002F9C1 File Offset: 0x0002DBC1
	public static bool IsPlort(Identifiable.Id id)
	{
		return Identifiable.PLORT_CLASS.Contains(id);
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0002F9CE File Offset: 0x0002DBCE
	public static bool IsAnimal(Identifiable.Id id)
	{
		return Identifiable.MEAT_CLASS.Contains(id) || Identifiable.CHICK_CLASS.Contains(id);
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0002F9EA File Offset: 0x0002DBEA
	public static bool IsChick(Identifiable.Id id)
	{
		return Identifiable.CHICK_CLASS.Contains(id);
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x0002F9F7 File Offset: 0x0002DBF7
	public static bool IsFood(Identifiable.Id id)
	{
		return Identifiable.FOOD_CLASS.Contains(id);
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x0002FA04 File Offset: 0x0002DC04
	public static bool IsVeggie(Identifiable.Id id)
	{
		return Identifiable.VEGGIE_CLASS.Contains(id);
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0002FA11 File Offset: 0x0002DC11
	public static bool IsFruit(Identifiable.Id id)
	{
		return Identifiable.FRUIT_CLASS.Contains(id);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0002FA1E File Offset: 0x0002DC1E
	public static bool IsCraft(Identifiable.Id id)
	{
		return Identifiable.CRAFT_CLASS.Contains(id);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0002FA2B File Offset: 0x0002DC2B
	public static bool IsEcho(Identifiable.Id id)
	{
		return Identifiable.ECHO_CLASS.Contains(id);
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0002FA38 File Offset: 0x0002DC38
	public static bool IsEchoNote(Identifiable.Id id)
	{
		return Identifiable.ECHO_NOTE_CLASS.Contains(id);
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0002FA45 File Offset: 0x0002DC45
	public static bool IsOrnament(Identifiable.Id id)
	{
		return Identifiable.ORNAMENT_CLASS.Contains(id);
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x0002FA52 File Offset: 0x0002DC52
	public static bool IsToy(Identifiable.Id id)
	{
		return Identifiable.TOY_CLASS.Contains(id);
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0002FA5F File Offset: 0x0002DC5F
	public static bool IsLiquid(Identifiable.Id id)
	{
		return Identifiable.LIQUID_CLASS.Contains(id);
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0002FA6C File Offset: 0x0002DC6C
	public static bool IsWater(Identifiable.Id id)
	{
		return id != Identifiable.Id.GLITCH_DEBUG_SPRAY_LIQUID && Identifiable.LIQUID_CLASS.Contains(id);
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x0002FA83 File Offset: 0x0002DC83
	public static bool IsFashion(Identifiable.Id id)
	{
		return Identifiable.FASHION_CLASS.Contains(id);
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0002FA90 File Offset: 0x0002DC90
	public static bool IsNonSlimeResource(Identifiable.Id id)
	{
		return Identifiable.NON_SLIMES_CLASS.Contains(id);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0002FA9D File Offset: 0x0002DC9D
	public static bool IsAllergyFree(Identifiable.Id id)
	{
		return Identifiable.ALLERGY_FREE_CLASS.Contains(id);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0002FAAA File Offset: 0x0002DCAA
	public static bool IsTarr(Identifiable.Id id)
	{
		return Identifiable.TARR_CLASS.Contains(id);
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0002FAB8 File Offset: 0x0002DCB8
	public static Identifiable.Id Combine(List<Identifiable.Id> ids)
	{
		if (ids.Count == 0)
		{
			return Identifiable.Id.NONE;
		}
		if (ids.Count == 1)
		{
			return ids[0];
		}
		if (ids.Count == 2)
		{
			string name = Enum.GetName(typeof(Identifiable.Id), ids[0]);
			string name2 = Enum.GetName(typeof(Identifiable.Id), ids[1]);
			if (!name.EndsWith("_SLIME") || !name2.EndsWith("_SLIME"))
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Illegal identifiable to combine, must be slime: ",
					ids[0],
					",",
					ids[1]
				}));
			}
			string text = name.Substring(0, name.Length - "_SLIME".Length);
			string text2 = name2.Substring(0, name2.Length - "_SLIME".Length);
			try
			{
				return (Identifiable.Id)Enum.Parse(typeof(Identifiable.Id), text + "_" + text2 + "_LARGO");
			}
			catch (Exception)
			{
				try
				{
					return (Identifiable.Id)Enum.Parse(typeof(Identifiable.Id), text2 + "_" + text + "_LARGO");
				}
				catch (Exception)
				{
					return Identifiable.Id.TARR_SLIME;
				}
			}
			return Identifiable.Id.TARR_SLIME;
		}
		return Identifiable.Id.TARR_SLIME;
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x0002FC2C File Offset: 0x0002DE2C
	public static Identifiable.Id GetId(GameObject gameObject)
	{
		Identifiable component = gameObject.GetComponent<Identifiable>();
		if (component != null)
		{
			return component.id;
		}
		return Identifiable.Id.NONE;
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0002FC54 File Offset: 0x0002DE54
	public static long GetActorId(GameObject gameObject)
	{
		Identifiable component = gameObject.GetComponent<Identifiable>();
		if (component != null)
		{
			return component.GetActorId();
		}
		throw new InvalidOperationException("Tried to get an actor ID for an object that had none: " + gameObject.name);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0002FC90 File Offset: 0x0002DE90
	public static string GetName(Identifiable.Id id, bool reportMissing = true)
	{
		Gadget.Id id2;
		if (Identifiable.GADGET_NAME_DICT.TryGetValue(id, out id2))
		{
			return Gadget.GetName(id2, reportMissing);
		}
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("actor");
		string key = string.Format("l.{0}", Enum.GetName(typeof(Identifiable.Id), id).ToLowerInvariant());
		string resourceString = bundle.GetResourceString(key, false);
		if (resourceString != null)
		{
			return resourceString;
		}
		MessageBundle bundle2 = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("pedia");
		key = string.Format("t.{0}", Enum.GetName(typeof(Identifiable.Id), id).ToLowerInvariant());
		resourceString = bundle2.GetResourceString(key, false);
		if (resourceString != null)
		{
			return resourceString;
		}
		PediaDirector.Id? pediaId = SRSingleton<SceneContext>.Instance.PediaDirector.GetPediaId(id);
		if (pediaId != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("pedia");
			key = string.Format("t.{0}", Enum.GetName(typeof(PediaDirector.Id), pediaId).ToLowerInvariant());
			if (resourceString != null)
			{
				return resourceString;
			}
		}
		if (reportMissing)
		{
			Log.Warning("Missing identifiable translation.", new object[]
			{
				"id",
				id
			});
		}
		return null;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0002FDBE File Offset: 0x0002DFBE
	public long GetActorId()
	{
		return this.model.actorId;
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(ActorModel model)
	{
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0002FDCC File Offset: 0x0002DFCC
	public void SetModel(ActorModel model)
	{
		this.model = model;
		this.member.regionsChanged += this.OnMemberChanged_UpdateRegistration;
		foreach (Region region in this.member.regions)
		{
			region.cellDir.Register(base.gameObject, model);
		}
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x0002FE4C File Offset: 0x0002E04C
	private void OnMemberChanged_UpdateRegistration(List<Region> left, List<Region> joined)
	{
		if (left != null)
		{
			foreach (Region region in left)
			{
				try
				{
					Identifiable.GetDirector(region).Unregister(base.gameObject, this.model);
				}
				catch (MissingReferenceException)
				{
				}
			}
		}
		if (joined != null)
		{
			foreach (Region region2 in joined)
			{
				try
				{
					Identifiable.GetDirector(region2).Register(base.gameObject, this.model);
				}
				catch (MissingReferenceException)
				{
				}
			}
		}
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0002FF20 File Offset: 0x0002E120
	public static bool IsEdible(GameObject gameObject)
	{
		Identifiable component = gameObject.GetComponent<Identifiable>();
		return component != null && component.IsEdible();
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0002FF45 File Offset: 0x0002E145
	public bool IsEdible()
	{
		return this.model != null && this.model.IsEdible();
	}

	// Token: 0x0400090D RID: 2317
	public static Identifiable.IdComparer idComparer = new Identifiable.IdComparer();

	// Token: 0x0400090E RID: 2318
	public Identifiable.Id id;

	// Token: 0x0400090F RID: 2319
	private ActorModel model;

	// Token: 0x04000910 RID: 2320
	private RegionMember member;

	// Token: 0x04000911 RID: 2321
	private const string VEGGIE_SUFFIX = "_VEGGIE";

	// Token: 0x04000912 RID: 2322
	private const string FRUIT_SUFFIX = "_FRUIT";

	// Token: 0x04000913 RID: 2323
	private const string TOFU_SUFFIX = "_TOFU";

	// Token: 0x04000914 RID: 2324
	private const string HEN_SUFFIX = "HEN";

	// Token: 0x04000915 RID: 2325
	private const string ROOSTER_SUFFIX = "ROOSTER";

	// Token: 0x04000916 RID: 2326
	private const string CHICK_SUFFIX = "_CHICK";

	// Token: 0x04000917 RID: 2327
	private const string SLIME_SUFFIX = "_SLIME";

	// Token: 0x04000918 RID: 2328
	private const string LARGO_SUFFIX = "_LARGO";

	// Token: 0x04000919 RID: 2329
	private const string GORDO_SUFFIX = "_GORDO";

	// Token: 0x0400091A RID: 2330
	private const string PLORT_SUFFIX = "_PLORT";

	// Token: 0x0400091B RID: 2331
	private const string LIQUID_SUFFIX = "_LIQUID";

	// Token: 0x0400091C RID: 2332
	private const string CRAFT_SUFFIX = "_CRAFT";

	// Token: 0x0400091D RID: 2333
	private const string FASHION_SUFFIX = "_FASHION";

	// Token: 0x0400091E RID: 2334
	private const string ECHO_SUFFIX = "_ECHO";

	// Token: 0x0400091F RID: 2335
	private const string ECHO_NOTE_PREFIX = "ECHO_NOTE_";

	// Token: 0x04000920 RID: 2336
	private const string ORNAMENT_SUFFIX = "_ORNAMENT";

	// Token: 0x04000921 RID: 2337
	private const string TOY_SUFFIX = "_TOY";

	// Token: 0x04000922 RID: 2338
	private const string TANGLE_STRING = "TANGLE";

	// Token: 0x04000923 RID: 2339
	public static HashSet<Identifiable.Id> VEGGIE_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000924 RID: 2340
	public static HashSet<Identifiable.Id> FRUIT_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000925 RID: 2341
	public static HashSet<Identifiable.Id> MEAT_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000926 RID: 2342
	public static HashSet<Identifiable.Id> TOFU_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000927 RID: 2343
	public static HashSet<Identifiable.Id> SLIME_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000928 RID: 2344
	public static HashSet<Identifiable.Id> LARGO_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000929 RID: 2345
	public static HashSet<Identifiable.Id> GORDO_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x0400092A RID: 2346
	public static HashSet<Identifiable.Id> PLORT_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x0400092B RID: 2347
	public static HashSet<Identifiable.Id> FOOD_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x0400092C RID: 2348
	public static HashSet<Identifiable.Id> NON_SLIMES_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x0400092D RID: 2349
	public static HashSet<Identifiable.Id> CHICK_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x0400092E RID: 2350
	public static HashSet<Identifiable.Id> LIQUID_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x0400092F RID: 2351
	public static HashSet<Identifiable.Id> CRAFT_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000930 RID: 2352
	public static HashSet<Identifiable.Id> FASHION_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000931 RID: 2353
	public static HashSet<Identifiable.Id> ECHO_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000932 RID: 2354
	public static HashSet<Identifiable.Id> ECHO_NOTE_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000933 RID: 2355
	public static HashSet<Identifiable.Id> ORNAMENT_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000934 RID: 2356
	public static HashSet<Identifiable.Id> TOY_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000935 RID: 2357
	public static HashSet<Identifiable.Id> EATERS_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000936 RID: 2358
	public static HashSet<Identifiable.Id> ALLERGY_FREE_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x04000937 RID: 2359
	public static HashSet<Identifiable.Id> STANDARD_CRATE_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer)
	{
		Identifiable.Id.CRATE_DESERT_01,
		Identifiable.Id.CRATE_MOSS_01,
		Identifiable.Id.CRATE_QUARRY_01,
		Identifiable.Id.CRATE_REEF_01,
		Identifiable.Id.CRATE_RUINS_01,
		Identifiable.Id.CRATE_WILDS_01
	};

	// Token: 0x04000938 RID: 2360
	public static HashSet<Identifiable.Id> ELDER_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer)
	{
		Identifiable.Id.ELDER_HEN,
		Identifiable.Id.ELDER_ROOSTER
	};

	// Token: 0x04000939 RID: 2361
	public static HashSet<Identifiable.Id> TARR_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer)
	{
		Identifiable.Id.TARR_SLIME,
		Identifiable.Id.GLITCH_TARR_SLIME
	};

	// Token: 0x0400093A RID: 2362
	public static HashSet<Identifiable.Id> BOOP_CLASS = new HashSet<Identifiable.Id>(Identifiable.idComparer)
	{
		Identifiable.Id.TABBY_SLIME,
		Identifiable.Id.PINK_TABBY_LARGO,
		Identifiable.Id.BOOM_TABBY_LARGO,
		Identifiable.Id.RAD_TABBY_LARGO,
		Identifiable.Id.ROCK_TABBY_LARGO,
		Identifiable.Id.PHOSPHOR_TABBY_LARGO,
		Identifiable.Id.TABBY_HUNTER_LARGO,
		Identifiable.Id.HONEY_TABBY_LARGO,
		Identifiable.Id.TABBY_CRYSTAL_LARGO,
		Identifiable.Id.QUANTUM_TABBY_LARGO,
		Identifiable.Id.TANGLE_TABBY_LARGO,
		Identifiable.Id.MOSAIC_TABBY_LARGO,
		Identifiable.Id.TABBY_DERVISH_LARGO,
		Identifiable.Id.SABER_TABBY_LARGO
	};

	// Token: 0x0400093B RID: 2363
	public static readonly HashSet<Identifiable.Id> SCENE_OBJECTS = new HashSet<Identifiable.Id>(Identifiable.idComparer)
	{
		Identifiable.Id.PLAYER,
		Identifiable.Id.FIRE_COLUMN,
		Identifiable.Id.SCARECROW,
		Identifiable.Id.PORTABLE_SCARECROW,
		Identifiable.Id.GLITCH_TARR_PORTAL
	};

	// Token: 0x0400093D RID: 2365
	public Identifiable.OnDestroyListener NotifyOnDestroy;

	// Token: 0x0400093E RID: 2366
	private static readonly Dictionary<Identifiable.Id, Gadget.Id> GADGET_NAME_DICT = new Dictionary<Identifiable.Id, Gadget.Id>(Identifiable.idComparer)
	{
		{
			Identifiable.Id.HANDLEBAR_FASHION,
			Gadget.Id.FASHION_POD_HANDLEBAR
		},
		{
			Identifiable.Id.SHADY_FASHION,
			Gadget.Id.FASHION_POD_SHADY
		},
		{
			Identifiable.Id.CLIP_ON_FASHION,
			Gadget.Id.FASHION_POD_CLIP_ON
		},
		{
			Identifiable.Id.GOOGLY_FASHION,
			Gadget.Id.FASHION_POD_GOOGLY
		},
		{
			Identifiable.Id.SERIOUS_FASHION,
			Gadget.Id.FASHION_POD_SERIOUS
		},
		{
			Identifiable.Id.SMART_FASHION,
			Gadget.Id.FASHION_POD_SMART
		},
		{
			Identifiable.Id.CUTE_FASHION,
			Gadget.Id.FASHION_POD_CUTE
		},
		{
			Identifiable.Id.ROYAL_FASHION,
			Gadget.Id.FASHION_POD_ROYAL
		},
		{
			Identifiable.Id.DANDY_FASHION,
			Gadget.Id.FASHION_POD_DANDY
		},
		{
			Identifiable.Id.PIRATEY_FASHION,
			Gadget.Id.FASHION_POD_PIRATEY
		},
		{
			Identifiable.Id.HEROIC_FASHION,
			Gadget.Id.FASHION_POD_HEROIC
		},
		{
			Identifiable.Id.SCIFI_FASHION,
			Gadget.Id.FASHION_POD_SCIFI
		},
		{
			Identifiable.Id.SCUBA_FASHION,
			Gadget.Id.FASHION_POD_SCUBA
		},
		{
			Identifiable.Id.PARTY_GLASSES_FASHION,
			Gadget.Id.FASHION_POD_PARTY_GLASSES
		},
		{
			Identifiable.Id.REMOVER_FASHION,
			Gadget.Id.FASHION_POD_REMOVER
		}
	};

	// Token: 0x02000215 RID: 533
	public enum Id
	{
		// Token: 0x04000940 RID: 2368
		NONE,
		// Token: 0x04000941 RID: 2369
		RAD_SLIME,
		// Token: 0x04000942 RID: 2370
		ROCK_SLIME,
		// Token: 0x04000943 RID: 2371
		PINK_SLIME,
		// Token: 0x04000944 RID: 2372
		RAD_PLORT,
		// Token: 0x04000945 RID: 2373
		ROCK_PLORT,
		// Token: 0x04000946 RID: 2374
		PINK_PLORT,
		// Token: 0x04000947 RID: 2375
		GOLD_PLORT,
		// Token: 0x04000948 RID: 2376
		CUBERRY_FRUIT,
		// Token: 0x04000949 RID: 2377
		MANGO_FRUIT,
		// Token: 0x0400094A RID: 2378
		TARR_SLIME,
		// Token: 0x0400094B RID: 2379
		GOLD_SLIME,
		// Token: 0x0400094C RID: 2380
		PINK_ROCK_LARGO,
		// Token: 0x0400094D RID: 2381
		RAD_ROCK_LARGO,
		// Token: 0x0400094E RID: 2382
		PINK_RAD_LARGO,
		// Token: 0x0400094F RID: 2383
		PLAYER,
		// Token: 0x04000950 RID: 2384
		HEN,
		// Token: 0x04000951 RID: 2385
		ROOSTER,
		// Token: 0x04000952 RID: 2386
		CHICK,
		// Token: 0x04000953 RID: 2387
		CARROT_VEGGIE,
		// Token: 0x04000954 RID: 2388
		OCAOCA_VEGGIE,
		// Token: 0x04000955 RID: 2389
		BOOM_SLIME,
		// Token: 0x04000956 RID: 2390
		PINK_BOOM_LARGO,
		// Token: 0x04000957 RID: 2391
		BOOM_ROCK_LARGO,
		// Token: 0x04000958 RID: 2392
		BOOM_RAD_LARGO,
		// Token: 0x04000959 RID: 2393
		BOOM_PLORT,
		// Token: 0x0400095A RID: 2394
		PEAR_FRUIT,
		// Token: 0x0400095B RID: 2395
		POGO_FRUIT,
		// Token: 0x0400095C RID: 2396
		PARSNIP_VEGGIE,
		// Token: 0x0400095D RID: 2397
		BEET_VEGGIE,
		// Token: 0x0400095E RID: 2398
		SCARECROW,
		// Token: 0x0400095F RID: 2399
		PHOSPHOR_SLIME,
		// Token: 0x04000960 RID: 2400
		PHOSPHOR_ROCK_LARGO,
		// Token: 0x04000961 RID: 2401
		BOOM_PHOSPHOR_LARGO,
		// Token: 0x04000962 RID: 2402
		PHOSPHOR_RAD_LARGO,
		// Token: 0x04000963 RID: 2403
		PINK_PHOSPHOR_LARGO,
		// Token: 0x04000964 RID: 2404
		PHOSPHOR_PLORT,
		// Token: 0x04000965 RID: 2405
		TABBY_SLIME,
		// Token: 0x04000966 RID: 2406
		TABBY_PLORT,
		// Token: 0x04000967 RID: 2407
		PINK_TABBY_LARGO,
		// Token: 0x04000968 RID: 2408
		BOOM_TABBY_LARGO,
		// Token: 0x04000969 RID: 2409
		RAD_TABBY_LARGO,
		// Token: 0x0400096A RID: 2410
		ROCK_TABBY_LARGO,
		// Token: 0x0400096B RID: 2411
		PHOSPHOR_TABBY_LARGO,
		// Token: 0x0400096C RID: 2412
		CRATE_REEF_01,
		// Token: 0x0400096D RID: 2413
		CRATE_QUARRY_01,
		// Token: 0x0400096E RID: 2414
		CRATE_MOSS_01,
		// Token: 0x0400096F RID: 2415
		CRATE_DESERT_01,
		// Token: 0x04000970 RID: 2416
		WATER_LIQUID,
		// Token: 0x04000971 RID: 2417
		ELDER_HEN,
		// Token: 0x04000972 RID: 2418
		ELDER_ROOSTER,
		// Token: 0x04000973 RID: 2419
		HUNTER_SLIME,
		// Token: 0x04000974 RID: 2420
		HUNTER_PLORT,
		// Token: 0x04000975 RID: 2421
		PINK_HUNTER_LARGO,
		// Token: 0x04000976 RID: 2422
		BOOM_HUNTER_LARGO,
		// Token: 0x04000977 RID: 2423
		RAD_HUNTER_LARGO,
		// Token: 0x04000978 RID: 2424
		ROCK_HUNTER_LARGO,
		// Token: 0x04000979 RID: 2425
		PHOSPHOR_HUNTER_LARGO,
		// Token: 0x0400097A RID: 2426
		TABBY_HUNTER_LARGO,
		// Token: 0x0400097B RID: 2427
		HONEY_SLIME,
		// Token: 0x0400097C RID: 2428
		HONEY_PLORT,
		// Token: 0x0400097D RID: 2429
		PINK_HONEY_LARGO,
		// Token: 0x0400097E RID: 2430
		HONEY_HUNTER_LARGO,
		// Token: 0x0400097F RID: 2431
		HONEY_BOOM_LARGO,
		// Token: 0x04000980 RID: 2432
		HONEY_RAD_LARGO,
		// Token: 0x04000981 RID: 2433
		HONEY_ROCK_LARGO,
		// Token: 0x04000982 RID: 2434
		HONEY_PHOSPHOR_LARGO,
		// Token: 0x04000983 RID: 2435
		HONEY_TABBY_LARGO,
		// Token: 0x04000984 RID: 2436
		STONY_HEN,
		// Token: 0x04000985 RID: 2437
		BRIAR_HEN,
		// Token: 0x04000986 RID: 2438
		STONY_CHICK,
		// Token: 0x04000987 RID: 2439
		BRIAR_CHICK,
		// Token: 0x04000988 RID: 2440
		PUDDLE_SLIME,
		// Token: 0x04000989 RID: 2441
		PUDDLE_PLORT,
		// Token: 0x0400098A RID: 2442
		DAILY_EXCHANGE_CRATE,
		// Token: 0x0400098B RID: 2443
		SPECIAL_EXCHANGE_CRATE,
		// Token: 0x0400098C RID: 2444
		KEY,
		// Token: 0x0400098D RID: 2445
		LUCKY_SLIME,
		// Token: 0x0400098E RID: 2446
		CRYSTAL_PLORT,
		// Token: 0x0400098F RID: 2447
		CRYSTAL_SLIME,
		// Token: 0x04000990 RID: 2448
		PINK_CRYSTAL_LARGO,
		// Token: 0x04000991 RID: 2449
		ROCK_CRYSTAL_LARGO,
		// Token: 0x04000992 RID: 2450
		TABBY_CRYSTAL_LARGO,
		// Token: 0x04000993 RID: 2451
		PHOSPHOR_CRYSTAL_LARGO,
		// Token: 0x04000994 RID: 2452
		BOOM_CRYSTAL_LARGO,
		// Token: 0x04000995 RID: 2453
		RAD_CRYSTAL_LARGO,
		// Token: 0x04000996 RID: 2454
		HONEY_CRYSTAL_LARGO,
		// Token: 0x04000997 RID: 2455
		HUNTER_CRYSTAL_LARGO,
		// Token: 0x04000998 RID: 2456
		ONION_VEGGIE,
		// Token: 0x04000999 RID: 2457
		QUANTUM_SLIME,
		// Token: 0x0400099A RID: 2458
		PINK_QUANTUM_LARGO,
		// Token: 0x0400099B RID: 2459
		QUANTUM_BOOM_LARGO,
		// Token: 0x0400099C RID: 2460
		QUANTUM_CRYSTAL_LARGO,
		// Token: 0x0400099D RID: 2461
		QUANTUM_HONEY_LARGO,
		// Token: 0x0400099E RID: 2462
		QUANTUM_HUNTER_LARGO,
		// Token: 0x0400099F RID: 2463
		QUANTUM_PHOSPHOR_LARGO,
		// Token: 0x040009A0 RID: 2464
		QUANTUM_RAD_LARGO,
		// Token: 0x040009A1 RID: 2465
		QUANTUM_ROCK_LARGO,
		// Token: 0x040009A2 RID: 2466
		QUANTUM_TABBY_LARGO,
		// Token: 0x040009A3 RID: 2467
		QUANTUM_PLORT,
		// Token: 0x040009A4 RID: 2468
		LEMON_FRUIT,
		// Token: 0x040009A5 RID: 2469
		LEMON_PHASE,
		// Token: 0x040009A6 RID: 2470
		DERVISH_SLIME,
		// Token: 0x040009A7 RID: 2471
		DERVISH_PLORT,
		// Token: 0x040009A8 RID: 2472
		MOSAIC_SLIME,
		// Token: 0x040009A9 RID: 2473
		MOSAIC_PLORT,
		// Token: 0x040009AA RID: 2474
		TANGLE_SLIME,
		// Token: 0x040009AB RID: 2475
		TANGLE_PLORT,
		// Token: 0x040009AC RID: 2476
		FIRE_SLIME,
		// Token: 0x040009AD RID: 2477
		FIRE_PLORT,
		// Token: 0x040009AE RID: 2478
		PAINTED_HEN,
		// Token: 0x040009AF RID: 2479
		PAINTED_CHICK,
		// Token: 0x040009B0 RID: 2480
		POLLEN_CLOUD,
		// Token: 0x040009B1 RID: 2481
		MAGIC_WATER_LIQUID,
		// Token: 0x040009B2 RID: 2482
		FIRE_COLUMN,
		// Token: 0x040009B3 RID: 2483
		PINK_TANGLE_LARGO,
		// Token: 0x040009B4 RID: 2484
		QUANTUM_TANGLE_LARGO,
		// Token: 0x040009B5 RID: 2485
		HONEY_TANGLE_LARGO,
		// Token: 0x040009B6 RID: 2486
		PHOSPHOR_TANGLE_LARGO,
		// Token: 0x040009B7 RID: 2487
		TANGLE_BOOM_LARGO,
		// Token: 0x040009B8 RID: 2488
		TANGLE_RAD_LARGO,
		// Token: 0x040009B9 RID: 2489
		TANGLE_ROCK_LARGO,
		// Token: 0x040009BA RID: 2490
		TANGLE_TABBY_LARGO,
		// Token: 0x040009BB RID: 2491
		TANGLE_HUNTER_LARGO,
		// Token: 0x040009BC RID: 2492
		TANGLE_CRYSTAL_LARGO,
		// Token: 0x040009BD RID: 2493
		PINK_MOSAIC_LARGO,
		// Token: 0x040009BE RID: 2494
		QUANTUM_MOSAIC_LARGO,
		// Token: 0x040009BF RID: 2495
		HONEY_MOSAIC_LARGO,
		// Token: 0x040009C0 RID: 2496
		PHOSPHOR_MOSAIC_LARGO,
		// Token: 0x040009C1 RID: 2497
		MOSAIC_TANGLE_LARGO,
		// Token: 0x040009C2 RID: 2498
		MOSAIC_BOOM_LARGO,
		// Token: 0x040009C3 RID: 2499
		MOSAIC_RAD_LARGO,
		// Token: 0x040009C4 RID: 2500
		MOSAIC_ROCK_LARGO,
		// Token: 0x040009C5 RID: 2501
		MOSAIC_TABBY_LARGO,
		// Token: 0x040009C6 RID: 2502
		MOSAIC_HUNTER_LARGO,
		// Token: 0x040009C7 RID: 2503
		MOSAIC_CRYSTAL_LARGO,
		// Token: 0x040009C8 RID: 2504
		PINK_DERVISH_LARGO,
		// Token: 0x040009C9 RID: 2505
		QUANTUM_DERVISH_LARGO,
		// Token: 0x040009CA RID: 2506
		HONEY_DERVISH_LARGO,
		// Token: 0x040009CB RID: 2507
		PHOSPHOR_DERVISH_LARGO,
		// Token: 0x040009CC RID: 2508
		TANGLE_DERVISH_LARGO,
		// Token: 0x040009CD RID: 2509
		MOSAIC_DERVISH_LARGO,
		// Token: 0x040009CE RID: 2510
		BOOM_DERVISH_LARGO,
		// Token: 0x040009CF RID: 2511
		RAD_DERVISH_LARGO,
		// Token: 0x040009D0 RID: 2512
		ROCK_DERVISH_LARGO,
		// Token: 0x040009D1 RID: 2513
		TABBY_DERVISH_LARGO,
		// Token: 0x040009D2 RID: 2514
		HUNTER_DERVISH_LARGO,
		// Token: 0x040009D3 RID: 2515
		CRYSTAL_DERVISH_LARGO,
		// Token: 0x040009D4 RID: 2516
		GINGER_VEGGIE,
		// Token: 0x040009D5 RID: 2517
		SPICY_TOFU,
		// Token: 0x040009D6 RID: 2518
		SABER_SLIME,
		// Token: 0x040009D7 RID: 2519
		SABER_PINK_LARGO,
		// Token: 0x040009D8 RID: 2520
		SABER_QUANTUM_LARGO,
		// Token: 0x040009D9 RID: 2521
		SABER_HONEY_LARGO,
		// Token: 0x040009DA RID: 2522
		SABER_PHOSPHOR_LARGO,
		// Token: 0x040009DB RID: 2523
		SABER_TANGLE_LARGO,
		// Token: 0x040009DC RID: 2524
		SABER_MOSAIC_LARGO,
		// Token: 0x040009DD RID: 2525
		SABER_BOOM_LARGO,
		// Token: 0x040009DE RID: 2526
		SABER_RAD_LARGO,
		// Token: 0x040009DF RID: 2527
		SABER_ROCK_LARGO,
		// Token: 0x040009E0 RID: 2528
		SABER_TABBY_LARGO,
		// Token: 0x040009E1 RID: 2529
		SABER_HUNTER_LARGO,
		// Token: 0x040009E2 RID: 2530
		SABER_CRYSTAL_LARGO,
		// Token: 0x040009E3 RID: 2531
		SABER_DERVISH_LARGO,
		// Token: 0x040009E4 RID: 2532
		SABER_PLORT,
		// Token: 0x040009E5 RID: 2533
		KOOKADOBA_FRUIT,
		// Token: 0x040009E6 RID: 2534
		QUICKSILVER_SLIME,
		// Token: 0x040009E7 RID: 2535
		QUICKSILVER_PLORT,
		// Token: 0x040009E8 RID: 2536
		KOOKADOBA_BALL,
		// Token: 0x040009E9 RID: 2537
		CRATE_RUINS_01,
		// Token: 0x040009EA RID: 2538
		CRATE_WILDS_01,
		// Token: 0x040009EB RID: 2539
		VALLEY_AMMO_1,
		// Token: 0x040009EC RID: 2540
		VALLEY_AMMO_2,
		// Token: 0x040009ED RID: 2541
		VALLEY_AMMO_3,
		// Token: 0x040009EE RID: 2542
		VALLEY_AMMO_4,
		// Token: 0x040009EF RID: 2543
		PORTABLE_SCARECROW,
		// Token: 0x040009F0 RID: 2544
		RAD_GORDO = 10000,
		// Token: 0x040009F1 RID: 2545
		ROCK_GORDO,
		// Token: 0x040009F2 RID: 2546
		PINK_GORDO,
		// Token: 0x040009F3 RID: 2547
		BOOM_GORDO,
		// Token: 0x040009F4 RID: 2548
		PHOSPHOR_GORDO,
		// Token: 0x040009F5 RID: 2549
		TABBY_GORDO,
		// Token: 0x040009F6 RID: 2550
		HUNTER_GORDO,
		// Token: 0x040009F7 RID: 2551
		HONEY_GORDO,
		// Token: 0x040009F8 RID: 2552
		PUDDLE_GORDO,
		// Token: 0x040009F9 RID: 2553
		CRYSTAL_GORDO,
		// Token: 0x040009FA RID: 2554
		QUANTUM_GORDO,
		// Token: 0x040009FB RID: 2555
		DERVISH_GORDO,
		// Token: 0x040009FC RID: 2556
		MOSAIC_GORDO,
		// Token: 0x040009FD RID: 2557
		TANGLE_GORDO,
		// Token: 0x040009FE RID: 2558
		GOLD_GORDO,
		// Token: 0x040009FF RID: 2559
		PRIMORDY_OIL_CRAFT = 11000,
		// Token: 0x04000A00 RID: 2560
		DEEP_BRINE_CRAFT,
		// Token: 0x04000A01 RID: 2561
		SPIRAL_STEAM_CRAFT,
		// Token: 0x04000A02 RID: 2562
		LAVA_DUST_CRAFT,
		// Token: 0x04000A03 RID: 2563
		BUZZ_WAX_CRAFT,
		// Token: 0x04000A04 RID: 2564
		WILD_HONEY_CRAFT,
		// Token: 0x04000A05 RID: 2565
		HEXACOMB_CRAFT,
		// Token: 0x04000A06 RID: 2566
		ROYAL_JELLY_CRAFT,
		// Token: 0x04000A07 RID: 2567
		JELLYSTONE_CRAFT,
		// Token: 0x04000A08 RID: 2568
		INDIGONIUM_CRAFT,
		// Token: 0x04000A09 RID: 2569
		SLIME_FOSSIL_CRAFT,
		// Token: 0x04000A0A RID: 2570
		STRANGE_DIAMOND_CRAFT,
		// Token: 0x04000A0B RID: 2571
		RED_ECHO,
		// Token: 0x04000A0C RID: 2572
		GREEN_ECHO,
		// Token: 0x04000A0D RID: 2573
		BLUE_ECHO,
		// Token: 0x04000A0E RID: 2574
		GOLD_ECHO,
		// Token: 0x04000A0F RID: 2575
		SILKY_SAND_CRAFT,
		// Token: 0x04000A10 RID: 2576
		PEPPER_JAM_CRAFT,
		// Token: 0x04000A11 RID: 2577
		GLASS_SHARD_CRAFT,
		// Token: 0x04000A12 RID: 2578
		MANIFOLD_CUBE_CRAFT,
		// Token: 0x04000A13 RID: 2579
		HANDLEBAR_FASHION = 12000,
		// Token: 0x04000A14 RID: 2580
		SHADY_FASHION,
		// Token: 0x04000A15 RID: 2581
		CLIP_ON_FASHION,
		// Token: 0x04000A16 RID: 2582
		GOOGLY_FASHION,
		// Token: 0x04000A17 RID: 2583
		SERIOUS_FASHION,
		// Token: 0x04000A18 RID: 2584
		SMART_FASHION,
		// Token: 0x04000A19 RID: 2585
		CUTE_FASHION,
		// Token: 0x04000A1A RID: 2586
		ROYAL_FASHION,
		// Token: 0x04000A1B RID: 2587
		DANDY_FASHION,
		// Token: 0x04000A1C RID: 2588
		PARTY_FASHION,
		// Token: 0x04000A1D RID: 2589
		PIRATEY_FASHION,
		// Token: 0x04000A1E RID: 2590
		HEROIC_FASHION,
		// Token: 0x04000A1F RID: 2591
		SCIFI_FASHION,
		// Token: 0x04000A20 RID: 2592
		SCUBA_FASHION,
		// Token: 0x04000A21 RID: 2593
		PARTY_GLASSES_FASHION,
		// Token: 0x04000A22 RID: 2594
		REMOVER_FASHION = 12099,
		// Token: 0x04000A23 RID: 2595
		BEACH_BALL_TOY = 13000,
		// Token: 0x04000A24 RID: 2596
		BIG_ROCK_TOY,
		// Token: 0x04000A25 RID: 2597
		YARN_BALL_TOY,
		// Token: 0x04000A26 RID: 2598
		NIGHT_LIGHT_TOY,
		// Token: 0x04000A27 RID: 2599
		POWER_CELL_TOY,
		// Token: 0x04000A28 RID: 2600
		BOMB_BALL_TOY,
		// Token: 0x04000A29 RID: 2601
		BUZZY_BEE_TOY,
		// Token: 0x04000A2A RID: 2602
		RUBBER_DUCKY_TOY,
		// Token: 0x04000A2B RID: 2603
		CRYSTAL_BALL_TOY,
		// Token: 0x04000A2C RID: 2604
		STUFFED_CHICKEN_TOY,
		// Token: 0x04000A2D RID: 2605
		PUZZLE_CUBE_TOY,
		// Token: 0x04000A2E RID: 2606
		DISCO_BALL_TOY,
		// Token: 0x04000A2F RID: 2607
		GYRO_TOP_TOY,
		// Token: 0x04000A30 RID: 2608
		SOL_MATE_TOY,
		// Token: 0x04000A31 RID: 2609
		CHARCOAL_BRICK_TOY,
		// Token: 0x04000A32 RID: 2610
		STEGO_BUDDY_TOY,
		// Token: 0x04000A33 RID: 2611
		TREASURE_CHEST_TOY,
		// Token: 0x04000A34 RID: 2612
		BOP_GOBLIN_TOY,
		// Token: 0x04000A35 RID: 2613
		ROBOT_TOY,
		// Token: 0x04000A36 RID: 2614
		OCTO_BUDDY_TOY,
		// Token: 0x04000A37 RID: 2615
		PINK_ORNAMENT = 14000,
		// Token: 0x04000A38 RID: 2616
		ROCK_ORNAMENT,
		// Token: 0x04000A39 RID: 2617
		TABBY_ORNAMENT,
		// Token: 0x04000A3A RID: 2618
		PHOSPHOR_ORNAMENT,
		// Token: 0x04000A3B RID: 2619
		RAD_ORNAMENT,
		// Token: 0x04000A3C RID: 2620
		BOOM_ORNAMENT,
		// Token: 0x04000A3D RID: 2621
		HONEY_ORNAMENT,
		// Token: 0x04000A3E RID: 2622
		HUNTER_ORNAMENT,
		// Token: 0x04000A3F RID: 2623
		QUANTUM_ORNAMENT,
		// Token: 0x04000A40 RID: 2624
		PUDDLE_ORNAMENT,
		// Token: 0x04000A41 RID: 2625
		TANGLE_ORNAMENT,
		// Token: 0x04000A42 RID: 2626
		DERVISH_ORNAMENT,
		// Token: 0x04000A43 RID: 2627
		MOSAIC_ORNAMENT,
		// Token: 0x04000A44 RID: 2628
		LUCKY_ORNAMENT,
		// Token: 0x04000A45 RID: 2629
		GOLD_ORNAMENT,
		// Token: 0x04000A46 RID: 2630
		TARR_ORNAMENT,
		// Token: 0x04000A47 RID: 2631
		STACHE_ORNAMENT,
		// Token: 0x04000A48 RID: 2632
		CRYSTAL_ORNAMENT,
		// Token: 0x04000A49 RID: 2633
		QUICKSILVER_ORNAMENT,
		// Token: 0x04000A4A RID: 2634
		FIRE_ORNAMENT,
		// Token: 0x04000A4B RID: 2635
		HENHEN_ORNAMENT,
		// Token: 0x04000A4C RID: 2636
		SEVENZ_ORNAMENT,
		// Token: 0x04000A4D RID: 2637
		CHEEVO_ORNAMENT,
		// Token: 0x04000A4E RID: 2638
		CLOUD_ORNAMENT,
		// Token: 0x04000A4F RID: 2639
		CLOVER_ORNAMENT,
		// Token: 0x04000A50 RID: 2640
		HEART_ORNAMENT,
		// Token: 0x04000A51 RID: 2641
		BRIAR_HEN_ORNAMENT,
		// Token: 0x04000A52 RID: 2642
		ELDER_HEN_ORNAMENT,
		// Token: 0x04000A53 RID: 2643
		PAINTED_HEN_ORNAMENT,
		// Token: 0x04000A54 RID: 2644
		STONY_HEN_ORNAMENT,
		// Token: 0x04000A55 RID: 2645
		JACK_ORNAMENT,
		// Token: 0x04000A56 RID: 2646
		NEWBUCK_ORNAMENT,
		// Token: 0x04000A57 RID: 2647
		PINK_PARTY_ORNAMENT,
		// Token: 0x04000A58 RID: 2648
		RAINBOW_ORNAMENT,
		// Token: 0x04000A59 RID: 2649
		SNOWFLAKE_ORNAMENT,
		// Token: 0x04000A5A RID: 2650
		STAR_ORNAMENT,
		// Token: 0x04000A5B RID: 2651
		STRIPES_GREEN_ORNAMENT,
		// Token: 0x04000A5C RID: 2652
		STRIPES_PURPLE_ORNAMENT,
		// Token: 0x04000A5D RID: 2653
		GLITCH_ORNAMENT,
		// Token: 0x04000A5E RID: 2654
		SABER_ORNAMENT,
		// Token: 0x04000A5F RID: 2655
		IMPOSTER_ORNAMENT,
		// Token: 0x04000A60 RID: 2656
		DRONE_ORNAMENT,
		// Token: 0x04000A61 RID: 2657
		DRONE_SLEEPY_ORNAMENT,
		// Token: 0x04000A62 RID: 2658
		STRIPES_RED_ORNAMENT,
		// Token: 0x04000A63 RID: 2659
		STRIPES_PINK_ORNAMENT,
		// Token: 0x04000A64 RID: 2660
		STRIPES_BLUE_ORNAMENT,
		// Token: 0x04000A65 RID: 2661
		STRIPES_TEAL_ORNAMENT,
		// Token: 0x04000A66 RID: 2662
		SUNNY_ORNAMENT,
		// Token: 0x04000A67 RID: 2663
		WILDFLOWER_ORNAMENT,
		// Token: 0x04000A68 RID: 2664
		FIREFLOWER_ORNAMENT,
		// Token: 0x04000A69 RID: 2665
		TARR_LANTERN_ORNAMENT,
		// Token: 0x04000A6A RID: 2666
		TWINKLE_ORNAMENT,
		// Token: 0x04000A6B RID: 2667
		SLIME_MOON_ORNAMENT,
		// Token: 0x04000A6C RID: 2668
		DUCKY_ORNAMENT,
		// Token: 0x04000A6D RID: 2669
		STEGO_ORNAMENT,
		// Token: 0x04000A6E RID: 2670
		BUZZY_ORNAMENT,
		// Token: 0x04000A6F RID: 2671
		IMPOSTER_TABBY_ORNAMENT,
		// Token: 0x04000A70 RID: 2672
		TREEFOX_ORNAMENT,
		// Token: 0x04000A71 RID: 2673
		PARTY_GORDO = 15000,
		// Token: 0x04000A72 RID: 2674
		CRATE_PARTY_01 = 15100,
		// Token: 0x04000A73 RID: 2675
		GLITCH_SLIME = 16000,
		// Token: 0x04000A74 RID: 2676
		GLITCH_DEBUG_SPRAY_LIQUID,
		// Token: 0x04000A75 RID: 2677
		GLITCH_BUG_REPORT,
		// Token: 0x04000A76 RID: 2678
		GLITCH_TARR_SLIME,
		// Token: 0x04000A77 RID: 2679
		GLITCH_TARR_PORTAL,
		// Token: 0x04000A78 RID: 2680
		ECHO_NOTE_01 = 17000,
		// Token: 0x04000A79 RID: 2681
		ECHO_NOTE_02,
		// Token: 0x04000A7A RID: 2682
		ECHO_NOTE_03,
		// Token: 0x04000A7B RID: 2683
		ECHO_NOTE_04,
		// Token: 0x04000A7C RID: 2684
		ECHO_NOTE_05,
		// Token: 0x04000A7D RID: 2685
		ECHO_NOTE_06,
		// Token: 0x04000A7E RID: 2686
		ECHO_NOTE_07,
		// Token: 0x04000A7F RID: 2687
		ECHO_NOTE_08,
		// Token: 0x04000A80 RID: 2688
		ECHO_NOTE_09,
		// Token: 0x04000A81 RID: 2689
		ECHO_NOTE_10,
		// Token: 0x04000A82 RID: 2690
		ECHO_NOTE_11,
		// Token: 0x04000A83 RID: 2691
		ECHO_NOTE_12,
		// Token: 0x04000A84 RID: 2692
		ECHO_NOTE_13
	}

	// Token: 0x02000216 RID: 534
	public class IdComparer : IEqualityComparer<Identifiable.Id>
	{
		// Token: 0x06000B55 RID: 2901 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(Identifiable.Id id1, Identifiable.Id id2)
		{
			return id1 == id2;
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(Identifiable.Id id)
		{
			return (int)id;
		}
	}

	// Token: 0x02000217 RID: 535
	// (Invoke) Token: 0x06000B59 RID: 2905
	public delegate void OnDestroyListener(Identifiable obj);
}
