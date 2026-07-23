using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020005EF RID: 1519
[RequireComponent(typeof(Image))]
public class PressButtonOnAction : MonoBehaviour
{
	// Token: 0x06001FE1 RID: 8161 RVA: 0x0007970D File Offset: 0x0007790D
	public void Start()
	{
		this.button = base.GetComponentsInParent<Button>(true)[0];
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x00079720 File Offset: 0x00077920
	public void Update()
	{
		if (!this.isPressed && SRInput.GetAction(this.action).WasPressed && this.IsButtonAvailable())
		{
			ExecuteEvents.Execute<IPointerEnterHandler>(this.button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
			ExecuteEvents.Execute<IPointerDownHandler>(this.button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
			this.isPressed = true;
			return;
		}
		if (this.isPressed && SRInput.GetAction(this.action).WasReleased && this.IsButtonAvailable())
		{
			ExecuteEvents.Execute<IPointerExitHandler>(this.button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
			ExecuteEvents.Execute<IPointerUpHandler>(this.button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
			ExecuteEvents.Execute<IPointerClickHandler>(this.button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
			this.isPressed = false;
			return;
		}
		if (this.isPressed && !this.IsButtonAvailable())
		{
			ExecuteEvents.Execute<IPointerExitHandler>(this.button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
			this.isPressed = false;
		}
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x0007985B File Offset: 0x00077A5B
	private bool IsButtonAvailable()
	{
		return this.button.IsInteractable() && this.button.isActiveAndEnabled && (!this.requiresCurrentSelection || EventSystem.current.currentSelectedGameObject == base.gameObject);
	}

	// Token: 0x04001F0A RID: 7946
	public string action;

	// Token: 0x04001F0B RID: 7947
	[Tooltip("If true, the mouse up/down events will not trigger unless the button is currently selected.")]
	public bool requiresCurrentSelection;

	// Token: 0x04001F0C RID: 7948
	private Button button;

	// Token: 0x04001F0D RID: 7949
	private bool isPressed;
}
