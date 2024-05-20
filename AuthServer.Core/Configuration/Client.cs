using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
  public class Client
  {
    public string Id { get; set; }
    public string Secret { get; set; }
    public List<string> Audiences { get; set; } //apis hangisine erişim sağlayacağını bilgisi
  }
  public class AppSettings
  {
    public List<Client> Clients { get; set; }
  }
  public class Secret
  {
    public static void Main(string[] args)
    {
      // Sabit secret değerleri
      var secrets = new Dictionary<string, string>
            {
                { "SpaApp", "dGhpc2lzYXNzYWNyZXRmb3JTcGFBcHA=" }, 
                { "MobilApp", "dGhpc2lzYXNzYWNyZXRmb3JNb2JpbEFwcA==" }
            };
      // appsettings.json dosyasını oku
      string jsonString = File.ReadAllText("appsettings.json");
      AppSettings appSettings = JsonSerializer.Deserialize<AppSettings>(jsonString);

      // Secret değerlerini güncelle
      foreach (var client in appSettings.Clients)
      {
        if (secrets.ContainsKey(client.Id))
        {
          client.Secret = secrets[client.Id];
        }
      }
      // Güncellenmiş appsettings.json dosyasını yaz
      jsonString = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
      File.WriteAllText("appsettings.json", jsonString);

    }
  }
}


