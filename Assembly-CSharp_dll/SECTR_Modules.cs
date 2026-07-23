using System;
using UnityEngine;

// Token: 0x0200009E RID: 158
public static class SECTR_Modules
{
	// Token: 0x0600036E RID: 878 RVA: 0x000166E0 File Offset: 0x000148E0
	static SECTR_Modules()
	{
		SECTR_Modules.AUDIO = (Type.GetType("SECTR_AudioSystem") != null);
		SECTR_Modules.VIS = (Type.GetType("SECTR_CullingCamera") != null);
		SECTR_Modules.STREAM = (Type.GetType("SECTR_Chunk") != null);
		SECTR_Modules.DEV = (Type.GetType("SECTR_Tests") != null);
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00016763 File Offset: 0x00014963
	public static bool HasPro()
	{
		return Application.HasProLicense();
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0001676A File Offset: 0x0001496A
	public static bool HasComplete()
	{
		return SECTR_Modules.AUDIO && SECTR_Modules.VIS && SECTR_Modules.STREAM;
	}

	// Token: 0x0400039D RID: 925
	public static bool AUDIO = false;

	// Token: 0x0400039E RID: 926
	public static bool VIS = false;

	// Token: 0x0400039F RID: 927
	public static bool STREAM = false;

	// Token: 0x040003A0 RID: 928
	public static bool DEV = false;

	// Token: 0x040003A1 RID: 929
	public static string VERSION = "1.1.4f";
}
