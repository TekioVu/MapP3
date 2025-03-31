//Diego García Alonso
//Sergio Valiente Urueña

using System;

namespace hidato
{
    class Program
    {
        struct Posicion
        {
            public int fil, col;
        };
        struct Casilla
        {
            public int num;   // número de la casilla (-1: no jugable, 0: vacía)
            public bool fija; // Indica si la casilla es fija (true) o editable (false)
        };
        struct Tablero
        {
            public Casilla[,] cas; // matriz de casillas
            public Posicion cursor; // posición actual del cursor
        };

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            int finPartida = 0;
  
            Tablero tab;
            Posicion pos;

            int[,] matInicial = CargaMatriz("hid1.txt");
            int[,] mat = matInicial;
            //Console.SetWindowSize(mat.GetLength(0) * 4, mat.GetLength(1) * 2);

            Inicializa(mat, out tab);
            Render(tab);

            while (finPartida == 0 && !Comprueba(tab, out pos))
            {
                char ch = LeeInput();
                if(ch == 'q') finPartida = 1;

                //Reinicia la partida
                if (ch == 'p') 
                {
                    mat = matInicial;
                    Inicializa(mat, out tab);
                }

                //Pista
                if (ch == 'c') RenderCont(tab, pos);

                ActualizaEstado(ch, ref tab);
                if (ch != ' ' && ch != 'c') Render(tab);
            }

            if(finPartida == 0)
            {
                Console.Clear();
                Console.WriteLine("YOU WIN");
            }
        }

