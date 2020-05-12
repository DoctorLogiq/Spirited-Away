using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using SpiritedAway.Dusts;
using SpiritedAway.Items;
using SpiritedAway.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace SpiritedAway.NPCs
{
	[AutoloadHead]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class NoFace : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spirit");
			Main.npcFrameCount[npc.type]             = 25;
			NPCID.Sets.ExtraFramesCount[npc.type]    = 9;
			NPCID.Sets.AttackFrameCount[npc.type]    = 4;
			NPCID.Sets.DangerDetectRange[npc.type]   = 700;
			NPCID.Sets.AttackType[npc.type]          = 0;
			NPCID.Sets.AttackTime[npc.type]          = 90;
			NPCID.Sets.AttackAverageChance[npc.type] = 30;
			NPCID.Sets.HatOffsetY[npc.type]          = 0;
		}

		public override void SetDefaults()
		{
			npc.townNPC         = true;
			npc.friendly        = true;
			npc.width           = 18;
			npc.height          = 40;
			npc.aiStyle         = 7;
			npc.damage          = 10;
			npc.defense         = 15;
			npc.lifeMax         = 250;
			npc.HitSound        = SoundID.NPCHit1;
			npc.DeathSound      = SoundID.NPCDeath1;
			npc.knockBackResist = .5F;
			this.animationType  = NPCID.Guide;
		}

		public const   double     DespawnTimeOfDay = 48600.0; // 6PM
		public static  double     SpawnTime        = double.MaxValue;
		public static  List<Item> ShopItems        = new List<Item>();
		private static Offer      CurrentOffer     = Offer.FakeGoldOre;
		private static bool       IsBeingCreepy;
		private        bool       ChatButtonSet;
		private        string     ChatButtonText = "Shop";

		#region Spawning
		
		public static NPC FindNPC(int type) => Main.npc.FirstOrDefault(npc => npc.type == type && npc.active);

		public static NPC Find()
		{
			return FindNPC(NPCType<NoFace>());
		}

		public static bool Exists()
		{
			return Find() != null;
		}

		public static void Despawn(NPC noFace)
		{
			SpiritedAway.Announce($"{noFace.FullName} has mysteriously vanished when nobody was looking!");
				
			noFace.active  = false;
			noFace.netSkip = -1;
			noFace.life    = 0;
			noFace         = null;
		}

		public static void SpawnOrRespawn()
		{
			NPC no_face = Find();
			if (no_face != null)
			{
				Despawn(no_face);
			}

			int new_no_face = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, NPCType<NoFace>(), 1);
			no_face           = Main.npc[new_no_face];
			no_face.homeless  = true;
			no_face.direction = Main.spawnTileX >= WorldGen.bestX ? -1 : 1;
			no_face.netUpdate = true;
			ShopItems         = CreateNewShop();

			// Prevent any more spawns for today
			SpawnTime = double.MaxValue;
				
			// Announce his arrival
			SpiritedAway.Announce($"A strange ghostly figure has appeared!");
		}

		public static void UpdateNoFaceMerchant()
		{
			NPC no_face = Find(); // Find NoFace if one is spawned in the world
			
			// If it is night or past the despawn time for the NPC, and he is not on the screen, despawn him
			if (no_face != null && (!Main.dayTime || Main.time >= DespawnTimeOfDay) && !IsNPCOnScreen(no_face.Center))
			{
				Despawn(no_face);
			}

			// Stop the NPC from spawning on this day if he stayed overnight
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (Main.dayTime && Main.time == 0)
			{
				if (no_face == null && Main.rand.NextBool(4 /* 25% chance */))
				{
					SpawnTime = GetRandomSpawnTime(5400, 8100);
				}
				else
				{
					SpawnTime = double.MaxValue;
				}
			}
			
			// Spawn if the conditions are met
			if (no_face == null && CanSpawnNow())
			{
				int new_no_face = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, NPCType<NoFace>(), 1);
				no_face           = Main.npc[new_no_face];
				no_face.homeless  = true;
				no_face.direction = Main.spawnTileX >= WorldGen.bestX ? -1 : 1;
				no_face.netUpdate = true;
				ShopItems         = CreateNewShop();

				// Prevent any more spawns for today
				SpawnTime = double.MaxValue;
				
				// Announce his arrival
				SpiritedAway.Announce($"A strange ghostly figure has appeared!");
			}
		}

		private static bool IsNPCOnScreen(Vector2 center)
		{
			int width  = NPC.sWidth  + NPC.safeRangeX * 2;
			int height = NPC.sHeight + NPC.safeRangeY * 2;
			Rectangle npc_screen_rect = new Rectangle((int)center.X - width / 2, (int)center.Y - height / 2, width, height);
			foreach (Player player in Main.player)
			{
				// If any player is close enough to the traveling merchant, it will prevent the npc from despawning
				if (player.active && player.getRect().Intersects(npc_screen_rect)) return true;
			}
			return false;
		}

		private static double GetRandomSpawnTime(int min, int max)
		{
			return (max - min) * Main.rand.NextDouble() + min;
		}

		private static bool CanSpawnNow()
		{
			// Shouldn't spawn if there's an invasion
			if (Main.eclipse || Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
				return false;

			// Shouldn't spawn if the sundial is active
			if (Main.fastForwardTime)
				return false;

			// Should spawn if it's daytime and between spawn and despawn times
			return Main.dayTime && Main.time >= SpawnTime && Main.time < DespawnTimeOfDay;
		}
		
		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			// Set to false because we manually spawn this NPC
			return false;
		}
		
		#endregion
		
		#region Shop
		
		/// <summary>
		/// Returns a string of text to display whenever a player talks to the NPC.
		/// We will use this to randomize his trades (for now). TODO(LOGIX): Alter this behaviour.
		/// </summary>
		public override string GetChat()
		{
			// 33% chance to be a creepy b****rd!
			IsBeingCreepy = Main.rand.NextFloat(10) <= 3.33F;
			
			// If he isn't being creepy, setup his trades
			if (!IsBeingCreepy)
			{
				// A 1 in 30 chance to offer a real material
				bool is_real = Main.rand.Next(30) == 1;
				
				// Select the correct offer based on whether or not it's a real deal
				switch (Main.rand.Next(4))
				{
					case 0:
						CurrentOffer = is_real ? Offer.RealGoldOre : Offer.FakeGoldOre;
						break;
					case 1:
						CurrentOffer = is_real ? Offer.RealSilverOre : Offer.FakeSilverOre;
						break;
					case 2:
						CurrentOffer = is_real ? Offer.RealPlatinumOre : Offer.FakePlatinumOre;
						break;
					default:
						CurrentOffer = is_real ? Offer.RealTitaniumOre : Offer.FakeTitaniumOre;
						break;
				}

				// Display a randomised string of text to the player, and include the name of the offer in some of these.
				// Note that it won't show "real" or "fake" in the names.
				switch (Main.rand.Next(5))
				{
					default: case 0:
						return $"No-Face gestures, offering what appears to be... {CurrentOffer.GetOfferName()}?";
					case 1:
						return $"No-Face holds his hands out, cups them, and as if by magic, {CurrentOffer.GetOfferName()} starts to appear, piling up in his hands and spilling onto the floor!";
					case 2:
						return $"No-Face appears to smile, and shows you a handful of {CurrentOffer.GetOfferName()}";
					case 3:
						return "You do what you can to tolerate his terrible stench, and approach No-Face to find out what he has for you this time.";
					case 4:
						return $"It looks like he... has something for you? Better come closer, take a look! Surely it's safe?";
				}
			}
			else // if IsBeingCreepy = true
			{
				switch (Main.rand.Next(4))
				{
					default:
						return "No-Face floats there, perfectly still and silent. You feel uneasy in his presence, and decide that maybe " +
						       "it's better just to back away... slowly!";
					case 1:
						return "...";
					case 2:
						return "You could've sworn you just saw No-Face nibbling on something, but he was facing the other way. " +
						       "Perhaps you should leave him be.";
					case 3:
						return "No-Face is fading in and out. Perhaps he is unwell.";
				}
			}
		}
		
		/// <summary>
		/// Returns a list of items. Most of these items will be balls of mud because No-Face is freakin'
		/// weird, but some of them will be the actual trades (I call them trades loosely).
		/// </summary>
		private static List<Item> CreateNewShop()
		{
			if (IsBeingCreepy)
				return new List<Item>();
			
			// Create the item ID list
			List<int> itemIDs = new List<int>();
			
			// Populate it with item IDs (looping through each slot in the inventory)
			for (int y = 0; y < 4; ++y)
			{
				for (int x = 0; x < 10; ++x)
				{
					if (Main.rand.Next(10) == 0)
						itemIDs.Add(CurrentOffer.GetItemType());
					else
						itemIDs.Add(ItemType<BallOfMud>());	
				}
			}
			
			// Now create the actual ITEM list, and use the IDs
			List<Item> items = new List<Item>();
			foreach (int itemID in itemIDs)
			{
				Item item = new Item();
				item.SetDefaults(itemID);
				items.Add(item);
			}

			return items;
		}
		
		/// <summary>
		/// Sets up the shop, populating the slots with the available items from CreateNewShop().
		/// </summary>
		public override void SetupShop(Chest shop, ref int nextSlot)
		{
			ShopItems = CreateNewShop();
			
			foreach (Item item in ShopItems)
			{
				if (item == null || item.type == 0)
					continue;
				
				shop.item[nextSlot].SetDefaults(item.type);
				nextSlot++;
			}
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			// The default "shop" button is: Language.GetTextValue("LegacyInterface.28");

			if (IsBeingCreepy)
			{
				return;
			}
			
			if (!this.ChatButtonSet)
			{
				switch (Main.rand.Next(4))
				{
					default:
						this.ChatButtonText = "Attempt to barter";
						break;
					case 1:
						this.ChatButtonText = "Approach cautiously";
						break;
					case 2:
						this.ChatButtonText = "Poke with a stick";
						break;
					case 3:
						this.ChatButtonText = "Encroach other's clearance";
						break;
				}
				
				this.ChatButtonSet = true;
			}

			button = this.ChatButtonText;
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			this.ChatButtonSet = false;
			
			if (firstButton && !IsBeingCreepy)
				shop = true;
		}
		
		#endregion

		#region Serialization
		
		private const string SerializeKeySpawnTime = "spawnTime";
		// TODO(LOGIX): Serialize CurrentOffer?
		private const string SerializeKeyShopItems = "shopItems";

		public static TagCompound Save()
		{
			return new TagCompound()
			{
					[SerializeKeySpawnTime] = SpawnTime,
					[SerializeKeyShopItems] = ShopItems
			};
		}

		public static void Load(TagCompound tag)
		{
			SpawnTime = tag.GetDouble(SerializeKeySpawnTime);
			ShopItems = tag.Get<List<Item>>(SerializeKeyShopItems);
		}
		
		#endregion

		#region Other
		
		public override void HitEffect(int hitDirection, double damage)
		{
			int num = npc.life > 0 ? 1 : 5;
			for (int i = 0; i < num; i++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, DustType<Spoopy>());
			}
		}

		public override void DrawEffects(ref Color drawColor)
		{
			if (Main.rand.Next(7) == 0)
				Dust.NewDust(npc.position, npc.width, (int)((npc.height * .5F) + (npc.height * .25F)), DustType<Spoopy>(), 0f, .4f);
			
			base.DrawEffects(ref drawColor);
		}

		public override string TownNPCName()
		{
			return "No-Face";
		}

		public override void AI()
		{
			npc.homeless = true;
		}

		public override void NPCLoot()
		{
			Item.NewItem(npc.getRect(), ItemType<BallOfMud>(), Main.rand.Next(5));
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 5;
			knockback = 6F;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileType<MudBall>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 12F;
			randomOffset = 2F;
		}
		
		#endregion
		
		internal enum Offer
		{
			FakeGoldOre,
			RealGoldOre,
			FakeSilverOre,
			RealSilverOre,
			FakePlatinumOre,
			RealPlatinumOre,
			FakeTitaniumOre,
			RealTitaniumOre
		}
	}
	
	internal static class NoFaceOfferExtensions
	{
		internal static int GetItemType(this NoFace.Offer offer)
		{
			switch (offer)
			{
				default:
				case NoFace.Offer.FakeGoldOre:
					return ItemType<FakeGoldOre>();
				case NoFace.Offer.RealGoldOre:
					return ItemID.GoldOre;
				
				case NoFace.Offer.FakeSilverOre:
					return ItemType<FakeSilverOre>();
				case NoFace.Offer.RealSilverOre:
					return ItemID.SilverOre;
				
				case NoFace.Offer.FakePlatinumOre:
					return ItemType<FakePlatinumOre>();
				case NoFace.Offer.RealPlatinumOre:
					return ItemID.PlatinumOre;
				
				case NoFace.Offer.FakeTitaniumOre:
					return ItemType<FakeTitaniumOre>();
				case NoFace.Offer.RealTitaniumOre:
					return ItemID.TitaniumOre;
			}
		}

		internal static string GetOfferName(this NoFace.Offer offer)
		{
			switch (offer)
			{
				default:
				case NoFace.Offer.FakeGoldOre:
				case NoFace.Offer.RealGoldOre:
					return "gold";
				case NoFace.Offer.FakeSilverOre:
				case NoFace.Offer.RealSilverOre:
					return "silver";
				case NoFace.Offer.FakePlatinumOre:
				case NoFace.Offer.RealPlatinumOre:
					return "platinum";
				case NoFace.Offer.FakeTitaniumOre:
				case NoFace.Offer.RealTitaniumOre:
					return "titanium";
			}
		}
	}
}