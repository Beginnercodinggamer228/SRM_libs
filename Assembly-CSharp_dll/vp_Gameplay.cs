using System;

// Token: 0x02000898 RID: 2200
public class vp_Gameplay
{
	// Token: 0x1700035A RID: 858
	// (get) Token: 0x0600303C RID: 12348 RVA: 0x000BE1AA File Offset: 0x000BC3AA
	// (set) Token: 0x0600303D RID: 12349 RVA: 0x000BE1BA File Offset: 0x000BC3BA
	public static bool isMaster
	{
		get
		{
			return !vp_Gameplay.isMultiplayer || vp_Gameplay.m_IsMaster;
		}
		set
		{
			if (!vp_Gameplay.isMultiplayer)
			{
				return;
			}
			vp_Gameplay.m_IsMaster = value;
		}
	}

	// Token: 0x04002E2A RID: 11818
	public static bool isMultiplayer = false;

	// Token: 0x04002E2B RID: 11819
	protected static bool m_IsMaster = true;
}
