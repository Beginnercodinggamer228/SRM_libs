using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000708 RID: 1800
public class Gadget : MonoBehaviour
{
	// Token: 0x0600258F RID: 9615 RVA: 0x00090504 File Offset: 0x0008E704
	public static void RegisterFashion(Gadget.Id id)
	{
		Gadget.ALL_FASHIONS.RemoveAll((Gadget.Id it) => it == id);
		Gadget.ALL_FASHIONS.Add(id);
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x00090545 File Offset: 0x0008E745
	public virtual void Awake()
	{
		this.rotationTransform = base.transform;
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x00090554 File Offset: 0x0008E754
	static Gadget()
	{
		foreach (object obj in Enum.GetValues(typeof(Gadget.Id)))
		{
			Gadget.Id id = (Gadget.Id)obj;
			if (id >= Gadget.Id.EXTRACTOR_DRILL_NOVICE && id < Gadget.Id.TELEPORTER_PINK)
			{
				Gadget.EXTRACTOR_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.TELEPORTER_PINK && id < Gadget.Id.WARP_DEPOT_PINK)
			{
				Gadget.TELEPORTER_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.WARP_DEPOT_PINK && id < Gadget.Id.MARKET_LINK)
			{
				Gadget.WARP_DEPOT_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.MARKET_LINK && id < Gadget.Id.ECHO_NET)
			{
				Gadget.MISC_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.ECHO_NET && id < Gadget.Id.LAMP_PINK)
			{
				Gadget.ECHO_NET_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.LAMP_PINK && id < Gadget.Id.FASHION_POD_HANDLEBAR)
			{
				Gadget.LAMP_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.FASHION_POD_HANDLEBAR && id < Gadget.Id.GORDO_SNARE_NOVICE)
			{
				Gadget.FASHION_POD_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.GORDO_SNARE_NOVICE && id < Gadget.Id.SPONGE_TREE)
			{
				Gadget.SNARE_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.SPONGE_TREE && id < Gadget.Id.DRONE)
			{
				Gadget.DECO_CLASS.Add(id);
			}
			else if (id >= Gadget.Id.DRONE && id <= Gadget.Id.DRONE_ADVANCED)
			{
				Gadget.DRONE_CLASS.Add(id);
			}
		}
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x00090828 File Offset: 0x0008EA28
	public static bool IsExtractor(Gadget.Id gadgetId)
	{
		return Gadget.EXTRACTOR_CLASS.Contains(gadgetId);
	}

	// Token: 0x06002593 RID: 9619 RVA: 0x00090835 File Offset: 0x0008EA35
	public static bool IsTeleporter(Gadget.Id gadgetId)
	{
		return Gadget.TELEPORTER_CLASS.Contains(gadgetId);
	}

	// Token: 0x06002594 RID: 9620 RVA: 0x00090842 File Offset: 0x0008EA42
	public static bool IsWarpDepot(Gadget.Id gadgetId)
	{
		return Gadget.WARP_DEPOT_CLASS.Contains(gadgetId);
	}

	// Token: 0x06002595 RID: 9621 RVA: 0x0009084F File Offset: 0x0008EA4F
	public static bool IsMisc(Gadget.Id gadgetId)
	{
		return Gadget.MISC_CLASS.Contains(gadgetId);
	}

	// Token: 0x06002596 RID: 9622 RVA: 0x0009085C File Offset: 0x0008EA5C
	public static bool IsEchoNet(Gadget.Id gadgetId)
	{
		return Gadget.ECHO_NET_CLASS.Contains(gadgetId);
	}

	// Token: 0x06002597 RID: 9623 RVA: 0x00090869 File Offset: 0x0008EA69
	public static bool IsDrone(Gadget.Id gadgetId)
	{
		return Gadget.DRONE_CLASS.Contains(gadgetId);
	}

	// Token: 0x06002598 RID: 9624 RVA: 0x00090876 File Offset: 0x0008EA76
	public static bool IsLamp(Gadget.Id gadgetId)
	{
		return Gadget.LAMP_CLASS.Contains(gadgetId);
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x00090883 File Offset: 0x0008EA83
	public static bool IsFashionPod(Gadget.Id gadgetId)
	{
		return Gadget.FASHION_POD_CLASS.Contains(gadgetId);
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x00090890 File Offset: 0x0008EA90
	public static bool IsSnare(Gadget.Id gadgetId)
	{
		return Gadget.SNARE_CLASS.Contains(gadgetId);
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x0009089D File Offset: 0x0008EA9D
	public static bool IsDeco(Gadget.Id gadgetId)
	{
		return Gadget.DECO_CLASS.Contains(gadgetId);
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x000908AC File Offset: 0x0008EAAC
	public bool DestroysLinkedPairOnRemoval()
	{
		Gadget.LinkDestroyer componentInChildren = base.GetComponentInChildren<Gadget.LinkDestroyer>();
		return componentInChildren != null && componentInChildren.ShouldDestroyPair();
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x000908CB File Offset: 0x0008EACB
	public bool DestroysOnRemoval()
	{
		return SRSingleton<GameContext>.Instance.LookupDirector.GetGadgetDefinition(this.id).destroyOnRemoval;
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x000908E8 File Offset: 0x0008EAE8
	public static bool IsLinkDestroyerType(Gadget.Id id)
	{
		string text = id.ToString();
		return text.StartsWith("TELEPORTER_") || text.StartsWith("WARP_DEPOT_");
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x00090920 File Offset: 0x0008EB20
	public void DestroyGadget()
	{
		GadgetSite componentInParent = base.GetComponentInParent<GadgetSite>();
		if (componentInParent != null)
		{
			componentInParent.DestroyAttached();
		}
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x00090943 File Offset: 0x0008EB43
	public void AddRotation(float adjustment)
	{
		this.SetRotation(this.GetRotation() + adjustment);
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x00090953 File Offset: 0x0008EB53
	public void SetRotation(float rotation)
	{
		this.rotationTransform.localRotation = Quaternion.Euler(0f, rotation, 0f);
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x00090970 File Offset: 0x0008EB70
	public float GetRotation()
	{
		return this.rotationTransform.localRotation.eulerAngles.y;
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void OnUserDestroyed()
	{
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x00090998 File Offset: 0x0008EB98
	public static string GetName(Gadget.Id id, bool reportMissing = true)
	{
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("pedia");
		string arg = Enum.GetName(typeof(Gadget.Id), id).ToLowerInvariant();
		string resourceString = bundle.GetResourceString(string.Format("m.gadget.name.{0}", arg));
		if (reportMissing && resourceString == null)
		{
			Log.Warning("Missing gadget translation.", new object[]
			{
				"id",
				id
			});
		}
		return resourceString;
	}

	// Token: 0x0400248B RID: 9355
	public static Gadget.IdComparer idComparer = new Gadget.IdComparer();

	// Token: 0x0400248C RID: 9356
	public static List<Gadget.Id> ALL_FASHIONS = new List<Gadget.Id>
	{
		Gadget.Id.FASHION_POD_HANDLEBAR,
		Gadget.Id.FASHION_POD_SHADY,
		Gadget.Id.FASHION_POD_CLIP_ON,
		Gadget.Id.FASHION_POD_GOOGLY,
		Gadget.Id.FASHION_POD_SERIOUS,
		Gadget.Id.FASHION_POD_SMART,
		Gadget.Id.FASHION_POD_DANDY,
		Gadget.Id.FASHION_POD_CUTE,
		Gadget.Id.FASHION_POD_ROYAL,
		Gadget.Id.FASHION_POD_PARTY_GLASSES,
		Gadget.Id.FASHION_POD_SCUBA,
		Gadget.Id.FASHION_POD_REMOVER
	};

	// Token: 0x0400248D RID: 9357
	public static HashSet<Gadget.Id> EXTRACTOR_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x0400248E RID: 9358
	public static HashSet<Gadget.Id> TELEPORTER_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x0400248F RID: 9359
	public static HashSet<Gadget.Id> WARP_DEPOT_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x04002490 RID: 9360
	public static HashSet<Gadget.Id> LAMP_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x04002491 RID: 9361
	public static HashSet<Gadget.Id> MISC_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x04002492 RID: 9362
	public static HashSet<Gadget.Id> FASHION_POD_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x04002493 RID: 9363
	public static HashSet<Gadget.Id> SNARE_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x04002494 RID: 9364
	public static HashSet<Gadget.Id> ECHO_NET_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x04002495 RID: 9365
	public static HashSet<Gadget.Id> DRONE_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x04002496 RID: 9366
	public static HashSet<Gadget.Id> DECO_CLASS = new HashSet<Gadget.Id>(Gadget.idComparer);

	// Token: 0x04002497 RID: 9367
	public Gadget.Id id;

	// Token: 0x04002498 RID: 9368
	protected Transform rotationTransform;

	// Token: 0x02000709 RID: 1801
	public enum Id
	{
		// Token: 0x0400249A RID: 9370
		NONE,
		// Token: 0x0400249B RID: 9371
		EXTRACTOR_DRILL_NOVICE = 100,
		// Token: 0x0400249C RID: 9372
		EXTRACTOR_DRILL_ADVANCED,
		// Token: 0x0400249D RID: 9373
		EXTRACTOR_DRILL_MASTER,
		// Token: 0x0400249E RID: 9374
		EXTRACTOR_DRILL_TITAN,
		// Token: 0x0400249F RID: 9375
		EXTRACTOR_PUMP_NOVICE = 110,
		// Token: 0x040024A0 RID: 9376
		EXTRACTOR_PUMP_ADVANCED,
		// Token: 0x040024A1 RID: 9377
		EXTRACTOR_PUMP_MASTER,
		// Token: 0x040024A2 RID: 9378
		EXTRACTOR_PUMP_ABYSSAL,
		// Token: 0x040024A3 RID: 9379
		EXTRACTOR_APIARY_NOVICE = 120,
		// Token: 0x040024A4 RID: 9380
		EXTRACTOR_APIARY_ADVANCED,
		// Token: 0x040024A5 RID: 9381
		EXTRACTOR_APIARY_MASTER,
		// Token: 0x040024A6 RID: 9382
		EXTRACTOR_APIARY_ROYAL,
		// Token: 0x040024A7 RID: 9383
		TELEPORTER_PINK = 200,
		// Token: 0x040024A8 RID: 9384
		TELEPORTER_BLUE,
		// Token: 0x040024A9 RID: 9385
		TELEPORTER_VIOLET,
		// Token: 0x040024AA RID: 9386
		TELEPORTER_GREY,
		// Token: 0x040024AB RID: 9387
		TELEPORTER_GREEN,
		// Token: 0x040024AC RID: 9388
		TELEPORTER_RED,
		// Token: 0x040024AD RID: 9389
		TELEPORTER_AMBER,
		// Token: 0x040024AE RID: 9390
		TELEPORTER_GOLD,
		// Token: 0x040024AF RID: 9391
		TELEPORTER_BERRY,
		// Token: 0x040024B0 RID: 9392
		TELEPORTER_COCOA,
		// Token: 0x040024B1 RID: 9393
		TELEPORTER_BUTTERSCOTCH,
		// Token: 0x040024B2 RID: 9394
		WARP_DEPOT_PINK = 300,
		// Token: 0x040024B3 RID: 9395
		WARP_DEPOT_BLUE,
		// Token: 0x040024B4 RID: 9396
		WARP_DEPOT_VIOLET,
		// Token: 0x040024B5 RID: 9397
		WARP_DEPOT_GREY,
		// Token: 0x040024B6 RID: 9398
		WARP_DEPOT_GREEN,
		// Token: 0x040024B7 RID: 9399
		WARP_DEPOT_RED,
		// Token: 0x040024B8 RID: 9400
		WARP_DEPOT_AMBER,
		// Token: 0x040024B9 RID: 9401
		WARP_DEPOT_GOLD,
		// Token: 0x040024BA RID: 9402
		WARP_DEPOT_BERRY,
		// Token: 0x040024BB RID: 9403
		WARP_DEPOT_COCOA,
		// Token: 0x040024BC RID: 9404
		WARP_DEPOT_BUTTERSCOTCH,
		// Token: 0x040024BD RID: 9405
		MARKET_LINK = 390,
		// Token: 0x040024BE RID: 9406
		MED_STATION = 400,
		// Token: 0x040024BF RID: 9407
		RAPID_MED_STATION,
		// Token: 0x040024C0 RID: 9408
		HYDRO_TURRET = 410,
		// Token: 0x040024C1 RID: 9409
		SUPER_HYDRO_TURRET,
		// Token: 0x040024C2 RID: 9410
		HYDRO_SHOWER = 420,
		// Token: 0x040024C3 RID: 9411
		SUPER_HYDRO_SHOWER,
		// Token: 0x040024C4 RID: 9412
		TAMING_BELL = 430,
		// Token: 0x040024C5 RID: 9413
		ELITE_TAMING_BELL,
		// Token: 0x040024C6 RID: 9414
		SPRING_PAD = 460,
		// Token: 0x040024C7 RID: 9415
		POTTED_TACTUS,
		// Token: 0x040024C8 RID: 9416
		REFINERY_LINK,
		// Token: 0x040024C9 RID: 9417
		SLIME_HOOP,
		// Token: 0x040024CA RID: 9418
		SLIME_STAGE,
		// Token: 0x040024CB RID: 9419
		ECHO_NET,
		// Token: 0x040024CC RID: 9420
		LAMP_PINK = 500,
		// Token: 0x040024CD RID: 9421
		LAMP_BLUE,
		// Token: 0x040024CE RID: 9422
		LAMP_VIOLET,
		// Token: 0x040024CF RID: 9423
		LAMP_GREY,
		// Token: 0x040024D0 RID: 9424
		LAMP_GREEN,
		// Token: 0x040024D1 RID: 9425
		LAMP_RED,
		// Token: 0x040024D2 RID: 9426
		LAMP_AMBER,
		// Token: 0x040024D3 RID: 9427
		LAMP_GOLD,
		// Token: 0x040024D4 RID: 9428
		LAMP_BERRY,
		// Token: 0x040024D5 RID: 9429
		LAMP_COCOA,
		// Token: 0x040024D6 RID: 9430
		LAMP_BUTTERSCOTCH,
		// Token: 0x040024D7 RID: 9431
		FASHION_POD_HANDLEBAR = 600,
		// Token: 0x040024D8 RID: 9432
		FASHION_POD_SHADY,
		// Token: 0x040024D9 RID: 9433
		FASHION_POD_CLIP_ON,
		// Token: 0x040024DA RID: 9434
		FASHION_POD_GOOGLY,
		// Token: 0x040024DB RID: 9435
		FASHION_POD_SERIOUS,
		// Token: 0x040024DC RID: 9436
		FASHION_POD_SMART,
		// Token: 0x040024DD RID: 9437
		FASHION_POD_CUTE,
		// Token: 0x040024DE RID: 9438
		FASHION_POD_ROYAL,
		// Token: 0x040024DF RID: 9439
		FASHION_POD_DANDY,
		// Token: 0x040024E0 RID: 9440
		FASHION_POD_PIRATEY,
		// Token: 0x040024E1 RID: 9441
		FASHION_POD_HEROIC,
		// Token: 0x040024E2 RID: 9442
		FASHION_POD_SCIFI,
		// Token: 0x040024E3 RID: 9443
		FASHION_POD_PARTY_GLASSES,
		// Token: 0x040024E4 RID: 9444
		FASHION_POD_SCUBA,
		// Token: 0x040024E5 RID: 9445
		FASHION_POD_REMOVER = 699,
		// Token: 0x040024E6 RID: 9446
		GORDO_SNARE_NOVICE,
		// Token: 0x040024E7 RID: 9447
		GORDO_SNARE_ADVANCED,
		// Token: 0x040024E8 RID: 9448
		GORDO_SNARE_MASTER,
		// Token: 0x040024E9 RID: 9449
		SPONGE_TREE = 11000,
		// Token: 0x040024EA RID: 9450
		SPONGE_SHRUB,
		// Token: 0x040024EB RID: 9451
		PINK_CORAL_COLUMNS,
		// Token: 0x040024EC RID: 9452
		CORAL_GRASS_PATCH,
		// Token: 0x040024ED RID: 9453
		MOSSY_TREE = 12000,
		// Token: 0x040024EE RID: 9454
		MOSSY_TREE_STUMP,
		// Token: 0x040024EF RID: 9455
		GLOW_CONES,
		// Token: 0x040024F0 RID: 9456
		WILDFLOWER_PATCH,
		// Token: 0x040024F1 RID: 9457
		JUMBO_SHROOM,
		// Token: 0x040024F2 RID: 9458
		MINTY_GRASS_PATCH = 13000,
		// Token: 0x040024F3 RID: 9459
		BLUE_CORAL_COLUMNS,
		// Token: 0x040024F4 RID: 9460
		HEXIUM_FORMATION,
		// Token: 0x040024F5 RID: 9461
		CRYSTAL_CLUSTER,
		// Token: 0x040024F6 RID: 9462
		FIREFLOWER_PATCH,
		// Token: 0x040024F7 RID: 9463
		SUNBURST_TREE = 14000,
		// Token: 0x040024F8 RID: 9464
		VERDANT_GRASS_PATCH,
		// Token: 0x040024F9 RID: 9465
		STAR_FLOWER_PATCH,
		// Token: 0x040024FA RID: 9466
		RUINED_PILLAR,
		// Token: 0x040024FB RID: 9467
		GLOW_STICKS,
		// Token: 0x040024FC RID: 9468
		CRYSTAL_SCONCE,
		// Token: 0x040024FD RID: 9469
		FIERY_GLASS_SCULPTURE = 15000,
		// Token: 0x040024FE RID: 9470
		THUNDERING_GLASS_SCULPTURE,
		// Token: 0x040024FF RID: 9471
		TOWERING_GLASS_SCULPTURE,
		// Token: 0x04002500 RID: 9472
		PALM_TREE,
		// Token: 0x04002501 RID: 9473
		PALM_SPROUT,
		// Token: 0x04002502 RID: 9474
		COIL_GRASS,
		// Token: 0x04002503 RID: 9475
		RUINED_DESERT_COLUMN,
		// Token: 0x04002504 RID: 9476
		RUINED_DESERT_BLOCKS,
		// Token: 0x04002505 RID: 9477
		DESERT_DECO_9,
		// Token: 0x04002506 RID: 9478
		WILDS_ROCKS_1 = 16000,
		// Token: 0x04002507 RID: 9479
		WILDS_ROCKS_2,
		// Token: 0x04002508 RID: 9480
		WILDS_ROCKS_3,
		// Token: 0x04002509 RID: 9481
		WILDS_TREE,
		// Token: 0x0400250A RID: 9482
		WILDS_CORAL_COLUMN,
		// Token: 0x0400250B RID: 9483
		WILDS_GRASS_PATCH,
		// Token: 0x0400250C RID: 9484
		WILDS_FLOWER_PATCH,
		// Token: 0x0400250D RID: 9485
		MAGNETICORE_ARRAY_SMALL = 17000,
		// Token: 0x0400250E RID: 9486
		MAGNETICORE_ARRAY_TALL,
		// Token: 0x0400250F RID: 9487
		MAGNETICORE_ARRAY_STURDY,
		// Token: 0x04002510 RID: 9488
		MAGNETICORE_ARRAY_ORNATE,
		// Token: 0x04002511 RID: 9489
		NIMBLE_GRASS_PATCH,
		// Token: 0x04002512 RID: 9490
		NIMBLE_NEEDLE_TREE,
		// Token: 0x04002513 RID: 9491
		DECO_BATTERY_TOWER = 17100,
		// Token: 0x04002514 RID: 9492
		DECO_DIGI_PANEL,
		// Token: 0x04002515 RID: 9493
		DECO_DIGI_SHRUB,
		// Token: 0x04002516 RID: 9494
		DECO_DIGI_TREE,
		// Token: 0x04002517 RID: 9495
		DECO_FIELD_KIT,
		// Token: 0x04002518 RID: 9496
		DECO_SUPPLY_DROP,
		// Token: 0x04002519 RID: 9497
		DRONE = 18000,
		// Token: 0x0400251A RID: 9498
		DRONE_ADVANCED,
		// Token: 0x0400251B RID: 9499
		CHICKEN_CLONER = 19000,
		// Token: 0x0400251C RID: 9500
		PORTABLE_WATER_TAP,
		// Token: 0x0400251D RID: 9501
		PORTABLE_SLIME_BAIT_FRUIT,
		// Token: 0x0400251E RID: 9502
		PORTABLE_SCARECROW,
		// Token: 0x0400251F RID: 9503
		DASH_PAD = 19005,
		// Token: 0x04002520 RID: 9504
		PORTABLE_SLIME_BAIT_VEGGIE,
		// Token: 0x04002521 RID: 9505
		PORTABLE_SLIME_BAIT_MEAT
	}

	// Token: 0x0200070A RID: 1802
	public class IdComparer : IEqualityComparer<Gadget.Id>
	{
		// Token: 0x060025A6 RID: 9638 RVA: 0x00017781 File Offset: 0x00015981
		public bool Equals(Gadget.Id id1, Gadget.Id id2)
		{
			return id1 == id2;
		}

		// Token: 0x060025A7 RID: 9639 RVA: 0x00017787 File Offset: 0x00015987
		public int GetHashCode(Gadget.Id obj)
		{
			return (int)obj;
		}
	}

	// Token: 0x0200070B RID: 1803
	public interface LinkDestroyer
	{
		// Token: 0x060025A9 RID: 9641
		bool ShouldDestroyPair();

		// Token: 0x060025AA RID: 9642
		Gadget.LinkDestroyer GetLinked();
	}
}
