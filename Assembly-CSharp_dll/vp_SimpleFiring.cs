using System;
using UnityEngine;

// Token: 0x02000891 RID: 2193
public class vp_SimpleFiring : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x06002FD7 RID: 12247 RVA: 0x000BC7CD File Offset: 0x000BA9CD
	protected virtual void Awake()
	{
		this.m_Player = (vp_PlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x000BC7F4 File Offset: 0x000BA9F4
	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.Register(this.m_Player);
		}
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x000BC810 File Offset: 0x000BAA10
	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.Unregister(this.m_Player);
		}
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x000BC82C File Offset: 0x000BAA2C
	protected virtual void Update()
	{
		if (this.m_Player.Attack.Active)
		{
			this.m_Player.Fire.Try();
		}
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x000350A2 File Offset: 0x000332A2
	public void Register(vp_EventHandler eventHandler)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x000350A2 File Offset: 0x000332A2
	public void Unregister(vp_EventHandler eventHandler)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04002DF7 RID: 11767
	protected vp_PlayerEventHandler m_Player;
}
