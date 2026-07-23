using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020002AC RID: 684
public class OptionsDirector : SRBehaviour
{
	// Token: 0x06000E7F RID: 3711 RVA: 0x0003A957 File Offset: 0x00038B57
	public void Awake()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0003A96A File Offset: 0x00038B6A
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		this.UpdateCameraFOVs();
		this.UpdateVsync();
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0003A978 File Offset: 0x00038B78
	public void OnEnable()
	{
		this.SetDefaultVolumes();
		this.UpdateCameraFOVs();
		this.UpdateVsync();
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0003A98C File Offset: 0x00038B8C
	public void ResetProfile()
	{
		this.disableCameraBob = true;
		this.enabledTutorials = OptionsDirector.EnabledTutorials.ALL;
		this.bufferForGif = true;
		this.bugReportEmail = "";
		this.vacLockOnHold = true;
		this.sprintHold = false;
		this.SetFOV(75f);
		this.SetDefaultVolumes();
		this.SetVsync(true);
		this.SetShowMinimalHUD(false);
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0003A9E6 File Offset: 0x00038BE6
	public void SetOverscanAdjustment(float value)
	{
		this.overscanAdjustment = value;
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0003A9EF File Offset: 0x00038BEF
	public float GetOverscanAdjustment()
	{
		return this.overscanAdjustment;
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x0003A9F8 File Offset: 0x00038BF8
	public void SetFOV(float value)
	{
		this.fov = value;
		foreach (CameraFOVAdjuster cameraFOVAdjuster in CameraFOVAdjuster.Instances)
		{
			cameraFOVAdjuster.SetFOV(this.fov);
		}
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x0003AA54 File Offset: 0x00038C54
	public void UpdateCameraFOVs()
	{
		this.SetFOV(this.fov);
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x0003AA62 File Offset: 0x00038C62
	public float GetFOV()
	{
		return this.fov;
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x0003AA6A File Offset: 0x00038C6A
	public void SetShowMinimalHUD(bool showMinimalHud)
	{
		this.showMinimalHud = showMinimalHud;
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x0003AA73 File Offset: 0x00038C73
	public bool GetShowMinimalHUD()
	{
		return this.showMinimalHud;
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x0003AA7B File Offset: 0x00038C7B
	public void SetVsync(bool enabled)
	{
		this.enableVsync = enabled;
		QualitySettings.vSyncCount = (this.enableVsync ? 1 : 0);
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x0003AA95 File Offset: 0x00038C95
	public void UpdateVsync()
	{
		this.SetVsync(this.enableVsync);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x0003AAA4 File Offset: 0x00038CA4
	private void SetDefaultVolumes()
	{
		SECTR_AudioBus sectr_AudioBus = null;
		if (SECTR_AudioSystem.System != null)
		{
			sectr_AudioBus = SECTR_AudioSystem.System.MasterBus;
		}
		if (sectr_AudioBus != null)
		{
			sectr_AudioBus.UserVolume = 0.8f;
			foreach (SECTR_AudioBus sectr_AudioBus2 in sectr_AudioBus.Children)
			{
				if (sectr_AudioBus2.name == "Music")
				{
					sectr_AudioBus2.UserVolume = 0.5f;
				}
				else if (sectr_AudioBus2.name == "SFX")
				{
					sectr_AudioBus2.UserVolume = 0.9f;
				}
				else
				{
					Log.Warning("Unknown top-level bus name: " + sectr_AudioBus2.name, Array.Empty<object>());
				}
			}
		}
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x0003AB7C File Offset: 0x00038D7C
	public void SetScreenResolution(int width, int height, bool fullScreen)
	{
		if (width == 0 || height == 0)
		{
			Log.Debug("Attempted to set resolution to an invalid value.", new object[]
			{
				"width",
				width,
				"height",
				height
			});
			return;
		}
		this.screenWidth = width;
		this.screenHeight = height;
		this.isFullScreen = fullScreen;
		Log.Debug("Setting screen res", new object[]
		{
			"width",
			width,
			"height",
			height,
			"fullScreen",
			fullScreen
		});
		Screen.SetResolution(width, height, fullScreen);
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x0003AC22 File Offset: 0x00038E22
	public void ResetScreenResolution()
	{
		this.SetScreenResolution(this.screenWidth, this.screenHeight, this.isFullScreen);
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x0003AC3C File Offset: 0x00038E3C
	public void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			this.ResetScreenResolution();
		}
	}

	// Token: 0x04000D90 RID: 3472
	public bool disableCameraBob = true;

	// Token: 0x04000D91 RID: 3473
	public OptionsDirector.EnabledTutorials enabledTutorials;

	// Token: 0x04000D92 RID: 3474
	public bool bufferForGif = true;

	// Token: 0x04000D93 RID: 3475
	public string bugReportEmail = "";

	// Token: 0x04000D94 RID: 3476
	public bool vacLockOnHold = true;

	// Token: 0x04000D95 RID: 3477
	public bool sprintHold;

	// Token: 0x04000D96 RID: 3478
	public bool enableVsync = true;

	// Token: 0x04000D97 RID: 3479
	private bool showMinimalHud;

	// Token: 0x04000D98 RID: 3480
	private float fov = 75f;

	// Token: 0x04000D99 RID: 3481
	private float overscanAdjustment;

	// Token: 0x04000D9A RID: 3482
	private const bool DEFAULT_ENABLED_VSYNC = true;

	// Token: 0x04000D9B RID: 3483
	public int screenWidth;

	// Token: 0x04000D9C RID: 3484
	public int screenHeight;

	// Token: 0x04000D9D RID: 3485
	public bool isFullScreen;

	// Token: 0x020002AD RID: 685
	public enum EnabledTutorials
	{
		// Token: 0x04000D9F RID: 3487
		ALL,
		// Token: 0x04000DA0 RID: 3488
		ADVANCED_ONLY,
		// Token: 0x04000DA1 RID: 3489
		NONE
	}
}
