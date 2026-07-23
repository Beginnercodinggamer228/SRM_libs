using System;
using UnityEngine;

// Token: 0x02000824 RID: 2084
public static class vp_TargetEvent<T, U, V>
{
	// Token: 0x06002BB4 RID: 11188 RVA: 0x000A49C4 File Offset: 0x000A2BC4
	public static void Register(object target, string eventName, Action<T, U, V> callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 3);
	}

	// Token: 0x06002BB5 RID: 11189 RVA: 0x000A478F File Offset: 0x000A298F
	public static void Unregister(object target, string eventName, Action<T, U, V> callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x06002BB6 RID: 11190 RVA: 0x000A4799 File Offset: 0x000A2999
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x06002BB7 RID: 11191 RVA: 0x000A49D0 File Offset: 0x000A2BD0
	public static void Send(object target, string eventName, T arg1, U arg2, V arg3, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 3, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action<T, U, V>)callback)(arg1, arg2, arg3);
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

	// Token: 0x06002BB8 RID: 11192 RVA: 0x000A4A28 File Offset: 0x000A2C28
	public static void SendUpwards(Component target, string eventName, T arg1, U arg2, V arg3, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 3, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action<T, U, V>)callback)(arg1, arg2, arg3);
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
