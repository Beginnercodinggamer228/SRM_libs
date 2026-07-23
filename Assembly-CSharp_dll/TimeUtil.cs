using System;

// Token: 0x020006AB RID: 1707
public static class TimeUtil
{
	// Token: 0x0600239D RID: 9117 RVA: 0x00089FF1 File Offset: 0x000881F1
	public static bool HasReached(double currentWorldTime, double targetWorldTime)
	{
		return currentWorldTime >= targetWorldTime;
	}
}
