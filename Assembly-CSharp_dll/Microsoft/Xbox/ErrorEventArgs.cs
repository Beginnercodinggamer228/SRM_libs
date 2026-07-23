using System;

namespace Microsoft.Xbox
{
	// Token: 0x02000BD2 RID: 3026
	public class ErrorEventArgs : EventArgs
	{
		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x060056A0 RID: 22176 RVA: 0x00106514 File Offset: 0x00104714
		// (set) Token: 0x060056A1 RID: 22177 RVA: 0x0010651C File Offset: 0x0010471C
		public string ErrorCode { get; private set; }

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x060056A2 RID: 22178 RVA: 0x00106525 File Offset: 0x00104725
		// (set) Token: 0x060056A3 RID: 22179 RVA: 0x0010652D File Offset: 0x0010472D
		public string ErrorMessage { get; private set; }

		// Token: 0x060056A4 RID: 22180 RVA: 0x00106536 File Offset: 0x00104736
		public ErrorEventArgs(string errorCode, string errorMessage)
		{
			this.ErrorCode = errorCode;
			this.ErrorMessage = errorMessage;
		}
	}
}
