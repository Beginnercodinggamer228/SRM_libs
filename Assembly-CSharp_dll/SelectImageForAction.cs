using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000615 RID: 1557
[RequireComponent(typeof(Image))]
public class SelectImageForAction : MonoBehaviour
{
	// Token: 0x060020A6 RID: 8358 RVA: 0x0007CDD4 File Offset: 0x0007AFD4
	public void Awake()
	{
		this.img = base.GetComponent<Image>();
		this.inputDir = SRSingleton<GameContext>.Instance.InputDirector;
		InputDirector inputDirector = this.inputDir;
		inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Combine(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x0007CE24 File Offset: 0x0007B024
	public void OnDestroy()
	{
		InputDirector inputDirector = this.inputDir;
		inputDirector.onKeysChanged = (InputDirector.OnKeysChanged)Delegate.Remove(inputDirector.onKeysChanged, new InputDirector.OnKeysChanged(this.OnKeysChanged));
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x0007CE4D File Offset: 0x0007B04D
	public void OnKeysChanged()
	{
		this.UpdateButtonImage();
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x0007CE4D File Offset: 0x0007B04D
	public void Start()
	{
		this.UpdateButtonImage();
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x0007CE58 File Offset: 0x0007B058
	public void UpdateButtonImage()
	{
		bool flag;
		this.img.sprite = this.inputDir.GetActiveDeviceIcon(this.action, this.isPauseAction, out flag);
	}

	// Token: 0x04002001 RID: 8193
	public string action;

	// Token: 0x04002002 RID: 8194
	public bool isPauseAction;

	// Token: 0x04002003 RID: 8195
	private Image img;

	// Token: 0x04002004 RID: 8196
	private InputDirector inputDir;
}
