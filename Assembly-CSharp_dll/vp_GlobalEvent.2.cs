using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x0200080F RID: 2063
public static class vp_GlobalEvent<T>
{
	// Token: 0x06002B51 RID: 11089 RVA: 0x000A2F74 File Offset: 0x000A1174
	public static void Register(string name, vp_GlobalCallback<T> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T>> list = (List<vp_GlobalCallback<T>>)vp_GlobalEvent<T>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallback<T>>();
			vp_GlobalEvent<T>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x000A2FD0 File Offset: 0x000A11D0
	public static void Unregister(string name, vp_GlobalCallback<T> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T>> list = (List<vp_GlobalCallback<T>>)vp_GlobalEvent<T>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x000A3021 File Offset: 0x000A1221
	public static void Send(string name, T arg1)
	{
		vp_GlobalEvent<T>.Send(name, arg1, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x000A302C File Offset: 0x000A122C
	public static void Send(string name, T arg1, vp_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (arg1 == null)
		{
			throw new ArgumentNullException("arg1");
		}
		List<vp_GlobalCallback<T>> list = (List<vp_GlobalCallback<T>>)vp_GlobalEvent<T>.m_Callbacks[name];
		if (list != null)
		{
			using (List<vp_GlobalCallback<T>>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					vp_GlobalCallback<T> vp_GlobalCallback = enumerator.Current;
					vp_GlobalCallback(arg1);
				}
				return;
			}
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
	}

	// Token: 0x040029F7 RID: 10743
	private static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}
