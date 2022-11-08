using Common.Library.Client;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;

namespace MeansToAnEnd
{
    internal class MeansEndHandler
    {

        public async Task ProcessRequestsAsync(ClientInfo info)
        {
            _ = info.Reader ?? throw new ArgumentNullException(nameof(info.Reader));
            _ = info.Writer ?? throw new ArgumentNullException(nameof(info.Writer));
            _ = info.Stream ?? throw new ArgumentNullException(nameof(info.Stream));

            Dictionary<int, int> table = new();
            var data = new byte[9];
            while (true) 
            {
                await info.Stream.ReadExactlyAsync(data).ConfigureAwait(false);
                info.RequestCount += 1;
                char type = (char)data[0];
                int first = BinaryPrimitives.ReadInt32BigEndian(data[1..5]);
                int second = BinaryPrimitives.ReadInt32BigEndian(data[5..]);
                Console.WriteLine($"{type} {first} {second}");

                if (type.Equals('I'))  // insert
                {
                    if (!table.ContainsKey(first))
                    {
                        table[first] = second;
                    }
                }
                else if (type.Equals('Q'))  // query
                {
                    int mean = 0;
                    if (first <= second)
                    {
                        var list = table
                                    .Where(kv => first <= kv.Key && kv.Key <= second)
                                    .Select(kv => kv.Value).ToList();
                        if (list.Count > 0)
                        {
                            mean = Convert.ToInt32(list.Average());
                        }
                    }
                    byte[] buffer = new byte[4];
                    BinaryPrimitives.WriteInt32BigEndian(buffer, mean);
                    await info.Stream.WriteAsync(buffer).ConfigureAwait(false);
                }
                else
                {
                    await info.Writer.WriteAsync("undefined, closing").ConfigureAwait(false);
                    break;
                }
            }
        }

    }
}

