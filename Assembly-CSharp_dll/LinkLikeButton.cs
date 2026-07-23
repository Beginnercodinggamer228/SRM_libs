using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020005AB RID: 1451
[RequireComponent(typeof(TMP_Text))]
public class LinkLikeButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06001E10 RID: 7696 RVA: 0x000723C3 File Offset: 0x000705C3
	public void Awake()
	{
		this.theText = base.GetComponent<TMP_Text>();
		this.normalColor = this.theText.color;
	}

	// Token: 0x06001E11 RID: 7697 RVA: 0x000723E2 File Offset: 0x000705E2
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.theText.color = this.highlightColor;
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x000723F5 File Offset: 0x000705F5
	public void OnPointerExit(PointerEventData eventData)
	{
		this.theText.color = this.normalColor;
	}

	// Token: 0x04001D31 RID: 7473
	public Color highlightColor;

	// Token: 0x04001D32 RID: 7474
	private TMP_Text theText;

	// Token: 0x04001D33 RID: 7475
	private Color normalColor = Color.blue;
}
