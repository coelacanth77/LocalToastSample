using System;
using LocalToastSample.Model;

namespace LocalToastSample.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("LocalToastSample");
            callback(item, null);
        }
    }
}