using System;
using UnityEngine;

// Token: 0x02000823 RID: 2083
public static class vp_TargetEvent<T, U>
{
	// Token: 0x06002BAF RID: 11183 RVA: 0x000A4908 File Offset: 0x000A2B08
	public static void Register(object target, string eventName, Action<T, U> callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 2);
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x000A478F File Offset: 0x000A298F
	public static void Unregister(object target, string eventName, Action<T, U> callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x06002BB1 RID: 11185 RVA: 0x000A4799 File Offset: 0x000A2999
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x06002BB2 RID: 11186 RVA: 0x000A4914 File Offset: 0x000A2B14
	public static void Send(object target, string eventName, T arg1, U arg2, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 2, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action<T, U>)callback)(arg1, arg2);
			}
			catch
			{
				eventName += "_";
				continue;
			}
			return;
		}
		vp_TargetEventHandler.OnNoReceiver(eventName, options);
	}

	// Token: 0x06002BB3 RID: 11187 RVA: 0x000A496C File Offset: 0x000A2B6C
	public static void SendUpwards(Component target, string eventName, T arg1, U arg2, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 2, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action<T, U>)callback)(arg1, arg2);
			}
			catch
			{
				eventName += "_";
				continue;
			}
			return;
		}
		vp_TargetEventHandler.OnNoReceiver(eventName, options);
	}
}
