using System;
using DLCPackage;
using UnityEngine;

// Token: 0x02000043 RID: 67
[Serializable]
public class MessageOfTheDayDirector
{
	// Token: 0x06000113 RID: 275 RVA: 0x00009D6E File Offset: 0x00007F6E
	public MessageOfTheDayProvider GetProvider()
	{
		return this.pcProvider;
	}

	// Token: 0x06000114 RID: 276 RVA: 0x00009D78 File Offset: 0x00007F78
	public void ActivateLink(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			Log.Warning("MotD url to activate was null or empty.", Array.Empty<object>());
			return;
		}
		if (url.StartsWith("dlc://"))
		{
			try
			{
				Id id = (Id)Enum.Parse(typeof(Id), url.Substring("dlc://".Length));
				this.gameContext.DLCDirector.ShowPackageInStore(id);
				return;
			}
			catch (Exception ex)
			{
				Log.Error("Exception when trying to extract DLC ID from DLC URL in MotD.", new object[]
				{
					"Message",
					ex.Message,
					"stackTrace",
					ex.StackTrace
				});
				return;
			}
		}
		Application.OpenURL(url);
	}

	// Token: 0x0400016A RID: 362
	public MessageOfTheDayProvider pcProvider;

	// Token: 0x0400016B RID: 363
	public MessageOfTheDayProvider epicProvider;

	// Token: 0x0400016C RID: 364
	public MessageOfTheDayProvider steamProvider;

	// Token: 0x0400016D RID: 365
	public MessageOfTheDayProvider ps4Provider;

	// Token: 0x0400016E RID: 366
	public MessageOfTheDayProvider xboxProvider;

	// Token: 0x0400016F RID: 367
	public MessageOfTheDayProvider tencentProvider;

	// Token: 0x04000170 RID: 368
	public GameContext gameContext;

	// Token: 0x04000171 RID: 369
	private const string DLC_PREFIX = "dlc://";
}
