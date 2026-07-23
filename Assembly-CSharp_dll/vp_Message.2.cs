using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x02000818 RID: 2072
public class vp_Message<V> : vp_Message
{
	// Token: 0x06002B7C RID: 11132 RVA: 0x00003296 File Offset: 0x00001496
	protected static void Empty<T>(T value)
	{
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x00003296 File Offset: 0x00001496
	public void EmptySender(V value)
	{
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x000A3A62 File Offset: 0x000A1C62
	public vp_Message(string name) : base(name, typeof(V), null)
	{
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x000A3A78 File Offset: 0x000A1C78
	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Send")
		};
		base.StoreInvokerFieldNames();
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message<>.Sender<>)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		this.Send = new vp_Message<V>.Sender<V>(this.EmptySender);
	}

	// Token: 0x040029FF RID: 10751
	public new vp_Message<V>.Sender<V> Send;

	// Token: 0x02000819 RID: 2073
	// (Invoke) Token: 0x06002B81 RID: 11137
	public delegate void Sender<T>(T value);
}
