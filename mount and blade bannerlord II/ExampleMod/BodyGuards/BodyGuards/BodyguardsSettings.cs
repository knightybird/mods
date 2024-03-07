using System;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;

namespace Bodyguards
{
	// Token: 0x02000002 RID: 2
	internal sealed class BodyguardsSettings
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		private bool _defaults
		{
			get
			{
				return GlobalSettings<BodyguardsSettingsMCM>.Instance == null;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002052 File Offset: 0x00000252
		private BodyguardsSettingsMCM _mcm
		{
			get
			{
				return GlobalSettings<BodyguardsSettingsMCM>.Instance;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002059 File Offset: 0x00000259
		public int maxBodyguards
		{
			get
			{
				return this._defaults ? 5 : this._mcm.maxBodyguards;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002071 File Offset: 0x00000271
		public float maxBodyguardsPercent
		{
			get
			{
				return this._defaults ? 0.05f : this._mcm.maxBodyguardsPercent;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000208D File Offset: 0x0000028D
		public string preferredBodyguardType
		{
			get
			{
				return this._defaults ? "Cavalry" : this._mcm.preferredBodyguardType.SelectedValue;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020AE File Offset: 0x000002AE
		public bool useControllableFormation
		{
			get
			{
				return !this._defaults && this._mcm.useControllableFormation;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000020C6 File Offset: 0x000002C6
		public bool createBodyguardsAtBattleStart
		{
			get
			{
				return this._defaults || this._mcm.createBodyguardsAtBattleStart;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020DE File Offset: 0x000002DE
		public bool enableBodyguardsDuringSieges
		{
			get
			{
				return this._defaults || this._mcm.enableBodyguardsDuringSieges;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000020F6 File Offset: 0x000002F6
		public bool enableBodyguardsForAIGenerals
		{
			get
			{
				return !this._defaults && this._mcm.enableBodyguardsForAIGenerals;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000210E File Offset: 0x0000030E
		public bool useSpecificTroop
		{
			get
			{
				return !this._defaults && this._mcm.useSpecificTroop;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002126 File Offset: 0x00000326
		public string specificTroopName
		{
			get
			{
				return this._defaults ? "" : this._mcm.specificTroopName;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002142 File Offset: 0x00000342
		public bool companionGuardMode
		{
			get
			{
				return !this._defaults && this._mcm.companionGuardMode;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000215A File Offset: 0x0000035A
		public bool doNotBackfill
		{
			get
			{
				return !this._defaults && this._mcm.doNotBackfill;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002172 File Offset: 0x00000372
		public bool onlyUseManuallySelectedCompanions
		{
			get
			{
				return this._defaults || this._mcm.onlyUseManuallySelectedCompanions;
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000218C File Offset: 0x0000038C
		public FormationClass getDesiredTroopFormationClass()
		{
			string preferredBodyguardType = this.preferredBodyguardType;
			string a = preferredBodyguardType;
			FormationClass result;
			if (!(a == "Infantry"))
			{
				if (!(a == "Ranged"))
				{
					if (!(a == "Cavalry"))
					{
						if (!(a == "Horse Archer"))
						{
							result = FormationClass.Cavalry;
						}
						else
						{
							result = FormationClass.HorseArcher;
						}
					}
					else
					{
						result = FormationClass.Cavalry;
					}
				}
				else
				{
					result = FormationClass.Ranged;
				}
			}
			else
			{
				result = FormationClass.Infantry;
			}
			return result;
		}
	}
}
