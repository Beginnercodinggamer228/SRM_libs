using System;
using UnityEngine;

// Token: 0x020005E9 RID: 1513
public class PlayerMapMarker : MapMarker
{
	// Token: 0x06001FCB RID: 8139 RVA: 0x000793B1 File Offset: 0x000775B1
	public override void Rotate(Quaternion rotation)
	{
		this.arrowRect.rotation = Quaternion.Euler(0f, 0f, -(rotation.eulerAngles.y + 45f));
	}

	// Token: 0x04001EF6 RID: 7926
	public RectTransform arrowRect;

	// Token: 0x04001EF7 RID: 7927
	public RectTransform iconRect;
}
