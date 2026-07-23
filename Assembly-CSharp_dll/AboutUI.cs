using System;
using UnityEngine.UI;

// Token: 0x02000535 RID: 1333
public class AboutUI : BaseUI
{
	// Token: 0x06001BB6 RID: 7094 RVA: 0x00069E98 File Offset: 0x00068098
	public void OnEnable()
	{
		bool active = SRSingleton<SceneContext>.Instance.AchievementsDirector.HasAchievement(AchievementsDirector.Achievement.FINISH_ADVENTURE);
		this.creditsBtn.gameObject.SetActive(active);
		this.dataPrivacyBtn.gameObject.SetActive(true);
		if (this.creditsBtn.gameObject.activeSelf)
		{
			this.creditsBtn.gameObject.AddComponent<InitSelected>();
			return;
		}
		if (this.dataPrivacyBtn.gameObject.activeSelf)
		{
			this.dataPrivacyBtn.gameObject.AddComponent<InitSelected>();
		}
	}

	// Token: 0x06001BB7 RID: 7095 RVA: 0x00069F20 File Offset: 0x00068120
	public void ShowCredits()
	{
		this.creditsBtn.interactable = false;
		CreditsUI component = SRSingleton<GameContext>.Instance.UITemplates.CreateCreditsPrefab(true).GetComponent<CreditsUI>();
		component.OnCreditsEnded = (CreditsUI.OnCreditsEndedEvent)Delegate.Combine(component.OnCreditsEnded, new CreditsUI.OnCreditsEndedEvent(this.OnCreditsEnded));
	}

	// Token: 0x06001BB8 RID: 7096 RVA: 0x00069F6F File Offset: 0x0006816F
	private void OnCreditsEnded()
	{
		this.creditsBtn.interactable = true;
	}

	// Token: 0x04001AD6 RID: 6870
	public Button creditsBtn;

	// Token: 0x04001AD7 RID: 6871
	public Button dataPrivacyBtn;
}
