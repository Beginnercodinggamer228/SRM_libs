using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007F3 RID: 2035
public class vp_StateManager
{
	// Token: 0x06002ACF RID: 10959 RVA: 0x000A1758 File Offset: 0x0009F958
	public vp_StateManager(vp_Component component, List<vp_State> states)
	{
		this.m_States = states;
		this.m_Component = component;
		this.m_Component.RefreshDefaultState();
		this.m_StateIds = new Dictionary<string, int>();
		foreach (vp_State vp_State in this.m_States)
		{
			vp_State.StateManager = this;
			if (!this.m_StateIds.ContainsKey(vp_State.Name))
			{
				this.m_StateIds.Add(vp_State.Name, this.m_States.IndexOf(vp_State));
			}
			else
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Warning: ",
					this.m_Component.GetType(),
					" on '",
					this.m_Component.name,
					"' has more than one state named: '",
					vp_State.Name,
					"'. Only the topmost one will be used."
				}));
				this.m_States[this.m_DefaultId].StatesToBlock.Add(this.m_States.IndexOf(vp_State));
			}
			if (vp_State.Preset == null)
			{
				vp_State.Preset = new vp_ComponentPreset();
			}
			if (vp_State.TextAsset != null)
			{
				vp_State.Preset.LoadFromTextAsset(vp_State.TextAsset);
			}
		}
		this.m_DefaultId = this.m_States.Count - 1;
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x000A18DC File Offset: 0x0009FADC
	public void ImposeBlockingList(vp_State blocker)
	{
		if (blocker == null)
		{
			return;
		}
		if (blocker.StatesToBlock == null)
		{
			return;
		}
		if (this.m_States == null)
		{
			return;
		}
		foreach (int index in blocker.StatesToBlock)
		{
			this.m_States[index].AddBlocker(blocker);
		}
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x000A1950 File Offset: 0x0009FB50
	public void RelaxBlockingList(vp_State blocker)
	{
		if (blocker == null)
		{
			return;
		}
		if (blocker.StatesToBlock == null)
		{
			return;
		}
		if (this.m_States == null)
		{
			return;
		}
		foreach (int index in blocker.StatesToBlock)
		{
			this.m_States[index].RemoveBlocker(blocker);
		}
	}

	// Token: 0x06002AD2 RID: 10962 RVA: 0x000A19C4 File Offset: 0x0009FBC4
	public void SetState(string state, bool setEnabled = true)
	{
		if (!vp_StateManager.AppPlaying())
		{
			return;
		}
		if (!this.m_StateIds.TryGetValue(state, out this.m_TargetId))
		{
			return;
		}
		if (this.m_TargetId == this.m_DefaultId && !setEnabled)
		{
			Debug.LogWarning("Warning: The 'Default' state cannot be disabled.");
			return;
		}
		this.m_States[this.m_TargetId].Enabled = setEnabled;
		this.CombineStates();
		this.m_Component.Refresh();
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x000A1A34 File Offset: 0x0009FC34
	public void Reset()
	{
		if (!vp_StateManager.AppPlaying())
		{
			return;
		}
		foreach (vp_State vp_State in this.m_States)
		{
			vp_State.Enabled = false;
		}
		this.m_States[this.m_DefaultId].Enabled = true;
		this.m_TargetId = this.m_DefaultId;
		this.CombineStates();
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x000A1AB8 File Offset: 0x0009FCB8
	public void CombineStates()
	{
		for (int i = this.m_States.Count - 1; i > -1; i--)
		{
			if ((i == this.m_DefaultId || (this.m_States[i].Enabled && !this.m_States[i].Blocked && !(this.m_States[i].TextAsset == null))) && this.m_States[i].Preset != null && !(this.m_States[i].Preset.ComponentType == null))
			{
				vp_ComponentPreset.Apply(this.m_Component, this.m_States[i].Preset);
			}
		}
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x000A1B79 File Offset: 0x0009FD79
	public bool IsEnabled(string state)
	{
		return vp_StateManager.AppPlaying() && this.m_StateIds.TryGetValue(state, out this.m_TargetId) && this.m_States[this.m_TargetId].Enabled;
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x00013CC5 File Offset: 0x00011EC5
	private static bool AppPlaying()
	{
		return true;
	}

	// Token: 0x040029C7 RID: 10695
	private vp_Component m_Component;

	// Token: 0x040029C8 RID: 10696
	[NonSerialized]
	private List<vp_State> m_States;

	// Token: 0x040029C9 RID: 10697
	private Dictionary<string, int> m_StateIds;

	// Token: 0x040029CA RID: 10698
	private int m_DefaultId;

	// Token: 0x040029CB RID: 10699
	private int m_TargetId;
}
