using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200081C RID: 2076
public abstract class vp_StateEventHandler : vp_EventHandler
{
	// Token: 0x06002B8C RID: 11148 RVA: 0x000A3BAC File Offset: 0x000A1DAC
	protected override void Awake()
	{
		this.EventHandlerType = vp_EventHandlerType.StateEventHandler;
		base.Awake();
		foreach (vp_Component vp_Component in base.transform.root.GetComponentsInChildren<vp_Component>(true))
		{
			if (vp_Component.Parent == null || vp_Component.Parent.GetComponent<vp_Component>() == null)
			{
				this.m_StateTargets.Add(vp_Component);
			}
		}
	}

	// Token: 0x06002B8D RID: 11149 RVA: 0x000A3C17 File Offset: 0x000A1E17
	protected void BindStateToActivity(vp_Activity a)
	{
		this.BindStateToActivityOnStart(a);
		this.BindStateToActivityOnStop(a);
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x000A3C28 File Offset: 0x000A1E28
	protected void BindStateToActivityOnStart(vp_Activity a)
	{
		if (!this.ActivityInitialized(a))
		{
			return;
		}
		string s = a.EventName;
		a.StartCallbacks = (vp_Activity.Callback)Delegate.Combine(a.StartCallbacks, new vp_Activity.Callback(delegate()
		{
			foreach (vp_Component vp_Component in this.m_StateTargets)
			{
				vp_Component.SetState(s, true, true, false);
			}
		}));
	}

	// Token: 0x06002B8F RID: 11151 RVA: 0x000A3C7C File Offset: 0x000A1E7C
	protected void BindStateToActivityOnStop(vp_Activity a)
	{
		if (!this.ActivityInitialized(a))
		{
			return;
		}
		string s = a.EventName;
		a.StopCallbacks = (vp_Activity.Callback)Delegate.Combine(a.StopCallbacks, new vp_Activity.Callback(delegate()
		{
			foreach (vp_Component vp_Component in this.m_StateTargets)
			{
				vp_Component.SetState(s, false, true, false);
			}
		}));
	}

	// Token: 0x06002B90 RID: 11152 RVA: 0x000A3CD0 File Offset: 0x000A1ED0
	public void RefreshActivityStates()
	{
		foreach (vp_Event vp_Event in this.m_HandlerEvents.Values)
		{
			if (vp_Event.EventType == vp_EventType.Activity)
			{
				foreach (vp_Component vp_Component in this.m_StateTargets)
				{
					vp_Component.SetState(vp_Event.EventName, ((vp_Activity)vp_Event).Active, true, false);
				}
			}
		}
	}

	// Token: 0x06002B91 RID: 11153 RVA: 0x000A3D7C File Offset: 0x000A1F7C
	public void ResetActivityStates()
	{
		foreach (vp_Component vp_Component in this.m_StateTargets)
		{
			vp_Component.ResetState();
		}
	}

	// Token: 0x06002B92 RID: 11154 RVA: 0x000A3DCC File Offset: 0x000A1FCC
	public void SetState(string state, bool setActive = true, bool recursive = true, bool includeDisabled = false)
	{
		foreach (vp_Component vp_Component in this.m_StateTargets)
		{
			vp_Component.SetState(state, setActive, recursive, includeDisabled);
		}
	}

	// Token: 0x06002B93 RID: 11155 RVA: 0x000A3E24 File Offset: 0x000A2024
	private bool ActivityInitialized(vp_Activity a)
	{
		if (a == null)
		{
			Debug.LogError("Error: (" + this + ") Activity is null.");
			return false;
		}
		if (string.IsNullOrEmpty(a.EventName))
		{
			Debug.LogError("Error: (" + this + ") Activity not initialized. Make sure the event handler has run its Awake call before binding layers.");
			return false;
		}
		return true;
	}

	// Token: 0x04002A01 RID: 10753
	private List<vp_Component> m_StateTargets = new List<vp_Component>();
}
