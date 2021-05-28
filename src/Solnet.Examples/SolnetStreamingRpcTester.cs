using System;
using NBitcoin.DataEncoders;
using Solnet.Rpc;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Models;

namespace Solnet.Examples
{
    public class SolnetStreamingRpcTester
    {
        static void Example(string[] args)
        {
            SolanaStreamingRpcClient c = new SolanaStreamingRpcClient("wss://api.mainnet-beta.solana.com/");
            var b64Dec = new Base64Encoder();
            c.Init().Wait();

            var sub = c.SubscribeAccountInfo(
                "4tSvZvnbyzHXLMTiFonMyxZoHmFqau1XArcRCVHLZ5gX",
                (s, data) =>
                {
                    // In a case where account data is received as jsonParsed
                    TokenAccountData tokenAcc = null;
                    var accData = data.Value.TryGetAccountData(out tokenAcc);
                    if (accData)
                        Console.WriteLine(
                            $"Channel: {s.Channel} Slot: {data.Context.Slot} Lamports: {data.Value.Lamports} Account Owner: {tokenAcc.Parsed.Info.Owner}");
                    
                    // In a case where account data is received as base64
                    string encodedData = null;
                    var dataString = data.Value.TryGetAccountData(out encodedData);
                    if (dataString)
                        Console.WriteLine(
                            $"Channel: {s.Channel} Slot: {data.Context.Slot} Lamports: {data.Value.Lamports} AccountData: {encodedData}");
                });

            sub.SubscriptionChanged += SubscriptionChanged;

            Console.ReadKey();
            Console.ReadKey();
        }

        private static void SubscriptionChanged(object sender, SubscriptionEvent e)
        {
            Console.WriteLine("Subscription changed to: " + e.Status);
        }
    }
}