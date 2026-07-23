using System;

// Token: 0x020007F8 RID: 2040
public class vp_Activity<V> : vp_Activity
{
	// Token: 0x06002AF5 RID: 10997 RVA: 0x000A2189 File Offset: 0x000A0389
	public vp_Activity(string name) : base(name)
	{
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000A2192 File Offset: 0x000A0392
	public bool TryStart<T>(T argument)
	{
		if (this.m_Active)
		{
			return false;
		}
		this.m_Argument = argument;
		return base.TryStart(true);
	}
}
