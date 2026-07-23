using System;
using UnityEngine;

// Token: 0x0200064C RID: 1612
public class StyleNameAttribute : PropertyAttribute
{
	// Token: 0x060021B6 RID: 8630 RVA: 0x000829D0 File Offset: 0x00080BD0
	public StyleNameAttribute(Type styleType)
	{
		this.styleType = styleType;
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x000829DF File Offset: 0x00080BDF
	public Type GetStyleType()
	{
		return this.styleType;
	}

	// Token: 0x04002100 RID: 8448
	private Type styleType;
}
