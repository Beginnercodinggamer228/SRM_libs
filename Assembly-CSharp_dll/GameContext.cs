using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using RichPresence;
using SRML;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class GameContext : SRSingleton<GameContext>
{
	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000A7B RID: 2683 RVA: 0x0002D36A File Offset: 0x0002B56A
	// (set) Token: 0x06000A7C RID: 2684 RVA: 0x0002D372 File Offset: 0x0002B572
	public LookupDirector LookupDirector { get; private set; }

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000A7D RID: 2685 RVA: 0x0002D37B File Offset: 0x0002B57B
	// (set) Token: 0x06000A7E RID: 2686 RVA: 0x0002D383 File Offset: 0x0002B583
	public AutoSaveDirector AutoSaveDirector { get; private set; }

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000A7F RID: 2687 RVA: 0x0002D38C File Offset: 0x0002B58C
	// (set) Token: 0x06000A80 RID: 2688 RVA: 0x0002D394 File Offset: 0x0002B594
	public SlimeShaders SlimeShaders { get; private set; }

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000A81 RID: 2689 RVA: 0x0002D39D File Offset: 0x0002B59D
	// (set) Token: 0x06000A82 RID: 2690 RVA: 0x0002D3A5 File Offset: 0x0002B5A5
	public MessageDirector MessageDirector { get; private set; }

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000A83 RID: 2691 RVA: 0x0002D3AE File Offset: 0x0002B5AE
	// (set) Token: 0x06000A84 RID: 2692 RVA: 0x0002D3B6 File Offset: 0x0002B5B6
	public UITemplates UITemplates { get; private set; }

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000A85 RID: 2693 RVA: 0x0002D3BF File Offset: 0x0002B5BF
	// (set) Token: 0x06000A86 RID: 2694 RVA: 0x0002D3C7 File Offset: 0x0002B5C7
	public InputDirector InputDirector { get; private set; }

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000A87 RID: 2695 RVA: 0x0002D3D0 File Offset: 0x0002B5D0
	// (set) Token: 0x06000A88 RID: 2696 RVA: 0x0002D3D8 File Offset: 0x0002B5D8
	public MusicDirector MusicDirector { get; private set; }

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000A89 RID: 2697 RVA: 0x0002D3E1 File Offset: 0x0002B5E1
	// (set) Token: 0x06000A8A RID: 2698 RVA: 0x0002D3E9 File Offset: 0x0002B5E9
	public OptionsDirector OptionsDirector { get; private set; }

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000A8B RID: 2699 RVA: 0x0002D3F2 File Offset: 0x0002B5F2
	// (set) Token: 0x06000A8C RID: 2700 RVA: 0x0002D3FA File Offset: 0x0002B5FA
	public GifRecorder GifRecorder { get; private set; }

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000A8D RID: 2701 RVA: 0x0002D403 File Offset: 0x0002B603
	// (set) Token: 0x06000A8E RID: 2702 RVA: 0x0002D40B File Offset: 0x0002B60B
	public PerformanceTracker PerformanceTracker { get; private set; }

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000A8F RID: 2703 RVA: 0x0002D414 File Offset: 0x0002B614
	// (set) Token: 0x06000A90 RID: 2704 RVA: 0x0002D41C File Offset: 0x0002B61C
	public GalaxyDirector GalaxyDirector { get; private set; }

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000A91 RID: 2705 RVA: 0x0002D425 File Offset: 0x0002B625
	// (set) Token: 0x06000A92 RID: 2706 RVA: 0x0002D42D File Offset: 0x0002B62D
	public RailDirector RailDirector { get; private set; }

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000A93 RID: 2707 RVA: 0x0002D436 File Offset: 0x0002B636
	// (set) Token: 0x06000A94 RID: 2708 RVA: 0x0002D43E File Offset: 0x0002B63E
	public Director RichPresenceDirector { get; private set; }

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000A95 RID: 2709 RVA: 0x0002D447 File Offset: 0x0002B647
	// (set) Token: 0x06000A96 RID: 2710 RVA: 0x0002D44F File Offset: 0x0002B64F
	public DLCDirector DLCDirector { get; private set; }

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000A97 RID: 2711 RVA: 0x0002D458 File Offset: 0x0002B658
	// (set) Token: 0x06000A98 RID: 2712 RVA: 0x0002D460 File Offset: 0x0002B660
	public RaycastBatcher RaycastBatcher { get; private set; }

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000A99 RID: 2713 RVA: 0x0002D469 File Offset: 0x0002B669
	// (set) Token: 0x06000A9A RID: 2714 RVA: 0x0002D471 File Offset: 0x0002B671
	public ToyDirector ToyDirector { get; private set; }

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000A9B RID: 2715 RVA: 0x0002D47A File Offset: 0x0002B67A
	public string LogText
	{
		get
		{
			return this.partialLogText.ToString();
		}
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0002D488 File Offset: 0x0002B688
	public override void Awake()
	{
		GameContext.LoadSRModLoader();
		if (SRSingleton<GameContext>.Instance == null)
		{
			base.Awake();
			this.LookupDirector = base.GetComponent<LookupDirector>();
			this.AutoSaveDirector = base.GetComponent<AutoSaveDirector>();
			this.SlimeShaders = base.GetComponent<SlimeShaders>();
			this.UITemplates = base.GetComponent<UITemplates>();
			this.InputDirector = base.GetComponent<InputDirector>();
			this.MessageDirector = base.GetComponent<MessageDirector>();
			this.MusicDirector = base.GetComponent<MusicDirector>();
			this.OptionsDirector = base.GetComponent<OptionsDirector>();
			this.GifRecorder = base.GetComponent<GifRecorder>();
			this.PerformanceTracker = base.GetComponent<PerformanceTracker>();
			this.GalaxyDirector = base.GetComponent<GalaxyDirector>();
			this.RailDirector = base.GetComponent<RailDirector>();
			this.RaycastBatcher = base.GetComponent<RaycastBatcher>();
			this.RichPresenceDirector = new Director();
			this.DLCDirector = new DLCDirector();
			this.ToyDirector = new ToyDirector();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			string[] joystickNames = Input.GetJoystickNames();
			Debug.Log(string.Format("Joystick Names: {0}", string.Join(",", joystickNames)));
			Application.logMessageReceived += this.LogReceived;
			Application.targetFrameRate = 120;
			return;
		}
		if (SRSingleton<GameContext>.Instance != this)
		{
			Destroyer.Destroy(base.gameObject, "GameContext.Awake");
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0002D5D0 File Offset: 0x0002B7D0
	public void Start()
	{
		this.RichPresenceDirector.Register(SRSingleton<SystemContext>.Instance.GameCoreXboxContext);
		this.RichPresenceDirector.Register(SRSingleton<SystemContext>.Instance.UWPContext);
		this.RichPresenceDirector.Register(SRSingleton<SystemContext>.Instance.PS4Context);
		this.RichPresenceDirector.Register(Discord.RichPresenceHandler);
		MessageOfTheDayProvider provider = this.MessageOfTheDayDirector.GetProvider();
		if (provider is MessageOfTheDayLocalProvider)
		{
			((MessageOfTheDayLocalProvider)provider).SetDLCDirector(this.DLCDirector);
		}
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0002D651 File Offset: 0x0002B851
	public override void OnDestroy()
	{
		base.OnDestroy();
		Application.logMessageReceived -= this.LogReceived;
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x0002D66A File Offset: 0x0002B86A
	public void TakeGifScreenshot()
	{
		this.GifRecorder.MaybeSaveGif();
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x0002D678 File Offset: 0x0002B878
	public void TakeScreenshot()
	{
		this.TakeScreenshot(default(GameContext.TakeScreenshot_Params));
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x0002D694 File Offset: 0x0002B894
	public void TakeScreenshot(GameContext.TakeScreenshot_Params args)
	{
		base.StartCoroutine(this.TakeScreenshotAsync(args));
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0002D6A4 File Offset: 0x0002B8A4
	private IEnumerator TakeScreenshotAsync(GameContext.TakeScreenshot_Params args)
	{
		string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), args.directory ?? string.Empty), args.name ?? string.Format("SlimeRancher-{0:yyyy-MM-dd-hh-mm-ss-ff}.png", DateTime.Now));
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		File.Delete(path);
		yield return null;
		GameObject hudRoot = null;
		if (SRSingleton<HudUI>.Instance != null)
		{
			hudRoot = SRSingleton<HudUI>.Instance.transform.parent.gameObject;
		}
		if (hudRoot != null)
		{
			hudRoot.SetActive(false);
		}
		yield return new WaitForEndOfFrame();
		ScreenCapture.CaptureScreenshot(path);
		if (hudRoot != null)
		{
			hudRoot.SetActive(true);
		}
		yield break;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x0002D6B4 File Offset: 0x0002B8B4
	private void LogReceived(string message, string stacktrace, LogType type)
	{
		this.partialLogText.AppendLine(string.Format("{0} +0000;[{1}];{2}", DateTime.UtcNow.ToString(), type.ToString().ToUpperInvariant(), message));
		if (!string.IsNullOrEmpty(stacktrace))
		{
			stacktrace = stacktrace.Replace("\n", "\n\t");
			stacktrace = stacktrace.TrimEnd(new char[]
			{
				'\t'
			});
			if (stacktrace != "" && type != LogType.Log)
			{
				this.partialLogText.Append(string.Format("\t{0}", stacktrace));
			}
		}
		if (this.Truncate(this.partialLogText, 51200, 25600))
		{
			this.partialLogText.Insert(0, string.Format("{0} +0000;[LOG];Truncated logs due to string size limit of approximately {1}KB\n", DateTime.UtcNow, 51200));
		}
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0002D78F File Offset: 0x0002B98F
	private bool Truncate(StringBuilder sb, int maxLength, int amount)
	{
		if (sb.Length > maxLength)
		{
			sb.Remove(0, amount);
			return true;
		}
		return false;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x0002D7BC File Offset: 0x0002B9BC
	public static void LoadSRModLoader()
	{
		try
		{
			string[] files = Directory.GetFiles("SRML/Libs", "*.dll", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				Assembly.LoadFrom(files[i]);
			}
			Main.PreLoad();
			return;
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Application.Quit();
		}
	}

	// Token: 0x0400089E RID: 2206
	public MessageOfTheDayDirector MessageOfTheDayDirector;

	// Token: 0x0400089F RID: 2207
	public SlimeDefinitions SlimeDefinitions;

	// Token: 0x040008A0 RID: 2208
	private const int LOG_MAX_CHARACTERS = 51200;

	// Token: 0x040008A1 RID: 2209
	private const int LOG_TRUNCATE_AMOUNT = 25600;

	// Token: 0x040008A2 RID: 2210
	private StringBuilder partialLogText = new StringBuilder();

	// Token: 0x040008A3 RID: 2211
	private const int MAX_FRAME_RATE = 120;

	// Token: 0x020001F8 RID: 504
	public struct TakeScreenshot_Params
	{
		// Token: 0x040008A4 RID: 2212
		public string directory;

		// Token: 0x040008A5 RID: 2213
		public string name;
	}
}
