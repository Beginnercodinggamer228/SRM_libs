using System;

// Token: 0x020005BF RID: 1471
public class Map : SRSingleton<Map>
{
	// Token: 0x06001E83 RID: 7811 RVA: 0x00073A05 File Offset: 0x00071C05
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x00073A1D File Offset: 0x00071C1D
	public void Start()
	{
		this.mapUI.gameObject.SetActive(false);
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x00073A30 File Offset: 0x00071C30
	public void Update()
	{
		if (SRInput.Actions.openMap.WasPressed && !this.timeDir.IsFastForwarding())
		{
			if (!this.mapUI.gameObject.activeSelf)
			{
				this.OpenMap(ZoneDirector.Zone.NONE);
				return;
			}
		}
		else if (SRInput.PauseActions.closeMap.WasPressed && this.mapUI.gameObject.activeSelf)
		{
			this.CloseMap();
		}
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x00073A9E File Offset: 0x00071C9E
	public void OpenMap(ZoneDirector.Zone unlockedZone)
	{
		if (unlockedZone != ZoneDirector.Zone.NONE)
		{
			this.mapUI.AddZoneToReveal(unlockedZone);
		}
		this.mapUI.gameObject.SetActive(true);
		this.mapUI.OpenMap();
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x00073ACC File Offset: 0x00071CCC
	private void CloseMap()
	{
		this.mapUI.Close();
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x00073AD9 File Offset: 0x00071CD9
	public void RegisterMarker(DisplayOnMap marker)
	{
		this.mapUI.RegisterObject(marker);
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x00073AE7 File Offset: 0x00071CE7
	public void DeregisterMarker(DisplayOnMap marker)
	{
		this.mapUI.DeregisterObject(marker);
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x00073AF5 File Offset: 0x00071CF5
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x04001D97 RID: 7575
	public MapUI mapUI;

	// Token: 0x04001D98 RID: 7576
	private TimeDirector timeDir;
}
