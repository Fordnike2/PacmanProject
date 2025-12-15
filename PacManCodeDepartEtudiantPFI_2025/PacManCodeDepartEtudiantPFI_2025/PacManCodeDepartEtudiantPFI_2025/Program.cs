using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

// ---- Constantes fournies - utilisez-les ----------------------------------------------------------------------------
const int NbCaseLargeur = 19;
const int NbCaseHauteur = 21;
const int NbPixelsParCase = 16;
// Vous pouvez en ajouter d'autres ici
// Les fantômes bougent à tous les 4 frames (version simplifiée, au lieu de 4)
const int VitesseFantome = 4;

// Ajoutez vos propres constantes ici...


// ---- Sous-programmes ------------------------------------------------------------------------------------------------

// ChargerCarteJeu : fournie (ne la modifiez pas!)
// Crée un tableau 2D d'Objets (voir enum plus bas) et charge le fichier Pac-Man carte.csv
// Lit le fichier pour remplir le tableau 2D d'objets à partir des informations qui s'y trouve
// Retourne le tableau 2D rempli
static Objet[,] ChargerCarteJeu()
{
    Objet[,] carte = new Objet[NbCaseHauteur, NbCaseLargeur];
    using (StreamReader reader = new StreamReader("Pac-Man carte.csv"))
    {
        for (int indiceLigne = 0; indiceLigne < NbCaseHauteur; indiceLigne++)
        {
            string line = reader.ReadLine();
            if (line != null)
            {
                //Console.WriteLine($"{indiceLigne} {line}");
                string[] values = line.Split(',');
                for (int indiceColonne = 0; indiceColonne < NbCaseLargeur; indiceColonne++)
                {
                    carte[indiceLigne, indiceColonne] = (Objet)int.Parse(values[indiceColonne]);
                }
            }
        }
    }
    return carte;
}

// Ajoutez vos propres sous-programmes ici
// AffichageCarte : affiche la carte du jeu dans la fenêtre
static void AffichageCarte(RenderWindow window, Objet[,] carte)
{

    Texture textureMur = new Texture("images/mur.bmp");
    Texture texturePoint = new Texture("images/point.bmp");
    Sprite spriteMur = new Sprite(textureMur);
    Sprite spritePoint = new Sprite(texturePoint);
    

    for (int y = 0; y < NbCaseHauteur; y++)
    {
        for (int x = 0; x < NbCaseLargeur; x++)
        {
            switch (carte[y, x])
            {
                case Objet.Mur:
                    spriteMur.Position = new Vector2f(x * NbPixelsParCase, y * NbPixelsParCase);
                    window.Draw(spriteMur);
                    break;
                case Objet.Point:
                    spritePoint.Position = new Vector2f(x * NbPixelsParCase, y * NbPixelsParCase);
                    window.Draw(spritePoint);
                    break;
                case Objet.Vide:

                    break;
            }
        }
    }
}
// Retourne la direction opposée à la direction donnée en paramètre
static Direction directionOpposee(Direction dir)
{
    Direction directionOpposee;
    if (dir == Direction.Haut) { directionOpposee = Direction.Bas; }
    else if (dir == Direction.Bas) { directionOpposee = Direction.Haut; }
    else if (dir == Direction.Gauche) { directionOpposee = Direction.Droite; }
    else { directionOpposee = Direction.Gauche; }
    return directionOpposee;
}
// Retourne une direction aléatoire pour le fantôme

static Direction directionFantomeAleatoire(Personnage fantome, Objet[,] carte)
{
    List<Direction> directions = new List<Direction>();
    if (carte[fantome.Y - 1, fantome.X] != Objet.Mur) { directions.Add(Direction.Haut); }
    if (carte[fantome.Y + 1, fantome.X] != Objet.Mur) { directions.Add(Direction.Bas); }
    if (carte[fantome.Y, fantome.X - 1] != Objet.Mur) { directions.Add(Direction.Gauche); }
    if (carte[fantome.Y, fantome.X + 1] != Objet.Mur) { directions.Add(Direction.Droite); }
    Direction opposee = directionOpposee(fantome.Direction);
    if (directions.Count > 1)
    { directions.Remove(opposee); }
    return directions[Random.Shared.Next(0,directions.Count)];
}

// j'ai utilisé List car c'est plus simple a manipuler que des tableaux normaux




