using System;
using UnityEngine;

// Token: 0x02000737 RID: 1847
public class NotifyTutorialInArea : MonoBehaviour
{
	// Token: 0x0600269A RID: 9882 RVA: 0x00093455 File Offset: 0x00091655
	public void Awake()
	{
		this.tutDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x00093468 File Offset: 0x00091668
	public void OnTriggerEnter(Collider collider)
	{
		Identifiable component = collider.GetComponent<Identifiable>();
		if (component != null && component.id == Identifiable.Id.PLAYER)
		{
			this.tutDir.SetInMarketArea(true);
		}
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x0009349C File Offset: 0x0009169C
	public void OnTriggerExit(Collider collider)
	{
		Identifiable component = collider.GetComponent<Identifiable>();
		if (component != null && component.id == Identifiable.Id.PLAYER)
		{
			this.tutDir.SetInMarketArea(false);
		}
	}

	// Token: 0x040025D6 RID: 9686
	private TutorialDirector tutDir;
}
