using Nakama;
using System.Threading.Tasks;
namespace Dawnshard.Network
{
    public interface INetworkGateway
    {
        Task SendPlayCardRequestAsync(PlayCardRequest request);

        Task SendConcedeRequest(ConcedeRequest request);

        Task SendFightCreatureRequestAsync(FightCreatureRequest request);

        Task SendDiscardCardRequestAsync(DiscardCardRequest request);

        Task SendReapCreatureRequestAsync(ReapCreatureRequest request);

        Task SendStopTurnRequestAsync(StopTurnRequest request);

        Task SendSelectActiveWorldRequestAsync(SelectActiveWorldRequest request);

        Task SendMulliganWorldRequestAsync(MulliganRequest request);

        void OnReceivedMatchState(IMatchState m);
    }
}