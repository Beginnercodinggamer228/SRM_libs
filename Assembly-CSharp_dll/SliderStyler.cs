using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200064B RID: 1611
[ExecuteInEditMode]
[RequireComponent(typeof(Slider))]
public class SliderStyler : SRBehaviour
{
	// Token: 0x060021B3 RID: 8627 RVA: 0x0008272E File Offset: 0x0008092E
	public void OnEnable()
	{
		this.styleDir = UIStyleDirector.Instance;
		this.slider = base.GetComponent<Slider>();
		this.ApplyStyle();
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x00082750 File Offset: 0x00080950
	private void ApplyStyle()
	{
		UIStyleDirector.SliderStyle sliderStyle = this.styleDir.GetSliderStyle(this.styleName);
		if (sliderStyle == null)
		{
			if (Application.isPlaying)
			{
				Log.Warning("Unknown slider style: " + this.styleName, Array.Empty<object>());
			}
			return;
		}
		Transform transform = this.slider.transform.Find("Background");
		Image image = (transform == null) ? null : transform.GetComponent<Image>();
		if (sliderStyle.bgSprite.apply && image != null)
		{
			image.enabled = (sliderStyle.bgSprite.value != null);
		}
		if (sliderStyle.bgColor.apply && image != null)
		{
			image.color = sliderStyle.bgColor.value;
		}
		if (sliderStyle.bgSprite.apply && image != null)
		{
			image.sprite = sliderStyle.bgSprite.value;
		}
		Image image2 = (this.slider.handleRect == null) ? null : this.slider.handleRect.GetComponent<Image>();
		if (sliderStyle.handleColor.apply && image2 != null)
		{
			image2.color = sliderStyle.handleColor.value;
		}
		if (sliderStyle.handleSprite.apply && image2 != null)
		{
			image2.sprite = sliderStyle.handleSprite.value;
		}
		Image image3 = (this.slider.fillRect == null) ? null : this.slider.fillRect.GetComponent<Image>();
		if (sliderStyle.fillColor.apply && image3 != null)
		{
			image3.color = sliderStyle.fillColor.value;
		}
		if (sliderStyle.fillSprite.apply && image3 != null)
		{
			image3.sprite = sliderStyle.fillSprite.value;
		}
		ColorBlock colors = this.slider.colors;
		if (sliderStyle.normalTint.apply)
		{
			colors.normalColor = sliderStyle.normalTint.value;
		}
		if (sliderStyle.highlightedTint.apply)
		{
			colors.highlightedColor = sliderStyle.highlightedTint.value;
		}
		if (sliderStyle.pressedTint.apply)
		{
			colors.pressedColor = sliderStyle.pressedTint.value;
		}
		if (sliderStyle.disabledTint.apply)
		{
			colors.disabledColor = sliderStyle.disabledTint.value;
		}
		this.slider.colors = colors;
	}

	// Token: 0x040020FD RID: 8445
	[StyleName(typeof(UIStyleDirector.SliderStyle))]
	public string styleName = "Default";

	// Token: 0x040020FE RID: 8446
	private UIStyleDirector styleDir;

	// Token: 0x040020FF RID: 8447
	private Slider slider;
}
