using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace Bodyguards
{
	// Token: 0x02000003 RID: 3
	internal sealed class BodyguardsSettingsMCM : AttributeGlobalSettings<BodyguardsSettingsMCM>
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000021F7 File Offset: 0x000003F7
		public override string Id
		{
			get
			{
				return "motes_Bodyguards_v1";
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000021FE File Offset: 0x000003FE
		public override string DisplayName
		{
			get
			{
				return "Bodyguards";
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002205 File Offset: 0x00000405
		public override string FolderName
		{
			get
			{
				return "Bodyguards";
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000220C File Offset: 0x0000040C
		public override string FormatType
		{
			get
			{
				return "json2";
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002213 File Offset: 0x00000413
		// (set) Token: 0x06000016 RID: 22 RVA: 0x0000221B File Offset: 0x0000041B
		[SettingPropertyInteger("{=BGBwQmIeC1F}Max Bodyguards", 0, 100, "Guards", Order = 0, RequireRestart = false, HintText = "{=BGCZjzuFSq9}Absolute maximum number of bodyguards")]
		[SettingPropertyGroup("{=BGXGxv4zzDs}General Settings", GroupOrder = 0)]
		public int maxBodyguards { get; set; } = 5;

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002224 File Offset: 0x00000424
		// (set) Token: 0x06000018 RID: 24 RVA: 0x0000222C File Offset: 0x0000042C
		[SettingPropertyFloatingInteger("{=BGV58IAHzbY}Percentage of troops to be guards", 0f, 1.00f, "#0%", Order = 1, RequireRestart = false, HintText = "{=BGe87VFzNgy}Max bodyguards will be the smaller of this and the preceeding value")]
		[SettingPropertyGroup("{=BGXGxv4zzDs}General Settings", GroupOrder = 0)]
		public float maxBodyguardsPercent { get; set; } = 0.05f;

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002235 File Offset: 0x00000435
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000223D File Offset: 0x0000043D
		[SettingPropertyDropdown("{=BGnknessElf}Preferred Bodyguard Type", Order = 2, RequireRestart = false, HintText = "{=BGBJlUSVExR}Troops of this type will be selected to be your bodyguards. If not enough are available, your strongest troops which are of the same mounted status as you will be chosen.")]
		[SettingPropertyGroup("{=BGXGxv4zzDs}General Settings", GroupOrder = 0)]
		public Dropdown<string> preferredBodyguardType { get; set; } = new Dropdown<string>(new string[]
		{
			"Infantry",
			"Ranged",
			"Cavalry",
			"Horse Archer"
		}, 2);

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002246 File Offset: 0x00000446
		// (set) Token: 0x0600001C RID: 28 RVA: 0x0000224E File Offset: 0x0000044E
		[SettingPropertyBool("{=BG2G38WRNqQ}Use Controllable Formation", Order = 3, RequireRestart = false, HintText = "{=BGNMVwi2cTE}Places bodyguards in a formation you can control. The formation will still use the bodyguard AI but you can override it with direct commands. You can also do things like form a circle around yourself.")]
		[SettingPropertyGroup("{=BGXGxv4zzDs}General Settings", GroupOrder = 0)]
		public bool useControllableFormation { get; set; } = false;

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002257 File Offset: 0x00000457
		// (set) Token: 0x0600001E RID: 30 RVA: 0x0000225F File Offset: 0x0000045F
		[SettingPropertyBool("{=BGssGTQgz5x}Create Bodyguards At Battle Start", Order = 4, RequireRestart = false, HintText = "{=BG6BSOfzno6}If you disable this you can manually create the bodyguards with CTRL+F1.")]
		[SettingPropertyGroup("{=BGXGxv4zzDs}General Settings", GroupOrder = 0)]
		public bool createBodyguardsAtBattleStart { get; set; } = true;

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002268 File Offset: 0x00000468
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002270 File Offset: 0x00000470
		[SettingPropertyBool("{=BGGQDvql7Ut}Enable Bodyguards During Sieges", Order = 5, RequireRestart = false, HintText = "")]
		[SettingPropertyGroup("{=BGXGxv4zzDs}General Settings", GroupOrder = 0)]
		public bool enableBodyguardsDuringSieges { get; set; } = true;

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002279 File Offset: 0x00000479
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002281 File Offset: 0x00000481
		[SettingPropertyBool("{=BGzcahscldo}Enable Bodyguards for AI Generals", Order = 6, RequireRestart = false, HintText = "{=BGNETdgxESM}Adds up to 5 bodyguards for AI generals. A General is the leader of a party or an Army. In the case of multi-army battles or multi-party battles (that aren't in the same army), you may see multiple bodyguard groups. Even without this mod an AI General may have bodyguards that protect them until the initial infantry clash.")]
		[SettingPropertyGroup("{=BGXGxv4zzDs}General Settings", GroupOrder = 0)]
		public bool enableBodyguardsForAIGenerals { get; set; } = false;

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000023 RID: 35 RVA: 0x0000228A File Offset: 0x0000048A
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002292 File Offset: 0x00000492
		[SettingPropertyBool("{=BGPD3v1g7yd}Use Specific Troop", RequireRestart = false, IsToggle = true)]
		[SettingPropertyGroup("{=BGJeLFroPNR}Use Specific Troop", GroupOrder = 1)]
		public bool useSpecificTroop { get; set; } = false;

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000025 RID: 37 RVA: 0x0000229B File Offset: 0x0000049B
		// (set) Token: 0x06000026 RID: 38 RVA: 0x000022A3 File Offset: 0x000004A3
		[SettingPropertyText("{=BG9wyWcIzFO}Specific Troop Name", -1, true, "", Order = 0, RequireRestart = false, HintText = "{=BGV3UWDhM4W}Type the troop type you'd like as guards exactly as it appears in game. For instance 'Imperial Legionary' or 'Imperial Elite Cataphract'. The preferred bodyguard type above doesn't need to match.")]
		[SettingPropertyGroup("{=BGJeLFroPNR}Use Specific Troop", GroupOrder = 1)]
		public string specificTroopName { get; set; } = "";

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000022AC File Offset: 0x000004AC
		// (set) Token: 0x06000028 RID: 40 RVA: 0x000022B4 File Offset: 0x000004B4
		[SettingPropertyBool("{=BGqiXcyCtd4}Companion Guards", IsToggle = true, RequireRestart = false, HintText = "{=BGEAhaDE7W1}Add your companions as guards, backfill with regular troops as needed.")]
		[SettingPropertyGroup("{=BGqpnc5cIen}Companion Guards", GroupOrder = 2)]
		public bool companionGuardMode { get; set; } = false;

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000029 RID: 41 RVA: 0x000022BD File Offset: 0x000004BD
		// (set) Token: 0x0600002A RID: 42 RVA: 0x000022C5 File Offset: 0x000004C5
		[SettingPropertyBool("{=BGK5iBS1R2N}Do Not Backfill", Order = 1, RequireRestart = false, HintText = "{=BGQl9piz9mz}Only use companions as bodyguards--do not backfill with regular troops to meet the bodyguard cap.")]
		[SettingPropertyGroup("{=BGqpnc5cIen}Companion Guards", GroupOrder = 2)]
		public bool doNotBackfill { get; set; } = false;

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000022CE File Offset: 0x000004CE
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000022D6 File Offset: 0x000004D6
		[SettingPropertyBool("{=BGWDsBUzyJi}Only Use Manually Selected Companions", Order = 2, RequireRestart = false, HintText = "{=BGdlRAJxjPb}Only use companions that have been manually selected to be bodyguards through the button on their encyclopedia page. Click/Right Click on companion pictures in the clan or party screens to quickly get to their encyclopedia page.")]
		[SettingPropertyGroup("{=BGqpnc5cIen}Companion Guards", GroupOrder = 2)]
		public bool onlyUseManuallySelectedCompanions { get; set; } = true;
	}
}
