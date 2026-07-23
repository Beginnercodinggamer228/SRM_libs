using System;
using UnityEngine;

// Token: 0x020003AC RID: 940
public interface Collidable
{
	// Token: 0x060013A1 RID: 5025
	void ProcessCollisionEnter(Collision col);

	// Token: 0x060013A2 RID: 5026
	void ProcessCollisionExit(Collision col);
}
