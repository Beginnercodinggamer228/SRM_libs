using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000539 RID: 1337
public class AchievementsUI : BaseUI
{
	// Token: 0x06001BC8 RID: 7112 RVA: 0x0006A30C File Offset: 0x0006850C
	public override void Awake()
	{
		base.Awake();
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.achieveBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("achieve");
		foreach (object obj in Enum.GetValues(typeof(AchievementsDirector.Achievement)))
		{
			AchievementsDirector.Achievement achievement = (AchievementsDirector.Achievement)obj;
			this.AddAchievement(achievement);
		}
		int num;
		int num2;
		this.achieveDir.GetOverallProgress(out num, out num2);
		this.overallText.text = this.uiBundle.Get("m.achieve_overall_progress", new object[]
		{
			num,
			num2
		});
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x0006A3E4 File Offset: 0x000685E4
	public void AddAchievement(AchievementsDirector.Achievement achievement)
	{
		this.CreateAchievement(achievement).transform.SetParent(this.achievementListPanel.transform, false);
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x0006A404 File Offset: 0x00068604
	private GameObject CreateAchievement(AchievementsDirector.Achievement achievement)
	{
		bool flag = this.achieveDir.HasAchievement(achievement);
		int num;
		int num2;
		this.achieveDir.GetProgress(achievement, out num, out num2);
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.achievementListItemPrefab);
		TMP_Text component = gameObject.transform.Find("InfoPanel/Name").GetComponent<TMP_Text>();
		TMP_Text component2 = gameObject.transform.Find("InfoPanel/Requirement").GetComponent<TMP_Text>();
		Image component3 = gameObject.transform.Find("Icon").GetComponent<Image>();
		Image component4 = gameObject.transform.Find("Complete").GetComponent<Image>();
		GameObject gameObject2 = gameObject.transform.Find("InfoPanel/Progress").gameObject;
		StatusBar component5 = gameObject.transform.Find("InfoPanel/Progress/ProgressMeter").GetComponent<StatusBar>();
		TMP_Text component6 = gameObject.transform.Find("InfoPanel/Progress/ProgressText").GetComponent<TMP_Text>();
		string text = achievement.ToString().ToLowerInvariant();
		component.text = this.achieveBundle.Xlate("t." + text);
		component2.text = this.achieveBundle.Xlate("m.reqmt." + text);
		component3.sprite = this.achieveDir.GetAchievementImage(text, achievement);
		if (!flag)
		{
			component3.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		}
		component4.enabled = flag;
		if (num2 > 1 && !flag)
		{
			gameObject2.SetActive(true);
			component5.maxValue = (float)num2;
			component5.currValue = (float)num;
			component6.text = this.uiBundle.Get("m.achieve_progress", new object[]
			{
				num,
				num2
			});
		}
		else
		{
			gameObject2.SetActive(false);
		}
		return gameObject;
	}

	// Token: 0x04001AE3 RID: 6883
	[Tooltip("Internal achievement content panel.")]
	public GameObject achievementListPanel;

	// Token: 0x04001AE4 RID: 6884
	[Tooltip("The prefab from which to create individual achievement panels.")]
	public GameObject achievementListItemPrefab;

	// Token: 0x04001AE5 RID: 6885
	[Tooltip("Internal overall achievement count text.")]
	public TMP_Text overallText;

	// Token: 0x04001AE6 RID: 6886
	private MessageBundle achieveBundle;

	// Token: 0x04001AE7 RID: 6887
	private AchievementsDirector achieveDir;
}
