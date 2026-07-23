using System;
using UnityEngine;

// Token: 0x020004BC RID: 1212
public class VacDelaunchTrigger : MonoBehaviour
{
	// Token: 0x06001964 RID: 6500 RVA: 0x00062E4C File Offset: 0x0006104C
	public void Start()
	{
		this.vacuumable = base.GetComponentInParent<Vacuumable>();
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x00062E5A File Offset: 0x0006105A
	public void SetTriggerEnabled(bool enabled)
	{
		base.gameObject.SetActive(enabled);
	}

	// Token: 0x06001966 RID: 6502 RVA: 0x00062E68 File Offset: 0x00061068
	public void Delaunch()
	{
		if (this.vacuumable == null)
		{
			this.vacuumable = base.GetComponentInParent<Vacuumable>();
		}
		if (this.vacuumable != null && this.vacuumable.delaunch())
		{
			Identifiable component = this.vacuumable.GetComponent<Identifiable>();
			if (component != null && Identifiable.IsSlime(component.id))
			{
				SRSingleton<SceneContext>.Instance.TutorialDirector.OnDelaunchedSlime();
			}
		}
	}

	// Token: 0x04001910 RID: 6416
	private Vacuumable vacuumable;
}
