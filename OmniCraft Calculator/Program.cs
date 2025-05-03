using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

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

            JArray[] workableObjects = GenerateJArrays(selectedList, selectedTree);

            SolveList(workableObjects[0], workableObjects[1]);

            done = ContinueProgram(done);
        }

        Console.WriteLine("Merci d'avoir utilise l'Omnicrafter de Jimmy");

        Environment.Exit(0);
    }

    /// <summary>
    /// Trouve les différents arbres de recettes présent dans Data Sources ou quitte le programme si aucun arbre de recettes n'est trouvé.
    /// </summary>
    /// <returns>Une liste d'arbre de recettes ou quitte le programme</returns>
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

    /// <summary>
    /// Permet à l'utilisateur de choisir l'arbre de recettes qu'il souhaite pour la suite de l'exécution du programme.
    /// </summary>
    /// <param name="craftingTrees"></param>
    /// <returns>Un arbre de recettes</returns>
    static string SelectCraftingTree(List<string> craftingTrees)
    {
        string selectedTree = "";
        int choice;

        Console.WriteLine($"Veuillez choisir un arbre de craft (entrer un nombre entre 0 et {craftingTrees.Count - 1})");

        while (!(int.TryParse(Console.ReadLine(), out choice) && choice >= 0 && choice < craftingTrees.Count))
        {
            Console.WriteLine($"Entree utilisateur erronee, entrer un nombre entre 0 et {craftingTrees.Count - 1}");
        }
        selectedTree = craftingTrees[choice];
        Console.WriteLine($"Vous avez selectionne : {Path.GetFileName(selectedTree)}");
        return selectedTree;
    }

    /// <summary>
    /// Trouve l'ensemble des listes de craft dans Data Sources pour l'arbre de recettes spécifié
    /// ou quitte le programme si aucune n'est trouvée et l'utilisateur ne souhaite pas en générer une à la volée.
    /// </summary>
    /// <param name="selectedTree"></param>
    /// <returns>Une liste de listes de craft ou une liste vide ou quitte le programme</returns>
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

    /// <summary>
    /// Permet à l'utilisateur de choisir la liste de craft qu'il souhaite pour la suite du programme
    /// </summary>
    /// <param name="craftingLists"></param>
    /// <returns>Une liste de craft ou rien</returns>
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
                    selectedList = craftingLists[choice];
                    Console.WriteLine($"Vous avez selectionne : {Path.GetFileName(selectedList)}");
                    return selectedList;
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

    /// <summary>
    /// Utilise les noms de fichiers déterminés par les fonctions <see cref="SelectCraftingList"/> et <see cref="SelectCraftingTree"/> pour générer un Array de JArray
    /// </summary>
    /// <param name="list"></param>
    /// <param name="tree"></param>
    /// <returns>Un Array de JArray</returns>
    static JArray[] GenerateJArrays(string list, string tree)
    {
        JArray[] result = { new JArray(), new JArray() };

        JArray treeItems = (JArray)JObject.Parse(File.ReadAllText(tree))["items"];
        result[0] = treeItems;

        if (list == "")
        {
            result[1] = GenerateOnTheFly(treeItems);
        }
        else
        {
            JObject crafts = (JObject)JObject.Parse(File.ReadAllText(list))["crafts"];
            foreach (var craft in crafts)
            {
                JObject craftItem = new JObject { ["name"] = craft.Key, ["quantity"] = craft.Value };
                result[1].Add(craftItem);
            }
        }

        return result;
    }

    /// <summary>
    /// Génère une liste de craft à la volée en demandant à l'utilisateur de saisir le nom ou l'index de l'objet qu'il souhaite ajouter, ainsi que la quantité.
    /// Lui propose de la sauvegarder si il le souahite en suivant la nomenclature
    /// </summary>
    /// <param name="items"></param>
    /// <returns>Une liste de craft</returns>
    static JArray GenerateOnTheFly(JArray items)
    {
        JArray result = new JArray();

        Console.WriteLine("Veuillez entrer le nom ou l'index de l'objet que vous souhaitez ajouter suivi de la quantite, si seule une partie du nom est saisie la liste sera filtree");
        Console.WriteLine("Saisir -1 pour finir l'ajout, -2 pour reinitialiser l'affichage des objets de l'arbre de recettes");
        Console.WriteLine($"{items.Count} objet(s) sont disponnible(s) a l'ajout de la liste de craft");

        foreach (var item in items
            .Select((value, index) => (value, index)))
        {
            Console.WriteLine(item.index + ". " + item.value["name"]);
        }
        while (true)
        {
            string[] userInput = Console.ReadLine().Split(' ');
            int index;
            int quantity;

            if (userInput.Length == 2 && int.TryParse(userInput[0], out index) && index >= 0 && index < items.Count && int.TryParse(userInput[1], out quantity))
            {
                JObject craftItem = new JObject { 
                    ["name"] = items[index]["name"],
                    ["quantity"] = quantity 
                };
                Console.WriteLine($"Vous avez ajoute : {items[index]["name"]} x {quantity} a la liste de craft");
                result.Add(craftItem);
            }
            else if (userInput.Length == 2 && int.TryParse(userInput[1], out quantity) && items
                .Where(item => item["name"].ToString()
                .Contains(userInput[0].ToLower()))
                .Count() != 0)
            {
                JObject craftItem = new JObject { 
                    ["name"] = items
                    .Where(item => item["name"].ToString()
                    .Contains(userInput[0].ToLower()))
                    .First()["name"].ToString(),
                    ["quantity"] = quantity
                };

                Console.WriteLine($"Vous avez ajoute : {items
                    .Where(item => item["name"].ToString()
                    .Contains(userInput[0].ToLower()))
                    .First()["name"]} x {quantity} a la liste de craft");
                result.Add(craftItem);
            }
            else if (userInput[0] == "-1")
            {
                if(result.Count() == 0)
                {
                    Console.WriteLine("Aucun objet n'a ete ajoute a la liste de craft, veuillez en ajouter au moins 1");
                }
                else
                {
                    Console.WriteLine("Fin de l'ajout d'objets a la liste de craft");
                    break;
                }
            }
            else if (userInput[0] == "-2")
            {
                Console.Clear();
                Console.WriteLine("Reinitialisation de l'affichage des objets de l'arbre de recettes");
                foreach (var item in items
                    .Select((value, index) => (value, index)))
                {
                    Console.WriteLine(item.index + ". " + item.value["name"]);
                }
            }
            else
            {
                Console.WriteLine("Entree utilisateur incorrecte, veuillez entrer le nom ou l'index de l'objet que vous souhaitez ajouter suivi de la quantite");
            }
        }
        Console.WriteLine("Souhaitez-vous sauvegarder la liste de craft? (Y/N)");
        if(Console.ReadLine().ToLower().StartsWith("y"))
        {
            string fileName = $"craftinglist_{Path.GetFileNameWithoutExtension(items.ToString())}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            File.WriteAllText(Path.Combine("Data Outputs", fileName), result.ToString());
            Console.WriteLine($"Liste de craft sauvegardee sous le nom : {fileName}");
        }
        else
        {
            Console.WriteLine("Liste de craft non sauvegardee");
        }
        return result;
    }

    /// <summary>
    /// Utilise deux JArrays, un qui représente tous les craft possible d'un jeu et l'autre les crafts et objets voulus pour afficher à l'utilisateur la totalité
    /// des crafts et matières premières nécessaires à l'accomplissement de sa lsite de craft.
    /// Propose à l'utilisateur de sauvegarder la solution au format JSON ou TXT dans Data Outputs selon ses préférences.
    /// </summary>
    /// <param name="craftTree"></param>
    /// <param name="craftList"></param>
    static void SolveList(JArray craftTree, JArray craftList)
    {
        // #TODO Solutionner les crafts
        Console.WriteLine("TODO Solutionner les crafts");
        return;
    }

    /// <summary>
    /// Inverse ou non le booléen en entrée, utilisé pour confirmer ou non la continuation du programme
    /// </summary>
    /// <param name="done"></param>
    /// <returns>Un booléen</returns>
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