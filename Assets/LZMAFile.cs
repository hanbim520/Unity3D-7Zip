﻿using SevenZip;
using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace YZL.Compress.LZMA
{
    public class LZMAFile
    {
        /**  进度  **/
        public class CodeProgress : ICodeProgress
        {
            public ProgressDelegate m_ProgressPercentDelegate = null;
            public ProgressDelegate m_ProgressDelegate = null;

            public CodeProgress(ProgressDelegate progressPercent, ProgressDelegate progress)
            {
                m_ProgressPercentDelegate = progressPercent;
                m_ProgressDelegate = progress;
            }

            public void SetProgress(Int64 inSize, Int64 outSize)
            {
                if(m_ProgressDelegate != null)
                    m_ProgressDelegate(inSize, outSize);
            }

            public void SetProgressPercent(Int64 processSize, Int64 fileSize)
            {
                if(m_ProgressPercentDelegate != null)
                    m_ProgressPercentDelegate(processSize, fileSize);
            }
        }

        /**  异步压缩一个文件  **/
        public static void CompressAsync(string inpath, string outpath, ProgressDelegate progressPercent, ProgressDelegate progress = null)
        {
            Thread compressThread = new Thread(new ParameterizedThreadStart(Compress));
            FileChangeInfo info = new FileChangeInfo();
            info.inpath = inpath;
            info.outpath = outpath;
            info.progressPercentDelegate = progressPercent;
            info.progressDelegate = progress;
            compressThread.Start(info);
        }

        /**  异步解压一个文件  **/
        public static void DeCompressAsync(string inpath, string outpath, ProgressDelegate progressPercent, ProgressDelegate progress = null)
        {
            Thread decompressThread = new Thread(new ParameterizedThreadStart(DeCompress));
            FileChangeInfo info = new FileChangeInfo();
            info.inpath = inpath;
            info.outpath = outpath;
            info.progressPercentDelegate = progressPercent;
            info.progressDelegate = progress;
            decompressThread.Start(info);
        }


        /**  同步压缩一个文件  **/
        private static void Compress(object obj)
        {
            FileChangeInfo info = (FileChangeInfo)obj;
            string inpath = info.inpath;
            string outpath = info.outpath;
            CodeProgress codeProgress = null;

            codeProgress = new CodeProgress(info.progressPercentDelegate, info.progressDelegate);

            try
            {
                SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
                FileStream inputFS = new FileStream(inpath, FileMode.Open);
                FileStream outputFS = new FileStream(outpath, FileMode.Create);

                encoder.WriteCoderProperties(outputFS);

                outputFS.Write(System.BitConverter.GetBytes(inputFS.Length), 0, 8);

                encoder.Code(inputFS, outputFS, inputFS.Length, -1, codeProgress);
                outputFS.Flush();
                outputFS.Close();
                inputFS.Close();
                Debug.Log("压缩完毕");

               
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
        public static void Compress(string inpath, string outpath, ProgressDelegate progress)
        {
            FileChangeInfo info = new FileChangeInfo();
            info.inpath = inpath;
            info.outpath = outpath;
            info.progressDelegate = progress;
            Compress(info);
        }

        /**  同步解压一个文件  **/
        private static void DeCompress(object obj)
        {
            FileChangeInfo info = (FileChangeInfo)obj;
            string inpath = info.inpath;
            string outpath = info.outpath;
            CodeProgress codeProgress = null;
            codeProgress = new CodeProgress(info.progressPercentDelegate,info.progressDelegate);

            try
            {
                SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
                FileStream inputFS = new FileStream(inpath, FileMode.Open);
                FileStream outputFS = new FileStream(outpath, FileMode.Create);

                int propertiesSize = 5;
                byte[] properties = new byte[propertiesSize];
                inputFS.Read(properties, 0, properties.Length);

                byte[] fileLengthBytes = new byte[8];
                inputFS.Read(fileLengthBytes, 0, 8);
                long fileLength = System.BitConverter.ToInt64(fileLengthBytes, 0);

                decoder.SetDecoderProperties(properties);
                decoder.Code(inputFS, outputFS, inputFS.Length, fileLength, codeProgress);
                outputFS.Flush();
                outputFS.Close();
                inputFS.Close();
                Debug.Log("解压完毕");
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
        public static void DeCompress(string inpath, string outpath, ProgressDelegate progress)
        {
            FileChangeInfo info = new FileChangeInfo();
            info.inpath = inpath;
            info.outpath = outpath;
            info.progressDelegate = progress;
            DeCompress(info);
        }
    }

}