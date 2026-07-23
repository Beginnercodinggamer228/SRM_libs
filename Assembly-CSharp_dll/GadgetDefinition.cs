using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200070D RID: 1805
[CreateAssetMenu(menuName = "Gadget/Gadget Definition")]
public class GadgetDefinition : ScriptableObject
{
	// Token: 0x060025AD RID: 9645 RVA: 0x00090A18 File Offset: 0x0008EC18
	public List<Gadget.Id> GetGadgetsToCountIds()
	{
		return new List<Gadget.Id>(this.countOtherIds)
		{
			this.id
		};
	}

	// Token: 0x04002523 RID: 9507
	public Gadget.Id id;

	// Token: 0x04002524 RID: 9508
	public GameObject prefab;

	// Token: 0x04002525 RID: 9509
	public Sprite icon;

	// Token: 0x04002526 RID: 9510
	public PediaDirector.Id pediaLink;

	// Token: 0x04002527 RID: 9511
	public int blueprintCost;

	// Token: 0x04002528 RID: 9512
	public GadgetDefinition.CraftCost[] craftCosts;

	// Token: 0x04002529 RID: 9513
	[Tooltip("Limits at buy time the number we can ever have.")]
	public int buyCountLimit;

	// Token: 0x0400252A RID: 9514
	[Tooltip("Limits at place time the number that can exist in the world.")]
	public int countLimit;

	// Token: 0x0400252B RID: 9515
	[Tooltip("Include these other IDs in counting at placement time.")]
	public Gadget.Id[] countOtherIds;

	// Token: 0x0400252C RID: 9516
	[Tooltip("Destroy the gadget instead of picking it up.")]
	public bool destroyOnRemoval;

	// Token: 0x0400252D RID: 9517
	[Tooltip("Do we have to buy these two at a time?")]
	public bool buyInPairs;

	// Token: 0x0200070E RID: 1806
	[Serializable]
	public class CraftCost
	{
		// Token: 0x0400252E RID: 9518
		public Identifiable.Id id;

		// Token: 0x0400252F RID: 9519
		public int amount;
	}
}
