using System;
using UnityEngine;

// Token: 0x0200088F RID: 2191
public class vp_PlayerRespawner : vp_Respawner, EventHandlerRegistrable
{
	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06002FB6 RID: 12214 RVA: 0x000BBFB8 File Offset: 0x000BA1B8
	public vp_PlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null)
			{
				this.m_Player = base.transform.GetComponent<vp_PlayerEventHandler>();
			}
			return this.m_Player;
		}
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x000BBFDF File Offset: 0x000BA1DF
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x000BBFE7 File Offset: 0x000BA1E7
	protected override void OnEnable()
	{
		if (this.Player != null)
		{
			this.Register(this.Player);
		}
		base.OnEnable();
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x000BC009 File Offset: 0x000BA209
	protected override void OnDisable()
	{
		if (this.Player != null)
		{
			this.Unregister(this.Player);
		}
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x000BC028 File Offset: 0x000BA228
	public override void Reset()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.Player == null)
		{
			return;
		}
		this.Player.Position.Set(this.Placement.Position);
		this.Player.Rotation.Set(this.Placement.Rotation.eulerAngles);
		this.Player.Stop.Send();
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x000350A2 File Offset: 0x000332A2
	public void Register(vp_EventHandler eventHandler)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x000350A2 File Offset: 0x000332A2
	public void Unregister(vp_EventHandler eventHandler)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04002DE2 RID: 11746
	private vp_PlayerEventHandler m_Player;
}
