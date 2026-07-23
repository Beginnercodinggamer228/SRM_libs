using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000722 RID: 1826
public abstract class IdHandler<M> : IdHandler, IdHandlerModel.Participant where M : IdHandlerModel
{
	// Token: 0x06002628 RID: 9768 RVA: 0x00092410 File Offset: 0x00090610
	public void InitModel(IdHandlerModel model)
	{
		M model2 = model as M;
		this.InitModel(model2);
	}

	// Token: 0x06002629 RID: 9769 RVA: 0x00092430 File Offset: 0x00090630
	public void SetModel(IdHandlerModel model)
	{
		M model2 = model as M;
		this.SetModel(model2);
	}

	// Token: 0x0600262A RID: 9770 RVA: 0x00092450 File Offset: 0x00090650
	public string GetId()
	{
		return base.id;
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x00092458 File Offset: 0x00090658
	public virtual void Awake()
	{
		if (Application.isPlaying && SRSingleton<SceneContext>.Instance != null)
		{
			this.unregistrant = this.Register(SRSingleton<SceneContext>.Instance.GameModel);
		}
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x00092484 File Offset: 0x00090684
	public virtual void OnDestroy()
	{
		if (this.unregistrant != null)
		{
			this.unregistrant();
			this.unregistrant = null;
		}
	}

	// Token: 0x0600262D RID: 9773
	protected abstract GameModel.Unregistrant Register(GameModel game);

	// Token: 0x0600262E RID: 9774
	protected abstract void InitModel(M model);

	// Token: 0x0600262F RID: 9775
	protected abstract void SetModel(M model);

	// Token: 0x04002589 RID: 9609
	private GameModel.Unregistrant unregistrant;
}
