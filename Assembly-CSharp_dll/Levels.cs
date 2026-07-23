using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Token: 0x02000225 RID: 549
public class Levels
{
	// Token: 0x06000BC9 RID: 3017 RVA: 0x00031674 File Offset: 0x0002F874
	static Levels()
	{
		SceneManager.activeSceneChanged += Levels.OnActiveSceneChanged;
		Levels.isActiveSceneSpecial = Levels.isSpecial(SceneManager.GetActiveScene().name);
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0003170D File Offset: 0x0002F90D
	private static void OnActiveSceneChanged(Scene replaced, Scene next)
	{
		Levels.isActiveSceneSpecial = Levels.isSpecial(next.name);
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x00031720 File Offset: 0x0002F920
	public static bool isSpecialNonAlloc()
	{
		return Levels.isActiveSceneSpecial;
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x00031728 File Offset: 0x0002F928
	public static bool isSpecial()
	{
		return Levels.isSpecial(SceneManager.GetActiveScene().name);
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x00031747 File Offset: 0x0002F947
	private static bool isSpecial(string name)
	{
		return Levels.specialScenes.Contains(name);
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00031754 File Offset: 0x0002F954
	public static bool isMainMenu()
	{
		return Levels.IsLevel("MainMenu");
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x00031760 File Offset: 0x0002F960
	public static bool IsLevel(string name)
	{
		return SceneManager.GetActiveScene().name == name;
	}

	// Token: 0x04000AB2 RID: 2738
	public const string COMPANY_LOGO = "CompanyLogoScene";

	// Token: 0x04000AB3 RID: 2739
	public const string STANDALONE_START = "StandaloneStart";

	// Token: 0x04000AB4 RID: 2740
	public const string MAIN_MENU = "MainMenu";

	// Token: 0x04000AB5 RID: 2741
	public const string XBOX_ONE_START = "XboxOneStart";

	// Token: 0x04000AB6 RID: 2742
	public const string UWP_START = "UWPStart";

	// Token: 0x04000AB7 RID: 2743
	public const string PS4_START = "PS4Start";

	// Token: 0x04000AB8 RID: 2744
	public const string GAMECORE_XBOX_START = "GameCoreXboxStart";

	// Token: 0x04000AB9 RID: 2745
	public const string WORLD = "worldGenerated";

	// Token: 0x04000ABA RID: 2746
	private static bool isActiveSceneSpecial = true;

	// Token: 0x04000ABB RID: 2747
	private static HashSet<string> specialScenes = new HashSet<string>
	{
		"CompanyLogoScene",
		"StandaloneStart",
		"XboxOneStart",
		"GameCoreXboxStart",
		"UWPStart",
		"PS4Start",
		"MainMenu"
	};
}
