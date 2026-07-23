using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using AOT;

// Token: 0x02000020 RID: 32
public class DiscordRpc
{
	// Token: 0x0600007A RID: 122 RVA: 0x0000542B File Offset: 0x0000362B
	[MonoPInvokeCallback(typeof(DiscordRpc.OnReadyInfo))]
	public static void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
	{
		DiscordRpc.Callbacks.readyCallback(ref connectedUser);
	}

	// Token: 0x0600007B RID: 123 RVA: 0x0000543D File Offset: 0x0000363D
	[MonoPInvokeCallback(typeof(DiscordRpc.OnDisconnectedInfo))]
	public static void DisconnectedCallback(int errorCode, string message)
	{
		DiscordRpc.Callbacks.disconnectedCallback(errorCode, message);
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00005450 File Offset: 0x00003650
	[MonoPInvokeCallback(typeof(DiscordRpc.OnErrorInfo))]
	public static void ErrorCallback(int errorCode, string message)
	{
		DiscordRpc.Callbacks.errorCallback(errorCode, message);
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00005463 File Offset: 0x00003663
	[MonoPInvokeCallback(typeof(DiscordRpc.OnJoinInfo))]
	public static void JoinCallback(string secret)
	{
		DiscordRpc.Callbacks.joinCallback(secret);
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00005475 File Offset: 0x00003675
	[MonoPInvokeCallback(typeof(DiscordRpc.OnSpectateInfo))]
	public static void SpectateCallback(string secret)
	{
		DiscordRpc.Callbacks.spectateCallback(secret);
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00005487 File Offset: 0x00003687
	[MonoPInvokeCallback(typeof(DiscordRpc.OnRequestInfo))]
	public static void RequestCallback(ref DiscordRpc.DiscordUser request)
	{
		DiscordRpc.Callbacks.requestCallback(ref request);
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000080 RID: 128 RVA: 0x00005499 File Offset: 0x00003699
	// (set) Token: 0x06000081 RID: 129 RVA: 0x000054A0 File Offset: 0x000036A0
	private static DiscordRpc.EventHandlers Callbacks { get; set; }

	// Token: 0x06000082 RID: 130 RVA: 0x000054A8 File Offset: 0x000036A8
	public static void Initialize(string applicationId, ref DiscordRpc.EventHandlers handlers, bool autoRegister, string optionalSteamId)
	{
		DiscordRpc.Callbacks = handlers;
		DiscordRpc.EventHandlers eventHandlers = default(DiscordRpc.EventHandlers);
		eventHandlers.readyCallback = (DiscordRpc.OnReadyInfo)Delegate.Combine(eventHandlers.readyCallback, new DiscordRpc.OnReadyInfo(DiscordRpc.ReadyCallback));
		eventHandlers.disconnectedCallback = (DiscordRpc.OnDisconnectedInfo)Delegate.Combine(eventHandlers.disconnectedCallback, new DiscordRpc.OnDisconnectedInfo(DiscordRpc.DisconnectedCallback));
		eventHandlers.errorCallback = (DiscordRpc.OnErrorInfo)Delegate.Combine(eventHandlers.errorCallback, new DiscordRpc.OnErrorInfo(DiscordRpc.ErrorCallback));
		eventHandlers.joinCallback = (DiscordRpc.OnJoinInfo)Delegate.Combine(eventHandlers.joinCallback, new DiscordRpc.OnJoinInfo(DiscordRpc.JoinCallback));
		eventHandlers.spectateCallback = (DiscordRpc.OnSpectateInfo)Delegate.Combine(eventHandlers.spectateCallback, new DiscordRpc.OnSpectateInfo(DiscordRpc.SpectateCallback));
		eventHandlers.requestCallback = (DiscordRpc.OnRequestInfo)Delegate.Combine(eventHandlers.requestCallback, new DiscordRpc.OnRequestInfo(DiscordRpc.RequestCallback));
		DiscordRpc.InitializeInternal(applicationId, ref eventHandlers, autoRegister, optionalSteamId);
	}

	// Token: 0x06000083 RID: 131
	[DllImport("discord-rpc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Initialize")]
	private static extern void InitializeInternal(string applicationId, ref DiscordRpc.EventHandlers handlers, bool autoRegister, string optionalSteamId);

	// Token: 0x06000084 RID: 132
	[DllImport("discord-rpc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Shutdown")]
	public static extern void Shutdown();

	// Token: 0x06000085 RID: 133
	[DllImport("discord-rpc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_RunCallbacks")]
	public static extern void RunCallbacks();

	// Token: 0x06000086 RID: 134
	[DllImport("discord-rpc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_UpdatePresence")]
	private static extern void UpdatePresenceNative(ref DiscordRpc.RichPresenceStruct presence);

	// Token: 0x06000087 RID: 135
	[DllImport("discord-rpc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_ClearPresence")]
	public static extern void ClearPresence();

	// Token: 0x06000088 RID: 136
	[DllImport("discord-rpc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_Respond")]
	public static extern void Respond(string userId, DiscordRpc.Reply reply);

	// Token: 0x06000089 RID: 137
	[DllImport("discord-rpc", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Discord_UpdateHandlers")]
	public static extern void UpdateHandlers(ref DiscordRpc.EventHandlers handlers);

	// Token: 0x0600008A RID: 138 RVA: 0x00005594 File Offset: 0x00003794
	public static void UpdatePresence(DiscordRpc.RichPresence presence)
	{
		DiscordRpc.RichPresenceStruct @struct = presence.GetStruct();
		DiscordRpc.UpdatePresenceNative(ref @struct);
		presence.FreeMem();
	}

	// Token: 0x02000021 RID: 33
	// (Invoke) Token: 0x0600008D RID: 141
	public delegate void OnReadyInfo(ref DiscordRpc.DiscordUser connectedUser);

	// Token: 0x02000022 RID: 34
	// (Invoke) Token: 0x06000091 RID: 145
	public delegate void OnDisconnectedInfo(int errorCode, string message);

	// Token: 0x02000023 RID: 35
	// (Invoke) Token: 0x06000095 RID: 149
	public delegate void OnErrorInfo(int errorCode, string message);

	// Token: 0x02000024 RID: 36
	// (Invoke) Token: 0x06000099 RID: 153
	public delegate void OnJoinInfo(string secret);

	// Token: 0x02000025 RID: 37
	// (Invoke) Token: 0x0600009D RID: 157
	public delegate void OnSpectateInfo(string secret);

	// Token: 0x02000026 RID: 38
	// (Invoke) Token: 0x060000A1 RID: 161
	public delegate void OnRequestInfo(ref DiscordRpc.DiscordUser request);

	// Token: 0x02000027 RID: 39
	public struct EventHandlers
	{
		// Token: 0x040000B4 RID: 180
		public DiscordRpc.OnReadyInfo readyCallback;

		// Token: 0x040000B5 RID: 181
		public DiscordRpc.OnDisconnectedInfo disconnectedCallback;

		// Token: 0x040000B6 RID: 182
		public DiscordRpc.OnErrorInfo errorCallback;

		// Token: 0x040000B7 RID: 183
		public DiscordRpc.OnJoinInfo joinCallback;

		// Token: 0x040000B8 RID: 184
		public DiscordRpc.OnSpectateInfo spectateCallback;

		// Token: 0x040000B9 RID: 185
		public DiscordRpc.OnRequestInfo requestCallback;
	}

	// Token: 0x02000028 RID: 40
	[Serializable]
	public struct RichPresenceStruct
	{
		// Token: 0x040000BA RID: 186
		public IntPtr state;

		// Token: 0x040000BB RID: 187
		public IntPtr details;

		// Token: 0x040000BC RID: 188
		public long startTimestamp;

		// Token: 0x040000BD RID: 189
		public long endTimestamp;

		// Token: 0x040000BE RID: 190
		public IntPtr largeImageKey;

		// Token: 0x040000BF RID: 191
		public IntPtr largeImageText;

		// Token: 0x040000C0 RID: 192
		public IntPtr smallImageKey;

		// Token: 0x040000C1 RID: 193
		public IntPtr smallImageText;

		// Token: 0x040000C2 RID: 194
		public IntPtr partyId;

		// Token: 0x040000C3 RID: 195
		public int partySize;

		// Token: 0x040000C4 RID: 196
		public int partyMax;

		// Token: 0x040000C5 RID: 197
		public IntPtr matchSecret;

		// Token: 0x040000C6 RID: 198
		public IntPtr joinSecret;

		// Token: 0x040000C7 RID: 199
		public IntPtr spectateSecret;

		// Token: 0x040000C8 RID: 200
		public bool instance;
	}

	// Token: 0x02000029 RID: 41
	[Serializable]
	public struct DiscordUser
	{
		// Token: 0x040000C9 RID: 201
		public string userId;

		// Token: 0x040000CA RID: 202
		public string username;

		// Token: 0x040000CB RID: 203
		public string discriminator;

		// Token: 0x040000CC RID: 204
		public string avatar;
	}

	// Token: 0x0200002A RID: 42
	public enum Reply
	{
		// Token: 0x040000CE RID: 206
		No,
		// Token: 0x040000CF RID: 207
		Yes,
		// Token: 0x040000D0 RID: 208
		Ignore
	}

	// Token: 0x0200002B RID: 43
	public class RichPresence
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x000055B8 File Offset: 0x000037B8
		internal DiscordRpc.RichPresenceStruct GetStruct()
		{
			if (this._buffers.Count > 0)
			{
				this.FreeMem();
			}
			this._presence.state = this.StrToPtr(this.state);
			this._presence.details = this.StrToPtr(this.details);
			this._presence.startTimestamp = this.startTimestamp;
			this._presence.endTimestamp = this.endTimestamp;
			this._presence.largeImageKey = this.StrToPtr(this.largeImageKey);
			this._presence.largeImageText = this.StrToPtr(this.largeImageText);
			this._presence.smallImageKey = this.StrToPtr(this.smallImageKey);
			this._presence.smallImageText = this.StrToPtr(this.smallImageText);
			this._presence.partyId = this.StrToPtr(this.partyId);
			this._presence.partySize = this.partySize;
			this._presence.partyMax = this.partyMax;
			this._presence.matchSecret = this.StrToPtr(this.matchSecret);
			this._presence.joinSecret = this.StrToPtr(this.joinSecret);
			this._presence.spectateSecret = this.StrToPtr(this.spectateSecret);
			this._presence.instance = this.instance;
			return this._presence;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000571C File Offset: 0x0000391C
		private IntPtr StrToPtr(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return IntPtr.Zero;
			}
			int byteCount = Encoding.UTF8.GetByteCount(input);
			IntPtr intPtr = Marshal.AllocHGlobal(byteCount + 1);
			for (int i = 0; i < byteCount + 1; i++)
			{
				Marshal.WriteByte(intPtr, i, 0);
			}
			this._buffers.Add(intPtr);
			Marshal.Copy(Encoding.UTF8.GetBytes(input), 0, intPtr, byteCount);
			return intPtr;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005784 File Offset: 0x00003984
		private static string StrToUtf8NullTerm(string toconv)
		{
			string text = toconv.Trim();
			byte[] bytes = Encoding.Default.GetBytes(text);
			if (bytes.Length != 0 && bytes[bytes.Length - 1] != 0)
			{
				text += "\0\0";
			}
			return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000057D4 File Offset: 0x000039D4
		internal void FreeMem()
		{
			for (int i = this._buffers.Count - 1; i >= 0; i--)
			{
				Marshal.FreeHGlobal(this._buffers[i]);
				this._buffers.RemoveAt(i);
			}
		}

		// Token: 0x040000D1 RID: 209
		private DiscordRpc.RichPresenceStruct _presence;

		// Token: 0x040000D2 RID: 210
		private readonly List<IntPtr> _buffers = new List<IntPtr>(10);

		// Token: 0x040000D3 RID: 211
		public string state;

		// Token: 0x040000D4 RID: 212
		public string details;

		// Token: 0x040000D5 RID: 213
		public long startTimestamp;

		// Token: 0x040000D6 RID: 214
		public long endTimestamp;

		// Token: 0x040000D7 RID: 215
		public string largeImageKey;

		// Token: 0x040000D8 RID: 216
		public string largeImageText;

		// Token: 0x040000D9 RID: 217
		public string smallImageKey;

		// Token: 0x040000DA RID: 218
		public string smallImageText;

		// Token: 0x040000DB RID: 219
		public string partyId;

		// Token: 0x040000DC RID: 220
		public int partySize;

		// Token: 0x040000DD RID: 221
		public int partyMax;

		// Token: 0x040000DE RID: 222
		public string matchSecret;

		// Token: 0x040000DF RID: 223
		public string joinSecret;

		// Token: 0x040000E0 RID: 224
		public string spectateSecret;

		// Token: 0x040000E1 RID: 225
		public bool instance;
	}
}
