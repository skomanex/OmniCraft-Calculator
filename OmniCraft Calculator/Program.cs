using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Xml.Schema;

public class Program
{
    static void Main()
    {
        Console.WriteLine("Bienvenu dans l'Omnicrafter de Jimmy");

        // Trouve les fichiers qui commencent par "craftingtree" qui sont l'ensemble des listes de recettes (une par jeu)
        Regex regexCraftingTree = new Regex(@"^craftingtree_", RegexOptions.IgnoreCase);
        List<string> craftingTrees = Directory
            .GetFiles("Data Sources")
            .Where(file => regexCraftingTree.IsMatch(Path.GetFileName(file)))
            .ToList();

        // Affiche la totalités des listes de recettes trouvées
        if (craftingTrees.Count == 0) {
            Console.WriteLine("Aucune liste de recettes pour craft trouvee, veuillez en renseigner au moins une dans le dossier Data Sources");
            Environment.Exit(1);
        }
        else
        {
            Console.WriteLine($"{craftingTrees.Count} liste(s) de recettes ont été trouvee(s):");
            foreach (var craftingTree in craftingTrees.Select((value, index) => (value, index)))
            {
                Console.WriteLine($"{craftingTree.index + 1}. {Path.GetFileName(craftingTree.value)}");
            }
        }
        Console.ReadLine();
    }
}

