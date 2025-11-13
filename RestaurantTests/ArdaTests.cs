using Menu;

namespace RestaurantTests;

[TestFixture]
public class ArdaTests
{
    [TestFixture]
    public class MenuItemTests
    {
        public class TestableMenuItem : MenuItems
        {
            public TestableMenuItem(
                string name,
                decimal price,
                bool isAvailable,
                string? description = null
            ) : base(name, price, isAvailable, description)
            {
            }
        }

        [Test]
        public void ConstructorValidValues()
        {
            var item = new TestableMenuItem("Pizza", 20, true, "Cheesy");

            Assert.That(item.Name, Is.EqualTo("Pizza"));
            Assert.That(item.Price, Is.EqualTo(20));
            Assert.That(item.IsAvailable, Is.True);
            Assert.That(item.Description, Is.EqualTo("Cheesy"));
        }

        [Test]
        public void ConstructorWithEmptyNameThrows()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new TestableMenuItem("", 10, true));

            Assert.That(ex.Message, Is.EqualTo("Name cannot be empty"));
        }

        [Test]
        public void UpdateMenuItemWorks()
        {
            var item = new TestableMenuItem("Burger", 18, true);

            item.UpdateMenuItem(
                name: "Veggie Burger",
                price: 20,
                isAvailable: false,
                description: "New!"
            );

            Assert.That(item.Name, Is.EqualTo("Veggie Burger"));
            Assert.That(item.Price, Is.EqualTo(20m));
            Assert.That(item.IsAvailable, Is.False);
            Assert.That(item.Description, Is.EqualTo("New!"));
        }
    }
}