using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class DroneProgramDestinationPlortMarket : DroneProgramDestination<ScorePlort>
{
	// Token: 0x06000801 RID: 2049 RVA: 0x00026B92 File Offset: 0x00024D92
	public override void Awake()
	{
		base.Awake();
		this.economyDirector = SRSingleton<SceneContext>.Instance.EconomyDirector;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x00026BAA File Offset: 0x00024DAA
	public override void Deselected()
	{
		base.Deselected();
		if (this.orientation != null)
		{
			this.orientation.Dispose();
			this.orientation = null;
		}
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x00026BCC File Offset: 0x00024DCC
	public override int GetAvailableSpace(Identifiable.Id id)
	{
		if (!this.GetDestinations(id, false).Any<ScorePlort>())
		{
			return 0;
		}
		return this.MAX_AVAIL_TO_REPORT;
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x00026BE8 File Offset: 0x00024DE8
	protected override IEnumerable<ScorePlort> GetDestinations(Identifiable.Id id, bool overflow)
	{
		return from s in this.drone.network.Markets
		where s.CanDeposit(id, true)
		select s;
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x00026C23 File Offset: 0x00024E23
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations()
	{
		this.orientation = base.GetTargetOrientation_Catcher(this.destination.gameObject);
		yield return this.orientation.orientation;
		yield break;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x00026C33 File Offset: 0x00024E33
	protected override Vector3 GetTargetPosition()
	{
		return this.destination.transform.position;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x00026C48 File Offset: 0x00024E48
	protected override bool OnAction_Deposit(bool overflow)
	{
		if (!this.timeDirector.HasReached(this.time) || this.economyDirector.IsMarketShutdown())
		{
			return false;
		}
		if (this.destination.Deposit(this.drone.ammo.GetSlotName(), 1, new PlayerState.CoinsType?(PlayerState.CoinsType.DRONE), false))
		{
			this.time = this.timeDirector.HoursFromNow(0.0016666668f);
			this.drone.ammo.Pop();
			return false;
		}
		return true;
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x00026CCC File Offset: 0x00024ECC
	public override DroneProgramDestination.FastForward_Response FastForward(Identifiable.Id id, bool overflow, double endTime, int maxFastForward)
	{
		ScorePlort.Deposit_Response deposit_Response = this.Prioritize(this.GetDestinations(id, overflow)).First<ScorePlort>().Deposit(id, maxFastForward, new PlayerState.CoinsType?(PlayerState.CoinsType.NONE), true);
		return new DroneProgramDestination.FastForward_Response
		{
			deposits = deposit_Response.deposits,
			currency = deposit_Response.currency
		};
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x00026D19 File Offset: 0x00024F19
	protected override IEnumerable<ScorePlort> Prioritize(IEnumerable<ScorePlort> destinations)
	{
		return destinations.OrderBy((ScorePlort d) => d, from m in new DroneProgramDestinationPlortMarket.Comparer()
		orderby (m.transform.position - this.drone.transform.position).sqrMagnitude
		select m);
	}

	// Token: 0x0400072A RID: 1834
	private double time;

	// Token: 0x0400072B RID: 1835
	private EconomyDirector economyDirector;

	// Token: 0x0400072C RID: 1836
	private DroneSubbehaviour.CatcherOrientation orientation;

	// Token: 0x02000178 RID: 376
	private class Comparer : SRComparer<ScorePlort>
	{
	}
}
