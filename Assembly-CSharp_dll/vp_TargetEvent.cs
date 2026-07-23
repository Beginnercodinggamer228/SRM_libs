using System;
using UnityEngine;

// Token: 0x02000821 RID: 2081
public static class vp_TargetEvent
{
	// Token: 0x06002BA4 RID: 11172 RVA: 0x000A4784 File Offset: 0x000A2984
	public static void Register(object target, string eventName, Action callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 0);
	}

	// Token: 0x06002BA5 RID: 11173 RVA: 0x000A478F File Offset: 0x000A298F
	public static void Unregister(object target, string eventName, Action callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x06002BA6 RID: 11174 RVA: 0x000A4799 File Offset: 0x000A2999
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x06002BA7 RID: 11175 RVA: 0x000A47A3 File Offset: 0x000A29A3
	public static void Unregister(Component component)
	{
		vp_TargetEventHandler.Unregister(component);
	}

	// Token: 0x06002BA8 RID: 11176 RVA: 0x000A47AC File Offset: 0x000A29AC
	public static void Send(object target, string eventName, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 0, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action)callback)();
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

	// Token: 0x06002BA9 RID: 11177 RVA: 0x000A4800 File Offset: 0x000A2A00
	public static void SendUpwards(Component target, string eventName, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 0, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action)callback)();
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
