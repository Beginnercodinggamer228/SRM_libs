using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000564 RID: 1380
public class DebugQualityDropdown : MonoBehaviour
{
	// Token: 0x06001CC8 RID: 7368 RVA: 0x0006DB58 File Offset: 0x0006BD58
	public void Awake()
	{
		List<SRQualitySettings.Level> levels = new List<SRQualitySettings.Level>
		{
			SRQualitySettings.Level.LOWEST,
			SRQualitySettings.Level.LOW,
			SRQualitySettings.Level.DEFAULT,
			SRQualitySettings.Level.HIGH,
			SRQualitySettings.Level.VERY_HIGH
		};
		this.dropdown.ClearOptions();
		this.dropdown.AddOptions((from level in levels
		select Enum.GetName(typeof(SRQualitySettings.Level), level)).ToList<string>());
		this.dropdown.onValueChanged.AddListener(delegate(int index)
		{
			SRQualitySettings.CurrentLevel = levels[index];
		});
		this.dropdown.value = 2;
	}

	// Token: 0x04001BE2 RID: 7138
	public Dropdown dropdown;
}
