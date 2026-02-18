using System;
using System.Collections.Generic;
using System.Text;

namespace Base2.Models
{
    /// <summary>
    /// Вузол дерева документа "Добовий наряд"
    /// </summary>
    public class DutySectionNode
    {
        public int DutySectionNodeId { get; set; }

        // FK
        public int? ParentDutySectionNodeId { get; set; }
        public int DutyOrderId { get; set; }

        /// <summary>
        /// Тип вузла (визначає шаблон рендерингу)
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// Порядок серед siblings (для сортування та нумерації)
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// Номер пункту (генерується автоматично: "1.2.3")
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Назва посади/ролі в наряді ("Черговий містечка №1")
        /// </summary>
        public string? DutyPositionTitle { get; set; }

        /// <summary>
        /// Текст локації ("Військове містечко №1 (Седнів)")
        /// </summary>
        public string? LocationText { get; set; }

        /// <summary>
        /// Часовий діапазон (для секцій типу TimeRange)
        /// </summary>
        public int? DutyTimeRangeId { get; set; }

        // Навігація
        public DutySectionNode? Parent { get; set; }
        public List<DutySectionNode> Children { get; set; } = new();
        public List<DutyAssignment> Assignments { get; set; } = new();
        public DutyTimeRange? DutyTimeRange { get; set; }
        public DutyOrder DutyOrder { get; set; } = null!;
    }
}
