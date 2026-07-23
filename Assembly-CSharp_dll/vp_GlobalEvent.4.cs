using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000811 RID: 2065
public static class vp_GlobalEvent<T, U, V>
{
	// Token: 0x06002B5B RID: 11099 RVA: 0x000A3238 File Offset: 0x000A1438
	public static void Register(string name, vp_GlobalCallback<T, U, V> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T, U, V>> list = (List<vp_GlobalCallback<T, U, V>>)vp_GlobalEvent<T, U, V>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallback<T, U, V>>();
			vp_GlobalEvent<T, U, V>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x000A3294 File Offset: 0x000A1494
	public static void Unregister(string name, vp_GlobalCallback<T, U, V> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T, U, V>> list = (List<vp_GlobalCallback<T, U, V>>)vp_GlobalEvent<T, U, V>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x000A32E5 File Offset: 0x000A14E5
	public static void Send(string name, T arg1, U arg2, V arg3)
	{
		vp_GlobalEvent<T, U, V>.Send(name, arg1, arg2, arg3, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x000A32F4 File Offset: 0x000A14F4
	public static void Send(string name, T arg1, U arg2, V arg3, vp_GlobalEventMode mode)
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
		if (arg3 == null)
		{
			throw new ArgumentNullException("arg3");
		}
		List<vp_GlobalCallback<T, U, V>> list = (List<vp_GlobalCallback<T, U, V>>)vp_GlobalEvent<T, U, V>.m_Callbacks[name];
		if (list != null)
		{
			using (List<vp_GlobalCallback<T, U, V>>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					vp_GlobalCallback<T, U, V> vp_GlobalCallback = enumerator.Current;
					vp_GlobalCallback(arg1, arg2, arg3);
				}
				return;
			}
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
	}

	// Token: 0x040029F9 RID: 10745
	private static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}
