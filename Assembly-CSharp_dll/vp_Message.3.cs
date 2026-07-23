using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x0200081A RID: 2074
public class vp_Message<V, VResult> : vp_Message
{
	// Token: 0x06002B84 RID: 11140 RVA: 0x000A3AEC File Offset: 0x000A1CEC
	protected static TResult Empty<T, TResult>(T value)
	{
		return default(TResult);
	}

	// Token: 0x06002B85 RID: 11141 RVA: 0x000A3B04 File Offset: 0x000A1D04
	protected VResult EmptySender(V value)
	{
		return default(VResult);
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x000A3B1A File Offset: 0x000A1D1A
	public vp_Message(string name) : base(name, typeof(V), typeof(VResult))
	{
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x000A3B38 File Offset: 0x000A1D38
	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Send")
		};
		base.StoreInvokerFieldNames();
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message<, >.Sender<, >)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		this.Send = new vp_Message<V, VResult>.Sender<V, VResult>(this.EmptySender);
	}

	// Token: 0x04002A00 RID: 10752
	public new vp_Message<V, VResult>.Sender<V, VResult> Send;

	// Token: 0x0200081B RID: 2075
	// (Invoke) Token: 0x06002B89 RID: 11145
	public delegate TResult Sender<T, TResult>(T value);
}
