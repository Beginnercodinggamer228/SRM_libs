using System;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class EchoNoteGameMetadata : ScriptableObject
{
	// Token: 0x0400084E RID: 2126
	[Tooltip("Instrument this note set corresponds to.")]
	public InstrumentModel.Instrument instrument;

	// Token: 0x0400084F RID: 2127
	[Tooltip("Translation key of the name of the instrument.")]
	public string xlateKey;

	// Token: 0x04000850 RID: 2128
	[Tooltip("Icon for this instrument.")]
	public Sprite icon;

	// Token: 0x04000851 RID: 2129
	[Tooltip("SFX cue to use with echo notes.")]
	public SECTR_AudioCue cue;
}
