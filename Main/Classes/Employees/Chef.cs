using System.Text.Json;
using Main.Classes.Menu;    // Needed for Ingredient
using System.Collections.Generic;
using Main.Classes.Employees;

[Serializable]
public class Chef : Staff
{
    private static List<Chef> _chefExtent = new List<Chef>();

    // ----- RELATION WITH INGREDIENT -----
    private readonly List<Ingredient> _ingredients = new();
    public IReadOnlyCollection<Ingredient> Ingredients => _ingredients.AsReadOnly();

    public void AddIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient));

        if (!_ingredients.Contains(ingredient))
        {
            _ingredients.Add(ingredient);
            ingredient.AddChef(this); // update reverse side
        }
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
            return;

        if (_ingredients.Remove(ingredient))
        {
            ingredient.RemoveChef(this); // update reverse
        }
    }

    // ----- EXTENT -----
    private static void AddToExtent(Chef chef)
    {
        if (chef == null)
            throw new ArgumentException("Chef cannot be null.");
        _chefExtent.Add(chef);
    }

    public static IReadOnlyList<Chef> GetExtent()
    {
        return _chefExtent.AsReadOnly();
    }

    public Chef(int staffId, string firstName, string lastName, decimal salary, string department)
        : base(staffId, firstName, lastName, salary, department)
    {
        AddToExtent(this);
    }

    public override void hireStaff()
    {
        throw new NotImplementedException();
    }

    public override void fireStaff()
    {
        throw new NotImplementedException();
    }

    public static void Save(string path = "chefs.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_chefExtent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save chefs.", ex);
        }
    }

    public static bool Load(string path = "chefs.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _chefExtent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _chefExtent = JsonSerializer.Deserialize<List<Chef>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _chefExtent.Clear();
            return false;
        }
    }
}
