using ggst_api.entity;

namespace ggst_api.ScheduleTask
{
    public interface IUpdateDbSchedule
    {
        public Task<int> updateDb();

        public List<PlayerInfoEntity> getAllDistinctFromDb();
    }
}
