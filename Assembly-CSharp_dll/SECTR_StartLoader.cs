using System;
using UnityEngine;

// Token: 0x020000B7 RID: 183
[RequireComponent(typeof(SECTR_Member))]
[AddComponentMenu("SECTR/Stream/SECTR Start Loader")]
public class SECTR_StartLoader : SECTR_Loader
{
	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000430 RID: 1072 RVA: 0x00019A98 File Offset: 0x00017C98
	public override bool Loaded
	{
		get
		{
			bool result = true;
			int num = this.cachedMember ? this.cachedMember.Sectors.Count : 0;
			for (int i = 0; i < num; i++)
			{
				SECTR_Sector sectr_Sector = this.cachedMember.Sectors[i];
				if (sectr_Sector.Frozen)
				{
					SECTR_Chunk component = sectr_Sector.GetComponent<SECTR_Chunk>();
					if (component && !component.IsLoaded())
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00019B10 File Offset: 0x00017D10
	private void OnEnable()
	{
		this.cachedMember = base.GetComponent<SECTR_Member>();
		if (this.FadeIn)
		{
			this.fadeTexture = new Texture2D(1, 1);
			this.fadeTexture.SetPixel(0, 0, this.FadeColor);
			this.fadeTexture.Apply();
		}
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00019B5C File Offset: 0x00017D5C
	private void OnDisable()
	{
		this.cachedMember = null;
		this.fadeTexture = null;
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00019B6C File Offset: 0x00017D6C
	private void Start()
	{
		this.cachedMember.ForceUpdate(true, false);
		int count = this.cachedMember.Sectors.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Chunk component = this.cachedMember.Sectors[i].GetComponent<SECTR_Chunk>();
			if (component)
			{
				component.AddReference();
			}
		}
		base.LockSelf(true);
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00019BCF File Offset: 0x00017DCF
	private void Update()
	{
		if (this.Loaded)
		{
			if (this.locked)
			{
				base.LockSelf(false);
			}
			if (!this.FadeIn)
			{
				Destroyer.Destroy(this, "SECTR_StartLoader.Update");
			}
		}
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x00019BFC File Offset: 0x00017DFC
	private void OnGUI()
	{
		if (this.FadeIn && base.enabled)
		{
			if (this.Loaded && !this.Paused)
			{
				float num = Time.deltaTime / this.FadeTime;
				this.fadeAmount -= num;
				this.fadeAmount = Mathf.Clamp01(this.fadeAmount);
			}
			GUI.color = new Color(1f, 1f, 1f, this.fadeAmount);
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.fadeTexture);
			if (this.fadeAmount == 0f)
			{
				Destroyer.Destroy(this, "SECTR_StartLoader.OnGUI");
			}
		}
	}

	// Token: 0x04000422 RID: 1058
	private Texture2D fadeTexture;

	// Token: 0x04000423 RID: 1059
	private float fadeAmount = 1f;

	// Token: 0x04000424 RID: 1060
	private SECTR_Member cachedMember;

	// Token: 0x04000425 RID: 1061
	[SECTR_ToolTip("Set to true if the scene should start at black and fade in when loaded.")]
	public bool FadeIn;

	// Token: 0x04000426 RID: 1062
	[SECTR_ToolTip("Amount of time to fade in.", "FadeIn")]
	public float FadeTime = 2f;

	// Token: 0x04000427 RID: 1063
	[SECTR_ToolTip("The color to fade the screen to on load.", "FadeIn")]
	public Color FadeColor = Color.black;

	// Token: 0x04000428 RID: 1064
	[NonSerialized]
	public bool Paused;
}
