using STPresenceControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STPresenceControl.Common
{
    public static class PresenceControlEntriesHelper
    {
        public static double GetLeftTimeMinutes(List<PresenceControlEntry> presenceControlEntries)
        {
            if (presenceControlEntries.Count == 0) return 0;

            var timeMins = GetTimeMinutes(presenceControlEntries);
            var dayOfWeek = presenceControlEntries.First().Date.Date.DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    return (new TimeSpan(8, 30, 0)).TotalMinutes - timeMins;
                case DayOfWeek.Friday:
                    return (new TimeSpan(6, 00, 0)).TotalMinutes - timeMins;
                default:
                    return 0;
            }
            
        }

        public static double GetTimeMinutes(List<PresenceControlEntry> presenceControlEntries)
        {
            try
            {
                if (presenceControlEntries.Count == 0) return 0;

                double timeMins = 0;
                for (int i = 0; i < presenceControlEntries.Count; i = i+2)//i++: En Zucchetti a partir de una hora empieza a devolver todos los registros como tipo "Entrada" hasta los que son "Salida"
                {
                    var presenceControlEntry = presenceControlEntries[i];
                    var nextPresenceControlEntry = presenceControlEntries.ElementAtOrDefault(i + 1);

                    //if (presenceControlEntry.Type == PresenceControlEntryTypeEnum.Entry)
                    //{
                        if (nextPresenceControlEntry != null)
                        {
                            var presenceControlExit = nextPresenceControlEntry;
                            //if (presenceControlExit.Type == PresenceControlEntryTypeEnum.Exit)
                            //{
                                timeMins += GetMinutesBetweenDates(presenceControlEntry.Date, presenceControlExit.Date);
                            //}
                            //else
                            //{
                            //    throw new Exception("Hay dos registro de entrada seguidas sin un registro de salida entre ambas.");
                            //}
                        }
                        else//is the last entry
                        {
                            if (presenceControlEntry.Date.Date == DateTime.Today)
                            {
                                timeMins += GetMinutesBetweenDates(presenceControlEntry.Date, DateTime.Now);
                            }
                            else
                            {
                                throw new Exception(string.Format("El día '{0}' no hay registro de fin de jornada laboral.", presenceControlEntry.Date.ToLongDateString()));
                            }
                        }
                    //}
                }
                return timeMins;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static double GetMinutesBetweenDates(DateTime startTime, DateTime endTime)
        {
            return endTime.Subtract(startTime).TotalMinutes;
        }
    }
}
