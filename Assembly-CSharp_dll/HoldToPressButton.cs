using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000599 RID: 1433
public class HoldToPressButton : Button, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
{
	// Token: 0x06001DC6 RID: 7622 RVA: 0x000715DA File Offset: 0x0006F7DA
	protected override void Awake()
	{
		base.Awake();
		this.holdToPress = base.GetComponent<HoldToPress>();
		this.stillPressed = false;
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x000715F5 File Offset: 0x0006F7F5
	public void Update()
	{
		if (base.IsPressed() && !this.stillPressed)
		{
			this.BeginPress();
			this.stillPressed = true;
			return;
		}
		if (this.stillPressed && !base.IsPressed())
		{
			this.EndPress();
			this.stillPressed = false;
		}
	}

	// Token: 0x06001DC8 RID: 7624 RVA: 0x00071632 File Offset: 0x0006F832
	public void BeginPress()
	{
		this.holdToPress.enabled = true;
	}

	// Token: 0x06001DC9 RID: 7625 RVA: 0x00071640 File Offset: 0x0006F840
	public void EndPress()
	{
		this.holdToPress.enabled = false;
	}

	// Token: 0x06001DCA RID: 7626 RVA: 0x00071640 File Offset: 0x0006F840
	public void OnHoldComplete()
	{
		this.holdToPress.enabled = false;
	}

	// Token: 0x04001CDA RID: 7386
	public HoldToPress holdToPress;

	// Token: 0x04001CDB RID: 7387
	public bool stillPressed;
}
