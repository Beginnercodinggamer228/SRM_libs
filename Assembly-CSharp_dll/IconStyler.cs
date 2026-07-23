using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200063E RID: 1598
[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class IconStyler : SRBehaviour
{
	// Token: 0x06002180 RID: 8576 RVA: 0x00080AF2 File Offset: 0x0007ECF2
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.img = base.GetComponent<Image>();
		this.ApplyStyle();
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x00080B14 File Offset: 0x0007ED14
	private void ApplyStyle()
	{
		UIStyleDirector.IconStyle iconStyle = this.styleDir.GetIconStyle(this.styleName);
		if (iconStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown icon style: " + this.styleName, Array.Empty<object>());
			}
			return;
		}
		if (iconStyle.color.apply)
		{
			this.img.color = iconStyle.color.value;
		}
		if (iconStyle.sprite.apply)
		{
			this.img.sprite = iconStyle.sprite.value;
		}
	}

	// Token: 0x040020CF RID: 8399
	[StyleName(typeof(UIStyleDirector.IconStyle))]
	public string styleName = "Default";

	// Token: 0x040020D0 RID: 8400
	private UIStyleDirector styleDir;

	// Token: 0x040020D1 RID: 8401
	private Image img;
}
