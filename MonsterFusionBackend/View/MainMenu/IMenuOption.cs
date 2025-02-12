﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterFusionBackend.View
{
    internal interface IMenuOption
    {
        string Name { get; }
        bool OptionAutoRun { get; }
        bool IsRunning { get; set; }
        Task Execute();
        void Kill();
    }
}
