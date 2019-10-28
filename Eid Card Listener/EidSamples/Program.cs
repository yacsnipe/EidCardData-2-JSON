/* ****************************************************************************

 * eID Middleware Project.
 * Copyright (C) 2010-2010 FedICT.
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License version
 * 3.0 as published by the Free Software Foundation.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, see
 * http://www.gnu.org/licenses/.

**************************************************************************** */
using System.Collections.Generic;
using System;
using System.Security.Cryptography.X509Certificates;
using EidSamples.tests;
using System.Management;
using System.ComponentModel;
using CommandLine;
using System.Text;
using System.Runtime.InteropServices;

namespace EidSamples
{
    class Program
    {
        public class Options
        {
            [Option('d', "daemon", Required = false, HelpText = "Set Eid Listener as a daemon")]
            public bool Daemon { get; set; }

            [Option('p', "path", Required = false, HelpText = "Set Eid Listener Json output path")]
            public string Path { get; set; }
        }


        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static string path;
        private static void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)

        {
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            foreach (var property in instance.Properties)
            {
                Console.WriteLine(property.Name + " = " + property.Value);
            }
            DataTests dt = new DataTests();
            dt.GetAllData();
            Console.WriteLine("Event done !");
        }

        private static void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            foreach (var property in instance.Properties)
            {
                Console.WriteLine(property.Name + " = " + property.Value);
            }
        }


        static void Main(string[] args)
        {


            var handle = GetConsoleWindow();

            if (args.Length > 0)
                Parser.Default.ParseArguments<Options>(args)
                              .WithParsed<Options>(o =>
                              {
                                  if (o.Daemon)
                                  {
                                      Console.WriteLine($"Verbose output enabled. Current Arguments: -d {o.Daemon}");
                                      Console.WriteLine("App is in Daemon mode!");
                                      ShowWindow(handle, SW_HIDE);

                                  }
                                  else
                                  {
                                      Console.WriteLine($"Current Arguments: -d {o.Daemon}");

                                  }

                                  if (o.Path.Length > 0)
                                  {
                                      Console.WriteLine($"Path output enabled. Current Arguments: -p {o.Path}");
                                      Console.WriteLine("Json output set in " + o.Path);
                                      path = o.Path;
                                  }
                                  else
                                  {
                                      Console.WriteLine($"Current Arguments: -p {o.Path}");
                                      path = ".";
                                  }
                              });
            else
                path = ".";


            if (System.IO.Directory.Exists(path))
            {

                Console.WriteLine("Good Path... the output path is " + path);
                ManagementEventWatcher watcher = new ManagementEventWatcher();
                WqlEventQuery query = new WqlEventQuery();

                query.EventClassName = "__InstanceCreationEvent";
                query.WithinInterval = new TimeSpan(0, 0, 1);
                query.Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'";
                watcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
                watcher.Query = query;
                watcher.Start();
                Console.WriteLine("Listening USB ports ... ");
                while (true)

                {
                    try
                    {
                        Console.WriteLine("Waiting USB event ...");
                        watcher.WaitForNextEvent();
                        Console.WriteLine("New USB event !");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                }

            }
            else
            {
                Console.WriteLine("Type a valid path name");
                Console.ReadLine();
            }

        }

    }
}
