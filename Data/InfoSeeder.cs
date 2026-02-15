using InfoInfo.Data;
using InfoInfo.Models;
using InfoInfo.Models.Campus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Data
{
    public class InfoSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();

                    if (await dbContext.Database.CanConnectAsync())
                    {
                        await SeedRolesAsync(dbContext, roleManager);
                        await SeedUsersAsync(dbContext, userManager);
                        await SeedCategoriesAsync(dbContext);
                        await SeedTextsAsync(dbContext);
                        await SeedOpinionsAsync(dbContext, userManager);
                        await SeedCampusAsync(dbContext);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Błąd podczas inicjalizacji danych: {ex.Message}");
                }
            }
        }

        private static async Task SeedRolesAsync(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "admin", "author" };
            foreach (var roleName in roleNames)
            {
                if (!await dbContext.Roles.AnyAsync(r => r.Name == roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = roleName, NormalizedName = roleName.ToUpper() });
                }
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            var usersToCreate = new List<(AppUser User, string Password, string Role)>
            {
                (new AppUser { UserName = "autor1@portal.pl", Email = "autor1@portal.pl", EmailConfirmed = true, FirstName = "Piotr", LastName = "Pisarski", Photo = "autor1.jpg", Information = "Programista." }, "Portalik1!", "author"),
                (new AppUser { UserName = "autor2@portal.pl", Email = "autor2@portal.pl", EmailConfirmed = true, FirstName = "Anna", LastName = "Autorska", Photo = "autor2.jpg", Information = "Blogerka." }, "Portalik1!", "author"),
                (new AppUser { UserName = "admin@portal.pl", Email = "admin@portal.pl", EmailConfirmed = true, FirstName = "Ewa", LastName = "Ważna", Photo = "woman.png", Information = "Admin." }, "Portalik1!", "admin")
            };

            foreach (var (user, password, role) in usersToCreate)
            {
                if (!await dbContext.Users.AnyAsync(u => u.UserName == user.UserName))
                {
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded) await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext dbContext)
        {
            if (!await dbContext.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Wiadomości", Active = true, Display = true, Icon = "chat-left-text", Description = "Newsy." },
                    new Category { Name = "Artykuły", Active = true, Display = true, Icon = "journal-richtext", Description = "Dłuższe teksty." },
                    new Category { Name = "Testy", Active = true, Display = true, Icon = "speedometer", Description = "Testy sprzętu." },
                    new Category { Name = "Porady", Active = true, Display = true, Icon = "life-preserver", Description = "Poradniki." },
                    new Category { Name = "Tutoriale", Active = true, Display = true, Icon = "display", Description = "Krok po kroku." },
                    new Category { Name = "Recenzje", Active = true, Display = true, Icon = "controller", Description = "Opinie." }
                };
                await dbContext.Categories.AddRangeAsync(categories);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedTextsAsync(ApplicationDbContext dbContext)
        {
            if (!await dbContext.Texts.AnyAsync())
            {
                var author1 = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "autor1@portal.pl");
                var author2 = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "autor2@portal.pl");

                if (author1 == null || author2 == null) return;

                var texts = new List<Text>();
                for (int i = 1; i <= 6; i++) // Kategorie
                {
                    for (int j = 0; j < 5; j++) // Teksty autora 1
                    {
                        texts.Add(new Text { Title = $"Tytuł{i}{j}", Summary = "Streszczenie...", Content = "Lorem ipsum...", CategoryId = i, UserId = author1.Id, Active = true, AddedDate = DateTime.Now.AddDays(-i * j) });
                    }
                    for (int j = 5; j < 10; j++) // Teksty autora 2
                    {
                        texts.Add(new Text { Title = $"Tytuł{i}{j}", Summary = "Streszczenie...", Content = "Lorem ipsum...", CategoryId = i, UserId = author2.Id, Active = true, AddedDate = DateTime.Now.AddDays(-i * j) });
                    }
                }
                await dbContext.Texts.AddRangeAsync(texts);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedOpinionsAsync(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            if (!await dbContext.Opinions.AnyAsync())
            {
                var author1 = await userManager.FindByEmailAsync("autor1@portal.pl");
                var admin = await userManager.FindByEmailAsync("admin@portal.pl");

                if (author1 != null && admin != null)
                {
                    // Dodawanie przykładowych opinii
                    var opinions = new List<Opinion>();
                    for (int i = 1; i <= 60; i++)
                    {
                        opinions.Add(new Opinion { Comment = "Komentarz admina", TextId = i, UserId = admin.Id, Rating = Rating.Excellent, AddedDate = DateTime.Now });
                        opinions.Add(new Opinion { Comment = "Komentarz autora", TextId = i, UserId = author1.Id, Rating = Rating.Good, AddedDate = DateTime.Now });
                    }
                    await dbContext.Opinions.AddRangeAsync(opinions);
                    await dbContext.SaveChangesAsync();
                }
            }
        }


        private static async Task SeedCampusAsync(ApplicationDbContext dbContext)
        {
            // Jeśli baza nie ma jeszcze tabel (brak migracji), nie przerywamy startu aplikacji.
            try
            {
                if (await dbContext.Buildings.AnyAsync()) return;
            }
            catch
            {
                // Uruchom lokalnie migracje: Add-Migration AddCampusEntities + Update-Database
                return;
            }

            var buildingA = new Building
            {
                Name = "Budynek A",
                Code = "A",
                Location = "Płock",
                Description = "Budynek A – część zachodnia (dane przykładowe do projektu PAI)."
            };

            var floor0 = new Floor
            {
                Name = "Parter",
                Level = 0,
                PlanImagePath = "/campus/plans/budynekA_parter.png",
                Building = buildingA
            };

            var floor1 = new Floor
            {
                Name = "I piętro",
                Level = 1,
                PlanImagePath = "/campus/plans/budynekA_pietro1.png",
                Building = buildingA
            };

            // Sale – zachodnia część (na podstawie numeracji z planów)
            var rooms0 = new List<Room>
            {
                new Room { Floor = floor0, Name = "11", RoomType = RoomType.ComputerLab, Description = "Sala komputerowa (parter)." },
                new Room { Floor = floor0, Name = "11A", RoomType = RoomType.Classroom, Description = "Pomieszczenie pomocnicze (parter)." },
                new Room { Floor = floor0, Name = "12", RoomType = RoomType.Classroom, Description = "Sala ćwiczeniowa (parter)." },
                new Room { Floor = floor0, Name = "13", RoomType = RoomType.Office, Description = "Sekretariat / biuro (parter)." },
                new Room { Floor = floor0, Name = "14", RoomType = RoomType.ComputerLab, Description = "Sala komputerowa (parter)." },
                new Room { Floor = floor0, Name = "15", RoomType = RoomType.Classroom, Description = "Sala ćwiczeniowa (parter)." },
                new Room { Floor = floor0, Name = "16", RoomType = RoomType.Classroom, Description = "Sala ćwiczeniowa (parter)." },
            };

            var rooms1 = new List<Room>
            {
                new Room { Floor = floor1, Name = "34", RoomType = RoomType.Classroom, Description = "Sala dydaktyczna (I piętro)." },
                new Room { Floor = floor1, Name = "35", RoomType = RoomType.Office, Description = "Pomieszczenia administracyjne (I piętro)." },
                new Room { Floor = floor1, Name = "36", RoomType = RoomType.Office, Description = "Pomieszczenia administracyjne (I piętro)." },
                new Room { Floor = floor1, Name = "37", RoomType = RoomType.ComputerLab, Description = "Sala komputerowa (I piętro)." },
                new Room { Floor = floor1, Name = "38", RoomType = RoomType.Classroom, Description = "Sala ćwiczeniowa (I piętro)." },
                new Room { Floor = floor1, Name = "39", RoomType = RoomType.Classroom, Description = "Sala ćwiczeniowa (I piętro)." },
                new Room { Floor = floor1, Name = "40", RoomType = RoomType.Classroom, Description = "Sala ćwiczeniowa (I piętro)." },
            };

            // Punkty ruchu – prosta, liniowa nawigacja korytarzem + wejścia do sal
            string mp = "https://my.matterport.com/show/?m=awVdjaW4BHa";
            var p0_entry = new MovementPoint { Floor = floor0, Name = "Parter – wejście zachód", Description = "Start spaceru (parter, część zachodnia).", MatterportUrl = mp, AltText = "Korytarz – parter, wejście zachód" };
            var p0_corr1 = new MovementPoint { Floor = floor0, Name = "Parter – korytarz (strefa 11–13)", Description = "Korytarz w pobliżu sal 11–13.", MatterportUrl = mp, AltText = "Korytarz – parter, okolice sal 11–13" };
            var p0_corr2 = new MovementPoint { Floor = floor0, Name = "Parter – korytarz (strefa 14–16)", Description = "Korytarz w pobliżu sal 14–16.", MatterportUrl = mp, AltText = "Korytarz – parter, okolice sal 14–16" };
            var p0_elev = new MovementPoint { Floor = floor0, Name = "Parter – winda", Description = "Winda (przejście na I piętro).", MatterportUrl = mp, AltText = "Winda – parter" };

            var p1_elev = new MovementPoint { Floor = floor1, Name = "I piętro – winda", Description = "Winda (I piętro).", MatterportUrl = mp, AltText = "Winda – I piętro" };
            var p1_corr1 = new MovementPoint { Floor = floor1, Name = "I piętro – korytarz (strefa 34–37)", Description = "Korytarz w pobliżu sal 34–37.", MatterportUrl = mp, AltText = "Korytarz – I piętro, okolice sal 34–37" };
            var p1_corr2 = new MovementPoint { Floor = floor1, Name = "I piętro – korytarz (strefa 38–40)", Description = "Korytarz w pobliżu sal 38–40.", MatterportUrl = mp, AltText = "Korytarz – I piętro, okolice sal 38–40" };

            await dbContext.Buildings.AddAsync(buildingA);
            await dbContext.Floors.AddRangeAsync(floor0, floor1);
            await dbContext.Rooms.AddRangeAsync(rooms0);
            await dbContext.Rooms.AddRangeAsync(rooms1);
            await dbContext.MovementPoints.AddRangeAsync(p0_entry, p0_corr1, p0_corr2, p0_elev, p1_elev, p1_corr1, p1_corr2);
            await dbContext.SaveChangesAsync();

            // powiązania sal z punktami (opcjonalnie)
            var r11 = rooms0[0]; var r12 = rooms0[2]; var r13 = rooms0[3]; var r14 = rooms0[4]; var r15 = rooms0[5]; var r16 = rooms0[6];
            r11.EntryPointId = p0_corr1.Id;
            r12.EntryPointId = p0_corr1.Id;
            r13.EntryPointId = p0_corr1.Id;
            r14.EntryPointId = p0_corr2.Id;
            r15.EntryPointId = p0_corr2.Id;
            r16.EntryPointId = p0_corr2.Id;

            var r34 = rooms1[0]; var r37 = rooms1[3]; var r38 = rooms1[4]; var r39 = rooms1[5]; var r40 = rooms1[6];
            r34.EntryPointId = p1_corr1.Id;
            r37.EntryPointId = p1_corr1.Id;
            r38.EntryPointId = p1_corr2.Id;
            r39.EntryPointId = p1_corr2.Id;
            r40.EntryPointId = p1_corr2.Id;

            // Kierunki (4 na punkt) – model przykładowy
            var directions = new List<Direction>
            {
                // Parter: wejście -> korytarz 11–13
                new Direction { FromPointId = p0_entry.Id, Kind = DirectionKind.Forward, IsActive = true, ToPointId = p0_corr1.Id, WeightMeters = 6, ElevatorFriendly = true },
                new Direction { FromPointId = p0_entry.Id, Kind = DirectionKind.Back, IsActive = false, WeightMeters = 0, ElevatorFriendly = true },
                new Direction { FromPointId = p0_entry.Id, Kind = DirectionKind.Left, IsActive = false, WeightMeters = 0, ElevatorFriendly = true },
                new Direction { FromPointId = p0_entry.Id, Kind = DirectionKind.Right, IsActive = false, WeightMeters = 0, ElevatorFriendly = true },

                // Parter: korytarz 11–13
                new Direction { FromPointId = p0_corr1.Id, Kind = DirectionKind.Back, IsActive = true, ToPointId = p0_entry.Id, WeightMeters = 6, ElevatorFriendly = true },
                new Direction { FromPointId = p0_corr1.Id, Kind = DirectionKind.Forward, IsActive = true, ToPointId = p0_corr2.Id, WeightMeters = 18, ElevatorFriendly = true },
                new Direction { FromPointId = p0_corr1.Id, Kind = DirectionKind.Left, IsActive = true, ToRoomId = r11.Id, WeightMeters = 2, ElevatorFriendly = true },
                new Direction { FromPointId = p0_corr1.Id, Kind = DirectionKind.Right, IsActive = true, ToRoomId = r12.Id, WeightMeters = 2, ElevatorFriendly = true },

                // Parter: korytarz 14–16
                new Direction { FromPointId = p0_corr2.Id, Kind = DirectionKind.Back, IsActive = true, ToPointId = p0_corr1.Id, WeightMeters = 18, ElevatorFriendly = true },
                new Direction { FromPointId = p0_corr2.Id, Kind = DirectionKind.Forward, IsActive = true, ToPointId = p0_elev.Id, WeightMeters = 8, ElevatorFriendly = true },
                new Direction { FromPointId = p0_corr2.Id, Kind = DirectionKind.Left, IsActive = true, ToRoomId = r14.Id, WeightMeters = 2, ElevatorFriendly = true },
                new Direction { FromPointId = p0_corr2.Id, Kind = DirectionKind.Right, IsActive = true, ToRoomId = r15.Id, WeightMeters = 2, ElevatorFriendly = true },

                // Parter: winda
                new Direction { FromPointId = p0_elev.Id, Kind = DirectionKind.Back, IsActive = true, ToPointId = p0_corr2.Id, WeightMeters = 8, ElevatorFriendly = true },
                new Direction { FromPointId = p0_elev.Id, Kind = DirectionKind.Forward, IsActive = true, ToPointId = p1_elev.Id, WeightMeters = 1, ElevatorFriendly = true },
                new Direction { FromPointId = p0_elev.Id, Kind = DirectionKind.Left, IsActive = false, WeightMeters = 0, ElevatorFriendly = true },
                new Direction { FromPointId = p0_elev.Id, Kind = DirectionKind.Right, IsActive = false, WeightMeters = 0, ElevatorFriendly = true },

                // I piętro: winda
                new Direction { FromPointId = p1_elev.Id, Kind = DirectionKind.Back, IsActive = true, ToPointId = p0_elev.Id, WeightMeters = 1, ElevatorFriendly = true },
                new Direction { FromPointId = p1_elev.Id, Kind = DirectionKind.Forward, IsActive = true, ToPointId = p1_corr1.Id, WeightMeters = 8, ElevatorFriendly = true },
                new Direction { FromPointId = p1_elev.Id, Kind = DirectionKind.Left, IsActive = false, WeightMeters = 0, ElevatorFriendly = true },
                new Direction { FromPointId = p1_elev.Id, Kind = DirectionKind.Right, IsActive = false, WeightMeters = 0, ElevatorFriendly = true },

                // I piętro: korytarz 34–37
                new Direction { FromPointId = p1_corr1.Id, Kind = DirectionKind.Back, IsActive = true, ToPointId = p1_elev.Id, WeightMeters = 8, ElevatorFriendly = true },
                new Direction { FromPointId = p1_corr1.Id, Kind = DirectionKind.Forward, IsActive = true, ToPointId = p1_corr2.Id, WeightMeters = 18, ElevatorFriendly = true },
                new Direction { FromPointId = p1_corr1.Id, Kind = DirectionKind.Left, IsActive = true, ToRoomId = r34.Id, WeightMeters = 2, ElevatorFriendly = true },
                new Direction { FromPointId = p1_corr1.Id, Kind = DirectionKind.Right, IsActive = true, ToRoomId = r37.Id, WeightMeters = 2, ElevatorFriendly = true },

                // I piętro: korytarz 38–40
                new Direction { FromPointId = p1_corr2.Id, Kind = DirectionKind.Back, IsActive = true, ToPointId = p1_corr1.Id, WeightMeters = 18, ElevatorFriendly = true },
                new Direction { FromPointId = p1_corr2.Id, Kind = DirectionKind.Forward, IsActive = false, WeightMeters = 0, ElevatorFriendly = true },
                new Direction { FromPointId = p1_corr2.Id, Kind = DirectionKind.Left, IsActive = true, ToRoomId = r38.Id, WeightMeters = 2, ElevatorFriendly = true },
                new Direction { FromPointId = p1_corr2.Id, Kind = DirectionKind.Right, IsActive = true, ToRoomId = r39.Id, WeightMeters = 2, ElevatorFriendly = true },
            };

            // Unikamy konfliktów: jeśli w przyszłości dopiszesz dane ręcznie, seed nie nadpisze.
            await dbContext.Directions.AddRangeAsync(directions);
            await dbContext.SaveChangesAsync();
        }
    }
}