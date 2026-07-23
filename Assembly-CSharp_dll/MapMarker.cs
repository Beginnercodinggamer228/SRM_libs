using System;
using UnityEngine;

// Token: 0x020005C1 RID: 1473
public class MapMarker : MonoBehaviour
{
	// Token: 0x06001E91 RID: 7825 RVA: 0x00073D31 File Offset: 0x00071F31
	public void Awake()
	{
		this.rect = base.GetComponent<RectTransform>();
	}

	// Token: 0x06001E92 RID: 7826 RVA: 0x00073D3F File Offset: 0x00071F3F
	public virtual Quaternion GetRotation()
	{
		return this.rect.localRotation;
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x00073D4C File Offset: 0x00071F4C
	public virtual void Rotate(Quaternion rotation)
	{
		this.rect.rotation = rotation;
	}

	// Token: 0x06001E94 RID: 7828 RVA: 0x00073D5A File Offset: 0x00071F5A
	public virtual Vector2 GetSize()
	{
		return this.rect.sizeDelta;
	}

	// Token: 0x06001E95 RID: 7829 RVA: 0x00073D67 File Offset: 0x00071F67
	public virtual void SetSize(float height, float width)
	{
		this.rect.sizeDelta = new Vector2(width, height);
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x00073D7B File Offset: 0x00071F7B
	public virtual void SetAnchoredPosition(Vector3 position)
	{
		this.rect.anchoredPosition = position;
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x00073D8E File Offset: 0x00071F8E
	public virtual Vector3 GetLocalPosition()
	{
		return base.gameObject.transform.localPosition;
	}

	// Token: 0x04001DA1 RID: 7585
	private RectTransform rect;
}
