using System;

// Token: 0x020006A1 RID: 1697
public class MissingResourceException : Exception
{
	// Token: 0x0600237A RID: 9082 RVA: 0x00089B3B File Offset: 0x00087D3B
	public MissingResourceException(string resourceName) : base("Missing Resource: " + resourceName)
	{
	}
}
