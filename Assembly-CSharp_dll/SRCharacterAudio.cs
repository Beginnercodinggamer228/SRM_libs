using System;

// Token: 0x020002D7 RID: 727
public class SRCharacterAudio : SECTR_CharacterAudio, EventHandlerRegistrable
{
	// Token: 0x06000F80 RID: 3968 RVA: 0x0003D550 File Offset: 0x0003B750
	public void Awake()
	{
		this.playerController = base.GetComponent<vp_FPController>();
		this.playerEvents = base.GetComponentInChildren<vp_FPPlayerEventHandler>();
		base.GetComponentInChildren<vp_FPCamera>().BobStepCallback = delegate()
		{
			if (this.playerController.Grounded)
			{
				base.OnFootstep(null);
			}
		};
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x0003D581 File Offset: 0x0003B781
	protected virtual void OnEnable()
	{
		if (this.playerEvents != null)
		{
			this.Register(this.playerEvents);
		}
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x0003D59D File Offset: 0x0003B79D
	protected virtual void OnDisable()
	{
		if (this.playerEvents != null)
		{
			this.Unregister(this.playerEvents);
		}
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x0003D5B9 File Offset: 0x0003B7B9
	public virtual void OnStart_Jump()
	{
		base.OnJump(null);
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0003D5C2 File Offset: 0x0003B7C2
	public virtual void OnMessage_FallImpact(float val)
	{
		if (val > 0.05f)
		{
			base.OnLand(null);
		}
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0003D5D3 File Offset: 0x0003B7D3
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
		eventHandler.RegisterActivity("Jump", new vp_Activity.Callback(this.OnStart_Jump), null, null, null, null, null);
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0003D60A File Offset: 0x0003B80A
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterMessage<float>("FallImpact", new vp_Message<float>.Sender<float>(this.OnMessage_FallImpact));
		eventHandler.UnregisterActivity("Jump", new vp_Activity.Callback(this.OnStart_Jump), null, null, null, null, null);
	}

	// Token: 0x04000E52 RID: 3666
	private vp_FPPlayerEventHandler playerEvents;

	// Token: 0x04000E53 RID: 3667
	private vp_FPController playerController;
}
