using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class FPSCartoon : MonoBehaviour
{
	// Token: 0x0600017E RID: 382 RVA: 0x0000B438 File Offset: 0x00009638
	private void Awake()
	{
		this.guiStyleHeader.fontSize = 14;
		this.guiStyleHeader.normal.textColor = new Color(1f, 1f, 1f);
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0000B46B File Offset: 0x0000966B
	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 30f, 30f), "FPS: " + (int)this.fps, this.guiStyleHeader);
	}

	// Token: 0x06000180 RID: 384 RVA: 0x0000B4A8 File Offset: 0x000096A8
	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.fps = (float)this.frames;
			this.timeleft = 1f;
			this.frames = 0;
		}
	}

	// Token: 0x040001AF RID: 431
	private readonly GUIStyle guiStyleHeader = new GUIStyle();

	// Token: 0x040001B0 RID: 432
	private float timeleft;

	// Token: 0x040001B1 RID: 433
	private float fps;

	// Token: 0x040001B2 RID: 434
	private int frames;
}
