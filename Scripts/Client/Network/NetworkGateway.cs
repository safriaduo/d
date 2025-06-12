using Google.Protobuf;
using Nakama;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Dawnshard.Network
{
    /// <summary>
    /// This component is the responsible for sending and receiving requests from the backend
    /// and to update the components that needs these data
    /// </summary>
    public class NetworkGateway : INetworkGateway
    {
        private readonly ISocket socket;
        private readonly IMatch match;
        private readonly IEventDispatcher eventDispatcher;

        public NetworkGateway(ISocket socket, IMatch match, IEventDispatcher eventDispatcher)
        {
            this.socket = socket;
            this.match = match;
            this.eventDispatcher = eventDispatcher;

        }

        public void OnReceivedMatchState(IMatchState matchState)
        {
            string json = Encoding.UTF8.GetString(matchState.State);

            Debug.Log($"Received match state! Code {(OpCode)matchState.OpCode}, State:\n {json}");

            switch ((OpCode)matchState.OpCode)
            {
                case OpCode.Start:
                    {
                        eventDispatcher.OnGameStart(GameStart.Parser.ParseJson(json));
                        break;
                    }
                case OpCode.Matchupdate:
                    {
                        eventDispatcher.OnGameStateUpdate(GameStateUpdate.Parser.ParseJson(json));
                        break;
                    }
                case OpCode.End:
                    {
                        eventDispatcher.OnGameEnd(GameEnd.Parser.ParseJson(json));
                        break;
                    }
                case OpCode.Opponentleft:
                    {
                        eventDispatcher.OnOpponentLeft(GameEnd.Parser.ParseJson(json));
                        break;
                    }
                case OpCode.Gameupdate:
                    {
                        eventDispatcher.OnGameUpdate(GameUpdate.Parser.ParseJson(json));
                        break;
                    }

                default:
                    Debug.LogError("Unsupported op code " + matchState.OpCode);
                    break;
            }
        }

        #region Requests

        public async Task SendConcedeRequest(ConcedeRequest request)
        {
            await socket.SendMatchStateAsync(match.Id, (long)OpCode.Concede, request.Encode());
        }

        public async Task SendPlayCardRequestAsync(PlayCardRequest request)
        {

            await socket.SendMatchStateAsync(match.Id, (long)OpCode.Play, request.Encode());
        }

        public async Task SendFightCreatureRequestAsync(FightCreatureRequest request)
        {
            await socket.SendMatchStateAsync(match.Id, (long)OpCode.Fight, request.Encode());
        }

        public async Task SendDiscardCardRequestAsync(DiscardCardRequest request)
        {
            await socket.SendMatchStateAsync(match.Id, (long)OpCode.Discard, request.Encode());
        }

        public async Task SendReapCreatureRequestAsync(ReapCreatureRequest request)
        {
            await socket.SendMatchStateAsync(match.Id, (long)OpCode.Reap, request.Encode());
        }

        public async Task SendStopTurnRequestAsync(StopTurnRequest request)
        {
            await socket.SendMatchStateAsync(match.Id, (long)OpCode.Stopturn, request.Encode());
        }

        public async Task SendSelectActiveWorldRequestAsync(SelectActiveWorldRequest request)
        {
            await socket.SendMatchStateAsync(match.Id, (long)OpCode.Selectactiveworld, request.Encode());
        }

        public async Task SendMulliganWorldRequestAsync(MulliganRequest request)
        {
            await socket.SendMatchStateAsync(match.Id, (long)OpCode.Mulligancompleted, request.Encode());
        }

        #endregion
    }
}