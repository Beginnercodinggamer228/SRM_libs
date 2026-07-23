using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class DroneMetadata : ScriptableObject
{
	// Token: 0x060006EF RID: 1775 RVA: 0x00023D54 File Offset: 0x00021F54
	public void OnEnable()
	{
		this.targets = new DroneMetadata.Program.Target[]
		{
			new DroneMetadata.Program.Target.Collection("plorts", Identifiable.PLORT_CLASS, this.imageTargetCollectionPlorts),
			new DroneMetadata.Program.Target.Collection("veggies", Identifiable.VEGGIE_CLASS, this.imageTargetCollectionVeggies),
			new DroneMetadata.Program.Target.Collection("fruits", Identifiable.FRUIT_CLASS, this.imageTargetCollectionFruits),
			new DroneMetadata.Program.Target.Collection("meats", Identifiable.MEAT_CLASS, this.imageTargetCollectionMeats),
			new DroneMetadata.Program.Target.Collection("elders", Identifiable.ELDER_CLASS, this.imageTargetCollectionElders),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.PINK_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.ROCK_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.TABBY_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.PHOSPHOR_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.RAD_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.BOOM_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.HONEY_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.PUDDLE_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.CRYSTAL_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.HUNTER_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.QUANTUM_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.MOSAIC_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.DERVISH_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.TANGLE_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.FIRE_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.SABER_PLORT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.CARROT_VEGGIE),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.OCAOCA_VEGGIE),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.BEET_VEGGIE),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.PARSNIP_VEGGIE),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.ONION_VEGGIE),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.POGO_FRUIT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.MANGO_FRUIT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.CUBERRY_FRUIT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.LEMON_FRUIT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.PEAR_FRUIT),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.HEN),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.ROOSTER),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.STONY_HEN),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.BRIAR_HEN),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.PAINTED_HEN),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.ELDER_HEN),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.ELDER_ROOSTER),
			new DroneMetadata.Program.Target.Basic(Identifiable.Id.SPICY_TOFU)
		};
		DroneMetadata.Program.Behaviour[] array = new DroneMetadata.Program.Behaviour[9];
		int num = 0;
		DroneMetadata.Program.Behaviour behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.source.name.corral";
		behaviour.image = this.imageSourceCorral;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramSourceCorral)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsPlort(p.target.ident));
		array[num] = behaviour;
		int num2 = 1;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.source.name.pond";
		behaviour.image = this.imageSourcePond;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramSourcePond)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsPlort(p.target.ident));
		array[num2] = behaviour;
		int num3 = 2;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.source.name.incinerator";
		behaviour.image = this.imageSourceIncinerator;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramSourceIncinerator)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsPlort(p.target.ident));
		array[num3] = behaviour;
		int num4 = 3;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.source.name.garden";
		behaviour.image = this.imageSourceGarden;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramSourceGarden)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsFruit(p.target.ident) || Identifiable.IsVeggie(p.target.ident));
		array[num4] = behaviour;
		int num5 = 4;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.source.name.coop";
		behaviour.image = this.imageSourceCoop;
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsAnimal(p.target.ident));
		behaviour.types = new Type[]
		{
			typeof(DroneProgramSourceElderCollector),
			typeof(DroneProgramSourceCoop)
		};
		array[num5] = behaviour;
		array[5] = new DroneMetadata.Program.Behaviour
		{
			id = "m.drone.source.name.silo",
			image = this.imageSourceSilo,
			types = new Type[]
			{
				typeof(DroneProgramSourceSilo)
			}
		};
		int num6 = 6;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.source.name.plort_collector";
		behaviour.image = this.imageSourcePlortCollector;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramSourcePlortCollector)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsPlort(p.target.ident));
		array[num6] = behaviour;
		int num7 = 7;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.source.name.dynamic";
		behaviour.image = this.imageSourceOutsidePlots;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramSourceOutsidePlots)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => !Identifiable.IsPlort(p.target.ident));
		array[num7] = behaviour;
		int num8 = 8;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.source.name.free_range";
		behaviour.image = this.imageSourceFreeRange;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramSourceFreeRange)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsPlort(p.target.ident));
		array[num8] = behaviour;
		this.sources = array;
		DroneMetadata.Program.Behaviour[] array2 = new DroneMetadata.Program.Behaviour[6];
		int num9 = 0;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.destination.name.corral";
		behaviour.image = this.imageDestinationCorral;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramDestinationCorral)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsFood(p.target.ident));
		array2[num9] = behaviour;
		array2[1] = new DroneMetadata.Program.Behaviour
		{
			id = "m.drone.destination.name.silo",
			image = this.imageDestinationSilo,
			types = new Type[]
			{
				typeof(DroneProgramDestinationSilo)
			}
		};
		int num10 = 2;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.destination.name.auto_feeder";
		behaviour.image = this.imageDestinationFeeder;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramDestinationFeeder)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsFood(p.target.ident));
		array2[num10] = behaviour;
		int num11 = 3;
		behaviour = new DroneMetadata.Program.Behaviour();
		behaviour.id = "m.drone.destination.name.incinerator";
		behaviour.image = this.imageDestinationIncinerator;
		behaviour.types = new Type[]
		{
			typeof(DroneProgramDestinationIncinerator)
		};
		behaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsFood(p.target.ident));
		array2[num11] = behaviour;
		int num12 = 4;
		DroneMetadata.Program.GadgetBehaviour gadgetBehaviour = new DroneMetadata.Program.GadgetBehaviour();
		gadgetBehaviour.id = "m.drone.destination.name.plort_market";
		gadgetBehaviour.image = this.imageDestinationPlortMarket;
		gadgetBehaviour.types = new Type[]
		{
			typeof(DroneProgramDestinationPlortMarket)
		};
		gadgetBehaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsPlort(p.target.ident));
		gadgetBehaviour.gadget = Gadget.Id.MARKET_LINK;
		gadgetBehaviour.gadgetPredicate = (() => SRSingleton<SceneContext>.Instance.Player.GetComponent<RegionMember>().regions.All((Region r) => r.gameObject.name != "cellRanch_Home"));
		array2[num12] = gadgetBehaviour;
		int num13 = 5;
		gadgetBehaviour = new DroneMetadata.Program.GadgetBehaviour();
		gadgetBehaviour.id = "m.drone.destination.name.refinery";
		gadgetBehaviour.image = this.imageDestinationRefinery;
		gadgetBehaviour.types = new Type[]
		{
			typeof(DroneProgramDestinationRefinery)
		};
		gadgetBehaviour.isCompatible = ((DroneMetadata.Program p) => Identifiable.IsPlort(p.target.ident));
		gadgetBehaviour.gadget = Gadget.Id.REFINERY_LINK;
		gadgetBehaviour.gadgetPredicate = (() => SRSingleton<SceneContext>.Instance.Player.GetComponent<RegionMember>().regions.All((Region r) => r.gameObject.name != "cellRanch_Lab"));
		array2[num13] = gadgetBehaviour;
		this.destinations = array2;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x00024528 File Offset: 0x00022728
	public DroneMetadata.Program.Target GetDefaultTarget()
	{
		DroneMetadata.Program.Target target = new DroneMetadata.Program.Target();
		target.id = "drone.target.none";
		target.image = this.imageNone;
		target.predicate = ((Identifiable.Id id) => false);
		return target;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x00024576 File Offset: 0x00022776
	public DroneMetadata.Program.Behaviour GetDefaultBehaviour()
	{
		return new DroneMetadata.Program.Behaviour
		{
			id = "drone.behaviour.none",
			image = this.imageNone,
			types = new Type[0]
		};
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x060006F2 RID: 1778 RVA: 0x000245A0 File Offset: 0x000227A0
	// (set) Token: 0x060006F3 RID: 1779 RVA: 0x000245A8 File Offset: 0x000227A8
	public DroneMetadata.Program.Target[] targets { get; private set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x060006F4 RID: 1780 RVA: 0x000245B1 File Offset: 0x000227B1
	// (set) Token: 0x060006F5 RID: 1781 RVA: 0x000245B9 File Offset: 0x000227B9
	public DroneMetadata.Program.Behaviour[] sources { get; private set; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x060006F6 RID: 1782 RVA: 0x000245C2 File Offset: 0x000227C2
	// (set) Token: 0x060006F7 RID: 1783 RVA: 0x000245CA File Offset: 0x000227CA
	public DroneMetadata.Program.Behaviour[] destinations { get; private set; }

	// Token: 0x0400064E RID: 1614
	[Header("GameObject Prefabs")]
	public DroneUI droneUI;

	// Token: 0x0400064F RID: 1615
	public DroneUIProgram droneUIProgram;

	// Token: 0x04000650 RID: 1616
	public DroneUIProgramPicker droneUIProgramPicker;

	// Token: 0x04000651 RID: 1617
	public DroneUIProgramButton droneUIProgramButton;

	// Token: 0x04000652 RID: 1618
	[Header("Program Component Images")]
	public Sprite imageTargetCollectionPlorts;

	// Token: 0x04000653 RID: 1619
	public Sprite imageTargetCollectionFruits;

	// Token: 0x04000654 RID: 1620
	public Sprite imageTargetCollectionVeggies;

	// Token: 0x04000655 RID: 1621
	public Sprite imageTargetCollectionMeats;

	// Token: 0x04000656 RID: 1622
	public Sprite imageTargetCollectionElders;

	// Token: 0x04000657 RID: 1623
	public Sprite imageSourceCorral;

	// Token: 0x04000658 RID: 1624
	public Sprite imageSourcePond;

	// Token: 0x04000659 RID: 1625
	public Sprite imageSourceIncinerator;

	// Token: 0x0400065A RID: 1626
	public Sprite imageSourceGarden;

	// Token: 0x0400065B RID: 1627
	public Sprite imageSourceCoop;

	// Token: 0x0400065C RID: 1628
	public Sprite imageSourceSilo;

	// Token: 0x0400065D RID: 1629
	public Sprite imageSourcePlortCollector;

	// Token: 0x0400065E RID: 1630
	public Sprite imageSourceOutsidePlots;

	// Token: 0x0400065F RID: 1631
	public Sprite imageSourceFreeRange;

	// Token: 0x04000660 RID: 1632
	public Sprite imageDestinationCorral;

	// Token: 0x04000661 RID: 1633
	public Sprite imageDestinationSilo;

	// Token: 0x04000662 RID: 1634
	public Sprite imageDestinationFeeder;

	// Token: 0x04000663 RID: 1635
	public Sprite imageDestinationIncinerator;

	// Token: 0x04000664 RID: 1636
	public Sprite imageDestinationPlortMarket;

	// Token: 0x04000665 RID: 1637
	public Sprite imageDestinationRefinery;

	// Token: 0x04000666 RID: 1638
	public Sprite imageNone;

	// Token: 0x04000667 RID: 1639
	public Sprite pickTargetIcon;

	// Token: 0x04000668 RID: 1640
	public Sprite pickSourceIcon;

	// Token: 0x04000669 RID: 1641
	public Sprite pickDestinationIcon;

	// Token: 0x0400066A RID: 1642
	[Header("SFX")]
	public SECTR_AudioCue onActiveCue;

	// Token: 0x0400066B RID: 1643
	public SECTR_AudioCue onGatherBeginCue;

	// Token: 0x0400066C RID: 1644
	public SECTR_AudioCue onGatherLoopCue;

	// Token: 0x0400066D RID: 1645
	public SECTR_AudioCue onGatherEndCue;

	// Token: 0x0400066E RID: 1646
	public SECTR_AudioCue onDepositBeginCue;

	// Token: 0x0400066F RID: 1647
	public SECTR_AudioCue onDepositLoopCue;

	// Token: 0x04000670 RID: 1648
	public SECTR_AudioCue onDepositEndCue;

	// Token: 0x04000671 RID: 1649
	public SECTR_AudioCue onRestBeginCue;

	// Token: 0x04000672 RID: 1650
	public SECTR_AudioCue onRestLoopCue;

	// Token: 0x04000673 RID: 1651
	public SECTR_AudioCue onRestEndCue;

	// Token: 0x04000674 RID: 1652
	public SECTR_AudioCue onHappyCue;

	// Token: 0x04000675 RID: 1653
	public SECTR_AudioCue onGrumpyCue;

	// Token: 0x04000676 RID: 1654
	public SECTR_AudioCue onBoppedCue;

	// Token: 0x04000677 RID: 1655
	public SECTR_AudioCue onBatteryFilledCue;

	// Token: 0x04000678 RID: 1656
	public SECTR_AudioCue onGuiEnableCue;

	// Token: 0x04000679 RID: 1657
	public SECTR_AudioCue onGuiDisableCue;

	// Token: 0x0400067A RID: 1658
	public SECTR_AudioCue onGuiButtonTargetCue;

	// Token: 0x0400067B RID: 1659
	public SECTR_AudioCue onGuiButtonSourceCue;

	// Token: 0x0400067C RID: 1660
	public SECTR_AudioCue onGuiButtonDestinationCue;

	// Token: 0x0400067D RID: 1661
	public SECTR_AudioCue onGuiButtonActivateCue;

	// Token: 0x0400067E RID: 1662
	public SECTR_AudioCue onGuiButtonResetCue;

	// Token: 0x0400067F RID: 1663
	[Header("FX")]
	public GameObject onBatteryFilledFX;

	// Token: 0x04000680 RID: 1664
	public GameObject onTeleportFX;

	// Token: 0x04000681 RID: 1665
	[Header("Coins Override")]
	public Sprite coinsIcon;

	// Token: 0x04000682 RID: 1666
	public Color coinsColor;

	// Token: 0x04000683 RID: 1667
	public SECTR_AudioCue coinsCue;

	// Token: 0x04000684 RID: 1668
	public const string TARGET_NONE_ID = "drone.target.none";

	// Token: 0x04000685 RID: 1669
	public const string BEHAVIOUR_NONE_ID = "drone.behaviour.none";

	// Token: 0x02000140 RID: 320
	public class Program
	{
		// Token: 0x060006F9 RID: 1785 RVA: 0x000053FC File Offset: 0x000035FC
		public Program()
		{
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x000245D3 File Offset: 0x000227D3
		public Program(DroneMetadata.Program.Target target, DroneMetadata.Program.Behaviour source, DroneMetadata.Program.Behaviour destination)
		{
			this.target = target;
			this.source = source;
			this.destination = destination;
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x000245F0 File Offset: 0x000227F0
		public DroneMetadata.Program Clone()
		{
			return new DroneMetadata.Program(this.target, this.source, this.destination);
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0002460C File Offset: 0x0002280C
		public bool IsComplete()
		{
			return this.target.id != "drone.target.none" && this.source.id != "drone.behaviour.none" && this.destination.id != "drone.behaviour.none";
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x00024660 File Offset: 0x00022860
		public bool IsReset()
		{
			return this.target.id == "drone.target.none" && this.source.id == "drone.behaviour.none" && this.destination.id == "drone.behaviour.none";
		}

		// Token: 0x04000689 RID: 1673
		public DroneMetadata.Program.Target target;

		// Token: 0x0400068A RID: 1674
		public DroneMetadata.Program.Behaviour source;

		// Token: 0x0400068B RID: 1675
		public DroneMetadata.Program.Behaviour destination;

		// Token: 0x02000141 RID: 321
		public abstract class BaseComponent
		{
			// Token: 0x060006FE RID: 1790 RVA: 0x000246B2 File Offset: 0x000228B2
			public virtual string GetName()
			{
				return SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui").Get(this.id);
			}

			// Token: 0x060006FF RID: 1791 RVA: 0x000246D3 File Offset: 0x000228D3
			public virtual Sprite GetImage()
			{
				return this.image;
			}

			// Token: 0x0400068C RID: 1676
			public string id;

			// Token: 0x0400068D RID: 1677
			public Sprite image;
		}

		// Token: 0x02000142 RID: 322
		public class Target : DroneMetadata.Program.BaseComponent
		{
			// Token: 0x0400068E RID: 1678
			public Identifiable.Id ident;

			// Token: 0x0400068F RID: 1679
			public Predicate<Identifiable.Id> predicate;

			// Token: 0x02000143 RID: 323
			public class Basic : DroneMetadata.Program.Target
			{
				// Token: 0x06000702 RID: 1794 RVA: 0x000246E4 File Offset: 0x000228E4
				public Basic(Identifiable.Id ident)
				{
					this.id = string.Format("m.drone.target.identifiable.{0}", Enum.GetName(typeof(Identifiable.Id), ident).ToLowerInvariant());
					this.ident = ident;
					this.predicate = ((Identifiable.Id rhs) => rhs == ident);
				}

				// Token: 0x06000703 RID: 1795 RVA: 0x00024751 File Offset: 0x00022951
				public override string GetName()
				{
					return Identifiable.GetName(this.ident, true);
				}

				// Token: 0x06000704 RID: 1796 RVA: 0x0002475F File Offset: 0x0002295F
				public override Sprite GetImage()
				{
					return SRSingleton<GameContext>.Instance.LookupDirector.GetIcon(this.ident);
				}
			}

			// Token: 0x02000145 RID: 325
			public class Collection : DroneMetadata.Program.Target
			{
				// Token: 0x06000707 RID: 1799 RVA: 0x00024784 File Offset: 0x00022984
				public Collection(string id, HashSet<Identifiable.Id> collection, Sprite image)
				{
					this.id = string.Format("m.drone.target.name.category_{0}", id);
					this.ident = collection.First<Identifiable.Id>();
					this.predicate = ((Identifiable.Id rhs) => collection.Contains(rhs));
					this.collection = collection;
					this.image = image;
				}

				// Token: 0x04000691 RID: 1681
				public HashSet<Identifiable.Id> collection;
			}
		}

		// Token: 0x02000147 RID: 327
		public class Behaviour : DroneMetadata.Program.BaseComponent
		{
			// Token: 0x04000693 RID: 1683
			public Type[] types;

			// Token: 0x04000694 RID: 1684
			public Predicate<DroneMetadata.Program> isCompatible = (DroneMetadata.Program p) => true;
		}

		// Token: 0x02000149 RID: 329
		public class GadgetBehaviour : DroneMetadata.Program.Behaviour
		{
			// Token: 0x0600070E RID: 1806 RVA: 0x00024834 File Offset: 0x00022A34
			public override string GetName()
			{
				if (this.gadgetPredicate())
				{
					return SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("pedia").Get(string.Format("m.gadget.name.{0}", this.gadget.ToString().ToLowerInvariant()));
				}
				return base.GetName();
			}

			// Token: 0x0600070F RID: 1807 RVA: 0x0002488E File Offset: 0x00022A8E
			public override Sprite GetImage()
			{
				if (this.gadgetPredicate())
				{
					return SRSingleton<GameContext>.Instance.LookupDirector.GetGadgetDefinition(this.gadget).icon;
				}
				return base.GetImage();
			}

			// Token: 0x04000697 RID: 1687
			public Func<bool> gadgetPredicate;

			// Token: 0x04000698 RID: 1688
			public Gadget.Id gadget;
		}
	}
}
