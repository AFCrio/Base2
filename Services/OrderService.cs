using Base2.Data;
using Base2.Models;
using Microsoft.EntityFrameworkCore;

namespace Base2.Services;

/// <summary>
/// Сервіс створення наказів з шаблонів
/// </summary>
public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Створити новий наказ на основі шаблону.
    /// Клонує дерево вузлів, створює DutyTimeRange для кожного TimeRange-вузла.
    /// </summary>
    public DutyOrder CreateOrderFromTemplate(int templateId, string orderNumber, DateOnly orderDate,
        DateTime start, DateTime end, string commanderInfo)
    {
        var template = _context.DutyTemplates
            .Include(t => t.Sections.Where(s => s.DutyTemplateId == templateId))
            .FirstOrDefault(t => t.DutyTemplateId == templateId)
            ?? throw new InvalidOperationException("Шаблон не знайдено");

        // Завантажуємо ВСЕ дерево шаблону (включаючи вкладені рівні)
        var allTemplateNodes = _context.DutySectionNodes
            .Where(n => n.DutyTemplateId == templateId)
            .OrderBy(n => n.OrderIndex)
            .AsNoTracking()
            .ToList();

        var order = new DutyOrder
        {
            SourceTemplateId = templateId,
            OrderNumber = orderNumber,
            OrderDate = orderDate,
            StartDateTime = start,
            EndDateTime = end,
            CommanderInfo = commanderInfo
        };

        _context.DutyOrders.Add(order);
        _context.SaveChanges(); // Отримуємо DutyOrderId

        // Клонуємо дерево: oldId → newNode
        var idMapping = new Dictionary<int, DutySectionNode>();

        // Спочатку кореневі вузли, потім дочірні (BFS по рівнях)
        var rootNodes = allTemplateNodes.Where(n => n.ParentDutySectionNodeId == null).ToList();
        var queue = new Queue<(DutySectionNode templateNode, int? newParentId)>();

        foreach (var root in rootNodes)
            queue.Enqueue((root, null));

        while (queue.Count > 0)
        {
            var (templateNode, newParentId) = queue.Dequeue();

            var cloned = new DutySectionNode
            {
                DutyOrderId = order.DutyOrderId,
                DutyTemplateId = null,
                ParentDutySectionNodeId = newParentId,
                NodeType = templateNode.NodeType,
                OrderIndex = templateNode.OrderIndex,
                DutyPositionTitle = templateNode.DutyPositionTitle,
                HasWeapon = templateNode.HasWeapon,
                HasAmmo = templateNode.HasAmmo,
                HasVehicle = templateNode.HasVehicle,
                MaxAssignments = templateNode.MaxAssignments,
                TimeRangeLabel = templateNode.TimeRangeLabel
            };

            _context.DutySectionNodes.Add(cloned);
            _context.SaveChanges(); // Отримуємо Id

            idMapping[templateNode.DutySectionNodeId] = cloned;

            // Створюємо DutyTimeRange для TimeRange-вузлів
            if (templateNode.NodeType == NodeType.TimeRange && !string.IsNullOrEmpty(templateNode.TimeRangeLabel))
            {
                var timeRange = new DutyTimeRange
                {
                    DutyOrderId = order.DutyOrderId,
                    Label = templateNode.TimeRangeLabel,
                    Start = order.StartDateTime,
                    End = order.EndDateTime
                };

                _context.DutyTimeRanges.Add(timeRange);
                _context.SaveChanges();

                cloned.DutyTimeRangeId = timeRange.DutyTimeRangeId;
                _context.SaveChanges();
            }

            // Додаємо дочірні вузли в чергу
            var children = allTemplateNodes
                .Where(n => n.ParentDutySectionNodeId == templateNode.DutySectionNodeId)
                .OrderBy(n => n.OrderIndex);

            foreach (var child in children)
                queue.Enqueue((child, cloned.DutySectionNodeId));
        }

        // Генеруємо Title
        GenerateTitles(order.DutyOrderId);

        return order;
    }

    /// <summary>
    /// Автоматична нумерація вузлів. Починається з SectionHeader.
    /// </summary>
    public void GenerateTitles(int orderId)
    {
        var allNodes = _context.DutySectionNodes
            .Where(n => n.DutyOrderId == orderId)
            .ToList();

        var roots = allNodes
            .Where(n => n.ParentDutySectionNodeId == null)
            .OrderBy(n => n.OrderIndex)
            .ToList();

        int sectionCounter = 0;

        foreach (var root in roots)
        {
            if (root.NodeType == NodeType.SectionHeader)
            {
                sectionCounter++;
                AssignTitle(root, $"{sectionCounter}", allNodes);
            }
            else
            {
                root.Title = null; // Не нумерується
            }
        }

        _context.SaveChanges();
    }

    private void AssignTitle(DutySectionNode node, string prefix, List<DutySectionNode> allNodes)
    {
        node.Title = prefix;

        var children = allNodes
            .Where(n => n.ParentDutySectionNodeId == node.DutySectionNodeId)
            .OrderBy(n => n.OrderIndex)
            .ToList();

        for (int i = 0; i < children.Count; i++)
        {
            AssignTitle(children[i], $"{prefix}.{i + 1}", allNodes);
        }
    }
}