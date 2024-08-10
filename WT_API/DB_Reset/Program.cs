using WT_API.Data;
using System.Text.Json;
using WT_API.Models;
using Microsoft.EntityFrameworkCore;

Context _context = new Context();
string fileName = args[0];

try
{
  if (_context.Serials.ToList().Count() == 0)
  {
    //Console.WriteLine("elso");
    string jsonString = File.ReadAllText(fileName);
    Serial serial = JsonSerializer.Deserialize<Serial>(jsonString)!;
    _context.Serials.Add(new WT_API.Models.Serial(serial));
    _context.SaveChanges();
    
  }
  else
  {
    //Console.WriteLine("masodik");
    _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Serials', RESEED, 0)");
    //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE Serial");
    var rows = _context.Serials.ToList();
    foreach (var row in rows)
    {
      _context.Serials.Remove(row);
    }
    string jsonString = File.ReadAllText(fileName);
    Serial serial = JsonSerializer.Deserialize<Serial>(jsonString)!;
    _context.Serials.Add(new WT_API.Models.Serial(serial));
    _context.SaveChanges();
  }
}
catch(Exception ex)
{
  Console.WriteLine(ex);
}
