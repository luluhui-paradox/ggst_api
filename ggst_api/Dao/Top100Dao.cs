
using ggst_api.entity;
using Microsoft.EntityFrameworkCore;

namespace ggst_api.Dao
{
    public class Top100Dao:ITop100Dao
    {
        public readonly DbContext _dbContext;

        public Top100Dao(DbContext dbContext) { 
            this._dbContext = dbContext;
        }

        public List<PlayerInfoEntity> selectAll()
        {

            return new List<PlayerInfoEntity>();
        }
    }
}
