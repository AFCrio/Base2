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
    /// Клонує дерево вузлів (batch insert), створює DutyTimeRange для кожного TimeRange-вузла.
    /// Фіксує версію шаблону в наказі.
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
            SourceTemplateVersion = template.Version,
            OrderNumber = orderNumber,
            OrderDate = orderDate,
            StartDateTime = start,
            EndDateTime = end,
            CommanderInfo = commanderInfo
        };

        _context.DutyOrders.Add(order);
        _context.SaveChanges(); // Отримуємо DutyOrderId

        // ── Batch-клонування дерева ──
        // Крок 1: створюємо всі вузли з тимчасовим ParentId = null
        var idMapping = new Dictionary<int, DutySectionNode>();
        var pendingParents = new List<(DutySectionNode cloned, int originalParentId)>();
        var timeRangeNodes = new List<DutySectionNode>();

        foreach (var templateNode in allTemplateNodes)
        {
            var cloned = new DutySectionNode
            {
                DutyOrderId = order.DutyOrderId,
                DutyTemplateId = null,
                ParentDutySectionNodeId = null, // Встановимо потім
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
            idMapping[templateNode.DutySectionNodeId] = cloned;

            if (templateNode.ParentDutySectionNodeId.HasValue)
                pendingParents.Add((cloned, templateNode.ParentDutySectionNodeId.Value));

            if (templateNode.NodeType == NodeType.TimeRange && !string.IsNullOrEmpty(templateNode.TimeRangeLabel))
                timeRangeNodes.Add(cloned);
        }

        _context.SaveChanges(); // Batch insert — всі вузли отримують Id

        // Крок 2: встановлюємо ParentId за маппінгом
        foreach (var (cloned, originalParentId) in pendingParents)
        {
            if (idMapping.TryGetValue(originalParentId, out var parentNode))
                cloned.ParentDutySectionNodeId = parentNode.DutySectionNodeId;
        }

        _context.SaveChanges(); // Оновлюємо FK батьків

        // Крок 3: створюємо DutyTimeRange для TimeRange-вузлів
        foreach (var node in timeRangeNodes)
        {
            var timeRange = new DutyTimeRange
            {
                DutyOrderId = order.DutyOrderId,
                Label = node.TimeRangeLabel!,
                Start = order.StartDateTime,
                End = order.EndDateTime
            };

            _context.DutyTimeRanges.Add(timeRange);
            _context.SaveChanges();

            node.DutyTimeRangeId = timeRange.DutyTimeRangeId;
        }

        _context.SaveChanges();

        // Генеруємо Title
        GenerateTitles(order.DutyOrderId);

        return order;
    }

    /// <summary>
    /// Інкрементує версію шаблону та записує зміну в журнал.
    /// Викликати при кожному збереженні змін до шаблону.
    /// </summary>
    public void IncrementTemplateVersion(DutyTemplate template, string changeDescription)
    {
        template.Version++;
        template.UpdatedAt = DateTime.Now;

        var log = new TemplateChangeLog
        {
            DutyTemplateId = template.DutyTemplateId,
            Version = template.Version,
            ChangedAt = DateTime.Now,
            ChangedBy = Environment.UserName,
            ChangeDescription = changeDescription
        };

        _context.TemplateChangeLogs.Add(log);
        _context.SaveChanges();
    }

    /// <summary>
    /// Повертає накази, створені з застарілої версії шаблону.
    /// </summary>
    public List<DutyOrder> GetOutdatedOrders(int templateId)
    {
        var template = _context.DutyTemplates.Find(templateId);
        if (template == null) return [];

        return _context.DutyOrders
            .Where(o => o.SourceTemplateId == templateId
                     && o.SourceTemplateVersion < template.Version)
            .ToList();
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