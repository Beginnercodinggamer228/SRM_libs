using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public class DisableAttractorInDarkness : SRBehaviour, CaveTrigger.Listener
{
	// Token: 0x06001414 RID: 5140 RVA: 0x0004DC1C File Offset: 0x0004BE1C
	public void Start()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.attractor = base.GetComponentInChildren<MosaicAttractor>(true);
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x0004DC3C File Offset: 0x0004BE3C
	public void Update()
	{
		float num = this.timeDir.CurrHourOrStart();
		bool flag;
		if (this.endHour < this.startHour)
		{
			flag = (num >= this.startHour || num <= this.endHour);
		}
		else
		{
			flag = (num <= this.endHour && num >= this.startHour);
		}
		this.SetAttractorActive(!flag && this.caves.Count <= 0);
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x0004DCB4 File Offset: 0x0004BEB4
	public void OnCaveEnter(GameObject caveObj, bool affectLighting, AmbianceDirector.Zone caveZone)
	{
		this.caves.Add(caveObj);
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x0004DCC3 File Offset: 0x0004BEC3
	public void OnCaveExit(GameObject caveObj, bool affectLighting, AmbianceDirector.Zone caveZone)
	{
		this.caves.Remove(caveObj);
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x0004DCD2 File Offset: 0x0004BED2
	private void SetAttractorActive(bool active)
	{
		this.attractor.gameObject.SetActive(active);
	}

	// Token: 0x040012C7 RID: 4807
	public float startHour = 18f;

	// Token: 0x040012C8 RID: 4808
	public float endHour = 6f;

	// Token: 0x040012C9 RID: 4809
	private TimeDirector timeDir;

	// Token: 0x040012CA RID: 4810
	private MosaicAttractor attractor;

	// Token: 0x040012CB RID: 4811
	private HashSet<GameObject> caves = new HashSet<GameObject>();
}
