namespace Komi.lib.types;

public enum EPacketType
{
    NetMessageUnknown,
    NetMessageServerHello,
    NetMessageGenericText,
    NetMessageGameMessage,
    NetMessageGamePacket,
    NetMessageError,
    NetMessageTrack,
    NetMessageClientLogRequest,
    NetMessageClientLogResponse,
    NetMessageMax,
}