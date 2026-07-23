using System;
using UnityEngine;

// Token: 0x02000094 RID: 148
[RequireComponent(typeof(Animator))]
[AddComponentMenu("SECTR/Audio/SECTR Door")]
public class SECTR_Door : MonoBehaviour
{
	// Token: 0x06000311 RID: 785 RVA: 0x00013A1E File Offset: 0x00011C1E
	public void OpenDoor()
	{
		this.openCount++;
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00013A2E File Offset: 0x00011C2E
	public void CloseDoor()
	{
		this.openCount--;
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00013A40 File Offset: 0x00011C40
	public bool IsFullyOpen()
	{
		return this.cachedAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == this.openState;
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00013A6C File Offset: 0x00011C6C
	public bool IsClosed()
	{
		return this.cachedAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == this.closedState;
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00013A98 File Offset: 0x00011C98
	protected virtual void OnEnable()
	{
		this.cachedAnimator = base.GetComponent<Animator>();
		this.controlParam = Animator.StringToHash(this.ControlParam);
		this.canOpenParam = Animator.StringToHash(this.CanOpenParam);
		this.closedState = Animator.StringToHash(this.ClosedState);
		this.waitingState = Animator.StringToHash(this.WaitingState);
		this.openingState = Animator.StringToHash(this.OpeningState);
		this.openState = Animator.StringToHash(this.OpenState);
		this.closingState = Animator.StringToHash(this.ClosingState);
	}

	// Token: 0x06000316 RID: 790 RVA: 0x00013B28 File Offset: 0x00011D28
	private void Start()
	{
		if (this.controlParam != 0)
		{
			this.cachedAnimator.SetBool(this.controlParam, false);
		}
		if (this.canOpenParam != 0)
		{
			this.cachedAnimator.SetBool(this.canOpenParam, false);
		}
		if (this.Portal)
		{
			this.Portal.SetFlag(SECTR_Portal.PortalFlags.Closed, true);
		}
		this.openCount = 0;
		this.lastState = this.closedState;
		base.SendMessage("OnClose", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00013BA4 File Offset: 0x00011DA4
	private void Update()
	{
		bool flag = this.CanOpen();
		if (this.canOpenParam != 0)
		{
			this.cachedAnimator.SetBool(this.canOpenParam, flag);
		}
		if (this.controlParam != 0 && (flag || this.canOpenParam != 0))
		{
			if (this.openCount > 0)
			{
				this.cachedAnimator.SetBool(this.controlParam, true);
			}
			else
			{
				this.cachedAnimator.SetBool(this.controlParam, false);
			}
		}
		int fullPathHash = this.cachedAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
		if (fullPathHash != this.lastState)
		{
			if (fullPathHash == this.closedState)
			{
				base.SendMessage("OnClose", SendMessageOptions.DontRequireReceiver);
			}
			if (fullPathHash == this.waitingState)
			{
				base.SendMessage("OnWaiting", SendMessageOptions.DontRequireReceiver);
			}
			else if (fullPathHash == this.openingState)
			{
				base.SendMessage("OnOpening", SendMessageOptions.DontRequireReceiver);
			}
			if (fullPathHash == this.openState)
			{
				base.SendMessage("OnOpen", SendMessageOptions.DontRequireReceiver);
			}
			else if (fullPathHash == this.closingState)
			{
				base.SendMessage("OnClosing", SendMessageOptions.DontRequireReceiver);
			}
			this.lastState = fullPathHash;
		}
		if (this.Portal)
		{
			this.Portal.SetFlag(SECTR_Portal.PortalFlags.Closed, this.IsClosed());
		}
	}

	// Token: 0x06000318 RID: 792 RVA: 0x00013A1E File Offset: 0x00011C1E
	protected virtual void OnTriggerEnter(Collider other)
	{
		this.openCount++;
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00013A2E File Offset: 0x00011C2E
	protected virtual void OnTriggerExit(Collider other)
	{
		this.openCount--;
	}

	// Token: 0x0600031A RID: 794 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected virtual bool CanOpen()
	{
		return true;
	}

	// Token: 0x0400032D RID: 813
	private int controlParam;

	// Token: 0x0400032E RID: 814
	private int canOpenParam;

	// Token: 0x0400032F RID: 815
	private int closedState;

	// Token: 0x04000330 RID: 816
	private int waitingState;

	// Token: 0x04000331 RID: 817
	private int openingState;

	// Token: 0x04000332 RID: 818
	private int openState;

	// Token: 0x04000333 RID: 819
	private int closingState;

	// Token: 0x04000334 RID: 820
	private int lastState;

	// Token: 0x04000335 RID: 821
	private Animator cachedAnimator;

	// Token: 0x04000336 RID: 822
	private int openCount;

	// Token: 0x04000337 RID: 823
	[SECTR_ToolTip("The portal this door affects (if any).")]
	public SECTR_Portal Portal;

	// Token: 0x04000338 RID: 824
	[SECTR_ToolTip("The name of the control param in the door.")]
	public string ControlParam = "Open";

	// Token: 0x04000339 RID: 825
	[SECTR_ToolTip("The name of the control param that indicates if we are allowed to open.")]
	public string CanOpenParam = "CanOpen";

	// Token: 0x0400033A RID: 826
	[SECTR_ToolTip("The full name (layer and state) of the Open state in the Animation Controller.")]
	public string OpenState = "Base Layer.Open";

	// Token: 0x0400033B RID: 827
	[SECTR_ToolTip("The full name (layer and state) of the Closed state in the Animation Controller.")]
	public string ClosedState = "Base Layer.Closed";

	// Token: 0x0400033C RID: 828
	[SECTR_ToolTip("The full name (layer and state) of the Opening state in the Animation Controller.")]
	public string OpeningState = "Base Layer.Opening";

	// Token: 0x0400033D RID: 829
	[SECTR_ToolTip("The full name (layer and state) of the Closing state in the Animation Controller.")]
	public string ClosingState = "Base Layer.Closing";

	// Token: 0x0400033E RID: 830
	[SECTR_ToolTip("The full name (layer and state) of the Wating state in the Animation Controller.")]
	public string WaitingState = "Base Layer.Waiting";
}
