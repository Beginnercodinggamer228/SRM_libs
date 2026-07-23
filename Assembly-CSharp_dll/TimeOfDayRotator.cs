using System;

// Token: 0x0200079D RID: 1949
public class TimeOfDayRotator : SRBehaviour
{
	// Token: 0x060028D7 RID: 10455 RVA: 0x0009A6D3 File Offset: 0x000988D3
	public void Start()
	{
		SRSingleton<SceneContext>.Instance.AmbianceDirector.RegisterTimeOfDayRotator(base.gameObject, this.isNightLight);
	}

	// Token: 0x0400284F RID: 10319
	public bool isNightLight;
}
