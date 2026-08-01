namespace MobileShop.Models;

public static class OrderStatusStyle
{
    public static string BadgeClass(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "ms-badge-pending",
        OrderStatus.Confirmed => "ms-badge-confirmed",
        OrderStatus.Processing => "ms-badge-processing",
        OrderStatus.Dispatched => "ms-badge-dispatched",
        OrderStatus.Delivered => "ms-badge-delivered",
        OrderStatus.Cancelled => "ms-badge-cancelled",
        _ => ""
    };
}
