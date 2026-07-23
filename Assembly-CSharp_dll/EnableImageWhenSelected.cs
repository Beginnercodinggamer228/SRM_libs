using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000573 RID: 1395
[RequireComponent(typeof(Image))]
public class EnableImageWhenSelected : MonoBehaviour
{
	// Token: 0x06001D12 RID: 7442 RVA: 0x0006E59F File Offset: 0x0006C79F
	public void Start()
	{
		this.img = base.GetComponent<Image>();
		this.selectableParent = base.GetComponentInParent<Selectable>();
	}

	// Token: 0x06001D13 RID: 7443 RVA: 0x0006E5BC File Offset: 0x0006C7BC
	public void Update()
	{
		this.img.enabled = ((!this.gamepadModeOnly || InputDirector.UsingGamepad()) && this.selectableParent != null && this.selectableParent.gameObject == EventSystem.current.currentSelectedGameObject);
	}

	// Token: 0x04001C21 RID: 7201
	public bool gamepadModeOnly = true;

	// Token: 0x04001C22 RID: 7202
	private Selectable selectableParent;

	// Token: 0x04001C23 RID: 7203
	private Image img;
}
