using System;
using System.Collections.Generic;
using System.Linq;

namespace CatalogoRevistas
{
    class Program
    {
        // Catálogo inicial 10 títulos
        static List<string> catalogo = new List<string>
        {
            "National Geographic",
            "Scientific American",
            "Time",
            "The Economist",
            "Wired",
            "Revista Educacion",
            "Nature",
            "PC Magazine",
            "Forbes",
            "Revista Tecnologia"
        };

        static void Main(string[] args)
        {
            Console.Title = "Catálogo de Revistas - Búsqueda";
            bool salir = false;

            while (!salir)
            {
                MostrarMenu();
                Console.Write("Seleccione una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        MostrarCatalogo();
                        break;
                    case "2":
                        BuscarIterativa();
                        break;
                    case "3":
                        BuscarRecursiva();
                        break;
                    case "4":
                        AgregarTitulo();
                        break;
                    case "5":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Intente nuevamente.");
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }

            Console.WriteLine("Gracias por usar el catálogo. ¡Hasta luego!");
        }

        static void MostrarMenu()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("  Catálogo de Revistas - Menú Principal");
            Console.WriteLine("=======================================");
            Console.WriteLine("1. Mostrar catálogo");
            Console.WriteLine("2. Buscar título (Búsqueda iterativa - lineal)");
            Console.WriteLine("3. Buscar título (Búsqueda recursiva - binaria)");
            Console.WriteLine("4. Agregar un título al catálogo");
            Console.WriteLine("5. Salir");
            Console.WriteLine("=======================================");
        }

        static void MostrarCatalogo()
        {
            Console.WriteLine("\nCatálogo actual:");
            for (int i = 0; i < catalogo.Count; i++)
            {
                Console.WriteLine($"{i+1}. {catalogo[i]}");
            }
        }

        
        // BÚSQUEDA ITERATIVA 
     
        static void BuscarIterativa()
        {
            Console.Write("\nIngrese el título a buscar: ");
            string titulo = Console.ReadLine()?.Trim() ?? "";

            bool encontrado = BusquedaLineal(catalogo, titulo);

            if (encontrado)
                Console.WriteLine("Encontrado");
            else
                Console.WriteLine("No encontrado");
        }

        // Implementación de búsqueda lineal (iterativa), case-insensitive
        static bool BusquedaLineal(List<string> lista, string objetivo)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (string.Equals(lista[i], objetivo, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        // -------------------------------------------------------
        // BÚSQUEDA RECURSIVA (BINARIA)
        // -------------------------------------------------------
        static void BuscarRecursiva()
        {
            Console.Write("\nIngrese el título a buscar (búsqueda recursiva): ");
            string titulo = Console.ReadLine()?.Trim() ?? "";

            // Para aplicar búsqueda binaria debemos ordenar copia del catálogo.
            // Usamos StringComparer.OrdinalIgnoreCase para comparar sin distinguir mayúsculas.
            List<string> ordenada = catalogo
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int indice = BusquedaBinariaRecursiva(ordenada, titulo, 0, ordenada.Count - 1);

            if (indice != -1)
                Console.WriteLine("Encontrado");
            else
                Console.WriteLine("No encontrado");
        }

        // Wrapper recursivo que devuelve el índice si lo encuentra o -1 si no.
        static int BusquedaBinariaRecursiva(List<string> lista, string objetivo, int bajo, int alto)
        {
            if (bajo > alto)
                return -1;

            int medio = bajo + (alto - bajo) / 2;

            // Compare usando StringComparison.OrdinalIgnoreCase
            int comparacion = string.Compare(lista[medio], objetivo, StringComparison.OrdinalIgnoreCase);

            if (comparacion == 0)
                return medio;
            else if (comparacion < 0)
                // lista[medio] < objetivo -> buscar a la derecha
                return BusquedaBinariaRecursiva(lista, objetivo, medio + 1, alto);
            else
                // lista[medio] > objetivo -> buscar a la izquierda
                return BusquedaBinariaRecursiva(lista, objetivo, bajo, medio - 1);
        }

        static void AgregarTitulo()
        {
            Console.Write("\nIngrese el nuevo título a agregar: ");
            string nuevo = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(nuevo))
            {
                Console.WriteLine("Título vacío. No se agregó nada.");
                return;
            }

            catalogo.Add(nuevo);
            Console.WriteLine($"Título \"{nuevo}\" agregado al catálogo.");
        }
    }
}
