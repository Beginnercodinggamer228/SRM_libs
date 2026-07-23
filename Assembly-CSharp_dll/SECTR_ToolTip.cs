using System;

// Token: 0x020000A6 RID: 166
[AttributeUsage(AttributeTargets.Field)]
public class SECTR_ToolTip : Attribute
{
	// Token: 0x060003B4 RID: 948 RVA: 0x00017796 File Offset: 0x00015996
	public SECTR_ToolTip(string tipText)
	{
		this.tipText = tipText;
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x000177A5 File Offset: 0x000159A5
	public SECTR_ToolTip(string tipText, float min, float max)
	{
		this.tipText = tipText;
		this.min = min;
		this.max = max;
		this.hasRange = true;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x000177C9 File Offset: 0x000159C9
	public SECTR_ToolTip(string tipText, string dependentProperty)
	{
		this.tipText = tipText;
		this.dependentProperty = dependentProperty;
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x000177DF File Offset: 0x000159DF
	public SECTR_ToolTip(string tipText, string dependentProperty, float min, float max)
	{
		this.tipText = tipText;
		this.dependentProperty = dependentProperty;
		this.min = min;
		this.max = max;
		this.hasRange = true;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0001780B File Offset: 0x00015A0B
	public SECTR_ToolTip(string tipText, bool devOnly)
	{
		this.tipText = tipText;
		this.devOnly = devOnly;
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00017821 File Offset: 0x00015A21
	public SECTR_ToolTip(string tipText, bool devOnly, bool treatAsLayer)
	{
		this.tipText = tipText;
		this.devOnly = devOnly;
		this.treatAsLayer = treatAsLayer;
	}

	// Token: 0x060003BA RID: 954 RVA: 0x0001783E File Offset: 0x00015A3E
	public SECTR_ToolTip(string tipText, string dependentProperty, Type enumType)
	{
		this.tipText = tipText;
		this.dependentProperty = dependentProperty;
		this.enumType = enumType;
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0001785B File Offset: 0x00015A5B
	public SECTR_ToolTip(string tipText, string dependentProperty, bool allowSceneObjects)
	{
		this.tipText = tipText;
		this.dependentProperty = dependentProperty;
		this.sceneObjectOverride = true;
		this.allowSceneObjects = allowSceneObjects;
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x060003BC RID: 956 RVA: 0x0001787F File Offset: 0x00015A7F
	public string TipText
	{
		get
		{
			return this.tipText;
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x060003BD RID: 957 RVA: 0x00017887 File Offset: 0x00015A87
	public string DependentProperty
	{
		get
		{
			return this.dependentProperty;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x060003BE RID: 958 RVA: 0x0001788F File Offset: 0x00015A8F
	public float Min
	{
		get
		{
			return this.min;
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x060003BF RID: 959 RVA: 0x00017897 File Offset: 0x00015A97
	public float Max
	{
		get
		{
			return this.max;
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060003C0 RID: 960 RVA: 0x0001789F File Offset: 0x00015A9F
	public Type EnumType
	{
		get
		{
			return this.enumType;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x060003C1 RID: 961 RVA: 0x000178A7 File Offset: 0x00015AA7
	public bool HasRange
	{
		get
		{
			return this.hasRange;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x060003C2 RID: 962 RVA: 0x000178AF File Offset: 0x00015AAF
	public bool DevOnly
	{
		get
		{
			return this.devOnly;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x060003C3 RID: 963 RVA: 0x000178B7 File Offset: 0x00015AB7
	public bool SceneObjectOverride
	{
		get
		{
			return this.sceneObjectOverride;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060003C4 RID: 964 RVA: 0x000178BF File Offset: 0x00015ABF
	public bool AllowSceneObjects
	{
		get
		{
			return this.allowSceneObjects;
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060003C5 RID: 965 RVA: 0x000178C7 File Offset: 0x00015AC7
	public bool TreatAsLayer
	{
		get
		{
			return this.treatAsLayer;
		}
	}

	// Token: 0x040003C1 RID: 961
	private string tipText;

	// Token: 0x040003C2 RID: 962
	private string dependentProperty;

	// Token: 0x040003C3 RID: 963
	private float min;

	// Token: 0x040003C4 RID: 964
	private float max;

	// Token: 0x040003C5 RID: 965
	private Type enumType;

	// Token: 0x040003C6 RID: 966
	private bool hasRange;

	// Token: 0x040003C7 RID: 967
	private bool devOnly;

	// Token: 0x040003C8 RID: 968
	private bool sceneObjectOverride;

	// Token: 0x040003C9 RID: 969
	private bool allowSceneObjects;

	// Token: 0x040003CA RID: 970
	private bool treatAsLayer;
}
