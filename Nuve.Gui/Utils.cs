﻿using System;
using System.Collections.Generic;

namespace Nuve.Gui
{
    internal class Utils
    {
        private static List<int> GenerateDifferentRandomNumbers(int count, int limit)
        {
            if (count > limit)
            {
                return null;
            }
            var random = new Random();
            var randoms = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int n;
                do
                {
                    n = random.Next(0, limit);
                } while (randoms.Contains(n));
                randoms.Add(n);
            }
            randoms.Sort();
            return randoms;
        }
    }
}