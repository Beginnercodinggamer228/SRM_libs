using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200064F RID: 1615
[ExecuteInEditMode]
public class UIStyleDirector : SRBehaviour
{
	// Token: 0x060021C2 RID: 8642 RVA: 0x00082E3F File Offset: 0x0008103F
	private UIStyleDirector.MeshTextStyle ConvertToMesh(UIStyleDirector.TextStyle style)
	{
		UIStyleDirector.MeshTextStyle meshTextStyle = new UIStyleDirector.MeshTextStyle();
		meshTextStyle.Convert(this, style);
		return meshTextStyle;
	}

	// Token: 0x060021C3 RID: 8643 RVA: 0x00082E4E File Offset: 0x0008104E
	private UIStyleDirector.MeshButtonStyle ConvertToMesh(UIStyleDirector.ButtonStyle style)
	{
		UIStyleDirector.MeshButtonStyle meshButtonStyle = new UIStyleDirector.MeshButtonStyle();
		meshButtonStyle.Convert(this, style);
		return meshButtonStyle;
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x00082E5D File Offset: 0x0008105D
	private UIStyleDirector.MeshDropdownStyle ConvertToMesh(UIStyleDirector.DropdownStyle style)
	{
		UIStyleDirector.MeshDropdownStyle meshDropdownStyle = new UIStyleDirector.MeshDropdownStyle();
		meshDropdownStyle.Convert(this, style);
		return meshDropdownStyle;
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x00082E6C File Offset: 0x0008106C
	private UIStyleDirector.MeshCheckboxStyle ConvertToMesh(UIStyleDirector.CheckboxStyle style)
	{
		UIStyleDirector.MeshCheckboxStyle meshCheckboxStyle = new UIStyleDirector.MeshCheckboxStyle();
		meshCheckboxStyle.Convert(this, style);
		return meshCheckboxStyle;
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x00082E7B File Offset: 0x0008107B
	private UIStyleDirector.MeshToggleButtonStyle ConvertToMesh(UIStyleDirector.ToggleButtonStyle style)
	{
		UIStyleDirector.MeshToggleButtonStyle meshToggleButtonStyle = new UIStyleDirector.MeshToggleButtonStyle();
		meshToggleButtonStyle.Convert(this, style);
		return meshToggleButtonStyle;
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x00082E8C File Offset: 0x0008108C
	public void OnEnable()
	{
		List<string> list = new List<string>();
		foreach (UIStyleDirector.TextStyle textStyle in this.textStyles)
		{
			this.textDict[textStyle.name] = textStyle;
			list.Add(textStyle.name);
		}
		this.textStyleNames = list.ToArray();
		List<string> list2 = new List<string>();
		foreach (UIStyleDirector.MeshTextStyle meshTextStyle in this.meshTextStyles)
		{
			this.meshTextDict[meshTextStyle.name] = meshTextStyle;
			list2.Add(meshTextStyle.name);
		}
		this.meshTextStyleNames = list2.ToArray();
		List<string> list3 = new List<string>();
		foreach (UIStyleDirector.ButtonStyle buttonStyle in this.buttonStyles)
		{
			this.buttonDict[buttonStyle.name] = buttonStyle;
			list3.Add(buttonStyle.name);
		}
		this.buttonStyleNames = list3.ToArray();
		List<string> list4 = new List<string>();
		foreach (UIStyleDirector.MeshButtonStyle meshButtonStyle in this.meshButtonStyles)
		{
			this.meshButtonDict[meshButtonStyle.name] = meshButtonStyle;
			list4.Add(meshButtonStyle.name);
		}
		this.meshButtonStyleNames = list4.ToArray();
		List<string> list5 = new List<string>();
		foreach (UIStyleDirector.DropdownStyle dropdownStyle in this.dropdownStyles)
		{
			this.dropdownDict[dropdownStyle.name] = dropdownStyle;
			list5.Add(dropdownStyle.name);
		}
		this.dropdownStyleNames = list5.ToArray();
		List<string> list6 = new List<string>();
		foreach (UIStyleDirector.MeshDropdownStyle meshDropdownStyle in this.meshDropdownStyles)
		{
			this.meshDropdownDict[meshDropdownStyle.name] = meshDropdownStyle;
			list6.Add(meshDropdownStyle.name);
		}
		this.meshDropdownStyleNames = list6.ToArray();
		List<string> list7 = new List<string>();
		foreach (UIStyleDirector.PanelStyle panelStyle in this.panelStyles)
		{
			this.panelDict[panelStyle.name] = panelStyle;
			list7.Add(panelStyle.name);
		}
		this.panelStyleNames = list7.ToArray();
		List<string> list8 = new List<string>();
		foreach (UIStyleDirector.IconStyle iconStyle in this.iconStyles)
		{
			this.iconDict[iconStyle.name] = iconStyle;
			list8.Add(iconStyle.name);
		}
		this.iconStyleNames = list8.ToArray();
		List<string> list9 = new List<string>();
		foreach (UIStyleDirector.FieldStyle fieldStyle in this.fieldStyles)
		{
			this.fieldDict[fieldStyle.name] = fieldStyle;
			list9.Add(fieldStyle.name);
		}
		this.fieldStyleNames = list9.ToArray();
		List<string> list10 = new List<string>();
		foreach (UIStyleDirector.ScrollbarStyle scrollbarStyle in this.scrollbarStyles)
		{
			this.scrollbarDict[scrollbarStyle.name] = scrollbarStyle;
			list10.Add(scrollbarStyle.name);
		}
		this.scrollbarStyleNames = list10.ToArray();
		List<string> list11 = new List<string>();
		foreach (UIStyleDirector.CheckboxStyle checkboxStyle in this.checkboxStyles)
		{
			this.checkboxDict[checkboxStyle.name] = checkboxStyle;
			list11.Add(checkboxStyle.name);
		}
		this.checkboxStyleNames = list11.ToArray();
		List<string> list12 = new List<string>();
		foreach (UIStyleDirector.MeshCheckboxStyle meshCheckboxStyle in this.meshCheckboxStyles)
		{
			this.meshCheckboxDict[meshCheckboxStyle.name] = meshCheckboxStyle;
			list12.Add(meshCheckboxStyle.name);
		}
		this.meshCheckboxStyleNames = list12.ToArray();
		List<string> list13 = new List<string>();
		foreach (UIStyleDirector.ToggleButtonStyle toggleButtonStyle in this.toggleButtonStyles)
		{
			this.toggleButtonDict[toggleButtonStyle.name] = toggleButtonStyle;
			list13.Add(toggleButtonStyle.name);
		}
		this.toggleButtonStyleNames = list13.ToArray();
		List<string> list14 = new List<string>();
		foreach (UIStyleDirector.MeshToggleButtonStyle meshToggleButtonStyle in this.meshToggleButtonStyles)
		{
			this.meshToggleButtonDict[meshToggleButtonStyle.name] = meshToggleButtonStyle;
			list14.Add(meshToggleButtonStyle.name);
		}
		this.meshToggleButtonStyleNames = list14.ToArray();
		List<string> list15 = new List<string>();
		foreach (UIStyleDirector.SliderStyle sliderStyle in this.sliderStyles)
		{
			this.sliderDict[sliderStyle.name] = sliderStyle;
			list15.Add(sliderStyle.name);
		}
		this.sliderStyleNames = list15.ToArray();
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x000833B5 File Offset: 0x000815B5
	public UIStyleDirector.TextStyle GetTextStyle(string name)
	{
		return this.textDict.Get(name);
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x000833C3 File Offset: 0x000815C3
	public string[] GetTextStyles()
	{
		return this.textStyleNames;
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x000833CB File Offset: 0x000815CB
	public UIStyleDirector.MeshTextStyle GetMeshTextStyle(string name)
	{
		return this.meshTextDict.Get(name);
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x000833D9 File Offset: 0x000815D9
	public string[] GetMeshTextStyles()
	{
		return this.meshTextStyleNames;
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x000833E1 File Offset: 0x000815E1
	public UIStyleDirector.ButtonStyle GetButtonStyle(string name)
	{
		return this.buttonDict.Get(name);
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000833EF File Offset: 0x000815EF
	public string[] GetButtonStyles()
	{
		return this.buttonStyleNames;
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x000833F7 File Offset: 0x000815F7
	public UIStyleDirector.MeshButtonStyle GetMeshButtonStyle(string name)
	{
		return this.meshButtonDict.Get(name);
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x00083405 File Offset: 0x00081605
	public string[] GetMeshButtonStyles()
	{
		return this.meshButtonStyleNames;
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x0008340D File Offset: 0x0008160D
	public UIStyleDirector.DropdownStyle GetDropdownStyle(string name)
	{
		return this.dropdownDict.Get(name);
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x0008341B File Offset: 0x0008161B
	public string[] GetDropdownStyles()
	{
		return this.dropdownStyleNames;
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x00083423 File Offset: 0x00081623
	public UIStyleDirector.MeshDropdownStyle GetMeshDropdownStyle(string name)
	{
		return this.meshDropdownDict.Get(name);
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x00083431 File Offset: 0x00081631
	public string[] GetMeshDropdownStyles()
	{
		return this.meshDropdownStyleNames;
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x00083439 File Offset: 0x00081639
	public UIStyleDirector.PanelStyle GetPanelStyle(string name)
	{
		return this.panelDict.Get(name);
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x00083447 File Offset: 0x00081647
	public string[] GetPanelStyles()
	{
		return this.panelStyleNames;
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x0008344F File Offset: 0x0008164F
	public UIStyleDirector.IconStyle GetIconStyle(string name)
	{
		return this.iconDict.Get(name);
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x0008345D File Offset: 0x0008165D
	public string[] GetIconStyles()
	{
		return this.iconStyleNames;
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x00083465 File Offset: 0x00081665
	public UIStyleDirector.FieldStyle GetFieldStyle(string name)
	{
		return this.fieldDict.Get(name);
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x00083473 File Offset: 0x00081673
	public string[] GetFieldStyles()
	{
		return this.fieldStyleNames;
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x0008347B File Offset: 0x0008167B
	public UIStyleDirector.ScrollbarStyle GetScrollbarStyle(string name)
	{
		return this.scrollbarDict.Get(name);
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x00083489 File Offset: 0x00081689
	public string[] GetScrollbarStyles()
	{
		return this.scrollbarStyleNames;
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x00083491 File Offset: 0x00081691
	public UIStyleDirector.CheckboxStyle GetCheckboxStyle(string name)
	{
		return this.checkboxDict.Get(name);
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x0008349F File Offset: 0x0008169F
	public string[] GetCheckboxStyles()
	{
		return this.checkboxStyleNames;
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x000834A7 File Offset: 0x000816A7
	public UIStyleDirector.MeshCheckboxStyle GetMeshCheckboxStyle(string name)
	{
		return this.meshCheckboxDict.Get(name);
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000834B5 File Offset: 0x000816B5
	public string[] GetMeshCheckboxStyles()
	{
		return this.meshCheckboxStyleNames;
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000834BD File Offset: 0x000816BD
	public UIStyleDirector.ToggleButtonStyle GetToggleButtonStyle(string name)
	{
		return this.toggleButtonDict.Get(name);
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000834CB File Offset: 0x000816CB
	public string[] GetToggleButtonStyles()
	{
		return this.toggleButtonStyleNames;
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000834D3 File Offset: 0x000816D3
	public UIStyleDirector.MeshToggleButtonStyle GetMeshToggleButtonStyle(string name)
	{
		return this.meshToggleButtonDict.Get(name);
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000834E1 File Offset: 0x000816E1
	public string[] GetMeshToggleButtonStyles()
	{
		return this.meshToggleButtonStyleNames;
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x000834E9 File Offset: 0x000816E9
	public UIStyleDirector.SliderStyle GetSliderStyle(string name)
	{
		return this.sliderDict.Get(name);
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000834F7 File Offset: 0x000816F7
	public string[] GetSliderStyles()
	{
		return this.sliderStyleNames;
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x00083500 File Offset: 0x00081700
	public string[] GetStyleNames(Type type)
	{
		if (type == typeof(UIStyleDirector.ButtonStyle))
		{
			return this.GetButtonStyles();
		}
		if (type == typeof(UIStyleDirector.MeshButtonStyle))
		{
			return this.GetMeshButtonStyles();
		}
		if (type == typeof(UIStyleDirector.DropdownStyle))
		{
			return this.GetDropdownStyles();
		}
		if (type == typeof(UIStyleDirector.MeshDropdownStyle))
		{
			return this.GetMeshDropdownStyles();
		}
		if (type == typeof(UIStyleDirector.TextStyle))
		{
			return this.GetTextStyles();
		}
		if (type == typeof(UIStyleDirector.MeshTextStyle))
		{
			return this.GetMeshTextStyles();
		}
		if (type == typeof(UIStyleDirector.PanelStyle))
		{
			return this.GetPanelStyles();
		}
		if (type == typeof(UIStyleDirector.IconStyle))
		{
			return this.GetIconStyles();
		}
		if (type == typeof(UIStyleDirector.FieldStyle))
		{
			return this.GetFieldStyles();
		}
		if (type == typeof(UIStyleDirector.ScrollbarStyle))
		{
			return this.GetScrollbarStyles();
		}
		if (type == typeof(UIStyleDirector.CheckboxStyle))
		{
			return this.GetCheckboxStyles();
		}
		if (type == typeof(UIStyleDirector.MeshCheckboxStyle))
		{
			return this.GetMeshCheckboxStyles();
		}
		if (type == typeof(UIStyleDirector.ToggleButtonStyle))
		{
			return this.GetToggleButtonStyles();
		}
		if (type == typeof(UIStyleDirector.MeshToggleButtonStyle))
		{
			return this.GetMeshToggleButtonStyles();
		}
		if (type == typeof(UIStyleDirector.SliderStyle))
		{
			return this.GetSliderStyles();
		}
		throw new Exception("Invalid type provided to style");
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x060021E7 RID: 8679 RVA: 0x0008368E File Offset: 0x0008188E
	public static UIStyleDirector Instance
	{
		get
		{
			if (UIStyleDirector.instance == null)
			{
				UIStyleDirector.instance = UIStyleDirector.CreateInstance();
			}
			return UIStyleDirector.instance;
		}
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x000836A6 File Offset: 0x000818A6
	private static UIStyleDirector CreateInstance()
	{
		return UnityEngine.Object.Instantiate<GameObject>(Resources.Load("UIStyleDirector") as GameObject).GetComponent<UIStyleDirector>();
	}

	// Token: 0x04002107 RID: 8455
	public UIStyleDirector.TextStyle[] textStyles = new UIStyleDirector.TextStyle[0];

	// Token: 0x04002108 RID: 8456
	public UIStyleDirector.MeshTextStyle[] meshTextStyles = new UIStyleDirector.MeshTextStyle[0];

	// Token: 0x04002109 RID: 8457
	public UIStyleDirector.DropdownStyle[] dropdownStyles = new UIStyleDirector.DropdownStyle[0];

	// Token: 0x0400210A RID: 8458
	public UIStyleDirector.MeshDropdownStyle[] meshDropdownStyles = new UIStyleDirector.MeshDropdownStyle[0];

	// Token: 0x0400210B RID: 8459
	public UIStyleDirector.ButtonStyle[] buttonStyles = new UIStyleDirector.ButtonStyle[0];

	// Token: 0x0400210C RID: 8460
	public UIStyleDirector.MeshButtonStyle[] meshButtonStyles = new UIStyleDirector.MeshButtonStyle[0];

	// Token: 0x0400210D RID: 8461
	public UIStyleDirector.PanelStyle[] panelStyles = new UIStyleDirector.PanelStyle[0];

	// Token: 0x0400210E RID: 8462
	public UIStyleDirector.IconStyle[] iconStyles = new UIStyleDirector.IconStyle[0];

	// Token: 0x0400210F RID: 8463
	public UIStyleDirector.FieldStyle[] fieldStyles = new UIStyleDirector.FieldStyle[0];

	// Token: 0x04002110 RID: 8464
	public UIStyleDirector.ScrollbarStyle[] scrollbarStyles = new UIStyleDirector.ScrollbarStyle[0];

	// Token: 0x04002111 RID: 8465
	public UIStyleDirector.CheckboxStyle[] checkboxStyles = new UIStyleDirector.CheckboxStyle[0];

	// Token: 0x04002112 RID: 8466
	public UIStyleDirector.MeshCheckboxStyle[] meshCheckboxStyles = new UIStyleDirector.MeshCheckboxStyle[0];

	// Token: 0x04002113 RID: 8467
	public UIStyleDirector.ToggleButtonStyle[] toggleButtonStyles = new UIStyleDirector.ToggleButtonStyle[0];

	// Token: 0x04002114 RID: 8468
	public UIStyleDirector.MeshToggleButtonStyle[] meshToggleButtonStyles = new UIStyleDirector.MeshToggleButtonStyle[0];

	// Token: 0x04002115 RID: 8469
	public UIStyleDirector.SliderStyle[] sliderStyles = new UIStyleDirector.SliderStyle[0];

	// Token: 0x04002116 RID: 8470
	private Dictionary<string, UIStyleDirector.TextStyle> textDict = new Dictionary<string, UIStyleDirector.TextStyle>();

	// Token: 0x04002117 RID: 8471
	private string[] textStyleNames = new string[0];

	// Token: 0x04002118 RID: 8472
	private Dictionary<string, UIStyleDirector.MeshTextStyle> meshTextDict = new Dictionary<string, UIStyleDirector.MeshTextStyle>();

	// Token: 0x04002119 RID: 8473
	private string[] meshTextStyleNames = new string[0];

	// Token: 0x0400211A RID: 8474
	private Dictionary<string, UIStyleDirector.ButtonStyle> buttonDict = new Dictionary<string, UIStyleDirector.ButtonStyle>();

	// Token: 0x0400211B RID: 8475
	private string[] buttonStyleNames = new string[0];

	// Token: 0x0400211C RID: 8476
	private Dictionary<string, UIStyleDirector.MeshButtonStyle> meshButtonDict = new Dictionary<string, UIStyleDirector.MeshButtonStyle>();

	// Token: 0x0400211D RID: 8477
	private string[] meshButtonStyleNames = new string[0];

	// Token: 0x0400211E RID: 8478
	private Dictionary<string, UIStyleDirector.DropdownStyle> dropdownDict = new Dictionary<string, UIStyleDirector.DropdownStyle>();

	// Token: 0x0400211F RID: 8479
	private string[] dropdownStyleNames = new string[0];

	// Token: 0x04002120 RID: 8480
	private Dictionary<string, UIStyleDirector.MeshDropdownStyle> meshDropdownDict = new Dictionary<string, UIStyleDirector.MeshDropdownStyle>();

	// Token: 0x04002121 RID: 8481
	private string[] meshDropdownStyleNames = new string[0];

	// Token: 0x04002122 RID: 8482
	private Dictionary<string, UIStyleDirector.PanelStyle> panelDict = new Dictionary<string, UIStyleDirector.PanelStyle>();

	// Token: 0x04002123 RID: 8483
	private string[] panelStyleNames = new string[0];

	// Token: 0x04002124 RID: 8484
	private Dictionary<string, UIStyleDirector.IconStyle> iconDict = new Dictionary<string, UIStyleDirector.IconStyle>();

	// Token: 0x04002125 RID: 8485
	private string[] iconStyleNames = new string[0];

	// Token: 0x04002126 RID: 8486
	private Dictionary<string, UIStyleDirector.FieldStyle> fieldDict = new Dictionary<string, UIStyleDirector.FieldStyle>();

	// Token: 0x04002127 RID: 8487
	private string[] fieldStyleNames = new string[0];

	// Token: 0x04002128 RID: 8488
	private Dictionary<string, UIStyleDirector.ScrollbarStyle> scrollbarDict = new Dictionary<string, UIStyleDirector.ScrollbarStyle>();

	// Token: 0x04002129 RID: 8489
	private string[] scrollbarStyleNames = new string[0];

	// Token: 0x0400212A RID: 8490
	private Dictionary<string, UIStyleDirector.CheckboxStyle> checkboxDict = new Dictionary<string, UIStyleDirector.CheckboxStyle>();

	// Token: 0x0400212B RID: 8491
	private string[] checkboxStyleNames = new string[0];

	// Token: 0x0400212C RID: 8492
	private Dictionary<string, UIStyleDirector.MeshCheckboxStyle> meshCheckboxDict = new Dictionary<string, UIStyleDirector.MeshCheckboxStyle>();

	// Token: 0x0400212D RID: 8493
	private string[] meshCheckboxStyleNames = new string[0];

	// Token: 0x0400212E RID: 8494
	private Dictionary<string, UIStyleDirector.ToggleButtonStyle> toggleButtonDict = new Dictionary<string, UIStyleDirector.ToggleButtonStyle>();

	// Token: 0x0400212F RID: 8495
	private string[] toggleButtonStyleNames = new string[0];

	// Token: 0x04002130 RID: 8496
	private Dictionary<string, UIStyleDirector.MeshToggleButtonStyle> meshToggleButtonDict = new Dictionary<string, UIStyleDirector.MeshToggleButtonStyle>();

	// Token: 0x04002131 RID: 8497
	private string[] meshToggleButtonStyleNames = new string[0];

	// Token: 0x04002132 RID: 8498
	private Dictionary<string, UIStyleDirector.SliderStyle> sliderDict = new Dictionary<string, UIStyleDirector.SliderStyle>();

	// Token: 0x04002133 RID: 8499
	private string[] sliderStyleNames = new string[0];

	// Token: 0x04002134 RID: 8500
	public TMP_FontAsset defaultMeshFont;

	// Token: 0x04002135 RID: 8501
	private static UIStyleDirector instance;

	// Token: 0x02000650 RID: 1616
	public class Setting
	{
	}

	// Token: 0x02000651 RID: 1617
	[Serializable]
	public class TransitionSetting : UIStyleDirector.Setting
	{
		// Token: 0x04002136 RID: 8502
		public bool apply;

		// Token: 0x04002137 RID: 8503
		public Selectable.Transition value;
	}

	// Token: 0x02000652 RID: 1618
	[Serializable]
	public class ColorSetting : UIStyleDirector.Setting
	{
		// Token: 0x04002138 RID: 8504
		public bool apply;

		// Token: 0x04002139 RID: 8505
		public Color value;
	}

	// Token: 0x02000653 RID: 1619
	[Serializable]
	public class SpriteSetting : UIStyleDirector.Setting
	{
		// Token: 0x0400213A RID: 8506
		public bool apply;

		// Token: 0x0400213B RID: 8507
		public Sprite value;
	}

	// Token: 0x02000654 RID: 1620
	[Serializable]
	public class GameObjSetting : UIStyleDirector.Setting
	{
		// Token: 0x0400213C RID: 8508
		public bool apply;

		// Token: 0x0400213D RID: 8509
		public GameObject value;
	}

	// Token: 0x02000655 RID: 1621
	[Serializable]
	public class FontSetting : UIStyleDirector.Setting
	{
		// Token: 0x0400213E RID: 8510
		public bool apply;

		// Token: 0x0400213F RID: 8511
		public Font value;
	}

	// Token: 0x02000656 RID: 1622
	[Serializable]
	public class FontStyleSetting : UIStyleDirector.Setting
	{
		// Token: 0x04002140 RID: 8512
		public bool apply;

		// Token: 0x04002141 RID: 8513
		public FontStyle value;
	}

	// Token: 0x02000657 RID: 1623
	[Serializable]
	public class MeshFontSetting : UIStyleDirector.Setting
	{
		// Token: 0x04002142 RID: 8514
		public bool apply;

		// Token: 0x04002143 RID: 8515
		public TMP_FontAsset value;
	}

	// Token: 0x02000658 RID: 1624
	[Serializable]
	public class IntSetting : UIStyleDirector.Setting
	{
		// Token: 0x04002144 RID: 8516
		public bool apply;

		// Token: 0x04002145 RID: 8517
		public int value;
	}

	// Token: 0x02000659 RID: 1625
	[Serializable]
	public class BoolSetting : UIStyleDirector.Setting
	{
		// Token: 0x04002146 RID: 8518
		public bool apply;

		// Token: 0x04002147 RID: 8519
		public bool value;
	}

	// Token: 0x0200065A RID: 1626
	[Serializable]
	public class FloatSetting : UIStyleDirector.Setting
	{
		// Token: 0x04002148 RID: 8520
		public bool apply;

		// Token: 0x04002149 RID: 8521
		public float value;
	}

	// Token: 0x0200065B RID: 1627
	[Serializable]
	public class MaterialPresetSetting : UIStyleDirector.Setting
	{
		// Token: 0x0400214A RID: 8522
		public bool apply;

		// Token: 0x0400214B RID: 8523
		public Material value;
	}

	// Token: 0x0200065C RID: 1628
	[Serializable]
	public class MeshTextStyle
	{
		// Token: 0x060021F6 RID: 8694 RVA: 0x000838EC File Offset: 0x00081AEC
		internal void Convert(UIStyleDirector styleDir, UIStyleDirector.TextStyle oldStyle)
		{
			this.bold = new UIStyleDirector.BoolSetting();
			this.bold.value = (oldStyle.fontStyle.value == FontStyle.Bold || oldStyle.fontStyle.value == FontStyle.BoldAndItalic);
			this.bold.apply = oldStyle.fontStyle.apply;
			this.italic = new UIStyleDirector.BoolSetting();
			this.italic.value = (oldStyle.fontStyle.value == FontStyle.Italic || oldStyle.fontStyle.value == FontStyle.BoldAndItalic);
			this.italic.apply = oldStyle.fontStyle.apply;
			this.font = new UIStyleDirector.MeshFontSetting();
			this.font.value = styleDir.defaultMeshFont;
			this.font.apply = oldStyle.font.apply;
			this.fontSize = new UIStyleDirector.IntSetting();
			this.fontSize.value = oldStyle.fontSize.value;
			this.fontSize.apply = oldStyle.fontSize.apply;
			this.textColor = new UIStyleDirector.ColorSetting();
			this.textColor.value = oldStyle.textColor.value;
			this.textColor.apply = oldStyle.textColor.apply;
			this.materialPreset = null;
			this.name = oldStyle.name;
		}

		// Token: 0x0400214C RID: 8524
		public string name;

		// Token: 0x0400214D RID: 8525
		public UIStyleDirector.ColorSetting textColor;

		// Token: 0x0400214E RID: 8526
		public UIStyleDirector.MeshFontSetting font;

		// Token: 0x0400214F RID: 8527
		public UIStyleDirector.MaterialPresetSetting materialPreset;

		// Token: 0x04002150 RID: 8528
		public UIStyleDirector.BoolSetting bold;

		// Token: 0x04002151 RID: 8529
		public UIStyleDirector.BoolSetting italic;

		// Token: 0x04002152 RID: 8530
		public UIStyleDirector.IntSetting fontSize;
	}

	// Token: 0x0200065D RID: 1629
	[Serializable]
	public class TextStyle
	{
		// Token: 0x04002153 RID: 8531
		public string name;

		// Token: 0x04002154 RID: 8532
		public UIStyleDirector.ColorSetting textColor;

		// Token: 0x04002155 RID: 8533
		public UIStyleDirector.FontSetting font;

		// Token: 0x04002156 RID: 8534
		public UIStyleDirector.FontStyleSetting fontStyle;

		// Token: 0x04002157 RID: 8535
		public UIStyleDirector.IntSetting fontSize;

		// Token: 0x04002158 RID: 8536
		public UIStyleDirector.ColorSetting outlineColor;

		// Token: 0x04002159 RID: 8537
		public UIStyleDirector.FloatSetting outlineWidth;
	}

	// Token: 0x0200065E RID: 1630
	[Serializable]
	public class MeshButtonStyle : UIStyleDirector.MeshTextStyle
	{
		// Token: 0x060021F9 RID: 8697 RVA: 0x00083A44 File Offset: 0x00081C44
		public void Convert(UIStyleDirector styleDir, UIStyleDirector.ButtonStyle buttonStyle)
		{
			base.Convert(styleDir, buttonStyle);
			this.bgSprite = buttonStyle.bgSprite;
			this.bgColor = buttonStyle.bgColor;
			this.normalTint = buttonStyle.normalTint;
			this.highlightedTint = buttonStyle.highlightedTint;
			this.pressedTint = buttonStyle.pressedTint;
			this.disabledTint = buttonStyle.disabledTint;
			this.includeChild = buttonStyle.includeChild;
			this.hideIfGamepad = buttonStyle.hideIfGamepad;
			this.transition = buttonStyle.transition;
			this.disabledSprite = buttonStyle.disabledSprite;
			this.highlightedSprite = buttonStyle.highlightedSprite;
			this.pressedSprite = buttonStyle.pressedSprite;
		}

		// Token: 0x0400215A RID: 8538
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x0400215B RID: 8539
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x0400215C RID: 8540
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x0400215D RID: 8541
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x0400215E RID: 8542
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x0400215F RID: 8543
		public UIStyleDirector.ColorSetting disabledTint;

		// Token: 0x04002160 RID: 8544
		public UIStyleDirector.GameObjSetting includeChild;

		// Token: 0x04002161 RID: 8545
		public bool hideIfGamepad;

		// Token: 0x04002162 RID: 8546
		public UIStyleDirector.TransitionSetting transition;

		// Token: 0x04002163 RID: 8547
		public UIStyleDirector.SpriteSetting disabledSprite;

		// Token: 0x04002164 RID: 8548
		public UIStyleDirector.SpriteSetting highlightedSprite;

		// Token: 0x04002165 RID: 8549
		public UIStyleDirector.SpriteSetting pressedSprite;
	}

	// Token: 0x0200065F RID: 1631
	[Serializable]
	public class ButtonStyle : UIStyleDirector.TextStyle
	{
		// Token: 0x04002166 RID: 8550
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x04002167 RID: 8551
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x04002168 RID: 8552
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x04002169 RID: 8553
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x0400216A RID: 8554
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x0400216B RID: 8555
		public UIStyleDirector.ColorSetting disabledTint;

		// Token: 0x0400216C RID: 8556
		public UIStyleDirector.GameObjSetting includeChild;

		// Token: 0x0400216D RID: 8557
		public bool hideIfGamepad;

		// Token: 0x0400216E RID: 8558
		public UIStyleDirector.TransitionSetting transition;

		// Token: 0x0400216F RID: 8559
		public UIStyleDirector.SpriteSetting disabledSprite;

		// Token: 0x04002170 RID: 8560
		public UIStyleDirector.SpriteSetting highlightedSprite;

		// Token: 0x04002171 RID: 8561
		public UIStyleDirector.SpriteSetting pressedSprite;
	}

	// Token: 0x02000660 RID: 1632
	[Serializable]
	public class MeshDropdownStyle : UIStyleDirector.MeshTextStyle
	{
		// Token: 0x060021FC RID: 8700 RVA: 0x00083AFC File Offset: 0x00081CFC
		public void Convert(UIStyleDirector styleDir, UIStyleDirector.DropdownStyle dropdownStyle)
		{
			base.Convert(styleDir, dropdownStyle);
			this.bgSprite = dropdownStyle.bgSprite;
			this.bgColor = dropdownStyle.bgColor;
			this.normalTint = dropdownStyle.normalTint;
			this.highlightedTint = dropdownStyle.highlightedTint;
			this.pressedTint = dropdownStyle.pressedTint;
			this.disabledTint = dropdownStyle.disabledTint;
			this.menuBgSprite = dropdownStyle.menuBgSprite;
			this.menuBgColor = dropdownStyle.menuBgColor;
			this.itemBgSprite = dropdownStyle.itemBgSprite;
			this.itemBgColor = dropdownStyle.itemBgColor;
			this.itemNormalTint = dropdownStyle.itemNormalTint;
			this.itemHighlightedTint = dropdownStyle.itemHighlightedTint;
			this.itemPressedTint = dropdownStyle.itemPressedTint;
			this.itemDisabledTint = dropdownStyle.itemDisabledTint;
		}

		// Token: 0x04002172 RID: 8562
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x04002173 RID: 8563
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x04002174 RID: 8564
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x04002175 RID: 8565
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x04002176 RID: 8566
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x04002177 RID: 8567
		public UIStyleDirector.ColorSetting disabledTint;

		// Token: 0x04002178 RID: 8568
		public UIStyleDirector.SpriteSetting menuBgSprite;

		// Token: 0x04002179 RID: 8569
		public UIStyleDirector.ColorSetting menuBgColor;

		// Token: 0x0400217A RID: 8570
		public UIStyleDirector.SpriteSetting itemBgSprite;

		// Token: 0x0400217B RID: 8571
		public UIStyleDirector.ColorSetting itemBgColor;

		// Token: 0x0400217C RID: 8572
		public UIStyleDirector.ColorSetting itemNormalTint;

		// Token: 0x0400217D RID: 8573
		public UIStyleDirector.ColorSetting itemHighlightedTint;

		// Token: 0x0400217E RID: 8574
		public UIStyleDirector.ColorSetting itemPressedTint;

		// Token: 0x0400217F RID: 8575
		public UIStyleDirector.ColorSetting itemDisabledTint;
	}

	// Token: 0x02000661 RID: 1633
	[Serializable]
	public class DropdownStyle : UIStyleDirector.TextStyle
	{
		// Token: 0x04002180 RID: 8576
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x04002181 RID: 8577
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x04002182 RID: 8578
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x04002183 RID: 8579
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x04002184 RID: 8580
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x04002185 RID: 8581
		public UIStyleDirector.ColorSetting disabledTint;

		// Token: 0x04002186 RID: 8582
		public UIStyleDirector.SpriteSetting menuBgSprite;

		// Token: 0x04002187 RID: 8583
		public UIStyleDirector.ColorSetting menuBgColor;

		// Token: 0x04002188 RID: 8584
		public UIStyleDirector.SpriteSetting itemBgSprite;

		// Token: 0x04002189 RID: 8585
		public UIStyleDirector.ColorSetting itemBgColor;

		// Token: 0x0400218A RID: 8586
		public UIStyleDirector.ColorSetting itemNormalTint;

		// Token: 0x0400218B RID: 8587
		public UIStyleDirector.ColorSetting itemHighlightedTint;

		// Token: 0x0400218C RID: 8588
		public UIStyleDirector.ColorSetting itemPressedTint;

		// Token: 0x0400218D RID: 8589
		public UIStyleDirector.ColorSetting itemDisabledTint;
	}

	// Token: 0x02000662 RID: 1634
	[Serializable]
	public class PanelStyle
	{
		// Token: 0x0400218E RID: 8590
		public string name;

		// Token: 0x0400218F RID: 8591
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x04002190 RID: 8592
		public UIStyleDirector.ColorSetting bgColor;
	}

	// Token: 0x02000663 RID: 1635
	[Serializable]
	public class IconStyle
	{
		// Token: 0x04002191 RID: 8593
		public string name;

		// Token: 0x04002192 RID: 8594
		public UIStyleDirector.SpriteSetting sprite;

		// Token: 0x04002193 RID: 8595
		public UIStyleDirector.ColorSetting color;
	}

	// Token: 0x02000664 RID: 1636
	[Serializable]
	public class FieldStyle : UIStyleDirector.TextStyle
	{
		// Token: 0x04002194 RID: 8596
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x04002195 RID: 8597
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x04002196 RID: 8598
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x04002197 RID: 8599
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x04002198 RID: 8600
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x04002199 RID: 8601
		public UIStyleDirector.ColorSetting disabledTint;

		// Token: 0x0400219A RID: 8602
		public UIStyleDirector.ColorSetting placeholderTextColor;

		// Token: 0x0400219B RID: 8603
		public UIStyleDirector.FontSetting placeholderFont;

		// Token: 0x0400219C RID: 8604
		public UIStyleDirector.FontStyleSetting placeholderFontStyle;

		// Token: 0x0400219D RID: 8605
		public UIStyleDirector.IntSetting placeholderFontSize;

		// Token: 0x0400219E RID: 8606
		public UIStyleDirector.ColorSetting placeholderOutlineColor;

		// Token: 0x0400219F RID: 8607
		public UIStyleDirector.FloatSetting placeholderOutlineWidth;

		// Token: 0x040021A0 RID: 8608
		public UIStyleDirector.ColorSetting selectionColor;
	}

	// Token: 0x02000665 RID: 1637
	[Serializable]
	public class ScrollbarStyle
	{
		// Token: 0x040021A1 RID: 8609
		public string name;

		// Token: 0x040021A2 RID: 8610
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x040021A3 RID: 8611
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x040021A4 RID: 8612
		public UIStyleDirector.SpriteSetting handleSprite;

		// Token: 0x040021A5 RID: 8613
		public UIStyleDirector.ColorSetting handleColor;

		// Token: 0x040021A6 RID: 8614
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x040021A7 RID: 8615
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x040021A8 RID: 8616
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x040021A9 RID: 8617
		public UIStyleDirector.ColorSetting disabledTint;
	}

	// Token: 0x02000666 RID: 1638
	[Serializable]
	public class MeshCheckboxStyle : UIStyleDirector.MeshTextStyle
	{
		// Token: 0x06002203 RID: 8707 RVA: 0x00083BBC File Offset: 0x00081DBC
		public void Convert(UIStyleDirector styleDir, UIStyleDirector.CheckboxStyle checkboxStyle)
		{
			base.Convert(styleDir, checkboxStyle);
			this.markSprite = checkboxStyle.markSprite;
			this.markColor = checkboxStyle.markColor;
			this.bgSprite = checkboxStyle.bgSprite;
			this.bgColor = checkboxStyle.bgColor;
			this.normalTint = checkboxStyle.normalTint;
			this.highlightedTint = checkboxStyle.highlightedTint;
			this.pressedTint = checkboxStyle.pressedTint;
			this.disabledTint = checkboxStyle.disabledTint;
		}

		// Token: 0x040021AA RID: 8618
		public UIStyleDirector.SpriteSetting markSprite;

		// Token: 0x040021AB RID: 8619
		public UIStyleDirector.ColorSetting markColor;

		// Token: 0x040021AC RID: 8620
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x040021AD RID: 8621
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x040021AE RID: 8622
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x040021AF RID: 8623
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x040021B0 RID: 8624
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x040021B1 RID: 8625
		public UIStyleDirector.ColorSetting disabledTint;
	}

	// Token: 0x02000667 RID: 1639
	[Serializable]
	public class CheckboxStyle : UIStyleDirector.TextStyle
	{
		// Token: 0x040021B2 RID: 8626
		public UIStyleDirector.SpriteSetting markSprite;

		// Token: 0x040021B3 RID: 8627
		public UIStyleDirector.ColorSetting markColor;

		// Token: 0x040021B4 RID: 8628
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x040021B5 RID: 8629
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x040021B6 RID: 8630
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x040021B7 RID: 8631
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x040021B8 RID: 8632
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x040021B9 RID: 8633
		public UIStyleDirector.ColorSetting disabledTint;
	}

	// Token: 0x02000668 RID: 1640
	[Serializable]
	public class MeshToggleButtonStyle : UIStyleDirector.MeshTextStyle
	{
		// Token: 0x06002206 RID: 8710 RVA: 0x00083C34 File Offset: 0x00081E34
		public void Convert(UIStyleDirector styleDir, UIStyleDirector.ToggleButtonStyle toggleButtonStyle)
		{
			base.Convert(styleDir, toggleButtonStyle);
			this.selectedSprite = toggleButtonStyle.selectedSprite;
			this.selectedColor = toggleButtonStyle.selectedColor;
			this.bgSprite = toggleButtonStyle.bgSprite;
			this.bgColor = toggleButtonStyle.bgColor;
			this.normalTint = toggleButtonStyle.normalTint;
			this.highlightedTint = toggleButtonStyle.highlightedTint;
			this.pressedTint = toggleButtonStyle.pressedTint;
			this.disabledTint = toggleButtonStyle.disabledTint;
		}

		// Token: 0x040021BA RID: 8634
		public UIStyleDirector.SpriteSetting selectedSprite;

		// Token: 0x040021BB RID: 8635
		public UIStyleDirector.ColorSetting selectedColor;

		// Token: 0x040021BC RID: 8636
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x040021BD RID: 8637
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x040021BE RID: 8638
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x040021BF RID: 8639
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x040021C0 RID: 8640
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x040021C1 RID: 8641
		public UIStyleDirector.ColorSetting disabledTint;
	}

	// Token: 0x02000669 RID: 1641
	[Serializable]
	public class ToggleButtonStyle : UIStyleDirector.TextStyle
	{
		// Token: 0x040021C2 RID: 8642
		public UIStyleDirector.SpriteSetting selectedSprite;

		// Token: 0x040021C3 RID: 8643
		public UIStyleDirector.ColorSetting selectedColor;

		// Token: 0x040021C4 RID: 8644
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x040021C5 RID: 8645
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x040021C6 RID: 8646
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x040021C7 RID: 8647
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x040021C8 RID: 8648
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x040021C9 RID: 8649
		public UIStyleDirector.ColorSetting disabledTint;
	}

	// Token: 0x0200066A RID: 1642
	[Serializable]
	public class SliderStyle
	{
		// Token: 0x040021CA RID: 8650
		public string name;

		// Token: 0x040021CB RID: 8651
		public UIStyleDirector.SpriteSetting bgSprite;

		// Token: 0x040021CC RID: 8652
		public UIStyleDirector.ColorSetting bgColor;

		// Token: 0x040021CD RID: 8653
		public UIStyleDirector.SpriteSetting fillSprite;

		// Token: 0x040021CE RID: 8654
		public UIStyleDirector.ColorSetting fillColor;

		// Token: 0x040021CF RID: 8655
		public UIStyleDirector.SpriteSetting handleSprite;

		// Token: 0x040021D0 RID: 8656
		public UIStyleDirector.ColorSetting handleColor;

		// Token: 0x040021D1 RID: 8657
		public UIStyleDirector.ColorSetting normalTint;

		// Token: 0x040021D2 RID: 8658
		public UIStyleDirector.ColorSetting highlightedTint;

		// Token: 0x040021D3 RID: 8659
		public UIStyleDirector.ColorSetting pressedTint;

		// Token: 0x040021D4 RID: 8660
		public UIStyleDirector.ColorSetting disabledTint;
	}
}
