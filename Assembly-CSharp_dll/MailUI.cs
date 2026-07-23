using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020005B8 RID: 1464
public class MailUI : BaseUI
{
	// Token: 0x06001E53 RID: 7763 RVA: 0x00072EE0 File Offset: 0x000710E0
	public override void Awake()
	{
		base.Awake();
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.mailDir = SRSingleton<SceneContext>.Instance.MailDirector;
		this.mailBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("mail");
		this.contentScroll.gameObject.SetActive(false);
		this.placeholderPanel.SetActive(true);
		foreach (MailDirector.Mail mail in this.mailDir.GetMailRecentFirst())
		{
			this.AddButton(mail);
		}
		Toggle[] componentsInChildren = this.buttonListPanel.GetComponentsInChildren<Toggle>(true);
		if (componentsInChildren.Length != 0)
		{
			componentsInChildren[0].gameObject.AddComponent<InitSelected>();
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Navigation navigation = default(Navigation);
			navigation.mode = Navigation.Mode.Explicit;
			if (i < componentsInChildren.Length - 1)
			{
				navigation.selectOnDown = componentsInChildren[i + 1];
			}
			if (i > 0)
			{
				navigation.selectOnUp = componentsInChildren[i - 1];
			}
			componentsInChildren[i].navigation = navigation;
		}
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x00073000 File Offset: 0x00071200
	public void OnEnable()
	{
		base.Play(this.openCue);
	}

	// Token: 0x06001E55 RID: 7765 RVA: 0x0007300E File Offset: 0x0007120E
	public void OnDisable()
	{
		base.Play(this.closeCue);
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x0007301C File Offset: 0x0007121C
	public void AddButton(MailDirector.Mail mail)
	{
		GameObject gameObject = this.CreateButton(mail);
		gameObject.transform.SetParent(this.buttonListPanel.transform, false);
		gameObject.GetComponent<Toggle>().group = this.buttonListPanel.GetComponentInParent<ToggleGroup>();
	}

	// Token: 0x06001E57 RID: 7767 RVA: 0x00073054 File Offset: 0x00071254
	private GameObject CreateButton(MailDirector.Mail mail)
	{
		GameObject buttonObj = UnityEngine.Object.Instantiate<GameObject>(this.buttonListItemPrefab);
		Toggle component = buttonObj.GetComponent<Toggle>();
		TMP_Text component2 = buttonObj.transform.Find("Info/From").gameObject.GetComponent<TMP_Text>();
		TMP_Text component3 = buttonObj.transform.Find("Info/Subject").gameObject.GetComponent<TMP_Text>();
		Image iconImg = buttonObj.transform.Find("Icon").gameObject.GetComponent<Image>();
		component2.text = this.mailBundle.Xlate("m.from." + mail.key);
		component3.text = this.mailBundle.Xlate("m.subj." + mail.key);
		iconImg.sprite = (mail.read ? this.mailReadIcon : this.mailUnreadIcon);
		UnityAction<bool> onButton = delegate(bool isOn)
		{
			if (isOn)
			{
				this.Select(mail);
			}
			iconImg.sprite = (mail.read ? this.mailReadIcon : this.mailUnreadIcon);
		};
		component.onValueChanged.AddListener(onButton);
		OnSelectDelegator.Create(buttonObj, delegate
		{
			onButton(true);
			buttonObj.GetComponent<Toggle>().isOn = true;
		});
		return buttonObj;
	}

	// Token: 0x06001E58 RID: 7768 RVA: 0x000731A8 File Offset: 0x000713A8
	public void Select(MailDirector.Mail mail)
	{
		this.contentScroll.gameObject.SetActive(true);
		this.placeholderPanel.SetActive(false);
		this.selectedFrom.text = this.mailBundle.Xlate("m.from." + mail.key);
		this.selectedSubj.text = this.mailBundle.Xlate("m.subj." + mail.key);
		this.selectedBody.text = this.mailBundle.Xlate("m.body." + mail.key);
		base.StartCoroutine(this.ScrollAtEndOfFrame());
		if (!mail.read && mail.key.StartsWith("casey_"))
		{
			this.progressDir.QueueRanchWistfulMusic();
		}
		if (!mail.read && mail.key == "casey_11")
		{
			this.progressDir.QueueCredits();
		}
		this.mailDir.MarkRead(mail);
		if (mail.key == RanchDirector.PARTNER_MAIL_KEY)
		{
			SRSingleton<SceneContext>.Instance.PediaDirector.UnlockWithoutPopup(PediaDirector.Id.PARTNER);
			if (!this.progressDir.HasProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER_UNLOCK))
			{
				this.progressDir.AddProgress(ProgressDirector.ProgressType.CORPORATE_PARTNER_UNLOCK);
				return;
			}
		}
		else if (mail.key == "hobson_1" && !this.progressDir.HasProgress(ProgressDirector.ProgressType.HOBSON_END_UNLOCK))
		{
			this.progressDir.AddProgress(ProgressDirector.ProgressType.HOBSON_END_UNLOCK);
		}
	}

	// Token: 0x06001E59 RID: 7769 RVA: 0x00073324 File Offset: 0x00071524
	public IEnumerator ScrollAtEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		this.contentScroll.verticalNormalizedPosition = 1f;
		yield break;
	}

	// Token: 0x04001D65 RID: 7525
	public GameObject contentPanel;

	// Token: 0x04001D66 RID: 7526
	public GameObject placeholderPanel;

	// Token: 0x04001D67 RID: 7527
	public GameObject buttonListPanel;

	// Token: 0x04001D68 RID: 7528
	public TMP_Text selectedFrom;

	// Token: 0x04001D69 RID: 7529
	public TMP_Text selectedSubj;

	// Token: 0x04001D6A RID: 7530
	public TMP_Text selectedBody;

	// Token: 0x04001D6B RID: 7531
	public GameObject buttonListItemPrefab;

	// Token: 0x04001D6C RID: 7532
	public ScrollRect contentScroll;

	// Token: 0x04001D6D RID: 7533
	public Sprite mailUnreadIcon;

	// Token: 0x04001D6E RID: 7534
	public Sprite mailReadIcon;

	// Token: 0x04001D6F RID: 7535
	public SECTR_AudioCue openCue;

	// Token: 0x04001D70 RID: 7536
	public SECTR_AudioCue closeCue;

	// Token: 0x04001D71 RID: 7537
	private MessageBundle mailBundle;

	// Token: 0x04001D72 RID: 7538
	private MailDirector mailDir;

	// Token: 0x04001D73 RID: 7539
	private ProgressDirector progressDir;
}
