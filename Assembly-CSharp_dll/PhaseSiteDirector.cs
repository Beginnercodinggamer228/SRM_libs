using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000413 RID: 1043
public class PhaseSiteDirector : SRBehaviour, WorldModel.Participant
{
	// Token: 0x060015C7 RID: 5575 RVA: 0x000548F5 File Offset: 0x00052AF5
	public void Awake()
	{
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		SRSingleton<SceneContext>.Instance.GameModel.RegisterWorldParticipant(this);
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(WorldModel model)
	{
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x00054918 File Offset: 0x00052B18
	public void SetModel(WorldModel model)
	{
		this.worldModel = model;
		this.ResetAllSites();
		foreach (PhaseSite phaseSite in new List<PhaseSite>(this.availablePhaseSites))
		{
			if (model.occupiedPhaseSites.Contains(phaseSite.id))
			{
				this.PlacePhaseObject(phaseSite);
			}
		}
		this.RefreshTotalPhaseableObjects();
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x00054998 File Offset: 0x00052B98
	public void ClearSites()
	{
		foreach (PhaseSite site in new List<PhaseSite>(this.occupiedPhaseSites))
		{
			this.ClearSite(site);
		}
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x000549F0 File Offset: 0x00052BF0
	public void Update()
	{
		foreach (PhaseSite item in this.occupiedPhaseSites)
		{
			this.local_occupiedPhaseSite.Add(item);
		}
		bool flag = false;
		if (this.nextSpawnableResourceUpdate < Time.time)
		{
			flag = true;
			this.nextSpawnableResourceUpdate = Time.time + 10f;
		}
		foreach (PhaseSite phaseSite in this.local_occupiedPhaseSite)
		{
			if (phaseSite.phaseableObject.ReadyToPhase())
			{
				PhaseSite target = this.PickRandomAvailableSite();
				this.Phase(phaseSite, target);
			}
			else if (flag)
			{
				SpawnResource component = phaseSite.phaseableObject.GetComponent<SpawnResource>();
				if (component != null && !component.isActiveAndEnabled)
				{
					component.UpdateToTime(this.timeDirector.WorldTime(), 0.0);
				}
			}
		}
		this.local_occupiedPhaseSite.Clear();
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x00054B10 File Offset: 0x00052D10
	public void PlacePhaseObject(string phaseSiteId)
	{
		PhaseSite phaseSite = this.availablePhaseSites.FirstOrDefault((PhaseSite s) => string.Compare(phaseSiteId, s.id) == 0);
		if (phaseSite != null)
		{
			this.PlacePhaseObject(phaseSite);
		}
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x00054B52 File Offset: 0x00052D52
	public void Phase(PhaseSite source, PhaseSite target)
	{
		source.phaseableObject.PhaseOut();
		this.PlacePhaseObject(target, source.phaseableObject);
		this.ClearSite(source);
		target.phaseableObject.PhaseIn();
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x00054B7E File Offset: 0x00052D7E
	public void ClearSite(PhaseSite site)
	{
		site.phaseableObject = null;
		this.availablePhaseSites.Add(site);
		this.occupiedPhaseSites.Remove(site);
		this.worldModel.occupiedPhaseSites.Remove(site.id);
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x00054BB7 File Offset: 0x00052DB7
	public void PlacePhaseObject(PhaseSite site)
	{
		this.PlacePhaseObject(site, UnityEngine.Object.Instantiate<PhaseableObject>(this.phaseableObjectPrefab, site.transform));
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x00054BD1 File Offset: 0x00052DD1
	public void PlacePhaseObject(PhaseSite site, PhaseableObject phasingObject)
	{
		site.phaseableObject = phasingObject;
		this.occupiedPhaseSites.Add(site);
		this.availablePhaseSites.Remove(site);
		this.worldModel.occupiedPhaseSites.Add(site.id);
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x00054C0A File Offset: 0x00052E0A
	public PhaseSite PickRandomAvailableSite()
	{
		return this.availablePhaseSites.ElementAt(UnityEngine.Random.Range(0, this.availablePhaseSites.Count - 1));
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x00054C2C File Offset: 0x00052E2C
	public void ResetAllSites()
	{
		foreach (PhaseSite phaseSite in new List<PhaseSite>(this.occupiedPhaseSites))
		{
			Component phaseableObject = phaseSite.phaseableObject;
			this.ClearSite(phaseSite);
			Destroyer.Destroy(phaseableObject.gameObject, "PhaseSiteDirector.ResetAllSites");
		}
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x00054C9C File Offset: 0x00052E9C
	public void RefreshTotalPhaseableObjects()
	{
		while (this.occupiedPhaseSites.Count > this.numberOfPhaseableObjects)
		{
			PhaseSite phaseSite = this.occupiedPhaseSites.ElementAt(0);
			Component phaseableObject = phaseSite.phaseableObject;
			this.ClearSite(phaseSite);
			Destroyer.Destroy(phaseableObject.gameObject, "PhaseSiteDirector.RefreshTotalPhaseableObjects");
		}
		while (this.occupiedPhaseSites.Count < this.numberOfPhaseableObjects)
		{
			this.PlacePhaseObject(this.PickRandomAvailableSite());
		}
	}

	// Token: 0x040014BD RID: 5309
	public List<PhaseSite> availablePhaseSites;

	// Token: 0x040014BE RID: 5310
	public List<PhaseSite> occupiedPhaseSites = new List<PhaseSite>();

	// Token: 0x040014BF RID: 5311
	public PhaseableObject phaseableObjectPrefab;

	// Token: 0x040014C0 RID: 5312
	public int numberOfPhaseableObjects;

	// Token: 0x040014C1 RID: 5313
	private const int MAX_SITE_SELECTION_ATTEMPTS = 10;

	// Token: 0x040014C2 RID: 5314
	private const float UPDATE_SPAWNABLE_RESOURCE_PERIOD = 10f;

	// Token: 0x040014C3 RID: 5315
	private float nextSpawnableResourceUpdate;

	// Token: 0x040014C4 RID: 5316
	private TimeDirector timeDirector;

	// Token: 0x040014C5 RID: 5317
	private WorldModel worldModel;

	// Token: 0x040014C6 RID: 5318
	private List<PhaseSite> local_occupiedPhaseSite = new List<PhaseSite>();
}
