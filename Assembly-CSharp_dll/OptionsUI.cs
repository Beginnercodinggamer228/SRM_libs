using System;
using System.Collections.Generic;
using System.Globalization;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020005CF RID: 1487
public class OptionsUI : BaseUI
{
	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06001EFF RID: 7935 RVA: 0x00075A18 File Offset: 0x00073C18
	public bool IsInitialzing
	{
		get
		{
			return this.initializing;
		}
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x00075A20 File Offset: 0x00073C20
	public override void Awake()
	{
		this.optionsDirector = SRSingleton<GameContext>.Instance.OptionsDirector;
		base.Awake();
		this.modsTab.SetActive(false);
		this.title.GetComponent<XlateText>().key = "t.options";
		this.gamepadPanel.SetActive(false);
		this.tabsPanel.SetActive(true);
		this.videoPanel.SetActive(true);
		this.videoTab.SetActive(true);
		this.inputTab.SetActive(true);
		this.SelectVideoTab();
		this.bufferForGifToggle.gameObject.SetActive(true);
		this.otherTabFovRow.SetActive(false);
		this.otherTabFovSlider.gameObject.SetActive(false);
		this.enableVsyncOtherRow.gameObject.SetActive(false);
		this.enableVsyncToggle.gameObject.SetActive(false);
		this.SetupVertNav(new Selectable[]
		{
			this.languageDropdown,
			this.tutorialsDropdown,
			this.disableCameraBobToggle,
			this.showMinimalHUDToggle,
			this.bufferForGifToggle,
			this.vacLockToggle,
			this.sprintHoldToggle,
			this.enableVsyncToggle,
			this.otherTabFovSlider,
			this.overscanSlider,
			this.resetProfileButton
		});
		BindingListenOptions listenOptions = SRInput.Actions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Combine(listenOptions.OnBindingAdded, new Action<PlayerAction, BindingSource>(this.OnBindingAdded));
		BindingListenOptions listenOptions2 = SRInput.Actions.ListenOptions;
		listenOptions2.OnBindingRejected = (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)Delegate.Combine(listenOptions2.OnBindingRejected, new Action<PlayerAction, BindingSource, BindingSourceRejectionType>(this.OnBindingRejected));
	}

	// Token: 0x06001F01 RID: 7937 RVA: 0x00075BBE File Offset: 0x00073DBE
	private void OnBindingAdded(PlayerAction action, BindingSource binding)
	{
		this.RefreshBindings();
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x00075BBE File Offset: 0x00073DBE
	private void OnBindingRejected(PlayerAction action, BindingSource binding, BindingSourceRejectionType rejection)
	{
		this.RefreshBindings();
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x00075BC8 File Offset: 0x00073DC8
	public override void OnDestroy()
	{
		base.OnDestroy();
		BindingListenOptions listenOptions = SRInput.Actions.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Remove(listenOptions.OnBindingAdded, new Action<PlayerAction, BindingSource>(this.OnBindingAdded));
		BindingListenOptions listenOptions2 = SRInput.Actions.ListenOptions;
		listenOptions2.OnBindingRejected = (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)Delegate.Remove(listenOptions2.OnBindingRejected, new Action<PlayerAction, BindingSource, BindingSourceRejectionType>(this.OnBindingRejected));
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x00075C31 File Offset: 0x00073E31
	private void SetupOptionsUI()
	{
		this.initializing = true;
		this.SetupOtherOptions();
		this.SetupMods();
		this.SetupVideoSettings();
		this.SetupAudio();
		this.SetupInput();
		this.SetupLanguages();
		this.initializing = false;
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x00075C68 File Offset: 0x00073E68
	private void SetupAudio()
	{
		this.masterSlider.value = this.masterBus.UserVolume;
		this.musicSlider.value = this.musicBus.UserVolume;
		this.sfxSlider.value = this.sfxBus.UserVolume;
		this.sensitivitySlider.value = SRSingleton<GameContext>.Instance.InputDirector.MouseLookSensitivity;
	}

	// Token: 0x06001F06 RID: 7942 RVA: 0x00075CD1 File Offset: 0x00073ED1
	public override void OnBundlesAvailable(MessageDirector msgDir)
	{
		base.OnBundlesAvailable(msgDir);
		this.SetupOptionsUI();
	}

	// Token: 0x06001F07 RID: 7943 RVA: 0x00075CE0 File Offset: 0x00073EE0
	private void SetupVideoSettings()
	{
		this.SetupDropdown<SRQualitySettings.Level>(this.overallDropdown, "l.quality_", (SRQualitySettings.Level level) => level == SRQualitySettings.CurrentLevel, delegate(SRQualitySettings.Level level)
		{
			SRQualitySettings.CurrentLevel = level;
		});
		this.SetupDropdown<SRQualitySettings.LightingLevel>(this.lightingDropdown, "l.lighting_", (SRQualitySettings.LightingLevel level) => level == SRQualitySettings.Lighting, delegate(SRQualitySettings.LightingLevel level)
		{
			SRQualitySettings.Lighting = level;
		});
		this.SetupDropdown<SRQualitySettings.ShadowsLevel>(this.shadowsDropdown, "l.shadows_", (SRQualitySettings.ShadowsLevel level) => level == SRQualitySettings.Shadows, delegate(SRQualitySettings.ShadowsLevel level)
		{
			SRQualitySettings.Shadows = level;
		});
		this.SetupDropdown<SRQualitySettings.TextureLevel>(this.texturesDropdown, "l.textures_", (SRQualitySettings.TextureLevel level) => level == SRQualitySettings.Textures, delegate(SRQualitySettings.TextureLevel level)
		{
			SRQualitySettings.Textures = level;
		});
		this.SetupDropdown<SRQualitySettings.ParticlesLevel>(this.particlesDropdown, "l.particles_", (SRQualitySettings.ParticlesLevel level) => level == SRQualitySettings.Particles, delegate(SRQualitySettings.ParticlesLevel level)
		{
			SRQualitySettings.Particles = level;
		});
		this.SetupDropdown<SRQualitySettings.ModelDetailLevel>(this.modelDetailDropdown, "l.model_detail_", (SRQualitySettings.ModelDetailLevel level) => level == SRQualitySettings.ModelDetail, delegate(SRQualitySettings.ModelDetailLevel level)
		{
			SRQualitySettings.ModelDetail = level;
		});
		this.SetupDropdown<SRQualitySettings.WaterDetailLevel>(this.waterDetailDropdown, "l.water_detail_", (SRQualitySettings.WaterDetailLevel level) => level == SRQualitySettings.WaterDetail, delegate(SRQualitySettings.WaterDetailLevel level)
		{
			SRQualitySettings.WaterDetail = level;
		});
		this.SetupDropdown<SRQualitySettings.AntialiasingMode>(this.antialiasingDropdown, "l.antialiasing_", (SRQualitySettings.AntialiasingMode level) => level == SRQualitySettings.Antialiasing, delegate(SRQualitySettings.AntialiasingMode level)
		{
			SRQualitySettings.Antialiasing = level;
		});
		this.ambientOcclusionToggle.isOn = SRQualitySettings.AmbientOcclusion;
		this.bloomToggle.isOn = SRQualitySettings.Bloom;
		this.onQualityChanged = (OptionsUI.OnQualityChanged)Delegate.Combine(this.onQualityChanged, new OptionsUI.OnQualityChanged(delegate()
		{
			this.ambientOcclusionToggle.isOn = SRQualitySettings.AmbientOcclusion;
			this.bloomToggle.isOn = SRQualitySettings.Bloom;
		}));
		this.fullscreenToggle.isOn = Screen.fullScreen;
		this.fovSlider.value = this.optionsDirector.GetFOV();
		this.fovValText.text = Mathf.RoundToInt(this.fovSlider.value).ToString();
		this.otherTabFovSlider.value = this.optionsDirector.GetFOV();
		this.otherTabFovValText.text = Mathf.RoundToInt(this.otherTabFovSlider.value).ToString();
		this.overscanSlider.value = this.optionsDirector.GetOverscanAdjustment() * 100f;
		this.overscanValText.text = Mathf.RoundToInt(this.overscanSlider.value).ToString();
		this.enableVsyncToggle.isOn = this.optionsDirector.enableVsync;
		this.enableVsyncVideoToggle.isOn = this.optionsDirector.enableVsync;
		int num = 0;
		Resolution currentResolution = Screen.currentResolution;
		currentResolution.width = Screen.width;
		currentResolution.height = Screen.height;
		Log.Debug("Current resolution", new object[]
		{
			"height",
			currentResolution.height,
			"width",
			currentResolution.width,
			"refreshRate",
			currentResolution.refreshRate,
			"fullScreenMode",
			Screen.fullScreenMode
		});
		int num2 = 0;
		int num3 = 0;
		foreach (Resolution item in Screen.resolutions)
		{
			if (num2 != item.height || num3 != item.width)
			{
				bool flag = currentResolution.width == item.width && currentResolution.height == item.height;
				num2 = item.height;
				num3 = item.width;
				if (flag || (item.width >= 800 && item.height >= 600))
				{
					string text = this.uiBundle.Get("m.resolution", new object[]
					{
						item.width,
						item.height
					});
					this.resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(text));
					if (flag)
					{
						this.resolutionDropdown.value = num;
						this.resolutionDropdown.captionText.text = text;
					}
					this.dropdownResolutions.Add(item);
					num++;
				}
			}
		}
		if (Application.isEditor)
		{
			this.resolutionDropdown.interactable = false;
			this.resolutionDropdown.captionText.text = "Disabled in Editor";
			this.fullscreenToggle.interactable = false;
			this.resolutionApplyButton.interactable = false;
			Navigation navigation = this.overallDropdown.navigation;
			navigation.selectOnUp = null;
			this.overallDropdown.navigation = navigation;
		}
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x000762A4 File Offset: 0x000744A4
	private void SetupLanguages()
	{
		this.SetupDropdown<MessageDirector.Lang>(this.languageDropdown, "l.lang_", delegate(MessageDirector.Lang lang)
		{
			CultureInfo culture = SRSingleton<GameContext>.Instance.MessageDirector.GetCulture();
			return Enum.GetName(typeof(MessageDirector.Lang), lang).ToLowerInvariant() == culture.TwoLetterISOLanguageName;
		}, delegate(MessageDirector.Lang lang)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.SetCulture(lang);
		});
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x00076300 File Offset: 0x00074500
	private void SetupOtherOptions()
	{
		this.disableCameraBobToggle.isOn = this.optionsDirector.disableCameraBob;
		this.bufferForGifToggle.isOn = this.optionsDirector.bufferForGif;
		this.vacLockToggle.isOn = this.optionsDirector.vacLockOnHold;
		this.sprintHoldToggle.isOn = this.optionsDirector.sprintHold;
		this.showMinimalHUDToggle.isOn = this.optionsDirector.GetShowMinimalHUD();
		this.SetupDropdown<OptionsDirector.EnabledTutorials>(this.tutorialsDropdown, "l.tutorials.", (OptionsDirector.EnabledTutorials value) => this.optionsDirector.enabledTutorials == value, delegate(OptionsDirector.EnabledTutorials value)
		{
			this.optionsDirector.enabledTutorials = value;
		});
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x000763A4 File Offset: 0x000745A4
	private void SetupInput()
	{
		for (int i = 0; i < this.bindingsPanel.transform.childCount; i++)
		{
			Destroyer.Destroy(this.bindingsPanel.transform.GetChild(i).gameObject, "OptionsUI.SetupInput");
		}
		this.CreateKeyBindingLine("key.forward", SRInput.Actions.verticalPos);
		this.CreateKeyBindingLine("key.left", SRInput.Actions.horizontalNeg);
		this.CreateKeyBindingLine("key.back", SRInput.Actions.verticalNeg);
		this.CreateKeyBindingLine("key.right", SRInput.Actions.horizontalPos);
		this.CreateKeyBindingLine("key.shoot", SRInput.Actions.attack);
		this.CreateKeyBindingLine("key.vac", SRInput.Actions.vac);
		this.CreateKeyBindingLine("key.burst", SRInput.Actions.burst);
		this.CreateKeyBindingLine("key.jump", SRInput.Actions.jump);
		this.CreateKeyBindingLine("key.run", SRInput.Actions.run);
		this.CreateKeyBindingLine("key.interact", SRInput.Actions.interact);
		this.CreateKeyBindingLine("key.gadgetMode", SRInput.Actions.toggleGadgetMode);
		this.CreateKeyBindingLine("key.flashlight", SRInput.Actions.light);
		this.CreateKeyBindingLine("key.radar", SRInput.Actions.radarToggle);
		this.CreateKeyBindingLine("key.map", SRInput.Actions.openMap);
		this.CreateKeyBindingLine("key.slot_1", SRInput.Actions.slot1);
		this.CreateKeyBindingLine("key.slot_2", SRInput.Actions.slot2);
		this.CreateKeyBindingLine("key.slot_3", SRInput.Actions.slot3);
		this.CreateKeyBindingLine("key.slot_4", SRInput.Actions.slot4);
		this.CreateKeyBindingLine("key.slot_5", SRInput.Actions.slot5);
		this.CreateKeyBindingLine("key.prev_slot", SRInput.Actions.prevSlot);
		this.CreateKeyBindingLine("key.next_slot", SRInput.Actions.nextSlot);
		this.CreateKeyBindingLine("key.reportissue", SRInput.Actions.reportIssue);
		this.CreateKeyBindingLine("key.screenshot", SRInput.Actions.screenshot);
		this.CreateKeyBindingLine("key.recordgif", SRInput.Actions.recordGif);
		this.CreateKeyBindingLine("key.pedia", SRInput.Actions.pedia);
		Button[] componentsInChildren = this.bindingsPanel.GetComponentsInChildren<Button>(true);
		for (int j = 0; j < componentsInChildren.Length; j += 2)
		{
			Navigation navigation = default(Navigation);
			navigation.mode = Navigation.Mode.Explicit;
			navigation.selectOnRight = componentsInChildren[j + 1];
			if (j < componentsInChildren.Length - 2)
			{
				navigation.selectOnDown = componentsInChildren[j + 2];
			}
			else
			{
				navigation.selectOnDown = this.sensitivitySlider;
				Navigation navigation2 = this.sensitivitySlider.navigation;
				navigation2.mode = Navigation.Mode.Explicit;
				navigation2.selectOnUp = componentsInChildren[j];
				this.sensitivitySlider.navigation = navigation2;
			}
			if (j > 0)
			{
				navigation.selectOnUp = componentsInChildren[j - 2];
			}
			else
			{
				navigation.selectOnUp = this.defaultKeyBtn;
				Navigation navigation3 = this.defaultKeyBtn.navigation;
				navigation3.mode = Navigation.Mode.Explicit;
				navigation3.selectOnDown = componentsInChildren[j];
				this.defaultKeyBtn.navigation = navigation3;
			}
			componentsInChildren[j].navigation = navigation;
			Navigation navigation4 = default(Navigation);
			navigation4.mode = Navigation.Mode.Explicit;
			navigation4.selectOnLeft = componentsInChildren[j];
			if (j < componentsInChildren.Length - 2)
			{
				navigation4.selectOnDown = componentsInChildren[j + 3];
			}
			else
			{
				navigation4.selectOnDown = this.sensitivitySlider;
			}
			if (j > 0)
			{
				navigation4.selectOnUp = componentsInChildren[j - 1];
			}
			else
			{
				navigation4.selectOnUp = this.defaultKeyBtn;
			}
			componentsInChildren[j + 1].navigation = navigation4;
		}
	}

	// Token: 0x06001F0B RID: 7947 RVA: 0x0007675C File Offset: 0x0007495C
	private void SetupMods()
	{
		Toggle[] componentsInChildren = this.modListPanel.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Destroyer.Destroy(componentsInChildren[i].gameObject, "OptionsUI.SetupMods");
		}
		foreach (object obj in Enum.GetValues(typeof(ModDirector.Mod)))
		{
			ModDirector.Mod mod = (ModDirector.Mod)obj;
			this.CreateModToggle(mod);
		}
		Toggle[] componentsInChildren2 = this.modListPanel.GetComponentsInChildren<Toggle>(true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			Navigation navigation = default(Navigation);
			navigation.mode = Navigation.Mode.Explicit;
			if (j < componentsInChildren2.Length - 1)
			{
				navigation.selectOnDown = componentsInChildren2[j + 1];
			}
			if (j > 0)
			{
				navigation.selectOnUp = componentsInChildren2[j - 1];
			}
			componentsInChildren2[j].navigation = navigation;
		}
		this.invertMouseLookYToggle.isOn = SRSingleton<GameContext>.Instance.InputDirector.GetInvertMouseLookY();
		this.disableMouseLookSmooth.isOn = SRSingleton<GameContext>.Instance.InputDirector.GetDisableMouseLookSmooth();
		if (componentsInChildren2.Length != 0)
		{
			componentsInChildren2[0].gameObject.AddComponent<InitSelected>();
		}
	}

	// Token: 0x06001F0C RID: 7948 RVA: 0x00076898 File Offset: 0x00074A98
	public void RefreshBindings()
	{
		if (this.initializing)
		{
			return;
		}
		BindingLineUI[] componentsInChildren = base.GetComponentsInChildren<BindingLineUI>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Refresh();
		}
		if (this.gamepadPanel != null)
		{
			this.gamepadPanel.GetComponent<GamepadPanel>().RefreshBindings();
		}
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x000768EC File Offset: 0x00074AEC
	private GameObject CreateKeyBindingLine(string label, PlayerAction action)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.bindingLinePrefab);
		gameObject.transform.SetParent(this.bindingsPanel.transform, false);
		BindingPanel.CreateBindingLine(label, action, gameObject, this.uiBundle, this.labelKeyDict, null);
		return gameObject;
	}

	// Token: 0x06001F0E RID: 7950 RVA: 0x00076933 File Offset: 0x00074B33
	public void ToggleDisableCameraBob()
	{
		this.optionsDirector.disableCameraBob = this.disableCameraBobToggle.isOn;
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x0007694B File Offset: 0x00074B4B
	public void ToggleEnableVsync()
	{
		this.optionsDirector.enableVsync = this.enableVsyncToggle.isOn;
		this.optionsDirector.UpdateVsync();
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x0007696E File Offset: 0x00074B6E
	public void ToggleEnableVsyncVideo()
	{
		this.optionsDirector.enableVsync = this.enableVsyncVideoToggle.isOn;
		this.optionsDirector.UpdateVsync();
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x00076991 File Offset: 0x00074B91
	public void ToggleBufferForGif()
	{
		this.optionsDirector.bufferForGif = this.bufferForGifToggle.isOn;
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x000769A9 File Offset: 0x00074BA9
	public void ToggleVacLock()
	{
		this.optionsDirector.vacLockOnHold = this.vacLockToggle.isOn;
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x000769C1 File Offset: 0x00074BC1
	public void ToggleSprintHold()
	{
		this.optionsDirector.sprintHold = this.sprintHoldToggle.isOn;
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000769DC File Offset: 0x00074BDC
	public override void Close()
	{
		base.Close();
		SRInput.AddOrReplaceBinding(SRInput.PauseActions.closeMap, SRInput.Actions.openMap);
		SRSingleton<SceneContext>.Instance.TutorialDirector.MaybeShowStatusTutorials();
		SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveProfile();
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x00076A28 File Offset: 0x00074C28
	private void DeselectAll()
	{
		this.videoPanel.SetActive(false);
		this.audioPanel.SetActive(false);
		this.inputPanel.SetActive(false);
		this.gamepadPanel.SetActive(false);
		this.modsPanel.SetActive(false);
		this.otherPanel.SetActive(false);
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x00076A7D File Offset: 0x00074C7D
	public void SelectVideoTab()
	{
		this.DeselectAll();
		this.videoPanel.SetActive(true);
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x00076A91 File Offset: 0x00074C91
	public void SelectAudioTab()
	{
		this.DeselectAll();
		this.audioPanel.SetActive(true);
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x00076AA5 File Offset: 0x00074CA5
	public void SelectInputTab()
	{
		this.DeselectAll();
		this.inputPanel.SetActive(true);
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x00076AB9 File Offset: 0x00074CB9
	public void SelectGamepadTab()
	{
		this.DeselectAll();
		this.gamepadPanel.SetActive(true);
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x00076ACD File Offset: 0x00074CCD
	public void SelectModsTab()
	{
		this.DeselectAll();
		this.modsPanel.SetActive(true);
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x00076AE1 File Offset: 0x00074CE1
	public void SelectOtherTab()
	{
		this.DeselectAll();
		this.otherPanel.SetActive(true);
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x00076AF8 File Offset: 0x00074CF8
	public void OnAudioLevelsChanged()
	{
		if (this.initializing)
		{
			return;
		}
		this.masterBus.UserVolume = this.masterSlider.value;
		this.musicBus.UserVolume = this.musicSlider.value;
		this.sfxBus.UserVolume = this.sfxSlider.value;
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x00076B50 File Offset: 0x00074D50
	public void OnSensitivityChanged()
	{
		float value = this.sensitivitySlider.value;
		SRSingleton<GameContext>.Instance.InputDirector.MouseLookSensitivity = value;
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x00076B79 File Offset: 0x00074D79
	public void OnAmbientOcclusionChanged()
	{
		SRQualitySettings.AmbientOcclusion = this.ambientOcclusionToggle.isOn;
		this.onQualityChanged();
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x00076B96 File Offset: 0x00074D96
	public void OnBloomChanged()
	{
		SRQualitySettings.Bloom = this.bloomToggle.isOn;
		this.onQualityChanged();
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x00076BB4 File Offset: 0x00074DB4
	public void OnFOVChanged()
	{
		this.optionsDirector.SetFOV(this.fovSlider.value);
		this.fovValText.text = Mathf.RoundToInt(this.fovSlider.value).ToString();
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x00076BFC File Offset: 0x00074DFC
	public void OnOtherTabFOVChanged()
	{
		this.optionsDirector.SetFOV(this.otherTabFovSlider.value);
		this.otherTabFovValText.text = Mathf.RoundToInt(this.otherTabFovSlider.value).ToString();
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x00076C44 File Offset: 0x00074E44
	public void OnOverscanAdjustmentChanged()
	{
		float overscanAdjustment = Mathf.Clamp(this.overscanSlider.value, 0f, 15f) * 0.01f;
		this.optionsDirector.SetOverscanAdjustment(overscanAdjustment);
		this.overscanValText.text = Mathf.RoundToInt(this.overscanSlider.value).ToString();
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x00076CA4 File Offset: 0x00074EA4
	public void OnApplyResolution()
	{
		this.CreateConfirmResolutionDialog();
		Resolution resolution = this.dropdownResolutions[this.resolutionDropdown.value];
		this.optionsDirector.SetScreenResolution(resolution.width, resolution.height, this.fullscreenToggle.isOn);
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x00076CF3 File Offset: 0x00074EF3
	public void ToggleInvertMouseLookY()
	{
		SRSingleton<GameContext>.Instance.InputDirector.SetInvertMouseLookY(this.invertMouseLookYToggle.isOn);
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x00076D0F File Offset: 0x00074F0F
	public void ToggleDisableMouseLookSmooth()
	{
		SRSingleton<GameContext>.Instance.InputDirector.SetDisableMouseLookSmooth(this.disableMouseLookSmooth.isOn);
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x00076D2B File Offset: 0x00074F2B
	public void ToggleShowMinimalHUD()
	{
		SRSingleton<GameContext>.Instance.OptionsDirector.SetShowMinimalHUD(this.showMinimalHUDToggle.isOn);
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x00076D47 File Offset: 0x00074F47
	public void ResetKeyMouseDefaults()
	{
		SRSingleton<GameContext>.Instance.InputDirector.ResetKeyMouseDefaults();
		this.RefreshBindings();
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x00076D5E File Offset: 0x00074F5E
	public void ResetProfile()
	{
		SRSingleton<GameContext>.Instance.UITemplates.CreateConfirmDialog("m.confirm_reset_profile", delegate
		{
			this.Close();
			SRSingleton<GameContext>.Instance.AutoSaveDirector.ResetProfile();
		});
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x00076D84 File Offset: 0x00074F84
	private void CreateModToggle(ModDirector.Mod mod)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.modTogglePrefab);
		gameObject.transform.SetParent(this.modListPanel, false);
		Toggle component = gameObject.GetComponent<Toggle>();
		component.isOn = SRSingleton<SceneContext>.Instance.ModDirector.IsModActive(mod);
		gameObject.transform.Find("Label").GetComponent<TMP_Text>().text = this.uiBundle.Get("l.mod_" + Enum.GetName(typeof(ModDirector.Mod), mod).ToLowerInvariant());
		component.onValueChanged.AddListener(delegate(bool selected)
		{
			if (selected)
			{
				SRSingleton<SceneContext>.Instance.ModDirector.ActivateMod(mod);
				return;
			}
			SRSingleton<SceneContext>.Instance.ModDirector.DeactivateMod(mod);
		});
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x00076E44 File Offset: 0x00075044
	private void SetupDropdown<T>(TMP_Dropdown dropdown, string msgPrefix, Predicate<T> isLevel, UnityAction<T> assignLevel)
	{
		int num = 0;
		dropdown.options.Clear();
		dropdown.onValueChanged.RemoveAllListeners();
		foreach (object obj in Enum.GetValues(typeof(T)))
		{
			T t = (T)((object)obj);
			string name = Enum.GetName(typeof(T), t);
			string levelMsg = this.uiBundle.Xlate(msgPrefix + name.ToLowerInvariant());
			dropdown.options.Add(new TMP_Dropdown.OptionData(levelMsg));
			T fLevel = t;
			int fIdx = num;
			OptionsUI.OnQualityChanged onQualityChanged = delegate()
			{
				if (isLevel(fLevel))
				{
					dropdown.value = fIdx;
					dropdown.captionText.text = levelMsg;
				}
			};
			this.onQualityChanged = (OptionsUI.OnQualityChanged)Delegate.Combine(this.onQualityChanged, onQualityChanged);
			onQualityChanged();
			dropdown.onValueChanged.AddListener(delegate(int val)
			{
				if (val == fIdx)
				{
					assignLevel(fLevel);
					this.NotifyQualityChanged();
				}
			});
			num++;
		}
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x00076FBC File Offset: 0x000751BC
	private void NotifyQualityChanged()
	{
		if (!this.notifyingQualityChanged)
		{
			try
			{
				this.notifyingQualityChanged = true;
				this.onQualityChanged();
			}
			finally
			{
				this.notifyingQualityChanged = false;
			}
		}
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x00077000 File Offset: 0x00075200
	private GameObject CreateConfirmResolutionDialog()
	{
		Resolution oldRes = default(Resolution);
		oldRes.width = Screen.width;
		oldRes.height = Screen.height;
		bool oldFullscreen = Screen.fullScreen;
		oldRes.refreshRate = Screen.currentResolution.refreshRate;
		this.PreventClosing(true);
		ConfirmResolutionUI.OnCancel onCancel = delegate()
		{
			this.optionsDirector.SetScreenResolution(oldRes.width, oldRes.height, oldFullscreen);
			this.fullscreenToggle.isOn = Screen.fullScreen;
			for (int i = 0; i < this.dropdownResolutions.Count; i++)
			{
				Resolution resolution = this.dropdownResolutions[i];
				if (oldRes.width == resolution.width && oldRes.height == resolution.height)
				{
					this.resolutionDropdown.value = i;
					string text = this.uiBundle.Get("m.resolution", new object[]
					{
						resolution.width,
						resolution.height
					});
					this.resolutionDropdown.captionText.text = text;
				}
			}
			this.resolutionApplyButton.Select();
			this.PreventClosing(false);
		};
		ConfirmResolutionUI.OnConfirm onConfirm = delegate()
		{
			this.resolutionApplyButton.Select();
			this.PreventClosing(false);
		};
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.confirmResolutionDialogPrefab);
		gameObject.GetComponent<ConfirmResolutionUI>().onCancel = onCancel;
		gameObject.GetComponent<ConfirmResolutionUI>().onConfirm = onConfirm;
		return gameObject;
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x000770AB File Offset: 0x000752AB
	protected override bool Closeable()
	{
		return base.Closeable() && !this.preventClosing;
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x000770C0 File Offset: 0x000752C0
	public void PreventClosing(bool prevent)
	{
		this.preventClosing = prevent;
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000770CC File Offset: 0x000752CC
	private void SetupVertNav(params Selectable[] selectables)
	{
		List<Selectable> list = new List<Selectable>();
		foreach (Selectable selectable in selectables)
		{
			if (selectable.gameObject.activeSelf)
			{
				list.Add(selectable);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			Navigation navigation = list[j].navigation;
			navigation.mode = Navigation.Mode.Explicit;
			if (j == 0)
			{
				navigation.selectOnUp = null;
			}
			else
			{
				navigation.selectOnUp = list[j - 1];
			}
			if (j == list.Count - 1)
			{
				navigation.selectOnDown = null;
			}
			else
			{
				navigation.selectOnDown = list[j + 1];
			}
			list[j].navigation = navigation;
		}
		if (list.Count > 0)
		{
			list[0].gameObject.AddComponent<InitSelected>();
		}
	}

	// Token: 0x04001E1E RID: 7710
	public GameObject videoPanel;

	// Token: 0x04001E1F RID: 7711
	public GameObject audioPanel;

	// Token: 0x04001E20 RID: 7712
	public GameObject inputPanel;

	// Token: 0x04001E21 RID: 7713
	public GameObject gamepadPanel;

	// Token: 0x04001E22 RID: 7714
	public GameObject modsPanel;

	// Token: 0x04001E23 RID: 7715
	public GameObject otherPanel;

	// Token: 0x04001E24 RID: 7716
	public GameObject tabsPanel;

	// Token: 0x04001E25 RID: 7717
	public GameObject bindingLinePrefab;

	// Token: 0x04001E26 RID: 7718
	public GameObject bindingsPanel;

	// Token: 0x04001E27 RID: 7719
	public GameObject title;

	// Token: 0x04001E28 RID: 7720
	public Toggle disableCameraBobToggle;

	// Token: 0x04001E29 RID: 7721
	public TMP_Dropdown tutorialsDropdown;

	// Token: 0x04001E2A RID: 7722
	public Toggle bufferForGifToggle;

	// Token: 0x04001E2B RID: 7723
	public Toggle vacLockToggle;

	// Token: 0x04001E2C RID: 7724
	public Toggle sprintHoldToggle;

	// Token: 0x04001E2D RID: 7725
	public SECTR_AudioBus masterBus;

	// Token: 0x04001E2E RID: 7726
	public SECTR_AudioBus musicBus;

	// Token: 0x04001E2F RID: 7727
	public SECTR_AudioBus sfxBus;

	// Token: 0x04001E30 RID: 7728
	public Slider masterSlider;

	// Token: 0x04001E31 RID: 7729
	public Slider musicSlider;

	// Token: 0x04001E32 RID: 7730
	public Slider sfxSlider;

	// Token: 0x04001E33 RID: 7731
	public Slider sensitivitySlider;

	// Token: 0x04001E34 RID: 7732
	public GameObject modTogglePrefab;

	// Token: 0x04001E35 RID: 7733
	public RectTransform modListPanel;

	// Token: 0x04001E36 RID: 7734
	public TMP_Dropdown overallDropdown;

	// Token: 0x04001E37 RID: 7735
	public TMP_Dropdown lightingDropdown;

	// Token: 0x04001E38 RID: 7736
	public TMP_Dropdown shadowsDropdown;

	// Token: 0x04001E39 RID: 7737
	public TMP_Dropdown texturesDropdown;

	// Token: 0x04001E3A RID: 7738
	public TMP_Dropdown particlesDropdown;

	// Token: 0x04001E3B RID: 7739
	public TMP_Dropdown modelDetailDropdown;

	// Token: 0x04001E3C RID: 7740
	public TMP_Dropdown waterDetailDropdown;

	// Token: 0x04001E3D RID: 7741
	public TMP_Dropdown antialiasingDropdown;

	// Token: 0x04001E3E RID: 7742
	public Toggle ambientOcclusionToggle;

	// Token: 0x04001E3F RID: 7743
	public Toggle bloomToggle;

	// Token: 0x04001E40 RID: 7744
	public Slider fovSlider;

	// Token: 0x04001E41 RID: 7745
	public Slider otherTabFovSlider;

	// Token: 0x04001E42 RID: 7746
	public TMP_Text fovValText;

	// Token: 0x04001E43 RID: 7747
	public TMP_Text otherTabFovValText;

	// Token: 0x04001E44 RID: 7748
	public GameObject otherTabFovRow;

	// Token: 0x04001E45 RID: 7749
	public Slider overscanSlider;

	// Token: 0x04001E46 RID: 7750
	public TMP_Text overscanValText;

	// Token: 0x04001E47 RID: 7751
	public GameObject overscanFovRow;

	// Token: 0x04001E48 RID: 7752
	public Button resetProfileButton;

	// Token: 0x04001E49 RID: 7753
	public Toggle enableVsyncToggle;

	// Token: 0x04001E4A RID: 7754
	public Toggle enableVsyncOtherRow;

	// Token: 0x04001E4B RID: 7755
	public Toggle enableVsyncVideoToggle;

	// Token: 0x04001E4C RID: 7756
	public TMP_Dropdown resolutionDropdown;

	// Token: 0x04001E4D RID: 7757
	public Toggle fullscreenToggle;

	// Token: 0x04001E4E RID: 7758
	public Button resolutionApplyButton;

	// Token: 0x04001E4F RID: 7759
	public Toggle disableGamepadToggle;

	// Token: 0x04001E50 RID: 7760
	public Toggle swapSticksToggle;

	// Token: 0x04001E51 RID: 7761
	public Toggle invertGamepadLookYToggle;

	// Token: 0x04001E52 RID: 7762
	public Toggle invertMouseLookYToggle;

	// Token: 0x04001E53 RID: 7763
	public Toggle disableMouseLookSmooth;

	// Token: 0x04001E54 RID: 7764
	public Toggle showMinimalHUDToggle;

	// Token: 0x04001E55 RID: 7765
	public Button defaultKeyBtn;

	// Token: 0x04001E56 RID: 7766
	public Button defaultGamepadBtn;

	// Token: 0x04001E57 RID: 7767
	public GameObject confirmResolutionDialogPrefab;

	// Token: 0x04001E58 RID: 7768
	public GameObject videoTab;

	// Token: 0x04001E59 RID: 7769
	public GameObject audioTab;

	// Token: 0x04001E5A RID: 7770
	public GameObject inputTab;

	// Token: 0x04001E5B RID: 7771
	public GameObject gamepadTab;

	// Token: 0x04001E5C RID: 7772
	public XlateText gamepadTabText;

	// Token: 0x04001E5D RID: 7773
	public GameObject modsTab;

	// Token: 0x04001E5E RID: 7774
	public GameObject otherTab;

	// Token: 0x04001E5F RID: 7775
	public TMP_Dropdown languageDropdown;

	// Token: 0x04001E60 RID: 7776
	private bool initializing;

	// Token: 0x04001E61 RID: 7777
	private List<Resolution> dropdownResolutions = new List<Resolution>();

	// Token: 0x04001E62 RID: 7778
	private Dictionary<BindingLineUI, string> labelKeyDict = new Dictionary<BindingLineUI, string>();

	// Token: 0x04001E63 RID: 7779
	private bool preventClosing;

	// Token: 0x04001E64 RID: 7780
	private OptionsDirector optionsDirector;

	// Token: 0x04001E65 RID: 7781
	public const int MIN_WIDTH = 800;

	// Token: 0x04001E66 RID: 7782
	public const int MIN_HEIGHT = 600;

	// Token: 0x04001E67 RID: 7783
	private OptionsUI.OnQualityChanged onQualityChanged;

	// Token: 0x04001E68 RID: 7784
	private bool notifyingQualityChanged;

	// Token: 0x020005D0 RID: 1488
	// (Invoke) Token: 0x06001F36 RID: 7990
	private delegate void OnQualityChanged();
}
