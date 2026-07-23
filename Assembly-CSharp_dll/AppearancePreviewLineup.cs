using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004C4 RID: 1220
public class AppearancePreviewLineup : MonoBehaviour
{
	// Token: 0x0600198D RID: 6541 RVA: 0x0006350C File Offset: 0x0006170C
	private void Start()
	{
		foreach (SlimeAppearanceApplicator slimeAppearanceApplicator in this.currentAppearancePreviews)
		{
			if (slimeAppearanceApplicator != null)
			{
				slimeAppearanceApplicator.ApplyAppearance();
			}
		}
		this.selectedSlimeDefinition.options.Clear();
		this.baseSlimes = (from slime in this.slimeDefinitions.Slimes
		where !slime.IsLargo
		select slime).ToList<SlimeDefinition>();
		this.baseSlimeCount = this.baseSlimes.Count;
		if (this.extraAppearances.Count > 0)
		{
			this.extraOffset = 1;
			this.selectedSlimeDefinition.options.Add(new Dropdown.OptionData("Extra Appearances"));
		}
		foreach (SlimeDefinition slimeDefinition in this.baseSlimes)
		{
			this.selectedSlimeDefinition.options.Add(new Dropdown.OptionData("All " + slimeDefinition.name));
		}
		foreach (SlimeDefinition slimeDefinition2 in this.slimeDefinitions.Slimes)
		{
			this.selectedSlimeDefinition.options.Add(new Dropdown.OptionData(slimeDefinition2.name));
		}
		this.selectedSlimeDefinition.value = 0;
		this.selectedSlimeDefinition.RefreshShownValue();
		this.OnDropdownValueChanged(0);
		this.ToggleControls();
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x000636B8 File Offset: 0x000618B8
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			this.ToggleControls();
		}
		if (this.menuControlsEnabled)
		{
			bool wasPressed = SRInput.PauseActions.menuDown.WasPressed;
			bool wasPressed2 = SRInput.PauseActions.menuUp.WasPressed;
			bool wasPressed3 = SRInput.PauseActions.menuLeft.WasPressed;
			bool wasPressed4 = SRInput.PauseActions.menuRight.WasPressed;
			if (wasPressed2 && this.selectedSlimeDefinition.value > 0)
			{
				Dropdown dropdown = this.selectedSlimeDefinition;
				int value = dropdown.value;
				dropdown.value = value - 1;
			}
			else if (wasPressed && this.selectedSlimeDefinition.value < this.selectedSlimeDefinition.options.Count)
			{
				Dropdown dropdown2 = this.selectedSlimeDefinition;
				int value = dropdown2.value;
				dropdown2.value = value + 1;
			}
			if (wasPressed3)
			{
				this.MoveCamera(-1);
			}
			else if (wasPressed4)
			{
				this.MoveCamera(1);
			}
		}
		if (this.targetCloakOpacity > this.currentCloakOpacity)
		{
			this.currentCloakOpacity = Mathf.Min(this.targetCloakOpacity, this.currentCloakOpacity + 2f * Time.deltaTime);
		}
		else if (this.targetCloakOpacity < this.currentCloakOpacity)
		{
			this.currentCloakOpacity = Mathf.Max(this.targetCloakOpacity, this.currentCloakOpacity - 2f * Time.deltaTime);
		}
		this.ApplyCloak();
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x000637FC File Offset: 0x000619FC
	private void ToggleControls()
	{
		this.slimePreviewCamera.zoomControlsEnabled = !this.slimePreviewCamera.zoomControlsEnabled;
		this.menuControlsEnabled = !this.slimePreviewCamera.zoomControlsEnabled;
		this.controlStateText.text = (this.menuControlsEnabled ? "Menu Controls Enabled" : "Camera Controls Enabled") + " (Tab to change)";
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x00063860 File Offset: 0x00061A60
	public void OnDropdownValueChanged(int index)
	{
		if (index < this.extraOffset)
		{
			List<SlimeDefinition> slimesToShow = this.extraAppearances.Select(delegate(SlimeAppearance appearance)
			{
				SlimeDefinition slimeDefinition = ScriptableObject.CreateInstance<SlimeDefinition>();
				slimeDefinition.PrefabScale = ((appearance.DependentAppearances.Length != 0) ? 2f : 1f);
				slimeDefinition.AppearancesDefault = new SlimeAppearance[]
				{
					appearance
				};
				return slimeDefinition;
			}).ToList<SlimeDefinition>();
			this.ShowAppearances(slimesToShow);
			return;
		}
		if (index < this.baseSlimeCount)
		{
			SlimeDefinition baseType = this.baseSlimes[index - this.extraOffset];
			List<SlimeDefinition> list = new List<SlimeDefinition>
			{
				baseType
			};
			list.AddRange((from slime in this.slimeDefinitions.Slimes
			where slime.BaseSlimes.Contains(baseType)
			select slime).ToList<SlimeDefinition>());
			this.ShowAppearances(list);
			return;
		}
		this.ShowAppearances(new List<SlimeDefinition>
		{
			this.slimeDefinitions.Slimes[index - this.baseSlimeCount - this.extraOffset]
		});
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x00063943 File Offset: 0x00061B43
	public void ShowAppearances(List<SlimeDefinition> slimesToShow)
	{
		this.currentDefinitions.Clear();
		this.currentDefinitions.AddRange(slimesToShow);
		this.currentFocusedIndex = 0;
		this.Refresh();
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x0006396C File Offset: 0x00061B6C
	public void Refresh()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(base.gameObject.transform.GetChild(i).gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject.transform.GetChild(i).gameObject);
			}
		}
		this.qubitPreviews.Clear();
		this.currentAppearancePreviews.Clear();
		int index = 0;
		using (List<SlimeDefinition>.Enumerator enumerator = this.currentDefinitions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SlimeDefinition definition = enumerator.Current;
				this.currentAppearancePreviews.AddRange(definition.Appearances.Select(delegate(SlimeAppearance appearance)
				{
					int index;
					SlimeAppearanceApplicator slimeAppearanceApplicator2 = this.CreateAndShowAppearance(this.appearancePreviewPrefab, appearance, definition, index, -0.5f);
					if (index == 0)
					{
						this.PopulateAnimationDropdown(slimeAppearanceApplicator2.GetComponentInChildren<Animator>());
					}
					index = index;
					index++;
					return slimeAppearanceApplicator2;
				}));
			}
		}
		this.stealthControllers.Clear();
		foreach (SlimeAppearanceApplicator slimeAppearanceApplicator in this.currentAppearancePreviews)
		{
			MaterialStealthController item = new MaterialStealthController(slimeAppearanceApplicator.gameObject);
			this.stealthControllers.Add(item);
		}
		if (this.currentFocusedIndex < this.currentAppearancePreviews.Count)
		{
			this.LookAtIndex(this.currentFocusedIndex);
		}
		else
		{
			this.LookAtIndex(0);
		}
		this.ApplyCloak();
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x00063B00 File Offset: 0x00061D00
	private SlimeAppearanceApplicator CreateAndShowAppearance(SlimeAppearanceApplicator prefab, SlimeAppearance appearance, SlimeDefinition definition, int index, float yOffset = -0.5f)
	{
		SlimeAppearanceApplicator slimeAppearanceApplicator = LineupUtils.GenerateAppearancePreview(this.appearancePreviewPrefab, definition, appearance);
		slimeAppearanceApplicator.transform.parent = base.transform;
		slimeAppearanceApplicator.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		slimeAppearanceApplicator.transform.localPosition = new Vector3((float)index * this.xSpacing, yOffset + definition.PrefabScale / 2f, 0f);
		if (this.qubitModeEnabled && appearance.QubitAppearance != null)
		{
			this.qubitPreviews.Add(this.CreateAndShowAppearance(this.qubitPreviewPrefab, appearance.QubitAppearance, definition, index, this.qubitYSpacing));
		}
		return slimeAppearanceApplicator;
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x00063BB4 File Offset: 0x00061DB4
	private void PopulateAnimationDropdown(Animator animator)
	{
		this.animationParamDropdown.ClearOptions();
		List<string> list = new List<string>();
		foreach (AnimatorControllerParameter animatorControllerParameter in animator.parameters)
		{
			if (animatorControllerParameter.type == AnimatorControllerParameterType.Bool)
			{
				list.Add(animatorControllerParameter.name);
			}
		}
		this.animationParamDropdown.AddOptions(list);
		this.UpdateCurrentAnimationValueText();
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x00063C12 File Offset: 0x00061E12
	public void MoveCamera(int direction)
	{
		this.LookAtIndex(this.currentFocusedIndex + direction);
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x00063C22 File Offset: 0x00061E22
	public void SetQubitMode(bool showQubits)
	{
		this.qubitModeEnabled = showQubits;
		this.Refresh();
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x00063C31 File Offset: 0x00061E31
	public void SetCloakedMode(bool cloak)
	{
		this.targetCloakOpacity = (cloak ? 0f : 1f);
		this.ApplyCloak();
	}

	// Token: 0x06001998 RID: 6552 RVA: 0x00063C50 File Offset: 0x00061E50
	private void ApplyCloak()
	{
		foreach (MaterialStealthController materialStealthController in this.stealthControllers)
		{
			materialStealthController.SetOpacity(this.currentCloakOpacity);
		}
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x00063CA8 File Offset: 0x00061EA8
	public void ToggleAnimationBool()
	{
		string text = this.animationParamDropdown.options[this.animationParamDropdown.value].text;
		if (!string.IsNullOrEmpty(text))
		{
			foreach (SlimeAppearanceApplicator slimeAppearanceApplicator in this.currentAppearancePreviews)
			{
				Animator componentInChildren = slimeAppearanceApplicator.GetComponentInChildren<Animator>();
				componentInChildren.SetBool(text, !componentInChildren.GetBool(text));
			}
			this.UpdateCurrentAnimationValueText();
		}
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x00063D3C File Offset: 0x00061F3C
	public void UpdateCurrentAnimationValueText()
	{
		string text = this.animationParamDropdown.options[this.animationParamDropdown.value].text;
		if (this.currentAppearancePreviews.Count > 0)
		{
			Animator componentInChildren = this.currentAppearancePreviews[0].GetComponentInChildren<Animator>();
			this.animationValueText.text = text + ": " + componentInChildren.GetBool(text).ToString();
			return;
		}
		this.animationValueText.text = "";
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x00063DC0 File Offset: 0x00061FC0
	private void LookAtIndex(int previewIndex)
	{
		if (previewIndex >= 0 && previewIndex < this.currentAppearancePreviews.Count)
		{
			this.currentFocusedIndex = previewIndex;
			this.slimePreviewCamera.ResetCamToTarget(this.currentAppearancePreviews[this.currentFocusedIndex].transform);
			this.slimeNameText.text = this.currentAppearancePreviews[this.currentFocusedIndex].Appearance.name;
		}
	}

	// Token: 0x04001922 RID: 6434
	[Header("Prefabs")]
	[Tooltip("SlimeAppearanceApplicator prefab for slime appearances.")]
	public SlimeAppearanceApplicator appearancePreviewPrefab;

	// Token: 0x04001923 RID: 6435
	[Tooltip("SlimeAppearanceApplicator prefab for qubit appearances.")]
	public SlimeAppearanceApplicator qubitPreviewPrefab;

	// Token: 0x04001924 RID: 6436
	[Header("Camera")]
	public SlimePreviewCamera slimePreviewCamera;

	// Token: 0x04001925 RID: 6437
	[Header("Slime Definitions")]
	public SlimeDefinitions slimeDefinitions;

	// Token: 0x04001926 RID: 6438
	[Header("UI")]
	public Text slimeNameText;

	// Token: 0x04001927 RID: 6439
	public Text controlStateText;

	// Token: 0x04001928 RID: 6440
	public Dropdown selectedSlimeDefinition;

	// Token: 0x04001929 RID: 6441
	public Dropdown animationParamDropdown;

	// Token: 0x0400192A RID: 6442
	public Text animationValueText;

	// Token: 0x0400192B RID: 6443
	[Header("Spacing")]
	public float xSpacing = 2f;

	// Token: 0x0400192C RID: 6444
	public float qubitYSpacing = 3f;

	// Token: 0x0400192D RID: 6445
	[Header("Extra Appearances")]
	[Tooltip("Extra appearances to show in the preview.")]
	public List<SlimeAppearance> extraAppearances = new List<SlimeAppearance>();

	// Token: 0x0400192E RID: 6446
	private int currentFocusedIndex;

	// Token: 0x0400192F RID: 6447
	private List<SlimeAppearanceApplicator> currentAppearancePreviews = new List<SlimeAppearanceApplicator>();

	// Token: 0x04001930 RID: 6448
	private List<SlimeDefinition> currentDefinitions = new List<SlimeDefinition>();

	// Token: 0x04001931 RID: 6449
	private List<SlimeAppearanceApplicator> qubitPreviews = new List<SlimeAppearanceApplicator>();

	// Token: 0x04001932 RID: 6450
	private List<MaterialStealthController> stealthControllers = new List<MaterialStealthController>();

	// Token: 0x04001933 RID: 6451
	private bool qubitModeEnabled;

	// Token: 0x04001934 RID: 6452
	private bool menuControlsEnabled;

	// Token: 0x04001935 RID: 6453
	private int extraOffset;

	// Token: 0x04001936 RID: 6454
	private int baseSlimeCount;

	// Token: 0x04001937 RID: 6455
	private List<SlimeDefinition> baseSlimes;

	// Token: 0x04001938 RID: 6456
	private float currentCloakOpacity = 1f;

	// Token: 0x04001939 RID: 6457
	private float targetCloakOpacity = 1f;

	// Token: 0x0400193A RID: 6458
	private const float OPACITY_CHANGE_PER_SEC = 2f;
}
