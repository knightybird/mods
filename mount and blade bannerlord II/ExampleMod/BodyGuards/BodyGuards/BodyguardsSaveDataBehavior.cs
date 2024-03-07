using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Bodyguards
{
	// Token: 0x02000006 RID: 6
	internal class BodyguardsSaveDataBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000049 RID: 73 RVA: 0x000033DF File Offset: 0x000015DF
		internal bool IsBodyguard(Hero hero)
		{
			return this._chosenBodyguards.Contains(hero);
		}

        internal bool IsBodyguard2(Hero hero)
        {
            return this._chosenBodyguards2.Contains(hero);
        }

        // Token: 0x0600004A RID: 74 RVA: 0x000033ED File Offset: 0x000015ED
        internal void AddBodyguard(Hero hero)
		{
			this._chosenBodyguards.Add(hero);
		}

        internal void AddBodyguard2(Hero hero)
        {
            this._chosenBodyguards2.Add(hero);
        }

        // Token: 0x0600004B RID: 75 RVA: 0x000033FC File Offset: 0x000015FC
        internal void RemoveBodyguard(Hero hero)
		{
			this._chosenBodyguards.Remove(hero);
		}

        internal void RemoveBodyguard2(Hero hero)
        {
            this._chosenBodyguards2.Remove(hero);
        }

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x0600004C RID: 76 RVA: 0x0000340B File Offset: 0x0000160B
        internal IEnumerable<Hero> GetBodyguards
		{
			get
			{
				return this._chosenBodyguards.AsEnumerable<Hero>();
			}
		}

        internal IEnumerable<Hero> GetBodyguards2
        {
            get
            {
                return this._chosenBodyguards2.AsEnumerable<Hero>();
            }
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00003418 File Offset: 0x00001618
        public override void RegisterEvents()
		{
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000341B File Offset: 0x0000161B
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Hero>>("_chosenBodyguards", ref this._chosenBodyguards);
            dataStore.SyncData<List<Hero>>("_chosenBodyguards2", ref this._chosenBodyguards2);
        }

        // Token: 0x04000015 RID: 21
        private List<Hero> _chosenBodyguards = new List<Hero>();
        private List<Hero> _chosenBodyguards2 = new List<Hero>();

        // Token: 0x0200000E RID: 14
        public class BodyguardsSaveDataTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x0600006F RID: 111 RVA: 0x00003B06 File Offset: 0x00001D06
			public BodyguardsSaveDataTypeDefiner() : base(513554635)
			{
			}

			// Token: 0x06000070 RID: 112 RVA: 0x00003B15 File Offset: 0x00001D15
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(List<Hero>));
			}
		}
	}
}
