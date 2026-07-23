using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200056C RID: 1388
public class DecorizerUIEntry : SRBehaviour
{
	// Token: 0x04001C03 RID: 7171
	[Tooltip("Main button component.")]
	public Button button;

	// Token: 0x04001C04 RID: 7172
	[Tooltip("Text component to display the item name.")]
	public new TMP_Text name;

	// Token: 0x04001C05 RID: 7173
	[Tooltip("Image component to display the item image.")]
	public Image image;

	// Token: 0x04001C06 RID: 7174
	[Tooltip("Text component to display the item content count.")]
	public TMP_Text count;
}
