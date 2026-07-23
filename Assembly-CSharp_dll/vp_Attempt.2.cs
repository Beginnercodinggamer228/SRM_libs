using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x020007FB RID: 2043
public class vp_Attempt<V> : vp_Attempt
{
	// Token: 0x06002AFE RID: 11006 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected static bool AlwaysOK<T>(T value)
	{
		return true;
	}

	// Token: 0x06002AFF RID: 11007 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public bool AttemptAlwaysOK(V value)
	{
		return true;
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x000A2267 File Offset: 0x000A0467
	public vp_Attempt(string name) : base(name, typeof(V))
	{
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x000A227C File Offset: 0x000A047C
	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Try")
		};
		base.StoreInvokerFieldNames();
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Attempt<>.Tryer<>)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnAttempt_",
				0
			}
		};
		this.Try = new vp_Attempt<V>.Tryer<V>(this.AttemptAlwaysOK);
	}

	// Token: 0x040029DB RID: 10715
	public new vp_Attempt<V>.Tryer<V> Try;

	// Token: 0x020007FC RID: 2044
	// (Invoke) Token: 0x06002B03 RID: 11011
	public delegate bool Tryer<T>(T value);
}
