using CsvHelper;
using System.Diagnostics;
using System.Globalization;

namespace PlainFiles.Core;

public class NugetCsvHelper
{
    public void Write(string path, IEnumerable<Person> people)
    {
        using var sw = new StreamWriter(path);
        using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
        cw.WriteRecords(people);
    }

    public IEnumerable<Person> Read(string path)
    {
        if (!File.Exists(path))
        {
            using var sw = new StreamWriter(path);
            using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
            cw.WriteHeader<Person>();
            sw.Flush();
            return new List<Person>();
        }

        using var sr = new StreamReader(path);
        using var cr = new CsvReader(sr, CultureInfo.InvariantCulture);
        return cr.GetRecords<Person>().ToList();
    }
    public void MostrarPersonas(List<Person> people)
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

    public Person CrearPersona(List<Person> people)
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

    public void EditarPersona(List<Person> people)
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

    public void BorrarPersona(List<Person> people)
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

    public void InformePorCiudad(List<Person> people)
    {
        var agrupado = people.GroupBy(p => p.City);
        decimal totalGeneral = 0;

        foreach (var grupo in agrupado)
        {
            Console.WriteLine($"\nCiudad: {grupo.Key}");
            Console.WriteLine("ID\tNombres\tApellidos\tSaldo");
            Console.WriteLine("---\t-------\t---------\t-----");
            decimal subtotal = 0;

            foreach (var p in grupo)
            {
                Console.WriteLine($"{p.Id}\t{p.FirstName,5}\t{p.LastName,20}\t{p.Balance:C}");
                subtotal += p.Balance;
            }
            Console.WriteLine(@$"                            ==========
Total {grupo.Key}    {subtotal,24:C}
");

            //Console.WriteLine($"\t\t\t=======\nTotal: {grupo.Key}\t\t{subtotal,18:C}");
            totalGeneral += subtotal;
        }
        Console.WriteLine(@$"                                ==========
Total General:{totalGeneral,29:C}
");
        //Console.WriteLine($"\n=======\nTotal General:\t\t{totalGeneral,19:C}");
    }
}