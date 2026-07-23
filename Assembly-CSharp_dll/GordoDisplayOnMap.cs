using System;

// Token: 0x0200071C RID: 1820
public class GordoDisplayOnMap : DisplayOnMap
{
	// Token: 0x060025FF RID: 9727 RVA: 0x000915E1 File Offset: 0x0008F7E1
	public override void Awake()
	{
		base.Awake();
		this.gordoEat = base.GetComponent<GordoEat>();
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x000915F8 File Offset: 0x0008F7F8
	public override bool ShowOnMap()
	{
		if (base.ShowOnMap())
		{
			int num = this.gordoEat.GetEatenCount();
			if (SRSingleton<SceneContext>.Instance.GameModel.currGameMode == PlayerState.GameMode.TIME_LIMIT_V2)
			{
				GordoNearBurstOnGameMode component = base.gameObject.GetComponent<GordoNearBurstOnGameMode>();
				num -= (int)((component == null) ? 0L : ((long)this.gordoEat.GetTargetCount() - (long)((ulong)component.remaining)));
			}
			return num > 0;
		}
		return false;
	}

	// Token: 0x0400255A RID: 9562
	private GordoEat gordoEat;
}
