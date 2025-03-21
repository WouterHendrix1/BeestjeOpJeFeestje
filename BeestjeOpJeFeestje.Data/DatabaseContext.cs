using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeestjeOpJeFeestje.Data
{
    public class DatabaseContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        private static bool _animalsSeeded = false;
        private static bool _adminSeeded = false;


        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {

            if (!_adminSeeded)
            {
                _adminSeeded = true;
            }


            if (!_animalsSeeded)
            {
                SeedAnimals();
                _animalsSeeded = true;
            }
        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Animals)
                .WithMany(a => a.Bookings)
                .UsingEntity(j => j.ToTable("BookingAnimals"));
        }

        private void SeedAnimals()
        {
            if (Animals.Any()) return;

            Animals.AddRange(
                new Animal { Name = "Aap", Type = AnimalType.Jungle, Price = 100, ImageUrl="/images/aap.jpg" },
                new Animal { Name = "Olifant", Type = AnimalType.Jungle, Price = 200, ImageUrl= "/images/olifant.jpg" },
                new Animal { Name = "Zebra", Type = AnimalType.Jungle, Price = 150, ImageUrl = "/images/zebra.jpg" },
                new Animal { Name = "Leeuw", Type = AnimalType.Jungle, Price = 250, ImageUrl = "/images/leeuw.jpg" },
                new Animal { Name = "Hond", Type = AnimalType.Farm, Price = 50, ImageUrl = "/images/hond.jpg" },
                new Animal { Name = "Ezel", Type = AnimalType.Farm, Price = 75, ImageUrl = "/images/ezel.jpg" },
                new Animal { Name = "Koe", Type = AnimalType.Farm, Price = 100, ImageUrl = "/images/koe.jpg" },
                new Animal { Name = "Eend", Type = AnimalType.Farm, Price = 30, ImageUrl = "/images/eend.jpg" },
                new Animal { Name = "Kuiken", Type = AnimalType.Farm, Price = 20, ImageUrl = "/images/kuiken.jpg" },
                new Animal { Name = "Pinguïn", Type = AnimalType.Snow, Price = 120, ImageUrl = "/images/pinguin.jpg" },
                new Animal { Name = "IJsbeer", Type = AnimalType.Snow, Price = 300, ImageUrl = "/images/ijsbeer.jpg" },
                new Animal { Name = "Zeehond", Type = AnimalType.Snow, Price = 180, ImageUrl = "/images/zeehond.jpg" },
                new Animal { Name = "Kameel", Type = AnimalType.Desert, Price = 200, ImageUrl = "/images/kameel.jpg" },
                new Animal { Name = "Slang", Type = AnimalType.Desert, Price = 100, ImageUrl = "/images/slang.jpg" },
                new Animal { Name = "T-Rex", Type = AnimalType.VIP, Price = 1000, ImageUrl = "/images/trex.jpg" },
                new Animal { Name = "Unicorn", Type = AnimalType.VIP, Price = 1500, ImageUrl = "/images/unicorn.jpg" }
            );

            SaveChanges();
        }
    }
}
