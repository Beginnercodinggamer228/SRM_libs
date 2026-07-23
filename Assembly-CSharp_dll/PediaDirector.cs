using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000741 RID: 1857
public class PediaDirector : SRBehaviour, PediaModel.Participant
{
	// Token: 0x060026D1 RID: 9937 RVA: 0x00093F2D File Offset: 0x0009212D
	public IEnumerable<PediaDirector.Id> GetInitUnlocked()
	{
		return this.initUnlocked;
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x00093F38 File Offset: 0x00092138
	public void OnUnlockedChanged(HashSet<PediaDirector.Id> unlocked)
	{
		if (this.entries.All((PediaDirector.IdEntry e) => unlocked.Contains(e.id) || PediaDirector.HIDDEN_ENTRIES.Contains(e.id)))
		{
			this.achieveDir.MaybeUpdateMaxStat(AchievementsDirector.IntStat.COMPLETED_SLIMEPEDIA, 1);
		}
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x00093F79 File Offset: 0x00092179
	public void UnlockScience()
	{
		this.pediaModel.Unlock(new PediaDirector.Id[]
		{
			PediaDirector.Id.REFINERY,
			PediaDirector.Id.FABRICATOR,
			PediaDirector.Id.BLUEPRINTS,
			PediaDirector.Id.EXTRACTORS,
			PediaDirector.Id.UTILITIES,
			PediaDirector.Id.WARP_TECH,
			PediaDirector.Id.DECORATIONS,
			PediaDirector.Id.CURIOS,
			PediaDirector.Id.DRONES,
			PediaDirector.Id.SSBASICS,
			PediaDirector.Id.GADGETMODE
		});
	}

	// Token: 0x060026D4 RID: 9940 RVA: 0x00093F98 File Offset: 0x00092198
	public void Unlock(params PediaDirector.Id[] ids)
	{
		this.pediaModel.Unlock(ids);
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x00093FA8 File Offset: 0x000921A8
	public void Awake()
	{
		foreach (PediaDirector.IdEntry idEntry in this.entries)
		{
			this.entryDict[idEntry.id] = idEntry;
		}
		foreach (PediaDirector.IdentMapEntry identMapEntry in this.identMapEntries)
		{
			this.identDict[identMapEntry.identId] = identMapEntry.pediaId;
		}
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x00094033 File Offset: 0x00092233
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterPedia(this);
	}

	// Token: 0x060026D7 RID: 9943 RVA: 0x00094045 File Offset: 0x00092245
	public void InitModel(PediaModel pediaModel)
	{
		pediaModel.ResetUnlocked(this.initUnlocked);
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x00094053 File Offset: 0x00092253
	public void SetModel(PediaModel pediaModel)
	{
		this.pediaModel = pediaModel;
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x0009405C File Offset: 0x0009225C
	public void Update()
	{
		if (SRInput.Actions.pedia.WasPressed)
		{
			PediaDirector.Id pediaId = PediaDirector.Id.BASICS;
			PediaPopupUI pediaPopupUI = UnityEngine.Object.FindObjectOfType<PediaPopupUI>();
			if (pediaPopupUI != null)
			{
				pediaId = pediaPopupUI.GetId();
				Destroyer.Destroy(pediaPopupUI.gameObject, "PediaDirector.Update");
			}
			this.ShowPedia(pediaId);
		}
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000940AE File Offset: 0x000922AE
	public PediaDirector.IdEntry Get(PediaDirector.Id id)
	{
		if (!this.IsUnlocked(id))
		{
			return this.lockedEntry;
		}
		if (this.entryDict.ContainsKey(id))
		{
			return this.entryDict.Get(id);
		}
		return this.lockedEntry;
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000940E1 File Offset: 0x000922E1
	public int GetUnlockedCount()
	{
		return this.pediaModel.unlocked.Count;
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000940F3 File Offset: 0x000922F3
	public void UnlockWithoutPopup(PediaDirector.Id id)
	{
		this.pediaModel.Unlock(new PediaDirector.Id[]
		{
			id
		});
	}

	// Token: 0x060026DD RID: 9949 RVA: 0x0009410C File Offset: 0x0009230C
	public void MaybeShowPopup(PediaDirector.Id id)
	{
		if (!this.IsUnlocked(id))
		{
			if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().assumeExperiencedUser)
			{
				this.pediaModel.unlocked.Add(id);
				return;
			}
			PediaDirector.PediaPopupCreator creator = new PediaDirector.PediaPopupCreator(this, id);
			if (!this.popupDir.IsQueued(creator))
			{
				this.popupDir.QueueForPopup(creator);
				this.popupDir.MaybePopupNext();
			}
		}
	}

	// Token: 0x060026DE RID: 9950 RVA: 0x00094178 File Offset: 0x00092378
	public void MaybeShowPopup(Identifiable.Id identId)
	{
		if (this.identDict.ContainsKey(identId))
		{
			this.MaybeShowPopup(this.identDict[identId]);
		}
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x0009419A File Offset: 0x0009239A
	public GameObject ShowPedia(PediaDirector.Id pediaId)
	{
		this.pediaUiObject = UnityEngine.Object.Instantiate<GameObject>(this.pediaPanelPrefab);
		PediaUI component = this.pediaUiObject.GetComponent<PediaUI>();
		component.SelectEntry(pediaId, true, pediaId);
		return component.gameObject;
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000941C6 File Offset: 0x000923C6
	public bool IsPediaOpen()
	{
		return this.pediaUiObject != null;
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000941D4 File Offset: 0x000923D4
	public PediaDirector.Id? GetPediaId(Identifiable.Id identId)
	{
		if (this.identDict.ContainsKey(identId))
		{
			return new PediaDirector.Id?(this.identDict[identId]);
		}
		return null;
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x0009420A File Offset: 0x0009240A
	public bool IsUnlocked(PediaDirector.Id id)
	{
		return this.pediaModel.unlocked.Contains(id);
	}

	// Token: 0x04002601 RID: 9729
	public static HashSet<PediaDirector.Id> HIDDEN_ENTRIES = new HashSet<PediaDirector.Id>
	{
		PediaDirector.Id.PARTY_GORDO_SLIME,
		PediaDirector.Id.ECHO_NOTE_GORDO_SLIME,
		PediaDirector.Id.ECHO_NOTES
	};

	// Token: 0x04002602 RID: 9730
	public const PediaDirector.Id DEFAULT_PEDIA_ENTRY = PediaDirector.Id.BASICS;

	// Token: 0x04002603 RID: 9731
	public Sprite unknownIcon;

	// Token: 0x04002604 RID: 9732
	public PediaDirector.IdEntry lockedEntry;

	// Token: 0x04002605 RID: 9733
	public PediaDirector.IdEntry[] entries;

	// Token: 0x04002606 RID: 9734
	public PediaDirector.Id[] initUnlocked;

	// Token: 0x04002607 RID: 9735
	public PediaDirector.IdentMapEntry[] identMapEntries;

	// Token: 0x04002608 RID: 9736
	public GameObject pediaPopupPrefab;

	// Token: 0x04002609 RID: 9737
	public GameObject pediaPanelPrefab;

	// Token: 0x0400260A RID: 9738
	public GameObject pediaListingPrefab;

	// Token: 0x0400260B RID: 9739
	private Dictionary<Identifiable.Id, PediaDirector.Id> identDict = new Dictionary<Identifiable.Id, PediaDirector.Id>(Identifiable.idComparer);

	// Token: 0x0400260C RID: 9740
	private Dictionary<PediaDirector.Id, PediaDirector.IdEntry> entryDict = new Dictionary<PediaDirector.Id, PediaDirector.IdEntry>();

	// Token: 0x0400260D RID: 9741
	private AchievementsDirector achieveDir;

	// Token: 0x0400260E RID: 9742
	private PopupDirector popupDir;

	// Token: 0x0400260F RID: 9743
	private GameObject pediaUiObject;

	// Token: 0x04002610 RID: 9744
	private PediaModel pediaModel;

	// Token: 0x02000742 RID: 1858
	public enum Id
	{
		// Token: 0x04002612 RID: 9746
		TUTORIALS,
		// Token: 0x04002613 RID: 9747
		SLIMES,
		// Token: 0x04002614 RID: 9748
		RESOURCES,
		// Token: 0x04002615 RID: 9749
		RANCH,
		// Token: 0x04002616 RID: 9750
		WORLD,
		// Token: 0x04002617 RID: 9751
		SCIENCE,
		// Token: 0x04002618 RID: 9752
		SPLASH = 1000,
		// Token: 0x04002619 RID: 9753
		BASICS,
		// Token: 0x0400261A RID: 9754
		VACING,
		// Token: 0x0400261B RID: 9755
		CAPTURETANKS,
		// Token: 0x0400261C RID: 9756
		ENERGY,
		// Token: 0x0400261D RID: 9757
		CORRALLING,
		// Token: 0x0400261E RID: 9758
		FEEDING,
		// Token: 0x0400261F RID: 9759
		PLORTS,
		// Token: 0x04002620 RID: 9760
		SSBASICS,
		// Token: 0x04002621 RID: 9761
		GADGETMODE,
		// Token: 0x04002622 RID: 9762
		WILDS_TUTORIAL,
		// Token: 0x04002623 RID: 9763
		VALLEY_TUTORIAL,
		// Token: 0x04002624 RID: 9764
		SLIMULATIONS_TUTORIAL,
		// Token: 0x04002625 RID: 9765
		PINK_SLIME = 2000,
		// Token: 0x04002626 RID: 9766
		ROCK_SLIME,
		// Token: 0x04002627 RID: 9767
		PHOSPHOR_SLIME,
		// Token: 0x04002628 RID: 9768
		TABBY_SLIME,
		// Token: 0x04002629 RID: 9769
		RAD_SLIME,
		// Token: 0x0400262A RID: 9770
		BOOM_SLIME,
		// Token: 0x0400262B RID: 9771
		HUNTER_SLIME,
		// Token: 0x0400262C RID: 9772
		HONEY_SLIME,
		// Token: 0x0400262D RID: 9773
		PUDDLE_SLIME,
		// Token: 0x0400262E RID: 9774
		CRYSTAL_SLIME,
		// Token: 0x0400262F RID: 9775
		TARR_SLIME = 2900,
		// Token: 0x04002630 RID: 9776
		GOLD_SLIME,
		// Token: 0x04002631 RID: 9777
		LUCKY_SLIME,
		// Token: 0x04002632 RID: 9778
		LARGO_SLIME = 2980,
		// Token: 0x04002633 RID: 9779
		GORDO_SLIME,
		// Token: 0x04002634 RID: 9780
		FERAL_SLIME,
		// Token: 0x04002635 RID: 9781
		QUANTUM_SLIME,
		// Token: 0x04002636 RID: 9782
		FIRE_SLIME,
		// Token: 0x04002637 RID: 9783
		MOSAIC_SLIME,
		// Token: 0x04002638 RID: 9784
		DERVISH_SLIME,
		// Token: 0x04002639 RID: 9785
		TANGLE_SLIME,
		// Token: 0x0400263A RID: 9786
		SABER_SLIME,
		// Token: 0x0400263B RID: 9787
		QUICKSILVER_SLIME,
		// Token: 0x0400263C RID: 9788
		PARTY_GORDO_SLIME,
		// Token: 0x0400263D RID: 9789
		ECHO_NOTE_GORDO_SLIME,
		// Token: 0x0400263E RID: 9790
		GLITCH_SLIME,
		// Token: 0x0400263F RID: 9791
		CARROT = 3000,
		// Token: 0x04002640 RID: 9792
		OCAOCA,
		// Token: 0x04002641 RID: 9793
		BEET,
		// Token: 0x04002642 RID: 9794
		PARSNIP,
		// Token: 0x04002643 RID: 9795
		POGO,
		// Token: 0x04002644 RID: 9796
		MANGO,
		// Token: 0x04002645 RID: 9797
		CUBERRY,
		// Token: 0x04002646 RID: 9798
		PEAR,
		// Token: 0x04002647 RID: 9799
		HENHEN,
		// Token: 0x04002648 RID: 9800
		BRIAR_HEN,
		// Token: 0x04002649 RID: 9801
		STONY_HEN,
		// Token: 0x0400264A RID: 9802
		ROOSTRO,
		// Token: 0x0400264B RID: 9803
		CHICKADOO,
		// Token: 0x0400264C RID: 9804
		BRIAR_CHICKADOO,
		// Token: 0x0400264D RID: 9805
		STONY_CHICKADOO,
		// Token: 0x0400264E RID: 9806
		ELDER_HEN,
		// Token: 0x0400264F RID: 9807
		ELDER_ROOSTRO,
		// Token: 0x04002650 RID: 9808
		ONION,
		// Token: 0x04002651 RID: 9809
		LEMON,
		// Token: 0x04002652 RID: 9810
		PAINTED_HEN,
		// Token: 0x04002653 RID: 9811
		PAINTED_CHICKADOO,
		// Token: 0x04002654 RID: 9812
		GINGER,
		// Token: 0x04002655 RID: 9813
		KOOKADOBA,
		// Token: 0x04002656 RID: 9814
		SPICY_TOFU,
		// Token: 0x04002657 RID: 9815
		PRIMORDY_OIL_CRAFT = 3900,
		// Token: 0x04002658 RID: 9816
		DEEP_BRINE_CRAFT,
		// Token: 0x04002659 RID: 9817
		SPIRAL_STEAM_CRAFT,
		// Token: 0x0400265A RID: 9818
		LAVA_DUST_CRAFT,
		// Token: 0x0400265B RID: 9819
		BUZZ_WAX_CRAFT,
		// Token: 0x0400265C RID: 9820
		WILD_HONEY_CRAFT,
		// Token: 0x0400265D RID: 9821
		HEXACOMB_CRAFT,
		// Token: 0x0400265E RID: 9822
		ROYAL_JELLY_CRAFT,
		// Token: 0x0400265F RID: 9823
		JELLYSTONE_CRAFT,
		// Token: 0x04002660 RID: 9824
		INDIGONIUM_CRAFT,
		// Token: 0x04002661 RID: 9825
		SLIME_FOSSIL_CRAFT,
		// Token: 0x04002662 RID: 9826
		STRANGE_DIAMOND_CRAFT,
		// Token: 0x04002663 RID: 9827
		ECHOES,
		// Token: 0x04002664 RID: 9828
		SLIME_TOYS,
		// Token: 0x04002665 RID: 9829
		SILKY_SAND_CRAFT,
		// Token: 0x04002666 RID: 9830
		PEPPER_JAM_CRAFT,
		// Token: 0x04002667 RID: 9831
		GLASS_SHARD_CRAFT,
		// Token: 0x04002668 RID: 9832
		ECHO_NOTES,
		// Token: 0x04002669 RID: 9833
		MANIFOLD_CUBE_CRAFT,
		// Token: 0x0400266A RID: 9834
		ORNAMENTS,
		// Token: 0x0400266B RID: 9835
		CORRAL = 4000,
		// Token: 0x0400266C RID: 9836
		COOP,
		// Token: 0x0400266D RID: 9837
		GARDEN,
		// Token: 0x0400266E RID: 9838
		SILO,
		// Token: 0x0400266F RID: 9839
		INCINERATOR,
		// Token: 0x04002670 RID: 9840
		POND,
		// Token: 0x04002671 RID: 9841
		PLORT_MARKET,
		// Token: 0x04002672 RID: 9842
		OVERGROWTH,
		// Token: 0x04002673 RID: 9843
		GROTTO,
		// Token: 0x04002674 RID: 9844
		LAB,
		// Token: 0x04002675 RID: 9845
		CHROMA,
		// Token: 0x04002676 RID: 9846
		PARTNER,
		// Token: 0x04002677 RID: 9847
		DOCKS,
		// Token: 0x04002678 RID: 9848
		OGDEN_RETREAT,
		// Token: 0x04002679 RID: 9849
		MOCHI_MANOR,
		// Token: 0x0400267A RID: 9850
		VALLEY,
		// Token: 0x0400267B RID: 9851
		VIKTOR_LAB,
		// Token: 0x0400267C RID: 9852
		REEF = 5000,
		// Token: 0x0400267D RID: 9853
		QUARRY,
		// Token: 0x0400267E RID: 9854
		MOSS,
		// Token: 0x0400267F RID: 9855
		DESERT,
		// Token: 0x04002680 RID: 9856
		SEA,
		// Token: 0x04002681 RID: 9857
		THE_RANCH,
		// Token: 0x04002682 RID: 9858
		KEYS,
		// Token: 0x04002683 RID: 9859
		EXTRACTORS,
		// Token: 0x04002684 RID: 9860
		UTILITIES,
		// Token: 0x04002685 RID: 9861
		WARP_TECH,
		// Token: 0x04002686 RID: 9862
		DECORATIONS,
		// Token: 0x04002687 RID: 9863
		CURIOS,
		// Token: 0x04002688 RID: 9864
		REFINERY,
		// Token: 0x04002689 RID: 9865
		FABRICATOR,
		// Token: 0x0400268A RID: 9866
		BLUEPRINTS,
		// Token: 0x0400268B RID: 9867
		RUINS,
		// Token: 0x0400268C RID: 9868
		WILDS,
		// Token: 0x0400268D RID: 9869
		SLIMULATIONS_WORLD,
		// Token: 0x0400268E RID: 9870
		DRONES = 6010,
		// Token: 0x0400268F RID: 9871
		LOCKED = 10000
	}

	// Token: 0x02000743 RID: 1859
	[Serializable]
	public class IdEntry
	{
		// Token: 0x04002690 RID: 9872
		public PediaDirector.Id id;

		// Token: 0x04002691 RID: 9873
		public Sprite icon;
	}

	// Token: 0x02000744 RID: 1860
	[Serializable]
	public class IdentMapEntry
	{
		// Token: 0x04002692 RID: 9874
		public Identifiable.Id identId;

		// Token: 0x04002693 RID: 9875
		public PediaDirector.Id pediaId;
	}

	// Token: 0x02000745 RID: 1861
	private class PediaPopupCreator : PopupDirector.PopupCreator
	{
		// Token: 0x060026E7 RID: 9959 RVA: 0x00094270 File Offset: 0x00092470
		public PediaPopupCreator(PediaDirector pediaDir, PediaDirector.Id id)
		{
			this.pediaDir = pediaDir;
			this.id = id;
		}

		// Token: 0x060026E8 RID: 9960 RVA: 0x00094286 File Offset: 0x00092486
		public override void Create()
		{
			this.pediaDir.pediaModel.Unlock(new PediaDirector.Id[]
			{
				this.id
			});
			PediaPopupUI.CreatePediaPopup(this.pediaDir.Get(this.id));
		}

		// Token: 0x060026E9 RID: 9961 RVA: 0x000942BE File Offset: 0x000924BE
		public override bool Equals(object other)
		{
			return other is PediaDirector.PediaPopupCreator && ((PediaDirector.PediaPopupCreator)other).id == this.id;
		}

		// Token: 0x060026EA RID: 9962 RVA: 0x000942DD File Offset: 0x000924DD
		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		// Token: 0x060026EB RID: 9963 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		public override bool ShouldClear()
		{
			return false;
		}

		// Token: 0x04002694 RID: 9876
		private PediaDirector pediaDir;

		// Token: 0x04002695 RID: 9877
		private PediaDirector.Id id;
	}
}
