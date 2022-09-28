// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.DataAnnotations.Tests
{
    [TestClass]
    public class NonDefualtAttributeTest
    {
        [TestMethod]
        public void DefaultTest()
        {
            var user = new User
            {
                Name = "name",
                NullableId = Guid.Empty
            };

            var results = ValidateModel(user);

            var isIdInvalid = results.Any(u => u.MemberNames.Contains(nameof(User.Id)));
            var isNullableIdInvalid = results.Any(u => u.MemberNames.Contains(nameof(User.NullableId)));
            var isAgeInvalid = results.Any(u => u.MemberNames.Contains(nameof(User.Age)));
            var isFavroiteInvalid = results.Any(u => u.MemberNames.Contains(nameof(User.Favroite)));

            Assert.IsTrue(isIdInvalid && isNullableIdInvalid && isAgeInvalid && isFavroiteInvalid);
        }

        [TestMethod]
        public void NonDefaultTest()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                NullableId = Guid.NewGuid(),
                Age = 20,
                Favroite = Favorites.Basketball,
                Name = "name"
            };

            var results = ValidateModel(user);

            var isIdInvalid = results.Any(u => u.MemberNames.Contains(nameof(User.Id)));
            var isNullableIdInvalid = results.Any(u => u.MemberNames.Contains(nameof(User.NullableId)));
            var isAgeInvalid = results.Any(u => u.MemberNames.Contains(nameof(User.Age)));
            var isFavroiteInvalid = results.Any(u => u.MemberNames.Contains(nameof(User.Favroite)));

            Assert.IsTrue(!isIdInvalid && !isNullableIdInvalid && !isAgeInvalid && !isFavroiteInvalid);
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
            [NonDefault]
            public Guid Id { get; set; }

            [NonDefault]
            public Guid? NullableId { get; set; }

            [Required]
            public string Name { get; set; } = default!;

            [NonDefault]
            public int Age { get; set; }

            [NonDefault]
            public Favorites Favroite { get; set; }
        }

        public enum Favorites { None, Basketball, Football }
    }
}
