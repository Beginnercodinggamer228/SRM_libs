using System;
using System.Globalization;
using MonomiPark.Controller;
using UnityEngine;

// Token: 0x0200051F RID: 1311
public class SystemContext : SRSingleton<SystemContext>
{
	// Token: 0x170001FF RID: 511
	// (get) Token: 0x06001B5F RID: 7007 RVA: 0x00068CA7 File Offset: 0x00066EA7
	// (set) Token: 0x06001B60 RID: 7008 RVA: 0x00068CAF File Offset: 0x00066EAF
	public GameCoreXboxContext GameCoreXboxContext { get; private set; }

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x06001B61 RID: 7009 RVA: 0x00068CB8 File Offset: 0x00066EB8
	// (set) Token: 0x06001B62 RID: 7010 RVA: 0x00068CC0 File Offset: 0x00066EC0
	public UWPContext UWPContext { get; private set; }

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06001B63 RID: 7011 RVA: 0x00068CC9 File Offset: 0x00066EC9
	// (set) Token: 0x06001B64 RID: 7012 RVA: 0x00068CD1 File Offset: 0x00066ED1
	public PS4Context PS4Context { get; private set; }

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06001B65 RID: 7013 RVA: 0x00068CDA File Offset: 0x00066EDA
	// (set) Token: 0x06001B66 RID: 7014 RVA: 0x00068CE2 File Offset: 0x00066EE2
	public RumbleHandler RumbleHandler { get; private set; }

	// Token: 0x06001B67 RID: 7015 RVA: 0x00068CEC File Offset: 0x00066EEC
	public override void Awake()
	{
		if (SRSingleton<SystemContext>.Instance == null)
		{
			base.Awake();
			if (Application.unityVersion == "_never_POSSIBLE_")
			{
				new ChineseLunisolarCalendar();
				new GregorianCalendar();
				new HebrewCalendar();
				new HijriCalendar();
				new JapaneseCalendar();
				new JapaneseLunisolarCalendar();
				new JulianCalendar();
				new KoreanCalendar();
				new KoreanLunisolarCalendar();
				new PersianCalendar();
				new TaiwanCalendar();
				new TaiwanLunisolarCalendar();
				new ThaiBuddhistCalendar();
				new UmAlQuraCalendar();
			}
			this.GameCoreXboxContext = base.GetComponent<GameCoreXboxContext>();
			this.UWPContext = base.GetComponent<UWPContext>();
			this.PS4Context = base.GetComponent<PS4Context>();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this.RumbleHandler = new EmptyRumbleHandler();
			return;
		}
		if (SRSingleton<SystemContext>.Instance != this)
		{
			Destroyer.Destroy(base.gameObject, "SystemContext.Awake");
		}
	}

	// Token: 0x04001AAC RID: 6828
	public static bool IsModded;

	// Token: 0x04001AB1 RID: 6833
	public IDateProvider DateProvider = new StandardDateProvider();
}