        static char LeeInput()
        {
            char d = ' ';

            if (Console.KeyAvailable)
            {
                string tecla = Console.ReadKey(true).Key.ToString();
                switch (tecla)
                {

                    /* INPUTS ELEMENTALES PARA EL JUEGO BÁSICO */

                    // movimiento del cursor 	
                    case "LeftArrow": d = 'l'; break;
                    case "UpArrow": d = 'u'; break;
                    case "RightArrow": d = 'r'; break;
                    case "DownArrow": d = 'd'; break;

                    // comprobar tablero 
                    case "c": case "C": d = 'c'; break;

                    // terminar juego
                    case "Escape": case "q": case "Q": d = 'q'; break;
                    //reinicar juego
                    case "r": case "R": d = 'p'; break;

                    // ver posibles valores en posicion	
                    default:
                        if (tecla.Length == 2 && tecla[0] == 'D' && tecla[1] >= '0' && tecla[1] <= '9')
                            d = tecla[1];
                        break;
                }
            }
            return d;
        }
        static void Inicializa(int[,] mat, out Tablero tab)
        {
            tab.cas = new Casilla[mat.GetLength(0), mat.GetLength(1)];

            tab.cursor.fil = 0;
            tab.cursor.col = 0;

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    tab.cas[i, j].fija = !(mat[i, j] == -1 || mat[i, j] == 0);
                    tab.cas[i, j].num = mat[i, j];
                }
            }
        }

        static void Render(Tablero tablero)
        {
            //Render Tablero
            for (int i = 0; i < tablero.cas.GetLength(0); i++)
            {
                for (int j = 0; j < tablero.cas.GetLength(1); j++)
                {
                    if (tablero.cas[i, j].num != -1)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = tablero.cas[i, j].fija ? ConsoleColor.Green : ConsoleColor.Black;
                        Console.Write($"{tablero.cas[i, j].num,2}");
                    }
                    else Console.Write("  ");

                    //Espacio entre celdas horizontales
                    Console.ResetColor();
                    Console.Write("  ");
                }
                //Espacio entre celdas verticales
                Console.WriteLine("\n");
            }

            //Render Cursor
            Console.SetCursorPosition(tablero.cursor.col * 4, tablero.cursor.fil * 2);
            if (tablero.cas[tablero.cursor.fil, tablero.cursor.col].fija)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.Write($"{tablero.cas[tablero.cursor.fil, tablero.cursor.col].num,2}");

            //Reset
            Console.ResetColor();
            Console.SetCursorPosition(0, 0);
        }

        static void ActualizaEstado(char c, ref Tablero tab)
        {
            //Movimiento cursor
            if (c == 'l' && tab.cursor.col > 0 && tab.cas[tab.cursor.col - 1, tab.cursor.fil].num != -1)
                tab.cursor.col--;

            else if (c == 'r' && tab.cursor.col < tab.cas.GetLength(1) - 1 && tab.cas[tab.cursor.col + 1, tab.cursor.fil].num != -1)
                tab.cursor.col++;

            else if (c == 'u' && tab.cursor.fil > 0 && tab.cas[tab.cursor.col, tab.cursor.fil - 1].num != -1)
                tab.cursor.fil--;

            else if (c == 'd' && tab.cursor.fil < tab.cas.GetLength(0) - 1 && tab.cas[tab.cursor.col, tab.cursor.fil + 1].num != -1)
                tab.cursor.fil++;


            //Escritura número
            if (!tab.cas[tab.cursor.fil, tab.cursor.col].fija && c >= '0' && c <= '9')
            {
                //Variable para mayor claridad de codigo
                int actual = tab.cas[tab.cursor.fil, tab.cursor.col].num;
              
                tab.cas[tab.cursor.fil, tab.cursor.col].num = (actual == 0 || actual >= 10) ?
                (c - '0') : actual * 10 + (c - '0');
            }
        }

        static Posicion Busca1(Tablero tab)
        {
            Posicion pos = new Posicion();
            pos.fil = -1;

            int i = 0, j = 0;
            while(pos.fil == -1) //Búsqueda -> While
            {
                if (tab.cas[i, j].num == 1)
                    (pos.fil, pos.col) = (i,j);

                //Recorrido por la matriz sin necesidad de condicionales
                j += (i + 1) / tab.cas.GetLength(0);
                i = (i + 1) % tab.cas.GetLength(0);
            }
            
            return pos;
        }

        static int Ultimo(Tablero tab)
        {
            int cont = 0;

            //Se hace un recorrido por la matriz, contando el número de celdas que no son -1
            for (int i = 0; i < tab.cas.GetLength(0); i++)
                for (int j = 0; j < tab.cas.GetLength(1); j++)
                    if (tab.cas[i, j].num != -1) cont++;

            return cont;
        }

        static bool Siguiente(Tablero tab, Posicion pos, out Posicion pos1)
        {
            bool adyacente = false; 

            //Nuevas variables para almacenar las direcciones a chequear
            int[] dirX = { -1, 1, 0, 0, -1, 1, -1, 1 };  
            int[] dirY = { 0, 0, -1, 1, -1, -1, 1, 1 };

            //Hay que darle valor a pos1, asi que se aprovecha como condición del while
            int i = 0;
            pos1.fil = -1;
            pos1.col = -1;

            while (pos1.fil == -1 && i < dirX.Length)
            {
                //Se calcula la nueva casilla
                int newFil = pos.fil + dirX[i];
                int newCol = pos.col + dirY[i];

                //Se verifica que este en los límites
                if (newFil >= 0 && newFil < tab.cas.GetLength(0) && newCol >= 0 && newCol < tab.cas.GetLength(1))
                {
                    if (tab.cas[newFil, newCol].num == tab.cas[pos.fil, pos.col].num + 1)
                    {
                        pos1.fil = newFil;
                        pos1.col = newCol;
                        adyacente = true;  
                    }
                }
                i++;
            }

            return adyacente;  
        }

        static bool Comprueba(Tablero tab, out Posicion pos)
        {
            bool completo = false;

            pos = Busca1(tab);
            int maxNum = Ultimo(tab);
            Posicion pos1;

            int i = 1;
            while(Siguiente(tab, pos, out pos1) && i < maxNum)
            {
                pos = pos1;
                i++;
            }

            if(i == maxNum) completo = true;
          
            return completo;
        }

        static void RenderCont(Tablero tab, Posicion pos)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = tab.cas[pos.fil, pos.col].fija ? ConsoleColor.Green : ConsoleColor.Black;
            Console.SetCursorPosition(pos.col * 4, pos.fil * 2); ;
            Console.Write($"{tab.cas[pos.fil, pos.col].num,2}");

            Console.ResetColor();
            Console.SetCursorPosition(0, 0);
        }

        static int[,] CargaMatriz(string file)
        {
            StreamReader _file = new StreamReader(file);
            string[] s = _file.ReadLine().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

            int fil = int.Parse(s[0]);
            int col = int.Parse(s[1]);
            int[,] matriz = new int[fil, col];
            
            for (int i = 0; i < col; i++)
            {
                s = _file.ReadLine().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < fil; j++)
                matriz[i, j] = int.Parse(s[j]);
            }

            _file.Close();
            return matriz;
        }

    }

}

