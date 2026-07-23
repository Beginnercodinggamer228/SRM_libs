using System;
using UnityEngine;

// Token: 0x0200078B RID: 1931
public class SwitchHandler
{
	// Token: 0x0600284D RID: 10317 RVA: 0x00098F9E File Offset: 0x0009719E
	public SwitchHandler(Animator anim, GameObject go)
	{
		this.anim = anim;
		this.animUpId = Animator.StringToHash("Up");
		this.animUpImmediateId = Animator.StringToHash("UpImmediate");
		this.animDownImmediateId = Animator.StringToHash("DownImmediate");
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x00098FE0 File Offset: 0x000971E0
	public void SetState(SwitchHandler.State state, bool immediate)
	{
		bool flag = state == SwitchHandler.State.UP;
		if (flag == this.anim.GetBool(this.animUpId))
		{
			return;
		}
		this.anim.SetBool(this.animUpId, flag);
		if (immediate)
		{
			if (flag)
			{
				this.anim.SetTrigger(this.animUpImmediateId);
				return;
			}
			this.anim.SetTrigger(this.animDownImmediateId);
		}
	}

	// Token: 0x040027F2 RID: 10226
	private Animator anim;

	// Token: 0x040027F3 RID: 10227
	private int animUpId;

	// Token: 0x040027F4 RID: 10228
	private int animUpImmediateId;

	// Token: 0x040027F5 RID: 10229
	private int animDownImmediateId;

	// Token: 0x0200078C RID: 1932
	public enum State
	{
		// Token: 0x040027F7 RID: 10231
		UP,
		// Token: 0x040027F8 RID: 10232
		DOWN
	}

	// Token: 0x0200078D RID: 1933
	public abstract class Switchable : SRBehaviour
	{
		// Token: 0x0600284F RID: 10319
		public abstract void SetState(SwitchHandler.State state, bool immediate);
	}
}
