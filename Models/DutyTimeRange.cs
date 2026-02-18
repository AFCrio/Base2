using System;
using System.Collections.Generic;
using System.Text;

namespace Base2.Models
{
    public class DutyTimeRange
    {
        public int DutyTimeRangeId { get; set; }

        /// <summary>
        /// Початок чергування
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Кінець чергування
        /// </summary>
        public DateTime End { get; set; }

        // Форматовані властивості для шаблонів
        public string StartTime => Start.ToString("HH:mm");
        public string StartDate => Start.ToString("dd.MM.yyyy");
        public string EndTime => End.ToString("HH:mm");
        public string EndDate => End.ToString("dd.MM.yyyy");
    }
}
