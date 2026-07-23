using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B2 RID: 434
public class DroneStationProgramPreview : SRBehaviour
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000929 RID: 2345 RVA: 0x0002910C File Offset: 0x0002730C
	// (set) Token: 0x0600092A RID: 2346 RVA: 0x00029114 File Offset: 0x00027314
	public DroneGadget gadget { get; private set; }

	// Token: 0x0600092B RID: 2347 RVA: 0x0002911D File Offset: 0x0002731D
	public void Start()
	{
		this.gadget = base.GetComponentInParent<DroneGadget>();
		this.gadget.onProgramsChanged += this.OnProgramsChanged;
		this.OnProgramsChanged(this.gadget.programs);
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x00029154 File Offset: 0x00027354
	private void OnProgramsChanged(DroneMetadata.Program[] programs)
	{
		Sprite sprite = programs[this.programIndex].IsComplete() ? programs[this.programIndex].target.GetImage() : this.gadget.metadata.imageNone;
		this.image.enabled = (sprite != null);
		this.image.sprite = sprite;
	}

	// Token: 0x040007B2 RID: 1970
	[Tooltip("Index into DroneGadget.programs to display.")]
	public int programIndex;

	// Token: 0x040007B3 RID: 1971
	[Tooltip("Image to update with the program preview.")]
	public Image image;
}
