using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003AB RID: 939
public class CollisionAggregator : MonoBehaviour
{
	// Token: 0x0600139B RID: 5019 RVA: 0x0004C2AF File Offset: 0x0004A4AF
	public void Register(CollidableActorBehaviour collidableBehaviour)
	{
		if (collidableBehaviour is Collidable)
		{
			this.Register(collidableBehaviour as Collidable);
		}
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x0004C2C5 File Offset: 0x0004A4C5
	private void Register(Collidable collidableBehaviour)
	{
		if (!this.collidableBehaviours.Contains(collidableBehaviour))
		{
			this.collidableBehaviours.Add(collidableBehaviour);
		}
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x0004C2E1 File Offset: 0x0004A4E1
	public void Deregister(CollidableActorBehaviour collidableBehaviour)
	{
		if (collidableBehaviour is Collidable)
		{
			this.Deregister(collidableBehaviour as Collidable);
		}
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x0004C2F7 File Offset: 0x0004A4F7
	private void Deregister(Collidable collidableBehaviour)
	{
		this.collidableBehaviours.Remove(collidableBehaviour);
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x0004C308 File Offset: 0x0004A508
	public void OnCollisionEnter(Collision col)
	{
		foreach (Collidable collidable in this.collidableBehaviours)
		{
			if (collidable != null)
			{
				collidable.ProcessCollisionEnter(col);
			}
		}
	}

	// Token: 0x04001258 RID: 4696
	private List<Collidable> collidableBehaviours = new List<Collidable>();
}
