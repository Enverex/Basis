using Basis.Scripts.BasisSdk;
using Basis.Scripts.BasisSdk.Players;
using Basis.Scripts.Networking;
using Basis.Scripts.Networking.NetworkedAvatar;
using LiteNetLib;
using UnityEngine;

public class BasisTestNetworkScene : MonoBehaviour
{
    public byte[] SendingData;
    public ushort[] Recipients;
    public ushort MessageIndex;
    public void Awake()
    {
        BasisNetworkManagement.OnLocalPlayerJoined += OnLocalPlayerJoined;
        BasisNetworkManagement.OnRemotePlayerJoined += OnRemotePlayerJoined;
    }
    public void OnEnable()
    {
        BasisNetworkSpawnItem.RequestSceneLoad("Scene", "https://BasisFramework.b-cdn.net/Worlds/DX11/3dd6aa45-a685-4ed2-ba6d-2d9c2f3c1765_638652274774362697.BasisEncyptedBundle", "https://BasisFramework.b-cdn.net/Worlds/DX11/3dd6aa45-a685-4ed2-ba6d-2d9c2f3c1765_638652274774362697.BasisEncyptedMeta",false);
    }
    /// <summary>
    /// this runs after a remote user connects and passes all there local checks and balances with the server
    /// </summary>
    /// <param name="player1"></param>
    /// <param name="player2"></param>
    private void OnRemotePlayerJoined(BasisNetworkPlayer player1, BasisRemotePlayer player2)
    {

    }
    /// <summary>
    /// this is called once
    /// level is loaded
    /// network is connected
    /// player is created
    /// player is authenticated
    /// </summary>
    /// <param name="player1"></param>
    /// <param name="player2"></param>
    public void OnLocalPlayerJoined(BasisNetworkPlayer player1, BasisLocalPlayer player2)
    {
        BasisScene.OnNetworkMessageReceived += OnNetworkMessageReceived;
        BasisScene.OnNetworkMessageSend(MessageIndex, SendingData, DeliveryMethod.ReliableOrdered, Recipients);
    }
    private void OnNetworkMessageReceived(ushort PlayerID, ushort MessageIndex, byte[] buffer, DeliveryMethod Method = DeliveryMethod.ReliableOrdered)
    {

    }
}
