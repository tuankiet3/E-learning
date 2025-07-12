namespace E_learning.Model.cloudeDB
{
    public class RedisModel
    {
        public string key;
        public string value;
        public RedisModel(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
        public RedisModel() { }
    }
}
