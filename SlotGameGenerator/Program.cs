// See https://aka.ms/new-console-template for more information
using CsvHelper;
using CsvHelper.Configuration;
using MySql.Data.MySqlClient;
using SlotGameGenerator;
using System.Globalization;
using System.Text;
using System.Text.Json;

Console.Write("請輸入TpId:");
var tpIdStr = Console.ReadLine();
if (!int.TryParse(tpIdStr, out var tpId)) throw new ArgumentException("invalid tpId");

Console.WriteLine("遊戲開關(0:關閉、1:開啟、3:Release階段)");
var stautsStr = Console.ReadLine();
if (!int.TryParse(stautsStr, out var status)) throw new ArgumentException("invalid status");


var connectionString = "server=13.251.227.70;port=3306;user=nova88;database=Siebog;password=Aa12345678!";
var connection = new MySqlConnection(connectionString);

var currencies = await connection.QueryGameCurrency(tpId);
var lastId = await connection.QueryLastGameId(tpId);
var id = lastId ?? tpId * 100000 + 10000;

var config = new CsvConfiguration(CultureInfo.CurrentCulture)
{
    HasHeaderRecord = true,
};

var csv = "slot.csv";
List<NewGame> newGames;
using (var streamReader = new StreamReader(csv))
using (var csvReader = new CsvReader(streamReader, config))
{
    newGames = csvReader.GetRecords<NewGame>().ToList();
}
CreateSlotGame(id, tpId, newGames, WriteFile);
WriteFile(Environment.NewLine);
CreateGameCasinoExtra(id, tpId, newGames, WriteFile);
WriteFile(Environment.NewLine);
CreateGameCasinoInfo(id, tpId, newGames, WriteFile);
WriteFile(Environment.NewLine);
CreateSlotGameStatus(status, currencies, id, newGames.Count, WriteFile);

// SlotGame
static void CreateSlotGame(int lastId, int tpId, List<NewGame> games, Action<string> write)
{
    var sb = new StringBuilder("INSERT INTO Siebog.SlotGame (Id, TpId, CategoryId, TypeId, GameId, AppId, Fun, Device) VALUES\r\n");
    for (int i = 0; i < games.Count; i++)
    {
        var nextId = lastId + i + 1;
        if (i == games.Count -1)
            sb.Append($"({nextId}, {tpId}, 2, 1, '{games[i].GameId}', '{games[i].GameId}', {games[i].Fun}, {games[i].Device});");
        else
            sb.AppendLine($"({nextId}, {tpId}, 2, 1, '{games[i].GameId}', '{games[i].GameId}', {games[i].Fun}, {games[i].Device}),");
    }
    var values = sb.ToString();
    write(values);
}

// GameCasinoExtra
static void CreateGameCasinoExtra(int lastId, int tpId, List<NewGame> games, Action<string> write)
{
    var sb = new StringBuilder("INSERT INTO Siebog.GameCasinoExtra (TpId, GameCasinoId, Reel, PayLine, RTP) VALUES");
    sb.AppendLine();
    for (int i = 0; i < games.Count; i++)
    {
        var nextId = lastId + i + 1;
        if(i == games.Count -1)
            sb.Append($"({tpId}, {nextId}, '{games[i].Reel}', '{games[i].PayLine}', '{games[i].RTP}');");
        else
            sb.AppendLine($"({tpId}, {nextId}, '{games[i].Reel}', '{games[i].PayLine}', '{games[i].RTP}'),");
    }
    var values = sb.ToString();
    write(values);
}

// GameCasinoInfo
static void CreateGameCasinoInfo(int lastId, int tpId, List<NewGame> games, Action<string> write)
{
    var sb = new StringBuilder("INSERT INTO Siebog.GameCasinoInfo (TpId, GameCasinoId, Name, Image, Lang) VALUES");
    sb.AppendLine();
    for (int i = 0; i < games.Count; i++)
    {
        var nextId = lastId + i + 1;
        if (i == games.Count - 1)
        {
            sb.AppendLine($"({tpId}, {nextId}, '{games[i].Ename}', 'null', 'en'),");
            sb.Append($"({tpId}, {nextId}, '{games[i].Cname}', 'null', 'cs');");
        }
        else 
        {
            sb.AppendLine($"({tpId}, {nextId}, '{games[i].Ename}', 'null', 'en'),");
            sb.AppendLine($"({tpId}, {nextId}, '{games[i].Cname}', 'null', 'cs'),");
        }
    }
    var values = sb.ToString();
    write(values);
}

// SlotGameStatus
static void CreateSlotGameStatus(int status, IEnumerable<GameCurrency> currencies, int lastId, int gameCount, Action<string> write) 
{
    var sb = new StringBuilder("INSERT INTO Siebog.SlotGameStatus (SiteId, CurrencyId, SlotGameId, Status, Operator) VALUES");
    sb.AppendLine();
    for (int i = 0; i < gameCount; i++)
    {
        var nextId = lastId + i + 1;
        foreach (var currency in currencies)
        {
            sb.AppendLine($"({currency.SiteId}, {currency.CurrencyId}, {nextId}, {status}, 'Admin'),");
        }
    }
    var values = sb.ToString().TrimEnd('\r', '\n', ',') + ";";
    write(values);
}

static void WriteFile(string script) 
{
    Console.WriteLine(script);
    using StreamWriter writer = new ("slot.sql", true);
    writer.Write(script);
}
public class NewGame
{
    public string Ename { get; set; }
    public string Cname { get; set; }
    public string GameId { get; set; }
    public string Reel { get; set; }
    public string PayLine { get; set; }
    public string RTP { get; set; }
    public int Fun { get; set; }
    public int Device { get; set; }
}
