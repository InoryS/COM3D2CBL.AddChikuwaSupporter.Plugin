using System;

namespace UnityObsoleteGui
{
	// Token: 0x02000016 RID: 22
	public class SliderEventArgs : EventArgs
	{
		// Token: 0x06000137 RID: 311 RVA: 0x0000F4F5 File Offset: 0x0000E4F5
		public SliderEventArgs(string name, float value)
		{
			this.Name = name;
			this.Value = value;
		}

		// Token: 0x04000118 RID: 280
		public string Name;

		// Token: 0x04000119 RID: 281
		public float Value;
	}
}
