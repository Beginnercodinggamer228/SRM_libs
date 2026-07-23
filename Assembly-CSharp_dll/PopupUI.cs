using System;

// Token: 0x020005EE RID: 1518
public abstract class PopupUI<T> : SRBehaviour
{
	// Token: 0x06001FDD RID: 8157 RVA: 0x000796BD File Offset: 0x000778BD
	public virtual void Init(T idEntry)
	{
		this.idEntry = idEntry;
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundleAvailable));
	}

	// Token: 0x06001FDE RID: 8158
	public abstract void OnBundleAvailable(MessageDirector msgDir);

	// Token: 0x06001FDF RID: 8159 RVA: 0x000796E2 File Offset: 0x000778E2
	public virtual void OnDestroy()
	{
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundleAvailable));
		}
	}

	// Token: 0x04001F09 RID: 7945
	protected T idEntry;
}
