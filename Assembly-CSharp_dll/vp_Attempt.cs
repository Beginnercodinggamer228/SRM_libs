using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x020007F9 RID: 2041
public class vp_Attempt : vp_Event
{
	// Token: 0x06002AF7 RID: 10999 RVA: 0x00013CC5 File Offset: 0x00011EC5
	public static bool AlwaysOK()
	{
		return true;
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x000A21B1 File Offset: 0x000A03B1
	public vp_Attempt(string name, Type eventArgumentType = null) : base(name, eventArgumentType, typeof(bool))
	{
		this.InitFields();
		this.EventType = vp_EventType.Attempt;
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x000A21D4 File Offset: 0x000A03D4
	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Try")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetType().GetMethod("AlwaysOK")
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Attempt.Tryer)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnAttempt_",
				0
			}
		};
		this.Try = new vp_Attempt.Tryer(vp_Attempt.AlwaysOK);
	}

	// Token: 0x040029DA RID: 10714
	public vp_Attempt.Tryer Try;

	// Token: 0x020007FA RID: 2042
	// (Invoke) Token: 0x06002AFB RID: 11003
	public delegate bool Tryer();
}
