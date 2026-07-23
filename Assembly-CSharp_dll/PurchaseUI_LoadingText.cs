using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Token: 0x020005FA RID: 1530
public class PurchaseUI_LoadingText : SRBehaviour
{
	// Token: 0x06002014 RID: 8212 RVA: 0x0007A76B File Offset: 0x0007896B
	public void Awake()
	{
		this.text = base.GetRequiredComponent<TMP_Text>();
		this.message = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui").Get("l.loading");
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x0007A79D File Offset: 0x0007899D
	public void OnEnable()
	{
		base.StartCoroutine(this.UpdateText_Coroutine());
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x0007A7AC File Offset: 0x000789AC
	private IEnumerator UpdateText_Coroutine()
	{
		int num;
		for (int maxLoops = 0; maxLoops < 2147483647; maxLoops = num)
		{
			for (int dotCount = 0; dotCount <= 3; dotCount = num)
			{
				this.text.text = this.message;
				for (int i = 0; i < dotCount; i++)
				{
					TMP_Text tmp_Text = this.text;
					tmp_Text.text += ".";
				}
				yield return new WaitForSecondsRealtime(0.5f);
				num = dotCount + 1;
			}
			num = maxLoops + 1;
		}
		yield break;
	}

	// Token: 0x04001F5F RID: 8031
	private TMP_Text text;

	// Token: 0x04001F60 RID: 8032
	private string message;
}
