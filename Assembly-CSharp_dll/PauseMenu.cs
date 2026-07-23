using System;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020005D9 RID: 1497
public class PauseMenu : SRSingleton<PauseMenu>
{
	// Token: 0x06001F5D RID: 8029 RVA: 0x0007758C File Offset: 0x0007578C
	public override void Awake()
	{
		base.Awake();
		this.bugReportButton.gameObject.SetActive(true);
		this.screenshotButton.gameObject.SetActive(true);
		this.achievementsButton.gameObject.SetActive(true);
		XlateText[] componentsInChildren = this.quitButton.GetComponentsInChildren<XlateText>(true);
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			componentsInChildren[0].key = "b.save_and_quit";
		}
		this.invertViewYAxisPanel.SetActive(false);
		InputManager.OnDeviceDetached += this.PauseOnDeviceDetach;
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x00077610 File Offset: 0x00075810
	private void Start()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.pauseUI.SetActive(false);
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x00077630 File Offset: 0x00075830
	public void Update()
	{
		if ((SRInput.Actions.menu.WasPressed || SRInput.PauseActions.unmenu.WasPressed) && !this.timeDir.IsFastForwarding())
		{
			if (this.pauseUI.activeSelf)
			{
				if (this.timeDir.ExactlyOnePauser() && !this.suppressUnpause)
				{
					this.UnPauseGame();
					return;
				}
			}
			else if (Time.timeScale > 0f)
			{
				this.PauseGame();
				return;
			}
		}
		else if (SRInput.PauseActions.cancel.WasPressed && !this.suppressUnpause && this.pauseUI.activeSelf && this.timeDir.ExactlyOnePauser())
		{
			this.UnPauseGame();
		}
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x000776E1 File Offset: 0x000758E1
	public bool IsOnlyPauser()
	{
		return this.pauseUI.activeSelf && this.timeDir.ExactlyOnePauser();
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x000776FD File Offset: 0x000758FD
	public void PauseGame()
	{
		SRInput.Instance.SetInputMode(SRInput.InputMode.PAUSE, base.gameObject.GetInstanceID());
		this.pauseUI.SetActive(true);
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x00077721 File Offset: 0x00075921
	public void UnPauseGame()
	{
		this.pauseUI.SetActive(false);
		SRInput.Instance.ClearInputMode(base.gameObject.GetInstanceID());
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x00077744 File Offset: 0x00075944
	public void Resume()
	{
		this.UnPauseGame();
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x0007774C File Offset: 0x0007594C
	public void Pedia()
	{
		this.InstantiateAndWait(SRSingleton<SceneContext>.Instance.PediaDirector.pediaPanelPrefab);
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x00077764 File Offset: 0x00075964
	public void Achievements()
	{
		this.InstantiateAndWait(SRSingleton<SceneContext>.Instance.AchievementsDirector.achievementsPanelPrefab);
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x0007777C File Offset: 0x0007597C
	public void EmergencyReturn()
	{
		this.WaitFor(SRSingleton<GameContext>.Instance.UITemplates.CreateConfirmDialog("m.emergency_return", delegate
		{
			DeathHandler.Kill(SRSingleton<SceneContext>.Instance.Player, DeathHandler.Source.EMERGENCY_RETURN, null, "PauseGame.EmergencyReturn");
			this.UnPauseGame();
		}));
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x0006E7A4 File Offset: 0x0006C9A4
	public void Quit()
	{
		if (SRSingleton<GameContext>.Instance.AutoSaveDirector.SaveAllNow())
		{
			SRSingleton<SceneContext>.Instance.OnSessionEnded();
			SceneManager.LoadScene("MainMenu");
		}
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x000777A4 File Offset: 0x000759A4
	public void Options()
	{
		this.InstantiateAndWait(this.optionsUI);
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x000777B3 File Offset: 0x000759B3
	public void ReportIssue()
	{
		this.InstantiateAndWait(this.bugReportUI);
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x000777C4 File Offset: 0x000759C4
	public GameObject InstantiateAndWait(GameObject prefab)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
		this.WaitFor(gameObject);
		return gameObject;
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x000777E0 File Offset: 0x000759E0
	public void WaitFor(GameObject uiObj)
	{
		BaseUI component = uiObj.GetComponent<BaseUI>();
		base.gameObject.SetActive(false);
		component.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(component.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
		{
			if (this != null && base.gameObject != null)
			{
				base.gameObject.SetActive(true);
			}
		}));
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x0006E798 File Offset: 0x0006C998
	public void Screenshot()
	{
		SRSingleton<GameContext>.Instance.TakeScreenshot();
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x00077815 File Offset: 0x00075A15
	public void OnEnable()
	{
		this.invertViewYAxisToggle.isOn = SRSingleton<GameContext>.Instance.InputDirector.GetInvertGamepadLookY();
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x00003296 File Offset: 0x00001496
	public void OnDisable()
	{
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x00077831 File Offset: 0x00075A31
	public void OnToggleYAxis()
	{
		SRSingleton<GameContext>.Instance.InputDirector.SetInvertGamepadLookY(this.invertViewYAxisToggle.isOn);
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x0007784D File Offset: 0x00075A4D
	public override void OnDestroy()
	{
		base.OnDestroy();
		SRInput.Instance.ClearInputMode(base.gameObject.GetInstanceID());
		InputManager.OnDeviceDetached -= this.PauseOnDeviceDetach;
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x0007787B File Offset: 0x00075A7B
	private void PauseOnDeviceDetach(InputDevice device)
	{
		if (Time.timeScale > 0f && !this.timeDir.IsFastForwarding() && SRInput.Instance.GetInputMode() == SRInput.InputMode.DEFAULT)
		{
			this.PauseGame();
		}
	}

	// Token: 0x04001E8E RID: 7822
	public GameObject pauseUI;

	// Token: 0x04001E8F RID: 7823
	public GameObject optionsUI;

	// Token: 0x04001E90 RID: 7824
	public GameObject bugReportUI;

	// Token: 0x04001E91 RID: 7825
	public Button resumeButton;

	// Token: 0x04001E92 RID: 7826
	public Button pediaButton;

	// Token: 0x04001E93 RID: 7827
	public Button achievementsButton;

	// Token: 0x04001E94 RID: 7828
	public Button optionsButton;

	// Token: 0x04001E95 RID: 7829
	public Button bugReportButton;

	// Token: 0x04001E96 RID: 7830
	public Button screenshotButton;

	// Token: 0x04001E97 RID: 7831
	public Button emergencyResetButton;

	// Token: 0x04001E98 RID: 7832
	public Button quitButton;

	// Token: 0x04001E99 RID: 7833
	public Toggle invertViewYAxisToggle;

	// Token: 0x04001E9A RID: 7834
	public GameObject invertViewYAxisPanel;

	// Token: 0x04001E9B RID: 7835
	private TimeDirector timeDir;

	// Token: 0x04001E9C RID: 7836
	private bool suppressUnpause;

	// Token: 0x04001E9D RID: 7837
	private SRInput.InputMode? previousInputMode;
}
