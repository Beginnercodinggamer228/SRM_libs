using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000395 RID: 917
public class AweTowardsLargos : FindConsumable
{
	// Token: 0x06001328 RID: 4904 RVA: 0x0004AE82 File Offset: 0x00049082
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.sfAnimator = base.GetComponent<SlimeFaceAnimator>();
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x0004AEA8 File Offset: 0x000490A8
	public override float Relevancy(bool isGrounded)
	{
		if (!isGrounded || !this.timeDir.HasReached(this.nextActivationTime))
		{
			return 0f;
		}
		AweTowardsLargos.localStaticLargoEntries.Clear();
		CellDirector.GetLargosNearMember(this.member, AweTowardsLargos.localStaticLargoEntries);
		float num;
		this.target = base.FindNearestConsumable(AweTowardsLargos.localStaticLargoEntries, out num);
		if (!(this.target == null))
		{
			return Randoms.SHARED.GetInRange(0.1f, 1f);
		}
		return 0f;
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x0004AF26 File Offset: 0x00049126
	public override void Action()
	{
		if (this.target != null)
		{
			base.RotateTowards(SlimeSubbehaviour.GetGotoPos(this.target) - base.transform.position);
		}
	}

	// Token: 0x0600132B RID: 4907 RVA: 0x0004AF57 File Offset: 0x00049157
	public override void Selected()
	{
		this.sfAnimator.SetTrigger("triggerLongAwe");
		this.nextActivationTime = this.timeDir.HoursFromNow(1f);
		this.endTime = Time.time + 3f;
	}

	// Token: 0x0600132C RID: 4908 RVA: 0x0004AF90 File Offset: 0x00049190
	public override bool CanRethink()
	{
		return Time.time >= this.endTime;
	}

	// Token: 0x0600132D RID: 4909 RVA: 0x0004AFA4 File Offset: 0x000491A4
	protected override Dictionary<Identifiable.Id, DriveCalculator> GetSearchIds()
	{
		SlimeVarietyModules component = base.GetComponent<SlimeVarietyModules>();
		Dictionary<Identifiable.Id, DriveCalculator> dictionary = new Dictionary<Identifiable.Id, DriveCalculator>(Identifiable.idComparer);
		foreach (Identifiable.Id id in Identifiable.LARGO_CLASS)
		{
			GameObject prefab = this.lookupDir.GetPrefab(id);
			if (prefab == null)
			{
				Log.Error("Null prefab!", new object[]
				{
					"id",
					id
				});
			}
			SlimeVarietyModules component2 = prefab.GetComponent<SlimeVarietyModules>();
			if (component2 != null)
			{
				bool flag = false;
				foreach (GameObject value in component.slimeModules)
				{
					if (Array.IndexOf<GameObject>(component2.slimeModules, value) != -1)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					dictionary[id] = AweTowardsLargos.largoDriveCalc;
				}
			}
		}
		return dictionary;
	}

	// Token: 0x040011F9 RID: 4601
	private GameObject target;

	// Token: 0x040011FA RID: 4602
	private static DriveCalculator largoDriveCalc = new DriveCalculator(SlimeEmotions.Emotion.NONE, 0f, 0f);

	// Token: 0x040011FB RID: 4603
	private TimeDirector timeDir;

	// Token: 0x040011FC RID: 4604
	private SlimeFaceAnimator sfAnimator;

	// Token: 0x040011FD RID: 4605
	private double nextActivationTime;

	// Token: 0x040011FE RID: 4606
	private float endTime;

	// Token: 0x040011FF RID: 4607
	private static List<GameObjectActorModelIdentifiableIndex.Entry> localStaticLargoEntries = new List<GameObjectActorModelIdentifiableIndex.Entry>();
}
