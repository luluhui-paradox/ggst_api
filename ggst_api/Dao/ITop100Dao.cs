

using ggst_api.entity;

namespace ggst_api.Dao
{
    public interface ITop100Dao
    {
        public List<PlayerInfoEntity> selectAll();
    }
}
