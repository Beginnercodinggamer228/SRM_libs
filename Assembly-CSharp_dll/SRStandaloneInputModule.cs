using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200060F RID: 1551
public class SRStandaloneInputModule : PointerInputModule
{
	// Token: 0x06002082 RID: 8322 RVA: 0x0007C2B0 File Offset: 0x0007A4B0
	protected SRStandaloneInputModule()
	{
	}

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x06002083 RID: 8323 RVA: 0x0007C2D5 File Offset: 0x0007A4D5
	// (set) Token: 0x06002084 RID: 8324 RVA: 0x0007C2DD File Offset: 0x0007A4DD
	public bool allowActivationOnMobileDevice
	{
		get
		{
			return this.m_AllowActivationOnMobileDevice;
		}
		set
		{
			this.m_AllowActivationOnMobileDevice = value;
		}
	}

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x06002085 RID: 8325 RVA: 0x0007C2E6 File Offset: 0x0007A4E6
	// (set) Token: 0x06002086 RID: 8326 RVA: 0x0007C2EE File Offset: 0x0007A4EE
	public bool processMouseEvents
	{
		get
		{
			return this.m_ProcessMouseEvents;
		}
		set
		{
			this.m_ProcessMouseEvents = value;
		}
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x06002087 RID: 8327 RVA: 0x0007C2F7 File Offset: 0x0007A4F7
	// (set) Token: 0x06002088 RID: 8328 RVA: 0x0007C2FF File Offset: 0x0007A4FF
	public float inputActionsPerSecond
	{
		get
		{
			return this.m_InputActionsPerSecond;
		}
		set
		{
			this.m_InputActionsPerSecond = value;
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x06002089 RID: 8329 RVA: 0x0007C308 File Offset: 0x0007A508
	// (set) Token: 0x0600208A RID: 8330 RVA: 0x0007C310 File Offset: 0x0007A510
	public float repeatDelay
	{
		get
		{
			return this.m_repeatDelay;
		}
		set
		{
			this.m_repeatDelay = value;
		}
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x0007C31C File Offset: 0x0007A51C
	public override void UpdateModule()
	{
		this.m_LastMousePosition = this.m_MousePosition;
		this.m_MousePosition = Input.mousePosition;
		if (base.eventSystem.currentSelectedGameObject == null && this.lastNavigatedViaButtons)
		{
			if (!(this.lastSelectedViaButtons == null) && this.lastSelectedViaButtons.activeInHierarchy)
			{
				base.eventSystem.SetSelectedGameObject(this.lastSelectedViaButtons, this.GetBaseEventData());
				return;
			}
			InitSelected current = InitSelected.Current;
			if (current != null)
			{
				base.eventSystem.SetSelectedGameObject(current.gameObject, this.GetBaseEventData());
				return;
			}
		}
		else if (base.eventSystem.currentSelectedGameObject != null && base.eventSystem.currentSelectedGameObject.GetComponent<InputField>() == null && !this.lastNavigatedViaButtons)
		{
			this.lastSelectedViaButtons = base.eventSystem.currentSelectedGameObject;
			base.eventSystem.SetSelectedGameObject(null, this.GetBaseEventData());
		}
	}

	// Token: 0x0600208C RID: 8332 RVA: 0x0007C413 File Offset: 0x0007A613
	public override bool IsModuleSupported()
	{
		return this.m_AllowActivationOnMobileDevice || Input.mousePresent;
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x0007C424 File Offset: 0x0007A624
	public override bool ShouldActivateModule()
	{
		return base.ShouldActivateModule() && (SRInput.PauseActions.submit.WasReleased | SRInput.PauseActions.cancel.WasReleased | SRInput.PauseActions.menuUp.WasReleased | SRInput.PauseActions.menuDown.WasReleased | SRInput.PauseActions.menuLeft.WasReleased | SRInput.PauseActions.menuRight.WasReleased | (this.m_MousePosition - this.m_LastMousePosition).sqrMagnitude > 0f | Input.GetMouseButtonDown(0));
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x0007C4C4 File Offset: 0x0007A6C4
	public override void ActivateModule()
	{
		base.ActivateModule();
		this.m_MousePosition = Input.mousePosition;
		this.m_LastMousePosition = Input.mousePosition;
		this.lastNavigatedViaButtons = InputDirector.UsingGamepad();
		if (this.lastNavigatedViaButtons)
		{
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, this.GetBaseEventData());
		}
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x0007C53D File Offset: 0x0007A73D
	public override void DeactivateModule()
	{
		base.DeactivateModule();
		base.ClearSelection();
	}

	// Token: 0x06002090 RID: 8336 RVA: 0x0007C54C File Offset: 0x0007A74C
	public override void Process()
	{
		bool flag = this.SendUpdateEventToSelectedObject();
		if (base.eventSystem.sendNavigationEvents)
		{
			if (!flag)
			{
				flag |= this.SendMoveEventToSelectedObject();
			}
			if (!flag)
			{
				this.SendSubmitEventToSelectedObject();
			}
		}
		if (this.m_ProcessMouseEvents && Cursor.visible)
		{
			this.ProcessMouseEvent();
		}
	}

	// Token: 0x06002091 RID: 8337 RVA: 0x0007C598 File Offset: 0x0007A798
	private bool SendSubmitEventToSelectedObject()
	{
		if (base.eventSystem.currentSelectedGameObject == null)
		{
			return false;
		}
		BaseEventData baseEventData = this.GetBaseEventData();
		if (SRInput.PauseActions.submit.WasReleased)
		{
			ExecuteEvents.Execute<ISubmitHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
		}
		if (SRInput.PauseActions.cancel.WasReleased)
		{
			ExecuteEvents.Execute<ICancelHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
		}
		return baseEventData.used;
	}

	// Token: 0x06002092 RID: 8338 RVA: 0x0007C618 File Offset: 0x0007A818
	private bool SendMoveEventToSelectedObject()
	{
		float unscaledTime = Time.unscaledTime;
		Vector2 vector = new Vector2(SRInput.PauseActions.menuRight.RawValue - SRInput.PauseActions.menuLeft.RawValue, SRInput.PauseActions.menuUp.RawValue - SRInput.PauseActions.menuDown.RawValue);
		if (Mathf.Approximately(vector.x, 0f) && Mathf.Approximately(vector.y, 0f))
		{
			this.m_ConsecutiveMovementCount = 0;
			return false;
		}
		bool flag = Vector2.Dot(vector, this.m_LastMoveVector) > 0f;
		if (flag && this.m_ConsecutiveMovementCount == 1)
		{
			if (unscaledTime <= this.m_PrevActionTime + this.m_repeatDelay)
			{
				return false;
			}
		}
		else if (unscaledTime <= this.m_PrevActionTime + 1f / this.m_InputActionsPerSecond)
		{
			return false;
		}
		AxisEventData axisEventData = this.GetAxisEventData(vector.x, vector.y, 0.5f);
		if (vector.sqrMagnitude > 0.25f)
		{
			ExecuteEvents.Execute<IMoveHandler>(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			if (!flag)
			{
				this.m_ConsecutiveMovementCount = 0;
			}
			this.m_ConsecutiveMovementCount++;
			this.m_PrevActionTime = unscaledTime;
			this.m_LastMoveVector = vector;
			this.lastNavigatedViaButtons = true;
			if (axisEventData.moveDir == MoveDirection.None)
			{
				this.m_ConsecutiveMovementCount = 0;
			}
			return true;
		}
		if (axisEventData.moveDir == MoveDirection.None)
		{
			this.m_ConsecutiveMovementCount = 0;
		}
		return false;
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x0007C774 File Offset: 0x0007A974
	protected void ProcessMouseEvent()
	{
		this.ProcessMouseEvent(0);
	}

	// Token: 0x06002094 RID: 8340 RVA: 0x0007C780 File Offset: 0x0007A980
	private void ProcessMouseEvent(int id)
	{
		PointerInputModule.MouseState mousePointerEventData = this.GetMousePointerEventData(id);
		bool pressed = mousePointerEventData.AnyPressesThisFrame();
		bool released = mousePointerEventData.AnyReleasesThisFrame();
		PointerInputModule.MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
		if (!SRStandaloneInputModule.UseMouse(pressed, released, eventData.buttonData))
		{
			return;
		}
		if (Cursor.visible)
		{
			this.lastNavigatedViaButtons = false;
		}
		this.ProcessMousePress(eventData);
		this.ProcessMove(eventData.buttonData);
		this.ProcessDrag(eventData.buttonData);
		this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
		this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
		this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
		this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
		if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
		{
			ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), eventData.buttonData, ExecuteEvents.scrollHandler);
		}
	}

	// Token: 0x06002095 RID: 8341 RVA: 0x0007C886 File Offset: 0x0007AA86
	private static bool UseMouse(bool pressed, bool released, PointerEventData pointerData)
	{
		return pressed || released || pointerData.IsPointerMoving() || pointerData.IsScrolling();
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x0007C8A0 File Offset: 0x0007AAA0
	private bool SendUpdateEventToSelectedObject()
	{
		if (base.eventSystem.currentSelectedGameObject == null)
		{
			return false;
		}
		BaseEventData baseEventData = this.GetBaseEventData();
		ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
		return baseEventData.used;
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x0007C8E8 File Offset: 0x0007AAE8
	private void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
	{
		PointerEventData buttonData = data.buttonData;
		GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
		if (data.PressedThisFrame())
		{
			buttonData.eligibleForClick = true;
			buttonData.delta = Vector2.zero;
			buttonData.dragging = false;
			buttonData.useDragThreshold = true;
			buttonData.pressPosition = buttonData.position;
			buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
			base.DeselectIfSelectionChanged(gameObject, buttonData);
			GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
			if (gameObject2 == null)
			{
				gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			}
			float unscaledTime = Time.unscaledTime;
			if (gameObject2 == buttonData.lastPress)
			{
				if (unscaledTime - buttonData.clickTime < 0.3f)
				{
					PointerEventData pointerEventData = buttonData;
					int clickCount = pointerEventData.clickCount + 1;
					pointerEventData.clickCount = clickCount;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.clickTime = unscaledTime;
			}
			else
			{
				buttonData.clickCount = 1;
			}
			buttonData.pointerPress = gameObject2;
			buttonData.rawPointerPress = gameObject;
			buttonData.clickTime = unscaledTime;
			buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
			if (buttonData.pointerDrag != null)
			{
				ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
			}
		}
		if (data.ReleasedThisFrame())
		{
			ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
			GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
			{
				ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
			}
			else if (buttonData.pointerDrag != null)
			{
				ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, buttonData, ExecuteEvents.dropHandler);
			}
			buttonData.eligibleForClick = false;
			buttonData.pointerPress = null;
			buttonData.rawPointerPress = null;
			if (buttonData.pointerDrag != null && buttonData.dragging)
			{
				ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
			}
			buttonData.dragging = false;
			buttonData.pointerDrag = null;
			if (gameObject != buttonData.pointerEnter)
			{
				base.HandlePointerExitAndEnter(buttonData, null);
				base.HandlePointerExitAndEnter(buttonData, gameObject);
			}
		}
	}

	// Token: 0x04001FEA RID: 8170
	private float m_NextAction;

	// Token: 0x04001FEB RID: 8171
	private Vector2 m_LastMousePosition;

	// Token: 0x04001FEC RID: 8172
	private Vector2 m_MousePosition;

	// Token: 0x04001FED RID: 8173
	[SerializeField]
	private float m_InputActionsPerSecond = 10f;

	// Token: 0x04001FEE RID: 8174
	[SerializeField]
	private bool m_AllowActivationOnMobileDevice;

	// Token: 0x04001FEF RID: 8175
	private bool lastNavigatedViaButtons;

	// Token: 0x04001FF0 RID: 8176
	private GameObject lastSelectedViaButtons;

	// Token: 0x04001FF1 RID: 8177
	private bool m_ProcessMouseEvents = true;

	// Token: 0x04001FF2 RID: 8178
	[SerializeField]
	private float m_repeatDelay = 0.5f;

	// Token: 0x04001FF3 RID: 8179
	private int m_ConsecutiveMovementCount;

	// Token: 0x04001FF4 RID: 8180
	private Vector2 m_LastMoveVector;

	// Token: 0x04001FF5 RID: 8181
	private float m_PrevActionTime;
}
