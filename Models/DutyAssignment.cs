using System;
using System.Collections.Generic;
using System.Text;

namespace Base2.Models
{

    /// <summary>
    /// Призначення особи на позицію в добовому наряді
    /// </summary>
    public class DutyAssignment
    {
        public int DutyAssignmentId { get; set; }

        // FK
        public int DutySectionNodeId { get; set; }
        public int PersonId { get; set; }
        public int? WeaponId { get; set; }
        public int? VehicleId { get; set; }

        /// <summary>
        /// Кількість набоїв (120, 16)
        /// </summary>
        public int? AmmoCount { get; set; }

        /// <summary>
        /// Тип набоїв ("5,45 мм", "9 мм")
        /// </summary>
        public string? AmmoType { get; set; }

        // Навігація
        public DutySectionNode DutySectionNode { get; set; } = null!;
        public Person Person { get; set; } = null!;
        public Weapon? Weapon { get; set; }
        public Vehicle? Vehicle { get; set; }
    }

}
