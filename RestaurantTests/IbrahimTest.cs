using Main.Classes.Orders;

namespace RestaurantTests;

[TestFixture]
public class IbrahimTest
{
     
     [Test]
     public void ConstructorValidValues()
     {
          var customer = new Customer(1, "Ibrahim", "Yesil", "453043988", "s30066@pjwstk.edu.pl");
          Assert.That(customer, Is.Not.Null);
          Assert.That(customer.Name, Is.EqualTo("Ibrahim"));
          Assert.That(customer.Surname, Is.EqualTo("Yesil"));
          Assert.That(customer.PhoneNumber, Is.EqualTo("453043988"));
          Assert.That(customer.Email, Is.EqualTo("s30066@pjwstk.edu.pl"));
          
     }

     [Test]
     public void ConstructorInvalidValues()
     {
          var ex = Assert.Throws<ArgumentException>(() => new Customer(-3, "Ibrahim", "Yesil", null, null));
          Assert.That(ex.Message, Is.EqualTo("CustomerId cannot be less than 1."));
     }

     [Test]
     public void ConstructorWithEmptySurname()
     {
          var ex2 = Assert.Throws<ArgumentException>(() => new Customer(2, "Ibrahim", "", null, null));
          Assert.That(ex2.Message, Is.EqualTo("Surname cannot be empty"));
          
     }
     
    
}

