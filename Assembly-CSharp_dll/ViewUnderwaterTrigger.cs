using System;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class ViewUnderwaterTrigger : MonoBehaviour
{
	// Token: 0x06000FC5 RID: 4037 RVA: 0x0003E342 File Offset: 0x0003C542
	public void Awake()
	{
		this.ambianceDir = SRSingleton<SceneContext>.Instance.AmbianceDirector;
		this.playerEvents = base.GetComponentInParent<vp_FPPlayerEventHandler>();
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x0003E360 File Offset: 0x0003C560
	public void OnTriggerEnter(Collider col)
	{
		LiquidSource component = col.GetComponent<LiquidSource>();
		if (component != null && component.CountsAsUnderwater())
		{
			this.ambianceDir.EnterWater();
			this.playerEvents.Underwater.TryStart(true);
		}
		if (col.GetComponent<JellySea>())
		{
			this.ambianceDir.EnterSea();
		}
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x0003E3BC File Offset: 0x0003C5BC
	public void OnTriggerExit(Collider col)
	{
		LiquidSource component = col.GetComponent<LiquidSource>();
		if (component != null && component.CountsAsUnderwater())
		{
			this.ambianceDir.ExitWater();
			this.playerEvents.Underwater.TryStop(true);
		}
		if (col.GetComponent<JellySea>())
		{
			this.ambianceDir.ExitSea();
		}
	}

	// Token: 0x04000E87 RID: 3719
	private AmbianceDirector ambianceDir;

	// Token: 0x04000E88 RID: 3720
	private vp_FPPlayerEventHandler playerEvents;
}
