using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace osuAT.Game.Skills
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SkillDebugValueAttribute : Attribute
    {
    }
    
}
