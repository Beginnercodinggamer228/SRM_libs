using System;
using System.Collections.Generic;
using UnityEngine.UI;

// Token: 0x02000797 RID: 1943
public class TeleporterGadget : TeleportDestination, Gadget.LinkDestroyer
{
	// Token: 0x06002889 RID: 10377 RVA: 0x000999D8 File Offset: 0x00097BD8
	public override void Awake()
	{
		TeleportSource component = base.GetComponent<TeleportSource>();
		component.waitForExternalActivation = true;
		this.destinationIcon.enabled = false;
		List<TeleportDestination> destinations = SRSingleton<SceneContext>.Instance.TeleportNetwork.GetDestinations(this.linkName);
		if (destinations.Count == 1)
		{
			this.linked = (TeleporterGadget)destinations[0];
			this.linked.linked = this;
			this.teleportDestinationName = string.Format("{0}_{1}", this.linkName, "linked");
			component.destinationSetName = this.linked.teleportDestinationName;
			component.waitForExternalActivation = false;
			TeleportSource component2 = this.linked.GetComponent<TeleportSource>();
			component2.destinationSetName = this.teleportDestinationName;
			component2.waitForExternalActivation = false;
		}
		else
		{
			this.teleportDestinationName = this.linkName;
		}
		base.Awake();
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x00099AA0 File Offset: 0x00097CA0
	public void Start()
	{
		if (this.linked != null)
		{
			this.destinationIcon.sprite = ZoneDirector.GetZoneIcon(this.linked.gameObject);
			this.destinationIcon.enabled = (this.destinationIcon.sprite != null);
			this.linked.destinationIcon.sprite = ZoneDirector.GetZoneIcon(base.gameObject);
			this.linked.destinationIcon.enabled = (this.linked.destinationIcon.sprite != null);
		}
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x00099B33 File Offset: 0x00097D33
	public bool ShouldDestroyPair()
	{
		return this.linked != null;
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x00099B41 File Offset: 0x00097D41
	public Gadget.LinkDestroyer GetLinked()
	{
		return this.linked;
	}

	// Token: 0x04002828 RID: 10280
	public Image destinationIcon;

	// Token: 0x04002829 RID: 10281
	public string linkName;

	// Token: 0x0400282A RID: 10282
	private TeleporterGadget linked;
}
