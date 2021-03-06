﻿using Know_Me_Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Know_Me_Api
{
    public class DbInitializer
    {
        public static void Initialize(DBContext context)
        {
            //context.Database.Migrate();
            context.Database.EnsureCreated();

            if (context.UserInfo.Any() || context.Role.Any())
            {
                return;   // DB has been seeded
            }

            var user = new UserInfo[]
            {
                new UserInfo{
                    userId = new Guid(),
                    email ="nanduhalasige@gmail.com",
                    firstName ="Nandan",
                    lastName ="Hegde",
                    isActive = true,
                    password = "password@123",
                    userName ="nanduhalasige",
                    roleId = 1
                }
            };
            foreach (UserInfo u in user)
            {
                context.UserInfo.Add(u);
            }

            var role = new Role[]
            {
                new Role{
                   role="Administrator"
                },
                new Role{
                    role="Manager"
                },
                new Role{
                    role="Employee"
                }
            };
            foreach (Role u in role)
            {
                context.Role.Add(u);
            }

            var wareHouse = new WareHouse[]
            {
                new WareHouse{
                   WhName = "WH 1"
                },
                 new WareHouse{
                   WhName = "WH 2"
                },
                 new WareHouse{
                   WhName = "WH 3"
                },
            };

            foreach (WareHouse w in wareHouse)
            {
                context.WareHouse.Add(w);
            }

            context.SaveChanges();
        }

    }
}
