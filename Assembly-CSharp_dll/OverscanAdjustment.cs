using System;
using UnityEngine;

// Token: 0x020005D6 RID: 1494
public class OverscanAdjustment : MonoBehaviour
{
	// Token: 0x06001F56 RID: 8022 RVA: 0x00077509 File Offset: 0x00075709
	public void Awake()
	{
		this.options = SRSingleton<GameContext>.Instance.OptionsDirector;
		this.rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x00077527 File Offset: 0x00075727
	public void Update()
	{
		this.rectTransform.localScale = Vector3.one - Vector3.one * this.options.GetOverscanAdjustment();
	}

	// Token: 0x04001E88 RID: 7816
	private OptionsDirector options;

	// Token: 0x04001E89 RID: 7817
	private RectTransform rectTransform;
}
