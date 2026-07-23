using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C2 RID: 450
public class DroneUI : BaseUI
{
	// Token: 0x06000999 RID: 2457 RVA: 0x0002A83C File Offset: 0x00028A3C
	public DroneUI Init(DroneGadget gadget)
	{
		this.gadget = gadget;
		this.programs = (from p in this.gadget.programs
		select p.Clone()).ToArray<DroneMetadata.Program>();
		this.ResetUI();
		string programWarning = this.GetProgramWarning();
		this.warningText.gameObject.SetActive(programWarning != null);
		if (programWarning != null)
		{
			this.warningText.text = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("ui").Xlate(programWarning);
		}
		return this;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0002A8D4 File Offset: 0x00028AD4
	public void Start()
	{
		SECTR_AudioSystem.Play(this.metadata.onGuiEnableCue, Vector3.zero, false);
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x0002A8ED File Offset: 0x00028AED
	public void OnEnable()
	{
		if (this.gadget != null)
		{
			SECTR_AudioSystem.Play(this.metadata.onGuiEnableCue, Vector3.zero, false);
		}
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0002A914 File Offset: 0x00028B14
	public void OnDisable()
	{
		SECTR_AudioSystem.Play(this.metadata.onGuiDisableCue, Vector3.zero, false);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0002A92D File Offset: 0x00028B2D
	private string GetProgramWarning()
	{
		if (!this.gadget.drone.ammo.IsEmpty())
		{
			return "w.drone_reprogram_drops_ammo";
		}
		return null;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0002A950 File Offset: 0x00028B50
	private void ResetUI()
	{
		foreach (DroneUIProgram droneUIProgram in this.programUIs)
		{
			Destroyer.Destroy(droneUIProgram.gameObject, "DroneUI.ResetUI");
		}
		this.programUIs.Clear();
		for (int i = 0; i < this.programs.Length; i++)
		{
			DroneMetadata.Program program = this.programs[i];
			int? index = (this.programs.Length >= 2) ? new int?(i + 1) : null;
			DroneUIProgram droneUIProgram2 = UnityEngine.Object.Instantiate<GameObject>(this.metadata.droneUIProgram.gameObject, this.programsParent).GetComponent<DroneUIProgram>().Init(program, index);
			this.programUIs.Add(droneUIProgram2);
			int idx = i;
			this.SetProgramPicker(droneUIProgram2.buttonTarget, program, (DroneMetadata.Program p) => new DroneMetadata.Program(this.metadata.GetDefaultTarget(), this.metadata.GetDefaultBehaviour(), this.metadata.GetDefaultBehaviour()), true, delegate(DroneMetadata.Program wp)
			{
				this.GatherTarget(wp, delegate(DroneMetadata.Program.Target tgt)
				{
					wp.target = tgt;
					this.programs[idx] = (program = wp);
					this.programsChanged = true;
				});
			});
			this.SetProgramPicker(droneUIProgram2.buttonSource, program, (DroneMetadata.Program p) => new DroneMetadata.Program(p.target, this.metadata.GetDefaultBehaviour(), this.metadata.GetDefaultBehaviour()), program.target.id != "drone.target.none", delegate(DroneMetadata.Program wp)
			{
				this.GatherSource(wp, delegate(DroneMetadata.Program.Behaviour src)
				{
					wp.source = src;
					this.programs[idx] = (program = wp);
					this.programsChanged = true;
				});
			});
			this.SetProgramPicker(droneUIProgram2.buttonDestination, program, (DroneMetadata.Program p) => new DroneMetadata.Program(p.target, p.source, this.metadata.GetDefaultBehaviour()), program.source.id != "drone.behaviour.none", delegate(DroneMetadata.Program wp)
			{
				this.GatherDestination(wp, delegate(DroneMetadata.Program.Behaviour dest)
				{
					wp.destination = dest;
					this.programs[idx] = (program = wp);
					this.programsChanged = true;
				});
			});
		}
		this.UpdateButtonState();
		this.SelectFirstButton();
		for (int j = 1; j < this.programUIs.Count; j++)
		{
			DroneUIProgram droneUIProgram3 = this.programUIs[j - 1];
			DroneUIProgram down = this.programUIs[j];
			droneUIProgram3.LinkGamepadNav(down);
		}
		this.programUIs.Last<DroneUIProgram>().LinkGamepadNav(this.activateButton.interactable ? this.activateButton : this.resetButton);
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x0002AB78 File Offset: 0x00028D78
	private void UpdateButtonState()
	{
		Selectable selectable = this.activateButton;
		bool interactable;
		if (this.programsChanged)
		{
			interactable = this.programs.Any((DroneMetadata.Program p) => p.IsComplete());
		}
		else
		{
			interactable = false;
		}
		selectable.interactable = interactable;
		this.resetButton.interactable = this.programs.Any((DroneMetadata.Program p) => !p.IsReset());
		SRBehaviour.LinkNavigation(this.activateButton, this.resetButton, SRBehaviour.NavigationDirection.DOWN_UP);
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x0002AC0C File Offset: 0x00028E0C
	private void SelectFirstButton()
	{
		for (int i = 0; i < this.programs.Length; i++)
		{
			if (this.programs[i].target.id == "drone.target.none")
			{
				this.programUIs[i].buttonTarget.button.Select();
				return;
			}
			if (this.programs[i].source.id == "drone.behaviour.none")
			{
				this.programUIs[i].buttonSource.button.Select();
				return;
			}
			if (this.programs[i].destination.id == "drone.behaviour.none")
			{
				this.programUIs[i].buttonDestination.button.Select();
				return;
			}
		}
		if (this.activateButton.interactable)
		{
			this.activateButton.Select();
			return;
		}
		this.programUIs[0].buttonTarget.button.Select();
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0002AD14 File Offset: 0x00028F14
	protected override bool Closeable()
	{
		return base.Closeable() && this.pickerUI == null;
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0002AD2C File Offset: 0x00028F2C
	public void OnClickConfirmation()
	{
		SECTR_AudioSystem.Play(this.metadata.onGuiButtonActivateCue, Vector3.zero, false);
		this.gadget.SetPrograms(this.programs);
		this.Close();
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x0002AD5C File Offset: 0x00028F5C
	public void OnClickReset()
	{
		SECTR_AudioSystem.Play(this.metadata.onGuiButtonResetCue, Vector3.zero, false);
		foreach (DroneMetadata.Program program in this.programs)
		{
			program.target = this.metadata.GetDefaultTarget();
			program.source = this.metadata.GetDefaultBehaviour();
			program.destination = this.metadata.GetDefaultBehaviour();
		}
		this.gadget.SetPrograms(this.programs);
		this.programsChanged = false;
		this.ResetUI();
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x0002ADE8 File Offset: 0x00028FE8
	private void SetProgramPicker(DroneUIProgramButton button, DroneMetadata.Program program, DroneUI.DeriveProgram deriver, bool interactable, Action<DroneMetadata.Program> onClicked)
	{
		button.button.interactable = interactable;
		button.button.onClick.AddListener(delegate()
		{
			DroneMetadata.Program obj = deriver(program);
			onClicked(obj);
		});
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x0002AE3C File Offset: 0x0002903C
	private void GatherTarget(DroneMetadata.Program workingProgram, Action<DroneMetadata.Program.Target> onComplete)
	{
		if (workingProgram.target.id == "drone.target.none")
		{
			this.CreatePicker<DroneMetadata.Program.Target>("t.drone.pick_target", this.metadata.pickTargetIcon, this.metadata.targets, this.metadata.onGuiButtonTargetCue, onComplete);
			return;
		}
		onComplete(workingProgram.target);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0002AE9C File Offset: 0x0002909C
	private void GatherSource(DroneMetadata.Program workingProgram, Action<DroneMetadata.Program.Behaviour> onComplete)
	{
		if (workingProgram.source.id == "drone.behaviour.none")
		{
			this.CreatePicker<DroneMetadata.Program.Behaviour>("t.drone.pick_source", this.metadata.pickSourceIcon, this.FilterSources(workingProgram, this.metadata.sources), this.metadata.onGuiButtonSourceCue, onComplete);
			return;
		}
		onComplete(workingProgram.source);
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x0002AF04 File Offset: 0x00029104
	private void GatherDestination(DroneMetadata.Program workingProgram, Action<DroneMetadata.Program.Behaviour> onComplete)
	{
		if (workingProgram.destination.id == "drone.behaviour.none")
		{
			this.CreatePicker<DroneMetadata.Program.Behaviour>("t.drone.pick_destination", this.metadata.pickDestinationIcon, this.FilterDestinations(workingProgram, this.metadata.destinations), this.metadata.onGuiButtonDestinationCue, onComplete);
			return;
		}
		onComplete(workingProgram.destination);
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x0002AF6C File Offset: 0x0002916C
	private DroneMetadata.Program.Behaviour[] FilterSources(DroneMetadata.Program workingProgram, DroneMetadata.Program.Behaviour[] allSrcs)
	{
		return (from s in allSrcs
		where s.isCompatible(workingProgram)
		select s).ToArray<DroneMetadata.Program.Behaviour>();
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x0002AFA0 File Offset: 0x000291A0
	private DroneMetadata.Program.Behaviour[] FilterDestinations(DroneMetadata.Program workingProgram, DroneMetadata.Program.Behaviour[] allDests)
	{
		return (from d in allDests
		where d.isCompatible(workingProgram) && d.id != workingProgram.source.id.Replace("source", "destination")
		select d).ToArray<DroneMetadata.Program.Behaviour>();
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0002AFD4 File Offset: 0x000291D4
	private void CreatePicker<T>(string title, Sprite titleIcon, T[] options, SECTR_AudioCue buttonCue, Action<T> onPicked) where T : DroneMetadata.Program.BaseComponent
	{
		if (this.pickerUI != null)
		{
			Destroyer.Destroy(this.pickerUI.gameObject, "DroneUI.SetProgramPicker");
		}
		this.pickerUI = UnityEngine.Object.Instantiate<GameObject>(this.metadata.droneUIProgramPicker.gameObject).GetComponent<DroneUIProgramPicker>();
		this.pickerUI.title.text = this.uiBundle.Get(title);
		this.pickerUI.icon.sprite = titleIcon;
		Button[] array = new Button[options.Length];
		for (int i = 0; i < options.Length; i++)
		{
			T option = options[i];
			DroneUIProgramButton droneUIProgramButton = UnityEngine.Object.Instantiate<GameObject>(this.metadata.droneUIProgramButton.gameObject, this.pickerUI.contentGrid).GetComponent<DroneUIProgramButton>().Init(option, null);
			droneUIProgramButton.button.onClick.AddListener(delegate()
			{
				SECTR_AudioSystem.Play(buttonCue, Vector3.zero, false);
				this.pickerUI.Close();
				onPicked(option);
			});
			array[i] = droneUIProgramButton.button;
			if (i == 0)
			{
				droneUIProgramButton.button.gameObject.AddComponent<InitSelected>();
			}
		}
		int num = Mathf.CeilToInt((float)array.Length / 6f);
		for (int j = 0; j < array.Length; j++)
		{
			int num2 = j / 6;
			int num3 = j % 6;
			Navigation navigation = array[j].navigation;
			navigation.mode = Navigation.Mode.Explicit;
			if (num2 > 0)
			{
				navigation.selectOnUp = array[(num2 - 1) * 6 + num3];
			}
			if (num2 < num - 1)
			{
				navigation.selectOnDown = array[Math.Min((num2 + 1) * 6 + num3, array.Length - 1)];
			}
			if (num3 > 0)
			{
				navigation.selectOnLeft = array[num2 * 6 + (num3 - 1)];
			}
			if (num3 < 5 && j < array.Length - 1)
			{
				navigation.selectOnRight = array[num2 * 6 + (num3 + 1)];
			}
			array[j].navigation = navigation;
		}
		DroneUIProgramPicker droneUIProgramPicker = this.pickerUI;
		droneUIProgramPicker.onDestroy = (BaseUI.OnDestroyDelegate)Delegate.Combine(droneUIProgramPicker.onDestroy, new BaseUI.OnDestroyDelegate(delegate()
		{
			if (SRSingleton<SceneContext>.Instance != null && this != null && this.gameObject != null)
			{
				this.ResetUI();
			}
		}));
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x060009AB RID: 2475 RVA: 0x0002B208 File Offset: 0x00029408
	private DroneMetadata metadata
	{
		get
		{
			return this.gadget.metadata;
		}
	}

	// Token: 0x040007FC RID: 2044
	public Transform programsParent;

	// Token: 0x040007FD RID: 2045
	public TMP_Text warningText;

	// Token: 0x040007FE RID: 2046
	public Button activateButton;

	// Token: 0x040007FF RID: 2047
	public Button resetButton;

	// Token: 0x04000800 RID: 2048
	private List<DroneUIProgram> programUIs = new List<DroneUIProgram>();

	// Token: 0x04000801 RID: 2049
	private bool programsChanged;

	// Token: 0x04000802 RID: 2050
	private const int GRID_COLUMNS = 6;

	// Token: 0x04000803 RID: 2051
	private DroneGadget gadget;

	// Token: 0x04000804 RID: 2052
	private DroneMetadata.Program[] programs;

	// Token: 0x04000805 RID: 2053
	private DroneUIProgramPicker pickerUI;

	// Token: 0x020001C3 RID: 451
	// (Invoke) Token: 0x060009B1 RID: 2481
	private delegate DroneMetadata.Program DeriveProgram(DroneMetadata.Program program);
}
