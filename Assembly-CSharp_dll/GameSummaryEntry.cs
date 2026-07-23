using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200058D RID: 1421
public class GameSummaryEntry : MonoBehaviour
{
	// Token: 0x06001D74 RID: 7540 RVA: 0x0006FE00 File Offset: 0x0006E000
	public void Init(GameData.Summary gameSummary)
	{
		this.gameIcon.sprite = SRSingleton<GameContext>.Instance.LookupDirector.GetIcon((gameSummary.iconId == Identifiable.Id.NONE) ? Identifiable.Id.CARROT_VEGGIE : gameSummary.iconId);
		this.gameName = gameSummary.name;
		this.saveName = gameSummary.saveName;
		this.gameNameText.text = gameSummary.displayName;
	}

	// Token: 0x06001D75 RID: 7541 RVA: 0x0006FE62 File Offset: 0x0006E062
	public string GetGameName()
	{
		return this.gameName;
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x0006FE6A File Offset: 0x0006E06A
	public string GetSaveName()
	{
		return this.saveName;
	}

	// Token: 0x04001C80 RID: 7296
	public Image gameIcon;

	// Token: 0x04001C81 RID: 7297
	public TMP_Text gameNameText;

	// Token: 0x04001C82 RID: 7298
	public string gameName;

	// Token: 0x04001C83 RID: 7299
	public string saveName;
}
