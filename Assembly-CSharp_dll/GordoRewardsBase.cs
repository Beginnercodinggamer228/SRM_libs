using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020003F5 RID: 1013
public abstract class GordoRewardsBase : SRBehaviour
{
	// Token: 0x06001525 RID: 5413 RVA: 0x00052020 File Offset: 0x00050220
	static GordoRewardsBase()
	{
		GordoRewardsBase.spawns[0] = Vector3.zero;
		for (int i = 0; i < 6; i++)
		{
			float f = 6.2831855f * (float)i / 6f;
			GordoRewardsBase.spawns[i + 1] = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		}
		for (int j = 0; j < 3; j++)
		{
			float f2 = 6.2831855f * (float)j / 3f + 0.5235988f;
			GordoRewardsBase.spawns[j + 7] = new Vector3(Mathf.Cos(f2) * 0.5f, 0.866f, Mathf.Sin(f2) * 0.5f);
		}
		for (int k = 0; k < 3; k++)
		{
			float f3 = 6.2831855f * (float)k / 3f - 0.5235988f;
			GordoRewardsBase.spawns[k + 10] = new Vector3(Mathf.Cos(f3) * 0.5f, -0.866f, Mathf.Sin(f3) * 0.5f);
		}
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x0005214A File Offset: 0x0005034A
	public void Start()
	{
		this.SetupActiveRewards();
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00052152 File Offset: 0x00050352
	public void SetupActiveRewards()
	{
		if (this.activeRewards == null)
		{
			this.activeRewards = new List<GameObject>(this.SelectActiveRewardPrefabs());
		}
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00052170 File Offset: 0x00050370
	public bool HasKeyReward()
	{
		if (this.activeRewards == null)
		{
			return false;
		}
		foreach (GameObject gameObject in this.activeRewards)
		{
			Identifiable component = gameObject.GetComponent<Identifiable>();
			if (component != null && component.id == Identifiable.Id.KEY)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x000521E8 File Offset: 0x000503E8
	public void GiveRewards()
	{
		if (this.activeRewards == null)
		{
			Log.Error("Active rewards on gordo are null.", new object[]
			{
				"gordo",
				base.name
			});
			return;
		}
		List<Identifiable.Id> allFashions = base.GetComponent<AttachFashions>().GetAllFashions();
		Identifiable component = base.gameObject.GetComponent<Identifiable>();
		Color[] colors = SlimeUtil.GetColors(base.gameObject, (component != null) ? component.id : Identifiable.Id.NONE, true);
		Region componentInParent = base.GetComponentInParent<Region>();
		List<Vector3> list = new List<Vector3>(GordoRewardsBase.spawns.Skip(1));
		int num = 0;
		while (list.Count > 0)
		{
			GameObject original = (num < this.activeRewards.Count) ? GordoRewardsBase.MaybeReplaceCratePrefab(this.activeRewards[num]) : this.slimePrefab;
			Vector3 vector = (num == 0) ? GordoRewardsBase.spawns[0] : Randoms.SHARED.Pluck<Vector3>(list, Vector3.zero);
			Vector3 position = base.transform.position + vector * 1.2f + GordoRewardsBase.SPAWN_OFFSET;
			Quaternion rotation = (num == 0) ? Quaternion.identity : Quaternion.LookRotation(vector, Vector3.up);
			GameObject gameObject = SRBehaviour.InstantiateActor(original, componentInParent.setId, position, rotation, true);
			gameObject.GetComponent<Rigidbody>().AddTorque(Randoms.SHARED.GetInRange(-10f, 10f), Randoms.SHARED.GetInRange(-10f, 10f), Randoms.SHARED.GetInRange(-10f, 10f));
			AttachFashions component2 = gameObject.GetComponent<AttachFashions>();
			if (component2 != null)
			{
				component2.SetFashions(allFashions);
			}
			RecolorSlimeMaterial[] componentsInChildren = SRBehaviour.SpawnAndPlayFX(this.slimeSpawnFXPrefab, position, rotation).GetComponentsInChildren<RecolorSlimeMaterial>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetColors(colors[0], colors[1], colors[2]);
			}
			this.OnInstantiatedReward(gameObject);
			num++;
		}
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x000523E0 File Offset: 0x000505E0
	private static GameObject MaybeReplaceCratePrefab(GameObject prefab)
	{
		if (!SRSingleton<SceneContext>.Instance.GameModel.GetHolidayModel().eventGordos.Any<HolidayModel.EventGordo>() || !Identifiable.STANDARD_CRATE_CLASS.Contains(Identifiable.GetId(prefab)) || !Randoms.SHARED.GetProbability(HolidayModel.EventGordo.CRATE_CHANCE))
		{
			return prefab;
		}
		return SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(HolidayModel.EventGordo.CRATE);
	}

	// Token: 0x0600152B RID: 5419
	protected abstract IEnumerable<GameObject> SelectActiveRewardPrefabs();

	// Token: 0x0600152C RID: 5420 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnInstantiatedReward(GameObject instance)
	{
	}

	// Token: 0x040013FA RID: 5114
	public GameObject slimePrefab;

	// Token: 0x040013FB RID: 5115
	public GameObject slimeSpawnFXPrefab;

	// Token: 0x040013FC RID: 5116
	private List<GameObject> activeRewards;

	// Token: 0x040013FD RID: 5117
	private const float SPAWN_RAD = 1.2f;

	// Token: 0x040013FE RID: 5118
	private const float SPAWN_VERT_OFFSET = 1.7f;

	// Token: 0x040013FF RID: 5119
	private const float SPAWN_TORQUE = 10f;

	// Token: 0x04001400 RID: 5120
	private static readonly Vector3 SPAWN_OFFSET = new Vector3(0f, 1.7f, 0f);

	// Token: 0x04001401 RID: 5121
	private static Vector3[] spawns = new Vector3[13];
}
