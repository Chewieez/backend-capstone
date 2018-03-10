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
                    State[] states = new State[]
                    {
                        new State {
                            Name = "AL",
                        },
                        new State {
                            Name = "AK",
                        },
    
    
                        new State {
                            Name = "AR",
                        },
                        new State {
                            Name = "CA",
                        },
                        new State {
                            Name = "CO",
                        },
                        new State {
                            Name = "CT",
                        },
                        new State {
                            Name = "DE",
                        },
                        new State {
                            Name = "Washington DC",
                        },
                        new State {
                            Name = "FL",
                        },
                        new State {
                            Name = "GA",
                        },
                        new State {
                            Name = "GU",
                        },
                        new State {
                            Name = "HI",
                        },
                        new State {
                            Name = "ID",
                        },
                        new State {
                            Name = "IL",
                        },
                        new State {
                            Name = "IN",
                        },
                        new State {
                            Name = "IA",
                        },
                        new State {
                            Name = "KS",
                        },
                        new State {
                            Name = "KY",
                        },
                        new State {
                            Name = "LA",
                        },
                        new State {
                            Name = "ME",
                        },
                        new State {
                            Name = "MD",
                        },
                        new State {
                            Name = "MH",
                        },
                        new State {
                            Name = "MI",
                        },
                        new State {
                            Name = "MN",
                        },
                        new State {
                            Name = "MS",
                        },
                        new State {
                            Name = "MO",
                        },
                        new State {
                            Name = "MT",
                        },
                        new State {
                            Name = "NE"
                        },
                        new State {
                            Name = "NV",
                        },
                        new State {
                            Name = "NH",
                        },
                        new State {
                            Name = "NJ",
                        },
                        new State {
                            Name = "NM",
                        },
                        new State {
                            Name = "NY",
                        },
                        new State {
                            Name = "NC",
                        },
                        new State {
                            Name = "ND",
                        },
                        new State {
                            Name = "OH",
                        },
                        new State {
                            Name = "OK",
                        },
                        new State {
                            Name = "OR",
                        },
                        new State {
                            Name = "PA",
                        },
                        new State {
                            Name = "PR",
                        },
                        new State {
                            Name = "RH",
                        },
                        new State {
                            Name = "SC",
                        },
                        new State {
                            Name = "SD",
                        },
                        new State {
                            Name = "TN",
                        },
                        new State {
                            Name = "TX",
                        },
                        new State {
                            Name = "UT",
                        },
                        new State {
                            Name = "VT",
                        },
                        new State {
                            Name = "VI",
                        },
                        new State {
                            Name = "WA",
                        },
                        new State {
                            Name = "WV",
                        },
                        new State {
                            Name = "WI",
                        },
                        new State {
                            Name = "WY",
                        },
                    };

                    states = states.OrderBy(s => s.Name).ToArray();

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
