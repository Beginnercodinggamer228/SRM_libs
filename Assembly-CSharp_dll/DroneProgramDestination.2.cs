using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000164 RID: 356
public abstract class DroneProgramDestination<T> : DroneProgramDestination where T : class
{
	// Token: 0x060007A3 RID: 1955 RVA: 0x00025EEC File Offset: 0x000240EC
	public sealed override bool Relevancy(bool overflow)
	{
		if (this.drone.ammo.IsEmpty())
		{
			return false;
		}
		Identifiable.Id slotName = this.drone.ammo.GetSlotName();
		if (!this.predicate(slotName))
		{
			return false;
		}
		this.destination = this.Prioritize(this.GetDestinations(slotName, overflow)).FirstOrDefault<T>();
		return !DroneProgramDestination<T>.IsNull(this.destination);
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00025F55 File Offset: 0x00024155
	public override void Selected()
	{
		base.Selected();
		this.overflow = false;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x00025F64 File Offset: 0x00024164
	protected sealed override bool OnAction()
	{
		if (!this.OnAction_Deposit(this.overflow))
		{
			return false;
		}
		if (this.drone.ammo.IsEmpty())
		{
			return true;
		}
		if (this.overflow)
		{
			Log.Error("Failed to complete overflow deposit.", new object[]
			{
				"destination",
				this.destination
			});
			return true;
		}
		if (this.GetDestinations(this.drone.ammo.GetSlotName(), false).Any((T d) => d != this.destination))
		{
			return true;
		}
		this.overflow = true;
		return false;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x00025FF9 File Offset: 0x000241F9
	protected override bool CanCancel()
	{
		return this.drone.ammo.IsEmpty() || DroneProgramDestination<T>.IsNull(this.destination);
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002601A File Offset: 0x0002421A
	private static bool IsNull(T destination)
	{
		return destination == null || destination.Equals(null);
	}

	// Token: 0x060007A8 RID: 1960
	protected abstract bool OnAction_Deposit(bool overflow);

	// Token: 0x060007A9 RID: 1961
	protected abstract IEnumerable<T> GetDestinations(Identifiable.Id id, bool overflow);

	// Token: 0x060007AA RID: 1962
	protected abstract IEnumerable<T> Prioritize(IEnumerable<T> destinations);

	// Token: 0x040006FA RID: 1786
	protected int MAX_AVAIL_TO_REPORT = 1000000;

	// Token: 0x040006FB RID: 1787
	protected T destination;

	// Token: 0x040006FC RID: 1788
	private bool overflow;
}
