using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public class TeleportNetwork : MonoBehaviour
{
	// Token: 0x06001B6C RID: 7020 RVA: 0x00069133 File Offset: 0x00067333
	public void Register(TeleportDestination exit)
	{
		this.GetOrCreateDestinationSet(exit.teleportDestinationName).exits.Add(exit);
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x0006914C File Offset: 0x0006734C
	public void Deregister(TeleportDestination exit)
	{
		TeleportNetwork.Destination destination;
		if (!this.destinationLookup.TryGetValue(exit.teleportDestinationName, out destination))
		{
			Log.Warning("Tried to remove a teleport exit from a non-existent destination.", new object[]
			{
				"exit.teleportDestinationName",
				exit.teleportDestinationName
			});
			return;
		}
		destination.exits.Remove(exit);
		if (destination.exits.Count == 0)
		{
			this.destinationLookup.Remove(exit.teleportDestinationName);
		}
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x000691BC File Offset: 0x000673BC
	public List<TeleportDestination> GetDestinations(string destinationName)
	{
		if (this.destinationLookup.ContainsKey(destinationName))
		{
			return this.destinationLookup[destinationName].exits;
		}
		return new List<TeleportDestination>();
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x000691E4 File Offset: 0x000673E4
	private TeleportNetwork.Destination GetOrCreateDestinationSet(string destinationName)
	{
		TeleportNetwork.Destination destination;
		if (!this.destinationLookup.TryGetValue(destinationName, out destination))
		{
			destination = default(TeleportNetwork.Destination);
			destination.name = destinationName;
			destination.exits = new List<TeleportDestination>();
			this.destinationLookup.Add(destinationName, destination);
		}
		return destination;
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x0006922C File Offset: 0x0006742C
	public bool TeleportToDestination(TeleportablePlayer toTeleport, TeleportSource source, string destinationName, Func<List<TeleportDestination>, TeleportDestination> pickFunction)
	{
		List<TeleportDestination> list = new List<TeleportDestination>(this.GetDestinations(source.destinationSetName));
		list.RemoveAll((TeleportDestination destination) => !destination.IsLinkActive());
		if (!list.Any<TeleportDestination>())
		{
			return false;
		}
		TeleportDestination teleportDestination = pickFunction(list);
		if (teleportDestination == null)
		{
			return false;
		}
		this.TeleportToDestination(toTeleport, source, teleportDestination);
		return true;
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x0006929C File Offset: 0x0006749C
	private void TeleportToDestination(TeleportablePlayer toTeleport, TeleportSource source, TeleportDestination destination)
	{
		source.OnDepart();
		destination.OnDepart();
		Vector3 position = destination.GetPosition();
		Vector3? eulerAngles = destination.GetEulerAngles();
		toTeleport.TeleportTo(position, destination.regionSetId, eulerAngles, true, true);
		destination.OnArrive();
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x000692D7 File Offset: 0x000674D7
	public void OnDestroy()
	{
		this.destinationLookup.Clear();
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000692E4 File Offset: 0x000674E4
	public bool IsLinkFullyActive(TeleportSource source)
	{
		if (source.IsLinkActive())
		{
			using (List<TeleportDestination>.Enumerator enumerator = this.GetDestinations(source.destinationSetName).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsLinkActive())
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x04001AB2 RID: 6834
	private Dictionary<string, TeleportNetwork.Destination> destinationLookup = new Dictionary<string, TeleportNetwork.Destination>();

	// Token: 0x02000522 RID: 1314
	[Serializable]
	private struct Destination
	{
		// Token: 0x04001AB3 RID: 6835
		public string name;

		// Token: 0x04001AB4 RID: 6836
		public List<TeleportDestination> exits;
	}
}
