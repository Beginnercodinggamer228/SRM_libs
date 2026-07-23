using System;
using UnityEngine;

// Token: 0x0200078E RID: 1934
public class SwitchableSlidingDoor : SwitchHandler.Switchable
{
	// Token: 0x06002851 RID: 10321 RVA: 0x00099042 File Offset: 0x00097242
	public void Awake()
	{
		this.upPos = this.upTrans.position;
		this.downPos = this.downTrans.position;
		this.slidingSound = base.gameObject.GetComponent<SECTR_PointSource>();
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x00099078 File Offset: 0x00097278
	public override void SetState(SwitchHandler.State state, bool immediate = false)
	{
		if (state != this.currentState)
		{
			this.slidingSound.Play();
			this.currentState = state;
		}
		this.tgtSlideDownAmt = (float)((state == SwitchHandler.State.UP) ? 0 : 1);
		if (immediate)
		{
			this.slideDownAmt = this.tgtSlideDownAmt;
			this.forceMove = true;
		}
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x000990C4 File Offset: 0x000972C4
	public void FixedUpdate()
	{
		if (this.forceMove || this.slideDownAmt != this.tgtSlideDownAmt)
		{
			if (this.slideDownAmt < this.tgtSlideDownAmt)
			{
				this.slideDownAmt = Mathf.Min(this.tgtSlideDownAmt, this.slideDownAmt + this.MOVE_RATE * Time.fixedDeltaTime);
			}
			else if (this.slideDownAmt > this.tgtSlideDownAmt)
			{
				this.slideDownAmt = Mathf.Max(this.tgtSlideDownAmt, this.slideDownAmt - this.MOVE_RATE * Time.fixedDeltaTime);
			}
			base.transform.position = Vector3.Lerp(this.upPos, this.downPos, this.slideDownAmt);
			this.forceMove = false;
		}
	}

	// Token: 0x040027F9 RID: 10233
	public Transform upTrans;

	// Token: 0x040027FA RID: 10234
	public Transform downTrans;

	// Token: 0x040027FB RID: 10235
	private SECTR_PointSource slidingSound;

	// Token: 0x040027FC RID: 10236
	private Vector3 upPos;

	// Token: 0x040027FD RID: 10237
	private Vector3 downPos;

	// Token: 0x040027FE RID: 10238
	private float tgtSlideDownAmt;

	// Token: 0x040027FF RID: 10239
	private float slideDownAmt;

	// Token: 0x04002800 RID: 10240
	private bool forceMove;

	// Token: 0x04002801 RID: 10241
	private float MOVE_RATE = 0.5f;

	// Token: 0x04002802 RID: 10242
	private SwitchHandler.State currentState;
}
