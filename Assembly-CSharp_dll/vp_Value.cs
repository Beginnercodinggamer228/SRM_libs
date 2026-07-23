using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x02000829 RID: 2089
public class vp_Value<V> : vp_Event
{
	// Token: 0x06002BCE RID: 11214 RVA: 0x000A4DB0 File Offset: 0x000A2FB0
	protected static T Empty<T>()
	{
		return default(T);
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x00003296 File Offset: 0x00001496
	protected static void Empty<T>(T value)
	{
	}

	// Token: 0x06002BD0 RID: 11216 RVA: 0x000A4DC8 File Offset: 0x000A2FC8
	public V GetEmpty()
	{
		return default(V);
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x00003296 File Offset: 0x00001496
	public void SetEmpty(V value)
	{
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x06002BD2 RID: 11218 RVA: 0x000A4DDE File Offset: 0x000A2FDE
	private FieldInfo[] Fields
	{
		get
		{
			return this.m_Fields;
		}
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x000A4DE6 File Offset: 0x000A2FE6
	public vp_Value(string name) : base(name, typeof(V), null)
	{
		this.InitFields();
		this.EventType = vp_EventType.Value;
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x000A4E08 File Offset: 0x000A3008
	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Get"),
			base.GetType().GetField("Set")
		};
		base.StoreInvokerFieldNames();
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Value<>.Getter<>),
			typeof(vp_Value<>.Setter<>)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"get_OnValue_",
				0
			},
			{
				"set_OnValue_",
				1
			}
		};
	}

	// Token: 0x04002A0B RID: 10763
	public vp_Value<V>.Getter<V> Get;

	// Token: 0x04002A0C RID: 10764
	public vp_Value<V>.Setter<V> Set;

	// Token: 0x0200082A RID: 2090
	// (Invoke) Token: 0x06002BD6 RID: 11222
	public delegate T Getter<T>();

	// Token: 0x0200082B RID: 2091
	// (Invoke) Token: 0x06002BDA RID: 11226
	public delegate void Setter<T>(T o);
}
