using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000576 RID: 1398
public class EndGameUIPlortLine : MonoBehaviour
{
	// Token: 0x06001D1C RID: 7452 RVA: 0x0006E820 File Offset: 0x0006CA20
	public void Init(Identifiable.Id id, int amount, int price)
	{
		this.icon.sprite = SRSingleton<GameContext>.Instance.LookupDirector.GetIcon(id);
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
		this.countText.text = bundle.Get("m.plort_amount", new object[]
		{
			amount
		});
		this.currencyText.text = price.ToString();
	}

	// Token: 0x04001C2D RID: 7213
	public Image icon;

	// Token: 0x04001C2E RID: 7214
	public TMP_Text countText;

	// Token: 0x04001C2F RID: 7215
	public TMP_Text currencyText;
}
