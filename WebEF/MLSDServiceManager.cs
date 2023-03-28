using System.Reflection;
using XRoadLib;
using XRoadLib.Headers;
using XRoadLib.Schema;

namespace WebEF
{
    public class MLSDServiceManager : ServiceManager<XRoadHeader40>
    {
        public MLSDServiceManager()
            : base("4.0", new DefaultSchemaExporter("http://mlsd_services.x-road.eu/", typeof(MLSDServiceManager).GetTypeInfo().Assembly))
        {
        }
    }
}