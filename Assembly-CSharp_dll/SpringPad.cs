using System;
using UnityEngine;

// Token: 0x0200078A RID: 1930
public class SpringPad : MonoBehaviour, ControllerCollisionListener
{
	// Token: 0x06002848 RID: 10312 RVA: 0x00098E73 File Offset: 0x00097073
	public void Awake()
	{
		this.springAnimId = Animator.StringToHash("spring");
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.waiter = base.GetComponentInParent<WaitForChargeup>();
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x00098EA4 File Offset: 0x000970A4
	public void OnControllerCollision(GameObject gameObj)
	{
		if (this.waiter.IsWaiting())
		{
			return;
		}
		vp_FPController component = gameObj.GetComponent<vp_FPController>();
		if (component != null && this.timeDir.HasReached(this.nextPlayerHitTime))
		{
			component.AddSoftForce(SpringPad.UP_PLAYER_FORCE, 5f);
			this.anim.SetTrigger(this.springAnimId);
			this.nextPlayerHitTime = this.timeDir.HoursFromNow(0.016666668f);
		}
	}

	// Token: 0x0600284A RID: 10314 RVA: 0x00098F1C File Offset: 0x0009711C
	public void OnCollisionEnter(Collision col)
	{
		if (this.waiter.IsWaiting())
		{
			return;
		}
		if (col.gameObject.layer == 16)
		{
			return;
		}
		Rigidbody rigidbody = col.rigidbody;
		if (rigidbody != null)
		{
			rigidbody.AddForce(SpringPad.UP_ACCEL, ForceMode.VelocityChange);
			this.anim.SetTrigger(this.springAnimId);
		}
	}

	// Token: 0x040027EB RID: 10219
	private static Vector3 UP_ACCEL = Vector3.up * 5f;

	// Token: 0x040027EC RID: 10220
	private static Vector3 UP_PLAYER_FORCE = Vector3.up * 1.667f;

	// Token: 0x040027ED RID: 10221
	public Animator anim;

	// Token: 0x040027EE RID: 10222
	private int springAnimId;

	// Token: 0x040027EF RID: 10223
	private double nextPlayerHitTime;

	// Token: 0x040027F0 RID: 10224
	private TimeDirector timeDir;

	// Token: 0x040027F1 RID: 10225
	private WaitForChargeup waiter;
}
