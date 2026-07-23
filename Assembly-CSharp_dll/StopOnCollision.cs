using System;
using UnityEngine;

// Token: 0x0200051D RID: 1309
public class StopOnCollision : CollidableActorBehaviour, Collidable
{
	// Token: 0x06001B4C RID: 6988 RVA: 0x00068C28 File Offset: 0x00066E28
	public void ProcessCollisionEnter(Collision col)
	{
		Vector3 position = col.contacts[0].point + col.contacts[0].normal * this.distFromCol;
		base.GetComponent<Rigidbody>().position = position;
		base.GetComponent<Rigidbody>().velocity = Vector3.zero;
		base.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	}

	// Token: 0x06001B4D RID: 6989 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x04001AAB RID: 6827
	public float distFromCol = 0.25f;
}
