using System;

namespace UnityObsoleteGui
{
	// Token: 0x02000019 RID: 25
	public class SelectEventArgs : EventArgs
	{
		// Token: 0x0600013A RID: 314 RVA: 0x0000F540 File Offset: 0x0000E540
		public SelectEventArgs(string name, int idx, string buttonName)
		{
			this.Name = name;
			this.Index = idx;
			this.ButtonName = buttonName;
		}

		// Token: 0x0400011E RID: 286
		public string Name;

		// Token: 0x0400011F RID: 287
		public int Index;

		// Token: 0x04000120 RID: 288
		public string ButtonName;
	}
}
