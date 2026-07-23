using System;

// Token: 0x02000534 RID: 1332
internal class UFPSCompilerHints
{
	// Token: 0x06001BB4 RID: 7092 RVA: 0x00069E6C File Offset: 0x0006806C
	public static bool CompilerHints()
	{
		bool flag = new vp_Message<int>("test") != null;
		vp_Message<string, int> vp_Message = new vp_Message<string, int>("test");
		return flag || vp_Message != null || true;
	}
}
