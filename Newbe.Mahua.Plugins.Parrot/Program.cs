using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;

namespace Newbe.Mahua.Plugins.Parrot
{
    class Program
    {
        static void Main(string[] args)
        {
            EntityHelper.Get<QQXX>(p => p.QQUSER_GUID.QQUSER_QQID == 123);
            //new GenerateTableHelper()
            //    .StructureSql<QQUSER>()
            //    .StructureSql<QQXXZM>()
            //    .SubmitSqlServer();
            //using (var host = new NancyHost(new Uri("http://localhost:65321")))
            //{
            //    host.Start();
            //    Console.WriteLine("Started! press Enter to exit.");
            //    Console.ReadLine();
            //}
        }
    }

}