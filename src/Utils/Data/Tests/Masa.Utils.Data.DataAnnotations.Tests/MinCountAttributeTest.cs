using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.DataAnnotations.Tests
{
    [TestClass]
    public class MinCountAttributeTest
    {
        [TestMethod]
        public void EmptyListWithMinCount1Test()
        {
            var user = new User
            {
                Name = "name",
                Favorites = new List<string>()
            };

            var results = ValidateModel(user);

            Assert.IsTrue(results.Any(u => u.MemberNames.Contains(nameof(User.Favorites))));
        }

        [TestMethod]
        public void NullValueWithMinCount1Test()
        {
            var user = new User
            {
                Name = "name",
            };

            var results = ValidateModel(user);

            Assert.IsFalse(results.Any(u => u.MemberNames.Contains(nameof(User.Favorites))));
        }

        [TestMethod]
        public void Count1WithMinCount1Test()
        {
            var user = new User
            {
                Name = "name",
                Favorites = new List<string>() { "A" }
            };

            var results = ValidateModel(user);

            Assert.IsFalse(results.Any(u => u.MemberNames.Contains(nameof(User.Favorites))));
        }

        [TestMethod]
        public void Count2WithMinCount1Test()
        {
            var user = new User
            {
                Name = "name",
                Favorites = new List<string>() { "A", "B" }
            };

            var results = ValidateModel(user);

            Assert.IsFalse(results.Any(u => u.MemberNames.Contains(nameof(User.Favorites))));
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validateResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model);
            Validator.TryValidateObject(model, ctx, validateResults, true);
            return validateResults;
        }

        public class User
        {
            [Required]
            public string Name { get; set; } = default!;

            [MinCount(1)]
            public List<string> Favorites { get; set; } = default!;
        }
    }
}
