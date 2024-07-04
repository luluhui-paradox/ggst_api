using ggst_api.entity;


namespace ggst_api.Services
{
    public interface ITop100Getter
    {
        public List<PlayerInfoEntity> getTop100();

        public List<PlayerInfoEntity> searchUsersFromRemote(string username);

        public List<PlayerInfoEntity> searchUsersFromDB(string username);

        public List<PlayerInfoEntity> selectUsersByNameAndRate(string username,int rating);

        public List<PlayerInfoEntity> searchUserExactFromRemote(string username, string userid, string? char_short);

        public List<PlayerInfoEntity> searchUserExactFromDB(string username, string userid, string? char_short);
    }
}
