# Database Variant Demo

This demo is a command-line tool that manages a database table of name/value pairs with an accompanying data type.  It's similar to SQL Server's sql_variant data type but implemented in a way that can work with most relational databases with an arbitrary set of .NET data types, not just those native to the database.  As long as the type implements conversion from/to a string in a round-trippable format or a custom implementation thereof is provided, the type will work.

## Initializing The Database

Find the following line in Data/DBVariantDemoContext.cs and change the connection string if necessary.

```csharp
optionsBuilder.UseSqlServer("Server=(local); Database=DBVariantDemo; Integrated Security=SSPI; MultipleActiveResultSets=True");
```

Then open the NuGet Package Console and run:

```powershell
Update-Database
```

## Using The Demo

### Command-line Syntax

This project builds as a console application for both .NET Framework 4.7 and .NET Core 2.0.

The command-line examples that follow use the Windows executable.  The .NET Core output is run using the "dotnet" tool:

```cmd
dotnet exec dbv.dll <command> [...]
```

If you want to use the the same command with the .NET Core "executable", you can create a shell script (called 'dbv') to simplify the commands so they match those of the .NET Framework executable.

Bash script:

```bash
#!/bin/env bash
dotnet exec dbv.dll "$@"
```

Batch file (dbv.bat or dbv.cmd):

```cmd
@echo off
dotnet exec dbv.dll %*
```

#### ls - List variables

```text
dbv ls
```

#### set - Set variable

```text
dbv set <name> <type> <value>
```

#### get - Get variable

```text
dbv get <name>
```

#### rm - Remove variable

```text
dbv rm <name>
```

### Data Types

| Name     | Description            |
|----------|------------------------|
| string   | System.**String**      |
| uuid     | System.**Guid**        |
| int      | System.**Int32**       |
| decimal  | System.**Decimal**     |
| uri      | System.**Uri**         |
| datetime | NodaTime.**Instant**   |
| date     | NodaTime.**LocalDate** |
| time     | NodaTime.**LocalTime** |

## Osmotic Instances

There are four osmotic instances, two parser and two formatters.

Although the parsers are configured identically in this demo, they are separate so that they can be configured differently.  For example, the "date" type could be accepted in the format of the current locale but stored in the database in ISO-8601 date format.  For the same reasons, the formatters are separate even though they happen to be configured identically for this demo.

`VariantUtility` has a parser/formatter pair that's responsible for conversion from/to the string column in the database.

`ParameterUtility` is responsible for parsing the value from the command line parameter as the specified type, and is public.

`ConsoleUtility` is responsible for formatting the variable for display in the console.  This utility could also have an Osmotic parser if parsing from _console_ input were used in this demo.  While this distinction may not have been necessary for the demo, real-world applications should not conflate such purposes without great consideration.  `ConsoleUtility` and `ParameterUtility` together are responsible the interface between the user and the program.
