using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x0200055F RID: 1375
public class CreditsUI : BaseUI
{
	// Token: 0x06001CB1 RID: 7345 RVA: 0x0006D36C File Offset: 0x0006B56C
	public void OnEnable()
	{
		this.musicDirector = SRSingleton<GameContext>.Instance.MusicDirector;
		this.musicDirector.RegisterSuppressor(this);
		if (SRSingleton<SceneContext>.Instance.Player != null)
		{
			this.camDisabler = SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<CameraDisabler>();
		}
		else if (Camera.main != null)
		{
			this.camDisabler = Camera.main.GetComponent<CameraDisabler>();
		}
		this.preCreditsMasterVolume = SECTR_AudioSystem.GetBusVolume("MasterBus");
		this.preCreditsMusicVolume = SECTR_AudioSystem.GetBusVolume("Music");
		SECTR_AudioSystem.PauseNonUISFX(true);
		base.StartCoroutine(this.DoSequence());
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x0006D410 File Offset: 0x0006B610
	public void OnDisable()
	{
		if (this.musicDirector != null)
		{
			this.musicDirector.SetCreditsMode(false);
			this.musicDirector.DeregisterSuppressor(this);
			this.musicDirector.ForceStopCurrent();
			this.musicDirector = null;
		}
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.ProgressDirector.NoteReturnedToRanch();
		}
		SECTR_AudioSystem.SetBusVolume("MasterBus", this.preCreditsMasterVolume);
		SECTR_AudioSystem.SetBusVolume("Music", this.preCreditsMusicVolume);
		SECTR_AudioSystem.PauseNonUISFX(false);
		if (this.camDisabler != null)
		{
			this.camDisabler.RemoveBlocker(this);
		}
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x0006D4B1 File Offset: 0x0006B6B1
	public IEnumerator DoSequence()
	{
		if (this.doPreCredits)
		{
			if (this.camDisabler != null)
			{
				this.camDisabler.AddBlocker(this);
			}
			this.preCreditsLine1.DOFade(1f, 1f).SetUpdate(true);
			yield return new WaitForSecondsRealtime(5f);
			this.preCreditsLine2.DOFade(1f, 1f).SetUpdate(true);
			yield return new WaitForSecondsRealtime(5f);
			this.preCreditsLine3.DOFade(1f, 1f).SetUpdate(true);
			yield return new WaitForSecondsRealtime(5f);
			yield return new WaitForEndOfFrame();
		}
		else
		{
			this.background.DOFade(1f, 1f).From(0f, true).SetUpdate(true);
			yield return new WaitForSecondsRealtime(1.25f);
			yield return new WaitForEndOfFrame();
			if (this.camDisabler != null)
			{
				this.camDisabler.AddBlocker(this);
			}
		}
		this.SetCreditsVolume();
		this.musicDirector.DeregisterSuppressor(this);
		this.musicDirector.SetCreditsMode(true);
		GameObject scroller = this.GetCreditsScroll();
		yield return new WaitForSecondsRealtime(0.25f);
		scroller.GetComponent<Animator>().SetBool("ReadyToRun", true);
		if (this.doPreCredits)
		{
			this.preCreditsLine1.DOFade(0f, 1f).SetUpdate(true);
			this.preCreditsLine2.DOFade(0f, 1f).SetUpdate(true);
			this.preCreditsLine3.DOFade(0f, 1f).SetUpdate(true);
		}
		yield return new WaitForSecondsRealtime(169f);
		if (this.camDisabler != null)
		{
			this.camDisabler.RemoveBlocker(this);
		}
		this.root.DOFade(0f, 1f).SetUpdate(true);
		yield return new WaitForSecondsRealtime(1f);
		this.endReached = true;
		if (this.OnCreditsEnded != null)
		{
			this.OnCreditsEnded();
		}
		this.Close();
		yield break;
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x0006D4C0 File Offset: 0x0006B6C0
	protected override bool Closeable()
	{
		return base.Closeable() && (this.skippable || this.endReached);
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x0006D4DC File Offset: 0x0006B6DC
	private GameObject GetCreditsScroll()
	{
		GameObject gameObject = this.CreateCreditsScrollPrefab();
		gameObject.transform.SetParent(base.transform, false);
		return gameObject;
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x0006D4F6 File Offset: 0x0006B6F6
	private void SetCreditsVolume()
	{
		if (this.preCreditsMasterVolume <= 0f)
		{
			SECTR_AudioSystem.SetBusVolume("MasterBus", 0.1f);
		}
		if (this.preCreditsMusicVolume <= 0f)
		{
			SECTR_AudioSystem.SetBusVolume("Music", 0.1f);
		}
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x0006D530 File Offset: 0x0006B730
	private GameObject CreateCreditsScrollPrefab()
	{
		string path = Path.Combine(Application.streamingAssetsPath, "credits");
		AssetBundle creditsBundle = AssetBundle.LoadFromFile(path);
		if (creditsBundle == null)
		{
			Debug.Log("Failed to load AssetBundle!: " + Application.streamingAssetsPath);
			return null;
		}
		GameObject result = UnityEngine.Object.Instantiate<GameObject>(creditsBundle.LoadAsset<GameObject>("Credit_screen"));
		this.onDestroy = delegate()
		{
			if (this != null && this.gameObject != null)
			{
				creditsBundle.Unload(true);
			}
		};
		return result;
	}

	// Token: 0x04001BB8 RID: 7096
	public bool skippable;

	// Token: 0x04001BB9 RID: 7097
	public bool doPreCredits;

	// Token: 0x04001BBA RID: 7098
	public CanvasGroup root;

	// Token: 0x04001BBB RID: 7099
	public CanvasGroup background;

	// Token: 0x04001BBC RID: 7100
	public CanvasGroup preCreditsLine1;

	// Token: 0x04001BBD RID: 7101
	public CanvasGroup preCreditsLine2;

	// Token: 0x04001BBE RID: 7102
	public CanvasGroup preCreditsLine3;

	// Token: 0x04001BBF RID: 7103
	public const float fadeTime = 1f;

	// Token: 0x04001BC0 RID: 7104
	public const float fadeTimeMargin = 0.25f;

	// Token: 0x04001BC1 RID: 7105
	public const float preCreditsLineFadeTime = 1f;

	// Token: 0x04001BC2 RID: 7106
	public const float preCreditsLineTime = 5f;

	// Token: 0x04001BC3 RID: 7107
	public const float creditsLifetime = 169f;

	// Token: 0x04001BC4 RID: 7108
	private bool endReached;

	// Token: 0x04001BC5 RID: 7109
	private CameraDisabler camDisabler;

	// Token: 0x04001BC6 RID: 7110
	public CreditsUI.OnCreditsEndedEvent OnCreditsEnded;

	// Token: 0x04001BC7 RID: 7111
	private const float DEFAULT_CREDITS_MASTER_VOLUME = 0.1f;

	// Token: 0x04001BC8 RID: 7112
	private const float DEFAULT_CREDITS_MUSIC_VOLUME = 0.1f;

	// Token: 0x04001BC9 RID: 7113
	private float preCreditsMasterVolume;

	// Token: 0x04001BCA RID: 7114
	private float preCreditsMusicVolume;

	// Token: 0x04001BCB RID: 7115
	private const string MUSIC_BUS_NAME = "Music";

	// Token: 0x04001BCC RID: 7116
	private const string MASTER_BUS_NAME = "MasterBus";

	// Token: 0x04001BCD RID: 7117
	private MusicDirector musicDirector;

	// Token: 0x02000560 RID: 1376
	// (Invoke) Token: 0x06001CBA RID: 7354
	public delegate void OnCreditsEndedEvent();
}
