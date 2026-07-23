using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200079E RID: 1950
[RequireComponent(typeof(Collider))]
public class ToyProximityTrigger : SRBehaviour
{
	// Token: 0x060028D9 RID: 10457 RVA: 0x0009A6F0 File Offset: 0x000988F0
	public void Awake()
	{
		this.id = base.GetComponentInParent<Identifiable>().id;
	}

	// Token: 0x060028DA RID: 10458 RVA: 0x0009A704 File Offset: 0x00098904
	public void OnDestroy()
	{
		List<GameObject> list = this.registered.Keys.ToList<GameObject>();
		for (int i = 0; i < list.Count; i++)
		{
			this.Deregister(list[i]);
		}
	}

	// Token: 0x060028DB RID: 10459 RVA: 0x0009A740 File Offset: 0x00098940
	public void Update()
	{
		if (Time.time >= this.nextCleanupTime)
		{
			List<GameObject> list = (from go in this.registered.Keys
			where go == null
			select go).ToList<GameObject>();
			for (int i = 0; i < list.Count; i++)
			{
				this.registered.Remove(list[i]);
			}
			this.nextCleanupTime = Time.time + 30f;
		}
	}

	// Token: 0x060028DC RID: 10460 RVA: 0x0009A7C4 File Offset: 0x000989C4
	public void OnTriggerEnter(Collider collider)
	{
		if (collider.isTrigger)
		{
			return;
		}
		SlimeEmotions component = collider.gameObject.GetComponent<SlimeEmotions>();
		if (component == null)
		{
			return;
		}
		ReactToToyNearby component2 = collider.gameObject.GetComponent<ReactToToyNearby>();
		if (component2 == null)
		{
			return;
		}
		if (this.registered.ContainsKey(collider.gameObject))
		{
			return;
		}
		this.Register(collider.gameObject, component, component2);
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x0009A828 File Offset: 0x00098A28
	public void OnTriggerExit(Collider collider)
	{
		if (collider.isTrigger)
		{
			return;
		}
		this.Deregister(collider.gameObject);
	}

	// Token: 0x060028DE RID: 10462 RVA: 0x0009A840 File Offset: 0x00098A40
	private void Register(GameObject other, SlimeEmotions emotions, ReactToToyNearby reaction)
	{
		ToyProximityTrigger.Metadata metadata = default(ToyProximityTrigger.Metadata);
		metadata.isFavorite = this.IsFavorite(other, reaction.slimeDefinition);
		this.registered[other] = metadata;
		emotions.AddNearbyToy(metadata.isFavorite);
		if (this.id == Identifiable.Id.RUBBER_DUCKY_TOY)
		{
			SlimeEatWater component = other.GetComponent<SlimeEatWater>();
			if (component != null)
			{
				component.EnterToyProximity();
			}
		}
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x0009A8A8 File Offset: 0x00098AA8
	private void Deregister(GameObject other)
	{
		ToyProximityTrigger.Metadata metadata;
		if (this.registered.TryGetValue(other, out metadata))
		{
			this.registered.Remove(other);
			if (other != null)
			{
				other.GetComponent<SlimeEmotions>().RemoveNearbyToy(metadata.isFavorite);
				if (this.id == Identifiable.Id.RUBBER_DUCKY_TOY)
				{
					SlimeEatWater component = other.GetComponent<SlimeEatWater>();
					if (component != null)
					{
						component.ExitToyProximity();
					}
				}
			}
		}
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x0009A910 File Offset: 0x00098B10
	private bool IsFavorite(GameObject other, SlimeDefinition slimeDefinition)
	{
		if (slimeDefinition.FavoriteToys.Contains(this.id))
		{
			return true;
		}
		if (this.fashion != Identifiable.Id.NONE)
		{
			AttachFashions component = other.GetComponent<AttachFashions>();
			if (component != null && component.HasFashion(this.fashion))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002850 RID: 10320
	[Tooltip("Fashion pairing. Slimes wearing this fashion will consider us a 'favorite' toy.")]
	public Identifiable.Id fashion;

	// Token: 0x04002851 RID: 10321
	private const float CLEANUP_TIME_DELAY = 30f;

	// Token: 0x04002852 RID: 10322
	private float nextCleanupTime;

	// Token: 0x04002853 RID: 10323
	private Dictionary<GameObject, ToyProximityTrigger.Metadata> registered = new Dictionary<GameObject, ToyProximityTrigger.Metadata>();

	// Token: 0x04002854 RID: 10324
	private Identifiable.Id id;

	// Token: 0x0200079F RID: 1951
	private struct Metadata
	{
		// Token: 0x04002855 RID: 10325
		public bool isFavorite;
	}
}
