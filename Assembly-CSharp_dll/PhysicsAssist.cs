using System;
using UnityEngine;

// Token: 0x02000747 RID: 1863
public class PhysicsAssist : MonoBehaviour
{
	// Token: 0x060026EE RID: 9966 RVA: 0x00094317 File Offset: 0x00092517
	public void OnCollisionEnter(Collision col)
	{
		col.rigidbody.AddForce(Vector3.down * this.assistAmount, ForceMode.VelocityChange);
	}

	// Token: 0x04002697 RID: 9879
	public float assistAmount = 5f;
}
