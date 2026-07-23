using System;
using UnityEngine;

// Token: 0x02000738 RID: 1848
public class NotifyTutorialInBarnArea : MonoBehaviour
{
	// Token: 0x0600269E RID: 9886 RVA: 0x000934CF File Offset: 0x000916CF
	public void Awake()
	{
		this.tutDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
	}

	// Token: 0x0600269F RID: 9887 RVA: 0x000934E4 File Offset: 0x000916E4
	public void OnTriggerEnter(Collider collider)
	{
		Identifiable component = collider.GetComponent<Identifiable>();
		if (component != null && component.id == Identifiable.Id.PLAYER)
		{
			this.tutDir.SetInBarnArea(true);
		}
	}

	// Token: 0x060026A0 RID: 9888 RVA: 0x00093518 File Offset: 0x00091718
	public void OnTriggerExit(Collider collider)
	{
		Identifiable component = collider.GetComponent<Identifiable>();
		if (component != null && component.id == Identifiable.Id.PLAYER)
		{
			this.tutDir.SetInBarnArea(false);
		}
	}

	// Token: 0x040025D7 RID: 9687
	private TutorialDirector tutDir;
}
