using Basis.Scripts.Networking;
using Basis.Scripts.Networking.Recievers;
using System.Threading;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;
using static SerializableBasis;

public static class BasisNetworkHandleVoice
{
    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
    private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private const int TimeoutMilliseconds = 1000;
    public static ConcurrentQueue<AudioSegmentMessage> Message = new ConcurrentQueue<AudioSegmentMessage>();
    public static async Task HandleAudioUpdate(LiteNetLib.NetPacketReader Reader)
    {
        // Cancel any ongoing task
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        try
        {
            await semaphore.WaitAsync(TimeoutMilliseconds);

            try
            {
                if (Message.TryDequeue(out AudioSegmentMessage audioUpdate) == false)
                {
                    audioUpdate = new AudioSegmentMessage();
                }
                audioUpdate.Deserialize(Reader);
                if (BasisNetworkManagement.RemotePlayers.TryGetValue(audioUpdate.playerIdMessage.playerID, out BasisNetworkReceiver player))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.Log("Operation canceled.");
                        return; // Exit early if a cancellation is requested
                    }

                    if (audioUpdate.wasSilentData)
                    {
                        player.ReceiveSilentNetworkAudio(audioUpdate.silentData);
                    }
                    else
                    {
                        player.ReceiveNetworkAudio(audioUpdate);
                    }
                }
                else
                {
                    Debug.Log($"Missing Player For Message {audioUpdate.playerIdMessage.playerID}");
                }
                Message.Enqueue(audioUpdate);
                while (Message.Count > 250)
                {
                    Message.TryDequeue(out AudioSegmentMessage seg);
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                Debug.LogError($"Error in HandleAudioUpdate: {ex.Message}");
            }
            finally
            {
                semaphore.Release();
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("HandleAudioUpdate task canceled.");
        }
    }
}