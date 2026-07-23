using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200056F RID: 1391
public class DropdownScroller : MonoBehaviour
{
	// Token: 0x06001CF4 RID: 7412 RVA: 0x0006E0EB File Offset: 0x0006C2EB
	public void Awake()
	{
		this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.UpdateScrollPosition));
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x0006E109 File Offset: 0x0006C309
	public void Start()
	{
		this.scrollRect.verticalNormalizedPosition = this.scrollPosition;
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x0006E11C File Offset: 0x0006C31C
	public void OnDestroy()
	{
		this.dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.UpdateScrollPosition));
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x0006E13A File Offset: 0x0006C33A
	private void UpdateScrollPosition(int index)
	{
		this.scrollPosition = 1f - 1f * (float)index / (float)this.dropdown.options.Count;
	}

	// Token: 0x04001C09 RID: 7177
	public Dropdown dropdown;

	// Token: 0x04001C0A RID: 7178
	public ScrollRect scrollRect;

	// Token: 0x04001C0B RID: 7179
	[SerializeField]
	private float scrollPosition = 1f;
}
