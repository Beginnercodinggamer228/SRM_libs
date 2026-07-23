using System;
using UnityEngine;

// Token: 0x02000826 RID: 2086
public static class vp_TargetEventReturn<T, R>
{
	// Token: 0x06002BBF RID: 11199 RVA: 0x000A4B44 File Offset: 0x000A2D44
	public static void Register(object target, string eventName, Func<T, R> callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 5);
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x000A478F File Offset: 0x000A298F
	public static void Unregister(object target, string eventName, Func<T, R> callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x000A4799 File Offset: 0x000A2999
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x000A4B50 File Offset: 0x000A2D50
	public static R Send(object target, string eventName, T arg, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 5, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<T, R>)callback)(arg);
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

	// Token: 0x06002BC3 RID: 11203 RVA: 0x000A4BB0 File Offset: 0x000A2DB0
	public static R SendUpwards(Component target, string eventName, T arg, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 5, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<T, R>)callback)(arg);
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
