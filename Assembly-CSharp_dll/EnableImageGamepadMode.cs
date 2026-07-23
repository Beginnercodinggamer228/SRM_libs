using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000572 RID: 1394
[RequireComponent(typeof(Image))]
public class EnableImageGamepadMode : MonoBehaviour
{
	// Token: 0x06001D0F RID: 7439 RVA: 0x0006E57F File Offset: 0x0006C77F
	public void Start()
	{
		this.img = base.GetComponent<Image>();
	}

	// Token: 0x06001D10 RID: 7440 RVA: 0x0006E58D File Offset: 0x0006C78D
	public void Update()
	{
		this.img.enabled = InputDirector.UsingGamepad();
	}

	// Token: 0x04001C20 RID: 7200
	private Image img;
}
