using System;
using RichPresence;
using UnityEngine;

// Token: 0x02000899 RID: 2201
public class UWPContext : MonoBehaviour, Handler
{
	// Token: 0x06003040 RID: 12352 RVA: 0x000BE1D8 File Offset: 0x000BC3D8
	public void SetRichPresence(MainMenuData data)
	{
		this.SetRichPresence("MainMenu");
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x000BE1E8 File Offset: 0x000BC3E8
	public void SetRichPresence(InZoneData data)
	{
		string richPresence;
		if (Director.TryGetZoneId(data.zone, out richPresence))
		{
			this.SetRichPresence(richPresence);
		}
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x00003296 File Offset: 0x00001496
	private void SetRichPresence(string id)
	{
	}

	// Token: 0x04002E2C RID: 11820
	public GameObject ControllerDisconnectedPopup;

	// Token: 0x04002E2D RID: 11821
	public GameObject UserSignOutPopup;
}
