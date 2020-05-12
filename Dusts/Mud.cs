using Terraria;
using Terraria.ModLoader;

namespace SpiritedAway.Dusts
{
	public class Mud : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= .4F;
			//dust.noGravity = false;
			//dust.noLight = false;
			dust.scale *= 1.5F;
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