using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000812 RID: 2066
public static class vp_GlobalEventReturn<R>
{
	// Token: 0x06002B60 RID: 11104 RVA: 0x000A33BC File Offset: 0x000A15BC
	public static void Register(string name, vp_GlobalCallbackReturn<R> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallbackReturn<R>> list = (List<vp_GlobalCallbackReturn<R>>)vp_GlobalEventReturn<R>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallbackReturn<R>>();
			vp_GlobalEventReturn<R>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x000A3418 File Offset: 0x000A1618
	public static void Unregister(string name, vp_GlobalCallbackReturn<R> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallbackReturn<R>> list = (List<vp_GlobalCallbackReturn<R>>)vp_GlobalEventReturn<R>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x000A3469 File Offset: 0x000A1669
	public static R Send(string name)
	{
		return vp_GlobalEventReturn<R>.Send(name, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x000A3474 File Offset: 0x000A1674
	public static R Send(string name, vp_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		List<vp_GlobalCallbackReturn<R>> list = (List<vp_GlobalCallbackReturn<R>>)vp_GlobalEventReturn<R>.m_Callbacks[name];
		if (list != null)
		{
			R result = default(R);
			foreach (vp_GlobalCallbackReturn<R> vp_GlobalCallbackReturn in list)
			{
				result = vp_GlobalCallbackReturn();
			}
			return result;
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
		return default(R);
	}

	// Token: 0x040029FA RID: 10746
	private static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}
