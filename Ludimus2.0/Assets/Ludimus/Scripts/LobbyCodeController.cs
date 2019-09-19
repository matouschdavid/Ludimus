using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class LobbyCodeController : MonoBehaviour
{
    public TextMeshProUGUI lobbyCode;
    public RawImage qrCode;
    // Start is called before the first frame update
    void Start()
    {
        string localIp = "";
        IPHostEntry host;
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily.ToString() == "InterNetwork")
            {
                localIp = ip.ToString();
            }
        }
        var b = BitConverter.GetBytes((int)IPAddress.Parse(localIp).Address);
        string lobbyCode = Convert.ToBase64String(b).Split('=')[0];
        this.lobbyCode.text = lobbyCode;
        qrCode.texture = generateQR(lobbyCode);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        Color32 black = new Color32(0, 0, 0, 255);
        Color32 newColor = new Color32(54, 66, 106, 255);
        for (int i = 0; i < color32.Length; i++)
        {
            if (color32[i].CompareRGB(black))
                color32[i] = newColor;
        }
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }

    private static Color32[] Encode(string textForEncoding,
            int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}
