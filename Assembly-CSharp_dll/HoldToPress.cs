using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000597 RID: 1431
public class HoldToPress : MonoBehaviour
{
	// Token: 0x06001DC1 RID: 7617 RVA: 0x00071516 File Offset: 0x0006F716
	public void OnEnable()
	{
		this.holdToPress = UnityEngine.Object.Instantiate<Image>(this.holdToPressPrefab, base.transform);
		this.holdToPress.fillAmount = 0.25f;
		this.holdComplete = false;
	}

	// Token: 0x06001DC2 RID: 7618 RVA: 0x00071546 File Offset: 0x0006F746
	public void OnDisable()
	{
		Destroyer.Destroy(this.holdToPress.gameObject, "HoldToPress.OnDisable");
		this.holdToPress = null;
		this.holdComplete = false;
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x0007156C File Offset: 0x0006F76C
	public void Update()
	{
		if (this.holdToPress != null)
		{
			if (this.holdToPress.fillAmount < 1f)
			{
				this.holdToPress.fillAmount += Time.unscaledDeltaTime / this.holdTime;
				return;
			}
			if (!this.holdComplete)
			{
				this.holdComplete = true;
				this.OnHoldComplete.Invoke();
			}
		}
	}

	// Token: 0x04001CD4 RID: 7380
	public float holdTime;

	// Token: 0x04001CD5 RID: 7381
	public Image holdToPressPrefab;

	// Token: 0x04001CD6 RID: 7382
	private Image holdToPress;

	// Token: 0x04001CD7 RID: 7383
	public UnityEvent OnHoldComplete;

	// Token: 0x04001CD8 RID: 7384
	private bool holdComplete;

	// Token: 0x04001CD9 RID: 7385
	private const float INITIAL_FILL_AMOUNT = 0.25f;

	// Token: 0x02000598 RID: 1432
	public class HoldCompleteEvent : UnityEvent
	{
	}
}
