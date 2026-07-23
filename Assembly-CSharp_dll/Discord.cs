using System;
using RichPresence;

// Token: 0x0200001D RID: 29
public static class Discord
{
	// Token: 0x17000019 RID: 25
	// (get) Token: 0x0600006B RID: 107 RVA: 0x000051ED File Offset: 0x000033ED
	public static Handler RichPresenceHandler
	{
		get
		{
			return Discord.RichPresenceHandlerImpl.Instance;
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x000051F4 File Offset: 0x000033F4
	static Discord()
	{
		try
		{
			DiscordRpc.Initialize("443564201349218305", ref Discord.staticEventHandlers, true, null);
			SRSingleton<GameContext>.Instance.gameObject.AddComponent<Discord.UnityEventListener>();
		}
		catch (Exception ex)
		{
			Log.Error("Failed to initialize Discord.", new object[]
			{
				"exception",
				ex
			});
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00003296 File Offset: 0x00001496
	private static void OnReadyCallback(ref DiscordRpc.DiscordUser user)
	{
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00003296 File Offset: 0x00001496
	private static void OnDisconnectedCallback(int errorCode, string message)
	{
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000052D4 File Offset: 0x000034D4
	private static void OnErrorCallback(int errorCode, string message)
	{
		Log.Error("Discord.errorCallback", new object[]
		{
			"errorCode",
			errorCode,
			"message",
			message
		});
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00003296 File Offset: 0x00001496
	private static void OnJoinCallback(string secret)
	{
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00003296 File Offset: 0x00001496
	private static void OnSpectateCallback(string secret)
	{
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00003296 File Offset: 0x00001496
	private static void OnRequestCallback(ref DiscordRpc.DiscordUser user)
	{
	}

	// Token: 0x040000AF RID: 175
	private static readonly DiscordRpc.EventHandlers staticEventHandlers = new DiscordRpc.EventHandlers
	{
		readyCallback = new DiscordRpc.OnReadyInfo(Discord.OnReadyCallback),
		disconnectedCallback = new DiscordRpc.OnDisconnectedInfo(Discord.OnDisconnectedCallback),
		errorCallback = new DiscordRpc.OnErrorInfo(Discord.OnErrorCallback),
		joinCallback = new DiscordRpc.OnJoinInfo(Discord.OnJoinCallback),
		spectateCallback = new DiscordRpc.OnSpectateInfo(Discord.OnSpectateCallback),
		requestCallback = new DiscordRpc.OnRequestInfo(Discord.OnRequestCallback)
	};

	// Token: 0x040000B0 RID: 176
	private const string DISCORD_ID = "443564201349218305";

	// Token: 0x040000B1 RID: 177
	private const string STEAM_ID = null;

	// Token: 0x0200001E RID: 30
	private class RichPresenceHandlerImpl : Handler
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00005304 File Offset: 0x00003504
		public void SetRichPresence(MainMenuData data)
		{
			MessageDirector messageDirector = SRSingleton<GameContext>.Instance.MessageDirector;
			DiscordRpc.UpdatePresence(new DiscordRpc.RichPresence
			{
				details = messageDirector.Get("global", "l.presence.in_menu"),
				largeImageKey = "main-menu-large"
			});
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005348 File Offset: 0x00003548
		public void SetRichPresence(InZoneData data)
		{
			string arg;
			if (Director.TryGetZoneId(data.zone, out arg))
			{
				MessageDirector messageDirector = SRSingleton<GameContext>.Instance.MessageDirector;
				DiscordRpc.UpdatePresence(new DiscordRpc.RichPresence
				{
					details = messageDirector.Get("global", string.Format("l.presence.{0}", arg)),
					largeImageKey = string.Format("zone-{0}-large", arg),
					state = string.Format("{0}, {1}", messageDirector.Get("ui", string.Format("m.gamemode_{0}", SRSingleton<SceneContext>.Instance.GameModel.currGameMode.ToString().ToLower())), SRSingleton<SceneContext>.Instance.TimeDirector.CurrDayString())
				});
			}
		}

		// Token: 0x040000B2 RID: 178
		public static Discord.RichPresenceHandlerImpl Instance = new Discord.RichPresenceHandlerImpl();
	}

	// Token: 0x0200001F RID: 31
	private class UnityEventListener : SRSingleton<Discord.UnityEventListener>
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00005410 File Offset: 0x00003610
		public void OnApplicationQuit()
		{
			DiscordRpc.ClearPresence();
			DiscordRpc.Shutdown();
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000541C File Offset: 0x0000361C
		public void Update()
		{
			DiscordRpc.RunCallbacks();
		}
	}
}
