using System;
using UnityEngine;

// Token: 0x0200041E RID: 1054
public class PollenCloudDestructor : RegisteredActorBehaviour, RegistryUpdateable
{
	// Token: 0x060015F9 RID: 5625 RVA: 0x0005549E File Offset: 0x0005369E
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.dieAtTime = this.timeDir.HoursFromNow(this.gameHrsToLive);
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x000554C8 File Offset: 0x000536C8
	public void RegistryUpdate()
	{
		if (this.timeDir.HasReached(this.dieAtTime) || this.timeDir.HasReached(this.contactDeathTime))
		{
			Destroyer.DestroyActor(base.gameObject, "PollenCloudDestructor.RegistryUpdate", false);
			SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x0005552E File Offset: 0x0005372E
	public void AddContact()
	{
		if (this.contacts == 0)
		{
			this.contactDeathTime = this.timeDir.HoursFromNow(this.gameHrsInContactBeforeDeath);
		}
		this.contacts++;
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x0005555D File Offset: 0x0005375D
	public void RemoveContact()
	{
		this.contacts--;
		if (this.contacts <= 0)
		{
			this.contactDeathTime = double.PositiveInfinity;
		}
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x00055585 File Offset: 0x00053785
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x040014F1 RID: 5361
	public float gameHrsToLive = 0.5f;

	// Token: 0x040014F2 RID: 5362
	public float gameHrsInContactBeforeDeath = 0.05f;

	// Token: 0x040014F3 RID: 5363
	public GameObject destroyFX;

	// Token: 0x040014F4 RID: 5364
	private double dieAtTime;

	// Token: 0x040014F5 RID: 5365
	private double contactDeathTime = double.PositiveInfinity;

	// Token: 0x040014F6 RID: 5366
	private int contacts;

	// Token: 0x040014F7 RID: 5367
	private TimeDirector timeDir;
}
