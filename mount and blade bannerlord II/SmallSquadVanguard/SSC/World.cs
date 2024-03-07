using System;
using System.Collections.Generic;
using SSC.misc;
using SSC.party;
using SSC.party.companions;
using SSC.world.castle_party;
using SSC.world.scout;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace SSC
{
	// Token: 0x0200000B RID: 11
	public static class World
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00003E50 File Offset: 0x00002050
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00003E57 File Offset: 0x00002057
		public static FieldProxy<bool> LargeSquadMode { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00003E5F File Offset: 0x0000205F
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00003E66 File Offset: 0x00002066
		public static FieldProxy<int> CompanionLimit { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00003E6E File Offset: 0x0000206E
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00003E75 File Offset: 0x00002075
		public static FieldProxy<int> PlayerStrength { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00003E7D File Offset: 0x0000207D
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00003E84 File Offset: 0x00002084
		public static FieldProxy<int> FirstPhaseTiers { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003E8C File Offset: 0x0000208C
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00003E93 File Offset: 0x00002093
		public static FieldProxy<int> BossPhaseTiers { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00003E9B File Offset: 0x0000209B
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00003EA2 File Offset: 0x000020A2
		public static FieldProxy<Hero> DesignatedScout { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00003EAA File Offset: 0x000020AA
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00003EB1 File Offset: 0x000020B1
		public static FieldProxy<Hero> DesignatedSurgeon { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00003EB9 File Offset: 0x000020B9
		// (set) Token: 0x0600007F RID: 127 RVA: 0x00003EC0 File Offset: 0x000020C0
		public static FieldProxy<Hero> DesignatedEngineer { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00003EC8 File Offset: 0x000020C8
		// (set) Token: 0x06000081 RID: 129 RVA: 0x00003ECF File Offset: 0x000020CF
		public static FieldProxy<Hero> DesignatedQuartermaster { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00003ED7 File Offset: 0x000020D7
		// (set) Token: 0x06000083 RID: 131 RVA: 0x00003EDE File Offset: 0x000020DE
		public static CastleParty CastleParty { get; set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000084 RID: 132 RVA: 0x00003EE8 File Offset: 0x000020E8
		// (remove) Token: 0x06000085 RID: 133 RVA: 0x00003F1C File Offset: 0x0000211C
		public static event World.WorldFlagsChange OnWorldFlagsChange;

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00003F4F File Offset: 0x0000214F
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00003F56 File Offset: 0x00002156
		public static HeroData heroData { get; private set; }

		// Token: 0x06000088 RID: 136 RVA: 0x00003F5E File Offset: 0x0000215E
		public static void setHeroData(HeroData data)
		{
			World.heroData = data;
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000089 RID: 137 RVA: 0x00003F68 File Offset: 0x00002168
		// (remove) Token: 0x0600008A RID: 138 RVA: 0x00003F9C File Offset: 0x0000219C
		public static event World.QueueAction OnQueueAction;

		// Token: 0x0600008B RID: 139 RVA: 0x00003FCF File Offset: 0x000021CF
		public static void queueAction(QueuedAction action)
		{
			World.QueueAction onQueueAction = World.OnQueueAction;
			if (onQueueAction == null)
			{
				return;
			}
			onQueueAction(action);
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600008C RID: 140 RVA: 0x00003FE4 File Offset: 0x000021E4
		// (remove) Token: 0x0600008D RID: 141 RVA: 0x00004018 File Offset: 0x00002218
		public static event World.ScoutReputationChangeRequested OnSetScoutReputation;

		// Token: 0x0600008E RID: 142 RVA: 0x0000404B File Offset: 0x0000224B
		public static void setScoutReputation(float reputation)
		{
			World.ScoutReputationChangeRequested onSetScoutReputation = World.OnSetScoutReputation;
			if (onSetScoutReputation == null)
			{
				return;
			}
			onSetScoutReputation(reputation);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000405D File Offset: 0x0000225D
		public static void setFlags(BitFlags flags)
		{
			World._flags = flags;
			World.WorldFlagsChange onWorldFlagsChange = World.OnWorldFlagsChange;
			if (onWorldFlagsChange == null)
			{
				return;
			}
			onWorldFlagsChange(World._flags);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004079 File Offset: 0x00002279
		public static void setFlag(Flags flag)
		{
			World._flags.setFlag(flag);
			World.WorldFlagsChange onWorldFlagsChange = World.OnWorldFlagsChange;
			if (onWorldFlagsChange == null)
			{
				return;
			}
			onWorldFlagsChange(World._flags);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000409A File Offset: 0x0000229A
		public static void unsetFlag(Flags flag)
		{
			World._flags.unsetFlag(flag);
			World.WorldFlagsChange onWorldFlagsChange = World.OnWorldFlagsChange;
			if (onWorldFlagsChange == null)
			{
				return;
			}
			onWorldFlagsChange(World._flags);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000040BB File Offset: 0x000022BB
		public static bool isFlagSet(Flags flag)
		{
			return World._flags.isFlagSet(flag);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000040C8 File Offset: 0x000022C8
		public static bool flagNotSet(Flags flag)
		{
			return !World._flags.isFlagSet(flag);
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000094 RID: 148 RVA: 0x000040D8 File Offset: 0x000022D8
		// (set) Token: 0x06000095 RID: 149 RVA: 0x000040DF File Offset: 0x000022DF
		public static int HideoutTiersAtStart { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000040E7 File Offset: 0x000022E7
		// (set) Token: 0x06000097 RID: 151 RVA: 0x000040EE File Offset: 0x000022EE
		public static int HideoutPlayerPartyCountAtStart { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000040F6 File Offset: 0x000022F6
		// (set) Token: 0x06000099 RID: 153 RVA: 0x000040FD File Offset: 0x000022FD
		public static int HideoutFirstFightMaximum { get; set; } = 20;

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00004105 File Offset: 0x00002305
		// (set) Token: 0x0600009B RID: 155 RVA: 0x0000410C File Offset: 0x0000230C
		public static Dictionary<Hero, int> BeingReleased { get; set; } = new Dictionary<Hero, int>();

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00004114 File Offset: 0x00002314
		// (set) Token: 0x0600009D RID: 157 RVA: 0x0000411B File Offset: 0x0000231B
		public static Dictionary<Hero, PartyBase> CaptorsHack { get; set; } = new Dictionary<Hero, PartyBase>();

		// Token: 0x0600009E RID: 158 RVA: 0x00004124 File Offset: 0x00002324
		public static int GetPlayerMaximumTroopCountForHideoutMission(MobileParty party)
		{
			float num = 14f;
			if (party.HasPerk(DefaultPerks.Tactics.SmallUnitTactics, false))
			{
				num += DefaultPerks.Tactics.SmallUnitTactics.PrimaryBonus;
			}
			return MathF.Round(num);
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004158 File Offset: 0x00002358
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x0000415F File Offset: 0x0000235F
		public static Action ConversationFinished { get; set; } = null;

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00004167 File Offset: 0x00002367
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x0000416E File Offset: 0x0000236E
		public static MobileParty ScoutMission_LedParty { get; set; }

		// Token: 0x060000A3 RID: 163 RVA: 0x00004178 File Offset: 0x00002378
		internal static bool leadingSubordinate(ScoutMission scoutData)
		{
			MobileParty mobileParty = scoutData.MobileParty;
			if (((mobileParty != null) ? mobileParty.ActualClan : null) != null)
			{
				IFaction mapFaction = scoutData.MobileParty.MapFaction;
				if (((mapFaction != null) ? mapFaction.Leader : null) != null)
				{
					bool flag = scoutData.MobileParty.ActualClan == Clan.PlayerClan;
					bool flag2 = scoutData.MobileParty.MapFaction.Leader == Hero.MainHero;
					return flag || flag2;
				}
			}
			return false;
		}

		// Token: 0x0400001E RID: 30
		public static readonly string MOD_TITLE = "Small Squad: Vanguard";

		// Token: 0x0400001F RID: 31
		public static readonly int SMALL_SQUAD_SIZE = 30;

		// Token: 0x04000020 RID: 32
		private static BitFlags _flags;

		// Token: 0x0200007A RID: 122
		// (Invoke) Token: 0x0600038C RID: 908
		public delegate void WorldFlagsChange(BitFlags newFlags);

		// Token: 0x0200007B RID: 123
		// (Invoke) Token: 0x06000390 RID: 912
		public delegate void QueueAction(QueuedAction action);

		// Token: 0x0200007C RID: 124
		// (Invoke) Token: 0x06000394 RID: 916
		public delegate void ScoutReputationChangeRequested(float reputation);
	}
}
