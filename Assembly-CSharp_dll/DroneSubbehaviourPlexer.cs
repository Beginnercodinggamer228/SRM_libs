using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001BA RID: 442
[RequireComponent(typeof(Drone))]
[RequireComponent(typeof(DroneSubbehaviourDumpAmmo))]
[RequireComponent(typeof(DroneSubbehaviourIdle))]
[RequireComponent(typeof(DroneSubbehaviourRest))]
public class DroneSubbehaviourPlexer : RegisteredActorBehaviour, RegistryFixedUpdateable
{
	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000959 RID: 2393 RVA: 0x00029DA4 File Offset: 0x00027FA4
	// (remove) Token: 0x0600095A RID: 2394 RVA: 0x00029DDC File Offset: 0x00027FDC
	public event DroneSubbehaviourPlexer.OnSubbehaviourSelected onSubbehaviourSelected;

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x0600095B RID: 2395 RVA: 0x00029E11 File Offset: 0x00028011
	public List<DroneSubbehaviourPlexer.Program> Programs
	{
		get
		{
			return this.subbehaviourPrograms;
		}
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x00029E1C File Offset: 0x0002801C
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.subbehaviourPrograms = new List<DroneSubbehaviourPlexer.Program>();
		this.subbehaviourIdle = base.GetRequiredComponent<DroneSubbehaviourIdle>();
		this.subbehaviourDumpAmmo = base.GetRequiredComponent<DroneSubbehaviourDumpAmmo>();
		this.subbehaviourRest = base.GetRequiredComponent<DroneSubbehaviourRest>();
		this.gadget = base.GetRequiredComponentInParent<DroneGadget>(false);
		this.gadget.onProgramsChanged += this.OnGadgetProgramsChanged;
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x00029E8C File Offset: 0x0002808C
	public override void Start()
	{
		base.Start();
		this.activationTime = Time.fixedTime + 3f;
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x00029EA5 File Offset: 0x000280A5
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.ForceRethink(0f);
		if (this.gadget != null)
		{
			this.gadget.onProgramsChanged -= this.OnGadgetProgramsChanged;
			this.gadget = null;
		}
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00029EE4 File Offset: 0x000280E4
	private void OnGadgetProgramsChanged(DroneMetadata.Program[] programs)
	{
		if (this.currBehaviour is DroneProgramSource || this.currBehaviour is DroneProgramDestination)
		{
			this.ForceRethink(0f);
		}
		this.subbehaviourPrograms.ForEach(delegate(DroneSubbehaviourPlexer.Program p)
		{
			p.Destroy();
		});
		this.subbehaviourPrograms.Clear();
		foreach (DroneMetadata.Program program in from p in programs
		where p.IsComplete()
		select p)
		{
			List<DroneProgramDestination> list = new List<DroneProgramDestination>();
			for (int i = 0; i < program.destination.types.Length; i++)
			{
				DroneProgramDestination droneProgramDestination = base.gameObject.AddComponent(program.destination.types[i]) as DroneProgramDestination;
				droneProgramDestination.predicate = program.target.predicate;
				list.Add(droneProgramDestination);
			}
			List<DroneProgramSource> list2 = new List<DroneProgramSource>();
			for (int j = 0; j < program.source.types.Length; j++)
			{
				DroneProgramSource droneProgramSource = base.gameObject.AddComponent(program.source.types[j]) as DroneProgramSource;
				droneProgramSource.predicate = program.target.predicate;
				droneProgramSource.destinations = list;
				list2.Add(droneProgramSource);
			}
			this.subbehaviourPrograms.Add(new DroneSubbehaviourPlexer.Program(list2, list));
		}
		DroneSubbehaviourRest droneSubbehaviourRest = this.currBehaviour as DroneSubbehaviourRest;
		if (droneSubbehaviourRest != null)
		{
			droneSubbehaviourRest.ForceRethink();
		}
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x0002A09C File Offset: 0x0002829C
	public void RegistryFixedUpdate()
	{
		if (Time.fixedTime < this.activationTime || this.timeDirector.IsFastForwarding() || this.gadget.drone.region.Hibernated)
		{
			return;
		}
		if (this.currBehaviour == null)
		{
			this.currBehaviour = this.PickNextBehaviour();
			this.currBehaviour.Selected();
			this.nextBehaviour = null;
			if (this.onSubbehaviourSelected != null)
			{
				this.onSubbehaviourSelected(this.currBehaviour);
			}
		}
		this.currBehaviour.Action();
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x0002A12B File Offset: 0x0002832B
	public void ForceRethink(float activationDelay = 0f)
	{
		this.activationTime = Time.fixedTime + activationDelay;
		if (this.currBehaviour != null)
		{
			this.currBehaviour.Deselected();
			this.currBehaviour = null;
		}
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x0002A15C File Offset: 0x0002835C
	public void ForceResting()
	{
		if (this.currBehaviour != this.subbehaviourRest)
		{
			this.nextBehaviour = this.subbehaviourRest;
			this.subbehaviourPrograms.ForEach(delegate(DroneSubbehaviourPlexer.Program p)
			{
				p.ResetProgram();
			});
			this.ForceRethink(0f);
		}
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x0002A1C0 File Offset: 0x000283C0
	public void ForceDumpAmmo(bool destructive)
	{
		this.subbehaviourDumpAmmo.destructive = destructive;
		if (this.currBehaviour != this.subbehaviourDumpAmmo)
		{
			this.nextBehaviour = this.subbehaviourDumpAmmo;
			this.subbehaviourPrograms.ForEach(delegate(DroneSubbehaviourPlexer.Program p)
			{
				p.ResetProgram();
			});
			this.ForceRethink(0f);
		}
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0002A234 File Offset: 0x00028434
	public bool IsResting()
	{
		return this.currBehaviour is DroneSubbehaviourRest;
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x0002A244 File Offset: 0x00028444
	public void OnFastForward()
	{
		this.subbehaviourPrograms.ForEach(delegate(DroneSubbehaviourPlexer.Program p)
		{
			p.ResetProgram();
		});
		this.ForceRethink(0f);
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x0002A27B File Offset: 0x0002847B
	public bool PickNextGatherBehaviour()
	{
		if (this.nextBehaviour == null)
		{
			this.nextBehaviour = this.PickNextProgramBehaviour();
			return this.nextBehaviour != null;
		}
		return false;
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0002A2A8 File Offset: 0x000284A8
	private DroneSubbehaviour PickNextBehaviour()
	{
		if (this.nextBehaviour != null)
		{
			return this.nextBehaviour;
		}
		if (this.subbehaviourIdle.Relevancy())
		{
			return this.subbehaviourIdle;
		}
		DroneSubbehaviour droneSubbehaviour = this.PickNextProgramBehaviour();
		if (droneSubbehaviour != null)
		{
			return droneSubbehaviour;
		}
		if (this.subbehaviourDumpAmmo.Relevancy())
		{
			return this.subbehaviourDumpAmmo;
		}
		if (this.subbehaviourRest.Relevancy())
		{
			return this.subbehaviourRest;
		}
		throw new Exception("Failed to get next drone subbehaviour.");
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0002A324 File Offset: 0x00028524
	private DroneSubbehaviour PickNextProgramBehaviour()
	{
		if (!this.subbehaviourPrograms.Any<DroneSubbehaviourPlexer.Program>())
		{
			return null;
		}
		int num = this.subbehaviourPrograms.Count;
		if (this.subbehaviourPrograms[0].state == DroneSubbehaviourPlexer.Program.State.DEPOSIT)
		{
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			DroneSubbehaviourPlexer.Program program = this.subbehaviourPrograms[0];
			DroneSubbehaviour droneSubbehaviour = program.PickNextBehaviour();
			if (droneSubbehaviour != null)
			{
				return droneSubbehaviour;
			}
			if (this.subbehaviourPrograms.Count > 1)
			{
				this.subbehaviourPrograms.RemoveAt(0);
				this.subbehaviourPrograms.Add(program);
			}
		}
		return null;
	}

	// Token: 0x040007DB RID: 2011
	private DroneSubbehaviourIdle subbehaviourIdle;

	// Token: 0x040007DC RID: 2012
	private List<DroneSubbehaviourPlexer.Program> subbehaviourPrograms;

	// Token: 0x040007DD RID: 2013
	private DroneSubbehaviourDumpAmmo subbehaviourDumpAmmo;

	// Token: 0x040007DE RID: 2014
	private DroneSubbehaviourRest subbehaviourRest;

	// Token: 0x040007DF RID: 2015
	private TimeDirector timeDirector;

	// Token: 0x040007E0 RID: 2016
	private DroneSubbehaviour currBehaviour;

	// Token: 0x040007E1 RID: 2017
	private DroneSubbehaviour nextBehaviour;

	// Token: 0x040007E2 RID: 2018
	private DroneGadget gadget;

	// Token: 0x040007E3 RID: 2019
	private float activationTime;

	// Token: 0x040007E4 RID: 2020
	private const float ACTIVATION_DELAY = 3f;

	// Token: 0x020001BB RID: 443
	// (Invoke) Token: 0x0600096B RID: 2411
	public delegate void OnSubbehaviourSelected(DroneSubbehaviour subbehaviour);

	// Token: 0x020001BC RID: 444
	public class Program
	{
		// Token: 0x17000129 RID: 297
		// (get) Token: 0x0600096E RID: 2414 RVA: 0x0002A3B6 File Offset: 0x000285B6
		public IEnumerable<DroneProgramSource> Sources
		{
			get
			{
				return this.sources;
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600096F RID: 2415 RVA: 0x0002A3BE File Offset: 0x000285BE
		public IEnumerable<DroneProgramDestination> Destinations
		{
			get
			{
				return this.destinations;
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000970 RID: 2416 RVA: 0x0002A3C6 File Offset: 0x000285C6
		// (set) Token: 0x06000971 RID: 2417 RVA: 0x0002A3CE File Offset: 0x000285CE
		public DroneSubbehaviourPlexer.Program.State state { get; private set; }

		// Token: 0x06000972 RID: 2418 RVA: 0x0002A3D7 File Offset: 0x000285D7
		public Program(List<DroneProgramSource> sources, List<DroneProgramDestination> destinations)
		{
			this.state = DroneSubbehaviourPlexer.Program.State.INACTIVE;
			this.sources = sources;
			this.destinations = destinations;
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0002A3F4 File Offset: 0x000285F4
		public DroneSubbehaviour PickNextBehaviour()
		{
			if (this.state == DroneSubbehaviourPlexer.Program.State.INACTIVE)
			{
				this.state = DroneSubbehaviourPlexer.Program.State.GATHER;
			}
			if (this.state == DroneSubbehaviourPlexer.Program.State.GATHER)
			{
				DroneSubbehaviour droneSubbehaviour = this.PickNextGatherBehaviour();
				if (droneSubbehaviour != null)
				{
					return droneSubbehaviour;
				}
				this.state = DroneSubbehaviourPlexer.Program.State.DEPOSIT;
			}
			if (this.state == DroneSubbehaviourPlexer.Program.State.DEPOSIT)
			{
				DroneSubbehaviour droneSubbehaviour2 = this.PickNextDepositBehaviour();
				if (droneSubbehaviour2 != null)
				{
					return droneSubbehaviour2;
				}
				this.state = DroneSubbehaviourPlexer.Program.State.INACTIVE;
			}
			return null;
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0002A458 File Offset: 0x00028658
		public void Destroy()
		{
			foreach (DroneProgramSource instance in this.sources)
			{
				Destroyer.Destroy(instance, "DroneSubbehaviourPlexer.Program.Destroy#1");
			}
			foreach (DroneProgramDestination instance2 in this.destinations)
			{
				Destroyer.Destroy(instance2, "DroneSubbehaviourPlexer.Program.Destroy#2");
			}
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x0002A4F4 File Offset: 0x000286F4
		public void ResetProgram()
		{
			this.state = DroneSubbehaviourPlexer.Program.State.INACTIVE;
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0002A4FD File Offset: 0x000286FD
		private DroneSubbehaviour PickNextGatherBehaviour()
		{
			return this.sources.FirstOrDefault((DroneProgramSource b) => b.Relevancy());
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0002A529 File Offset: 0x00028729
		private DroneSubbehaviour PickNextDepositBehaviour()
		{
			return this.destinations.FirstOrDefault((DroneProgramDestination b) => b.Relevancy(false));
		}

		// Token: 0x040007E6 RID: 2022
		private List<DroneProgramSource> sources;

		// Token: 0x040007E7 RID: 2023
		private List<DroneProgramDestination> destinations;

		// Token: 0x020001BD RID: 445
		public enum State
		{
			// Token: 0x040007E9 RID: 2025
			INACTIVE,
			// Token: 0x040007EA RID: 2026
			GATHER,
			// Token: 0x040007EB RID: 2027
			DEPOSIT
		}
	}
}
