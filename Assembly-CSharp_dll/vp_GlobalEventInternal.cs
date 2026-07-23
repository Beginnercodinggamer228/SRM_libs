using System;
using System.Collections;

// Token: 0x0200080B RID: 2059
internal static class vp_GlobalEventInternal
{
	// Token: 0x06002B47 RID: 11079 RVA: 0x000A2DFD File Offset: 0x000A0FFD
	public static vp_GlobalEventInternal.UnregisterException ShowUnregisterException(string name)
	{
		return new vp_GlobalEventInternal.UnregisterException(string.Format("Attempting to Unregister the event {0} but vp_GlobalEvent has not registered this event.", name));
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x000A2E0F File Offset: 0x000A100F
	public static vp_GlobalEventInternal.SendException ShowSendException(string name)
	{
		return new vp_GlobalEventInternal.SendException(string.Format("Attempting to Send the event {0} but vp_GlobalEvent has not registered this event.", name));
	}

	// Token: 0x040029F5 RID: 10741
	public static Hashtable Callbacks = new Hashtable();

	// Token: 0x0200080C RID: 2060
	public class UnregisterException : Exception
	{
		// Token: 0x06002B4A RID: 11082 RVA: 0x00035738 File Offset: 0x00033938
		public UnregisterException(string msg) : base(msg)
		{
		}
	}

	// Token: 0x0200080D RID: 2061
	public class SendException : Exception
	{
		// Token: 0x06002B4B RID: 11083 RVA: 0x00035738 File Offset: 0x00033938
		public SendException(string msg) : base(msg)
		{
		}
	}
}
