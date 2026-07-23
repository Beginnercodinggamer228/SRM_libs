using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007F2 RID: 2034
[Serializable]
public class vp_State
{
	// Token: 0x06002AC7 RID: 10951 RVA: 0x000A168D File Offset: 0x0009F88D
	public vp_State(string typeName, string name = "Untitled", string path = null, TextAsset asset = null)
	{
		this.TypeName = typeName;
		this.Name = name;
		this.TextAsset = asset;
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06002AC8 RID: 10952 RVA: 0x000A16AB File Offset: 0x0009F8AB
	// (set) Token: 0x06002AC9 RID: 10953 RVA: 0x000A16B3 File Offset: 0x0009F8B3
	public bool Enabled
	{
		get
		{
			return this.m_Enabled;
		}
		set
		{
			this.m_Enabled = value;
			if (this.StateManager == null)
			{
				return;
			}
			if (this.m_Enabled)
			{
				this.StateManager.ImposeBlockingList(this);
				return;
			}
			this.StateManager.RelaxBlockingList(this);
		}
	}

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x06002ACA RID: 10954 RVA: 0x000A16E6 File Offset: 0x0009F8E6
	public bool Blocked
	{
		get
		{
			return this.CurrentlyBlockedBy.Count > 0;
		}
	}

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x06002ACB RID: 10955 RVA: 0x000A16F6 File Offset: 0x0009F8F6
	public int BlockCount
	{
		get
		{
			return this.CurrentlyBlockedBy.Count;
		}
	}

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x06002ACC RID: 10956 RVA: 0x000A1703 File Offset: 0x0009F903
	protected List<vp_State> CurrentlyBlockedBy
	{
		get
		{
			if (this.m_CurrentlyBlockedBy == null)
			{
				this.m_CurrentlyBlockedBy = new List<vp_State>();
			}
			return this.m_CurrentlyBlockedBy;
		}
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x000A171E File Offset: 0x0009F91E
	public void AddBlocker(vp_State blocker)
	{
		if (!this.CurrentlyBlockedBy.Contains(blocker))
		{
			this.CurrentlyBlockedBy.Add(blocker);
		}
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x000A173A File Offset: 0x0009F93A
	public void RemoveBlocker(vp_State blocker)
	{
		if (this.CurrentlyBlockedBy.Contains(blocker))
		{
			this.CurrentlyBlockedBy.Remove(blocker);
		}
	}

	// Token: 0x040029BF RID: 10687
	public vp_StateManager StateManager;

	// Token: 0x040029C0 RID: 10688
	public string TypeName;

	// Token: 0x040029C1 RID: 10689
	public string Name;

	// Token: 0x040029C2 RID: 10690
	public TextAsset TextAsset;

	// Token: 0x040029C3 RID: 10691
	public vp_ComponentPreset Preset;

	// Token: 0x040029C4 RID: 10692
	public List<int> StatesToBlock;

	// Token: 0x040029C5 RID: 10693
	[NonSerialized]
	protected bool m_Enabled;

	// Token: 0x040029C6 RID: 10694
	[NonSerialized]
	protected List<vp_State> m_CurrentlyBlockedBy;
}
