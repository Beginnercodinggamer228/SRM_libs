using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001CE RID: 462
public class DroneUIProgram : SRBehaviour
{
	// Token: 0x060009CD RID: 2509 RVA: 0x0002B59C File Offset: 0x0002979C
	public DroneUIProgram Init(DroneMetadata.Program program, int? index)
	{
		this.buttonTarget.Init(program.target, new DroneUIProgramButton.Title
		{
			type = DroneUIProgramButton.Title.Type.TARGET,
			index = index
		});
		this.buttonSource.Init(program.source, new DroneUIProgramButton.Title
		{
			type = DroneUIProgramButton.Title.Type.SOURCE,
			index = index
		});
		this.buttonDestination.Init(program.destination, new DroneUIProgramButton.Title
		{
			type = DroneUIProgramButton.Title.Type.DESTINATION,
			index = index
		});
		return this;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0002B61C File Offset: 0x0002981C
	public void LinkGamepadNav(DroneUIProgram down)
	{
		this.LinkDefaultNavigation();
		SRBehaviour.LinkNavigation(this.buttonTarget.button, down.GetButtonOrRightmost(down.buttonTarget.button), SRBehaviour.NavigationDirection.DOWN);
		SRBehaviour.LinkNavigation(this.buttonSource.button, down.GetButtonOrRightmost(down.buttonSource.button), SRBehaviour.NavigationDirection.DOWN);
		SRBehaviour.LinkNavigation(this.buttonDestination.button, down.GetButtonOrRightmost(down.buttonDestination.button), SRBehaviour.NavigationDirection.DOWN);
		SRBehaviour.LinkNavigation(down.buttonTarget.button, this.GetButtonOrRightmost(this.buttonTarget.button), SRBehaviour.NavigationDirection.UP);
		SRBehaviour.LinkNavigation(down.buttonSource.button, this.GetButtonOrRightmost(this.buttonSource.button), SRBehaviour.NavigationDirection.UP);
		SRBehaviour.LinkNavigation(down.buttonDestination.button, this.GetButtonOrRightmost(this.buttonDestination.button), SRBehaviour.NavigationDirection.UP);
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0002B6FB File Offset: 0x000298FB
	public void LinkGamepadNav(Selectable down)
	{
		this.LinkDefaultNavigation();
		SRBehaviour.LinkNavigation(this.buttonTarget.button, down, SRBehaviour.NavigationDirection.DOWN_UP);
		SRBehaviour.LinkNavigation(this.buttonSource.button, down, SRBehaviour.NavigationDirection.DOWN);
		SRBehaviour.LinkNavigation(this.buttonDestination.button, down, SRBehaviour.NavigationDirection.DOWN);
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0002B739 File Offset: 0x00029939
	private void LinkDefaultNavigation()
	{
		SRBehaviour.LinkNavigation(this.buttonTarget.button, this.buttonSource.button, SRBehaviour.NavigationDirection.RIGHT_LEFT);
		SRBehaviour.LinkNavigation(this.buttonSource.button, this.buttonDestination.button, SRBehaviour.NavigationDirection.RIGHT_LEFT);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0002B773 File Offset: 0x00029973
	private Selectable GetButtonOrRightmost(Selectable selectable)
	{
		if (selectable.interactable)
		{
			return selectable;
		}
		if (selectable == this.buttonDestination.button)
		{
			return this.GetButtonOrRightmost(this.buttonSource.button);
		}
		return this.buttonTarget.button;
	}

	// Token: 0x0400081D RID: 2077
	[Tooltip("Drone program button: target")]
	public DroneUIProgramButton buttonTarget;

	// Token: 0x0400081E RID: 2078
	[Tooltip("Drone program button: source")]
	public DroneUIProgramButton buttonSource;

	// Token: 0x0400081F RID: 2079
	[Tooltip("Drone program button: destination")]
	public DroneUIProgramButton buttonDestination;
}
