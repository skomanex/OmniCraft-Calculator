# Bienvenue sur le repo de l'OmniCraft Calculator

L'objectif de cet outil est de proposer une solution suffisamment générique pour calculer les quantités d'ingrédients nécessaires aux crafts dans les jeux utilisant ce système.

## Utilisation

Le programme utilise deux sources de données :
- **Crafting Trees** : Contiennent la liste de tous les objets du jeu (ou au moins l'arborescence nécessaire pour solutionner un craft).
- **Crafting Lists** : Fichiers contenant les crafts à solutionner.

Des exemples de ces fichiers sont fournis dans le repo. Il est nécessaire de suivre la mise en forme et le schéma de nommage des fichiers tests si vous souhaitez ajouter d'autres fichiers.

## Points d'amélioration

- **Ajout de commentaires** : Le code contient peu (pas) de commentaires en dehors des summary des différentes méthodes, ceci est du au fait que la plupart des méthodes ne font que le gestion d'entrées utilisateur et je ne les juge pas intéressantes. 

- **Refactorisation du code** : Le code devrait être découpé en différentes classes/fichiers. Au début du projet, il me semblait faisable de tout garder dans un fichier `Program`, mais la gestion des entrées utilisateur et notamment des erreurs s'est avérée plus lourde que prévu. Le fichier unique compte presque 500 lignes, avec des méthodes dépassant parfois les 50 lignes. Cependant, le code reste sectionné en méthodes de manière logique, avec la quasi-totalité de la résolution se trouvant dans `SolveList`.

- **Sauvegarde des listes de crafting** : La sauvegarde des listes de crafting générées à la volée nécessite un ajustement du chemin pour placer les solutions dans le dossier `Data Outputs`, car la console s'exécute en mode debug. De plus, les listes générées à la volée ne se sauvegardent pas dans un format exploitable par le programme (mauvais formatage du JSON).

- **Schéma de nommage des sauvegardes** : Le schéma de nommage utilise la date de la sauvegarde mais devrait simplement incrémenter. Cependant, cela entraînerait trop de code ou de passage d'arguments, donc j'ai choisi la solution de facilité ici.

- **Amélioration du solver** : Le solver reste un solver récursif classique et ne gère pas les recettes qui donneraient en résultat des objets passés en entrée. Il n'y a pas non plus de gestion des recettes donnant plus d'un objet par craft (au sens de la variété, pas de la quantité).

- **Utilisation de LINQ** : Le code utilise LINQ de manière intéressante quasi uniquement dans la génération de listes à la volée `GenerateOnTheFly` (filtrage des résultats, etc.).
