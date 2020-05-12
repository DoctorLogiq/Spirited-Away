using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using SpiritedAway.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace SpiritedAway.Projectiles
{
	public class MudBall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.friendly = true;
			projectile.magic = true; // TODO(LOGIX): Should this be true? (Find out what it does)
			projectile.penetrate = 3;
			projectile.timeLeft = 600;
		}

		public override void AI()
		{
			projectile.velocity.Y += projectile.ai[0];
			if (Main.rand.NextBool(3))
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustType<Mud>(), projectile.velocity.X * .5F, projectile.velocity.Y * .5F);
			}

			if (projectile.frame + 1 == 3)
			{
				projectile.frame = 0;
			}
			else projectile.frame++;
		}

		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.penetrate--;
			if (projectile.penetrate <= 0)
			{
				projectile.Kill();
			}
			else
			{
				projectile.ai[0] += .1F;
				if (projectile.velocity.X != oldVelocity.X)
				{
					projectile.velocity.X = -oldVelocity.X;
				}
				if (projectile.velocity.Y != oldVelocity.Y)
				{
					projectile.velocity.Y = -oldVelocity.Y;
				}

				projectile.velocity *= .75F;
				Main.PlaySound(SoundID.Item10, projectile.position); // TODO(LOGIX): Update
			}

			return false;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; ++i)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustType<Mud>(), projectile.velocity.X * .5F, projectile.velocity.Y * .5F);
			}
			Main.PlaySound(SoundID.Item25, projectile.position); // TODO(LOGIX): Update
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			projectile.ai[0] += .1F;
			projectile.velocity *= .75F;
		}
	}
}