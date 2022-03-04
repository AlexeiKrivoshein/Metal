using System;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Deployment.WindowsInstaller;
using System.Management;
using System.Linq;

namespace WixAdditionalTools
{
    public class CustomActions
    {
        private static void Execute(string command)
        {
            var processStartInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Произошла ошибка при выполнении команды cmd.exe {command}\r\nExit Code = {process.ExitCode}");
                }
            }
        }

        private static void StopService(string serviceName, string processName)
        {
            var processes = Process.GetProcessesByName(processName);

            using (var searcher = new ManagementObjectSearcher("SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process"))
            {
                using (var results = searcher.Get())
                {
                    processes = (from p in processes
                                 join mo in results.Cast<ManagementObject>()
                                 on p.Id equals (int)(uint)mo["ProcessId"]
                                 select p).ToArray();

                }
            }

            try
            {
                var service = ServiceController.GetServices()
                    .FirstOrDefault(sc => sc.ServiceName == serviceName);

                if (service != null)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            finally
            {
                try
                {
                    Array.ForEach(processes, p => p.Kill());
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }
        }

        [CustomAction]
        public static ActionResult StartService(Session session)
        {
            try
            {
                var serviceName = session?[CustomActionData.PropertyName];

                var service = ServiceController.GetServices()
                    .FirstOrDefault(sc => sc.ServiceName == serviceName);

                if (service != null)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(45));
                }
            }
            catch
            {
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult StopService(Session session)
        {
            for (var i = 0; i < 3; ++i)
            {
                StopService(session?[CustomActionData.PropertyName], null);
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DeleteService(Session session)
        {
            try
            {
                var serviceName = session[CustomActionData.PropertyName];

                var service = ServiceController.GetServices()
                    .FirstOrDefault(sc => sc.ServiceName == serviceName);

                if (service != null)
                {
                    Execute($"sc delete \"{serviceName}\"");
                }
            }
            catch (Exception e)
            {
                Record record = new Record
                {
                    FormatString = $"Во время выполнения установки произошла ошибка \r\n{e}"
                };
                session.Message(InstallMessage.Error, record);

                record.Dispose();

                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult AddOrUpdateService(Session session)
        {
            try
            {
                var serviceName = session.CustomActionData["ServiceName"];
                var serviceDisplayName = session.CustomActionData["ServiceDisplayName"];
                var serviceDescription = session.CustomActionData["ServiceDescription"];
                var servicePath = session.CustomActionData["ServicePath"];
                var serviceUserName = session.CustomActionData["ServiceUserName"];
                var servicePassword = session.CustomActionData["ServicePassword"];

                var service = ServiceController.GetServices()
                    .FirstOrDefault(sc => sc.ServiceName == serviceName);

                if (service == null)
                {
                    Execute($"sc create \"{serviceName}\" start= auto binpath= \"{servicePath}\"");
                }

                try
                {
                    Execute($"sc description \"{serviceName}\" \"{serviceDescription}\"");
                    Execute($"sc config \"{serviceName}\" displayname= \"{serviceDisplayName}\"");
                    Execute($"sc config \"{serviceName}\" start= auto");
                    Execute($"sc config \"{serviceName}\" obj= \"{serviceUserName}\" password= \"{servicePassword}\"");
                    Execute($"sc failure \"{serviceName}\" actions= restart/300000/restart/300000/\"\"/300000 reset= 86400");
                }
                catch (Exception)
                {
                    return ActionResult.Success;
                }
            }
            catch (Exception e)
            {
                Record record = new Record
                {
                    FormatString = $"Во время выполнения установки произошла ошибка \r\n{e}"
                };
                session.Message(InstallMessage.Error, record);

                record.Dispose();

                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult StopMetalServer(Session session)
        {
            for (var i = 0; i < 3; ++i)
            {
                StopService("MetalServer", "MetalServer");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CompareVersion(Session session)
        {
            string installerVersion = session["CURRENT_VERSION"].ToString();
            string installedVersion = session["INSTALLED_VERSION"].ToString();

            if (!installerVersionIsNewer(installerVersion, installedVersion))
            {
                Record record = new Record();
                record.FormatString = string.Format("На компьютере обнаружена более новая версия продукта {0}. "
                                                    +
                                                    "Версия инсталлятора: {1}. Понижение версии запрещено."
                    , installedVersion, installerVersion);

                session.Message(InstallMessage.Error, record);
                return ActionResult.Failure;

            }
            return ActionResult.Success;
        }

        private static bool installerVersionIsNewer(string installerVersion, string installedVersion)
        {
            if (installedVersion.StartsWith("0.")) return true;

            var version1 = new Version(installerVersion);
            var version2 = new Version(installedVersion);

            if (version1.CompareTo(version2) > 0)
                return true;

            return false;
        }
    }
}
