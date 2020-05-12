using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace SpiritedAway.NPCs
{
	internal class Susuwatari : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Susuwatari");
			Main.npcFrameCount[npc.type] = 6;
			Main.npcCatchable[npc.type] = true;
		}

		public override void SetDefaults()
		{
			npc.lavaImmune = true;

			npc.width = 34;
			npc.height = 34;
			npc.aiStyle = 67;
			npc.damage = 0;
			npc.defense = 5;
			npc.lifeMax = 50;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.npcSlots = 0.5f;
			npc.noGravity = false;
			npc.catchItem = (short)ItemType<SusuwatariItem>();
			this.aiType = NPCID.GoldfishWalker;
			this.animationType = NPCID.GoldfishWalker;
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return true;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return true;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.Overworld.Chance;
		}
	}

	internal class SusuwatariItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Susuwatari");
		}

		public override void SetDefaults()
		{
			item.useStyle = 1;
			item.autoReuse = true;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.maxStack = 999;
			item.consumable = true;
			//item.noUseGraphic = true;
			item.width = 34;
			item.height = 34;
			item.makeNPC = (short) NPCType<Susuwatari>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}