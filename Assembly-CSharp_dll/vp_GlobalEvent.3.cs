using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000810 RID: 2064
public static class vp_GlobalEvent<T, U>
{
	// Token: 0x06002B56 RID: 11094 RVA: 0x000A30CC File Offset: 0x000A12CC
	public static void Register(string name, vp_GlobalCallback<T, U> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T, U>> list = (List<vp_GlobalCallback<T, U>>)vp_GlobalEvent<T, U>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallback<T, U>>();
			vp_GlobalEvent<T, U>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x000A3128 File Offset: 0x000A1328
	public static void Unregister(string name, vp_GlobalCallback<T, U> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T, U>> list = (List<vp_GlobalCallback<T, U>>)vp_GlobalEvent<T, U>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000A3179 File Offset: 0x000A1379
	public static void Send(string name, T arg1, U arg2)
	{
		vp_GlobalEvent<T, U>.Send(name, arg1, arg2, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x000A3184 File Offset: 0x000A1384
	public static void Send(string name, T arg1, U arg2, vp_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (arg1 == null)
		{
			throw new ArgumentNullException("arg1");
		}
		if (arg2 == null)
		{
			throw new ArgumentNullException("arg2");
		}
		List<vp_GlobalCallback<T, U>> list = (List<vp_GlobalCallback<T, U>>)vp_GlobalEvent<T, U>.m_Callbacks[name];
		if (list != null)
		{
			using (List<vp_GlobalCallback<T, U>>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					vp_GlobalCallback<T, U> vp_GlobalCallback = enumerator.Current;
					vp_GlobalCallback(arg1, arg2);
				}
				return;
			}
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
	}

	// Token: 0x040029F8 RID: 10744
	private static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}
