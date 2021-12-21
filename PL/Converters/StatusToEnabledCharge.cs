﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using IBL.BO;

namespace PL.Converters
{
    class StatusToEnabledCharge : IValueConverter
    {
        /// <summary>
        /// Defines whether the button can be pressed according to the status
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((DroneStatuses)value)
            {
                case DroneStatuses.Available:
                    return true;
                case DroneStatuses.Maintenance:
                    return true;
                case DroneStatuses.Sendering:
                    return false;
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
