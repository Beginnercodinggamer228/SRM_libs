using System;
using UnityEngine;

// Token: 0x020006DB RID: 1755
public class ConvertToCurrency : MonoBehaviour
{
	// Token: 0x060024A2 RID: 9378 RVA: 0x0008D2CC File Offset: 0x0008B4CC
	public void Awake()
	{
		this.convertTime = Time.time + this.delay;
		this.destroyTime = this.convertTime + 4f;
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.playerState.AddCurrency(this.amount, PlayerState.CoinsType.NONE);
		this.playerState.AddCurrencyDisplayDelta(-this.amount);
	}

	// Token: 0x060024A3 RID: 9379 RVA: 0x0008D334 File Offset: 0x0008B534
	private void Update()
	{
		if (Time.time >= this.convertTime && this.amount > 0)
		{
			SRSingleton<PopupElementsUI>.Instance.CreateCoinsPopup(this.amount, PlayerState.CoinsType.NORM);
			this.playerState.AddCurrencyDisplayDelta(this.amount);
			this.amount = 0;
		}
		if (Time.time >= this.destroyTime)
		{
			if (this.destroyFX != null)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.destroyFX, base.gameObject.transform.position, base.gameObject.transform.rotation);
			}
			Destroyer.Destroy(base.gameObject, "ConvertToCurrency.Update");
		}
	}

	// Token: 0x04002392 RID: 9106
	[Tooltip("Delay, in real-time seconds, before the currency is granted.")]
	public float delay = 0.25f;

	// Token: 0x04002393 RID: 9107
	[Tooltip("Amount of currency to grant.")]
	public int amount;

	// Token: 0x04002394 RID: 9108
	public GameObject destroyFX;

	// Token: 0x04002395 RID: 9109
	private PlayerState playerState;

	// Token: 0x04002396 RID: 9110
	private float convertTime;

	// Token: 0x04002397 RID: 9111
	private const float ANIMATION_DURATION = 4f;

	// Token: 0x04002398 RID: 9112
	private float destroyTime;
}
