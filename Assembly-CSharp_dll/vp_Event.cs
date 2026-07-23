using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x020007FE RID: 2046
public abstract class vp_Event
{
	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06002B06 RID: 11014 RVA: 0x000A22F0 File Offset: 0x000A04F0
	public string EventName
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x06002B07 RID: 11015 RVA: 0x000A22F8 File Offset: 0x000A04F8
	public Type ArgumentType
	{
		get
		{
			return this.m_ArgumentType;
		}
	}

	// Token: 0x170002AB RID: 683
	// (get) Token: 0x06002B08 RID: 11016 RVA: 0x000A2300 File Offset: 0x000A0500
	public Type ReturnType
	{
		get
		{
			return this.m_ReturnType;
		}
	}

	// Token: 0x06002B09 RID: 11017
	protected abstract void InitFields();

	// Token: 0x06002B0A RID: 11018 RVA: 0x000A2308 File Offset: 0x000A0508
	public vp_Event(string name = "", Type eventArgumentType = null, Type eventReturnType = null)
	{
		this.EventType = vp_EventType.Event;
		this.m_ArgumentType = eventArgumentType;
		if (eventReturnType == null)
		{
			this.m_ReturnType = typeof(void);
		}
		else
		{
			this.m_ReturnType = eventReturnType;
		}
		this.m_Name = name;
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x000A2348 File Offset: 0x000A0548
	protected void StoreInvokerFieldNames()
	{
		this.InvokerFieldNames = new string[this.m_Fields.Length];
		for (int i = 0; i < this.m_Fields.Length; i++)
		{
			this.InvokerFieldNames[i] = this.m_Fields[i].Name;
		}
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x00003296 File Offset: 0x00001496
	protected void Refresh()
	{
	}

	// Token: 0x040029E2 RID: 10722
	protected string m_Name;

	// Token: 0x040029E3 RID: 10723
	protected Type m_ArgumentType;

	// Token: 0x040029E4 RID: 10724
	protected Type m_ReturnType;

	// Token: 0x040029E5 RID: 10725
	protected FieldInfo[] m_Fields;

	// Token: 0x040029E6 RID: 10726
	protected Type[] m_DelegateTypes;

	// Token: 0x040029E7 RID: 10727
	protected MethodInfo[] m_DefaultMethods;

	// Token: 0x040029E8 RID: 10728
	public string[] InvokerFieldNames;

	// Token: 0x040029E9 RID: 10729
	public Dictionary<string, int> Prefixes;

	// Token: 0x040029EA RID: 10730
	public vp_EventType EventType;
}
