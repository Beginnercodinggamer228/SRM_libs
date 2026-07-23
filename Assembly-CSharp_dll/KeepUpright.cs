using System;
using UnityEngine;

// Token: 0x02000402 RID: 1026
public class KeepUpright : RegisteredActorBehaviour, RegistryFixedUpdateable
{
	// Token: 0x0600156D RID: 5485 RVA: 0x00053508 File Offset: 0x00051708
	public override void Start()
	{
		base.Start();
		this.body = base.GetComponent<Rigidbody>();
		this.speedFactor = 57.29578f * this.stability / this.speed;
		this.momentum = this.speed * this.speed * this.body.mass;
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x0005355F File Offset: 0x0005175F
	public virtual void RegistryFixedUpdate()
	{
		this.DoUpright(Vector3.up);
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x0005356C File Offset: 0x0005176C
	protected void DoUpright(Vector3 desiredUp)
	{
		if (this.body != null)
		{
			Vector3 angularVelocity = this.body.angularVelocity;
			Vector3 a = Vector3.Cross(Quaternion.AngleAxis(angularVelocity.magnitude * this.speedFactor, angularVelocity) * base.transform.up, desiredUp);
			this.body.AddTorque(a * this.momentum);
		}
	}

	// Token: 0x04001461 RID: 5217
	public float stability = 0.3f;

	// Token: 0x04001462 RID: 5218
	public float speed = 2f;

	// Token: 0x04001463 RID: 5219
	private Rigidbody body;

	// Token: 0x04001464 RID: 5220
	private float speedFactor;

	// Token: 0x04001465 RID: 5221
	private float momentum;
}
