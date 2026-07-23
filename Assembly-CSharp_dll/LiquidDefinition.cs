using System;
using UnityEngine;

// Token: 0x0200072A RID: 1834
[CreateAssetMenu(menuName = "World/Liquid Definition")]
public class LiquidDefinition : ScriptableObject
{
	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06002646 RID: 9798 RVA: 0x000927FB File Offset: 0x000909FB
	public Identifiable.Id Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06002647 RID: 9799 RVA: 0x00092803 File Offset: 0x00090A03
	public GameObject InFx
	{
		get
		{
			return this.inFX;
		}
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x06002648 RID: 9800 RVA: 0x0009280B File Offset: 0x00090A0B
	public GameObject VacFailFx
	{
		get
		{
			return this.vacFailFX;
		}
	}

	// Token: 0x04002596 RID: 9622
	[SerializeField]
	private Identifiable.Id id;

	// Token: 0x04002597 RID: 9623
	[SerializeField]
	private GameObject inFX;

	// Token: 0x04002598 RID: 9624
	[SerializeField]
	private GameObject vacFailFX;
}