// ---- Programme principal --------------------------------------------------------------------------------------------


// Charger la carte du jeu
Objet[,] carte = ChargerCarteJeu();
// Compter le nombre de points dans la carte
int count = 0;
foreach (int value in carte)
{
    if (value == 1)
        count++;
}



// Créer la fenêtre du jeu
VideoMode mode = new VideoMode(NbCaseLargeur * NbPixelsParCase, NbCaseHauteur * NbPixelsParCase);
RenderWindow window = new RenderWindow(mode, "Pacman");
window.Closed += (s, e) => window.Close();
window.SetFramerateLimit(10);
// Créer les personnages: Pacman et les quatre fantômes
Personnage shadow = new Personnage("images/shadow.bmp", 9, 7, Direction.Droite);
Personnage bashful = new Personnage("images/bashful.bmp", 8, 9, Direction.Droite);
Personnage speedy = new Personnage("images/speedy.bmp", 9, 9, Direction.Haut);
Personnage pokey = new Personnage("images/pokey.bmp", 10, 9, Direction.Gauche);
Personnage pacman = new Personnage("images/pacman.bmp", 9, 15, Direction.Droite);

// Variables pour la boucle principale
Thread.Sleep(1000);
int compteurFrame = 0;
bool GagnerOuPerdu = false;


// Boucle principale du jeu
while ((!Keyboard.IsKeyPressed(Keyboard.Key.Escape)) && (!GagnerOuPerdu)) 
{

    window.DispatchEvents();
    window.Clear(Color.Black);
    // Déplacer Pacman en fonction des touches fléchées
    int dx = 0, dy = 0;
    if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) { dx = -1; }
    else if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) { dx = 1; }
    else if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) { dy = -1; }
    else if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) { dy = 1; }
    if (dx != 0 || dy != 0)
    {
        if (carte[pacman.Y, pacman.X] == Objet.Point)
        {
            carte[pacman.Y, pacman.X] = Objet.Vide;
            count--;
        }
        int futurX = pacman.X + dx;
        int futurY = pacman.Y + dy;
        if (carte[futurY, futurX] != Objet.Mur)
        {
            pacman.X = futurX;
            pacman.Y = futurY;


            if (dx == 1) { pacman.Direction = Direction.Droite; }
            if (dx == -1) { pacman.Direction = Direction.Gauche; }
            if (dy == -1) { pacman.Direction = Direction.Haut; }
            if (dy == 1) { pacman.Direction = Direction.Bas; }
        }

    }
    // Déplacer les fantômes (déplacement aléatoire à chaque 4 frames)
    if (compteurFrame % 4 == 0)
    {
        Direction dirShadow = directionFantomeAleatoire(shadow, carte);
        if (dirShadow == Direction.Haut) { shadow.Y--; }
        if (dirShadow == Direction.Bas) { shadow.Y++; }
        if (dirShadow == Direction.Gauche) { shadow.X--; }
        if (dirShadow == Direction.Droite) { shadow.X++; }
        shadow.Direction = dirShadow;
        Direction dirPokey = directionFantomeAleatoire(pokey, carte);
        if (dirPokey == Direction.Haut) { pokey.Y--; }
        if (dirPokey == Direction.Bas) { pokey.Y++; }
        if (dirPokey == Direction.Gauche) { pokey.X--; }
        if (dirPokey == Direction.Droite) { pokey.X++; }
        pokey.Direction = dirPokey;
        Direction dirBashful = directionFantomeAleatoire(bashful, carte);
        if (dirBashful == Direction.Haut) { bashful.Y--; }
        if (dirBashful == Direction.Bas) { bashful.Y++; }
        if (dirBashful == Direction.Gauche) { bashful.X--; }
        if (dirBashful == Direction.Droite) { bashful.X++; }
        bashful.Direction = dirBashful;
        Direction dirSpeedy = directionFantomeAleatoire(speedy, carte);
        if (dirSpeedy == Direction.Haut) { speedy.Y--; }
        if (dirSpeedy == Direction.Bas) { speedy.Y++; }
        if (dirSpeedy == Direction.Gauche) { speedy.X--; }
        if (dirSpeedy == Direction.Droite) { speedy.X++; }
        speedy.Direction = dirSpeedy;
    }

    // Afficher la carte et les personnages
    compteurFrame++;
    AffichageCarte(window, carte);
    pacman.Afficher(window, NbPixelsParCase);
    shadow.Afficher(window, NbPixelsParCase);
    bashful.Afficher(window, NbPixelsParCase);
    speedy.Afficher(window, NbPixelsParCase);
    pokey.Afficher(window, NbPixelsParCase);
    // Vérifier les conditions de fin de partie (collision avec un fantôme ou tous les points mangés)
    if ((pacman.X == shadow.X && pacman.Y == shadow.Y) || (pacman.X == bashful.X && pacman.Y == bashful.Y) || (pacman.X == pokey.X && pacman.Y == pokey.Y) || (pacman.X == speedy.X && pacman.Y == speedy.Y))
    {
        Texture Perdu = new Texture("images/perdu.bmp");
        Sprite PerduSprite = new Sprite(Perdu);
        PerduSprite.Position = new Vector2f(5 * NbPixelsParCase, 2 * NbPixelsParCase);
        window.Draw(PerduSprite);
        window.Display();
        Thread.Sleep(5000);
        GagnerOuPerdu = true;
    }
    if (count == 0)
    {
        Texture Gagne = new Texture("images/gagne.bmp");
        Sprite GagneSprite = new Sprite(Gagne);
        GagneSprite.Position = new Vector2f(5 * NbPixelsParCase, 2 * NbPixelsParCase);
        window.Draw(GagneSprite);
        window.Display();
        Thread.Sleep(5000);
        GagnerOuPerdu = true;
    }
    window.Display();
    
}



