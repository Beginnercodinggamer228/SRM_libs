using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007C7 RID: 1991
public class TutorialRadar : MonoBehaviour
{
	// Token: 0x060029B1 RID: 10673 RVA: 0x0009CCDC File Offset: 0x0009AEDC
	public void Awake()
	{
		TutorialRadar.allRadars.Add(this);
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x0009CCE9 File Offset: 0x0009AEE9
	public void OnDestroy()
	{
		TutorialRadar.allRadars.Remove(this);
	}

	// Token: 0x040028ED RID: 10477
	public TutorialDirector.Id tutorialId;

	// Token: 0x040028EE RID: 10478
	public static List<TutorialRadar> allRadars = new List<TutorialRadar>();
}
