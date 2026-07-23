using System;
using UnityEngine.UI;

// Token: 0x0200037E RID: 894
public class SRToggleGroup : ToggleGroup
{
	// Token: 0x06001271 RID: 4721 RVA: 0x000493DB File Offset: 0x000475DB
	protected override void Awake()
	{
		base.Awake();
		this.slime1992_requiresHotfix = !base.allowSwitchOff;
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x000493F2 File Offset: 0x000475F2
	public void OnToggleEnable(SRToggle instance)
	{
		if (this.slime1992_requiresHotfix && instance == this.slime1992_previousEnabled)
		{
			this.slime1992_previousEnabled.SetIsOnWithoutNotify(true);
			this.slime1992_previousEnabled = null;
			base.allowSwitchOff = false;
		}
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x00049424 File Offset: 0x00047624
	public void OnToggleWillDisable(SRToggle instance)
	{
		if (this.slime1992_requiresHotfix && instance != null && instance.isOn)
		{
			base.allowSwitchOff = true;
			this.slime1992_previousEnabled = instance;
			this.slime1992_previousEnabled.SetIsOnWithoutNotify(false);
		}
	}

	// Token: 0x040011A2 RID: 4514
	private SRToggle slime1992_previousEnabled;

	// Token: 0x040011A3 RID: 4515
	private bool slime1992_requiresHotfix;
}
