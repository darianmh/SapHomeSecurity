using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SapSecurity.Model;
using SapSecurity.Data;

namespace SapSecurity.Infrastructure.Repositories;

public class SensorLogRepository : BaseRepository<SensorLog>, ISensorLogRepository
{

    #region Fields



    #endregion
    #region Methods

    public override async Task InsertAsync(SensorLog entity)
    {
        var connection = new SqlConnection("Data Source=109.122.199.199;Initial Catalog=SapSecurity_Db;User Id=Sa;password=rasoul3744;Trusted_Connection=false;MultipleActiveResultSets=true;Encrypt=False;");
        var sql = @"INSERT INTO [dbo].[SensorLogs] ([DateTimeUtc],[Status],[SensorDetailId])
VALUES (@DateTimeUtc, @Status, @SensorDetailId)";
        var rowsAffected = await connection.ExecuteAsync(sql, entity);
        return;
    }

    public async Task<List<SensorLog>> GetLastLogsBySensor(int sensorId, DateTime dateTime)
        => await DbSet.Where(x => x.SensorDetailId == sensorId && x.DateTimeUtc >= dateTime).ToListAsync();

    public async Task<SensorLog?> GetLastLogAsync(int sensorId)
    {
        return await DbSet.Where(x => x.SensorDetailId == sensorId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor

    public SensorLogRepository(ApplicationContext context) : base(context)
    {
    }


    #endregion

}