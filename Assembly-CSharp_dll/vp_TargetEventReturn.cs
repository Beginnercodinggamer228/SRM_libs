using System;
using UnityEngine;

// Token: 0x02000825 RID: 2085
public static class vp_TargetEventReturn<R>
{
	// Token: 0x06002BB9 RID: 11193 RVA: 0x000A4A80 File Offset: 0x000A2C80
	public static void Register(object target, string eventName, Func<R> callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 4);
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x000A478F File Offset: 0x000A298F
	public static void Unregister(object target, string eventName, Func<R> callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x06002BBB RID: 11195 RVA: 0x000A4799 File Offset: 0x000A2999
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x000A47A3 File Offset: 0x000A29A3
	public static void Unregister(Component component)
	{
		vp_TargetEventHandler.Unregister(component);
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x000A4A8C File Offset: 0x000A2C8C
	public static R Send(object target, string eventName, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 4, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<R>)callback)();
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

	// Token: 0x06002BBE RID: 11198 RVA: 0x000A4AE8 File Offset: 0x000A2CE8
	public static R SendUpwards(Component target, string eventName, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 4, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<R>)callback)();
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
