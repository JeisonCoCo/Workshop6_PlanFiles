using PlainFiles.Core;

Console.Write("Escribe el nombre del archivo (sin extensión): ");
string fileName = Console.ReadLine()!;
string path = Path.Combine("data", $"{fileName}.csv");

var directory = Path.GetDirectoryName(path);
if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
    Directory.CreateDirectory(directory);

var helper = new NugetCsvHelper();
var people = helper.Read(path).ToList();


string opcion;
do
{
    opcion = MostrarMenu();
    switch (opcion)
    {
        case "1": // Show
            MostrarPersonas(people);
            break;

        case "2": // Add
            try
            {
                var nueva = CrearPersona(people);
                people.Add(nueva);
                Console.WriteLine("Persona agregada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            break;

        case "3": // Edit
            EditarPersona(people);
            break;

        case "4": // Delete
            BorrarPersona(people);
            break;

        case "5": // Save
            helper.Write(path, people);
            Console.WriteLine("Cambios guardados.");
            break;

        case "6": // Infor
            InformePorCiudad(people);
            break;

        case "0": // Exit
            Console.WriteLine("Saliendo...");
            break;

        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}
while (opcion != "0");


static string MostrarMenu()
{
    Console.Write(@"
========================================
  
  Seleccione una opción:

    1. Mostrar personas
    2. Agregar persona
    3. Editar persona
    4. Eliminar persona
    5. Guardar cambios
    6. Informe por ciudad
    0. Salir

========================================

Tu opción es: ");
    
  return Console.ReadLine() ?? "";
}


//Console.WriteLine("========================================");
//static string MostrarMenu()
//{
//    Console.WriteLine("\n==============================");
//    Console.WriteLine("1. Mostrar personas");
//    Console.WriteLine("2. Agregar persona");
//    Console.WriteLine("3. Editar persona");
//    Console.WriteLine("4. Eliminar persona");
//    Console.WriteLine("5. Guardar cambios");
//    Console.WriteLine("6. Informe por ciudad");
//    Console.WriteLine("0. Salir");
//    Console.Write("Elige una opción: ");
//    return Console.ReadLine() ?? "";
//}

static void MostrarPersonas(List<Person> people)
{
    if (people.Count == 0)
    {
        Console.WriteLine("No hay registros.");
        return;
    }

    foreach (var p in people)
    {
        Console.WriteLine($"{p.Id}\n{p.FirstName} {p.LastName}\nTeléfono: {p.Phone}\nCiudad: {p.City}\nSaldo: {p.Balance:C}\n");
    }
}

static Person CrearPersona(List<Person> people)
{
    Console.Write("ID: ");
    if (!int.TryParse(Console.ReadLine(), out var id) || people.Any(p => p.Id == id))
        throw new Exception("ID inválido o duplicado.");

    Console.Write("Nombres: ");
    var nombres = Console.ReadLine();
    Console.Write("Apellidos: ");
    var apellidos = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(apellidos))
        throw new Exception("Nombre y apellido requeridos.");

    Console.Write("Teléfono (formato 322 311 4015): ");
    var telefono = Console.ReadLine();
    if (!System.Text.RegularExpressions.Regex.IsMatch(telefono ?? "", @"^\d{3} \d{3} \d{4}$"))
        throw new Exception("Teléfono inválido.");

    Console.Write("Ciudad: ");
    var ciudad = Console.ReadLine();

    Console.Write("Saldo: ");
    if (!decimal.TryParse(Console.ReadLine(), out var saldo) || saldo < 0)
        throw new Exception("Saldo inválido.");

    return new Person
    {
        Id = id,
        FirstName = nombres!,
        LastName = apellidos!,
        Phone = telefono!,
        City = ciudad!,
        Balance = saldo
    };
}

static void EditarPersona(List<Person> people)
{
    Console.Write("ID a editar: ");
    int.TryParse(Console.ReadLine(), out var id);
    var persona = people.FirstOrDefault(p => p.Id == id);
    if (persona == null)
    {
        Console.WriteLine("No existe.");
        return;
    }

    Console.Write($"Nombre ({persona.FirstName}): ");
    var nombre = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(nombre)) persona.FirstName = nombre;

    Console.Write($"Apellido ({persona.LastName}): ");
    var apellido = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(apellido)) persona.LastName = apellido;

    Console.Write($"Teléfono ({persona.Phone}): ");
    var tel = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(tel)) persona.Phone = tel;

    Console.Write($"Ciudad ({persona.City}): ");
    var ciudad = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(ciudad)) persona.City = ciudad;

    Console.Write($"Saldo ({persona.Balance}): ");
    var saldoStr = Console.ReadLine();
    if (decimal.TryParse(saldoStr, out var saldo) && saldo >= 0)
        persona.Balance = saldo;

    Console.WriteLine("Persona actualizada.");
}

static void BorrarPersona(List<Person> people)
{
    Console.Write("ID a borrar: ");
    int.TryParse(Console.ReadLine(), out var id);
    var persona = people.FirstOrDefault(p => p.Id == id);
    if (persona == null)
    {
        Console.WriteLine("No existe.");
        return;
    }

    Console.WriteLine($"Nombre: {persona.FirstName} {persona.LastName}, Ciudad: {persona.City}, Saldo: {persona.Balance:C}");
    Console.Write("¿Confirmar borrado? (s/n): ");
    var confirm = Console.ReadLine();
    if (confirm?.ToLower() == "s")
    {
        people.Remove(persona);
        Console.WriteLine("Persona eliminada.");
    }
}

static void InformePorCiudad(List<Person> people)
{
    var agrupado = people.GroupBy(p => p.City);
    decimal totalGeneral = 0;

    foreach (var grupo in agrupado)
    {
        Console.WriteLine($"\nCiudad: {grupo.Key}\n");
        Console.WriteLine("ID\tNombres\tApellidos\tSaldo");
        Console.WriteLine("---\t-------\t---------\t----------");
        decimal subtotal = 0;

        foreach (var p in grupo)
        {
            Console.WriteLine($"{p.Id}\t{p.FirstName,5}\t{p.LastName,5}\t\t{p.Balance:C}");
            subtotal += p.Balance;
        }

 Console.WriteLine(@$"                                ==========
Total: {grupo.Key}      {subtotal,22:C}
");

        //Console.WriteLine($"\t\t\t=======\nTotal: {grupo.Key}\t\t{subtotal,18:C}");
        totalGeneral += subtotal;
    }
Console.WriteLine(@$"                                ==========
Total General:{totalGeneral,29:C}
");
    //Console.WriteLine($"\n=======\nTotal General:\t\t{totalGeneral,19:C}");
}