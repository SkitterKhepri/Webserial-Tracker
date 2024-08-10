using WT_API.Controllers;
using WT_API.Models;
using System.Text.Json;
using WT_API.Data;


string fileName = args[0];
string jsonString = File.ReadAllText(fileName);
Serial serial = JsonSerializer.Deserialize<Serial>(jsonString)!;

Context _context = new Context();

SerialsController ctrl = new SerialsController(_context);

await ctrl.GetSerialDataHAP(serial);

