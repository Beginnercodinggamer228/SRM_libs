using System;
using System.Collections;
using Assets.Script.Util.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005CA RID: 1482
public class MessageOfTheDayUI : MonoBehaviour
{
	// Token: 0x06001ED5 RID: 7893 RVA: 0x00075226 File Offset: 0x00073426
	private void Awake()
	{
		this.messageDirector = SRSingleton<GameContext>.Instance.MessageDirector;
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x00075238 File Offset: 0x00073438
	private void Start()
	{
		this.messageWindow.gameObject.SetActive(false);
		this.RequestAndSetupMessage();
	}

	// Token: 0x06001ED7 RID: 7895 RVA: 0x00075254 File Offset: 0x00073454
	public void Update()
	{
		if (this.isMessageSet)
		{
			string currentLanguageCode = this.messageDirector.GetCurrentLanguageCode();
			if (this.currentLanguage.CompareTo(currentLanguageCode) != 0)
			{
				this.UpdateLocalizedMessage(currentLanguageCode);
				this.currentLanguage = currentLanguageCode;
			}
		}
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x00075294 File Offset: 0x00073494
	private void UpdateLocalizedMessage(string lang)
	{
		this.announcementLabel.text = this.message.GetAnnouncementText(lang);
		this.titleLabel.text = this.message.GetTitleText(lang);
		this.bodyLabel.text = this.message.GetBodyText(lang);
		this.button.UpdateButton(this.message.GetId(), this.message.GetButtonText(lang), this.message.GetUrl(lang));
		this.iconImage.sprite = this.message.GetSprite();
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x0007532A File Offset: 0x0007352A
	private void RequestAndSetupMessage()
	{
		SRSingleton<GameContext>.Instance.MessageOfTheDayDirector.GetProvider().Get(delegate(MessageOfTheDay m)
		{
			MessageOfTheDayUI.OnMessageSuccess(this, m);
		}, delegate
		{
			MessageOfTheDayUI.OnMessageFailure(this);
		});
	}

	// Token: 0x06001EDA RID: 7898 RVA: 0x00075358 File Offset: 0x00073558
	private static void OnMessageSuccess(MessageOfTheDayUI instance, MessageOfTheDay retrievedMessage)
	{
		if (instance != null && instance.gameObject.activeInHierarchy)
		{
			instance.message = retrievedMessage;
			instance.isMessageSet = true;
			instance.gameObject.StartCoroutine(instance.AnimateIn());
		}
	}

	// Token: 0x06001EDB RID: 7899 RVA: 0x0007538F File Offset: 0x0007358F
	private static void OnMessageFailure(MessageOfTheDayUI instance)
	{
		if (instance != null && instance.gameObject.activeInHierarchy)
		{
			instance.isMessageSet = false;
		}
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x000753AE File Offset: 0x000735AE
	public IEnumerator AnimateIn()
	{
		this.messageWindow.gameObject.SetActive(true);
		RectTransform component = this.messageWindow.gameObject.GetComponent<RectTransform>();
		Transform transform = this.messageWindow.transform;
		float d = transform.position.x + component.rect.width;
		transform.position += Vector3.left * d;
		this.messageWindow.transform.DOBlendableMoveBy(Vector3.right * d, this.moveTime, false).SetEase(this.easeType).SetUpdate(true);
		yield return new WaitForSecondsRealtime(this.moveTime);
		yield break;
	}

	// Token: 0x04001DF3 RID: 7667
	[Header("UI Elements")]
	public GameObject messageWindow;

	// Token: 0x04001DF4 RID: 7668
	public Image iconImage;

	// Token: 0x04001DF5 RID: 7669
	public TMP_Text announcementLabel;

	// Token: 0x04001DF6 RID: 7670
	public TMP_Text titleLabel;

	// Token: 0x04001DF7 RID: 7671
	public TMP_Text bodyLabel;

	// Token: 0x04001DF8 RID: 7672
	public MessageOfTheDayButton button;

	// Token: 0x04001DF9 RID: 7673
	[Header("Button Animation")]
	public Ease easeType;

	// Token: 0x04001DFA RID: 7674
	public float moveTime;

	// Token: 0x04001DFB RID: 7675
	public float moveDistance;

	// Token: 0x04001DFC RID: 7676
	private MessageOfTheDay message;

	// Token: 0x04001DFD RID: 7677
	private string currentLanguage = "";

	// Token: 0x04001DFE RID: 7678
	private bool isMessageSet;

	// Token: 0x04001DFF RID: 7679
	private MessageDirector messageDirector;
}
