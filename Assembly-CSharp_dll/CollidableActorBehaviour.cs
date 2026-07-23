using System;

// Token: 0x020000E8 RID: 232
public class CollidableActorBehaviour : RegisteredActorBehaviour
{
	// Token: 0x06000558 RID: 1368 RVA: 0x00020597 File Offset: 0x0001E797
	public virtual void Awake()
	{
		this.collisionBehaviour = base.GetComponent<CollisionAggregator>();
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x000205A5 File Offset: 0x0001E7A5
	public override void Start()
	{
		base.Start();
		if (this.collisionBehaviour != null && base.enabled)
		{
			this.collisionBehaviour.Register(this);
		}
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x000205CF File Offset: 0x0001E7CF
	public override void OnEnable()
	{
		if (this.collisionBehaviour != null)
		{
			this.collisionBehaviour.Register(this);
		}
		base.OnEnable();
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x000205F1 File Offset: 0x0001E7F1
	public override void OnDisable()
	{
		if (this.collisionBehaviour != null)
		{
			this.collisionBehaviour.Deregister(this);
		}
		base.OnDisable();
	}

	// Token: 0x0400055F RID: 1375
	private CollisionAggregator collisionBehaviour;
}
