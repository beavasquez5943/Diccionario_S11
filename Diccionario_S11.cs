using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

class TranslatorApp
{
    static Dictionary<string,string> engToSpa = new Dictionary<string,string>();
    static Dictionary<string,string> spaToEng = new Dictionary<string,string>();
    static void Main()
    {
        SeedDictionary();
        RunMenu();
    }

    static void RunMenu()
    {
        while (true)
        {
            Console.WriteLine("\n==================== MENÚ ====================\n");
            Console.WriteLine("1. Traducir una frase");
            Console.WriteLine("2. Agregar palabras al diccionario");
            Console.WriteLine("0. Salir\n");
            Console.Write("Seleccione una opción: ");

            string option = Console.ReadLine()?.Trim() ?? "";

            switch (option)
            {
                case "1":
                    TranslatePhrase();
                    break;
                case "2":
                    AddWordInteractive();
                    break;
                case "0":
                    Console.WriteLine("Saliendo. ¡Hasta luego!");
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente nuevamente.");
                    break;
            }
        }
    }

    static void TranslatePhrase()
    {
        Console.Write("\nIngrese la frase a traducir:\n> ");
        string input = Console.ReadLine() ?? "";

        Console.WriteLine("\nSeleccione la dirección de traducción:");
        Console.WriteLine("1. Inglés -> Español");
        Console.WriteLine("2. Español -> Inglés");
        Console.Write("Opción: ");
        string dir = Console.ReadLine()?.Trim() ?? "";

        Dictionary<string,string> dict;
        if (dir == "1") dict = engToSpa;
        else if (dir == "2") dict = spaToEng;
        else
        {
            Console.WriteLine("Dirección inválida. Se cancela la traducción.");
            return;
        }

        string result = TranslateKeepingPunctuation(input, dict);
        Console.WriteLine("\nTraducción parcial (solo palabras registradas traducidas):");
        Console.WriteLine(result);
    }

    static void AddWordInteractive()
    {
        Console.WriteLine("\nAgregar palabra al diccionario.");
        Console.WriteLine("Indique la dirección del par:");
        Console.WriteLine("1. Inglés -> Español");
        Console.WriteLine("2. Español -> Inglés");
        Console.Write("Opción: ");
        string opt = Console.ReadLine()?.Trim() ?? "";

        if (opt != "1" && opt != "2")
        {
            Console.WriteLine("Opción inválida. Cancelado.");
            return;
        }

        if (opt == "1")
        {
            Console.Write("Palabra en inglés: ");
            string eng = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Traducción al español (puede incluir variantes separadas por '/'): ");
            string spa = Console.ReadLine()?.Trim() ?? "";
            AddPair(eng, spa);
            Console.WriteLine($"Añadido: {eng} -> {spa}");
        }
        else
        {
            Console.Write("Palabra en español: ");
            string spa = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Traducción al inglés (puede incluir variantes separadas por '/'): ");
            string eng = Console.ReadLine()?.Trim() ?? "";
            AddPair(eng, spa);
            Console.WriteLine($"Añadido: {spa} -> {eng}");
        }
    }

    static void AddPair(string eng, string spa)
    {
        if (string.IsNullOrWhiteSpace(eng) || string.IsNullOrWhiteSpace(spa)) return;

        string engKey = Normalize(eng.Split('/')[0]);
        string spaKey = Normalize(spa.Split('/')[0]);

        // store english -> spanish (whole value with possible / variants)
        if (!engToSpa.ContainsKey(engKey))
            engToSpa[engKey] = spa;
        else
            engToSpa[engKey] = spa; // actualizar

        if (!spaToEng.ContainsKey(spaKey))
            spaToEng[spaKey] = eng;
        else
            spaToEng[spaKey] = eng; // actualizar
    }

    static string TranslateKeepingPunctuation(string text, Dictionary<string,string> dict)
    {
        // Tokenizar en secuencias de letras (\p{L}+) o secuencias de no-letras (\P{L}+)
        var matches = Regex.Matches(text, @"\p{L}+|\P{L}+");
        var sb = new StringBuilder();

        foreach (Match m in matches)
        {
            string token = m.Value;
            if (token.All(c => Char.IsLetter(c)))
            {
                string normalized = Normalize(token);
                if (dict.TryGetValue(normalized, out string translatedFull))
                {
                    string chosen = translatedFull.Split('/')[0].Trim();
                    chosen = MatchCapitalization(token, chosen);
                    sb.Append(chosen);
                }
                else
                {
                    sb.Append(token);
                }
            }
            else
            {
                sb.Append(token);
            }
        }
        return sb.ToString();
    }

    static string Normalize(string s)
    {
        if (s == null) return "";
        s = s.Trim().ToLowerInvariant();
        string formD = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in formD)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    static string MatchCapitalization(string original, string translation)
    {
        if (string.IsNullOrEmpty(translation)) return translation;
        if (original == original.ToUpperInvariant())
            return translation.ToUpperInvariant();
        if (char.IsUpper(original[0]))
            return char.ToUpper(translation[0]) + (translation.Length > 1 ? translation.Substring(1) : "");
        return translation;
    }

    static void SeedDictionary()
    {
        // Lista sugerida (al menos 10)
        AddPair("Time", "tiempo");
        AddPair("Person", "persona");
        AddPair("Year", "año");
        AddPair("Way", "camino/forma");
        AddPair("Day", "día");
        AddPair("Thing", "cosa");
        AddPair("Man", "hombre");
        AddPair("World", "mundo");
        AddPair("Life", "vida");
        AddPair("Hand", "mano");
        AddPair("Part", "parte");
        AddPair("Child", "niño/niña");
        AddPair("Eye", "ojo");
        AddPair("Woman", "mujer");
        AddPair("Place", "lugar");
        AddPair("Work", "trabajo");
        AddPair("Week", "semana");
        AddPair("Case", "caso");
        AddPair("Point", "punto/tema");
        AddPair("Government", "gobierno");
        AddPair("Company", "empresa/compañía");

        // Extras útiles
        AddPair("Beautiful", "hermoso/hermosa");
        AddPair("This", "este/esta");
        AddPair("The", "el/la/los/las");
    }
}
