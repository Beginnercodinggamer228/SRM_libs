using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000592 RID: 1426
public class GardenCountdownUI : MonoBehaviour
{
	// Token: 0x06001D9F RID: 7583 RVA: 0x00070E3C File Offset: 0x0006F03C
	public void Update()
	{
		Identifiable.Id attachedCropId = this.plot.GetAttachedCropId();
		this.mainPanel.SetActive(attachedCropId > Identifiable.Id.NONE);
		if (attachedCropId != Identifiable.Id.NONE)
		{
			this.cropImg.sprite = SRSingleton<GameContext>.Instance.LookupDirector.GetIcon(attachedCropId);
		}
	}

	// Token: 0x04001CBA RID: 7354
	public GameObject mainPanel;

	// Token: 0x04001CBB RID: 7355
	public Image cropImg;

	// Token: 0x04001CBC RID: 7356
	public LandPlot plot;
}
