using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200080E RID: 2062
public static class vp_GlobalEvent
{
	// Token: 0x06002B4C RID: 11084 RVA: 0x000A2E30 File Offset: 0x000A1030
	public static void Register(string name, vp_GlobalCallback callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback> list = (List<vp_GlobalCallback>)vp_GlobalEvent.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallback>();
			vp_GlobalEvent.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06002B4D RID: 11085 RVA: 0x000A2E8C File Offset: 0x000A108C
	public static void Unregister(string name, vp_GlobalCallback callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback> list = (List<vp_GlobalCallback>)vp_GlobalEvent.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x000A2EDD File Offset: 0x000A10DD
	public static void Send(string name)
	{
		vp_GlobalEvent.Send(name, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x000A2EE8 File Offset: 0x000A10E8
	public static void Send(string name, vp_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		List<vp_GlobalCallback> list = (List<vp_GlobalCallback>)vp_GlobalEvent.m_Callbacks[name];
		if (list != null)
		{
			using (List<vp_GlobalCallback>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					vp_GlobalCallback vp_GlobalCallback = enumerator.Current;
					vp_GlobalCallback();
				}
				return;
			}
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
	}

	// Token: 0x040029F6 RID: 10742
	private static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}
