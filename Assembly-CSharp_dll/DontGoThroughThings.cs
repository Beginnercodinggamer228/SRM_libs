using System;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class DontGoThroughThings : MonoBehaviour
{
	// Token: 0x06000637 RID: 1591 RVA: 0x000226D4 File Offset: 0x000208D4
	public void Awake()
	{
		this.myRigidbody = base.GetComponent<Rigidbody>();
		this.myRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x000226F0 File Offset: 0x000208F0
	public void FixedUpdate()
	{
		if (this.allowDestroy && this.myRigidbody.velocity.sqrMagnitude < 1f)
		{
			this.myRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			Destroyer.Destroy(this, "DontGoThroughThings.FixedUpdate");
			return;
		}
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x00022737 File Offset: 0x00020937
	public void AllowDestroy()
	{
		this.allowDestroy = true;
	}

	// Token: 0x040005F2 RID: 1522
	private Rigidbody myRigidbody;

	// Token: 0x040005F3 RID: 1523
	private bool allowDestroy;

	// Token: 0x040005F4 RID: 1524
	private const float MIN_VELOCITY = 1f;

	// Token: 0x040005F5 RID: 1525
	private const float MIN_VELOCITY_SQR = 1f;
}
