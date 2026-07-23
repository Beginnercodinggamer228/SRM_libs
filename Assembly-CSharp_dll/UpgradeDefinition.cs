using System;
using UnityEngine;

// Token: 0x020002DC RID: 732
[CreateAssetMenu(menuName = "Player/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x0003DCFE File Offset: 0x0003BEFE
	public PlayerState.Upgrade Upgrade
	{
		get
		{
			return this.upgrade;
		}
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x0003DD06 File Offset: 0x0003BF06
	public Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x0003DD0E File Offset: 0x0003BF0E
	public int Cost
	{
		get
		{
			return this.cost;
		}
	}

	// Token: 0x04000E67 RID: 3687
	[SerializeField]
	private PlayerState.Upgrade upgrade;

	// Token: 0x04000E68 RID: 3688
	[SerializeField]
	private Sprite icon;

	// Token: 0x04000E69 RID: 3689
	[SerializeField]
	private int cost;
}
