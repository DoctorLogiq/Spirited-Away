using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritedAway
{
	public class SpiritedAway : Mod
	{
		public override void Unload()
		{
			NPCs.NoFace.ShopItems.Clear();
		}

		public static void Announce(string message, byte red = 50, byte green = 125, byte blue = 255)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				Main.NewText(message, red, green, blue);
			}
			else
			{
				NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), new Color(red, green, blue));
			}
		}
	}
}