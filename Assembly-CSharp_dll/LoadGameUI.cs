using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005AE RID: 1454
public class LoadGameUI : BaseUI
{
	// Token: 0x06001E1B RID: 7707 RVA: 0x000724F9 File Offset: 0x000706F9
	public override void Awake()
	{
		base.Awake();
		this.autoSaveDirector = SRSingleton<GameContext>.Instance.AutoSaveDirector;
		this.UpdateAvailGames();
	}

	// Token: 0x06001E1C RID: 7708 RVA: 0x00072518 File Offset: 0x00070718
	private void UpdateAvailGames()
	{
		foreach (Toggle toggle in this.gameToggles)
		{
			Destroyer.Destroy(toggle.gameObject, "LoadGameUI.UpdateAvailGames");
		}
		this.gameToggles.Clear();
		this.noSavesPanel.gameObject.SetActive(true);
		this.loadingPanel.SetActive(true);
		base.StartCoroutine(this.FinishUpdateAvailGames());
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x000725A8 File Offset: 0x000707A8
	private IEnumerator FinishUpdateAvailGames()
	{
		yield return new WaitForSeconds(0f);
		this.availGames.Clear();
		foreach (KeyValuePair<string, List<GameData.Summary>> keyValuePair in this.autoSaveDirector.AvailableGamesByDisplayName())
		{
			this.availGames.Add(keyValuePair.Value[0]);
		}
		this.loadingPanel.SetActive(false);
		this.summaryPanel.gameObject.SetActive(this.availGames.Count > 0);
		this.noSavesPanel.gameObject.SetActive(this.availGames.Count <= 0);
		foreach (GameData.Summary gameSummary in this.availGames)
		{
			GameObject gameObject = this.CreateLoadGameButton(gameSummary);
			gameObject.transform.SetParent(this.loadButtonPanel.transform, false);
			this.gameToggles.Add(gameObject.GetComponent<Toggle>());
		}
		if (this.gameToggles.Count > 0)
		{
			this.gameToggles[0].gameObject.AddComponent<InitSelected>();
		}
		for (int i = 0; i < this.gameToggles.Count; i++)
		{
			Navigation navigation = default(Navigation);
			navigation.mode = Navigation.Mode.Explicit;
			if (i > 0)
			{
				navigation.selectOnUp = this.gameToggles[i - 1];
			}
			if (i < this.gameToggles.Count - 1)
			{
				navigation.selectOnDown = this.gameToggles[i + 1];
			}
			this.gameToggles[i].navigation = navigation;
			this.AddToggleListener(i);
		}
		if (this.availGames.Count > 0)
		{
			this.SetSelectedIdx(0);
		}
		base.StartCoroutine(this.ScrollToTop());
		yield break;
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000725B8 File Offset: 0x000707B8
	private void AddToggleListener(int idx)
	{
		this.gameToggles[idx].onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn && !this.settingToggleStates)
			{
				this.SetSelectedIdx(idx);
			}
		});
	}

	// Token: 0x06001E1F RID: 7711 RVA: 0x00072600 File Offset: 0x00070800
	private IEnumerator ScrollToTop()
	{
		yield return new WaitForEndOfFrame();
		this.scroller.verticalNormalizedPosition = 1f;
		yield break;
	}

	// Token: 0x06001E20 RID: 7712 RVA: 0x00072610 File Offset: 0x00070810
	public void LoadSelectedGame()
	{
		GameSummaryEntry gameSummaryEntry = this.SelectedGame();
		if (gameSummaryEntry != null)
		{
			this.LoadGame(gameSummaryEntry.gameName, gameSummaryEntry.saveName);
		}
	}

	// Token: 0x06001E21 RID: 7713 RVA: 0x00072640 File Offset: 0x00070840
	public void DeleteSelectedGame()
	{
		GameSummaryEntry gameSummaryEntry = this.SelectedGame();
		if (gameSummaryEntry != null)
		{
			this.DeleteGame(gameSummaryEntry.saveName);
		}
	}

	// Token: 0x06001E22 RID: 7714 RVA: 0x00070A85 File Offset: 0x0006EC85
	public override void Close()
	{
		base.Close();
	}

	// Token: 0x06001E23 RID: 7715 RVA: 0x0007266C File Offset: 0x0007086C
	private void SetSelectedIdx(int idx)
	{
		this.selectedIdx = idx;
		try
		{
			this.settingToggleStates = true;
			this.gameToggles[idx].Select();
			this.summaryPanel.Init(this.availGames[idx]);
			this.playButton.interactable = (!this.availGames[idx].isInvalid && !this.availGames[idx].gameOver && !this.autoSaveDirector.IsLoadingGame());
		}
		finally
		{
			this.settingToggleStates = false;
		}
	}

	// Token: 0x06001E24 RID: 7716 RVA: 0x0007270C File Offset: 0x0007090C
	public void SelectNextGame()
	{
		this.SetSelectedIdx(Math.Min(this.gameToggles.Count - 1, this.selectedIdx + 1));
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x0007272E File Offset: 0x0007092E
	public void SelectPrevGame()
	{
		this.SetSelectedIdx(Math.Max(0, this.selectedIdx - 1));
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x00072744 File Offset: 0x00070944
	private void DeleteGame(string saveName)
	{
		GameData.Summary gameToDelete = this.autoSaveDirector.LoadSummary(saveName);
		base.gameObject.SetActive(false);
		this.CreateDeleteGameDialog(gameToDelete, delegate
		{
			this.gameObject.SetActive(true);
			this.autoSaveDirector.DeleteGame(gameToDelete.name);
		}, delegate
		{
			this.gameObject.SetActive(true);
			this.UpdateAvailGames();
		});
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x000727A4 File Offset: 0x000709A4
	private GameSummaryEntry SelectedGame()
	{
		ToggleGroup component = this.loadButtonPanel.GetComponent<ToggleGroup>();
		if (component.GetActive() != null)
		{
			return component.GetActive().GetComponent<GameSummaryEntry>();
		}
		return null;
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x000727D8 File Offset: 0x000709D8
	private void LoadGame(string gameName, string saveName)
	{
		base.gameObject.SetActive(false);
		this.autoSaveDirector.BeginLoad(gameName, saveName, delegate
		{
			base.gameObject.SetActive(true);
		});
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x00072800 File Offset: 0x00070A00
	private GameObject CreateLoadGameButton(GameData.Summary gameSummary)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.loadGameButtonPrefab);
		Toggle toggle = gameObject.GetComponent<Toggle>();
		toggle.group = this.loadButtonPanel.GetComponent<ToggleGroup>();
		gameObject.GetComponent<GameSummaryEntry>().Init(gameSummary);
		OnSelectDelegator.Create(gameObject, delegate
		{
			toggle.isOn = true;
		});
		return gameObject;
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x00072861 File Offset: 0x00070A61
	private GameObject CreateDeleteGameDialog(GameData.Summary gameSummary, ConfirmUI.OnConfirm onConfirm, BaseUI.OnDestroyDelegate onDestroy)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.deleteUIPrefab);
		gameObject.GetComponent<ConfirmUI>().onConfirm = onConfirm;
		gameObject.GetComponent<ConfirmUI>().onDestroy = onDestroy;
		gameObject.transform.Find("MainPanel/GameSummaryPanel").GetComponent<GameSummaryPanel>().Init(gameSummary);
		return gameObject;
	}

	// Token: 0x04001D3C RID: 7484
	public GameObject loadGameButtonPrefab;

	// Token: 0x04001D3D RID: 7485
	public GameObject loadButtonPanel;

	// Token: 0x04001D3E RID: 7486
	public GameObject mainMenuUIPrefab;

	// Token: 0x04001D3F RID: 7487
	public GameObject deleteUIPrefab;

	// Token: 0x04001D40 RID: 7488
	public ScrollRect scroller;

	// Token: 0x04001D41 RID: 7489
	public TMP_Text status;

	// Token: 0x04001D42 RID: 7490
	public Button playButton;

	// Token: 0x04001D43 RID: 7491
	public Button deleteButton;

	// Token: 0x04001D44 RID: 7492
	public Button backButton;

	// Token: 0x04001D45 RID: 7493
	public GameObject loadingPanel;

	// Token: 0x04001D46 RID: 7494
	public GameSummaryPanel summaryPanel;

	// Token: 0x04001D47 RID: 7495
	public GameObject noSavesPanel;

	// Token: 0x04001D48 RID: 7496
	private List<Toggle> gameToggles = new List<Toggle>();

	// Token: 0x04001D49 RID: 7497
	private int selectedIdx;

	// Token: 0x04001D4A RID: 7498
	private bool settingToggleStates;

	// Token: 0x04001D4B RID: 7499
	private List<GameData.Summary> availGames = new List<GameData.Summary>();

	// Token: 0x04001D4C RID: 7500
	private AutoSaveDirector autoSaveDirector;
}
