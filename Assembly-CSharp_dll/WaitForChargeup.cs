using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007CB RID: 1995
public class WaitForChargeup : MonoBehaviour, GadgetModel.Participant
{
	// Token: 0x060029BF RID: 10687 RVA: 0x0009CE15 File Offset: 0x0009B015
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.gadgetDir = SRSingleton<SceneContext>.Instance.GadgetDirector;
	}

	// Token: 0x060029C0 RID: 10688 RVA: 0x0009CE37 File Offset: 0x0009B037
	public void InitModel(GadgetModel model)
	{
		model.waitForChargeupTime = this.timeDir.HoursFromNow(this.waitTimeGameHrs);
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x0009CE50 File Offset: 0x0009B050
	public void SetModel(GadgetModel model)
	{
		this.model = model;
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x0009CE5C File Offset: 0x0009B05C
	public void Update()
	{
		bool flag = !this.timeDir.HasReached(this.model.waitForChargeupTime);
		if (flag && this.waitingObj == null)
		{
			this.waitingObj = UnityEngine.Object.Instantiate<GameObject>(this.gadgetDir.waitForChargeupPrefab, base.transform.position, base.transform.rotation, base.transform);
			this.waitingText = this.waitingObj.transform.Find("InstallationRing/TextUI/ClockPanel/TimeText").GetComponent<Text>();
		}
		else if (!flag && this.waitingObj != null)
		{
			Destroyer.Destroy(this.waitingObj, "WaitForChargeup.Update");
			this.waitingObj = null;
			this.waitingText = null;
			this.lastWaitingMins = -1;
		}
		if (this.waitingText != null)
		{
			int num = (int)Math.Ceiling(this.timeDir.HoursUntil(this.model.waitForChargeupTime) * 60.0);
			if (num != this.lastWaitingMins)
			{
				this.waitingText.text = this.timeDir.FormatTime(num);
				this.lastWaitingMins = num;
			}
		}
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x0009CF7A File Offset: 0x0009B17A
	public bool IsWaiting()
	{
		return this.waitingObj != null;
	}

	// Token: 0x040028F4 RID: 10484
	public float waitTimeGameHrs = 2f;

	// Token: 0x040028F5 RID: 10485
	private TimeDirector timeDir;

	// Token: 0x040028F6 RID: 10486
	private GadgetDirector gadgetDir;

	// Token: 0x040028F7 RID: 10487
	private GameObject waitingObj;

	// Token: 0x040028F8 RID: 10488
	private Text waitingText;

	// Token: 0x040028F9 RID: 10489
	private int lastWaitingMins = -1;

	// Token: 0x040028FA RID: 10490
	private GadgetModel model;
}
