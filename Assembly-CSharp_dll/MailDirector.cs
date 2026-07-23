using System;
using System.Collections.Generic;
using DLCPackage;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x0200072D RID: 1837
public class MailDirector : MonoBehaviour, MailModel.Participant
{
	// Token: 0x0600265C RID: 9820 RVA: 0x00092C10 File Offset: 0x00090E10
	public void Awake()
	{
		this.dlcDirector = SRSingleton<GameContext>.Instance.DLCDirector;
		this.mailBundle = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("mail");
		this.popupDir = SRSingleton<SceneContext>.Instance.PopupDirector;
		this.progressDir = SRSingleton<SceneContext>.Instance.ProgressDirector;
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.tutDir = SRSingleton<SceneContext>.Instance.TutorialDirector;
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x00092C97 File Offset: 0x00090E97
	public void OnDestroy()
	{
		if (this.dlcDirector != null)
		{
			this.dlcDirector.onPackageInstalled -= this.OnDLCPackageStateChanged;
			this.dlcDirector = null;
		}
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x00092CC0 File Offset: 0x00090EC0
	public void Start()
	{
		this.timeDirector.OnPassedTime(561600.0, delegate()
		{
			if (!this.model.hasPartnerMail && SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().enablePartnerRewards)
			{
				this.SendMail(MailDirector.Type.PERSONAL, "partner_rewards");
			}
		});
		if (!this.model.hasSecretStyleMail)
		{
			this.dlcDirector.onPackageInstalled += this.OnDLCPackageStateChanged;
			this.OnDLCPackageStateChanged(Id.SECRET_STYLE);
		}
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x00092D19 File Offset: 0x00090F19
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterMail(this);
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x00092D2B File Offset: 0x00090F2B
	public void InitModel(MailModel model)
	{
		model.Reset();
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().AllowMail() && !Levels.isSpecial())
		{
			model.AddMail(new MailDirector.Mail(MailDirector.Type.PERSONAL, "welcome"));
			model.hasNewMail = true;
		}
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x00092D68 File Offset: 0x00090F68
	public void SetModel(MailModel model)
	{
		this.model = model;
		this.model.MailListChanged();
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x00092D7C File Offset: 0x00090F7C
	public Sprite GetIcon(MailDirector.Type type)
	{
		switch (type)
		{
		case MailDirector.Type.PERSONAL:
			return this.personalIcon;
		case MailDirector.Type.UPGRADE:
			return this.upgradeIcon;
		case MailDirector.Type.EXCHANGE:
			return this.exchangeIcon;
		default:
			Log.Error("Invalid mail type: " + type, Array.Empty<object>());
			return null;
		}
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x00092DCD File Offset: 0x00090FCD
	public List<MailDirector.Mail> GetMailRecentFirst()
	{
		List<MailDirector.Mail> list = new List<MailDirector.Mail>(this.model.allMail);
		list.Reverse();
		return list;
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x00092DE5 File Offset: 0x00090FE5
	public bool SendMailIfExists(MailDirector.Type type, string key)
	{
		return this.mailBundle.Exists("m.subj." + key) && this.SendMail(type, key);
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x00092E0C File Offset: 0x0009100C
	public bool SendMail(MailDirector.Type type, string key)
	{
		MailDirector.Mail mail = new MailDirector.Mail(type, key);
		if (this.model.allMailDict.ContainsKey(mail))
		{
			return false;
		}
		if (SRSingleton<SceneContext>.Instance.GameModeConfig.GetModeSettings().AllowMail() && !Levels.isSpecialNonAlloc())
		{
			this.model.AddMail(mail);
			this.popupDir.QueueForPopup(new MailDirector.MailPopupCreator(mail));
			if (key == "partner_rewards")
			{
				this.model.hasPartnerMail = true;
			}
			this.model.hasNewMail = true;
			return true;
		}
		return false;
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x00092E9A File Offset: 0x0009109A
	public void MarkRead(MailDirector.Mail mail)
	{
		mail.read = true;
		this.model.MailListChanged();
		this.popupDir.CheckShouldClear();
		this.playerState.OnMailRead();
		this.tutDir.OnMailRead(mail);
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x00092ED0 File Offset: 0x000910D0
	public void OnMailListChanged()
	{
		this.progressDir.CheckTrackers();
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x00092EDD File Offset: 0x000910DD
	public bool HasNewMail()
	{
		return this.model.hasNewMail;
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x00092EEC File Offset: 0x000910EC
	public int GetNewMailCount()
	{
		int num = 0;
		using (List<MailDirector.Mail>.Enumerator enumerator = this.model.allMail.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.read)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x00092F4C File Offset: 0x0009114C
	public bool HasReadMail(MailDirector.Mail mail)
	{
		return this.model != null && this.model.allMailDict.ContainsKey(mail) && this.model.allMailDict[mail].read;
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x00092F84 File Offset: 0x00091184
	private void OnDLCPackageStateChanged(Id package)
	{
		if (package == Id.SECRET_STYLE && this.dlcDirector.IsPackageInstalledAndEnabled(package))
		{
			this.timeDirector.OnPassedTime(this.timeDirector.WorldTime() + 600.0, delegate()
			{
				this.SendMail(MailDirector.Type.PERSONAL, "secret_styles");
				this.model.hasSecretStyleMail = true;
			});
			this.dlcDirector.onPackageInstalled -= this.OnDLCPackageStateChanged;
		}
	}

	// Token: 0x0400259F RID: 9631
	public Sprite personalIcon;

	// Token: 0x040025A0 RID: 9632
	public Sprite upgradeIcon;

	// Token: 0x040025A1 RID: 9633
	public Sprite exchangeIcon;

	// Token: 0x040025A2 RID: 9634
	private MessageBundle mailBundle;

	// Token: 0x040025A3 RID: 9635
	private PopupDirector popupDir;

	// Token: 0x040025A4 RID: 9636
	private ProgressDirector progressDir;

	// Token: 0x040025A5 RID: 9637
	private PlayerState playerState;

	// Token: 0x040025A6 RID: 9638
	private TutorialDirector tutDir;

	// Token: 0x040025A7 RID: 9639
	private TimeDirector timeDirector;

	// Token: 0x040025A8 RID: 9640
	private DLCDirector dlcDirector;

	// Token: 0x040025A9 RID: 9641
	private MailModel model;

	// Token: 0x040025AA RID: 9642
	public const string PARTNER_MAIL_KEY = "partner_rewards";

	// Token: 0x040025AB RID: 9643
	public const float PARTNER_UNLOCK_TIME = 561600f;

	// Token: 0x040025AC RID: 9644
	public const string CASEY_MAIL_PREFIX = "casey_";

	// Token: 0x040025AD RID: 9645
	public const string CASEY_MAIL_FINAL = "casey_11";

	// Token: 0x040025AE RID: 9646
	public const string HOBSON_MAIL_KEY = "hobson_1";

	// Token: 0x040025AF RID: 9647
	public const string OGDEN_MAIL_KEY = "ogden_invite";

	// Token: 0x040025B0 RID: 9648
	public const string MOCHI_MAIL_KEY = "mochi_invite";

	// Token: 0x040025B1 RID: 9649
	public const string VIKTOR_MAIL_KEY = "viktor_invite";

	// Token: 0x040025B2 RID: 9650
	public const string SECRET_STYLE_MAIL_KEY = "secret_styles";

	// Token: 0x040025B3 RID: 9651
	public const float SECRET_STYLE_MAIL_DELAY = 600f;

	// Token: 0x0200072E RID: 1838
	public enum Type
	{
		// Token: 0x040025B5 RID: 9653
		PERSONAL,
		// Token: 0x040025B6 RID: 9654
		UPGRADE,
		// Token: 0x040025B7 RID: 9655
		EXCHANGE
	}

	// Token: 0x0200072F RID: 1839
	[Serializable]
	public class Mail : IEquatable<MailDirector.Mail>
	{
		// Token: 0x0600266F RID: 9839 RVA: 0x000053FC File Offset: 0x000035FC
		public Mail()
		{
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x00093034 File Offset: 0x00091234
		public Mail(MailDirector.Type type, string key)
		{
			this.type = type;
			this.key = key;
			this.read = false;
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x00093051 File Offset: 0x00091251
		public bool Equals(MailDirector.Mail other)
		{
			return other != null && this.type == other.type && this.key == other.key;
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x00093077 File Offset: 0x00091277
		public override bool Equals(object obj)
		{
			return this.Equals(obj as MailDirector.Mail);
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x00093085 File Offset: 0x00091285
		public override int GetHashCode()
		{
			return this.type.GetHashCode() ^ this.key.GetHashCode();
		}

		// Token: 0x040025B8 RID: 9656
		public MailDirector.Type type;

		// Token: 0x040025B9 RID: 9657
		public string key;

		// Token: 0x040025BA RID: 9658
		public bool read;
	}

	// Token: 0x02000730 RID: 1840
	private class MailPopupCreator : PopupDirector.PopupCreator
	{
		// Token: 0x06002674 RID: 9844 RVA: 0x000930A4 File Offset: 0x000912A4
		public MailPopupCreator(MailDirector.Mail mail)
		{
			this.mail = mail;
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x000930B3 File Offset: 0x000912B3
		public override void Create()
		{
			MailPopupUI.CreateMailPopup(this.mail);
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x000930C4 File Offset: 0x000912C4
		public override bool Equals(object other)
		{
			return other is MailDirector.MailPopupCreator && ((MailDirector.MailPopupCreator)other).mail.key == this.mail.key && ((MailDirector.MailPopupCreator)other).mail.type == this.mail.type;
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x0009311A File Offset: 0x0009131A
		public override int GetHashCode()
		{
			return this.mail.key.GetHashCode() ^ this.mail.type.GetHashCode();
		}

		// Token: 0x06002678 RID: 9848 RVA: 0x00093143 File Offset: 0x00091343
		public override bool ShouldClear()
		{
			return this.mail.read;
		}

		// Token: 0x040025BB RID: 9659
		private MailDirector.Mail mail;
	}
}
