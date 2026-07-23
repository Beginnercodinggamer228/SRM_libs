using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020004CE RID: 1230
public class SlimeLineupUI : MonoBehaviour
{
	// Token: 0x060019BA RID: 6586 RVA: 0x000648E0 File Offset: 0x00062AE0
	private void Start()
	{
		this.baseSlimeTypes = (from slime in this.slimeLineup.slimeDefinitions.Slimes
		where !slime.IsLargo
		select slime).ToArray<SlimeDefinition>();
		this.dropdown.AddOptions((from slime in this.baseSlimeTypes
		select slime.Name).ToList<string>());
		this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnSlimeTypeSelected));
		this.showSlimeButton.onClick.AddListener(new UnityAction(this.ShowSelectedSlime));
		this.showSlimesAndLargosButton.onClick.AddListener(new UnityAction(this.ShowSelectedSlimeAndLargos));
	}

	// Token: 0x060019BB RID: 6587 RVA: 0x000649BA File Offset: 0x00062BBA
	public void OnSlimeTypeSelected(int index)
	{
		this.selectedIndex = index;
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x000649C3 File Offset: 0x00062BC3
	public void ShowSelectedSlime()
	{
		this.slimeLineup.ShowSlime(this.baseSlimeTypes[this.selectedIndex]);
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x000649DD File Offset: 0x00062BDD
	public void ShowSelectedSlimeAndLargos()
	{
		this.slimeLineup.ShowSlimeAndLargos(this.baseSlimeTypes[this.selectedIndex]);
	}

	// Token: 0x0400195A RID: 6490
	public Dropdown dropdown;

	// Token: 0x0400195B RID: 6491
	public SlimeLineup slimeLineup;

	// Token: 0x0400195C RID: 6492
	public Button showSlimeButton;

	// Token: 0x0400195D RID: 6493
	public Button showSlimesAndLargosButton;

	// Token: 0x0400195E RID: 6494
	private SlimeDefinition[] baseSlimeTypes;

	// Token: 0x0400195F RID: 6495
	private int selectedIndex;
}
