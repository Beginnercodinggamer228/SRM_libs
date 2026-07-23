using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000649 RID: 1609
[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class PanelStyler : SRBehaviour
{
	// Token: 0x060021AD RID: 8621 RVA: 0x0008242A File Offset: 0x0008062A
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.bg = base.GetComponent<Image>();
		this.ApplyStyle();
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x0008244C File Offset: 0x0008064C
	private void ApplyStyle()
	{
		UIStyleDirector.PanelStyle panelStyle = this.styleDir.GetPanelStyle(this.styleName);
		if (panelStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown panel style: " + this.styleName, Array.Empty<object>());
			}
			return;
		}
		if (panelStyle.bgSprite.apply)
		{
			this.bg.enabled = (panelStyle.bgSprite != null);
		}
		if (panelStyle.bgColor.apply)
		{
			this.bg.color = panelStyle.bgColor.value;
		}
		if (panelStyle.bgSprite.apply)
		{
			this.bg.sprite = panelStyle.bgSprite.value;
		}
	}

	// Token: 0x040020F6 RID: 8438
	[StyleName(typeof(UIStyleDirector.PanelStyle))]
	public string styleName = "Default";

	// Token: 0x040020F7 RID: 8439
	private UIStyleDirector styleDir;

	// Token: 0x040020F8 RID: 8440
	private Image bg;
}
