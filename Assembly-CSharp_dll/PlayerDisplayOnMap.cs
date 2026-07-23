using System;
using System.Linq;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000748 RID: 1864
public class PlayerDisplayOnMap : DisplayOnMap
{
	// Token: 0x060026F0 RID: 9968 RVA: 0x00094348 File Offset: 0x00092548
	public override void Awake()
	{
		base.Awake();
		this.playerUnknownLocationMarker = UnityEngine.Object.Instantiate<MapMarker>(this.playerUnknownLocationMarkerPrefab);
		this.playerZoneTracker = base.GetComponent<PlayerZoneTracker>();
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x0009436D File Offset: 0x0009256D
	public override ZoneDirector.Zone GetZoneId()
	{
		return this.playerZoneTracker.GetCurrentZone();
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x0009437A File Offset: 0x0009257A
	public bool IsInUnknownArea()
	{
		return this.IsInHiddenCell();
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x00094382 File Offset: 0x00092582
	public override RegionRegistry.RegionSetId GetRegionSetId()
	{
		if (!this.isInHiddenCell)
		{
			return SRSingleton<SceneContext>.Instance.RegionRegistry.GetCurrentRegionSetId();
		}
		return RegionRegistry.RegionSetId.HOME;
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x0009439D File Offset: 0x0009259D
	private bool IsInHiddenCell()
	{
		return (from r in base.GetComponent<RegionMember>().regions
		where r.cellDir.notShownOnMap
		select r).Count<Region>() > 0;
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000943D6 File Offset: 0x000925D6
	public override void OnDestroy()
	{
		base.OnDestroy();
		Destroyer.Destroy(this.playerUnknownLocationMarker, "PlayerDisplayOnMap.OnDestroy");
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000943EE File Offset: 0x000925EE
	public override Vector3 GetCurrentPosition()
	{
		if (!this.isInHiddenCell)
		{
			return base.GetCurrentPosition();
		}
		return Vector3.zero;
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x00094404 File Offset: 0x00092604
	public override void Refresh()
	{
		base.Refresh();
		this.isInHiddenCell = this.IsInHiddenCell();
		if (this.isInHiddenCell)
		{
			this.playerUnknownLocationMarker.gameObject.SetActive(true);
			base.GetMarker().gameObject.SetActive(false);
			return;
		}
		this.playerUnknownLocationMarker.gameObject.SetActive(false);
		base.GetMarker().gameObject.SetActive(true);
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public override bool ShowOnMap()
	{
		return true;
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x00094470 File Offset: 0x00092670
	public override MapMarker GetMarker()
	{
		if (!this.isInHiddenCell)
		{
			return base.GetMarker();
		}
		return this.playerUnknownLocationMarker;
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x00094487 File Offset: 0x00092687
	public override Quaternion GetCurrentRotation()
	{
		if (!this.isInHiddenCell)
		{
			return base.gameObject.transform.rotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x04002698 RID: 9880
	public MapMarker playerUnknownLocationMarkerPrefab;

	// Token: 0x04002699 RID: 9881
	private MapMarker playerUnknownLocationMarker;

	// Token: 0x0400269A RID: 9882
	private bool isInHiddenCell;

	// Token: 0x0400269B RID: 9883
	private PlayerZoneTracker playerZoneTracker;
}
