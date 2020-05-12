using Terraria;
using Terraria.ModLoader;

namespace SpiritedAway.Dusts
{
	public class Spoopy : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= .1F;
			dust.noGravity = true;
			dust.scale *= 2F;
			dust.fadeIn = 1F;
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * .15F;
			dust.scale *= .95F;
			return false;
		}
	}
}