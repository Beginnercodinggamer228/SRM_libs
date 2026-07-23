using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020005E0 RID: 1504
public class PerformWhileMouseDown : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	// Token: 0x06001FA0 RID: 8096 RVA: 0x000786F6 File Offset: 0x000768F6
	public void Update()
	{
		if (this.isMouseDown)
		{
			this.WhileMouseIsDown();
		}
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x0007870B File Offset: 0x0007690B
	public void OnPointerDown(PointerEventData eventData)
	{
		this.isMouseDown = true;
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x00078714 File Offset: 0x00076914
	public void OnPointerUp(PointerEventData eventData)
	{
		this.isMouseDown = false;
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x0007871D File Offset: 0x0007691D
	public void OnDestroy()
	{
		this.WhileMouseIsDown = null;
	}

	// Token: 0x04001ED5 RID: 7893
	private bool isMouseDown;

	// Token: 0x04001ED6 RID: 7894
	public PerformWhileMouseDown.MouseIsDownEvent WhileMouseIsDown;

	// Token: 0x020005E1 RID: 1505
	// (Invoke) Token: 0x06001FA6 RID: 8102
	public delegate void MouseIsDownEvent();
}
