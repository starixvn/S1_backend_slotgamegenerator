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
using (var streamReader = new StreamReader(csv))
using (var csvReader = new CsvReader(streamReader, config))
{
    var slotGameBuilder = new StringBuilder("INSERT INTO Siebog.SlotGame (Id, TpId, CategoryId, TypeId, GameId, AppId, Fun, Device) VALUES");
    var gameCasinoExtraBuilder = new StringBuilder("INSERT INTO Siebog.GameCasinoExtra (TpId, GameCasinoId, Reel, PayLine, RTP) VALUES");
    var gameCasinoInfoBuilder = new StringBuilder("INSERT INTO Siebog.GameCasinoInfo (TpId, GameCasinoId, Name, Image, Lang) VALUES");
    var slotGameStatusBuilder = new StringBuilder("INSERT INTO Siebog.SlotGameStatus (SiteId, CurrencyId, SlotGameId, Status, Operator) VALUES");
    var newGames = csvReader.GetRecords<NewGame>();
    var i = 1;
    foreach (var game in newGames)
    {
        var nextId = id + i;
        slotGameBuilder.AppendLine();
        slotGameBuilder.Append($"({nextId}, {tpId}, 2, 1, '{game.GameId}', '{game.GameId}', {game.Fun}, {game.Device}),");

        gameCasinoExtraBuilder.AppendLine();
        gameCasinoExtraBuilder.Append($"({tpId}, {nextId}, '{game.Reel}', '{game.PayLine}', '{game.RTP}'),");

        gameCasinoInfoBuilder.AppendLine();
        gameCasinoInfoBuilder.Append($"({tpId}, {nextId}, '{game.Ename}', '', 'en'),");
        gameCasinoInfoBuilder.AppendLine();
        gameCasinoInfoBuilder.Append($"({tpId}, {nextId}, '{game.Cname}', '', 'cs'),");

        foreach (var currency in currencies)
        {
            slotGameStatusBuilder.AppendLine();
            slotGameStatusBuilder.Append($"({currency.SiteId}, {currency.CurrencyId}, {nextId}, {status}, 'Admin'),");
        }
        i++;
    }
    WriteFile(slotGameBuilder.ToString().TrimEnd(',') + ";");
    WriteFile(Environment.NewLine);
    WriteFile(gameCasinoExtraBuilder.ToString().TrimEnd(',') + ";");
    WriteFile(Environment.NewLine);
    WriteFile(gameCasinoInfoBuilder.ToString().TrimEnd(',') + ";");
    WriteFile(Environment.NewLine);
    WriteFile(slotGameStatusBuilder.ToString().TrimEnd(',') + ";");
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
