using System;

// Token: 0x02000794 RID: 1940
public class TeleportDisplayOnMap : DisplayOnMap
{
	// Token: 0x06002873 RID: 10355 RVA: 0x000996D1 File Offset: 0x000978D1
	public override void Awake()
	{
		base.Awake();
		this.teleportSrc = base.GetComponentInChildren<TeleportSource>();
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x000996E5 File Offset: 0x000978E5
	public override bool ShowOnMap()
	{
		return base.ShowOnMap() && SRSingleton<SceneContext>.Instance.TeleportNetwork.IsLinkFullyActive(this.teleportSrc);
	}

	// Token: 0x04002817 RID: 10263
	private TeleportSource teleportSrc;
}
