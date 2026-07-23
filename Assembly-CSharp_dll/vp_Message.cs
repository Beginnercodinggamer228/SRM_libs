using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x02000816 RID: 2070
public class vp_Message : vp_Event
{
	// Token: 0x06002B74 RID: 11124 RVA: 0x00003296 File Offset: 0x00001496
	protected static void Empty()
	{
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x000A3998 File Offset: 0x000A1B98
	public vp_Message(string name, Type eventArgumentType = null, Type eventReturnType = null) : base(name, eventArgumentType, eventReturnType)
	{
		this.InitFields();
		this.EventType = vp_EventType.Message;
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x000A39B0 File Offset: 0x000A1BB0
	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Send")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetType().GetMethod("Empty")
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message.Sender)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		this.Send = new vp_Message.Sender(vp_Message.Empty);
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x000A3A43 File Offset: 0x000A1C43
	public void Register(vp_Message.Sender sender)
	{
		this.Send = (vp_Message.Sender)Delegate.Combine(this.Send, sender);
		base.Refresh();
	}

	// Token: 0x040029FE RID: 10750
	public vp_Message.Sender Send;

	// Token: 0x02000817 RID: 2071
	// (Invoke) Token: 0x06002B79 RID: 11129
	public delegate void Sender();
}
