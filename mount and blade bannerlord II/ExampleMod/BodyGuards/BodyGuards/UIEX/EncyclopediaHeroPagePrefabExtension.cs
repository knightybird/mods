using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Bodyguards.UIEX
{
	// Token: 0x0200000A RID: 10
	[PrefabExtension("EncyclopediaHeroPage", "descendant::RichTextWidget[@Text='@InformationText']")]
	internal class EncyclopediaHeroPagePrefabExtension : PrefabExtensionInsertPatch
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00003820 File Offset: 0x00001A20
		public override InsertType Type
		{
			get
			{
				return InsertType.Append;
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003824 File Offset: 0x00001A24
		[PrefabExtensionInsertPatch.PrefabExtensionXmlNodesAttribute]
		public IEnumerable<XmlNode> GetNodes()
		{
			bool flag = this._nodes == null;
			if (flag)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml("<DiscardedRoot><ListPanel HorizontalAlignment='Center' HeightSizePolicy ='CoverChildren' WidthSizePolicy='CoverChildren' MarginTop='10'><Children><ButtonWidget DoNotPassEventsToChildren='true' WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='227' SuggestedHeight='40' MarginLeft='5' MarginRight='5' Brush='ButtonBrush2' HorizontalAlignment='Left' UpdateChildrenStates='true' Command.Click='ToggleBodyguard' IsVisible='@CanBeBodyguard'><Children><TextWidget WidthSizePolicy='StretchToParent' HeightSizePolicy='StretchToParent' Brush='Kingdom.GeneralButtons.Text' Text='@ToggleBodyguardActionName' /></Children></ButtonWidget><HintWidget DataSource='{ToggleBodyguardHint}' DoNotAcceptEvents='true' WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' Command.HoverBegin='ExecuteBeginHint' Command.HoverEnd='ExecuteEndHint' IsEnabled='false'/></Children></ListPanel></DiscardedRoot>");
				this._nodes = xmlDocument.DocumentElement.ChildNodes.Cast<XmlNode>();
			}
			return this._nodes;
		}

		// Token: 0x0400001C RID: 28
		private IEnumerable<XmlNode> _nodes;
	}
}
