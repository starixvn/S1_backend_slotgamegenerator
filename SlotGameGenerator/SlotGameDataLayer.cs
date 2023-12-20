using Dapper;
using System.Data.Common;
namespace SlotGameGenerator
{
    public static class SlotGameDataLayer
    {
        public static async Task<int?> QueryLastGameId(this DbConnection connection, int tpId) =>
            await connection.QueryFirstOrDefaultAsync<int?>($"SELECT MAX(SlotGame.Id) AS NextId " +
                                                           $"FROM Siebog.SlotGame " +
                                                           $"WHERE SlotGame.TpId = {tpId} AND SlotGame.TypeId = 1;");

        public static async Task<IEnumerable<GameCurrency>> QueryGameCurrency(this DbConnection connection, int tpId) =>
            await connection.QueryAsync<GameCurrency>($"SELECT TGC.SiteId, TGC.CurrencyId FROM Siebog.TpGameTypeCurrency AS TGC " +
                                                  $"INNER JOIN Siebog.TpGameType AS TGT ON TGC.TpGameType = TGT.Type " +
                                                  $"WHERE TGT.TpId={tpId} AND TGT.CategoryId=2;");
    }

    public class GameCurrency 
    {
        public int SiteId { get; set; }
        public int CurrencyId { get; set; }
    }
}
