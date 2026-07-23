using System;
using UnityEngine;

// Token: 0x020006DA RID: 1754
[RequireComponent(typeof(Rigidbody))]
public class ContinualRotation : MonoBehaviour
{
	// Token: 0x0600249F RID: 9375 RVA: 0x0008D220 File Offset: 0x0008B420
	public void Start()
	{
		this.ownRigidbody = base.GetComponent<Rigidbody>();
		this.ownRigidbody.isKinematic = true;
		this.ownRigidbody.useGravity = false;
		this.angularVel = new Vector3(0f, 360f / this.secsPerRotate, 0f);
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x0008D274 File Offset: 0x0008B474
	public void FixedUpdate()
	{
		this.ownRigidbody.MoveRotation(Quaternion.Euler(this.ownRigidbody.rotation.eulerAngles + this.angularVel * Time.fixedDeltaTime));
	}

	// Token: 0x0400238F RID: 9103
	public float secsPerRotate = 10f;

	// Token: 0x04002390 RID: 9104
	private Vector3 angularVel;

	// Token: 0x04002391 RID: 9105
	private Rigidbody ownRigidbody;
}
