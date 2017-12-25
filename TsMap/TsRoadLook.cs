﻿using System;
using System.Collections.Generic;

namespace TsMap
{
    public class TsRoadLook
    {
        public string LookId { get; private set; }

        public float Offset;
        public float SizeLeft;
        public float SizeRight;
        public float ShoulderLeft;
        public float ShoulderRight;

        public readonly List<string> LanesLeft;
        public readonly List<string> LanesRight;

        public static double Hermite(float s, float x, float z, double tanX, double tanZ)
        {
            double h1 = 2 * Math.Pow(s, 3) - 3 * Math.Pow(s, 2) + 1;
            double h2 = -2 * Math.Pow(s, 3) + 3 * Math.Pow(s, 2);
            double h3 = Math.Pow(s, 3) - 2 * Math.Pow(s, 2) + s;
            double h4 = Math.Pow(s, 3) - Math.Pow(s, 2);
            return h1 * x + h2 * z + h3 * tanX + h4 * tanZ;
        }

        public TsRoadLook(string look)
        {
            LanesLeft = new List<string>();
            LanesRight = new List<string>();
            LookId = look;
        }

        public float GetWidth()
        {
            return Offset + 4.5f + LanesLeft.Count + 4.5f * LanesRight.Count;
        }

    }
}
