using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sentry;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020007DE RID: 2014
public class SentrySdk : MonoBehaviour
{
	// Token: 0x06002A20 RID: 10784 RVA: 0x0009DD80 File Offset: 0x0009BF80
	public void Start()
	{
		if (this.Dsn == string.Empty)
		{
			Debug.LogWarning("No DSN defined. The Sentry SDK will be disabled.");
			return;
		}
		if (SentrySdk._instance == null)
		{
			try
			{
				this._dsn = new Dsn(this.Dsn);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error parsing DSN: {0}", ex.Message));
				return;
			}
			this._breadcrumbs = new Breadcrumb[100];
			UnityEngine.Object.DontDestroyOnLoad(this);
			SentrySdk._instance = this;
			this._initialized = true;
			return;
		}
		UnityEngine.Object.Destroy(this);
	}

	// Token: 0x06002A21 RID: 10785 RVA: 0x0009DE1C File Offset: 0x0009C01C
	public static void AddBreadcrumb(string message)
	{
		if (SentrySdk._instance == null)
		{
			return;
		}
		SentrySdk._instance.DoAddBreadcrumb(message);
	}

	// Token: 0x06002A22 RID: 10786 RVA: 0x0009DE37 File Offset: 0x0009C037
	public static void CaptureMessage(string message)
	{
		if (SentrySdk._instance == null)
		{
			return;
		}
		SentrySdk._instance.DoCaptureMessage(message, null);
	}

	// Token: 0x06002A23 RID: 10787 RVA: 0x0009DE53 File Offset: 0x0009C053
	public static void CaptureEvent(SentryEvent @event, SentrySdk.OnCaptureCompleteDelegate onCaptureComplete = null)
	{
		if (SentrySdk._instance == null)
		{
			return;
		}
		SentrySdk._instance.DoCaptureEvent(@event, onCaptureComplete);
	}

	// Token: 0x06002A24 RID: 10788 RVA: 0x0009DE6F File Offset: 0x0009C06F
	public static void CaptureFeedback(string summary, string description, SentrySdk.OnCaptureCompleteDelegate onCaptureComplete = null)
	{
		if (SentrySdk._instance == null)
		{
			return;
		}
		SentrySdk._instance.DoCaptureFeedback(summary, description, onCaptureComplete);
	}

	// Token: 0x06002A25 RID: 10789 RVA: 0x0009DE8C File Offset: 0x0009C08C
	private void DoCaptureFeedback(string summary, string description, SentrySdk.OnCaptureCompleteDelegate onCaptureComplete = null)
	{
		SentryEvent @event = new SentryEvent(summary + "\n\n" + description, this.GetBreadcrumbs())
		{
			tags = 
			{
				isUserFeedback = true
			},
			level = "info"
		};
		base.StartCoroutine(this.ContinueSendingEvent<SentryEvent>(@event, onCaptureComplete));
	}

