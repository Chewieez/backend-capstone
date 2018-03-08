using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using RepPortal.Models;

namespace RepPortal.Data
{
    public static class DbInitializer
    {
        public async static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var userstore = new UserStore<ApplicationUser>(context);

                if (!context.Roles.Any(r => r.Name == "Administrator"))
                {
                    var AdminRole = new IdentityRole { Name = "Administrator", NormalizedName = "Administrator" };
                    var RepRole = new IdentityRole { Name = "Rep", NormalizedName = "REP" };
                    await roleStore.CreateAsync(AdminRole);
                    await roleStore.CreateAsync(RepRole);
                }

                if (!context.ApplicationUser.Any(u => u.FirstName == "admin"))
                {
                    //  This method will be called after migrating to the latest version.
                    ApplicationUser user = new ApplicationUser
                    {
                        FirstName = "admin",
                        LastName = "admin",
                        UserName = "admin@admin.com",
                        NormalizedUserName = "ADMIN@ADMIN.COM",
                        Email = "admin@admin.com",
                        NormalizedEmail = "ADMIN@ADMIN.COM",
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };
                    var passwordHash = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = passwordHash.HashPassword(user, "Admin1!");
                    await userstore.CreateAsync(user);
                    await userstore.AddToRoleAsync(user, "Administrator");
                }



                if (!context.State.Any())
                {
                    var states = new State[]
                    {
                        new State {
                            Name = "TN",
                        },
                        new State {
                            Name = "MS",
                        },
                        new State {
                            Name = "AL",
                        }
                    };

                    foreach (State s in states)
                    {
                        context.State.Add(s);
                    }
                    context.SaveChanges();
                }

                if (!context.Status.Any())
                {
                    var statuses = new Status[]
                    {
                        new Status {
                            Name = "Active",
                            Color = "Green"
                        },
                        new Status {
                            Name = "Warn",
                            Color = "Yellow"
                        },
                        new Status {
                            Name = "Inactive",
                            Color = "Red"
                        },
                        new Status {
                            Name = "Closed",
                            Color = "Grey"
                        }
                    };

                    foreach (Status s in statuses)
                    {
                        context.Status.Add(s);
                    }
                    context.SaveChanges();
                }

            }
        }
    }
}
