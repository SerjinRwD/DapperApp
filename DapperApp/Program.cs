using System;
using System.Collections.Generic;

namespace DapperApp
{
    class Program
    {
        private static readonly string _connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=stuff;Integrated Security=True";

        private static readonly char[] spaceSplitter = new char[] { ' ' };
        private static readonly Dictionary<string, Action<string[]>> _commandHandlers;
        private static readonly Repositories.ItemsRepository _items;
        private static bool _terminate;

        static Program()
        {
            _terminate = false;

            _items = new Repositories.ItemsRepository(_connectionString);

            _commandHandlers = new Dictionary<string, Action<string[]>>
            {
                { "help", HelpHandler },
                { "add", AddHandler },
                { "select", SelectHandler },
                { "update", UpdateHandler },
                { "delete", DeleteHandler },
                { "quit", QuitHandler },
                { "exit", QuitHandler }
            };
        }

        static void Main(string[] args)
        {
            HelpHandler();

            while (true)
            {
                var arr = Console.ReadLine().Split(spaceSplitter, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    HandleInput(arr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (_terminate)
                {
                    break;
                }
            }
        }

        private static void HelpHandler(string[] args = null)
        {
            Console.WriteLine("Commands:");

            foreach(var item in _commandHandlers)
            {
                Console.WriteLine(item.Key);
            }
        }

        private static void HandleInput(string[] args)
        {
            if (_commandHandlers.ContainsKey(args[0]))
            {
                _commandHandlers[args[0]](args);
            }
            else
            {
                Console.WriteLine("Uknown command");
            }
        }

        private static void AddHandler(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing argument");
                return;
            }

            var id = _items.Add(args[1]);

            Console.WriteLine("New item id: {0}", id);
        }

        private static void QuitHandler(string[] args = null)
        {
            _terminate = true;

            Console.WriteLine("Bye!");
        }

        private static void SelectHandler(string[] args)
        {
            var items = new List<Models.Item>();

            if (args.Length == 1)
            {
                items = _items.Select();
            }
            else
            {
                if (args[1] == "name")
                {
                    items.Add(_items.Select(args[2]));
                }
                else if (args[1] == "id")
                {
                    items.Add(_items.Select(int.Parse(args[2])));
                }
                else
                {
                    Console.WriteLine("Uknown selector");
                    return;
                }
            }

            foreach(var item in items)
            {
                Console.WriteLine("{0}: {1}", item.Id, item.Name);
            }
        }

        private static void DeleteHandler(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing argument");
                return;
            }

            _items.Delete(int.Parse(args[1]));

            Console.WriteLine("Deleted");
        }

        private static void UpdateHandler(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Missing arguments");
                return;
            }

            _items.Update(int.Parse(args[1]), args[2]);

            Console.WriteLine("Updated");
        }
    }
}
