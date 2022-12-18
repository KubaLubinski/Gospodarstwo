using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gospodarstwo.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Cryptography;
using Microsoft.AspNetCore.SignalR;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using Info.Data;
using System.Runtime.Intrinsics.Arm;
using System.Threading;
using System;
using Text = Gospodarstwo.Models.Item;
using Gospodarstwo.Data;
using Gospodarstwo.Models;

namespace Info.Data
{
    public class InfoSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))

                if (dbContext.Database.CanConnect())
                {
                    SeedRoles(dbContext);
                    SeedUsers(dbContext);
                    SeedCategoris(dbContext);
                    SeedUnits(dbContext);
                    SeedOpinions(dbContext);
                    SeedTexts(dbContext);

                }
        }

        //zakładanie ról w apliakcji, o ile nie istnieją
        private static void SeedRoles(ApplicationDbContext dbContext)
        {
            var roleStore = new RoleStore<IdentityRole>(dbContext);

            if (!dbContext.Roles.Any(r => r.Name == "admin"))
            {
                roleStore.CreateAsync(new IdentityRole
                {
                    Name = "admin",
                    NormalizedName = "admin"
                }).Wait();
            }

            if (!dbContext.Roles.Any(r => r.Name == "author"))
            {
                roleStore.CreateAsync(new IdentityRole
                {
                    Name = "author",
                    NormalizedName = "author"
                }).Wait();
            }
        }  // koniec ról

        //zakładanie kont uzytkowników w apliakcji, o ile nie istnieją
        private static void SeedUsers(ApplicationDbContext dbContext)
        {
            if (!dbContext.Users.Any(u => u.UserName == "autor1@portal.pl"))
            {
                var user = new AppUser
                {
                    UserName = "autor1@portal.pl",
                    NormalizedUserName = "autor1@portal.pl",
                    Email = "autor1@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Piotr",
                    LastName = "Pisarski",
                    Photo = "autor1.jpg",
                    Information = "Wszechstronny programista aplikacji sieciowych i internetowych. W portfolio ma kilka ciekawych projektów zrealizowanych dla firm z branży finansowej. Współpracuje z innowacyjnymi startupami."
                };
                var password = new PasswordHasher<AppUser>();
                var hashed = password.HashPassword(user, "Portalik1!");
                user.PasswordHash = hashed;

                var userStore = new UserStore<AppUser>(dbContext);
                userStore.CreateAsync(user).Wait();
                userStore.AddToRoleAsync(user, "author").Wait();

                dbContext.SaveChanges();
            }

            if (!dbContext.Users.Any(u => u.UserName == "autor2@portal.pl"))
            {
                var user = new AppUser
                {
                    UserName = "autor2@portal.pl",
                    NormalizedUserName = "autor2@portal.pl",
                    Email = "autor2@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Anna",
                    LastName = "Autorska",
                    Photo = "autor2.jpg",
                    Information = "Doświadczona programistka i projektantka stron internetowych oraz uznana blogierka. Specjalizuje się w HTML5, CSS3, JavaScript, jQuery i Bootstrap. Obecnie pracuje nad nowymi rozwiązaniami dla graczy."
                };
                var password = new PasswordHasher<AppUser>();
                var hashed = password.HashPassword(user, "Portalik1!");
                user.PasswordHash = hashed;

                var userStore = new UserStore<AppUser>(dbContext);
                userStore.CreateAsync(user).Wait();
                userStore.AddToRoleAsync(user, "author").Wait();
                dbContext.SaveChanges();
            }

            if (!dbContext.Users.Any(u => u.UserName == "admin@portal.pl"))
            {
                var user = new AppUser
                {
                    UserName = "admin@portal.pl",
                    NormalizedUserName = "admin@portal.pl",
                    Email = "admin@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Ewa",
                    LastName = "Ważna",
                    Photo = "woman.png",
                    Information = ""
                };
                var password = new PasswordHasher<AppUser>();
                var hashed = password.HashPassword(user, "Portalik1!");
                user.PasswordHash = hashed;

                var userStore = new UserStore<AppUser>(dbContext);
                userStore.CreateAsync(user).Wait();
                userStore.AddToRoleAsync(user, "admin").Wait();
                dbContext.SaveChanges();
            }
        } // koniec użytkowników

        //dodawanie danych kategorii
        private static void SeedCategoris(ApplicationDbContext dbContext)
        {
            if (!dbContext.Categories.Any())
            {
                var kat = new List<Category>
                {
                    new Category { CategoryName = "Maszyny", Active = true, Display=true,  Description="Maszyny oraz urządzenia wykorzystywane w gospodarstwie" },
                    new Category { CategoryName = "Produkty zwierzęce", Active = true, Display=true, Description="Produkty pochodzenia zwierzęcego"},
                    new Category { CategoryName = "Produkty roślinne", Active = true, Display=true,  Description="Produkty roślinne"},
                    new Category { CategoryName = "Środki ochrony roślin", Active = true, Display=true, Description="Środki chemiczne przeznaczone do ochrony roślin" },
                    new Category { CategoryName = "Paliwo", Active = true, Display=true, Description="Olej napędowy do maszyn" },
                };
                dbContext.AddRange(kat);
                dbContext.SaveChanges();
            }
        } //koniec danych kategorii

        //dodawanie danych tekstów, o ile nie istnieją
        private static void SeedTexts(ApplicationDbContext dbContext)
        {
            if (!dbContext.Items.Any())
            {
                for (int i = 1; i <= 5; i++) //sześć kategorii
                {
                    var idUzytkownika1 = dbContext.AppUsers
                    .Where(u => u.UserName == "autor1@portal.pl").FirstOrDefault().Id;

                    for (int j = 0; j <= 4; j++) //teksty autora1
                    {
                        var tekst = new Text()
                        {
                            ItemName = "Tytuł" + i.ToString() + j.ToString(),
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
                            AddedDate = DateTime.Now.AddDays(-i * j),
                            CategoryId = i,
                            Id = idUzytkownika1,
                            Active = true,
                            MaxStoreCapacity = j * 15,
                            ItemQuantity = j * 9,
                            UnitId = i
                        };
                        dbContext.Set<Text>().Add(tekst);
                    }
                    dbContext.SaveChanges();

                    var idUzytkownika2 = dbContext.AppUsers
                    .Where(u => u.UserName == "autor2@portal.pl")
                    .FirstOrDefault()
                    .Id;

                    for (int j = 5; j <= 9; j++) //teksty autora2
                    {
                        var tekst = new Text()
                        {
                            ItemName = "Tytuł" + i.ToString() + j.ToString(),
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
                            AddedDate = DateTime.Now.AddDays(-i * j),
                            CategoryId = i,
                            Id = idUzytkownika2,
                            Active = true,
                            MaxStoreCapacity = j * 12,
                            ItemQuantity = j * 7,
                            UnitId = i
                        };
                        dbContext.Set<Text>().Add(tekst);
                    }
                    dbContext.SaveChanges();
                }
            }
        } // koniec danych tekstów

        //dodawanie treści opinii, o ile nie istnieją
        private static void SeedOpinions(ApplicationDbContext dbContext)
        {
            if (!dbContext.Notes.Any())
            {
                var idUzytkownika1 = dbContext.AppUsers
                .Where(u => u.UserName == "autor2@portal.pl").FirstOrDefault()
                .Id;

                for (int i = 1; i <= 40; i++) //sześćdziesiąt tekstów
                {
                    var komentarz = new Note()
                    {
                        Content = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
                        AddedDate = DateTime.Now.AddDays(-i),
                        Id = idUzytkownika1,
                        ItemId = i + 3,
                    };
                    dbContext.Set<Note>().Add(komentarz);
                }
                dbContext.SaveChanges();

                var idUzytkownika2 = dbContext.AppUsers.Where(u => u.UserName == "autor1@portal.pl").FirstOrDefault().Id;

                for (int i = 1; i <= 40; i++)
                {
                    var komentarz = new Note()
                    {
                        Content = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident.",
                        AddedDate = DateTime.Now.AddDays(-i),
                        Id = idUzytkownika2,
                        ItemId = i + 3,
                    };
                    dbContext.Set<Note>().Add(komentarz);
                }
                dbContext.SaveChanges();
            }
        } //koniec treści opinii

        private static void SeedUnits(ApplicationDbContext dbContext)
        {
            if (!dbContext.Units.Any())
            {
                var unit = new List<Unit>
                {
                    new Unit{ UnitName = "Kilogram", UnitShortName = "kg" },
                    new Unit { UnitName = "Litr", UnitShortName = "l"},
                    new Unit { UnitName = "Sztuka", UnitShortName ="szt."},
                    new Unit { UnitName = "Tona", UnitShortName = "t" },
                    new Unit { UnitName = "Gram", UnitShortName = "g" },
                };
                dbContext.AddRange(unit);
                dbContext.SaveChanges();
            }
        }
    }
}