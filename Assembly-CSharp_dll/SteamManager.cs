using System;
using System.Text;
using Steamworks;
using UnityEngine;

// Token: 0x0200051A RID: 1306
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	// Token: 0x170001FD RID: 509
	// (get) Token: 0x06001B38 RID: 6968 RVA: 0x0006890A File Offset: 0x00066B0A
	protected static SteamManager Instance
	{
		get
		{
			if (SteamManager.s_instance == null)
			{
				return new GameObject("SteamManager").AddComponent<SteamManager>();
			}
			return SteamManager.s_instance;
		}
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x06001B39 RID: 6969 RVA: 0x0006892E File Offset: 0x00066B2E
	public static bool Initialized
	{
		get
		{
			return SteamManager.Instance.m_bInitialized;
		}
	}

	// Token: 0x06001B3A RID: 6970 RVA: 0x0006893A File Offset: 0x00066B3A
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x00068944 File Offset: 0x00066B44
	protected virtual void Awake()
	{
		if (SteamManager.s_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		SteamManager.s_instance = this;
		if (SteamManager.s_EverInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException arg)
		{
			Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + arg, this);
			Application.Quit();
			return;
		}
		this.m_bInitialized = SteamAPI.Init();
		if (!this.m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamManager.s_EverInitialized = true;
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x00068A18 File Offset: 0x00066C18
	protected virtual void OnEnable()
	{
		if (SteamManager.s_instance == null)
		{
			SteamManager.s_instance = this;
		}
		if (!this.m_bInitialized)
		{
			return;
		}
		if (this.m_SteamAPIWarningMessageHook == null)
		{
			this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
		}
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x00068A66 File Offset: 0x00066C66
	public static void AddDestroyCallback(SteamManager.DestroyCallback callback)
	{
		SteamManager instance = SteamManager.Instance;
		instance.destroyCallbacks = (SteamManager.DestroyCallback)Delegate.Combine(instance.destroyCallbacks, callback);
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x00068A84 File Offset: 0x00066C84
	protected virtual void OnDestroy()
	{
		if (SteamManager.s_instance != this)
		{
			return;
		}
		if (this.destroyCallbacks != null)
		{
			this.destroyCallbacks();
		}
		SteamManager.s_instance = null;
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.Shutdown();
		if (SteamManager.s_UserStatsReceived != null)
		{
			Debug.Log("Disposing of UserStatsReceived callback");
			SteamManager.s_UserStatsReceived.Dispose();
		}
		if (SteamManager.s_GameOverlayActivated != null)
		{
			Debug.Log("Disposing of GameOverlayActivated callback");
			SteamManager.s_GameOverlayActivated.Dispose();
		}
		if (SteamManager.s_GamepadTextInputDismissed != null)
		{
			Debug.Log("Disposing of GamepadTextInputDismissed callback");
			SteamManager.s_GamepadTextInputDismissed.Dispose();
		}
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x00068B17 File Offset: 0x00066D17
	protected virtual void Update()
	{
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.RunCallbacks();
	}

	// Token: 0x06001B40 RID: 6976 RVA: 0x00068B28 File Offset: 0x00066D28
	public static void AddAchievement(AchievementsDirector.Achievement achievement)
	{
		if (SteamManager.s_UserStatsReceived_Initialized)
		{
			SteamUserStats.SetAchievement(Enum.GetName(typeof(AchievementsDirector.Achievement), achievement));
			SteamUserStats.StoreStats();
			return;
		}
		SteamManager.s_UserStatsReceived_OnInitialized += delegate()
		{
			SteamManager.AddAchievement(achievement);
		};
	}

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06001B41 RID: 6977 RVA: 0x00068B84 File Offset: 0x00066D84
	// (remove) Token: 0x06001B42 RID: 6978 RVA: 0x00068BB8 File Offset: 0x00066DB8
	public static event Action s_UserStatsReceived_OnInitialized;

	// Token: 0x06001B43 RID: 6979 RVA: 0x00068BEB File Offset: 0x00066DEB
	public static void c_UserStatsReceived(UserStatsReceived_t response)
	{
		SteamManager.s_UserStatsReceived_Initialized = true;
		SteamManager.s_UserStatsReceived.Unregister();
		SteamManager.s_UserStatsReceived = null;
		if (SteamManager.s_UserStatsReceived_OnInitialized != null)
		{
			SteamManager.s_UserStatsReceived_OnInitialized();
			SteamManager.s_UserStatsReceived_OnInitialized = null;
		}
	}

	// Token: 0x04001AA0 RID: 6816
	protected static bool s_EverInitialized;

	// Token: 0x04001AA1 RID: 6817
	protected static SteamManager s_instance;

	// Token: 0x04001AA2 RID: 6818
	protected bool m_bInitialized;

	// Token: 0x04001AA3 RID: 6819
	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	// Token: 0x04001AA4 RID: 6820
	public SteamManager.DestroyCallback destroyCallbacks;

	// Token: 0x04001AA5 RID: 6821
	public static Callback<GameOverlayActivated_t> s_GameOverlayActivated;

	// Token: 0x04001AA6 RID: 6822
	public static Callback<GamepadTextInputDismissed_t> s_GamepadTextInputDismissed;

	// Token: 0x04001AA8 RID: 6824
	public static Callback<UserStatsReceived_t> s_UserStatsReceived;

	// Token: 0x04001AA9 RID: 6825
	public static bool s_UserStatsReceived_Initialized;

	// Token: 0x0200051B RID: 1307
	// (Invoke) Token: 0x06001B47 RID: 6983
	public delegate void DestroyCallback();
}
