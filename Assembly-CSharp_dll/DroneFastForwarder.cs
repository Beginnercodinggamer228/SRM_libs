using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000131 RID: 305
public static class DroneFastForwarder
{
	// Token: 0x0600069F RID: 1695 RVA: 0x000232F4 File Offset: 0x000214F4
	public static void FastForward_Pre(RanchCellFastForwarder source)
	{
		if (!DroneFastForwarder.ACTIVE.Add(source.gameObject))
		{
			return;
		}
		SRSingleton<PopupElementsUI>.Instance.RegisterBlocker(source.gameObject);
		if (DroneFastForwarder.ACTIVE.Count == 1)
		{
			DroneFastForwarder.GatherGroup.BLACKLIST.Clear();
			PlayerState playerState = SRSingleton<SceneContext>.Instance.PlayerState;
			playerState.SetCurrencyDisplay(new int?(playerState.GetCurrency()));
			DroneFastForwarder.coinsPopup = 0;
		}
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x0002335C File Offset: 0x0002155C
	public static void FastForward_Post(RanchCellFastForwarder source)
	{
		if (!DroneFastForwarder.ACTIVE.Remove(source.gameObject))
		{
			return;
		}
		SRSingleton<PopupElementsUI>.Instance.DeregisterBlocker(source.gameObject);
		if (DroneFastForwarder.ACTIVE.Count == 0)
		{
			DroneFastForwarder.GatherGroup.BLACKLIST.Clear();
			SRSingleton<PopupElementsUI>.Instance.CreateCoinsPopup(DroneFastForwarder.coinsPopup, PlayerState.CoinsType.DRONE);
			SRSingleton<SceneContext>.Instance.PlayerState.SetCurrencyDisplay(null);
		}
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x000233CC File Offset: 0x000215CC
	public static void FastForward(Drone drone, double startTime, double endTime)
	{
		if (drone.station.battery.Time <= startTime)
		{
			return;
		}
		if (drone.plexer.Programs.Count == 0)
		{
			drone.ForceResting(false);
			return;
		}
		double num = Math.Min(drone.station.battery.Time, endTime) - startTime;
		if (drone.ammo.Any())
		{
			DroneFastForwarder.GatherGroup group = new DroneFastForwarder.GatherGroup.Ammo(drone.ammo);
			DroneSubbehaviourPlexer.Program program = drone.plexer.Programs[0];
			while (DroneFastForwarder.FastForward_Deposit(drone, program, group, endTime, ref num))
			{
			}
		}
		if (drone.plexer.Programs.Count > 1)
		{
			Queue<int> queue = new Queue<int>(Enumerable.Range(0, drone.plexer.Programs.Count));
			while (queue.Any<int>())
			{
				int num2 = queue.Dequeue();
				DroneSubbehaviourPlexer.Program program2 = drone.plexer.Programs[num2];
				if (DroneFastForwarder.FastForward_GatherDeposit_Advanced(drone, program2, endTime, ref num))
				{
					queue.Enqueue(num2);
				}
			}
		}
		else
		{
			DroneFastForwarder.FastForward_GatherDeposit_Basic(drone, endTime, ref num);
		}
		if (drone.station.battery.Time <= endTime)
		{
			if (drone.ammo.Any())
			{
				num = double.MaxValue;
				DroneFastForwarder.GatherGroup group2 = new DroneFastForwarder.GatherGroup.Ammo(drone.ammo);
				foreach (DroneSubbehaviourPlexer.Program program3 in drone.plexer.Programs)
				{
					while (DroneFastForwarder.FastForward_Deposit(drone, program3, group2, endTime, ref num))
					{
					}
				}
			}
			drone.ForceResting(false);
		}
		drone.plexer.OnFastForward();
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x0002356C File Offset: 0x0002176C
	private static void FastForward_GatherDeposit_Basic(Drone drone, double endTime, ref double remainingTime)
	{
		DroneSubbehaviourPlexer.Program program = drone.plexer.Programs[0];
		DroneFastForwarder.FastForward_GatherDeposit(DroneFastForwarder.GatherDepositMode.MULTIPLE, drone, program, endTime, ref remainingTime);
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00023596 File Offset: 0x00021796
	private static bool FastForward_GatherDeposit_Advanced(Drone drone, DroneSubbehaviourPlexer.Program program, double endTime, ref double remainingTime)
	{
		return DroneFastForwarder.FastForward_GatherDeposit(DroneFastForwarder.GatherDepositMode.SINGLE, drone, program, endTime, ref remainingTime) >= 1;
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000235A8 File Offset: 0x000217A8
	private static int FastForward_GatherDeposit(DroneFastForwarder.GatherDepositMode mode, Drone drone, DroneSubbehaviourPlexer.Program program, double endTime, ref double remainingTime)
	{
		int num = 0;
		if (remainingTime >= 7200.0)
		{
			DroneFastForwarder.GATHER_GROUPS.AddRange(program.Sources.SelectMany((DroneProgramSource s) => s.GetFastForwardGroups(endTime)));
			while (DroneFastForwarder.GATHER_GROUPS.Any<DroneFastForwarder.GatherGroup>() && remainingTime >= 7200.0)
			{
				int @int = Randoms.SHARED.GetInt(DroneFastForwarder.GATHER_GROUPS.Count);
				DroneFastForwarder.GatherGroup gatherGroup = DroneFastForwarder.GATHER_GROUPS[@int];
				bool flag = DroneFastForwarder.FastForward_Deposit(drone, program, gatherGroup, endTime, ref remainingTime);
				if (flag)
				{
					remainingTime -= 3600.0;
					num++;
					if (mode == DroneFastForwarder.GatherDepositMode.SINGLE)
					{
						break;
					}
				}
				if (!flag || gatherGroup.None())
				{
					DroneFastForwarder.GATHER_GROUPS.RemoveAt(@int);
					gatherGroup.Dispose();
				}
			}
			DroneFastForwarder.GATHER_GROUPS.ForEach(delegate(DroneFastForwarder.GatherGroup g)
			{
				g.Dispose();
			});
			DroneFastForwarder.GATHER_GROUPS.Clear();
		}
		return num;
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000236B4 File Offset: 0x000218B4
	private static bool FastForward_Deposit(Drone drone, DroneSubbehaviourPlexer.Program program, DroneFastForwarder.GatherGroup group, double endTime, ref double remainingTime)
	{
		if (group.None() || remainingTime < 3600.0)
		{
			return false;
		}
		DroneProgramDestination droneProgramDestination = Randoms.SHARED.Pick<DroneProgramDestination>(from d in program.Destinations
		where d.HasAvailableSpace(@group.id)
		select d, null);
		if (droneProgramDestination == null)
		{
			return false;
		}
		DroneProgramDestination.FastForward_Response fastForward_Response = droneProgramDestination.FastForward(group.id, group.overflow, endTime, Mathf.Min(50, group.count));
		DroneFastForwarder.coinsPopup += fastForward_Response.currency;
		if (fastForward_Response.deposits <= 0)
		{
			return false;
		}
		group.Decrement(fastForward_Response.deposits);
		remainingTime -= 3600.0;
		return true;
	}

	// Token: 0x0400062F RID: 1583
	private const double GATHER_TIME = 3600.0;

	// Token: 0x04000630 RID: 1584
	private const double DEPOSIT_TIME = 3600.0;

	// Token: 0x04000631 RID: 1585
	private const double CYCLE_TIME = 7200.0;

	// Token: 0x04000632 RID: 1586
	private static List<DroneFastForwarder.GatherGroup> GATHER_GROUPS = new List<DroneFastForwarder.GatherGroup>();

	// Token: 0x04000633 RID: 1587
	private static HashSet<GameObject> ACTIVE = new HashSet<GameObject>();

	// Token: 0x04000634 RID: 1588
	private static int coinsPopup;

	// Token: 0x02000132 RID: 306
	private enum GatherDepositMode
	{
		// Token: 0x04000636 RID: 1590
		SINGLE,
		// Token: 0x04000637 RID: 1591
		MULTIPLE
	}

	// Token: 0x02000133 RID: 307
	public abstract class GatherGroup : IDisposable
	{
		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060006A7 RID: 1703
		public abstract Identifiable.Id id { get; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x060006A8 RID: 1704
		public abstract int count { get; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x060006A9 RID: 1705
		public abstract bool overflow { get; }

		// Token: 0x060006AA RID: 1706
		public abstract void Decrement(int decrement);

		// Token: 0x060006AB RID: 1707
		public abstract void Dispose();

		// Token: 0x060006AC RID: 1708 RVA: 0x0002379D File Offset: 0x0002199D
		public bool Any()
		{
			return this.count > 0;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x000237A8 File Offset: 0x000219A8
		public bool None()
		{
			return this.count <= 0;
		}

		// Token: 0x04000638 RID: 1592
		public static HashSet<GameObject> BLACKLIST = new HashSet<GameObject>();

		// Token: 0x02000134 RID: 308
		public class Dynamic : DroneFastForwarder.GatherGroup
		{
			// Token: 0x170000E7 RID: 231
			// (get) Token: 0x060006B0 RID: 1712 RVA: 0x000237C2 File Offset: 0x000219C2
			public override Identifiable.Id id
			{
				get
				{
					return this.sources.First<Identifiable>().id;
				}
			}

			// Token: 0x170000E8 RID: 232
			// (get) Token: 0x060006B1 RID: 1713 RVA: 0x000237D4 File Offset: 0x000219D4
			public override int count
			{
				get
				{
					return this.sources.Count;
				}
			}

			// Token: 0x170000E9 RID: 233
			// (get) Token: 0x060006B2 RID: 1714 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool overflow
			{
				get
				{
					return false;
				}
			}

			// Token: 0x060006B3 RID: 1715 RVA: 0x000237E1 File Offset: 0x000219E1
			public Dynamic(IEnumerable<Identifiable> sources)
			{
				this.sources = (from i in sources
				where DroneFastForwarder.GatherGroup.BLACKLIST.Add(i.gameObject)
				select i).ToList<Identifiable>();
			}

			// Token: 0x060006B4 RID: 1716 RVA: 0x0002381C File Offset: 0x00021A1C
			public override void Decrement(int decrement)
			{
				for (int i = 0; i < decrement; i++)
				{
					Destroyer.DestroyActor(Randoms.SHARED.Pluck<Identifiable>(this.sources, null).gameObject, "GatherGroup.Dynamic.Decrement", false);
				}
			}

			// Token: 0x060006B5 RID: 1717 RVA: 0x00023856 File Offset: 0x00021A56
			public override void Dispose()
			{
				DroneFastForwarder.GatherGroup.BLACKLIST.ExceptWith(from i in this.sources
				select i.gameObject);
			}

			// Token: 0x04000639 RID: 1593
			private List<Identifiable> sources;
		}

		// Token: 0x02000136 RID: 310
		public class Storage : DroneFastForwarder.GatherGroup
		{
			// Token: 0x170000EA RID: 234
			// (get) Token: 0x060006BA RID: 1722 RVA: 0x000238B2 File Offset: 0x00021AB2
			public override Identifiable.Id id
			{
				get
				{
					return this.storage.id;
				}
			}

			// Token: 0x170000EB RID: 235
			// (get) Token: 0x060006BB RID: 1723 RVA: 0x000238BF File Offset: 0x00021ABF
			public override int count
			{
				get
				{
					return this.storage.count;
				}
			}

			// Token: 0x170000EC RID: 236
			// (get) Token: 0x060006BC RID: 1724 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
			public override bool overflow
			{
				get
				{
					return false;
				}
			}

			// Token: 0x060006BD RID: 1725 RVA: 0x000238CC File Offset: 0x00021ACC
			public Storage(DroneNetwork.StorageMetadata storage)
			{
				this.storage = storage;
			}

			// Token: 0x060006BE RID: 1726 RVA: 0x000238DB File Offset: 0x00021ADB
			public override void Decrement(int decrement)
			{
				this.storage.Decrement(decrement);
			}

			// Token: 0x060006BF RID: 1727 RVA: 0x00003296 File Offset: 0x00001496
			public override void Dispose()
			{
			}

			// Token: 0x0400063D RID: 1597
			private DroneNetwork.StorageMetadata storage;
		}

		// Token: 0x02000137 RID: 311
		public class Ammo : DroneFastForwarder.GatherGroup
		{
			// Token: 0x170000ED RID: 237
			// (get) Token: 0x060006C0 RID: 1728 RVA: 0x000238E9 File Offset: 0x00021AE9
			public override Identifiable.Id id
			{
				get
				{
					return this.ammo.GetSlotName();
				}
			}

			// Token: 0x170000EE RID: 238
			// (get) Token: 0x060006C1 RID: 1729 RVA: 0x000238F6 File Offset: 0x00021AF6
			public override int count
			{
				get
				{
					return this.ammo.GetSlotCount();
				}
			}

			// Token: 0x170000EF RID: 239
			// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00013CC5 File Offset: 0x00011EC5
			public override bool overflow
			{
				get
				{
					return true;
				}
			}

			// Token: 0x060006C3 RID: 1731 RVA: 0x00023903 File Offset: 0x00021B03
			public Ammo(DroneAmmo ammo)
			{
				this.ammo = ammo;
			}

			// Token: 0x060006C4 RID: 1732 RVA: 0x00023912 File Offset: 0x00021B12
			public override void Decrement(int decrement)
			{
				this.ammo.Decrement(0, decrement);
			}

			// Token: 0x060006C5 RID: 1733 RVA: 0x00003296 File Offset: 0x00001496
			public override void Dispose()
			{
			}

			// Token: 0x0400063E RID: 1598
			private DroneAmmo ammo;
		}
	}
}
