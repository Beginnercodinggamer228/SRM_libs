using System;
using UnityEngine;

// Token: 0x0200052E RID: 1326
[CreateAssetMenu(menuName = "Toy/Toy Definition")]
public class ToyDefinition : ScriptableObject
{
	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06001B9A RID: 7066 RVA: 0x000699F3 File Offset: 0x00067BF3
	public Identifiable.Id ToyId
	{
		get
		{
			return this.toyId;
		}
	}

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001B9B RID: 7067 RVA: 0x000699FB File Offset: 0x00067BFB
	public Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06001B9C RID: 7068 RVA: 0x00069A03 File Offset: 0x00067C03
	public int Cost
	{
		get
		{
			return this.cost;
		}
	}

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x06001B9D RID: 7069 RVA: 0x00069A0B File Offset: 0x00067C0B
	public string NameKey
	{
		get
		{
			return this.nameKey;
		}
	}

	// Token: 0x04001AC5 RID: 6853
	[SerializeField]
	private Identifiable.Id toyId;

	// Token: 0x04001AC6 RID: 6854
	[SerializeField]
	private Sprite icon;

	// Token: 0x04001AC7 RID: 6855
	[SerializeField]
	private int cost;

	// Token: 0x04001AC8 RID: 6856
	[SerializeField]
	private string nameKey;
}
