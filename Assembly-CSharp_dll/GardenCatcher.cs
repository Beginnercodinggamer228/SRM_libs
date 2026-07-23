using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000317 RID: 791
public class GardenCatcher : SRBehaviour
{
	// Token: 0x060010C9 RID: 4297 RVA: 0x00043444 File Offset: 0x00041644
	public void Awake()
	{
		foreach (GardenCatcher.PlantSlot plantSlot in this.plantable)
		{
			this.plantableDict[plantSlot.id] = plantSlot.plantedPrefab;
			this.deluxeDict[plantSlot.id] = plantSlot.deluxePlantedPrefab;
		}
		this.tutDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x000434A8 File Offset: 0x000416A8
	public void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger)
		{
			return;
		}
		Identifiable.Id id = Identifiable.GetId(col.gameObject);
		if (!this.CanAccept(id))
		{
			return;
		}
		this.Plant(id, false);
		this.tutDir.OnPlanted();
		if (this.acceptFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.acceptFX, base.transform.position, base.transform.rotation);
		}
		Destroyer.DestroyActor(col.gameObject, "GardenCatcher.OnTriggerEnter", false);
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x0004352C File Offset: 0x0004172C
	public GameObject Plant(Identifiable.Id cropId, bool isReplacement)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.activator.HasUpgrade(LandPlot.Upgrade.DELUXE_GARDEN) ? this.deluxeDict[cropId] : this.plantableDict[cropId], this.activator.transform.position, this.activator.transform.rotation);
		if (Identifiable.FRUIT_CLASS.Contains(cropId))
		{
			this.activator.Attach(gameObject, false, isReplacement, this.treeScaleUpCue);
		}
		else
		{
			this.activator.Attach(gameObject, false, isReplacement, null);
		}
		return gameObject;
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x000435BB File Offset: 0x000417BB
	public bool CanAccept(Identifiable.Id id)
	{
		return !this.activator.HasAttached() && this.plantableDict.ContainsKey(id);
	}

	// Token: 0x04000FAD RID: 4013
	public GardenCatcher.PlantSlot[] plantable;

	// Token: 0x04000FAE RID: 4014
	public LandPlot activator;

	// Token: 0x04000FAF RID: 4015
	public GameObject acceptFX;

	// Token: 0x04000FB0 RID: 4016
	public SECTR_AudioCue treeScaleUpCue;

	// Token: 0x04000FB1 RID: 4017
	public SECTR_AudioCue treeScaleDownCue;

	// Token: 0x04000FB2 RID: 4018
	private Dictionary<Identifiable.Id, GameObject> plantableDict = new Dictionary<Identifiable.Id, GameObject>(Identifiable.idComparer);

	// Token: 0x04000FB3 RID: 4019
	private Dictionary<Identifiable.Id, GameObject> deluxeDict = new Dictionary<Identifiable.Id, GameObject>(Identifiable.idComparer);

	// Token: 0x04000FB4 RID: 4020
	private TutorialDirector tutDir;

	// Token: 0x02000318 RID: 792
	[Serializable]
	public class PlantSlot
	{
		// Token: 0x04000FB5 RID: 4021
		public Identifiable.Id id;

		// Token: 0x04000FB6 RID: 4022
		public GameObject plantedPrefab;

		// Token: 0x04000FB7 RID: 4023
		public GameObject deluxePlantedPrefab;
	}
}
