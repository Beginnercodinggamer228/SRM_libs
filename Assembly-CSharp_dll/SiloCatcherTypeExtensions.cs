using System;

// Token: 0x0200033F RID: 831
public static class SiloCatcherTypeExtensions
{
	// Token: 0x06001172 RID: 4466 RVA: 0x000464E1 File Offset: 0x000446E1
	public static bool HasInput(this SiloCatcher.Type type)
	{
		return type == SiloCatcher.Type.SILO_DEFAULT || type == SiloCatcher.Type.REFINERY || type == SiloCatcher.Type.DECORIZER || type == SiloCatcher.Type.VIKTOR_STORAGE;
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x000464F4 File Offset: 0x000446F4
	public static bool HasOutput(this SiloCatcher.Type type)
	{
		return type == SiloCatcher.Type.SILO_DEFAULT || type == SiloCatcher.Type.SILO_OUTPUT_ONLY || type == SiloCatcher.Type.DECORIZER || type == SiloCatcher.Type.VIKTOR_STORAGE;
	}
}
