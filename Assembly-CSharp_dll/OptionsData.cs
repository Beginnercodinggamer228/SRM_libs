using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

// Token: 0x0200025D RID: 605
[Serializable]
public class OptionsData : DataModule<OptionsData>
{
	// Token: 0x04000BCD RID: 3021
	public const int CURR_FORMAT_ID = 2;

	// Token: 0x04000BCE RID: 3022
	public List<OptionsData.ButtonData> buttons = new List<OptionsData.ButtonData>();

	// Token: 0x04000BCF RID: 3023
	public List<OptionsData.AxisData> axes = new List<OptionsData.AxisData>();

	// Token: 0x04000BD0 RID: 3024
	public bool disableCameraBob = true;

	// Token: 0x04000BD1 RID: 3025
	public float masterVolume = 1f;

	// Token: 0x04000BD2 RID: 3026
	public float musicVolume = 0.5f;

	// Token: 0x04000BD3 RID: 3027
	public float sfxVolume = 1f;

	// Token: 0x04000BD4 RID: 3028
	public string bugReportEmail;

	// Token: 0x04000BD5 RID: 3029
	public bool bufferForGif;

	// Token: 0x04000BD6 RID: 3030
	public float mouseSensitivity = 1f;

	// Token: 0x04000BD7 RID: 3031
	public bool disableTutorials;

	// Token: 0x04000BD8 RID: 3032
	public SRQualitySettings.Settings qualitySettings;

	// Token: 0x04000BD9 RID: 3033
	public SRQualitySettings.Level qualityLevel = SRQualitySettings.Level.DEFAULT;

	// Token: 0x04000BDA RID: 3034
	public int screenWidth = 800;

	// Token: 0x04000BDB RID: 3035
	public int screenHeight = 600;

	// Token: 0x04000BDC RID: 3036
	public bool fullScreen = true;

	// Token: 0x04000BDD RID: 3037
	public bool swapSticks;

	// Token: 0x04000BDE RID: 3038
	public bool invertGamepadLookY;

	// Token: 0x04000BDF RID: 3039
	public bool invertMouseLookY;

	// Token: 0x04000BE0 RID: 3040
	public bool disableGamepad;

	// Token: 0x04000BE1 RID: 3041
	public bool vacLockOnHold;

	// Token: 0x04000BE2 RID: 3042
	public float lookSensitivityX;

	// Token: 0x04000BE3 RID: 3043
	public float lookSensitivityY = -0.2f;

	// Token: 0x04000BE4 RID: 3044
	public List<OptionsData.BindingData> bindings = new List<OptionsData.BindingData>();

	// Token: 0x0200025E RID: 606
	[Serializable]
	public class ButtonData
	{
		// Token: 0x06000CE2 RID: 3298 RVA: 0x000352C6 File Offset: 0x000334C6
		public ButtonData(string action, KeyCode primary, KeyCode secondary, KeyCode gamepad)
		{
			this.action = action;
			this.primary = primary;
			this.secondary = secondary;
			this.gamepad = gamepad;
		}

		// Token: 0x04000BE5 RID: 3045
		public string action;

		// Token: 0x04000BE6 RID: 3046
		public KeyCode primary;

		// Token: 0x04000BE7 RID: 3047
		public KeyCode secondary;

		// Token: 0x04000BE8 RID: 3048
		public KeyCode gamepad;
	}

	// Token: 0x0200025F RID: 607
	[Serializable]
	public class AxisData
	{
		// Token: 0x06000CE3 RID: 3299 RVA: 0x000352EB File Offset: 0x000334EB
		public AxisData(string action, KeyCode primaryPos, KeyCode primaryNeg, KeyCode secondaryPos, KeyCode secondaryNeg)
		{
			this.action = action;
			this.primaryPos = primaryPos;
			this.primaryNeg = primaryNeg;
			this.secondaryPos = secondaryPos;
			this.secondaryNeg = secondaryNeg;
		}

		// Token: 0x04000BE9 RID: 3049
		public string action;

		// Token: 0x04000BEA RID: 3050
		public KeyCode primaryPos;

		// Token: 0x04000BEB RID: 3051
		public KeyCode primaryNeg;

		// Token: 0x04000BEC RID: 3052
		public KeyCode secondaryPos;

		// Token: 0x04000BED RID: 3053
		public KeyCode secondaryNeg;
	}

	// Token: 0x02000260 RID: 608
	[Serializable]
	public class BindingData
	{
		// Token: 0x06000CE4 RID: 3300 RVA: 0x000053FC File Offset: 0x000035FC
		public BindingData()
		{
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x00035318 File Offset: 0x00033518
		public BindingData(string action, Key primKey, Mouse primMouse, Key secondary, InputControlType gamepad)
		{
			this.action = action;
			this.primKey = primKey;
			this.primMouse = primMouse;
			this.secondary = secondary;
			this.gamepad = gamepad;
		}

		// Token: 0x04000BEE RID: 3054
		public string action;

		// Token: 0x04000BEF RID: 3055
		public Key primKey;

		// Token: 0x04000BF0 RID: 3056
		public Mouse primMouse;

		// Token: 0x04000BF1 RID: 3057
		public Key secondary;

		// Token: 0x04000BF2 RID: 3058
		public InputControlType gamepad;
	}
}
