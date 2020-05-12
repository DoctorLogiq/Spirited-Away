using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SpiritedAway.Items
{
	public class FakeTitaniumOre : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Titanium Ore");
			Tooltip.SetDefault("Smells an awful lot like mud!\nCan be placed\nMaterial");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.value = 1;
			item.rare = 3;
		}

		public override void UpdateInventory(Player player)
		{
			ModifyTooltips(new List<TooltipLine>());
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