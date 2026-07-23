using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000632 RID: 1586
public class TutorialButtonLine : MonoBehaviour
{
	// Token: 0x06002140 RID: 8512 RVA: 0x0007F2B9 File Offset: 0x0007D4B9
	public void Awake()
	{
		this.inputDirector = SRSingleton<GameContext>.Instance.InputDirector;
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x0007F2CB File Offset: 0x0007D4CB
	public void Init(string inputKey, string descStr)
	{
		this.inputKey = inputKey;
		this.keyText.text = inputKey;
		this.desc.text = descStr;
		this.UpdateInstructionIcon();
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x0007F2F4 File Offset: 0x0007D4F4
	private void UpdateInstructionIcon()
	{
		bool flag = InputDirector.UsingGamepad();
		bool flag2;
		this.btnImage.sprite = this.inputDirector.GetActiveDeviceIcon(this.inputKey, false, out flag2);
		this.btnImage.gameObject.SetActive(flag2 || flag);
		this.keyText.gameObject.SetActive(!flag2 && !flag);
	}

	// Token: 0x04002097 RID: 8343
	public TMP_Text keyText;

	// Token: 0x04002098 RID: 8344
	public Image btnImage;

	// Token: 0x04002099 RID: 8345
	public TMP_Text desc;

	// Token: 0x0400209A RID: 8346
	private InputDirector inputDirector;

	// Token: 0x0400209B RID: 8347
	private string inputKey;
}
