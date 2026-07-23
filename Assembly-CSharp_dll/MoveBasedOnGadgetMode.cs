using System;
using UnityEngine;

// Token: 0x02000736 RID: 1846
public class MoveBasedOnGadgetMode : MonoBehaviour
{
	// Token: 0x06002697 RID: 9879 RVA: 0x000933AB File Offset: 0x000915AB
	public void Awake()
	{
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000933C0 File Offset: 0x000915C0
	public void Update()
	{
		float num = this.playerState.InGadgetMode ? 1f : 0f;
		if (num > this.lerpVal)
		{
			this.lerpVal = Mathf.Min(num, this.lerpVal + 4f * Time.deltaTime);
		}
		else
		{
			this.lerpVal = Mathf.Max(num, this.lerpVal - 4f * Time.deltaTime);
		}
		this.toMove.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(this.gadgetModeOffPos, this.gadgetModeOnPos, this.lerpVal);
	}

	// Token: 0x040025D0 RID: 9680
	public GameObject toMove;

	// Token: 0x040025D1 RID: 9681
	public Vector2 gadgetModeOnPos;

	// Token: 0x040025D2 RID: 9682
	public Vector2 gadgetModeOffPos;

	// Token: 0x040025D3 RID: 9683
	private PlayerState playerState;

	// Token: 0x040025D4 RID: 9684
	private float lerpVal;

	// Token: 0x040025D5 RID: 9685
	private const float TRANS_SPEED = 4f;
}
