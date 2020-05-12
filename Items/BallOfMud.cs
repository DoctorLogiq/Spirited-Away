using Terraria.ModLoader;

namespace SpiritedAway.Items
{
	public class BallOfMud : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ball Of Mud");
			Tooltip.SetDefault("Material");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.value = 0;
			item.rare = 0;
		}
	}
}