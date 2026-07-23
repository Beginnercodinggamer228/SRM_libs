using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000629 RID: 1577
[RequireComponent(typeof(ToggleGroup))]
public class TabByMenuKeys : MonoBehaviour
{
	// Token: 0x06002116 RID: 8470 RVA: 0x0007E74A File Offset: 0x0007C94A
	public void Awake()
	{
		this.tabs = base.GetComponentsInChildren<Toggle>(false);
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x0007E74A File Offset: 0x0007C94A
	public void Start()
	{
		this.tabs = base.GetComponentsInChildren<Toggle>(false);
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x0007E759 File Offset: 0x0007C959
	public void Update()
	{
		if (TabByMenuKeys.disabledForBinding)
		{
			return;
		}
		if (SRInput.PauseActions.menuTabRight.WasPressed)
		{
			this.SelectNextTab();
			return;
		}
		if (SRInput.PauseActions.menuTabLeft.WasPressed)
		{
			this.SelectPrevTab();
		}
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x0007E792 File Offset: 0x0007C992
	public void SelectNextTab()
	{
		this.currIdx = Math.Min(this.currIdx + 1, this.tabs.Length - 1);
		this.tabs[this.currIdx].isOn = true;
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x0007E7C4 File Offset: 0x0007C9C4
	public void SelectPrevTab()
	{
		this.currIdx = Math.Max(this.currIdx - 1, 0);
		this.tabs[this.currIdx].isOn = true;
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x0007E7F0 File Offset: 0x0007C9F0
	public void RecalcSelected()
	{
		if (this.tabs == null)
		{
			return;
		}
		for (int i = 0; i < this.tabs.Length; i++)
		{
			if (this.tabs[i].isOn)
			{
				this.currIdx = i;
				return;
			}
		}
	}

	// Token: 0x04002074 RID: 8308
	public static bool disabledForBinding;

	// Token: 0x04002075 RID: 8309
	private Toggle[] tabs;

	// Token: 0x04002076 RID: 8310
	private int currIdx;
}
