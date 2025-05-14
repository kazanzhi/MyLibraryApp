﻿using Microsoft.AspNetCore.Identity;
using Moq;
using MyLibraryApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibraryApp.Tests.UnitTests.Helpers
{
    public static class MockUserManager
    {
        public static Mock<UserManager<TUser>> CreateMockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();

            return new Mock<UserManager<TUser>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        } 
    }
}
