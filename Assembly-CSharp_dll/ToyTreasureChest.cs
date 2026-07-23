using System;
using UnityEngine;

// Token: 0x02000533 RID: 1331
[RequireComponent(typeof(Vacuumable))]
public class ToyTreasureChest : MonoBehaviour
{
	// Token: 0x06001BB1 RID: 7089 RVA: 0x00069E43 File Offset: 0x00068043
	public void Awake()
	{
		this.vacuumable = base.GetComponent<Vacuumable>();
	}

	// Token: 0x06001BB2 RID: 7090 RVA: 0x00069E51 File Offset: 0x00068051
	public void Update()
	{
		this.chestLid.isKinematic = this.vacuumable.isHeld();
	}

	// Token: 0x04001AD4 RID: 6868
	[Tooltip("Rigidbody of the chest lid.")]
	public Rigidbody chestLid;

	// Token: 0x04001AD5 RID: 6869
	private Vacuumable vacuumable;
}
