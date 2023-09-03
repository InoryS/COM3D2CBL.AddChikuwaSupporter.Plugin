using System;

namespace UnityObsoleteGui
{
	// Token: 0x02000017 RID: 23
	public class ButtonEventArgs : EventArgs
	{
		// Token: 0x06000138 RID: 312 RVA: 0x0000F50E File Offset: 0x0000E50E
		public ButtonEventArgs(string name, string buttonName)
		{
			this.Name = name;
			this.ButtonName = buttonName;
		}

		// Token: 0x0400011A RID: 282
		public string Name;

		// Token: 0x0400011B RID: 283
		public string ButtonName;
	}
}
