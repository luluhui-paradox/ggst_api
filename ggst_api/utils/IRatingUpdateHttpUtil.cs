namespace ggst_api.utils
{
    public interface IRatingUpdateHttpUtil
    {
        public  Task<HttpResponseMessage> sendHttpAsync(string apiString);
    }
}
