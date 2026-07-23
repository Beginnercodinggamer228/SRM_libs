using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020005C2 RID: 1474
public class MapScrollRect : ScrollRect
{
	// Token: 0x06001E99 RID: 7833 RVA: 0x00073DA0 File Offset: 0x00071FA0
	public float GetCurrentZoom()
	{
		return this.currentZoom;
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x00073DA8 File Offset: 0x00071FA8
	public override void OnScroll(PointerEventData data)
	{
		if (data.scrollDelta.y < 0f)
		{
			this.ZoomOut();
			return;
		}
		if (data.scrollDelta.y > 0f)
		{
			this.ZoomIn();
		}
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x00073DDC File Offset: 0x00071FDC
	public void Scroll(Vector2 scrollDelta)
	{
		base.OnScroll(new PointerEventData(null)
		{
			scrollDelta = scrollDelta
		});
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x00073DFE File Offset: 0x00071FFE
	public void ScrollUp()
	{
		this.Scroll(new Vector2(0f, base.scrollSensitivity));
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x00073E16 File Offset: 0x00072016
	public void ScrollDown()
	{
		this.Scroll(new Vector2(0f, -base.scrollSensitivity));
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x00073E2F File Offset: 0x0007202F
	public void ScrollLeft()
	{
		this.Scroll(new Vector2(base.scrollSensitivity, 0f));
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x00073E47 File Offset: 0x00072047
	public void ScrollRight()
	{
		this.Scroll(new Vector2(-base.scrollSensitivity, 0f));
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x00073E60 File Offset: 0x00072060
	public void ClampContentToScrollView()
	{
		base.OnScroll(new PointerEventData(null)
		{
			scrollDelta = new Vector2(0f, 0f)
		});
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x00073E90 File Offset: 0x00072090
	public void ZoomIn()
	{
		this.ZoomTo(this.currentZoom + 0.04f);
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x00073EA4 File Offset: 0x000720A4
	public void ZoomOut()
	{
		this.ZoomTo(this.currentZoom - 0.04f);
	}

	// Token: 0x06001EA3 RID: 7843 RVA: 0x00073EB8 File Offset: 0x000720B8
	public void ResetToDefaultZoom()
	{
		this.ZoomTo(1f);
	}

	// Token: 0x06001EA4 RID: 7844 RVA: 0x00073EC8 File Offset: 0x000720C8
	public void ZoomTo(float requestedZoomTarget)
	{
		float num = Mathf.Clamp(requestedZoomTarget, MapScrollRect.MinZoom, MapScrollRect.MaxZoom);
		if (num == this.currentZoom)
		{
			return;
		}
		float d = num / this.currentZoom;
		Vector3 localPosition = base.content.localPosition;
		base.content.transform.localScale = Vector3.one * num;
		base.content.localPosition = localPosition * d;
		new PointerEventData(null).scrollDelta = new Vector2(0f, 0f);
		this.ClampContentToScrollView();
		this.currentZoom = num;
		if (this.onZoom != null)
		{
			this.onZoom(this.currentZoom);
		}
	}

	// Token: 0x04001DA2 RID: 7586
	private const float DEFAULT_ZOOM = 1f;

	// Token: 0x04001DA3 RID: 7587
	private const float ZOOM_CHANGE_PER_FRAME = 0.04f;

	// Token: 0x04001DA4 RID: 7588
	private float currentZoom = 1f;

	// Token: 0x04001DA5 RID: 7589
	public static float MinZoom = 0.55f;

	// Token: 0x04001DA6 RID: 7590
	public static float MaxZoom = 2f;

	// Token: 0x04001DA7 RID: 7591
	public MapScrollRect.OnZoomEvent onZoom;

	// Token: 0x020005C3 RID: 1475
	// (Invoke) Token: 0x06001EA8 RID: 7848
	public delegate void OnZoomEvent(float zoomLevel);
}
