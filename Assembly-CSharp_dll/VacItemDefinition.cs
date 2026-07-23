using System;
using UnityEngine;

// Token: 0x020006B1 RID: 1713
[CreateAssetMenu(menuName = "Vac/Vac Item Definition")]
public class VacItemDefinition : ScriptableObject
{
	// Token: 0x17000244 RID: 580
	// (get) Token: 0x060023AD RID: 9133 RVA: 0x0008A37A File Offset: 0x0008857A
	public Identifiable.Id Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x060023AE RID: 9134 RVA: 0x0008A382 File Offset: 0x00088582
	public Color Color
	{
		get
		{
			return this.color;
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x060023AF RID: 9135 RVA: 0x0008A38A File Offset: 0x0008858A
	public Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x0008A392 File Offset: 0x00088592
	public static VacItemDefinition CreateVacItemDefinition(Identifiable.Id id, Color color, Sprite icon)
	{
		VacItemDefinition vacItemDefinition = ScriptableObject.CreateInstance<VacItemDefinition>();
		vacItemDefinition.id = id;
		vacItemDefinition.color = color;
		vacItemDefinition.icon = icon;
		return vacItemDefinition;
	}

	// Token: 0x040022D7 RID: 8919
	[SerializeField]
	private Identifiable.Id id;

	// Token: 0x040022D8 RID: 8920
	[SerializeField]
	private Color color;

	// Token: 0x040022D9 RID: 8921
	[SerializeField]
	private Sprite icon;
}
