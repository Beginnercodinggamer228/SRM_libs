using System;
using UnityEngine;

// Token: 0x02000296 RID: 662
public abstract class ControllerCollisionListenerBehaviour : SRBehaviour, ControllerCollisionListener
{
	// Token: 0x06000DEA RID: 3562 RVA: 0x00038908 File Offset: 0x00036B08
	public void OnControllerCollision(GameObject collision)
	{
		this.isControllerColliding |= this.Predicate(collision);
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x0003891E File Offset: 0x00036B1E
	public void LateUpdate()
	{
		if (this.wasControllerColliding != this.isControllerColliding)
		{
			if (this.isControllerColliding)
			{
				this.OnControllerCollisionEntered();
			}
			else
			{
				this.OnControllerCollisionExited();
			}
		}
		this.wasControllerColliding = this.isControllerColliding;
		this.isControllerColliding = false;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnControllerCollisionEntered()
	{
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnControllerCollisionExited()
	{
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected virtual bool Predicate(GameObject collision)
	{
		return true;
	}

	// Token: 0x04000D19 RID: 3353
	private bool isControllerColliding;

	// Token: 0x04000D1A RID: 3354
	private bool wasControllerColliding;
}
