using System;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class ActivateOnProgressRange : MonoBehaviour
{
	// Token: 0x0600101F RID: 4127 RVA: 0x00040C0C File Offset: 0x0003EE0C
	public void Start()
	{
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		ProgressDirector progressDirector = this.progressDir;
		progressDirector.onProgressChanged = (ProgressDirector.OnProgressChanged)Delegate.Combine(progressDirector.onProgressChanged, new ProgressDirector.OnProgressChanged(this.OnProgressChanged));
		this.OnProgressChanged();
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x00040C4B File Offset: 0x0003EE4B
	public void OnDestroy()
	{
		if (this.progressDir != null)
		{
			ProgressDirector progressDirector = this.progressDir;
			progressDirector.onProgressChanged = (ProgressDirector.OnProgressChanged)Delegate.Remove(progressDirector.onProgressChanged, new ProgressDirector.OnProgressChanged(this.OnProgressChanged));
		}
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x00040C84 File Offset: 0x0003EE84
	private void OnProgressChanged()
	{
		int progress = this.progressDir.GetProgress(this.progressType);
		base.gameObject.SetActive(this.minProgress <= progress && progress <= this.maxProgress);
	}

	// Token: 0x04000EF1 RID: 3825
	public ProgressDirector.ProgressType progressType = ProgressDirector.ProgressType.CORPORATE_PARTNER;

	// Token: 0x04000EF2 RID: 3826
	public int minProgress = int.MinValue;

	// Token: 0x04000EF3 RID: 3827
	public int maxProgress = int.MaxValue;

	// Token: 0x04000EF4 RID: 3828
	private ProgressDirector progressDir;
}
