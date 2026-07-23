using System;
using System.Collections.Generic;
using System.Linq;
using DLCPackage;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020004D1 RID: 1233
public class SlimePreviewUI : MonoBehaviour
{
	// Token: 0x060019C6 RID: 6598 RVA: 0x00064BB2 File Offset: 0x00062DB2
	public void Awake()
	{
		this.DLCDirector = SRSingleton<GameContext>.Instance.DLCDirector;
		this.DLCDirector.onPackageInstalled += this.OnDLCPackageInstalled;
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x00064BDB File Offset: 0x00062DDB
	public void OnDestroy()
	{
		if (this.DLCDirector != null)
		{
			this.DLCDirector.onPackageInstalled -= this.OnDLCPackageInstalled;
			this.DLCDirector = null;
		}
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x00064C04 File Offset: 0x00062E04
	private void Start()
	{
		this.typeDropdown.ClearOptions();
		this.appearanceDropdown.ClearOptions();
		this.typeDropdown.AddOptions((from slime in this.slimeDefinitions.Slimes
		select slime.Name).ToList<string>());
		this.typeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnTypeSelected));
		this.appearanceDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnAppearanceSelected));
		this.refreshButton.onClick.AddListener(new UnityAction(this.RefreshAppearance));
		this.lookAtButton.onClick.AddListener(delegate()
		{
			this.slimeCam.ResetCamToTarget(this.slimeAppearanceApplicator.transform);
		});
		this.groundedToggle.onValueChanged.AddListener(delegate(bool value)
		{
			this.RefreshAppearance();
		});
		this.OnTypeSelected(0);
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x00064CFC File Offset: 0x00062EFC
	private void OnTypeSelected(int index)
	{
		this.currentSlimeDefinition = this.slimeDefinitions.Slimes[index];
		this.currentSlimeAppearances.Clear();
		this.currentSlimeAppearances.AddRange(from appearance in this.currentSlimeDefinition.Appearances.SelectMany((SlimeAppearance appearance) => new SlimeAppearance[]
		{
			appearance,
			appearance.QubitAppearance,
			appearance.ShockedAppearance
		})
		where appearance != null
		select appearance);
		this.appearanceDropdown.ClearOptions();
		this.appearanceDropdown.AddOptions((from appearance in this.currentSlimeAppearances
		select appearance.name).ToList<string>());
		this.OnAppearanceSelected(0);
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x00064DD1 File Offset: 0x00062FD1
	private void OnAppearanceSelected(int index)
	{
		this.currentAppearance = this.currentSlimeAppearances[index];
		this.RefreshAppearance();
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x00064DEC File Offset: 0x00062FEC
	private void RefreshAppearance()
	{
		this.slimeAppearanceApplicator.SlimeDefinition = this.currentSlimeDefinition;
		this.slimeAppearanceApplicator.Appearance = this.currentAppearance;
		this.slimeAppearanceApplicator.ApplyAppearance();
		this.slimeAppearanceApplicator.transform.localScale = new Vector3(this.currentSlimeDefinition.PrefabScale, this.currentSlimeDefinition.PrefabScale, this.currentSlimeDefinition.PrefabScale);
		EnableBasedOnGrounded[] componentsInChildren = this.slimeAppearanceApplicator.GetComponentsInChildren<EnableBasedOnGrounded>();
		this.groundedToggle.gameObject.SetActive(componentsInChildren.Length != 0);
		foreach (EnableBasedOnGrounded enableBasedOnGrounded in componentsInChildren)
		{
			enableBasedOnGrounded.gameObject.SetActive(enableBasedOnGrounded.enableOnGrounded ^ this.groundedToggle.isOn);
		}
		DeactivateOnHeld[] componentsInChildren2 = this.slimeAppearanceApplicator.GetComponentsInChildren<DeactivateOnHeld>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = false;
		}
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x00064ED4 File Offset: 0x000630D4
	private void OnDLCPackageInstalled(Id package)
	{
		if (package == Id.SECRET_STYLE)
		{
			this.OnTypeSelected(0);
		}
	}

	// Token: 0x0400196C RID: 6508
	public SlimeDefinitions slimeDefinitions;

	// Token: 0x0400196D RID: 6509
	public SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x0400196E RID: 6510
	public SlimePreviewCamera slimeCam;

	// Token: 0x0400196F RID: 6511
	public Toggle groundedToggle;

	// Token: 0x04001970 RID: 6512
	public Dropdown typeDropdown;

	// Token: 0x04001971 RID: 6513
	public Dropdown appearanceDropdown;

	// Token: 0x04001972 RID: 6514
	public Button refreshButton;

	// Token: 0x04001973 RID: 6515
	public Button lookAtButton;

	// Token: 0x04001974 RID: 6516
	private SlimeDefinition currentSlimeDefinition;

	// Token: 0x04001975 RID: 6517
	private SlimeAppearance currentAppearance;

	// Token: 0x04001976 RID: 6518
	private readonly List<SlimeAppearance> currentSlimeAppearances = new List<SlimeAppearance>();

	// Token: 0x04001977 RID: 6519
	private DLCDirector DLCDirector;
}
