using System;
using UnityEngine.UI;

// Token: 0x02000590 RID: 1424
public class GamepadSettingsUI : BaseUI
{
	// Token: 0x06001D92 RID: 7570 RVA: 0x000709F0 File Offset: 0x0006EBF0
	public override void Awake()
	{
		base.Awake();
		this.swapSticksToggle.isOn = SRSingleton<GameContext>.Instance.InputDirector.GetSwapSticks();
		this.invertGamepadLookYToggle.isOn = SRSingleton<GameContext>.Instance.InputDirector.GetInvertGamepadLookY();
		this.sprintHoldToggle.isOn = SRSingleton<GameContext>.Instance.OptionsDirector.sprintHold;
		this.lookSensitivityXSlider.value = SRSingleton<GameContext>.Instance.InputDirector.GamepadLookSensitivityX;
		this.lookSensitivityYSlider.value = SRSingleton<GameContext>.Instance.InputDirector.GamepadLookSensitivityY;
	}

	// Token: 0x06001D93 RID: 7571 RVA: 0x00070A85 File Offset: 0x0006EC85
	public override void Close()
	{
		base.Close();
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x00070A8D File Offset: 0x0006EC8D
	public void ToggleInvertGamepadLookY()
	{
		SRSingleton<GameContext>.Instance.InputDirector.SetInvertGamepadLookY(this.invertGamepadLookYToggle.isOn);
	}

	// Token: 0x06001D95 RID: 7573 RVA: 0x00070AA9 File Offset: 0x0006ECA9
	public void ToggleSwapSticks()
	{
		SRSingleton<GameContext>.Instance.InputDirector.SetSwapSticks(this.swapSticksToggle.isOn);
	}

	// Token: 0x06001D96 RID: 7574 RVA: 0x00070AC5 File Offset: 0x0006ECC5
	public void ToggleSprintHold()
	{
		SRSingleton<GameContext>.Instance.OptionsDirector.sprintHold = this.sprintHoldToggle.isOn;
	}

	// Token: 0x06001D97 RID: 7575 RVA: 0x00070AE4 File Offset: 0x0006ECE4
	public void OnLookSensitivityXChanged()
	{
		float value = this.lookSensitivityXSlider.value;
		SRSingleton<GameContext>.Instance.InputDirector.GamepadLookSensitivityX = value;
	}

	// Token: 0x06001D98 RID: 7576 RVA: 0x00070B10 File Offset: 0x0006ED10
	public void OnLookSensitivityYChanged()
	{
		float value = this.lookSensitivityYSlider.value;
		SRSingleton<GameContext>.Instance.InputDirector.GamepadLookSensitivityY = value;
	}

	// Token: 0x04001CA5 RID: 7333
	public Toggle swapSticksToggle;

	// Token: 0x04001CA6 RID: 7334
	public Toggle invertGamepadLookYToggle;

	// Token: 0x04001CA7 RID: 7335
	public Toggle sprintHoldToggle;

	// Token: 0x04001CA8 RID: 7336
	public Slider lookSensitivityXSlider;

	// Token: 0x04001CA9 RID: 7337
	public Slider lookSensitivityYSlider;
}
