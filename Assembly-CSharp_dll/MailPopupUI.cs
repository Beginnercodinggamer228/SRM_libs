using System;
using TMPro;
using UnityEngine;

// Token: 0x020005B7 RID: 1463
public class MailPopupUI : PopupUI<MailDirector.Mail>, PopupDirector.Popup
{
	// Token: 0x06001E4B RID: 7755 RVA: 0x00072DBF File Offset: 0x00070FBF
	public virtual void Awake()
	{
		this.timeOfDeath = Time.time + this.lifetime;
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.popupDir.PopupActivated(this);
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x00072DF0 File Offset: 0x00070FF0
	public override void OnBundleAvailable(MessageDirector msgDir)
	{
		TMP_Text component = base.transform.Find("UIContainer/MainPanel/IntroPanel/Intro").GetComponent<TMP_Text>();
		MessageBundle bundle = msgDir.GetBundle("mail");
		MessageBundle bundle2 = msgDir.GetBundle("ui");
		string key = this.idEntry.key;
		component.text = bundle2.Get("m.mail_from_wrap", new string[]
		{
			bundle.Get("m.from." + key)
		});
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x00072E60 File Offset: 0x00071060
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.popupDir.PopupDeactivated(this);
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x00072E74 File Offset: 0x00071074
	public void Update()
	{
		if (Time.time >= this.timeOfDeath)
		{
			Destroyer.Destroy(base.gameObject, "MailPopupUI.Update");
		}
	}

	// Token: 0x06001E4F RID: 7759 RVA: 0x00072E93 File Offset: 0x00071093
	public MailDirector.Mail GetId()
	{
		return this.idEntry;
	}

	// Token: 0x06001E50 RID: 7760 RVA: 0x00072E9B File Offset: 0x0007109B
	public bool ShouldClear()
	{
		return this.idEntry.read;
	}

	// Token: 0x06001E51 RID: 7761 RVA: 0x00072EA8 File Offset: 0x000710A8
	public static GameObject CreateMailPopup(MailDirector.Mail mail)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(SRSingleton<GameContext>.Instance.UITemplates.mailPrefab);
		gameObject.GetComponent<MailPopupUI>().Init(mail);
		return gameObject;
	}

	// Token: 0x04001D62 RID: 7522
	[Tooltip("If not killed before then, how long this popup will stick around.")]
	public float lifetime = 10f;

	// Token: 0x04001D63 RID: 7523
	protected float timeOfDeath;

	// Token: 0x04001D64 RID: 7524
	protected PopupDirector popupDir;
}
