using System;
using System.Linq;
using DLCPackage;
using UnityEngine;

// Token: 0x02000044 RID: 68
[CreateAssetMenu(menuName = "Message Of The Day/Local Provider")]
public class MessageOfTheDayLocalProvider : MessageOfTheDayProvider
{
	// Token: 0x06000116 RID: 278 RVA: 0x00009E30 File Offset: 0x00008030
	public void SetDLCDirector(DLCDirector director)
	{
		this.dlcDirector = director;
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00009E3C File Offset: 0x0000803C
	protected override void RetrieveMessage(MessageOfTheDayProvider.SuccessHandler onSuccess, MessageOfTheDayProvider.ErrorHandler onError)
	{
		if (!(this.messageCollection != null) || this.messageCollection.messages.Count <= 0)
		{
			onError();
			return;
		}
		MessageOfTheDay messageOfTheDay = (this.dlcDirector != null) ? this.messageCollection.GetRandomMessage(new Predicate<BundledMessageOfTheDay>(this.CanShowMessage)) : this.messageCollection.GetRandomMessage();
		if (messageOfTheDay != null)
		{
			onSuccess(messageOfTheDay);
			return;
		}
		onError();
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00009EB3 File Offset: 0x000080B3
	private bool CanShowMessage(BundledMessageOfTheDay msg)
	{
		if (msg.showForAvailableDLCPackages.Count == 0)
		{
			return true;
		}
		return msg.showForAvailableDLCPackages.Any((Id packageId) => !SRSingleton<GameContext>.Instance.AutoSaveDirector.ProfileManager.Profile.DLC.installed.Contains(packageId));
	}

	// Token: 0x04000172 RID: 370
	public MessageOfTheDayCollection messageCollection;

	// Token: 0x04000173 RID: 371
	private DLCDirector dlcDirector;
}
