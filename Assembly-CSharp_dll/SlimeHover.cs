using System;
using UnityEngine;

// Token: 0x0200048C RID: 1164
public class SlimeHover : SlimeSubbehaviour, Collidable
{
	// Token: 0x0600182F RID: 6191 RVA: 0x0005DAB9 File Offset: 0x0005BCB9
	public override void Awake()
	{
		base.Awake();
		this.calmed = base.GetComponent<CalmedByWaterSpray>();
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x0005DACD File Offset: 0x0005BCCD
	public override void Start()
	{
		base.Start();
		this.cancelHover = false;
		this.nextHoverTime = Time.fixedTime + Randoms.SHARED.GetFloat(this.HoverDelay());
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x0005DAF8 File Offset: 0x0005BCF8
	public override float Relevancy(bool isGrounded)
	{
		if (this.nextHoverTime <= Time.fixedTime)
		{
			return 0.3f;
		}
		return 0f;
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x0005DB14 File Offset: 0x0005BD14
	public void ProcessCollisionEnter(Collision coll)
	{
		if (coll.rigidbody == null)
		{
			foreach (ContactPoint contactPoint in coll.contacts)
			{
				if (contactPoint.point.y > base.transform.position.y + 0.25f * base.transform.lossyScale.y)
				{
					this.cancelHover = true;
				}
			}
		}
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x0005DB88 File Offset: 0x0005BD88
	public override void Selected()
	{
		this.endTime = Time.time + 6f;
		this.nextHoverTime = this.endTime + this.HoverDelay();
		this.floatDir = new Vector3(Randoms.SHARED.GetInRange(-1f, 1f), 0f, Randoms.SHARED.GetInRange(-1f, 1f));
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x000526CC File Offset: 0x000508CC
	public override void Deselected()
	{
		base.Deselected();
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x0005DBF1 File Offset: 0x0005BDF1
	public void FixedUpdate()
	{
		if (this.calmed.IsCalmed())
		{
			this.nextHoverTime += Time.fixedDeltaTime;
		}
	}

	// Token: 0x06001837 RID: 6199 RVA: 0x0005DC14 File Offset: 0x0005BE14
	public override void Action()
	{
		if (this.cancelHover)
		{
			return;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(this.slimeBody.position, -Vector3.up, out raycastHit, this.GetHoverHeight()))
		{
			this.slimeBody.AddForce(Vector3.up * (this.GetHoverAccel() * this.slimeBody.mass * Time.fixedDeltaTime) * (1f - raycastHit.distance * this.GetInvHoverHeight()));
		}
		this.slimeBody.AddForce(this.floatDir * (100f * this.slimeBody.mass * Time.fixedDeltaTime));
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x0005DCC1 File Offset: 0x0005BEC1
	protected virtual float GetHoverAccel()
	{
		return 1200f;
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x0005DCC8 File Offset: 0x0005BEC8
	protected virtual float GetHoverHeight()
	{
		return 5f;
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x0004BA37 File Offset: 0x00049C37
	protected virtual float GetInvHoverHeight()
	{
		return 0.2f;
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x0005DCCF File Offset: 0x0005BECF
	public override bool CanRethink()
	{
		return this.cancelHover || Time.time >= this.endTime;
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x0005DCEC File Offset: 0x0005BEEC
	private float HoverDelay()
	{
		return Mathf.Lerp(10f, 25f, Mathf.Clamp(Randoms.SHARED.GetInRange(-0.1f, 0.1f) + (1f - this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION)), 0f, 1f));
	}

	// Token: 0x04001790 RID: 6032
	private const float HOVER_MIN_DELAY = 10f;

	// Token: 0x04001791 RID: 6033
	private const float HOVER_MAX_DELAY = 25f;

	// Token: 0x04001792 RID: 6034
	private float nextHoverTime;

	// Token: 0x04001793 RID: 6035
	private float endTime;

	// Token: 0x04001794 RID: 6036
	private Vector3 floatDir;

	// Token: 0x04001795 RID: 6037
	private CalmedByWaterSpray calmed;

	// Token: 0x04001796 RID: 6038
	private bool cancelHover;

	// Token: 0x04001797 RID: 6039
	private const float HOVER_TIME = 6f;

	// Token: 0x04001798 RID: 6040
	private const float HOVER_HEIGHT = 5f;

	// Token: 0x04001799 RID: 6041
	private const float INV_HOVER_HEIGHT = 0.2f;
}
