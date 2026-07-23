using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200058E RID: 1422
public class GameSummaryPanel : MonoBehaviour
{
	// Token: 0x06001D78 RID: 7544 RVA: 0x0006FE74 File Offset: 0x0006E074
	public void Init(GameData.Summary gameSummary)
	{
		MessageBundle bundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui");
		this.gameIcon.sprite = SRSingleton<GameContext>.Instance.LookupDirector.GetIcon((gameSummary.iconId == Identifiable.Id.NONE) ? Identifiable.Id.CARROT_VEGGIE : gameSummary.iconId);
		this.gameNameText.text = gameSummary.displayName;
		string str = gameSummary.gameMode.ToString().ToLowerInvariant();
		this.modeText.text = bundle.Xlate("m.gamemode_" + str);
		this.modeDescText.text = bundle.Xlate("m.desc.gamemode_" + str);
		this.dayText.text = bundle.Xlate(MessageUtil.Tcompose("m.day", new object[]
		{
			gameSummary.day + 1
		}));
		this.currencyText.text = gameSummary.currency.ToString();
		int num = Math.Max(0, gameSummary.pediaCount - SRSingleton<SceneContext>.Instance.PediaDirector.GetUnlockedCount());
		this.pediaText.text = bundle.Xlate(MessageUtil.Tcompose("l.pedia_count", new object[]
		{
			num
		}));
		this.versionText.text = ((gameSummary.version == null) ? "pre-0.3.0" : gameSummary.version);
		this.validPanel.SetActive(!gameSummary.isInvalid);
		this.invalidPanel.SetActive(gameSummary.isInvalid);
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x0006FFF5 File Offset: 0x0006E1F5
	public string GetGameName()
	{
		return this.gameNameText.text;
	}

	// Token: 0x04001C84 RID: 7300
	public Image gameIcon;

	// Token: 0x04001C85 RID: 7301
	public TMP_Text gameNameText;

	// Token: 0x04001C86 RID: 7302
	public TMP_Text modeText;

	// Token: 0x04001C87 RID: 7303
	public TMP_Text modeDescText;

	// Token: 0x04001C88 RID: 7304
	public TMP_Text dayText;

	// Token: 0x04001C89 RID: 7305
	public TMP_Text currencyText;

	// Token: 0x04001C8A RID: 7306
	public TMP_Text pediaText;

	// Token: 0x04001C8B RID: 7307
	public TMP_Text versionText;

	// Token: 0x04001C8C RID: 7308
	public GameObject validPanel;

	// Token: 0x04001C8D RID: 7309
	public GameObject invalidPanel;
}
