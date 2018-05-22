using System;
using System.Collections.Generic;
using System.Linq;

namespace DBVariantDemo
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Queue<string> argv = new Queue<string>(args);

			if (!argv.Any())
			{
				Console.Error.WriteLine(@"dbv <command> [...]

commands:
    ls                          List all variables.
    set <name> <type> [value]   Set a variable's value.  If the value is omitted, null is set.
    get <name>                  Get a variable's value.
    rm <name>                   Remove a variable.");
				return;
			}

			string command = argv.Dequeue();
			switch (command)
			{
				case "ls":
					{
						var allVariables = VariantUtility.GetAllVariables().ToList();
						foreach (VariantItem variant in allVariables)
							DisplayVariable(variant);
						Console.WriteLine("{0} variable(s)", allVariables.Count);
					}
					break;

				case "get":
					{
						string name = argv.Dequeue();
						if (name == null)
						{
							Console.Error.WriteLine("Variable name is missing.");
							return;
						}

						VariantItem item = VariantUtility.GetVariable(name);
						DisplayVariable(item);
					}
					break;

				case "set":
					{
						string name = argv.DequeueOrDefault();
						string typeName = argv.DequeueOrDefault();
						string value = argv.DequeueOrDefault();

						if (name == null)
						{
							Console.Error.WriteLine("Variable name is missing.");
							return;
						}
						if (typeName == null)
						{
							Console.Error.WriteLine("Type name is missing.");
							return;
						}

						Type type = VariantUtility.FindTypeByTypeName(typeName);
						if (type == null)
						{
							Console.Error.WriteLine("Unrecognized data type.");
							return;
						}

						VariantUtility.SetVariableValue(name, type, ParameterUtility.For(type).Parse(value));
					}
					break;

				case "rm":
					{
						string name = argv.DequeueOrDefault();
						if (name != null)
							VariantUtility.RemoveVariable(name);
					}
					break;

				default:
					Console.Error.WriteLine("Unrecognized command.");
					break;
			}
		}

		private static void DisplayVariable(VariantItem item)
		{
			Console.Out.WriteLine("{0} [{1}]:", item.Name, VariantUtility.FindTypeNameByType(item.Type));
			Console.Out.WriteLine("\t{0}", ConsoleUtility.For(item.Type).Format(item.Value) ?? "<null>");
			Console.Out.WriteLine();
		}
	}
}
