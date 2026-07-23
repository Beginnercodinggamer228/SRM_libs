using System;
using UnityEngine;

// Token: 0x02000046 RID: 70
public abstract class MessageOfTheDayProvider : ScriptableObject
{
	// Token: 0x0600011D RID: 285 RVA: 0x00009F2B File Offset: 0x0000812B
	public void Get(MessageOfTheDayProvider.SuccessHandler onSuccess, MessageOfTheDayProvider.ErrorHandler onError)
	{
		this.RetrieveMessage(onSuccess, onError);
	}

	// Token: 0x0600011E RID: 286
	protected abstract void RetrieveMessage(MessageOfTheDayProvider.SuccessHandler onSuccess, MessageOfTheDayProvider.ErrorHandler onError);

	// Token: 0x02000047 RID: 71
	// (Invoke) Token: 0x06000121 RID: 289
	public delegate void SuccessHandler(MessageOfTheDay message);

	// Token: 0x02000048 RID: 72
	// (Invoke) Token: 0x06000125 RID: 293
	public delegate void ErrorHandler();
}
