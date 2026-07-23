using System;
using UnityEngine;

// Token: 0x0200048D RID: 1165
public class SlimeIgniteReact : SRBehaviour, Ignitable
{
	// Token: 0x0600183E RID: 6206 RVA: 0x0005DD3E File Offset: 0x0005BF3E
	public void Awake()
	{
		this.faceAnim = base.GetComponent<SlimeFaceAnimator>();
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.body = base.GetComponent<Rigidbody>();
		this.selfIsIgniter = (base.GetComponent<FireSlimeIgnition>() != null);
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x0005DD78 File Offset: 0x0005BF78
	public void Ignite(GameObject igniter)
	{
		if (this.selfIsIgniter)
		{
			return;
		}
		if (Time.time < this.throttle)
		{
			return;
		}
		this.throttle = Time.time + 0.2f;
		if (this.igniteFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.igniteFX, base.transform.position, base.transform.rotation);
		}
		this.faceAnim.SetTrigger("triggerAlarm");
		this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, 0.5f);
		Vector3 vector = base.transform.position - igniter.transform.position;
		this.body.AddForce(vector.normalized * 1f + Vector3.up * 3f, ForceMode.Impulse);
	}

	// Token: 0x0400179A RID: 6042
	public GameObject igniteFX;

	// Token: 0x0400179B RID: 6043
	private SlimeFaceAnimator faceAnim;

	// Token: 0x0400179C RID: 6044
	private SlimeEmotions emotions;

	// Token: 0x0400179D RID: 6045
	private Rigidbody body;

	// Token: 0x0400179E RID: 6046
	private bool selfIsIgniter;

	// Token: 0x0400179F RID: 6047
	private float throttle;

	// Token: 0x040017A0 RID: 6048
	private const float BACK_FORCE = 1f;

	// Token: 0x040017A1 RID: 6049
	private const float UP_FORCE = 3f;

	// Token: 0x040017A2 RID: 6050
	private const float IGNITE_THROTTLE_TIME = 0.2f;

	// Token: 0x040017A3 RID: 6051
	private const float IGNITE_AGITATION = 0.5f;
}
