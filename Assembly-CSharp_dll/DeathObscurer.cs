using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000297 RID: 663
public class DeathObscurer : MonoBehaviour
{
	// Token: 0x06000DF0 RID: 3568 RVA: 0x00038958 File Offset: 0x00036B58
	public void Start()
	{
		LockOnDeath instance = SRSingleton<LockOnDeath>.Instance;
		instance.onLockChanged = (LockOnDeath.OnLockChanged)Delegate.Combine(instance.onLockChanged, new LockOnDeath.OnLockChanged(this.OnLocked));
		this.bgImage = base.GetComponent<Image>();
		this.adjust = 1f;
		SRSingleton<SceneContext>.Instance.PlayerState.onEndGame += delegate()
		{
			Color color = this.bgImage.color;
			color.a = 0f;
			this.bgImage.color = color;
			this.targetAlpha = 0f;
		};
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x000389BD File Offset: 0x00036BBD
	public void OnLocked(bool locked)
	{
		if (locked)
		{
			this.targetAlpha = 1f;
			base.gameObject.SetActive(true);
			return;
		}
		this.targetAlpha = 0f;
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x000389E8 File Offset: 0x00036BE8
	public void Update()
	{
		float a = this.bgImage.color.a;
		if (a < this.targetAlpha)
		{
			this.bgImage.color = new Color(this.bgImage.color.r, this.bgImage.color.g, this.bgImage.color.b, Mathf.Min(this.targetAlpha, a + this.adjust * Time.deltaTime));
		}
		else if (a > this.targetAlpha)
		{
			this.bgImage.color = new Color(this.bgImage.color.r, this.bgImage.color.g, this.bgImage.color.b, Mathf.Max(this.targetAlpha, a - this.adjust * Time.deltaTime));
		}
		if (this.bgImage.color.a == 0f)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000D1B RID: 3355
	private Image bgImage;

	// Token: 0x04000D1C RID: 3356
	private float targetAlpha;

	// Token: 0x04000D1D RID: 3357
	private float adjust;
}
