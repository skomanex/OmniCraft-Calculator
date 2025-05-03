using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Schema;

public class Program
{
    static void Main()
    {
        bool done = false;

        Console.WriteLine("Bienvenu dans l'Omnicrafter de Jimmy");

        while (!done)
        {
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

            string selectedList = SelectCraftingList(craftingLists);

            if (selectedList == "")
            {
                //#TODO Generation liste a volee
                Console.WriteLine("TODO generation liste a volee");
                selectedList = "";
            }

            SolveList(selectedTree, selectedList);

            done = ContinueProgram(done);
        }

        Console.WriteLine("Merci d'avoir utilise l'Omnicrafter de Jimmy");

        Environment.Exit(0);
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
        string selectedTree = "";
        int choice;

        Console.WriteLine($"Veuillez choisir un arbre de craft (entrer un nombre entre 0 et {craftingTrees.Count - 1})");

        while (!(int.TryParse(Console.ReadLine(), out choice) && choice >= 0 && choice < craftingTrees.Count))
        {
            Console.WriteLine($"Entree utilisateur erronee, entrer un nombre entre 0 et {craftingTrees.Count - 1}");
        }
        Console.WriteLine($"Vous avez selectionne : {Path.GetFileName(selectedTree)}");
        selectedTree = craftingTrees[choice];
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
            while (true)
            {
                if (Console.ReadLine().ToLower().StartsWith("y"))
                {
                    return craftingLists;
                }
                else if (Console.ReadLine().ToLower().StartsWith("n"))
                {
                    Console.WriteLine("L'utilisateur ne souhaite pas generer de liste a la volee et aucune liste trouvee, merci d'ajouter une liste de craft dans Data Sources");
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("Entree utilisateur incorrecte, entrez Y ou N");
                }
            }
        }

        return craftingLists;
    }

    static string SelectCraftingList(List<string> craftingLists)
    {
        string selectedList = "";
        int choice;

        if (craftingLists.Count == 0)
        {
            return selectedList;
        }

        Console.WriteLine($"Veuillez choisir une arbre de craft (entrer un nombre entre 0 et {craftingLists.Count - 1}), -1 pour generer la liste a la volee");

        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                if (choice == -1)
                {
                    return selectedList;
                }
                else if (choice >= 0 && choice < craftingLists.Count)
                {
                    Console.WriteLine($"Vous avez selectionne : {Path.GetFileName(selectedList)}");
                    return craftingLists[choice];
                }
                else
                {
                    Console.WriteLine($"Entree utilisateur erronee,  veuillez entrer un nombre entre 0 et {craftingLists.Count - 1}, ou -1 pour generer la liste a la volee.");
                }
            }
            else
            {
                Console.WriteLine($"Entree utilisateur erronee,  veuillez entrer un nombre entre 0 et {craftingLists.Count - 1}, ou -1 pour generer la liste a la volee.");
            }
        }
    }

    static void SolveList(string selectedTree, string selectedList)
    {
        // #TODO Solutionner les crafts
        Console.WriteLine("TODO Solutionner les crafts");
        return;
    }

    static bool ContinueProgram(bool done)
    {
        Console.WriteLine("Souhaitez-vous continuer? (Y/N)");
        while (true)
        {
            string userInput = Console.ReadLine();
            if (userInput.ToLower().StartsWith("y"))
            {
                done = false;
                return done;
            }
            else if (userInput.ToLower().StartsWith("n"))
            {
                done = true;
                return done;
            }
            else
            {
                Console.WriteLine("Entree utilisateur incorrecte, entrez Y ou N");
            }
        }
    }
}

