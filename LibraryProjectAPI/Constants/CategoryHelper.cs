namespace LibraryProjectAPI.Constants
{
    public class CategoryHelper
    {
        public static readonly List<string> Categories = ["Horror", "Romance", "Comedy", "Adventure", "Fiction", "Non-fiction", "Graphic Novel"];

        public static string PickRandomCategory() 
        {
            var random = new Random();
            return Categories[random.Next(Categories.Count)];
        }
    }
}
