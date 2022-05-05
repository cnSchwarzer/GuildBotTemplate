using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GuildBotTemplate.Database; 
 
public class JsonSingleton {
    [Key] public string Name { get; set; }
    public JsonDocument Value { get; set; }
}