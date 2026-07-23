using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Sentry
{
	// Token: 0x020008A3 RID: 2211
	[Serializable]
	public class SentryEvent
	{
		// Token: 0x0600304D RID: 12365 RVA: 0x000BE464 File Offset: 0x000BC664
		public SentryEvent(string message, List<Breadcrumb> breadcrumbs = null)
		{
			this.event_id = Guid.NewGuid().ToString("N");
			this.message = SentryEvent.GetDescription(message);
			this.timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
			this.level = "error";
			this.breadcrumbs = breadcrumbs;
			this.contexts = new Context();
			this.release = SentryEvent.GetVersion();
			this.tags = new Tags();
			this.tags.cultureName = CultureInfo.CurrentCulture.Name;
			if (SRSingleton<GameContext>.Instance != null && SRSingleton<GameContext>.Instance.MessageDirector != null)
			{
				this.tags.gameLanguage = SRSingleton<GameContext>.Instance.MessageDirector.GetCurrentLanguageCode();
			}
			this.tags.isModded = SystemContext.IsModded;
		}

		// Token: 0x0600304E RID: 12366 RVA: 0x000BE568 File Offset: 0x000BC768
		private static string GetDescription(string description)
		{
			string result;
			try
			{
				string text = "NONE";
				if (SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.Player != null)
				{
					Transform transform = SRSingleton<SceneContext>.Instance.Player.transform;
					text = transform.position + " Facing: " + transform.eulerAngles;
				}
				if (SRSingleton<GameContext>.Instance != null)
				{
					result = string.Format("{0}\n\nVersion: {1}\nPosition: {2}\n\nLog:\n{3}", new object[]
					{
						description,
						SentryEvent.GetVersion(),
						text,
						SRSingleton<GameContext>.Instance.LogText
					});
				}
				else
				{
					result = string.Format("{0}\n\nVersion: {1}\nPosition: {2}\n\nLog:\n{3}", new object[]
					{
						description,
						SentryEvent.GetVersion(),
						text,
						"Log text not available."
					});
				}
			}
			catch (Exception ex)
			{
				result = string.Format("Caught exception while getting description for Sentry: {0}", ex.Message);
			}
			return result;
		}

		// Token: 0x0600304F RID: 12367 RVA: 0x000BE658 File Offset: 0x000BC858
		private static string GetVersion()
		{
			if (SRSingleton<GameContext>.Instance != null)
			{
				return SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("build").Xlate("m.version");
			}
			return Application.version;
		}

		// Token: 0x04002E5F RID: 11871
		public string event_id;

		// Token: 0x04002E60 RID: 11872
		public string message;

		// Token: 0x04002E61 RID: 11873
		public string timestamp;

		// Token: 0x04002E62 RID: 11874
		public string logger;

		// Token: 0x04002E63 RID: 11875
		public string level;

		// Token: 0x04002E64 RID: 11876
		public string platform = "csharp";

		// Token: 0x04002E65 RID: 11877
		public string release;

		// Token: 0x04002E66 RID: 11878
		public Context contexts;

		// Token: 0x04002E67 RID: 11879
		public SdkVersion sdk = new SdkVersion();

		// Token: 0x04002E68 RID: 11880
		public List<Breadcrumb> breadcrumbs;

		// Token: 0x04002E69 RID: 11881
		public User user = new User();

		// Token: 0x04002E6A RID: 11882
		public Tags tags;
	}
}
