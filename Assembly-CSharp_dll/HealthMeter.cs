using System;

// Token: 0x02000596 RID: 1430
public class HealthMeter : SRBehaviour
{
	// Token: 0x06001DBE RID: 7614 RVA: 0x000714C2 File Offset: 0x0006F6C2
	public void Start()
	{
		this.player = SRSingleton<SceneContext>.Instance.PlayerState;
		this.statusBar = base.GetComponent<StatusBar>();
		this.Update();
	}

	// Token: 0x06001DBF RID: 7615 RVA: 0x000714E6 File Offset: 0x0006F6E6
	public void Update()
	{
		this.statusBar.currValue = (float)this.player.GetCurrHealth();
		this.statusBar.maxValue = (float)this.player.GetMaxHealth();
	}

	// Token: 0x04001CD2 RID: 7378
	private PlayerState player;

	// Token: 0x04001CD3 RID: 7379
	private StatusBar statusBar;
}
