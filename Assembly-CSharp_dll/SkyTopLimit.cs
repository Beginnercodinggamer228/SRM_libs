using System;

// Token: 0x0200077E RID: 1918
public class SkyTopLimit : SRSingleton<SkyTopLimit>
{
	// Token: 0x06002821 RID: 10273 RVA: 0x000980DF File Offset: 0x000962DF
	public override void Awake()
	{
		base.Awake();
		this.bottomY = base.transform.position.y - 0.5f * base.transform.localScale.y;
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x00098114 File Offset: 0x00096314
	public float DownwardExtraGravity(float y)
	{
		float num = y - this.bottomY;
		if (num <= 0f)
		{
			return 0f;
		}
		return num * 0.04f;
	}

	// Token: 0x040027A7 RID: 10151
	private const float GRAV_PER_Y = 0.04f;

	// Token: 0x040027A8 RID: 10152
	private float bottomY;
}
