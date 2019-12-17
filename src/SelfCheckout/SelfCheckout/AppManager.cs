namespace SelfCheckout
{
    public class AppManager
    {
        static AppManager _instance;
        static object syncRoot = new object();

        public static AppManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new AppManager();
                    }
                }
                return _instance;
            }
        }
    }
}
