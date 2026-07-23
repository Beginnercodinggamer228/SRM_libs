using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000801 RID: 2049
public abstract class vp_EventHandler : MonoBehaviour
{
	// Token: 0x06002B14 RID: 11028 RVA: 0x000A2828 File Offset: 0x000A0A28
	protected virtual void Awake()
	{
		this.EventHandlerType = vp_EventHandlerType.EventHandler;
		this.m_Initialized = true;
		while (this.m_PendingRegistrants.Count > 0)
		{
			EventHandlerRegistrable eventHandlerRegistrable = this.m_PendingRegistrants.Dequeue();
			if (eventHandlerRegistrable != null)
			{
				eventHandlerRegistrable.Register(this);
			}
		}
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x000A286C File Offset: 0x000A0A6C
	private T GetEvent<T>(string name) where T : vp_Event
	{
		vp_Event vp_Event = null;
		if (!this.m_HandlerEvents.TryGetValue(name, out vp_Event))
		{
			throw new Exception("Failed to find event " + name);
		}
		if (!(vp_Event is T))
		{
			throw new Exception(string.Format("Expected event {0} to be of type {1} but was {2}", name, typeof(T), vp_Event.GetType()));
		}
		return (T)((object)vp_Event);
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x000A28CC File Offset: 0x000A0ACC
	public void RegisterMessage(string name, vp_Message.Sender onMessage)
	{
		vp_Message @event = this.GetEvent<vp_Message>(name);
		if (onMessage != null)
		{
			vp_Message vp_Message = @event;
			vp_Message.Send = (vp_Message.Sender)Delegate.Combine(vp_Message.Send, onMessage);
		}
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x000A28FC File Offset: 0x000A0AFC
	public void RegisterMessage<T>(string name, vp_Message<T>.Sender<T> onMessage)
	{
		vp_Message<T> @event = this.GetEvent<vp_Message<T>>(name);
		if (onMessage != null)
		{
			vp_Message<T> vp_Message = @event;
			vp_Message.Send = (vp_Message<T>.Sender<T>)Delegate.Combine(vp_Message.Send, onMessage);
		}
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x000A292C File Offset: 0x000A0B2C
	public void RegisterMessage<T, V>(string name, vp_Message<T, V>.Sender<T, V> onMessage)
	{
		vp_Message<T, V> @event = this.GetEvent<vp_Message<T, V>>(name);
		if (onMessage != null)
		{
			vp_Message<T, V> vp_Message = @event;
			vp_Message.Send = (vp_Message<T, V>.Sender<T, V>)Delegate.Combine(vp_Message.Send, onMessage);
		}
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x000A295C File Offset: 0x000A0B5C
	public void UnregisterMessage(string name, vp_Message.Sender onMessage)
	{
		vp_Message @event = this.GetEvent<vp_Message>(name);
		if (onMessage != null)
		{
			vp_Message vp_Message = @event;
			vp_Message.Send = (vp_Message.Sender)Delegate.Remove(vp_Message.Send, onMessage);
		}
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x000A298C File Offset: 0x000A0B8C
	public void UnregisterMessage<T>(string name, vp_Message<T>.Sender<T> onMessage)
	{
		vp_Message<T> @event = this.GetEvent<vp_Message<T>>(name);
		if (onMessage != null)
		{
			vp_Message<T> vp_Message = @event;
			vp_Message.Send = (vp_Message<T>.Sender<T>)Delegate.Remove(vp_Message.Send, onMessage);
		}
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x000A29BC File Offset: 0x000A0BBC
	public void UnregisterMessage<T, V>(string name, vp_Message<T, V>.Sender<T, V> onMessage)
	{
		vp_Message<T, V> @event = this.GetEvent<vp_Message<T, V>>(name);
		if (onMessage != null)
		{
			vp_Message<T, V> vp_Message = @event;
			vp_Message.Send = (vp_Message<T, V>.Sender<T, V>)Delegate.Remove(vp_Message.Send, onMessage);
		}
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x000A29EC File Offset: 0x000A0BEC
	public void RegisterActivity(string name, vp_Activity.Callback onStart, vp_Activity.Callback onStop, vp_Activity.Condition canStart, vp_Activity.Condition canStop, vp_Activity.Callback onFailStart, vp_Activity.Callback onFailStop)
	{
		vp_Activity @event = this.GetEvent<vp_Activity>(name);
		if (onStart != null)
		{
			vp_Activity vp_Activity = @event;
			vp_Activity.StartCallbacks = (vp_Activity.Callback)Delegate.Combine(vp_Activity.StartCallbacks, onStart);
		}
		if (onStop != null)
		{
			vp_Activity vp_Activity2 = @event;
			vp_Activity2.StopCallbacks = (vp_Activity.Callback)Delegate.Combine(vp_Activity2.StopCallbacks, onStop);
		}
		if (canStart != null)
		{
			vp_Activity vp_Activity3 = @event;
			vp_Activity3.StartConditions = (vp_Activity.Condition)Delegate.Combine(vp_Activity3.StartConditions, canStart);
		}
		if (canStop != null)
		{
			vp_Activity vp_Activity4 = @event;
			vp_Activity4.StopConditions = (vp_Activity.Condition)Delegate.Combine(vp_Activity4.StopConditions, canStop);
		}
		if (onFailStart != null)
		{
			vp_Activity vp_Activity5 = @event;
			vp_Activity5.FailStartCallbacks = (vp_Activity.Callback)Delegate.Combine(vp_Activity5.FailStartCallbacks, onFailStart);
		}
		if (onFailStop != null)
		{
			vp_Activity vp_Activity6 = @event;
			vp_Activity6.FailStopCallbacks = (vp_Activity.Callback)Delegate.Combine(vp_Activity6.FailStopCallbacks, onFailStart);
		}
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x000A2AA8 File Offset: 0x000A0CA8
	public void RegisterActivity<T>(string name, vp_Activity.Callback onStart, vp_Activity.Callback onStop, vp_Activity.Condition canStart, vp_Activity.Condition canStop, vp_Activity.Callback onFailStart, vp_Activity.Callback onFailStop)
	{
		vp_Activity<T> @event = this.GetEvent<vp_Activity<T>>(name);
		if (onStart != null)
		{
			vp_Activity<T> vp_Activity = @event;
			vp_Activity.StartCallbacks = (vp_Activity.Callback)Delegate.Combine(vp_Activity.StartCallbacks, onStart);
		}
		if (onStop != null)
		{
			vp_Activity<T> vp_Activity2 = @event;
			vp_Activity2.StopCallbacks = (vp_Activity.Callback)Delegate.Combine(vp_Activity2.StopCallbacks, onStop);
		}
		if (canStart != null)
		{
			vp_Activity<T> vp_Activity3 = @event;
			vp_Activity3.StartConditions = (vp_Activity.Condition)Delegate.Combine(vp_Activity3.StartConditions, canStart);
		}
		if (canStop != null)
		{
			vp_Activity<T> vp_Activity4 = @event;
			vp_Activity4.StopConditions = (vp_Activity.Condition)Delegate.Combine(vp_Activity4.StopConditions, canStop);
		}
		if (onFailStart != null)
		{
			vp_Activity<T> vp_Activity5 = @event;
			vp_Activity5.FailStartCallbacks = (vp_Activity.Callback)Delegate.Combine(vp_Activity5.FailStartCallbacks, onFailStart);
		}
		if (onFailStop != null)
		{
			vp_Activity<T> vp_Activity6 = @event;
			vp_Activity6.FailStopCallbacks = (vp_Activity.Callback)Delegate.Combine(vp_Activity6.FailStopCallbacks, onFailStart);
		}
	}

	// Token: 0x06002B1E RID: 11038 RVA: 0x000A2B64 File Offset: 0x000A0D64
	public void UnregisterActivity(string name, vp_Activity.Callback onStart, vp_Activity.Callback onStop, vp_Activity.Condition canStart, vp_Activity.Condition canStop, vp_Activity.Callback onFailStart, vp_Activity.Callback onFailStop)
	{
		vp_Activity @event = this.GetEvent<vp_Activity>(name);
		if (onStart != null)
		{
			vp_Activity vp_Activity = @event;
			vp_Activity.StartCallbacks = (vp_Activity.Callback)Delegate.Remove(vp_Activity.StartCallbacks, onStart);
		}
		if (onStop != null)
		{
			vp_Activity vp_Activity2 = @event;
			vp_Activity2.StopCallbacks = (vp_Activity.Callback)Delegate.Remove(vp_Activity2.StopCallbacks, onStop);
		}
		if (canStart != null)
		{
			vp_Activity vp_Activity3 = @event;
			vp_Activity3.StartConditions = (vp_Activity.Condition)Delegate.Remove(vp_Activity3.StartConditions, canStart);
		}
		if (canStop != null)
		{
			vp_Activity vp_Activity4 = @event;
			vp_Activity4.StopConditions = (vp_Activity.Condition)Delegate.Remove(vp_Activity4.StopConditions, canStop);
		}
		if (onFailStart != null)
		{
			vp_Activity vp_Activity5 = @event;
			vp_Activity5.FailStartCallbacks = (vp_Activity.Callback)Delegate.Remove(vp_Activity5.FailStartCallbacks, onFailStart);
		}
		if (onFailStop != null)
		{
			vp_Activity vp_Activity6 = @event;
			vp_Activity6.FailStopCallbacks = (vp_Activity.Callback)Delegate.Remove(vp_Activity6.FailStopCallbacks, onFailStart);
		}
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x000A2C20 File Offset: 0x000A0E20
	public void UnregisterActivity<T>(string name, vp_Activity.Callback onStart, vp_Activity.Callback onStop, vp_Activity.Condition canStart, vp_Activity.Condition canStop, vp_Activity.Callback onFailStart, vp_Activity.Callback onFailStop)
	{
		vp_Activity<T> @event = this.GetEvent<vp_Activity<T>>(name);
		if (onStart != null)
		{
			vp_Activity<T> vp_Activity = @event;
			vp_Activity.StartCallbacks = (vp_Activity.Callback)Delegate.Remove(vp_Activity.StartCallbacks, onStart);
		}
		if (onStop != null)
		{
			vp_Activity<T> vp_Activity2 = @event;
			vp_Activity2.StopCallbacks = (vp_Activity.Callback)Delegate.Remove(vp_Activity2.StopCallbacks, onStop);
		}
		if (canStart != null)
		{
			vp_Activity<T> vp_Activity3 = @event;
			vp_Activity3.StartConditions = (vp_Activity.Condition)Delegate.Remove(vp_Activity3.StartConditions, canStart);
		}
		if (canStop != null)
		{
			vp_Activity<T> vp_Activity4 = @event;
			vp_Activity4.StopConditions = (vp_Activity.Condition)Delegate.Remove(vp_Activity4.StopConditions, canStop);
		}
		if (onFailStart != null)
		{
			vp_Activity<T> vp_Activity5 = @event;
			vp_Activity5.FailStartCallbacks = (vp_Activity.Callback)Delegate.Remove(vp_Activity5.FailStartCallbacks, onFailStart);
		}
		if (onFailStop != null)
		{
			vp_Activity<T> vp_Activity6 = @event;
			vp_Activity6.FailStopCallbacks = (vp_Activity.Callback)Delegate.Remove(vp_Activity6.FailStopCallbacks, onFailStart);
		}
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x000A2CDC File Offset: 0x000A0EDC
	public void RegisterAttempt(string name, vp_Attempt.Tryer onAttempt)
	{
		vp_Attempt @event = this.GetEvent<vp_Attempt>(name);
		if (onAttempt != null)
		{
			@event.Try = onAttempt;
		}
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x000A2CFC File Offset: 0x000A0EFC
	public void RegisterAttempt<T>(string name, vp_Attempt<T>.Tryer<T> onAttempt)
	{
		vp_Attempt<T> @event = this.GetEvent<vp_Attempt<T>>(name);
		if (onAttempt != null)
		{
			@event.Try = onAttempt;
		}
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x000A2D1C File Offset: 0x000A0F1C
	public void UnregisterAttempt<T>(string name, vp_Attempt<T>.Tryer<T> onAttempt)
	{
		vp_Attempt<T> @event = this.GetEvent<vp_Attempt<T>>(name);
		if (onAttempt != null)
		{
			@event.Try = new vp_Attempt<T>.Tryer<T>(@event.AttemptAlwaysOK);
		}
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x000A2D48 File Offset: 0x000A0F48
	public void UnregisterAttempt(string name, vp_Attempt.Tryer onAttempt)
	{
		vp_Attempt @event = this.GetEvent<vp_Attempt>(name);
		if (onAttempt != null)
		{
			@event.Try = new vp_Attempt.Tryer(vp_Attempt.AlwaysOK);
		}
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x000A2D74 File Offset: 0x000A0F74
	public void RegisterValue<T>(string name, vp_Value<T>.Getter<T> onValueGet, vp_Value<T>.Setter<T> onValueSet)
	{
		vp_Value<T> @event = this.GetEvent<vp_Value<T>>(name);
		if (onValueGet != null)
		{
			@event.Get = onValueGet;
		}
		if (onValueSet != null)
		{
			@event.Set = onValueSet;
		}
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x000A2DA0 File Offset: 0x000A0FA0
	public void UnregisterValue<T>(string name, vp_Value<T>.Getter<T> onValueGet, vp_Value<T>.Setter<T> onValueSet)
	{
		vp_Value<T> @event = this.GetEvent<vp_Value<T>>(name);
		if (onValueGet != null)
		{
			@event.Get = new vp_Value<T>.Getter<T>(@event.GetEmpty);
		}
		if (onValueSet != null)
		{
			@event.Set = new vp_Value<T>.Setter<T>(@event.SetEmpty);
		}
	}

	// Token: 0x040029EE RID: 10734
	protected bool m_Initialized;

	// Token: 0x040029EF RID: 10735
	protected Dictionary<string, vp_Event> m_HandlerEvents = new Dictionary<string, vp_Event>();

	// Token: 0x040029F0 RID: 10736
	protected Queue<EventHandlerRegistrable> m_PendingRegistrants = new Queue<EventHandlerRegistrable>();

	// Token: 0x040029F1 RID: 10737
	public vp_EventHandlerType EventHandlerType;
}
