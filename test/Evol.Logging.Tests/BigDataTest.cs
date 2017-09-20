using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using log4net.Appender;

namespace Evol.Logging.Tests
{
    /// <summary>
    /// Log4net��Nlog��־�ļ�д��Ա�
    /// </summary>
    public class BigDataTest
    {
        private readonly ITestOutputHelper output;
        public BigDataTest(ITestOutputHelper outputHelper)
        {
            output = outputHelper;
        }

        /// <summary>
        /// ʹ��Log4net��������20W���ַ���
        /// </summary>
        [Fact]
        public void Log4netBigDataTest()
        {
            log4net.Repository.ILoggerRepository repository = log4net.LogManager.CreateRepository("NETCoreRepository");
            var fileInfo = new FileInfo("config/log4net.config");
            log4net.Config.XmlConfigurator.Configure(repository, fileInfo);
            log4net.Config.BasicConfigurator.Configure(repository);
            log4net.ILog log = log4net.LogManager.GetLogger(repository.Name, "NETCorelog4net");

            var total = 200000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < total; i++)
            {
                log.Info("log4 bigdata test: " + i);
            }
            sw.Stop();
            log.Info($"total: {total}, Elapsed:{sw.ElapsedMilliseconds}");
            output.WriteLine($"Log4net���� total: {total}, Elapsed:{sw.ElapsedMilliseconds}");
        }

        /// <summary>
        /// ʹ��Nlog��������20W���ַ���
        /// </summary>
        [Fact]
        public void NlogBigDataTest()
        {

            NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
            var total = 200000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < total; i++)
            {
                log.Info("nlog bigdata test: " + i);
            }
            sw.Stop();
            log.Info($"total: {total}, Elapsed:{sw.ElapsedMilliseconds}");
            output.WriteLine($"NLog���� total: {total}, Elapsed:{sw.ElapsedMilliseconds}");
        }



        /// <summary>
        /// ʹ��Log4net��������20W���ַ���
        /// </summary>
        [Fact]
        public void Log4netCustomPathTest()
        {
            var fileInfo = new FileInfo("config/log4net-custem-path.config");

            log4net.Repository.ILoggerRepository repository = log4net.LogManager.CreateRepository("NETCoreRepository");

            log4net.Config.XmlConfigurator.Configure(repository, fileInfo);
            var allAppeders = repository.GetAppenders();
            var targetApder = allAppeders.FirstOrDefault(e => e.Name == "CustemPathRollingLogFileAppender") as RollingFileAppender;
            var path = Path.GetDirectoryName(targetApder.File);
            targetApder.File = Path.Combine(path, "custem1/");
            targetApder.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(repository);
            log4net.ILog log = log4net.LogManager.GetLogger(repository.Name, "Custem1Log");



            log4net.Repository.ILoggerRepository custemRepos2 = log4net.LogManager.CreateRepository("Custem2Repository");
            log4net.Config.XmlConfigurator.Configure(custemRepos2, fileInfo);

            var cusAppeders2 = custemRepos2.GetAppenders();
            var targetApder2 = cusAppeders2.FirstOrDefault(e => e.Name == "CustemPathRollingLogFileAppender") as RollingFileAppender;
            var path2 = Path.GetDirectoryName(targetApder2.File);
            targetApder2.File = Path.Combine(path2, "custem2/");
            targetApder2.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(custemRepos2);
            log4net.ILog log2 = log4net.LogManager.GetLogger(custemRepos2.Name, "Custem2Log");

            var isExist = (log4net.LogManager.GetRepository(custemRepos2.Name) != null)  && log4net.LogManager.Exists(custemRepos2.Name, "Custem2Log") != null;
            output.WriteLine($"repo={custemRepos2.Name} name=Custem2Log,������{ isExist }");


            var isExistReposNot = false;
            if (log4net.LogManager.GetAllRepositories().Any(e => e.Name == "noRepos"))
                if (log4net.LogManager.Exists("noRepos", "noCustemLog") != null)
                    isExistReposNot = true;
            output.WriteLine($"repo=noRepos,name=noCustemLog,������{ isExistReposNot }");

            var isExistLogNot = false;
            if (log4net.LogManager.GetAllRepositories().Any(e => e.Name == "Custem2Repository"))
                if (log4net.LogManager.Exists("Custem2Repository", "noCustemLog") != null)
                    isExistLogNot = true;
            output.WriteLine($"repo=Custem2Repository,name=noCustemLog,������{ isExistLogNot }");


            for (int i = 0; i < 1000; i++)
            {
                log.Info("log4 custom1: " + targetApder.File);
                output.WriteLine("log4 custom1 path test: " + targetApder.File.Replace(AppDomain.CurrentDomain.BaseDirectory, ""));

                log2.Info("log4 custom2: " + targetApder2.File);
                output.WriteLine("log4 custom2 path test: " + targetApder2.File + targetApder.File.Replace(AppDomain.CurrentDomain.BaseDirectory, ""));
            }


        }

    }
}
