using System;
using System.Linq;
using UnityEngine.UI;

// Token: 0x020005AD RID: 1453
public static class ToggleGroupExtension
{
	// Token: 0x06001E1A RID: 7706 RVA: 0x000724EC File Offset: 0x000706EC
	public static Toggle GetActive(this ToggleGroup aGroup)
	{
		return aGroup.ActiveToggles().FirstOrDefault<Toggle>();
	}
}
