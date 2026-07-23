using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000190 RID: 400
public abstract class DroneProgramSourceDynamic : DroneProgramSource<Identifiable>
{
	// Token: 0x06000882 RID: 2178 RVA: 0x00027BC0 File Offset: 0x00025DC0
	public override void Selected()
	{
		base.Selected();
		this.pickupDelay = null;
		this.maxPickup = null;
		if (this.source != null)
		{
			this.source.gameObject.GetComponent<RegionMember>().regionsChanged += this.OnRegionsChanged;
		}
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x00027C1A File Offset: 0x00025E1A
	public override void Deselected()
	{
		base.Deselected();
		if (this.source != null)
		{
			this.source.gameObject.GetComponent<RegionMember>().regionsChanged -= this.OnRegionsChanged;
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x00027C54 File Offset: 0x00025E54
	protected override bool CanCancel()
	{
		if (this.source == null || !this.source.gameObject.activeInHierarchy)
		{
			return true;
		}
		if (this.maxPickup != null)
		{
			int? num = this.maxPickup;
			int num2 = 0;
			return num.GetValueOrDefault() <= num2 & num != null;
		}
		return false;
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x00027CB0 File Offset: 0x00025EB0
	protected override IEnumerable<DroneProgram.Orientation> GetTargetOrientations(Identifiable source)
	{
		return base.GetTargetOrientations_Gather(source.gameObject);
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x00027CBE File Offset: 0x00025EBE
	protected override Vector3 GetTargetPosition(Identifiable source)
	{
		return source.transform.position;
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x000238AA File Offset: 0x00021AAA
	protected override GameObject GetTargetGameObject(Identifiable source)
	{
		return source.gameObject;
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00027CCB File Offset: 0x00025ECB
	protected override void OnReachedDestination()
	{
		base.OnReachedDestination();
		this.maxPickup = new int?(this.GetMaxPickup(this.source.id));
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x00027CF0 File Offset: 0x00025EF0
	protected override bool OnAction()
	{
		if (this.pickupDelay == null)
		{
			this.pickupDelay = new float?(Time.fixedTime + (this.SphereCastPickup(this.source, this.maxPickup.Value, this.GetPickupRadius(), new Predicate<Identifiable>(this.SourcePredicate)) ? 0.3f : 0f));
		}
		return this.pickupDelay.Value <= Time.fixedTime;
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00027D68 File Offset: 0x00025F68
	protected override IEnumerable<Identifiable> GetSources(Predicate<Identifiable.Id> predicate)
	{
		return from e in this.drone.network.gameObject.GetComponent<CellDirector>().identifiableIndex.GetAllRegistered()
		select e.GameObject.GetComponent<Identifiable>() into s
		where predicate(s.id) && this.SourcePredicate(s)
		orderby (s.transform.position - this.drone.transform.position).sqrMagnitude
		select s;
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00027DF0 File Offset: 0x00025FF0
	public override IEnumerable<DroneFastForwarder.GatherGroup> GetFastForwardGroups(double endTime)
	{
		return (from source in this.GetSources(this.predicate)
		group source by source.id into @group
		select new DroneFastForwarder.GatherGroup.Dynamic(@group) into @group
		where @group.Any()
		select @group).Cast<DroneFastForwarder.GatherGroup>();
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00027E7A File Offset: 0x0002607A
	protected bool SourcePredicate(Identifiable source)
	{
		return this.SourcePredicate(this.drone.network.GetContaining(source), source);
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00027E94 File Offset: 0x00026094
	protected virtual bool SourcePredicate(DroneNetwork.LandPlotMetadata metadata, Identifiable source)
	{
		return source != null && source.transform.parent == null && source.gameObject.activeInHierarchy && DroneNetwork.IsResourceReady(source.gameObject);
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00027ECC File Offset: 0x000260CC
	protected virtual float GetPickupRadius()
	{
		return 1f;
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00027ED4 File Offset: 0x000260D4
	protected virtual int GetMaxPickup(Identifiable.Id id)
	{
		int slotMaxCount = this.drone.ammo.GetSlotMaxCount();
		int availableDestinationSpace = base.GetAvailableDestinationSpace(id);
		return Mathf.Min(slotMaxCount, availableDestinationSpace) - this.drone.ammo.GetSlotCount();
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x00027F10 File Offset: 0x00026110
	private void OnRegionsChanged(List<Region> left, List<Region> joined)
	{
		if (this.source != null)
		{
			this.source.gameObject.GetComponent<RegionMember>().regionsChanged -= this.OnRegionsChanged;
			this.source = null;
		}
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x00027F48 File Offset: 0x00026148
	protected bool SphereCastPickup(Identifiable source, int maxPickup, float radius, Predicate<Identifiable> predicate)
	{
		if (maxPickup < 1 || !predicate(source) || !this.drone.ammo.CouldAddToSlot(source.id))
		{
			return false;
		}
		this.SphereCastPickupTween(source);
		int num = maxPickup - 1;
		if (num <= 0)
		{
			return true;
		}
		HashSet<GameObject> hashSet = new HashSet<GameObject>
		{
			source.gameObject
		};
		Vector3 direction = source.transform.position - this.drone.transform.position;
		foreach (RaycastHit raycastHit in Physics.SphereCastAll(this.drone.transform.position, radius, direction, direction.magnitude, 34816, QueryTriggerInteraction.Ignore))
		{
			Identifiable component = raycastHit.collider.gameObject.GetComponent<Identifiable>();
			if (component != null && component.id == source.id && hashSet.Add(component.gameObject) && predicate(component))
			{
				this.SphereCastPickupTween(component);
				num--;
				if (num <= 0)
				{
					break;
				}
			}
		}
		return true;
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x00028064 File Offset: 0x00026264
	protected void SphereCastPickupTween(Identifiable source)
	{
		List<Collider> list = new List<Collider>();
		foreach (Collider collider in source.gameObject.GetComponents<Collider>())
		{
			if (collider.enabled)
			{
				list.Add(collider);
				collider.enabled = false;
			}
		}
		float endValue = source.transform.localScale.x * 0.2f;
		float x = source.transform.localScale.x;
		DOTween.Sequence().Append(source.transform.DOScale(endValue, 0.3f).SetEase(Ease.Linear)).Append(source.transform.DOScale(x, 0.3f).SetEase(Ease.Linear));
		source.transform.DOMove(this.drone.transform.position, 0.375f, false).SetEase(Ease.Linear);
		source.transform.DORotate(Quaternion.LookRotation(this.drone.transform.position - source.transform.position).eulerAngles, 0.45000002f, RotateMode.Fast).SetEase(Ease.Linear);
		SRSingleton<SceneContext>.Instance.StartCoroutine(this.SphereCastPickupCoroutine(source, list));
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0002819B File Offset: 0x0002639B
	protected IEnumerator SphereCastPickupCoroutine(Identifiable identifiable, List<Collider> disabledColliders)
	{
		yield return new WaitForSeconds(0.3f);
		if (identifiable != null)
		{
			if (this.drone.ammo.MaybeAddToSlot(identifiable.id))
			{
				Destroyer.DestroyActor(identifiable.gameObject, "DroneSubbehaviour.SphereCastPickupCoroutine", false);
			}
			else
			{
				using (List<Collider>.Enumerator enumerator = disabledColliders.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Collider collider = enumerator.Current;
						collider.enabled = true;
					}
					yield break;
				}
			}
		}
		yield break;
	}

	// Token: 0x04000765 RID: 1893
	private float? pickupDelay;

	// Token: 0x04000766 RID: 1894
	private int? maxPickup;

	// Token: 0x04000767 RID: 1895
	private const int PICKUP_MASK = 34816;

	// Token: 0x04000768 RID: 1896
	protected const float PICKUP_DURATION = 0.3f;
}
