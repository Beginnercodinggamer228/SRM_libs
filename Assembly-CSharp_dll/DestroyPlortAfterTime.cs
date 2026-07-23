using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class DestroyPlortAfterTime : RegisteredActorBehaviour, RegistryUpdateable, ActorModel.Participant
{
	// Token: 0x06000627 RID: 1575 RVA: 0x00022428 File Offset: 0x00020628
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x0002243A File Offset: 0x0002063A
	public void InitModel(ActorModel model)
	{
		((PlortModel)model).destroyTime = this.timeDir.HoursFromNowOrStart(this.lifeTimeHours);
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00022458 File Offset: 0x00020658
	public void SetModel(ActorModel model)
	{
		this.plortModel = (PlortModel)model;
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x00022468 File Offset: 0x00020668
	public void RegistryUpdate()
	{
		if (this.timeDir.HasReached(this.plortModel.destroyTime) && !this.destroying)
		{
			this.destroying = true;
			bool flag = this.timeDir.HasReached(this.plortModel.destroyTime + 3600.0);
			DestroyAfterTimeListener component = base.GetComponent<DestroyAfterTimeListener>();
			if (component != null)
			{
				component.WillDestroyAfterTime();
			}
			if (flag)
			{
				this.DoDestroy("DestroyAfterTime.RegistryUpdate (skippedFX)");
				return;
			}
			this.DoDestroy("DestroyAfterTime.RegistryUpdate");
			if (this.destroyFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, Quaternion.identity);
			}
		}
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x00022512 File Offset: 0x00020712
	private void DoDestroy(string reason)
	{
		Destroyer.DestroyActor(base.gameObject, reason, false);
	}

	// Token: 0x040005E9 RID: 1513
	public float lifeTimeHours = 24f;

	// Token: 0x040005EA RID: 1514
	public GameObject destroyFX;

	// Token: 0x040005EB RID: 1515
	private TimeDirector timeDir;

	// Token: 0x040005EC RID: 1516
	private bool destroying;

	// Token: 0x040005ED RID: 1517
	private PlortModel plortModel;
}
