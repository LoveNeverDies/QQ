using Newbe.Mahua.Plugins.Parrot.Helper;
using Newbe.Mahua.Plugins.Parrot.Model;

namespace Newbe.Mahua.Plugins.Parrot
{
    class Program
    {
        static void Main(string[] args)
        {
            QQUSER qq = new QQUSER()
            {
                BREAK = "备注",
                QQUSER_QQID = 1401210070,
                QQUSER_QQQID = 111222333,
                QQUSER_EXPERIENCE = 123
            };
            qq.Get();
            GenerateTableHelper create = new GenerateTableHelper();
            create.StructureSql<QQUSER>()
                .StructureSql<QQXXZM>()
                .SubmitSqlServer();
            //using (var host = new NancyHost(new Uri("http://localhost:65321")))
            //{
            //    host.Start();
            //    Console.WriteLine("Started! press Enter to exit.");
            //    Console.ReadLine();
            //}
        }
    }

}