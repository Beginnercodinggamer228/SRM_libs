using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020005CE RID: 1486
public class OnSelectDelegator : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	// Token: 0x06001EFB RID: 7931 RVA: 0x000759F3 File Offset: 0x00073BF3
	public void SetDelegate(UnityAction onSelectDel)
	{
		this.onSelectDel = onSelectDel;
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x000759FC File Offset: 0x00073BFC
	public void OnSelect(BaseEventData data)
	{
		this.onSelectDel();
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x00075A09 File Offset: 0x00073C09
	public static OnSelectDelegator Create(GameObject obj, UnityAction onSelectDel)
	{
		OnSelectDelegator onSelectDelegator = obj.AddComponent<OnSelectDelegator>();
		onSelectDelegator.SetDelegate(onSelectDel);
		return onSelectDelegator;
	}

	// Token: 0x04001E1D RID: 7709
	private UnityAction onSelectDel;
}
