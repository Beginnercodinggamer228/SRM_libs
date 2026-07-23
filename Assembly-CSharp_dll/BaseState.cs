using System;

// Token: 0x0200000F RID: 15
public abstract class BaseState
{
	// Token: 0x06000045 RID: 69 RVA: 0x000030CD File Offset: 0x000012CD
	public BaseState(string name)
	{
		this.name = name;
	}

	// Token: 0x0400002C RID: 44
	public string name;

	// Token: 0x0400002D RID: 45
	public bool Enabled;
}
