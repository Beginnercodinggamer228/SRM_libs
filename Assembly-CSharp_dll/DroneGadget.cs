using System;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class DroneGadget : Gadget, GadgetModel.Participant
{
	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x060006CD RID: 1741 RVA: 0x00023956 File Offset: 0x00021B56
	// (set) Token: 0x060006CE RID: 1742 RVA: 0x0002395E File Offset: 0x00021B5E
	public Drone drone { get; private set; }

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x060006CF RID: 1743 RVA: 0x00023967 File Offset: 0x00021B67
	// (set) Token: 0x060006D0 RID: 1744 RVA: 0x0002396F File Offset: 0x00021B6F
	public DroneStation station { get; private set; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x060006D1 RID: 1745 RVA: 0x00023978 File Offset: 0x00021B78
	// (set) Token: 0x060006D2 RID: 1746 RVA: 0x00023980 File Offset: 0x00021B80
	public Region region { get; private set; }

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x060006D3 RID: 1747 RVA: 0x0002398C File Offset: 0x00021B8C
	// (remove) Token: 0x060006D4 RID: 1748 RVA: 0x000239C4 File Offset: 0x00021BC4
	public event DroneGadget.OnProgramsChanged onProgramsChanged;

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x060006D5 RID: 1749 RVA: 0x000239F9 File Offset: 0x00021BF9
	// (set) Token: 0x060006D6 RID: 1750 RVA: 0x00023A01 File Offset: 0x00021C01
	public DroneMetadata.Program[] programs { get; private set; }

	// Token: 0x060006D7 RID: 1751 RVA: 0x00023A0A File Offset: 0x00021C0A
	public override void Awake()
	{
		base.Awake();
		this.station = base.GetComponentInChildren<DroneStation>();
		this.region = base.GetComponentInParent<Region>();
		this.rotationTransform = this.station.transform;
		this.InstantiateDrone();
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00023A41 File Offset: 0x00021C41
	public void OnDestroy()
	{
		this.drone.onDestroyed -= this.InstantiateDrone;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00023A5C File Offset: 0x00021C5C
	public void InitModel(GadgetModel model)
	{
		DroneModel droneModel = (DroneModel)model;
		droneModel.programs = new DroneModel.ProgramData[this.programCount];
		for (int i = 0; i < droneModel.programs.Length; i++)
		{
			droneModel.programs[i] = new DroneModel.ProgramData
			{
				target = "drone.target.none",
				source = "drone.behaviour.none",
				destination = "drone.behaviour.none"
			};
		}
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00023ACD File Offset: 0x00021CCD
	public void SetModel(GadgetModel model)
	{
		this.droneModel = (DroneModel)model;
		this.SetPrograms(this.ProgramsFromData(this.droneModel.programs));
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x00023AF4 File Offset: 0x00021CF4
	public DroneMetadata.Program[] ProgramsFromData(DroneModel.ProgramData[] programData)
	{
		DroneMetadata.Program[] array = new DroneMetadata.Program[programData.Length];
		for (int i = 0; i < programData.Length; i++)
		{
			DroneModel.ProgramData programDataItem = programData[i];
			array[i] = new DroneMetadata.Program
			{
				target = (this.metadata.targets.FirstOrDefault((DroneMetadata.Program.Target c) => c.id == programDataItem.target) ?? this.metadata.GetDefaultTarget()),
				source = (this.metadata.sources.FirstOrDefault((DroneMetadata.Program.Behaviour c) => c.id == programDataItem.source) ?? this.metadata.GetDefaultBehaviour()),
				destination = (this.metadata.destinations.FirstOrDefault((DroneMetadata.Program.Behaviour c) => c.id == programDataItem.destination) ?? this.metadata.GetDefaultBehaviour())
			};
		}
		return array;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00023BD0 File Offset: 0x00021DD0
	public DroneModel.ProgramData[] DataFromPrograms(DroneMetadata.Program[] programs)
	{
		DroneModel.ProgramData[] array = new DroneModel.ProgramData[programs.Length];
		for (int i = 0; i < programs.Length; i++)
		{
			DroneMetadata.Program program = programs[i];
			array[i] = new DroneModel.ProgramData
			{
				target = program.target.id,
				source = program.source.id,
				destination = program.destination.id
			};
		}
		return array;
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00023C3F File Offset: 0x00021E3F
	public void SetPrograms(DroneMetadata.Program[] programs)
	{
		this.programs = programs;
		this.droneModel.programs = this.DataFromPrograms(programs);
		if (this.onProgramsChanged != null)
		{
			this.onProgramsChanged(programs);
		}
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00023C6E File Offset: 0x00021E6E
	public override void OnUserDestroyed()
	{
		base.OnUserDestroyed();
		this.drone.OnGadgetDestroyed();
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x00023C84 File Offset: 0x00021E84
	private void InstantiateDrone()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, base.transform);
		this.drone = gameObject.GetComponent<Drone>();
		this.drone.onDestroyed += this.InstantiateDrone;
		this.drone.TeleportToStation(false);
	}

	// Token: 0x04000643 RID: 1603
	[Tooltip("Drone metadata.")]
	public DroneMetadata metadata;

	// Token: 0x04000644 RID: 1604
	[Tooltip("Drone prefab.")]
	public GameObject prefab;

	// Token: 0x04000645 RID: 1605
	[Tooltip("Number of programs accessible to the drone.")]
	public int programCount;

	// Token: 0x0400064A RID: 1610
	private DroneModel droneModel;

	// Token: 0x0200013C RID: 316
	// (Invoke) Token: 0x060006E2 RID: 1762
	public delegate void OnProgramsChanged(DroneMetadata.Program[] programs);
}
