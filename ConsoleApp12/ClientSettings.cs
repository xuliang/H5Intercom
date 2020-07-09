﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ConsoleApp12
{
    using System.Net;

    public class ClientSettings
    {
        public static bool IsSsl
        {
            get
            {
                string ssl = ExampleHelper.Configuration["ssl"];
                return !string.IsNullOrEmpty(ssl) && bool.Parse(ssl);
            }
        }

        public static IPAddress Host => IPAddress.Parse(ExampleHelper.Configuration["host"]);

        public static int Port => int.Parse(ExampleHelper.Configuration["port"]);

        public static int Size => int.Parse(ExampleHelper.Configuration["size"]);

        public static string File => ExampleHelper.Configuration["file"];
        public static bool UseLibuv
        {
            get
            {
                string libuv = ExampleHelper.Configuration["libuv"];
                return !string.IsNullOrEmpty(libuv) && bool.Parse(libuv);
            }
        }
    }
}