using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DLCPackage;
using MonomiPark.SlimeRancher.Persist;
using UnityEngine;

// Token: 0x020000ED RID: 237
public class DLCDirector
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x06000562 RID: 1378 RVA: 0x000206B4 File Offset: 0x0001E8B4
	// (remove) Token: 0x06000563 RID: 1379 RVA: 0x000206EC File Offset: 0x0001E8EC
	public event DLCDirector.OnPackageInstalledDelegate onPackageInstalled = delegate(Id <p0>)
	{
	};

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000564 RID: 1380 RVA: 0x00020721 File Offset: 0x0001E921
	public IEnumerable<Id> Installed
	{
		get
		{
			return from package in this.GetSupportedPackages()
			where this.GetPackageState(package) >= State.INSTALLED
			select package;
		}
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0002073A File Offset: 0x0001E93A
	public bool SetProvider(DLCProvider provider)
	{
		if (this.provider != null)
		{
			Log.Error("Attempting to replace existing DLC provider.", Array.Empty<object>());
			return false;
		}
		this.provider = provider;
		return true;
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0002075D File Offset: 0x0001E95D
	public IEnumerable<Id> GetSupportedPackages()
	{
		if (this.provider != null)
		{
			foreach (Id id in this.provider.GetSupported())
			{
				yield return id;
			}
			IEnumerator<Id> enumerator = null;
		}
		yield break;
		yield break;
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0002076D File Offset: 0x0001E96D
	public bool HasReached(Id id, State state)
	{
		return this.provider != null && this.provider.GetSupported().Contains(id) && this.provider.GetState(id) >= state;
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0002079E File Offset: 0x0001E99E
	public State GetPackageState(Id id)
	{
		if (this.provider == null)
		{
			return State.UNDEFINED;
		}
		return this.provider.GetState(id);
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x000207B6 File Offset: 0x0001E9B6
	public void ShowPackageInStore(Id id)
	{
		if (this.provider != null)
		{
			this.provider.ShowInStore(id);
		}
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x000207CC File Offset: 0x0001E9CC
	public bool IsPackageInstalledAndEnabled(Id id)
	{
		return SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().enableDLC && this.HasReached(id, State.INSTALLED);
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x000207EE File Offset: 0x0001E9EE
	public void InitForLevel()
	{
		if (this.provider != null && !Levels.isSpecial())
		{
			this.RegisterPackages();
		}
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x00020805 File Offset: 0x0001EA05
	public IEnumerator RefreshPackagesAsync()
	{
		if (this.provider != null)
		{
			yield return this.provider.Refresh();
		}
		yield break;
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x00020814 File Offset: 0x0001EA14
	public IEnumerator RegisterPackagesAsync()
	{
		yield return this.RefreshPackagesAsync();
		this.RegisterPackages();
		yield break;
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x00020824 File Offset: 0x0001EA24
	public void RegisterPackages()
	{
		foreach (Id id in this.Installed)
		{
			DLCContentMetadata[] content = this.GetPackageLoader(id).content;
			for (int i = 0; i < content.Length; i++)
			{
				content[i].Register();
			}
			this.onPackageInstalled(id);
		}
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0002089C File Offset: 0x0001EA9C
	public IEnumerable<DLCPackageMetadata> LoadPackageMetadatas()
	{
		return from id in this.GetSupportedPackages()
		select this.GetPackageLoader(id).package;
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x000208B8 File Offset: 0x0001EAB8
	private DLCDirector.PackageLoader GetPackageLoader(Id id)
	{
		if (this.packageLoadersDict.ContainsKey(id))
		{
			return this.packageLoadersDict[id];
		}
		return this.packageLoadersDict[id] = new DLCDirector.PackageLoader(id);
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x000208F8 File Offset: 0x0001EAF8
	public void Purge(GameV12 game)
	{
		Id[] array = (from Id p in Enum.GetValues(typeof(Id))
		where this.GetPackageState(p) < State.INSTALLED && this.PurgePackage(game, p)
		select p).ToArray<Id>();
		if (array.Any<Id>())
		{
			throw new DLCPurgedException(array);
		}
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00020954 File Offset: 0x0001EB54
	private bool PurgePackage(GameV12 game, Id package)
	{
		int num = 0;
		switch (package)
		{
		case Id.PLAYSET_PIRATE:
			num += this.PurgeChromaPack(game, RanchDirector.Palette.PALETTE27);
			num += this.PurgeFashion(game, Gadget.Id.FASHION_POD_PIRATEY, Identifiable.Id.PIRATEY_FASHION);
			num += this.PurgeToy(game, Identifiable.Id.TREASURE_CHEST_TOY);
			break;
		case Id.PLAYSET_HEROIC:
			num += this.PurgeChromaPack(game, RanchDirector.Palette.PALETTE28);
			num += this.PurgeFashion(game, Gadget.Id.FASHION_POD_HEROIC, Identifiable.Id.HEROIC_FASHION);
			num += this.PurgeToy(game, Identifiable.Id.BOP_GOBLIN_TOY);
			break;
		case Id.PLAYSET_SCIFI:
			num += this.PurgeChromaPack(game, RanchDirector.Palette.PALETTE29);
			num += this.PurgeFashion(game, Gadget.Id.FASHION_POD_SCIFI, Identifiable.Id.SCIFI_FASHION);
			num += this.PurgeToy(game, Identifiable.Id.ROBOT_TOY);
			break;
		default:
			if (package == Id.SECRET_STYLE)
			{
				foreach (KeyValuePair<Identifiable.Id, List<SlimeAppearance.AppearanceSaveSet>> keyValuePair in game.appearances.unlocks)
				{
					num += keyValuePair.Value.RemoveAll((SlimeAppearance.AppearanceSaveSet it) => it == SlimeAppearance.AppearanceSaveSet.SECRET_STYLE);
					game.appearances.selections[keyValuePair.Key] = SlimeAppearance.AppearanceSaveSet.CLASSIC;
				}
				using (Dictionary<string, TreasurePodV01>.Enumerator enumerator2 = game.world.treasurePods.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, TreasurePodV01> keyValuePair2 = enumerator2.Current;
						if (DLCDirector.SECRET_STYLE_TREASURE_PODS.Contains(keyValuePair2.Key) && keyValuePair2.Value.state != TreasurePod.State.LOCKED)
						{
							keyValuePair2.Value.state = TreasurePod.State.LOCKED;
							num++;
						}
					}
					break;
				}
			}
			throw new InvalidOperationException();
		}
		return num > 0;
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x00020B28 File Offset: 0x0001ED28
	private int PurgeChromaPack(GameV12 game, RanchDirector.Palette palette)
	{
		int num = 0;
		foreach (RanchDirector.PaletteType key in game.ranch.palettes.Keys.ToList<RanchDirector.PaletteType>())
		{
			if (game.ranch.palettes[key] == palette)
			{
				game.ranch.palettes[key] = RanchDirector.Palette.DEFAULT;
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x00020BB0 File Offset: 0x0001EDB0
	private int PurgeFashion(GameV12 game, Gadget.Id gadget, Identifiable.Id fashion)
	{
		int num = 0;
		Predicate<Identifiable.Id> <>9__4;
		foreach (GordoV01 gordoV in game.world.gordos.Values)
		{
			int num2 = num;
			List<Identifiable.Id> fashions = gordoV.fashions;
			Predicate<Identifiable.Id> match;
			if ((match = <>9__4) == null)
			{
				match = (<>9__4 = ((Identifiable.Id it) => it == fashion));
			}
			num = num2 + fashions.RemoveAll(match);
		}
		Predicate<Identifiable.Id> <>9__5;
		foreach (ActorDataV09 actorDataV in game.actors)
		{
			int num3 = num;
			List<Identifiable.Id> fashions2 = actorDataV.fashions;
			Predicate<Identifiable.Id> match2;
			if ((match2 = <>9__5) == null)
			{
				match2 = (<>9__5 = ((Identifiable.Id it) => it == fashion));
			}
			num = num3 + fashions2.RemoveAll(match2);
		}
		foreach (string key in game.world.placedGadgets.Keys.ToList<string>())
		{
			if (game.world.placedGadgets[key].gadgetId == gadget)
			{
				num += Convert.ToInt32(game.world.placedGadgets.Remove(key));
			}
		}
		Predicate<Identifiable.Id> <>9__6;
		Predicate<Identifiable.Id> <>9__7;
		foreach (PlacedGadgetV08 placedGadgetV in game.world.placedGadgets.Values)
		{
			int num4 = num;
			List<Identifiable.Id> fashions3 = placedGadgetV.fashions;
			Predicate<Identifiable.Id> match3;
			if ((match3 = <>9__6) == null)
			{
				match3 = (<>9__6 = ((Identifiable.Id it) => it == fashion));
			}
			num = num4 + fashions3.RemoveAll(match3);
			if (placedGadgetV.drone != null)
			{
				int num5 = num;
				List<Identifiable.Id> fashions4 = placedGadgetV.drone.drone.fashions;
				Predicate<Identifiable.Id> match4;
				if ((match4 = <>9__7) == null)
				{
					match4 = (<>9__7 = ((Identifiable.Id it) => it == fashion));
				}
				num = num5 + fashions4.RemoveAll(match4);
			}
		}
		num += game.actors.RemoveAll((ActorDataV09 it) => it.typeId == (int)fashion);
		num += game.player.ammo[PlayerState.AmmoMode.DEFAULT].RemoveAll((AmmoDataV02 d) => d.id == fashion);
		num += game.player.blueprints.RemoveAll((Gadget.Id it) => it == gadget);
		num += game.player.availBlueprints.RemoveAll((Gadget.Id it) => it == gadget);
		num += Convert.ToInt32(game.player.blueprintLocks.Remove(gadget));
		num += Convert.ToInt32(game.player.gadgets.Remove(gadget));
		return num;
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x00020EB4 File Offset: 0x0001F0B4
	private int PurgeToy(GameV12 game, Identifiable.Id toy)
	{
		return game.actors.RemoveAll((ActorDataV09 it) => it.typeId == (int)toy);
	}

	// Token: 0x04000565 RID: 1381
	public static HashSet<string> SECRET_STYLE_TREASURE_PODS = new HashSet<string>
	{
		"pod1067506426",
		"pod0573382639",
		"pod0528457170",
		"pod0209361044",
		"pod2089428629",
		"pod0797820130",
		"pod0793461898",
		"pod1736587205",
		"pod1843001748",
		"pod1327729579",
		"pod1761631840",
		"pod0942230423",
		"pod1507800227",
		"pod0403498756",
		"pod0732526653",
		"pod1897070320",
		"pod1003003618",
		"pod0084486208",
		"pod0463402699",
		"pod1284546475"
	};

	// Token: 0x04000567 RID: 1383
	private DLCProvider provider;

	// Token: 0x04000568 RID: 1384
	private Dictionary<Id, DLCDirector.PackageLoader> packageLoadersDict = new Dictionary<Id, DLCDirector.PackageLoader>(IdComparer.Instance);

	// Token: 0x020000EE RID: 238
	// (Invoke) Token: 0x0600057B RID: 1403
	public delegate void OnPackageInstalledDelegate(Id package);

	// Token: 0x020000EF RID: 239
	private class PackageLoader
	{
		// Token: 0x0600057E RID: 1406 RVA: 0x00021048 File Offset: 0x0001F248
		public PackageLoader(Id id)
		{
			string path = Path.Combine("DLC", id.ToString().ToLowerInvariant());
			this.package = Resources.Load<DLCPackageMetadata>(Path.Combine(path, "package"));
			this.content = Resources.LoadAll<DLCContentMetadata>(Path.Combine(path, "package_metadata"));
			if (this.package == null)
			{
				throw new Exception(string.Format("Failed to load DLC package. [id={0}]", id));
			}
			if (this.content == null)
			{
				throw new Exception(string.Format("Failed to load DLC package contents. [id={0}]", id));
			}
		}

		// Token: 0x04000569 RID: 1385
		public readonly DLCPackageMetadata package;

		// Token: 0x0400056A RID: 1386
		public readonly DLCContentMetadata[] content;
	}
}