// ---- Classe Personnage - utilisez-la, mais ne modifiez pas son contenu! ---------------------------------------------


/// <summary>
/// La classe Personnage sert à créer Pacman et les quatre fantômes
/// </summary>
class Personnage
{
    /// <summary>
    /// Constructeur du personnage: pour créer un personnage, utilisez l'opérateur new
    /// </summary>
    /// <param name="fichierImage">Nom du fichier image</param>
    /// <param name="x">Position initiale en x</param>
    /// <param name="y">Position initiale en y</param>
    /// <param name="dir">Direction intiale</param>
    public Personnage(string fichierImage, int x, int y, Direction dir)
    {
        X = x;
        Y = y;
        Direction = dir;
        Texture = new Texture(fichierImage);

        TextureEnFuite = new Texture("images/peur.bmp");
        Sprite = new Sprite(Texture);
    }

    /// <summary>
    /// Utilisez la méthode Afficher du personnage pour afficher le personnage
    /// dans une fenêtre
    /// </summary>
    /// <param name="fenetre">Objet représentant la fenêtre</param>
    /// <param name="echelle">Facteur multiplicatif permettant de passer de la position
    /// dans le tableau 2D à la position dans l'image</param>
    public void Afficher(RenderWindow fenetre, int echelle)
    {
        Sprite.Position = new Vector2f(echelle * X, echelle * Y);
        fenetre.Draw(Sprite);
    }
    /// <value>
    /// Utilisez la propriété X afin de modifier la position horizontale
    /// </value>     
    public int X { get; set; }
    /// <value>
    /// Utilisez la propriété Y afin de modifier la position verticale
    /// </value>     
    public int Y { get; set; }
    /// <value>
    /// Utilisez la propriété Direction afin de conserver la direction du personnage
    /// </value>     
    public Direction Direction { get; set; }

    /// <value>
    /// Modifiez cette propriété pour indiquer qu'un fantome est en fuite (bonus suelement)
    /// </value>
    public bool EstEnFuite
    {
        get => estEnFuite;
        set => Sprite.Texture = new Texture((estEnFuite = value) ? TextureEnFuite : Texture);
    }
    #region
    private bool estEnFuite = false;
    private Texture Texture { get; set; }
    private Texture TextureEnFuite { get; set; }
    private Sprite Sprite { get; set; }
    #endregion
}




// ---- Enumérations --------------------------------------------------------------------------------------------------

// Énumération pour représenter une case du labyrinthe: Vide, Mur, Point, Power pellet (pour le bonus)
enum Objet { Mur, Point, Vide, Power };

// Énumération pour représenter une direction
enum Direction { Haut, Bas, Gauche, Droite };

// Vous pouvez ajouter d'autres enums si vous y voyez une utilité.