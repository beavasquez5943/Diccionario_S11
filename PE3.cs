using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TorneoFutbol
{
    // Clase simple que representa un jugador
    public class Playern    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }

        public Player(int id, string name, string position)
        {
            Id = id;
            Name = name;
            Position = position;
        }

        public override string ToString() => $"[{Id}] {Name} - {Position}";
    }

    // Clase que representa un equipo y contiene un diccionario de jugadores
    public class Teamn    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Map: playerId -> Player (Dictionary acts as map)
        public Dictionary<int, Player> Players { get; } = new Dictionary<int, Player>();

        public Team(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => $"[{Id}] {Name} (Jugadores: {Players.Count})";
    }

    // Gestor del torneo: mantiene Mapas y Conjuntos
    public class TournamentManager
    {
        // Map: teamId -> Team
        private readonly Dictionary<int, Team> teams = new Dictionary<int, Team>();
        // Set: conjunto de ids de jugadores registrados en el torneo
        private readonly HashSet<int> registeredPlayerIds = new HashSet<int>();

        private int nextTeamId = 1;
        private int nextPlayerId = 1;

        // Stopwatch para medir tiempos de ejecución
        private readonly Stopwatch sw = new Stopwatch();

        public int AddTeam(string teamName)
        {
            sw.Restart();
            var id = nextTeamId++;
            teams[id] = new Team(id, teamName);
            sw.Stop();
            Console.WriteLine($"[Medición] Tiempo AddTeam: {sw.Elapsed.TotalMilliseconds} ms");
            return id;
        }

        public int AddPlayerToTeam(int teamId, string playerName, string position)
        {
            if (!teams.ContainsKey(teamId)) throw new ArgumentException("Equipo no existe");

            sw.Restart();
            var playerId = nextPlayerId++;
            var player = new Player(playerId, playerName, position);
            teams[teamId].Players[playerId] = player;
            registeredPlayerIds.Add(playerId);
            sw.Stop();
            Console.WriteLine($"[Medición] Tiempo AddPlayerToTeam: {sw.Elapsed.TotalMilliseconds} ms");
            return playerId;
        }

        public bool RemovePlayerFromTeam(int teamId, int playerId)
        {
            if (!teams.ContainsKey(teamId)) return false;
            sw.Restart();
            var removed = teams[teamId].Players.Remove(playerId);
            if (removed) registeredPlayerIds.Remove(playerId);
            sw.Stop();
            Console.WriteLine($"[Medición] Tiempo RemovePlayerFromTeam: {sw.Elapsed.TotalMilliseconds} ms");
            return removed;
        }

        public Team GetTeam(int teamId)
        {
            teams.TryGetValue(teamId, out var t);
            return t;
        }

        public IEnumerable<Team> ListTeams()
        {
            sw.Restart();
            var list = teams.Values.ToList();
            sw.Stop();
            Console.WriteLine($"[Medición] Tiempo ListTeams: {sw.Elapsed.TotalMilliseconds} ms");
            return list;
        }

        public IEnumerable<Player> ListAllPlayers()
        {
            sw.Restart();
            var all = teams.Values.SelectMany(t => t.Players.Values).ToList();
            sw.Stop();
            Console.WriteLine($"[Medición] Tiempo ListAllPlayers: {sw.Elapsed.TotalMilliseconds} ms");
            return all;
        }

        public Player SearchPlayerByName(string name)
        {
            sw.Restart();
            // busqueda lineal sobre players (podría optimizarse con un mapa name->id)
            var player = teams.Values.SelectMany(t => t.Players.Values)
                .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            sw.Stop();
            Console.WriteLine($"[Medición] Tiempo SearchPlayerByName: {sw.Elapsed.TotalMilliseconds} ms");
            return player;
        }

        // Reportería: devuelve un reporte simple de la estructura
        public string GenerateReport()
        {
            var lines = new List<string>();
            lines.Add("--- REPORTE DEL TORNEO ---");
            lines.Add($"Equipos registrados: {teams.Count}");
            foreach (var t in teams.Values)
            {
                lines.Add(t.ToString());
                foreach (var p in t.Players.Values)
                {
                    lines.Add("  - " + p.ToString());
                }
            }
            lines.Add($"Total jugadores únicos (conjunto): {registeredPlayerIds.Count}");
            return string.Join(Environment.NewLine, lines);
        }

        // Operaciones con conjuntos: unión de jugadores entre 2 equipos
        public IEnumerable<Player> UnionPlayersBetweenTeams(int teamAId, int teamBId)
        {
            if (!teams.ContainsKey(teamAId) || !teams.ContainsKey(teamBId)) return Enumerable.Empty<Player>();
            var set = new HashSet<int>(teams[teamAId].Players.Keys);
            set.UnionWith(teams[teamBId].Players.Keys);
            return set.Select(id => teams.Values.SelectMany(t => t.Players.Values).First(p => p.Id == id));
        }
    }

    class Program
    {
        static void Main()
        {
            var manager = new TournamentManager();

            Console.WriteLine("Torneo de Fútbol - Demo (Consola)");
            // Datos de ejemplo
            var t1 = manager.AddTeam("Imparables");
            var t2 = manager.AddTeam("Solo Panas");

            manager.AddPlayerToTeam(t1, "Juan Perez", "Delantero");
            manager.AddPlayerToTeam(t1, "Carlos Ruiz", "Mediocampista");
            manager.AddPlayerToTeam(t2, "Miguel Soto", "Defensa");
            manager.AddPlayerToTeam(t2, "Juan Perez", "Delantero"); // mismo nombre, distinto id

            Console.WriteLine();
            Console.WriteLine(manager.GenerateReport());

            Console.WriteLine();
            Console.WriteLine("Buscar jugador: 'Juan Perez'");
            var found = manager.SearchPlayerByName("Juan Perez");
            Console.WriteLine(found != null ? found.ToString() : "No encontrado");

            Console.WriteLine();
            Console.WriteLine("Ejemplo: Unión de jugadores entre equipos 1 y 2");
            var union = manager.UnionPlayersBetweenTeams(t1, t2);
            foreach (var p in union)
                Console.WriteLine(p);

            Console.WriteLine();
            Console.WriteLine("Fin de la demo. Presione Enter para salir.");
            Console.ReadLine();
        }
    }
}