using System;
using UnityEngine;

namespace Sentry
{
	// Token: 0x020008A0 RID: 2208
	[Serializable]
	public class Context
	{
		// Token: 0x0600304A RID: 12362 RVA: 0x000BE22C File Offset: 0x000BC42C
		public Context()
		{
			this.os = new OperatingSystem
			{
				name = SystemInfo.operatingSystem
			};
			this.device = new Device();
			switch (Input.deviceOrientation)
			{
			case DeviceOrientation.Portrait:
			case DeviceOrientation.PortraitUpsideDown:
				this.device.orientation = "portrait";
				break;
			case DeviceOrientation.LandscapeLeft:
			case DeviceOrientation.LandscapeRight:
				this.device.orientation = "landscape";
				break;
			}
			string deviceModel = SystemInfo.deviceModel;
			if (deviceModel != "n/a" && deviceModel != "System Product Name (System manufacturer)")
			{
				this.device.model = deviceModel;
			}
			this.device.battery_level = SystemInfo.batteryLevel * 100f;
			this.device.battery_status = SystemInfo.batteryStatus.ToString();
			if (SystemInfo.systemMemorySize != 0)
			{
				this.device.memory_size = (long)SystemInfo.systemMemorySize * 1048576L;
			}
			this.device.device_type = SystemInfo.deviceType.ToString();
			this.device.cpu_description = SystemInfo.processorType;
			this.device.simulator = false;
			this.gpu = new Gpu
			{
				id = SystemInfo.graphicsDeviceID,
				name = SystemInfo.graphicsDeviceName,
				vendor_id = SystemInfo.graphicsDeviceVendorID,
				vendor_name = SystemInfo.graphicsDeviceVendor,
				memory_size = SystemInfo.graphicsMemorySize,
				multi_threaded_rendering = SystemInfo.graphicsMultiThreaded,
				npot_support = SystemInfo.npotSupport.ToString(),
				version = SystemInfo.graphicsDeviceVersion,
				api_type = SystemInfo.graphicsDeviceType.ToString()
			};
			this.app = new App();
			this.app.app_start_time = DateTimeOffset.UtcNow.AddSeconds((double)(-(double)Time.realtimeSinceStartup)).ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
			if (Debug.isDebugBuild)
			{
				this.app.build_type = "debug";
				return;
			}
			this.app.build_type = "release";
		}

		// Token: 0x04002E56 RID: 11862
		public App app;

		// Token: 0x04002E57 RID: 11863
		public Gpu gpu;

		// Token: 0x04002E58 RID: 11864
		public OperatingSystem os;

		// Token: 0x04002E59 RID: 11865
		public Device device;
	}
}
