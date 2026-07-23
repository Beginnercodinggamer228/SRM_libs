using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000813 RID: 2067
public static class vp_GlobalEventReturn<T, R>
{
	// Token: 0x06002B65 RID: 11109 RVA: 0x000A3514 File Offset: 0x000A1714
	public static void Register(string name, vp_GlobalCallbackReturn<T, R> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallbackReturn<T, R>> list = (List<vp_GlobalCallbackReturn<T, R>>)vp_GlobalEventReturn<T, R>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallbackReturn<T, R>>();
			vp_GlobalEventReturn<T, R>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x000A3570 File Offset: 0x000A1770
	public static void Unregister(string name, vp_GlobalCallbackReturn<T, R> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallbackReturn<T, R>> list = (List<vp_GlobalCallbackReturn<T, R>>)vp_GlobalEventReturn<T, R>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x06002B67 RID: 11111 RVA: 0x000A35C1 File Offset: 0x000A17C1
	public static R Send(string name, T arg1)
	{
		return vp_GlobalEventReturn<T, R>.Send(name, arg1, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x06002B68 RID: 11112 RVA: 0x000A35CC File Offset: 0x000A17CC
	public static R Send(string name, T arg1, vp_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (arg1 == null)
		{
			throw new ArgumentNullException("arg1");
		}
		List<vp_GlobalCallbackReturn<T, R>> list = (List<vp_GlobalCallbackReturn<T, R>>)vp_GlobalEventReturn<T, R>.m_Callbacks[name];
		if (list != null)
		{
			R result = default(R);
			foreach (vp_GlobalCallbackReturn<T, R> vp_GlobalCallbackReturn in list)
			{
				result = vp_GlobalCallbackReturn(arg1);
			}
			return result;
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
		return default(R);
	}

	// Token: 0x040029FB RID: 10747
	private static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}
