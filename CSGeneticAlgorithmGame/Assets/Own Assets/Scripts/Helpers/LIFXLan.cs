using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

public class LIFXBulb // small class to organise bulbs
{
    public string IP; // local IP of the bulb
    public string Label; // name of bulb set by user
    public LIFXBulb(string ip, string label) // constructor
    {
        IP = ip;
        Label = label;
    }
    public override string ToString() // override the string method (pretty much debug only)
    {
        return string.Format("{0} ({1})", IP, Label); // formats it with IP and label
    }
}
public class LIFXLan // actual manager class
{
    public const int LIFX_PORT = 56700; // use a const so we don't have magic numbers
    public static LIFXBulb[] Cached; // caches the bulbs so it doesn't have to calculate each time
    public static void SendPayload(string ip, byte[] payload, int port = LIFX_PORT) // allows for ip in different formats
    {
        SendPayload(IPAddress.Parse(ip), payload, port); // send it with the IP type
    }
    public static void Initialise() // run this when the game is first loaded
    {
        Cached = ListBulbs(); // list all the bulbs
    }
    public static void SendPayload(IPAddress ip, byte[] payload, int port = LIFX_PORT) // actual payload sending method
    {
        UdpClient client = new UdpClient(); // creates a new broadcast client
        IPEndPoint endpoint = new IPEndPoint(ip, port); // creates the target endpoint
        client.Send(payload, payload.Length, endpoint); // sends the payload
        client.Close(); // closes connection
    }
    public static LIFXBulb[] ListBulbs() // method to list all bulbs (requires new .NET for async)
    {
        string localIP = ""; // get local IP so we don't accidentally use it as a bulb
        using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) // creates sockets with using so it will be disposed
        {
            sock.Connect("1.1.1.1", 1337); // random IP (hopefully it doesn't actually connect)
            IPEndPoint endPoint = sock.LocalEndPoint as IPEndPoint; // gets local endpoint
            localIP = endPoint.Address.ToString(); // gets address
        }
        byte[] payload = RepeatByte(0x00, 33); // create payload (most are 0x00 bytes, so just initialise an array of 0x00)
        payload[0] = 0x21; // general LIFX LAN protocol
        payload[3] = 0x34; //
        payload[22] = 0x11; // 
        payload[32] = 0x17; // 
        bool doListen = true; // keep iterating broadcast until listen flag is off
        List<LIFXBulb> bulbs = new List<LIFXBulb>(); // creates a list (in theory, we should probably use a concurrentbag)
        Task.Run(async () => // async lambda call
        {
            using (UdpClient listen = new UdpClient(LIFX_PORT)) // creates a listening client to be disposed
            {
                while (doListen) // do this while the flag is set
                {
                    var result = await listen.ReceiveAsync(); // async call to listen
                    if (result.Buffer.Length == 68) // length of expected response
                    {
                        string ip = result.RemoteEndPoint.Address.ToString(); // get sender IP
                        if ((ip != localIP) && !(bulbs.ContainsIP(ip))) // check that the IP isn't the devices, or has already been recorded
                        {
                            bulbs.Add(new LIFXBulb(ip, Encoding.ASCII.GetString(result.Buffer.Skip(36).ToArray()).Replace("\0", ""))); // reads the byte array to get name, also removes nulls
                        }
                    }
                }
            }
        });
        for (int i = 0; i < 5; i++) // does this 5 times (we're using UDP and it's not exactly the most reliable)
        {
            SendPayload(IPAddress.Broadcast, payload); // send the payload to all devices
            Thread.Sleep(75); // delays for 75ms
        }
        Thread.Sleep(200); // delays for 200ms to wait for all responses
        doListen = false; // kills the async task
        Thread.Sleep(50); // delays another 50ms to prevent any late responses
        Cached = bulbs.ToArray(); // overwrites the cache
        return bulbs.ToArray(); // returns the array
    }
    public static int[] StringToRGB(string input) // converts from a hex code to an RGB aray
    {
        try
        {
            string stripped = input.Substring((input[0] == '#') ? 1 : 0, 6).ToUpper(); // the hash might be left in
            char[] accepted = "0123456789ABCDEF".ToCharArray(); // probably could check the ASCII for each character, but this is quicker
            foreach (char c in stripped.ToUpper()) // iterate through each character of the hex code
            {
                if (!accepted.Contains(c)) // if it's not in the accepted character list
                {
                    return new int[0]; // returns empty array (the code will then flag that as an error)
                }
            }
            return new int[] { // otherwise convert the numbers from hex to dec
                    Convert.ToInt32(stripped.Substring(0, 2), 16),
                    Convert.ToInt32(stripped.Substring(2, 2), 16),
                    Convert.ToInt32(stripped.Substring(4, 2), 16)
                };
        }
        catch // if anything is invalid (e.g mismatched length)
        {
            return new int[0]; // return same empty array
        }
    }
    public static double[] RGBtoHSV(int[] c) // converts from RGB to HSV (hue, saturation, value)
    { 
        double[] normalised = new double[3]; // we want to operate on a [0, 1] range
        for (int i = 0; i < 3; i++) // go through each input
        {
            normalised[i] = c[i] / 255f; // divide by 255 to get it in range
        }
        double min = normalised.Min(); // get the highest value
        double max = normalised.Max(); // get the lowest value
        double delta = max - min; // the difference between highest and lowest
        double h = 0; // set hue as 0
        if (delta != 0) // basic RGB conversion formula
        {
            if (max == normalised[0])
            {
                h = ((normalised[1] - normalised[2]) / delta);
            }
            else if (max == normalised[1])
            {
                h = ((normalised[2] - normalised[0]) / delta) + 2.0;
            }
            else
            {
                h = ((normalised[0] - normalised[1]) / delta) + 4.0;
            }
            h /= 6.0;
            if (h < 0.0)
            {
                h += 1.0;
            }
        }
        double s = (max == 0.0) ? 0.0 : (delta / max); // get saturation from delta and max
        double v = max; // value is set to max
        return new double[] { h, s, v }; // return the converted values
    }
    public static byte[] RepeatByte(byte b, int i) // repeat the byte b, i times
    {
        return Enumerable.Repeat(b, i).ToArray(); // cleaner to use this method than to have the enumerable conversion each time
    }
    public static byte[] ConstructColourPayload(double[] c, int time) // creates the payload for colour change over time
    {
        List<byte> payload = new List<byte>(); // create the byte array
        payload.AddRange(RepeatByte(0x00, 37)); // once again, the payload contains a huge amount of 0x00 bytes
        payload[0] = 0x31; // sets the protocol
        payload[3] = 0x34; // sets the length
        payload[32] = 0x66; // sets the data type
        foreach (double d in c)
        {
            payload.AddRange(BitConverter.GetBytes((ushort)(d * 65535.0))); // adds the colour data
        }
        payload.AddRange(new byte[] { 0xAC, 0x0D }); // adds the kelvin to be 3500k (as neutral as possible) 
        payload.AddRange(BitConverter.GetBytes((uint)time)); // adds the time in
        return payload.ToArray(); // returns the payload as an array
    }
    public static bool ChangeColour(string hex, int time) // change colour method for every bulb
    {
        return ChangeColour(hex, time, Cached); // passes into the method below
    }
    public static bool ChangeColour(string hex, int time, params LIFXBulb[] targets) // actual broadcast method
    {
        LIFXBulb[] actualTargets; // creates a list of actual targets
        if (targets == null) // in case the function is accessed without initialisation (i.e. the lighting menu hasn't been done yet)
        {
            return false;
        }
        if (targets.Length == 0) // if params is empty
        {
            actualTargets = Cached; // use cached bulbs
        }
        else // otherwise
        {
            actualTargets = targets; // use the ones specified in the array
        }
        int[] result = StringToRGB(hex); // gets the result of the conversion
        if (result.Length != 3) // if it's not the expected length
        {
            return false; // return an error
        }
        double[] hsv = RGBtoHSV(result); // converts to HSV for LIFX protocol
        byte[] payload = ConstructColourPayload(hsv, time); // creates the byte payload
        foreach (LIFXBulb target in actualTargets) // broadcast to each bulb
        {
            new Thread(() => // spawn new thread for each bulb
            {
                for (int i = 0; i < 3; i++) // do this 3 times (once again, we're using UDP)
                {
                    SendPayload(target.IP, payload); // send the payload
                }
            }).Start(); // start thread
        }
        return true; // no errors
    }
}
public static class LIFXExtensions // extension classes to clean the code
{
    public static bool ContainsIP(this List<LIFXBulb> list, string ip) // check if the IP is in the list of bulbs
    {
        foreach (LIFXBulb bulb in list) // goes through each bulb in the list
        {
            if (bulb.IP == ip) // if the IP matches
            {
                return true; // true
            }
        }
        return false; // otherwise it's false
    }
}
