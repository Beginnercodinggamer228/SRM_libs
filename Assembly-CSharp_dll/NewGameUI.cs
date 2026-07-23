using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005CC RID: 1484
public class NewGameUI : BaseUI
{
	// Token: 0x06001EE6 RID: 7910 RVA: 0x000754E0 File Offset: 0x000736E0
	public override void Awake()
	{
		base.Awake();
		this.autoSaveDirector = SRSingleton<GameContext>.Instance.AutoSaveDirector;
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000754F8 File Offset: 0x000736F8
	public void Start()
	{
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
		HashSet<string> availableGameDisplayNames = this.GetAvailableGameDisplayNames();
		for (int i = 1; i < 1000; i++)
		{
			string text = bundle.Get("m.default_game_name", new object[]
			{
				i
			});
			if (!availableGameDisplayNames.Contains(text))
			{
				this.gameNameField.text = text;
				break;
			}
		}
		this.classicToggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this.SetGameMode(PlayerState.GameMode.CLASSIC);
			}
		});
		this.casualToggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this.SetGameMode(PlayerState.GameMode.CASUAL);
			}
		});
		this.timeLimitToggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this.SetGameMode(PlayerState.GameMode.TIME_LIMIT_V2);
			}
		});
		this.SetGameMode(PlayerState.GameMode.CLASSIC);
		this.iconToggles = new Toggle[this.availIconIds.Length];
		bool flag = true;
		LookupDirector lookupDirector = SRSingleton<GameContext>.Instance.LookupDirector;
		for (int j = 0; j < this.availIconIds.Length; j++)
		{
			Identifiable.Id id = this.availIconIds[j];
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gameIconPrefab);
			gameObject.transform.SetParent(this.iconGroup.transform, false);
			Toggle toggle = gameObject.GetComponent<Toggle>();
			gameObject.transform.Find("GameIcon").GetComponent<Image>().sprite = lookupDirector.GetIcon(id);
			toggle.group = this.iconGroup;
			this.iconToggles[j] = toggle;
			int idxToSet = j;
			toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn && !this.settingToggleStates)
				{
					this.SetIconIdIdx(idxToSet);
				}
			});
			OnSelectDelegator.Create(toggle.gameObject, delegate
			{
				toggle.isOn = true;
			});
			if (flag)
			{
				flag = false;
				toggle.isOn = true;
			}
		}
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000756ED File Offset: 0x000738ED
	private HashSet<string> GetAvailableGameDisplayNames()
	{
		return new HashSet<string>(this.autoSaveDirector.AvailableGamesByDisplayName().Keys);
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x00075704 File Offset: 0x00073904
	public void PlayNewGame()
	{
		string text = this.gameNameField.text;
		if (!this.autoSaveDirector.DisplayNameAvailable(text))
		{
			this.waitForErrorDialog = SRSingleton<GameContext>.Instance.UITemplates.CreateErrorDialog("e.game_name_exists");
			return;
		}
		if (text.Length > 24 || text.Length < 1)
		{
			this.waitForErrorDialog = SRSingleton<GameContext>.Instance.UITemplates.CreateErrorDialogWithArgs("e.max_length", new object[]
			{
				24
			});
			return;
		}
		this.playButton.interactable = false;
		base.gameObject.SetActive(false);
		SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame(text, this.GetGameIconId(), this.GetGameMode(), delegate
		{
			this.playButton.interactable = true;
			base.gameObject.SetActive(true);
		});
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x000757C4 File Offset: 0x000739C4
	protected override bool Closeable()
	{
		return this.waitForErrorDialog == null;
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x000757D2 File Offset: 0x000739D2
	private PlayerState.GameMode GetGameMode()
	{
		return this.selGameMode;
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x000757DC File Offset: 0x000739DC
	private void SetGameMode(PlayerState.GameMode mode)
	{
		this.selGameMode = mode;
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
		if (mode == PlayerState.GameMode.TIME_LIMIT_V2)
		{
			int? stat = SRSingleton<SceneContext>.Instance.AchievementsDirector.GetStat(AchievementsDirector.IntStat.TIME_LIMIT_V2_CURRENCY);
			string key = string.Format("m.desc.gamemode_{0}{1}", mode.ToString().ToLowerInvariant(), (stat == null) ? string.Empty : "_high_score");
			this.gameModeText.text = bundle.Get(key, new object[]
			{
				stat
			});
			return;
		}
		string key2 = string.Format("m.desc.gamemode_{0}", mode.ToString().ToLowerInvariant());
		this.gameModeText.text = bundle.Get(key2);
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x000758A0 File Offset: 0x00073AA0
	private void SetIconIdIdx(int idx)
	{
		this.selIconIdIdx = idx;
		try
		{
			this.settingToggleStates = true;
			this.iconToggles[idx].isOn = true;
			this.leftIconButton.interactable = (idx > 0);
			this.rightIconButton.interactable = (idx < this.iconToggles.Length - 1);
			this.iconTabByMenuKeys.RecalcSelected();
		}
		finally
		{
			this.settingToggleStates = false;
		}
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x00075918 File Offset: 0x00073B18
	private Identifiable.Id GetGameIconId()
	{
		return this.availIconIds[this.selIconIdIdx];
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x00070A85 File Offset: 0x0006EC85
	public override void Close()
	{
		base.Close();
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x00075927 File Offset: 0x00073B27
	public void SelectNextIcon()
	{
		this.SetIconIdIdx(Math.Min(this.availIconIds.Length - 1, this.selIconIdIdx + 1));
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x00075946 File Offset: 0x00073B46
	public void SelectPrevIcon()
	{
		this.SetIconIdIdx(Math.Max(0, this.selIconIdIdx - 1));
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x0007595C File Offset: 0x00073B5C
	public override void Update()
	{
		base.Update();
		this.iconTabByMenuKeys.enabled = (!this.gameNameField.isFocused || InputDirector.UsingGamepad());
	}

	// Token: 0x04001E03 RID: 7683
	public GameObject mainMenuUIPrefab;

	// Token: 0x04001E04 RID: 7684
	public InputField gameNameField;

	// Token: 0x04001E05 RID: 7685
	public Button playButton;

	// Token: 0x04001E06 RID: 7686
	public Toggle classicToggle;

	// Token: 0x04001E07 RID: 7687
	public Toggle casualToggle;

	// Token: 0x04001E08 RID: 7688
	public Toggle timeLimitToggle;

	// Token: 0x04001E09 RID: 7689
	public ToggleGroup iconGroup;

	// Token: 0x04001E0A RID: 7690
	public TMP_Text gameModeText;

	// Token: 0x04001E0B RID: 7691
	public GameObject gameIconPrefab;

	// Token: 0x04001E0C RID: 7692
	public Button leftIconButton;

	// Token: 0x04001E0D RID: 7693
	public Button rightIconButton;

	// Token: 0x04001E0E RID: 7694
	public Identifiable.Id[] availIconIds;

	// Token: 0x04001E0F RID: 7695
	[Tooltip("TabByMenuKeys attached to the icon selection scrollview.")]
	public TabByMenuKeys iconTabByMenuKeys;

	// Token: 0x04001E10 RID: 7696
	private PlayerState.GameMode selGameMode;

	// Token: 0x04001E11 RID: 7697
	private int selIconIdIdx;

	// Token: 0x04001E12 RID: 7698
	private Toggle[] iconToggles;

	// Token: 0x04001E13 RID: 7699
	private bool settingToggleStates;

	// Token: 0x04001E14 RID: 7700
	private const string ERR_EXISTS = "e.game_name_exists";

	// Token: 0x04001E15 RID: 7701
	private const string ERR_LETTERS_NUMS_ONLY = "e.letters_nums_only";

	// Token: 0x04001E16 RID: 7702
	private const string ERR_MAX_LENGTH = "e.max_length";

	// Token: 0x04001E17 RID: 7703
	private const int GAME_NAME_MAX_LENGTH = 24;

	// Token: 0x04001E18 RID: 7704
	private AutoSaveDirector autoSaveDirector;

	// Token: 0x04001E19 RID: 7705
	private GameObject waitForErrorDialog;
}
