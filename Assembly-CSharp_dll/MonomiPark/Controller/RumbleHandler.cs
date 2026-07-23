using System;

namespace MonomiPark.Controller
{
	// Token: 0x02000A36 RID: 2614
	public interface RumbleHandler
	{
		// Token: 0x06004626 RID: 17958
		void EnableRumble();

		// Token: 0x06004627 RID: 17959
		void DisableRumble();

		// Token: 0x06004628 RID: 17960
		bool IsRumbleEnabled();

		// Token: 0x06004629 RID: 17961
		void AddRumble(Rumble rumble);
	}
}
