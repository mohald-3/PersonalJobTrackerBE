using Bogus;
using Domain.Models.Entities;
using Domain.Models.Enums;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Database.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var faker = new Faker("en");

            // ---------------------------
            // 1) Seed Roles
            // ---------------------------
            if (!await context.Roles.AnyAsync())
            {
                var roles = new[]
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" },
                    new Role { Name = "CompanyUser" },
                    new Role { Name = "Auditor" },
                    new Role { Name = "Manager" }
                };
                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            // ---------------------------
            // 2) Seed Users
            // ---------------------------
            if (!await context.Users.AnyAsync())
            {
                var roles = await context.Roles.ToListAsync();
                var adminRole = roles.First(r => r.Name == "Admin");
                var userRole = roles.First(r => r.Name == "User");
                var companyRole = roles.First(r => r.Name == "CompanyUser");

                var users = new List<User>();

                // Admin
                users.Add(CreateUserWithPassword("admin", "admin@bank.com", "admin123", new[] { adminRole }));

                // Regular users
                for (int i = 0; i < 5; i++)
                {
                    var user = CreateUserWithPassword(
                        faker.Internet.UserName(),
                        faker.Internet.Email(),
                        "user123",
                        new[] { userRole });
                    users.Add(user);
                }

                // Company/internal users
                for (int i = 0; i < 3; i++)
                {
                    var user = CreateUserWithPassword(
                        $"employee_{faker.Random.Number(100, 999)}",
                        faker.Internet.Email(),
                        "company123",
                        new[] { companyRole });
                    users.Add(user);
                }

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            // ==================================================
            // 3) Seed Companies
            // ==================================================
            if (!await context.Companies.AnyAsync())
            {
                var companies = new List<Company>();

                for (int i = 0; i < 20; i++)
                {
                    companies.Add(new Company
                    {
                        Id = Guid.NewGuid(),
                        Name = faker.Company.CompanyName(),
                        OrgNumber = faker.Random.Replace("##########"), // random numeric string
                        City = faker.Address.City(),
                        Country = faker.Address.Country(),
                        Industry = faker.Company.CatchPhrase(),
                        WebsiteUrl = faker.Internet.Url(),
                        Notes = faker.Lorem.Sentence()
                    });
                }

                await context.Companies.AddRangeAsync(companies);
                await context.SaveChangesAsync();
            }


            // ==================================================
            // 4) Seed Job Applications
            // ==================================================
            if (!await context.Applications.AnyAsync())
            {
                var companies = await context.Companies.ToListAsync();
                var applications = new List<JobApplication>();

                var statuses = Enum.GetValues(typeof(ApplicationStatus))
                                   .Cast<ApplicationStatus>()
                                   .ToList();

                for (int i = 0; i < 50; i++)
                {
                    var company = faker.PickRandom(companies);
                    var appliedDate = faker.Date.Past(1); // within last year

                    applications.Add(new JobApplication
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = company.Id,
                        PositionTitle = faker.Name.JobTitle(),
                        Status = faker.PickRandom(statuses),
                        AppliedDate = appliedDate,
                        LastUpdated = DateTime.UtcNow,
                        ContactEmail = faker.Internet.Email(),
                        ContactPhone = faker.Phone.PhoneNumber(),
                        Source = faker.Company.CompanySuffix(),
                        Priority = faker.Random.Int(1, 5),
                        Notes = faker.Lorem.Sentences(2)
                    });
                }

                await context.Applications.AddRangeAsync(applications);
                await context.SaveChangesAsync();
            }
        }


        private static User CreateUserWithPassword(string username, string email, string password, Role[] roles)
        {
            using var hmac = new HMACSHA512();
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var salt = hmac.Key;

            return new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Roles = roles.Select(role => new UserRole
                {
                    RoleId = role.Id
                }).ToList()
            };
        }
    }
}
