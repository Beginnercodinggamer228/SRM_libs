using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005C9 RID: 1481
public class MessageOfTheDayButton : MonoBehaviour
{
	// Token: 0x06001ED1 RID: 7889 RVA: 0x000751C4 File Offset: 0x000733C4
	public void Awake()
	{
		this.motdDirector = SRSingleton<GameContext>.Instance.MessageOfTheDayDirector;
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000751D6 File Offset: 0x000733D6
	public void UpdateButton(string messageId, string buttonText, string url)
	{
		this.messageId = messageId;
		this.buttonLabel.text = buttonText;
		this.url = url;
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x000751F2 File Offset: 0x000733F2
	public void OnClick()
	{
		AnalyticsUtil.CustomEvent("MotDClicked", new Dictionary<string, object>
		{
			{
				"MessageId",
				this.messageId
			}
		}, false);
		this.motdDirector.ActivateLink(this.url);
	}

	// Token: 0x04001DED RID: 7661
	public const string UNITY_ANALYTICS_CLICK_EVENT = "MotDClicked";

	// Token: 0x04001DEE RID: 7662
	public const string UNITY_ANALYTICS_MESSAGE_ID_KEY = "MessageId";

	// Token: 0x04001DEF RID: 7663
	public TMP_Text buttonLabel;

	// Token: 0x04001DF0 RID: 7664
	private MessageOfTheDayDirector motdDirector;

	// Token: 0x04001DF1 RID: 7665
	private string messageId;

	// Token: 0x04001DF2 RID: 7666
	private string url;
}