	// Token: 0x06002A26 RID: 10790 RVA: 0x0009DED8 File Offset: 0x0009C0D8
	private void DoCaptureMessage(string message, SentrySdk.OnCaptureCompleteDelegate onCaptureComplete = null)
	{
		if (this.showDebugMessagesInLog)
		{
			Debug.Log("sending message to sentry.");
		}
		SentryEvent @event = new SentryEvent(message, this.GetBreadcrumbs())
		{
			level = "info"
		};
		this.DoCaptureEvent(@event, onCaptureComplete);
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x0009DF17 File Offset: 0x0009C117
	private void DoCaptureEvent(SentryEvent @event, SentrySdk.OnCaptureCompleteDelegate onCaptureComplete)
	{
		if (this.showDebugMessagesInLog)
		{
			Debug.Log("sending event to sentry.");
		}
		base.StartCoroutine(this.ContinueSendingEvent<SentryEvent>(@event, onCaptureComplete));
	}

	// Token: 0x06002A28 RID: 10792 RVA: 0x0009DF3C File Offset: 0x0009C13C
	private void DoAddBreadcrumb(string message)
	{
		if (!this._initialized)
		{
			Debug.LogError("Cannot AddBreadcrumb if we are not initialized");
			return;
		}
		string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
		this._breadcrumbs[this._lastBreadcrumbPos] = new Breadcrumb(timestamp, message);
		this._lastBreadcrumbPos++;
		this._lastBreadcrumbPos %= 100;
		if (this._noBreadcrumbs < 100)
		{
			this._noBreadcrumbs++;
		}
	}

	// Token: 0x06002A29 RID: 10793 RVA: 0x0009DFB8 File Offset: 0x0009C1B8
	private List<Breadcrumb> GetBreadcrumbs()
	{
		return Breadcrumb.CombineBreadcrumbs(this._breadcrumbs, this._lastBreadcrumbPos, this._noBreadcrumbs);
	}

	// Token: 0x06002A2A RID: 10794 RVA: 0x0009DFD1 File Offset: 0x0009C1D1
	public void OnEnable()
	{
		Application.logMessageReceived += this.OnLogMessageReceived;
	}

	// Token: 0x06002A2B RID: 10795 RVA: 0x0009DFE4 File Offset: 0x0009C1E4
	public void OnDisable()
	{
		Application.logMessageReceived -= this.OnLogMessageReceived;
	}

	// Token: 0x06002A2C RID: 10796 RVA: 0x0009DFF8 File Offset: 0x0009C1F8
	public void ScheduleException(string condition, string stackTrace)
	{
		if (this.showDebugMessagesInLog)
		{
			Debug.Log("sending exception to sentry.");
		}
		List<StackTraceSpec> list = new List<StackTraceSpec>();
		string[] array = condition.Split(new char[]
		{
			':'
		}, 2);
		string exceptionType = array[0];
		string exceptionValue = (array.Length > 1) ? array[1].Trim() : array[0];
		foreach (StackTraceSpec item in SentrySdk.GetStackTraces(stackTrace))
		{
			list.Add(item);
		}
		SentryExceptionEvent @event = new SentryExceptionEvent(exceptionType, exceptionValue, this.GetBreadcrumbs(), list);
		base.StartCoroutine(this.ContinueSendingEvent<SentryExceptionEvent>(@event, null));
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x0009E0B0 File Offset: 0x0009C2B0
	private static IEnumerable<StackTraceSpec> GetStackTraces(string stackTrace)
	{
		string[] stackList = stackTrace.Split(new char[]
		{
			'\n'
		});
		int num3;
		for (int i = stackList.Length - 1; i >= 0; i = num3 - 1)
		{
			string text = stackList[i];
			if (!(text == string.Empty))
			{
				int num = text.IndexOf(')');
				if (num != -1)
				{
					string text2;
					string text3;
					int lineNo;
					try
					{
						text2 = text.Substring(0, num + 1);
						if (text.Length < num + 6)
						{
							text3 = string.Empty;
							lineNo = -1;
						}
						else if (text.Substring(num + 1, 5) != " (at ")
						{
							Debug.Log("failed parsing " + text);
							text2 = text;
							lineNo = -1;
							text3 = string.Empty;
						}
						else
						{
							int num2 = text.LastIndexOf(':', text.Length - 1, text.Length - num);
							if (num == text.Length - 1)
							{
								text3 = string.Empty;
								lineNo = -1;
							}
							else if (num2 == -1)
							{
								text3 = text.Substring(num + 6, text.Length - num - 7);
								lineNo = -1;
							}
							else
							{
								text3 = text.Substring(num + 6, num2 - num - 6);
								lineNo = Convert.ToInt32(text.Substring(num2 + 1, text.Length - 2 - num2));
							}
						}
					}
					catch
					{
						goto IL_1DC;
					}
					bool inApp;
					if (text3 == string.Empty || (text3[0] == '<' && text3[text3.Length - 1] == '>'))
					{
						text3 = string.Empty;
						inApp = true;
						if (text2.Contains("UnityEngine."))
						{
							inApp = false;
						}
					}
					else
					{
						inApp = text3.Contains("Assets/");
					}
					yield return new StackTraceSpec(text3, text2, lineNo, inApp);
				}
			}
			IL_1DC:
			num3 = i;
		}
		yield break;
	}

	// Token: 0x06002A2E RID: 10798 RVA: 0x0009E0C0 File Offset: 0x0009C2C0
	public void OnLogMessageReceived(string condition, string stackTrace, LogType type)
	{
		if (!this._initialized)
		{
			return;
		}
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (this.previouslySentErrors.Contains(condition))
		{
			return;
		}
		if (Time.time - this._timeLastError <= 10f)
		{
			return;
		}
		this._timeLastError = Time.time;
		this.previouslySentErrors.Add(condition);
		this.ScheduleException(condition, stackTrace);
	}

	// Token: 0x06002A2F RID: 10799 RVA: 0x0009E125 File Offset: 0x0009C325
	private IEnumerator ContinueSendingEvent<T>(T @event, SentrySdk.OnCaptureCompleteDelegate onCaptureComplete = null) where T : SentryEvent
	{
		yield return new WaitForSecondsRealtime(5f);
		string s = JsonUtility.ToJson(@event);
		string publicKey = this._dsn.publicKey;
		string secretKey = this._dsn.secretKey;
		string text = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
		string value = string.Concat(new string[]
		{
			"Sentry sentry_version=5,sentry_client=Unity0.1,sentry_timestamp=",
			text,
			",sentry_key=",
			publicKey,
			",sentry_secret=",
			secretKey
		});
		UnityWebRequest www = new UnityWebRequest(this._dsn.callUri.ToString());
		www.method = "POST";
		www.SetRequestHeader("X-Sentry-Auth", value);
		www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
		www.downloadHandler = new DownloadHandlerBuffer();
		yield return www.SendWebRequest();
		while (!www.isDone)
		{
			yield return null;
		}
		if (www.isNetworkError || www.isHttpError || www.responseCode != 200L)
		{
			Debug.LogWarning("error sending request to sentry: " + www.error);
			if (onCaptureComplete != null)
			{
				onCaptureComplete(new Exception(www.error));
			}
		}
		else if (this.showDebugMessagesInLog)
		{
			Debug.Log("Sentry sent back: " + www.downloadHandler.text);
			if (onCaptureComplete != null)
			{
				onCaptureComplete(null);
			}
		}
		yield break;
	}

	// Token: 0x0400293E RID: 10558
	public static readonly string SESSION_USER_ID = Guid.NewGuid().ToString();

	// Token: 0x0400293F RID: 10559
	private float _timeLastError;

	// Token: 0x04002940 RID: 10560
	private const float MinTime = 10f;

	// Token: 0x04002941 RID: 10561
	private Breadcrumb[] _breadcrumbs;

	// Token: 0x04002942 RID: 10562
	private int _lastBreadcrumbPos;

	// Token: 0x04002943 RID: 10563
	private int _noBreadcrumbs;

	// Token: 0x04002944 RID: 10564
	[Header("Sentry Authentication")]
	public string Dsn;

	// Token: 0x04002945 RID: 10565
	[Header("Debug Settings")]
	public bool showDebugMessagesInLog = true;

	// Token: 0x04002946 RID: 10566
	[Tooltip("Enable to log exceptions from the editor.")]
	public bool postExceptionsFromEditor;

	// Token: 0x04002947 RID: 10567
	private Dsn _dsn;

	// Token: 0x04002948 RID: 10568
	private bool _initialized;

	// Token: 0x04002949 RID: 10569
	private readonly HashSet<string> previouslySentErrors = new HashSet<string>();

	// Token: 0x0400294A RID: 10570
	private static SentrySdk _instance;

	// Token: 0x020007DF RID: 2015
	// (Invoke) Token: 0x06002A33 RID: 10803
	public delegate void OnCaptureCompleteDelegate(Exception exception);
}
