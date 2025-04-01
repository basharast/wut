/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUniversalTool.Models
{
    public class RenderEffect
    {
        public string Name;
        public int Order = 0;
        public List<byte[]> Values1;
        public double Value1;
        public double Value2;
        public double Value3;
        public double Value4;
        public CanvasBitmap tempResult;
        public RenderEffect(string name, double value1 = 0, double value2 = 0, double value3 = 0, double value4 = 0)
        {
            Name = name;
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
        }
        public RenderEffect(string name, List<byte[]> values1)
        {
            Name = name;
            Values1 = values1;
        }
    }
}
