using System;
using UnityEngine;

// Token: 0x020005C0 RID: 1472
public class MapControls : MonoBehaviour
{
	// Token: 0x06001E8C RID: 7820 RVA: 0x00073B08 File Offset: 0x00071D08
	public void Awake()
	{
		if (this.scrollView != null)
		{
			this.mapScrollRect = this.scrollView.GetComponent<MapScrollRect>();
			this.WireButton(this.upButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ScrollUp));
			this.WireButton(this.downButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ScrollDown));
			this.WireButton(this.leftButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ScrollLeft));
			this.WireButton(this.rightButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ScrollRight));
			this.WireButton(this.zoomInButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ZoomIn));
			this.WireButton(this.zoomOutButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ZoomOut));
		}
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x00073BE8 File Offset: 0x00071DE8
	public void OnDestroy()
	{
		if (this.mapScrollRect != null)
		{
			this.UnwireButton(this.upButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ScrollUp));
			this.UnwireButton(this.downButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ScrollDown));
			this.UnwireButton(this.leftButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ScrollLeft));
			this.UnwireButton(this.rightButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ScrollRight));
			this.UnwireButton(this.zoomInButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ZoomIn));
			this.UnwireButton(this.zoomOutButton, new PerformWhileMouseDown.MouseIsDownEvent(this.mapScrollRect.ZoomOut));
		}
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x00073CB4 File Offset: 0x00071EB4
	private void WireButton(GameObject button, PerformWhileMouseDown.MouseIsDownEvent eventHandler)
	{
		if (button != null)
		{
			PerformWhileMouseDown component = button.GetComponent<PerformWhileMouseDown>();
			if (component != null)
			{
				PerformWhileMouseDown performWhileMouseDown = component;
				performWhileMouseDown.WhileMouseIsDown = (PerformWhileMouseDown.MouseIsDownEvent)Delegate.Combine(performWhileMouseDown.WhileMouseIsDown, eventHandler);
			}
		}
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x00073CF4 File Offset: 0x00071EF4
	private void UnwireButton(GameObject button, PerformWhileMouseDown.MouseIsDownEvent eventHandler)
	{
		if (button != null)
		{
			PerformWhileMouseDown component = button.GetComponent<PerformWhileMouseDown>();
			if (component != null)
			{
				PerformWhileMouseDown performWhileMouseDown = component;
				performWhileMouseDown.WhileMouseIsDown = (PerformWhileMouseDown.MouseIsDownEvent)Delegate.Remove(performWhileMouseDown.WhileMouseIsDown, eventHandler);
			}
		}
	}

	// Token: 0x04001D99 RID: 7577
	public GameObject upButton;

	// Token: 0x04001D9A RID: 7578
	public GameObject downButton;

	// Token: 0x04001D9B RID: 7579
	public GameObject leftButton;

	// Token: 0x04001D9C RID: 7580
	public GameObject rightButton;

	// Token: 0x04001D9D RID: 7581
	public GameObject zoomInButton;

	// Token: 0x04001D9E RID: 7582
	public GameObject zoomOutButton;

	// Token: 0x04001D9F RID: 7583
	public GameObject scrollView;

	// Token: 0x04001DA0 RID: 7584
	private MapScrollRect mapScrollRect;
}
