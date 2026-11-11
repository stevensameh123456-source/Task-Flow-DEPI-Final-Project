using Microsoft.AspNetCore.SignalR;

namespace TaskFlow.Web.Hubs
{
    public class KanbanHub : Hub
    {
        // هذه الدالة تستقبل تحديث حالة المهمة من أي مستخدم وتوزعها على الباقيين
        public async Task UpdateTaskStatus(string taskId, string newStatus)
        {
            // إرسال التحديث لكل المتصلين ما عدا المرسل نفسه (أو للكل لو حابب)
            await Clients.Others.SendAsync("ReceiveTaskMove", taskId, newStatus);
        }
    }
}