using Nebula.Modules.Taller.Models;

namespace Nebula.Modules.Taller;

/// <summary>
/// Modelo de datos para Imprimir un Ticket.
/// </summary>
public class TallerRepairOrderTicket
{
    public TallerRepairOrder RepairOrder { get; set; } = new TallerRepairOrder();
    public List<TallerItemRepairOrder> ItemsRepairOrder { get; set; } = new List<TallerItemRepairOrder>();
}