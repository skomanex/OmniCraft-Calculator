using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Schema;

public class Program
{
    static void Main()
    {
        Console.WriteLine("Bienvenu dans l'Omnicrafter de Jimmy");

        List<string> craftingTrees = FindCraftingTrees();

        Console.WriteLine($"{craftingTrees.Count} arbres(s) de recettes trouve(s):");
        foreach (var craftingTree in craftingTrees.Select((value, index) => (value, index)))
        {
            Console.WriteLine($"{craftingTree.index}. {Path.GetFileName(craftingTree.value)}");
        }

        string selectedTree = SelectCraftingTree(craftingTrees);

        List<string> craftingLists = FindCraftingLists(selectedTree);

        Console.WriteLine($"{craftingLists.Count} liste(s) de craft trouvee(s) pour l'arbre de recettes specifie:");
        foreach (var craftingList in craftingLists.Select((value, index) => (value, index)))
        {
            Console.WriteLine($"{craftingList.index}. {Path.GetFileName(craftingList.value)}");
        }


        Console.ReadLine();

    }

    static List<string> FindCraftingTrees()
    {
        Regex regexCraftingTree = new Regex(@"^craftingtree_.*\.json$", RegexOptions.IgnoreCase);
        List<string> craftingTrees = Directory
            .GetFiles("Data Sources")
            .Where(file => regexCraftingTree.IsMatch(Path.GetFileName(file)))
            .ToList();

        if (craftingTrees.Count == 0)
        {
            Console.WriteLine("Aucun arbre de recettes pour craft trouve, veuillez en renseigner au moins un dans le dossier Data Sources");
            Environment.Exit(1);
        }

        return craftingTrees;
    }

    static string SelectCraftingTree(List<string> craftingTrees)
    {
        Console.WriteLine($"Veuillez choisir un arbre de craft (entrer un nombre entre 0 et {craftingTrees.Count - 1})");
        string selectedTree = "";
        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= craftingTrees.Count)
        {
            selectedTree = craftingTrees[choice];
            Console.WriteLine($"Vous avez selectionne : {Path.GetFileName(selectedTree)}");
        }
        else
        {
            Console.WriteLine("Entree utilisateur erronee");
            SelectCraftingTree(craftingTrees);
        }
        return selectedTree;
    }

    static List<string> FindCraftingLists(string selectedTree)
    {
        string gameName = Path.GetFileNameWithoutExtension(selectedTree).Replace("craftingtree_","");
        Regex regexCraftingList = new Regex($@"^craftinglist_{gameName}_[0-9]*\.json$", RegexOptions.IgnoreCase);
        List<string> craftingLists = Directory
            .GetFiles("Data Sources")
            .Where(file => regexCraftingList.IsMatch(Path.GetFileName(file)))
            .ToList();
        if (craftingLists.Count == 0)
        {
            Console.WriteLine("Aucune liste de craft trouvee pour l'arbre de recettes specifie, souhaitez-vous creer une liste a la volee? (Y/N):\n");
            Console.ReadLine();
        }

        return craftingLists;
    }
}

