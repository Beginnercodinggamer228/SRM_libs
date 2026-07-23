using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000557 RID: 1367
public class ControlIconUI : MonoBehaviour
{
	// Token: 0x06001C71 RID: 7281 RVA: 0x0006C568 File Offset: 0x0006A768
	public void UpdateIcon()
	{
		string activeDeviceString = SRSingleton<GameContext>.Instance.InputDirector.GetActiveDeviceString(this.action, false);
		bool flag = InputDirector.UsingGamepad();
		bool flag2;
		this.ctrlImage.sprite = SRSingleton<GameContext>.Instance.InputDirector.GetActiveDeviceIcon(this.action, false, out flag2);
		this.ctrlText.text = XlateKeyText.XlateKey(activeDeviceString);
		this.ctrlImage.gameObject.SetActive(flag2 || flag);
		this.ctrlText.gameObject.SetActive(!flag2 && !flag);
	}

	// Token: 0x04001B77 RID: 7031
	[SerializeField]
	private TMP_Text ctrlText;

	// Token: 0x04001B78 RID: 7032
	[SerializeField]
	private Image ctrlImage;

	// Token: 0x04001B79 RID: 7033
	[SerializeField]
	private string action;
}
