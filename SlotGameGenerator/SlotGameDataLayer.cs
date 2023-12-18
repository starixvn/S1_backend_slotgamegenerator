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

        public static async Task<IEnumerable<int>> QueryTpCurrency(this DbConnection connection, int tpId) =>
            await connection.QueryAsync<int>($"SELECT CurrencyId FROM Siebog.TpCurrency WHERE TpId={tpId};");
    }
}
