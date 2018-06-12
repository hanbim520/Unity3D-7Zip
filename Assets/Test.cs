using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using YZL.Compress.LZMA;

public class Test : MonoBehaviour {
    public UnityEngine.UI.Text text;
    private float m_Percent;

    // Use this for initialization
    void Start () {
      //  CopyTools.CopyFolder(Application.streamingAssetsPath, Application.persistentDataPath);
    }
    public void CompressFile()
    {
        //压缩文件
        // CompressFileLZMA(Application.streamingAssetsPath + "/SA2013.pdf", Application.persistentDataPath + "/SA2013.zip");
        LZMAFile.CompressAsync(Application.streamingAssetsPath + "/SA2013.pdf", Application.persistentDataPath + "/SA2013.zip", ShowProgressPercent);
      //  UnityEditor.AssetDatabase.Refresh();

    }

    void ShowProgressPercent(long now, long all)
    {
        m_Percent = (float)now / all;
        //  Debug.Log("当前进度为: " + progress);
        
    }

     void Update()
    {
        if (m_Percent > 0f )
            text.text = ((int)(m_Percent * 100)).ToString();
    }

    public void DecompressFile()
    {
        //解压文件
        //DecompressFileLZMA(Application.persistentDataPath + "/SA2013.zip", Application.streamingAssetsPath + "/SA2013cp.pdf");
       //// UnityEditor.AssetDatabase.Refresh();

        LZMAFile.DeCompressAsync(Application.persistentDataPath + "/SA2013.zip", Application.streamingAssetsPath + "/SA2013cp.pdf", ShowProgressPercent);
    }

    private static void CompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        // Write the encoder properties
        coder.WriteCoderProperties(output);

        // Write the decompressed file size.
        output.Write(BitConverter.GetBytes(input.Length), 0, 8);

        // Encode the file.
        coder.Code(input, output, input.Length, -1, null);
        output.Flush();
        output.Close();
        input.Close();
    }


    private static void DecompressFileLZMA(string inFile, string outFile)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inFile, FileMode.Open);
        FileStream output = new FileStream(outFile, FileMode.Create);

        // Read the decoder properties
        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);

        // Read in the decompress file size.
        byte[] fileLengthBytes = new byte[8];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        // Decompress the file.
        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        output.Flush();
        output.Close();
        input.Close();
    }

}
