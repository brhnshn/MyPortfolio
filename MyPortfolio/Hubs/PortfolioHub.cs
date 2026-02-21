using Microsoft.AspNetCore.SignalR;

namespace MyPortfolio.Hubs
{
    public class PortfolioHub : Hub
    {
        // Ihtiyac halinde istemcilerden (client) server'a metod çagrıları buraya eklenebilir.
        // Şimdilik sadece sunucudan (server) istemciye "UpdateComponent" tetiklemesi göndereceğiz.
    }
}
