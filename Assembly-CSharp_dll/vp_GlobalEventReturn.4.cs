using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000815 RID: 2069
public static class vp_GlobalEventReturn<T, U, V, R>
{
	// Token: 0x06002B6F RID: 11119 RVA: 0x000A3800 File Offset: 0x000A1A00
	public static void Register(string name, vp_GlobalCallbackReturn<T, U, V, R> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallbackReturn<T, U, V, R>> list = (List<vp_GlobalCallbackReturn<T, U, V, R>>)vp_GlobalEventReturn<T, U, V, R>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallbackReturn<T, U, V, R>>();
			vp_GlobalEventReturn<T, U, V, R>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06002B70 RID: 11120 RVA: 0x000A385C File Offset: 0x000A1A5C
	public static void Unregister(string name, vp_GlobalCallbackReturn<T, U, V, R> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallbackReturn<T, U, V, R>> list = (List<vp_GlobalCallbackReturn<T, U, V, R>>)vp_GlobalEventReturn<T, U, V, R>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x06002B71 RID: 11121 RVA: 0x000A38AD File Offset: 0x000A1AAD
	public static R Send(string name, T arg1, U arg2, V arg3)
	{
		return vp_GlobalEventReturn<T, U, V, R>.Send(name, arg1, arg2, arg3, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x06002B72 RID: 11122 RVA: 0x000A38BC File Offset: 0x000A1ABC
	public static R Send(string name, T arg1, U arg2, V arg3, vp_GlobalEventMode mode)
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
		List<vp_GlobalCallbackReturn<T, U, V, R>> list = (List<vp_GlobalCallbackReturn<T, U, V, R>>)vp_GlobalEventReturn<T, U, V, R>.m_Callbacks[name];
		if (list != null)
		{
			R result = default(R);
			foreach (vp_GlobalCallbackReturn<T, U, V, R> vp_GlobalCallbackReturn in list)
			{
				result = vp_GlobalCallbackReturn(arg1, arg2, arg3);
			}
			return result;
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
		return default(R);
	}

	// Token: 0x040029FD RID: 10749
	private static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}
