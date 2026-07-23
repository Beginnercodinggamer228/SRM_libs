using System;
using UnityEngine;

// Token: 0x02000827 RID: 2087
public static class vp_TargetEventReturn<T, U, R>
{
	// Token: 0x06002BC4 RID: 11204 RVA: 0x000A4C10 File Offset: 0x000A2E10
	public static void Register(object target, string eventName, Func<T, U, R> callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 6);
	}

	// Token: 0x06002BC5 RID: 11205 RVA: 0x000A478F File Offset: 0x000A298F
	public static void Unregister(object target, string eventName, Func<T, U, R> callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x06002BC6 RID: 11206 RVA: 0x000A4799 File Offset: 0x000A2999
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x06002BC7 RID: 11207 RVA: 0x000A4C1C File Offset: 0x000A2E1C
	public static R Send(object target, string eventName, T arg1, U arg2, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 6, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<T, U, R>)callback)(arg1, arg2);
			}
			catch
			{
				eventName += "_";
				continue;
			}
			return result;
		}
		vp_TargetEventHandler.OnNoReceiver(eventName, options);
		result = default(R);
		return result;
	}

	// Token: 0x06002BC8 RID: 11208 RVA: 0x000A4C7C File Offset: 0x000A2E7C
	public static R SendUpwards(Component target, string eventName, T arg1, U arg2, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 6, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<T, U, R>)callback)(arg1, arg2);
			}
			catch
			{
				eventName += "_";
				continue;
			}
			return result;
		}
		vp_TargetEventHandler.OnNoReceiver(eventName, options);
		result = default(R);
		return result;
	}
}
