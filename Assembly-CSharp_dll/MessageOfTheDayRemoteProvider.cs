using System;
using MonomiPark.SlimeRancher.Services;
using MonomiPark.SlimeRancher.Services.Messages;
using UnityEngine;

// Token: 0x02000049 RID: 73
[CreateAssetMenu(menuName = "Message Of The Day/Remote Provider")]
public class MessageOfTheDayRemoteProvider : MessageOfTheDayProvider
{
	// Token: 0x06000128 RID: 296 RVA: 0x00009F38 File Offset: 0x00008138
	protected override void RetrieveMessage(MessageOfTheDayProvider.SuccessHandler onSuccess, MessageOfTheDayProvider.ErrorHandler onError)
	{
		MessageOfTheDayServiceRequest messageOfTheDayServiceRequest = new MessageOfTheDayServiceRequest(this.url, this.timeout);
		messageOfTheDayServiceRequest.OnError += delegate()
		{
			onError();
		};
		messageOfTheDayServiceRequest.OnSuccess += delegate(MessageOfTheDayV01 message, Texture2D image)
		{
			onSuccess(this.CreateLocalizedMessage(message, image));
		};
		messageOfTheDayServiceRequest.Begin();
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00009F9C File Offset: 0x0000819C
	private MessageOfTheDay CreateLocalizedMessage(MessageOfTheDayV01 serviceMessage, Texture2D image)
	{
		Texture2D texture2D = new Texture2D(image.width, image.height, TextureFormat.RGBA32, true);
		texture2D.SetPixels(image.GetPixels());
		texture2D.Apply();
		Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)image.width, (float)image.height), new Vector2(0.5f, 0.5f));
		LocalizedMessageOfTheDay localizedMessageOfTheDay = new LocalizedMessageOfTheDay(serviceMessage.MessageId, sprite, "en");
		foreach (MessageOfTheDayV01.LocalizedMessage localizedMessage in serviceMessage.LocalizedMessages)
		{
			localizedMessageOfTheDay.AddEntry(localizedMessage.LanguageCode, localizedMessage.AnnouncementText, localizedMessage.TitleText, localizedMessage.BodyText, localizedMessage.ButtonText, localizedMessage.Url);
		}
		return localizedMessageOfTheDay;
	}

	// Token: 0x04000176 RID: 374
	public string url;

	// Token: 0x04000177 RID: 375
	public int timeout;
}
