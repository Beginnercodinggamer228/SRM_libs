using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020004B3 RID: 1203
public class TrackContainedIdentifiables : SRBehaviour
{
	// Token: 0x06001930 RID: 6448 RVA: 0x00003296 File Offset: 0x00001496
	public void Awake()
	{
	}

	// Token: 0x06001931 RID: 6449 RVA: 0x00062312 File Offset: 0x00060512
	public void GetTrackedItemsOfType(Identifiable.Id identId, List<Identifiable> trackedItems)
	{
		if (this.trackedObjects.ContainsKey(identId))
		{
			trackedItems.AddRange(this.trackedObjects[identId]);
		}
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x00062334 File Offset: 0x00060534
	public void GetTrackedItemsOfClass(HashSet<Identifiable.Id> idClass, List<Identifiable> trackedItems)
	{
		foreach (Identifiable.Id identId in idClass)
		{
			this.GetTrackedItemsOfType(identId, trackedItems);
		}
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x00062384 File Offset: 0x00060584
	public void GetTrackedItemsOfType<T>(Identifiable.Id identId, List<T> trackedItems)
	{
		if (this.trackedObjects.ContainsKey(identId))
		{
			foreach (Identifiable identifiable in this.trackedObjects[identId])
			{
				T component = identifiable.GetComponent<T>();
				trackedItems.Add(component);
			}
		}
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x000623F0 File Offset: 0x000605F0
	public void GetTrackedItemsOfClass<T>(HashSet<Identifiable.Id> idClass, List<T> trackedItems)
	{
		foreach (Identifiable.Id identId in idClass)
		{
			this.GetTrackedItemsOfType<T>(identId, trackedItems);
		}
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x00062440 File Offset: 0x00060640
	public void GetTrackedItemsOfClass(HashSet<Identifiable.Id> idClass, List<GameObject> trackedItems)
	{
		foreach (Identifiable.Id key in idClass)
		{
			if (this.trackedObjects.ContainsKey(key))
			{
				foreach (Identifiable identifiable in this.trackedObjects[key])
				{
					trackedItems.Add(identifiable.gameObject);
				}
			}
		}
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x000624E4 File Offset: 0x000606E4
	public bool HasIdentifiableType(Identifiable.Id identId)
	{
		return this.trackedObjects.ContainsKey(identId) && this.trackedObjects[identId].Count > 0;
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x0006250A File Offset: 0x0006070A
	public void GetTrackedIdentifiableTypes(List<Identifiable.Id> identIds)
	{
		identIds.AddRange(this.uniqueIdentifiableTypes);
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x00062518 File Offset: 0x00060718
	public void GetTrackedIdentifiableTypes(HashSet<Identifiable.Id> typesToFind)
	{
		typesToFind.IntersectWith(this.uniqueIdentifiableTypes);
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x00062526 File Offset: 0x00060726
	public IEnumerable<KeyValuePair<Identifiable.Id, HashSet<Identifiable>>> GetAllTracked()
	{
		return this.trackedObjects;
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x0006252E File Offset: 0x0006072E
	public IEnumerable<Identifiable.Id> GetTrackedIdentifiableTypes()
	{
		return this.uniqueIdentifiableTypes;
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x00062536 File Offset: 0x00060736
	public int Count(Identifiable.Id id)
	{
		if (this.trackedObjects.ContainsKey(id))
		{
			return this.trackedObjects[id].Count;
		}
		return 0;
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x00062559 File Offset: 0x00060759
	public bool Contains(Identifiable ident)
	{
		return this.trackedObjects.ContainsKey(ident.id) && this.trackedObjects[ident.id].Contains(ident);
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x00062588 File Offset: 0x00060788
	public Identifiable RemoveTrackedObject(Identifiable.Id id)
	{
		Identifiable identifiable = this.trackedObjects[id].First<Identifiable>();
		this.RemoveTrackedObject(identifiable);
		return identifiable;
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x000625B0 File Offset: 0x000607B0
	public void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
		{
			return;
		}
		Identifiable identifiable = this.GetIdentifiable(other);
		if (identifiable != null)
		{
			this.AddTrackedObject(identifiable);
		}
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x000625E0 File Offset: 0x000607E0
	public void OnTriggerExit(Collider other)
	{
		if (other.isTrigger)
		{
			return;
		}
		Identifiable identifiable = this.GetIdentifiable(other);
		if (identifiable != null)
		{
			this.RemoveTrackedObject(identifiable);
		}
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x00062610 File Offset: 0x00060810
	private Identifiable GetIdentifiable(Collider col)
	{
		Identifiable result = null;
		if (col.gameObject != null)
		{
			result = col.gameObject.GetComponent<Identifiable>();
		}
		return result;
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x0006263A File Offset: 0x0006083A
	public void OnTrackedDestroyed(Identifiable trackedObject)
	{
		this.RemoveTrackedObject(trackedObject);
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x00062644 File Offset: 0x00060844
	private void AddTrackedObject(Identifiable ident)
	{
		if (!this.trackedObjects.ContainsKey(ident.id))
		{
			this.trackedObjects.Add(ident.id, new HashSet<Identifiable>());
		}
		if (!this.trackedObjects[ident.id].Add(ident))
		{
			return;
		}
		if (this.OnIdentifiableEntered != null)
		{
			this.OnIdentifiableEntered(this, ident);
		}
		if (!this.uniqueIdentifiableTypes.Contains(ident.id))
		{
			this.uniqueIdentifiableTypes.Add(ident.id);
			if (this.OnNewIdentifiableTypeEntered != null)
			{
				this.OnNewIdentifiableTypeEntered(this, ident);
			}
		}
		ident.NotifyOnDestroy = (Identifiable.OnDestroyListener)Delegate.Combine(ident.NotifyOnDestroy, new Identifiable.OnDestroyListener(this.OnTrackedDestroyed));
		if (this.IsTrackingIntegrity(ident))
		{
			int instanceID = ident.gameObject.GetInstanceID();
			Destroyer.Monitor(ident.gameObject, delegate(Destroyer.Metadata metadata)
			{
				this.metadataDict[instanceID] = metadata;
			});
		}
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x00062744 File Offset: 0x00060944
	private void RemoveTrackedObject(Identifiable ident)
	{
		if (!this.trackedObjects.ContainsKey(ident.id))
		{
			Log.Debug("Request to remove object where the Identifiable.Id is not being tracked.", new object[]
			{
				"Identifiable.Id",
				ident.id
			});
			return;
		}
		if (this.trackedObjects[ident.id].Remove(ident))
		{
			if (this.trackedObjects[ident.id].Count == 0)
			{
				this.uniqueIdentifiableTypes.Remove(ident.id);
			}
			ident.NotifyOnDestroy = (Identifiable.OnDestroyListener)Delegate.Remove(ident.NotifyOnDestroy, new Identifiable.OnDestroyListener(this.OnTrackedDestroyed));
			if (this.IsTrackingIntegrity(ident))
			{
				this.wasRemoved.Add(ident.gameObject.GetInstanceID());
			}
		}
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x0006280F File Offset: 0x00060A0F
	public void OnDestroy()
	{
		this.airNets.Clear();
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x0006281C File Offset: 0x00060A1C
	private bool IsTrackingIntegrity(Identifiable ident)
	{
		if (!Identifiable.IsSlime(ident.id) || Identifiable.IsTarr(ident.id))
		{
			return false;
		}
		if (ident.gameObject.GetComponent<QuantumSlimeSuperposition>() != null)
		{
			return false;
		}
		if (!this.airNets.Any((AirNet net) => net.IsNetActive()))
		{
			return false;
		}
		int instanceID = ident.gameObject.GetInstanceID();
		Destroyer.Metadata metadata;
		this.metadataDict.TryGetValue(instanceID, out metadata);
		return metadata == null || (!metadata.source.Contains("DestroyOnTouching.DestroyAndWater") && !metadata.source.Contains("DestroyOutsideHoursOfDay") && !metadata.source.Contains("Vacuumable"));
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x000628E0 File Offset: 0x00060AE0
	public void LateUpdate()
	{
		if (Time.frameCount >= this.nextTrackFrameCount)
		{
			if (this.wasRemoved.Count >= 5)
			{
				Log.Error("Found potential missing slime issue... " + new
				{
					corralID = base.gameObject.GetComponentInParent<IdHandler>().id,
					currentFrame = Time.frameCount,
					missingSlimes = string.Join(", ", (from id in this.wasRemoved
					select new
					{
						instanceID = id,
						metadata = delegate
						{
							Destroyer.Metadata metadata;
							if (this.metadataDict.TryGetValue(id, out metadata))
							{
								return metadata.ToString();
							}
							return "null";
						}(),
						gameObject = delegate
						{
							foreach (object obj in SRSingleton<DynamicObjectContainer>.Instance.transform)
							{
								Transform transform = (Transform)obj;
								if (transform.gameObject.GetInstanceID() == id)
								{
									return new
									{
										transform.gameObject.name,
										transform.gameObject.transform.position,
										transform.gameObject.GetComponent<Rigidbody>().velocity,
										transform.gameObject.GetComponent<Rigidbody>().angularVelocity
									}.ToString();
								}
							}
							return "null";
						}()
					}.ToString()).ToArray<string>())
				}.ToString(), Array.Empty<object>());
			}
			this.nextTrackFrameCount += 3;
			foreach (KeyValuePair<int, Destroyer.Metadata> item in this.metadataDict)
			{
				if (Time.frameCount - item.Value.frame > 3)
				{
					this.local_ToRemoveFromDict.Add(item);
				}
			}
			foreach (KeyValuePair<int, Destroyer.Metadata> keyValuePair in this.local_ToRemoveFromDict)
			{
				this.metadataDict.Remove(keyValuePair.Key);
			}
			this.local_ToRemoveFromDict.Clear();
			this.wasRemoved.Clear();
		}
	}

	// Token: 0x040018F6 RID: 6390
	[Tooltip("List of AirNet components to be checked during the slime integrity tracker.")]
	public List<AirNet> airNets;

	// Token: 0x040018F7 RID: 6391
	private Dictionary<Identifiable.Id, HashSet<Identifiable>> trackedObjects = new Dictionary<Identifiable.Id, HashSet<Identifiable>>(Identifiable.idComparer);

	// Token: 0x040018F8 RID: 6392
	private HashSet<Identifiable.Id> uniqueIdentifiableTypes = new HashSet<Identifiable.Id>(Identifiable.idComparer);

	// Token: 0x040018F9 RID: 6393
	public TrackContainedIdentifiables.IdentifiableEntered OnIdentifiableEntered;

	// Token: 0x040018FA RID: 6394
	public TrackContainedIdentifiables.NewIdentifiableTypeEntered OnNewIdentifiableTypeEntered;

	// Token: 0x040018FB RID: 6395
	private Dictionary<int, Destroyer.Metadata> metadataDict = new Dictionary<int, Destroyer.Metadata>();

	// Token: 0x040018FC RID: 6396
	private List<int> wasRemoved = new List<int>();

	// Token: 0x040018FD RID: 6397
	private int nextTrackFrameCount;

	// Token: 0x040018FE RID: 6398
	private List<KeyValuePair<int, Destroyer.Metadata>> local_ToRemoveFromDict = new List<KeyValuePair<int, Destroyer.Metadata>>(64);

	// Token: 0x020004B4 RID: 1204
	// (Invoke) Token: 0x0600194A RID: 6474
	public delegate void IdentifiableEntered(TrackContainedIdentifiables container, Identifiable ident);

	// Token: 0x020004B5 RID: 1205
	// (Invoke) Token: 0x0600194E RID: 6478
	public delegate void NewIdentifiableTypeEntered(TrackContainedIdentifiables container, Identifiable ident);
}
