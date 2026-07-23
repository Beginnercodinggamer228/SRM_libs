using System;

namespace Microsoft.Xbox
{
	// Token: 0x02000BD3 RID: 3027
	public class GameSaveLoadedArgs : EventArgs
	{
		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x060056A5 RID: 22181 RVA: 0x0010654C File Offset: 0x0010474C
		// (set) Token: 0x060056A6 RID: 22182 RVA: 0x00106554 File Offset: 0x00104754
		public byte[] Data { get; private set; }

		// Token: 0x060056A7 RID: 22183 RVA: 0x0010655D File Offset: 0x0010475D
		public GameSaveLoadedArgs(byte[] data)
		{
			this.Data = data;
		}
	}
}
