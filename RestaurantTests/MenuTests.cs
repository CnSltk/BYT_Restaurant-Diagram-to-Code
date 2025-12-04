using Main.Classes.Employees;
using Menu;

namespace RestaurantTests;

[TestFixture]
public class MenuTests
{
    
    [Test]
    public void Menu_ValidValues_AssignedCorrectly()
    {
        var menu = new Menu.Menu(0, "Lunch Menu", "v1.0", true);

        Assert.That(menu.Name, Is.EqualTo("Lunch Menu"));
        Assert.That(menu.Version, Is.EqualTo("v1.0"));
        Assert.That(menu.IsActive, Is.True);
    }

    [Test]
    public void Menu_NameTooLong_ThrowsException()
    {
        var menu = new Menu.Menu(0,"Lunch Menu", "v1.0", true);
        var longName = new string('A', 51);

        var ex = Assert.Throws<ArgumentException>(() => menu.Name = longName);

        Assert.That(ex!.Message, Is.EqualTo("Menu name length must be between 2 and 50 characters"));
    }

    [Test]
    public void Menu_SaveAndLoadExtent_RestoresMenus()
    {
        Menu.Menu.ClearExtentForTest();

        var m1 = new Menu.Menu(0,"Lunch Menu", "v1.0", true);
        var m2 = new Menu.Menu(1,"Dinner Menu", "v2.0", false);

        var path = "Menu_Test.json";
        if (File.Exists(path))
            File.Delete(path);

        Menu.Menu.Save(path);
        Menu.Menu.ClearExtentForTest();
        Menu.Menu.Load(path);

        Assert.That(Menu.Menu.GetExtent().Count, Is.EqualTo(2));
        Assert.That(Menu.Menu.GetExtent().Any(m => m.Name == "Lunch Menu"));
        Assert.That(Menu.Menu.GetExtent().Any(m => m.Name == "Dinner Menu"));
    }
    [Test]
    public void ShiftChange_UpdatesShiftCorrectly()
    {
        var ft = new FullTime(1,"Derya", "Ogus", 5000m, "Cashier", Shift.Morning);

        Assert.That(ft.Shift, Is.EqualTo(Shift.Morning));

        ft.ShiftChange(Shift.Night);
        Assert.That(ft.Shift, Is.EqualTo(Shift.Night));

        ft.ShiftChange(Shift.Evening);
        Assert.That(ft.Shift, Is.EqualTo(Shift.Evening));
    }
}
