﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Location
    {
        /// <summary>
        /// Gets the longitude.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Gets the lattitude.
        /// </summary>
        public double Lattitude { get; set; }


        public override string ToString()
        {
            return "Details of longitude: " + Longitude + "\nLattitude: " + Lattitude + "\n";
        }
    }
}
