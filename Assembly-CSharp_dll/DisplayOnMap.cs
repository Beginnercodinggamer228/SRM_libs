using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public class DisplayOnMap : MonoBehaviour
{
	// Token: 0x060024EE RID: 9454 RVA: 0x0008DFD4 File Offset: 0x0008C1D4
	public virtual void Awake()
	{
		SRSingleton<Map>.Instance.RegisterMarker(this);
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.marker = UnityEngine.Object.Instantiate<MapMarker>(this.markerPrefab, base.transform);
		this.marker.gameObject.SetActive(false);
	}

	// Token: 0x060024EF RID: 9455 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void Refresh()
	{
	}

	// Token: 0x060024F0 RID: 9456 RVA: 0x0008E024 File Offset: 0x0008C224
	public virtual Vector3 GetCurrentPosition()
	{
		return base.gameObject.transform.position;
	}

	// Token: 0x060024F1 RID: 9457 RVA: 0x0008E036 File Offset: 0x0008C236
	public virtual MapMarker GetMarker()
	{
		return this.marker;
	}

	// Token: 0x060024F2 RID: 9458 RVA: 0x0008E03E File Offset: 0x0008C23E
	public virtual ZoneDirector.Zone GetZoneId()
	{
		return base.GetComponentInParent<ZoneDirector>().zone;
	}

	// Token: 0x060024F3 RID: 9459 RVA: 0x0008E04C File Offset: 0x0008C24C
	public virtual bool ShowOnMap()
	{
		CellDirector parentCellDirector = this.GetParentCellDirector();
		return (!(parentCellDirector != null) || !parentCellDirector.notShownOnMap) && (!this.HideInFog || this.playerState.HasUnlockedMap(this.GetZoneId()));
	}

	// Token: 0x060024F4 RID: 9460 RVA: 0x0008E091 File Offset: 0x0008C291
	public virtual Quaternion GetCurrentRotation()
	{
		return Quaternion.identity;
	}

	// Token: 0x060024F5 RID: 9461 RVA: 0x0008E098 File Offset: 0x0008C298
	public virtual RegionRegistry.RegionSetId GetRegionSetId()
	{
		if (this.regionSetId != null)
		{
			return this.regionSetId.Value;
		}
		RegionMember component = base.GetComponent<RegionMember>();
		if (component != null)
		{
			this.regionSetId = new RegionRegistry.RegionSetId?(component.setId);
			return this.regionSetId.Value;
		}
		Region componentInParent = base.GetComponentInParent<Region>();
		if (componentInParent != null)
		{
			this.regionSetId = new RegionRegistry.RegionSetId?(componentInParent.setId);
			return this.regionSetId.Value;
		}
		throw new Exception(string.Format("Failed to get RegionSetId for DisplayOnMap. [name={0}]", base.gameObject.name));
	}

	// Token: 0x060024F6 RID: 9462 RVA: 0x0008E132 File Offset: 0x0008C332
	protected CellDirector GetParentCellDirector()
	{
		return base.gameObject.GetComponentInParent<CellDirector>();
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x0008E13F File Offset: 0x0008C33F
	public virtual void OnDestroy()
	{
		if (SRSingleton<Map>.Instance != null)
		{
			SRSingleton<Map>.Instance.DeregisterMarker(this);
		}
		if (this.marker != null)
		{
			Destroyer.Destroy(this.marker.gameObject, "DisplayOnMap.OnDestroy");
		}
	}

	// Token: 0x040023D6 RID: 9174
	public MapMarker markerPrefab;

	// Token: 0x040023D7 RID: 9175
	public bool HideInFog;

	// Token: 0x040023D8 RID: 9176
	private MapMarker marker;

	// Token: 0x040023D9 RID: 9177
	private PlayerState playerState;

	// Token: 0x040023DA RID: 9178
	private RegionRegistry.RegionSetId? regionSetId;
}
