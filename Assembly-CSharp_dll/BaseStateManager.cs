using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000010 RID: 16
public abstract class BaseStateManager<TState, VComp> : StateManager where TState : BaseState where VComp : vp_Component
{
	// Token: 0x06000046 RID: 70 RVA: 0x000030DC File Offset: 0x000012DC
	public BaseStateManager(VComp managedComponent)
	{
		this.managedComponent = managedComponent;
	}

	// Token: 0x06000047 RID: 71 RVA: 0x000030F6 File Offset: 0x000012F6
	protected void AddState(TState state, int index)
	{
		this.states[index] = state;
		this.stateNameIndex.Add(state.name, index);
	}

	// Token: 0x06000048 RID: 72 RVA: 0x0000311C File Offset: 0x0000131C
	public void SetState(string stateName, bool setEnabled = true)
	{
		int num = -1;
		if (!this.stateNameIndex.TryGetValue(stateName, out num))
		{
			return;
		}
		if (num == this.states.Length - 1 && !setEnabled)
		{
			Debug.LogWarning("Warning: The 'Default' state cannot be disabled.");
			return;
		}
		this.states[num].Enabled = setEnabled;
		this.ApplyStates();
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00003174 File Offset: 0x00001374
	public void Reset()
	{
		for (int i = 0; i < this.states.Length - 1; i++)
		{
			this.states[i].Enabled = false;
		}
		this.states[this.states.Length - 1].Enabled = true;
		this.ApplyStates();
	}

	// Token: 0x0600004A RID: 74 RVA: 0x000031D4 File Offset: 0x000013D4
	public void ApplyStates()
	{
		int num = this.states.Length - 1;
		for (int i = this.states.Length - 1; i >= 0; i--)
		{
			TState tstate = this.states[i];
			if (tstate.Enabled || i == num)
			{
				this.ApplyState(tstate);
			}
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003226 File Offset: 0x00001426
	public void CombineStates()
	{
		this.ApplyStates();
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00003230 File Offset: 0x00001430
	public bool IsEnabled(string stateName)
	{
		int num = -1;
		return this.stateNameIndex.TryGetValue(stateName, out num) && this.states[num].Enabled;
	}

	// Token: 0x0600004D RID: 77
	public abstract void ApplyState(TState state);

	// Token: 0x0400002E RID: 46
	protected VComp managedComponent;

	// Token: 0x0400002F RID: 47
	protected TState[] states;

	// Token: 0x04000030 RID: 48
	protected Dictionary<string, int> stateNameIndex = new Dictionary<string, int>();
}
