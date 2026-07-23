using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000538 RID: 1336
public class AchievementAwardUI : PopupUI<AchievementsDirector.Achievement>
{
	// Token: 0x06001BC3 RID: 7107 RVA: 0x0006A221 File Offset: 0x00068421
	public virtual void Awake()
	{
		this.timeOfDeath = Time.time + this.lifetime;
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.achieveDir.PopupActivated(this);
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x0006A251 File Offset: 0x00068451
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.achieveDir.PopupDeactivated(this);
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x0006A265 File Offset: 0x00068465
	public void Update()
	{
		if (Time.time >= this.timeOfDeath)
		{
			Destroyer.Destroy(base.gameObject, "AchievementAwardUI.Update");
		}
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x0006A284 File Offset: 0x00068484
	public override void OnBundleAvailable(MessageDirector msgDir)
	{
		string text = Enum.GetName(typeof(AchievementsDirector.Achievement), this.idEntry).ToLowerInvariant();
		Sprite achievementImage = this.achieveDir.GetAchievementImage(text, this.idEntry);
		MessageBundle bundle = msgDir.GetBundle("achieve");
		this.img.sprite = achievementImage;
		this.titleText.text = bundle.Get("t." + text);
	}

	// Token: 0x04001ADE RID: 6878
	public Image img;

	// Token: 0x04001ADF RID: 6879
	public TMP_Text titleText;

	// Token: 0x04001AE0 RID: 6880
	[Tooltip("If not killed before then, how long this popup will stick around.")]
	public float lifetime = 10f;

	// Token: 0x04001AE1 RID: 6881
	protected float timeOfDeath;

	// Token: 0x04001AE2 RID: 6882
	private AchievementsDirector achieveDir;
}
