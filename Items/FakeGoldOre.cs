using System.Collections.Generic;
using Terraria.ModLoader;

namespace SpiritedAway.Items
{
	public class FakeGoldOre : ModItem
	{
		// TODO(LOGIX): When in the shop, should "Can be placed" and "Material" show?
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gold Ore");
			Tooltip.SetDefault("Smells an awful lot like mud!\nCan be placed\nMaterial");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.value = 1;
			item.rare = 0;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			bool has_buy_price = false;
			foreach (TooltipLine tt in tooltips)
			{
				if (tt.text.Contains("Buy price"))
				{
					has_buy_price = true;
					break;
				}
			}

			if (has_buy_price)
			{
				foreach (TooltipLine tt in tooltips)
				{
					if (tt.text.Contains("mud"))
					{
						tooltips.Remove(tt);
						break;
					}
				}
			}
		}
	}
}