using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DLCPackage;

// Token: 0x02000108 RID: 264
public abstract class DLCProvider
{
	// Token: 0x060005D7 RID: 1495 RVA: 0x00021C30 File Offset: 0x0001FE30
	public DLCProvider(IEnumerable<Id> supported)
	{
		this.supported = new HashSet<Id>(supported, IdComparer.Instance);
		this.cache = this.supported.ToDictionary((Id id) => id, (Id id) => State.AVAILABLE, IdComparer.Instance);
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00021CA8 File Offset: 0x0001FEA8
	protected void ResetAllPackageStates()
	{
		this.cache = this.supported.ToDictionary((Id id) => id, (Id id) => State.AVAILABLE, IdComparer.Instance);
	}

	// Token: 0x060005D9 RID: 1497
	public abstract IEnumerator Refresh();

	// Token: 0x060005DA RID: 1498
	public abstract void ShowInStore(Id id);

	// Token: 0x060005DB RID: 1499 RVA: 0x00021D09 File Offset: 0x0001FF09
	public IEnumerable<Id> GetSupported()
	{
		return this.supported;
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00021D14 File Offset: 0x0001FF14
	public State GetState(Id id)
	{
		State result;
		this.cache.TryGetValue(id, out result);
		return result;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00021D34 File Offset: 0x0001FF34
	protected bool SetState(Id id, State state)
	{
		State state2 = this.GetState(id);
		if (state2 > state)
		{
			Log.Error("Attempting to downgrade DLC state.", new object[]
			{
				"id",
				id,
				"current",
				state2,
				"updated",
				state
			});
			return false;
		}
		this.cache[id] = state;
		return true;
	}

	// Token: 0x040005AD RID: 1453
	private HashSet<Id> supported;

	// Token: 0x040005AE RID: 1454
	private Dictionary<Id, State> cache;
}
