﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace uEmuera
{
    public static class Logger
    {
        public static void Info(object content)
        {
            if(info == null)
                return;
            info(content);
        }
        public static void Warn(object content)
        {
            if(warn == null)
                return;
            warn(content);
        }
        public static void Error(object content)
        {
            if(error == null)
                return;
            error(content);
        }
        public static System.Action<object> info;
        public static System.Action<object> warn;
        public static System.Action<object> error;
    }

    public static class Utils
    {
        public static void SetSHIFTJIS_to_UTF8Dict(Dictionary<string, string> dict)
        {
            shiftjis_to_utf8 = dict;
        }
        public static void SetUTF8ZHCN_to_UTF8Dict(Dictionary<string, string> dict)
        {
            utf8zhcn_to_utf8 = dict;
        }
        public static string SHIFTJIS_to_UTF8(string text, string md5)
        {
            if(shiftjis_to_utf8 == null)
                return null;
            string result = null;
            shiftjis_to_utf8.TryGetValue(md5, out result);
            if(string.IsNullOrEmpty(result))
                utf8zhcn_to_utf8.TryGetValue(text, out result);
            return result;
        }
        static Dictionary<string, string> shiftjis_to_utf8;
        static Dictionary<string, string> utf8zhcn_to_utf8;

        /// <summary>
        /// 标准化目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            var ps = path.Split('/', '\\');
            var n = "";
            for(int i = 0; i < ps.Length - 1; ++i)
            {
                var p = ps[i];
                if(string.IsNullOrEmpty(p))
                    continue;
                n = string.Concat(n, p, '/');
            }
            if(ps.Length == 1)
                return ps[0];
            else if(ps.Length > 0)
                return n + ps[ps.Length - 1];
            return "";
        }

        public static string GetSuffix(string filename)
        {
            int last_slash = filename.LastIndexOf('.');
            if(last_slash != -1)
                return filename.Substring(last_slash + 1);
            return filename;
        }
        /// <summary>
        /// 获取文本长
        /// </summary>
        /// <param name="s"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static int GetDisplayLength(string s, uEmuera.Drawing.Font font)
        {
            return GetDisplayLength(s, font.Size);
        }

        public static readonly HashSet<char> halfsize = new HashSet<char>
        {
            '▀','▁','▂','▃','▄','▅',
            '▆','▇','█','▉','▊','▋',
            '▌','▍','▎','▏','▐','░',
            '▒','▓','▔','▕', '▮',
            '┮', '╮', '◮', '♮', '❮',
            '⟮', '⠮','⡮','⢮', '⣮',
            '▤','▥','▦', '▧', '▨', '▩',
            '▪', '▫',
        };
        public static bool CheckHalfSize(char c)
        {
            return c < 0x127 || halfsize.Contains(c);
        }
        /// <summary>
        /// 获取文本长
        /// </summary>
        /// <param name="s"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static int GetDisplayLength(string s, float fontsize)
        {
            float xsize = 0;
            char c = '\x0';
            for(int i = 0; i < s.Length; ++i)
            {
                c = s[i];
                if(CheckHalfSize(c))
                    xsize += fontsize / 2;
                else
                    xsize += fontsize;
            }

            return (int)xsize;
        }

        public static string GetStBar(char c, uEmuera.Drawing.Font font)
        {
            return GetStBar(c, font.Size);
        }

        public static string GetStBar(char c, float fontsize)
        {
            float s = fontsize;
            if(CheckHalfSize(c))
                s /= 2;
            var w = MinorShift.Emuera.Config.DrawableWidth;
            var count = (int)System.Math.Floor(w / s);
            var build = new System.Text.StringBuilder(count);
            for(int i = 0; i < count; ++i)
                build.Append(c);
            return build.ToString();
        }

        public static int GetByteCount(string str)
        {
            if(string.IsNullOrEmpty(str))
                return 0;
            var count = 0;
            var length = str.Length;
            for(int i = 0; i < length; ++i)
            {
                if(CheckHalfSize(str[i]))
                    count += 1;
                else
                    count += 2;
            }
            return count;
        }
        public static List<string> GetFiles(string search, string extension, SearchOption option)
        {
            var files = Directory.GetFiles(search, "*.???", option);
            var filecount = files.Length;
            var result = new List<string>();
            for(int i=0; i<filecount; ++i)
            {
                var file = files[i];
                string ext = Path.GetExtension(file);
                if(string.Compare(ext, extension, true) == 0)
                    result.Add(file);
            }
            return result;
        }
        public static List<string> GetFiles(string search, string[] extensions, SearchOption option)
        {
            var extension_checker = new HashSet<string>();
            for(int i = 0; i < extensions.Length; ++i)
                extension_checker.Add(extensions[i].ToUpper());

            var files = Directory.GetFiles(search, "*.???", option);
            var filecount = files.Length;
            var result = new List<string>();
            for(int i = 0; i < filecount; ++i)
            {
                var file = files[i];
                string ext = Path.GetExtension(file).ToUpper();
                if(extension_checker.Contains(ext))
                    result.Add(file);
            }
            return result;
        }
        public static List<string> GetContentFiles()
        {
            if(content_files.Count > 0)
                return content_files;

            var contentdir = MinorShift._Library.Sys.ExeDir + "resources/";
            if(!Directory.Exists(contentdir))
                return content_files;

            content_files.AddRange(Directory.GetFiles(contentdir, "*.png", SearchOption.AllDirectories));
            content_files.AddRange(Directory.GetFiles(contentdir, "*.bmp", SearchOption.AllDirectories));
            content_files.AddRange(Directory.GetFiles(contentdir, "*.jpg", SearchOption.AllDirectories));
            content_files.AddRange(Directory.GetFiles(contentdir, "*.gif", SearchOption.AllDirectories));
            content_files.AddRange(Directory.GetFiles(contentdir, "*.webp", SearchOption.AllDirectories));
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            content_files.AddRange(Directory.GetFiles(contentdir, "*.PNG", SearchOption.AllDirectories));
            content_files.AddRange(Directory.GetFiles(contentdir, "*.BMP", SearchOption.AllDirectories));
            content_files.AddRange(Directory.GetFiles(contentdir, "*.JPG", SearchOption.AllDirectories));
            content_files.AddRange(Directory.GetFiles(contentdir, "*.GIF", SearchOption.AllDirectories));
            content_files.AddRange(Directory.GetFiles(contentdir, "*.WEBP", SearchOption.AllDirectories));
#endif

            return content_files;
        }
        public static string[] GetResourceCSVLines(
            string csvpath, System.Text.Encoding encoding)
        {
            string[] lines = null;
            if(resource_csv_lines_.TryGetValue(csvpath, out lines))
                return lines;
            lines = File.ReadAllLines(csvpath, encoding);

            for (int i = 0; i < lines.Length; ++i)
                lines[i] = lines[i].Replace('\\', '/');

            return lines;
        }
        public static void ResourcePrepare()
        {
            var content_files = GetContentFiles();
            if(content_files.Count == 0)
                return;

            var contentdir = MinorShift._Library.Sys.ExeDir + "resources/";
            List<string> csvFiles = new List<string>(Directory.GetFiles(
                contentdir, "*.csv", SearchOption.AllDirectories));
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            csvFiles.AddRange(Directory.GetFiles(
                contentdir, "*.CSV", SearchOption.AllDirectories));
#endif

            var encoder = MinorShift.Emuera.Config.Encode;
            var filecount = csvFiles.Count;
            for(int index=0; index < filecount; ++index)
            {
                var filename = csvFiles[index];
                var fileParent = Path.GetDirectoryName(filename) + "/";
                //SpriteManager.ClearResourceCSVLines(filename);
                string[] lines = SpriteManager.GetResourceCSVLines(filename);
                if(lines != null)
                {

                    for (int i = 0; i < lines.Length; ++i)
                        lines[i] = lines[i].Replace('\\', '/');

                    resource_csv_lines_.Add(filename, lines);
                    continue;
                }

                List<string> newlines = new List<string>();
                lines = File.ReadAllLines(filename, encoder);

                for (int i = 0; i < lines.Length; ++i)
                    lines[i] = lines[i].Replace('\\', '/');

                int fixcount = 0;
                for(int i = 0; i < lines.Length; ++i)
                {
                    var line = lines[i];
                    if(line.Length == 0)
                        continue;
                    string str = line.Trim();
                    if(str.Length == 0 || str.StartsWith(";"))
                        continue;

                    string[] tokens = str.Split(',');
                    if (tokens.Length < 2)
                        continue;

                    if (tokens.Length >= 6)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(tokens[2]) &&
                                !string.IsNullOrEmpty(tokens[3]) &&
                                !string.IsNullOrEmpty(tokens[4]) &&
                                !string.IsNullOrEmpty(tokens[5]))
                            {
                                var w = int.Parse(tokens[4]);
                                var h = int.Parse(tokens[5]);
                                if (w != 0 && h != 0)
                                {
                                    newlines.Add(line);
                                    continue;
                                }
                            }
                        }
                        catch { }
                    }

                    string imagepath = fileParent + tokens[1].ToUpper();

                    var ti = SpriteManager.GetTextureInfo(imagepath);
                    if(ti == null)
                        continue;

                    line = string.Format("{0},{1},0,0,{2},{3}",
                        tokens[0], tokens[1], ti.width, ti.height);
                    newlines.Add(line);
                    fixcount += 1;
                }
                lines = newlines.ToArray();
                resource_csv_lines_.Add(filename, lines);
                if(fixcount > 0)
                    SpriteManager.SetResourceCSVLine(filename, lines);
            }
        }
        public static void ResourcePrepareSimple()
        {
            var content_files = GetContentFiles();
            if(content_files.Count == 0)
                return;

            var contentdir = MinorShift._Library.Sys.ExeDir + "resources/";
            List<string> csvFiles = new List<string>(Directory.GetFiles(
                contentdir, "*.csv", SearchOption.AllDirectories));
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            csvFiles.AddRange(Directory.GetFiles(
                contentdir, "*.CSV", SearchOption.AllDirectories));
#endif

            var encoder = MinorShift.Emuera.Config.Encode;
            var filecount = csvFiles.Count;
            for(int index = 0; index < filecount; ++index)
            {
                var filename = csvFiles[index];
                //SpriteManager.ClearResourceCSVLines(filename);
                string[] lines = SpriteManager.GetResourceCSVLines(filename);
                if(lines != null)
                    resource_csv_lines_.Add(filename, lines);
            }
        }
        public static void ResourceClear()
        {
            content_files.Clear();
            resource_csv_lines_.Clear();
        }
        static List<string> content_files = new List<string>();
        static Dictionary<string, string[]> resource_csv_lines_ = new Dictionary<string, string[]>();

#if UNITY_ANDROID && !UNITY_EDITOR
        public static int GetAndroidSDKVersion()
        {
            AndroidJavaClass javaClass = new AndroidJavaClass("android.os.Build$VERSION");
            return javaClass.GetStatic<int>("SDK_INT");
        }

        public static bool HasAndroidAllFilesAccess()
        {
            AndroidJavaClass javaClass = new AndroidJavaClass("android.os.Environment");
            return javaClass.CallStatic<bool>("isExternalStorageManager");
	    }

        public static void RequestAndroidAllFilesAccess()
        {
            if (GetAndroidSDKVersion() < 30 || HasAndroidAllFilesAccess())
                return;
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", "package:com.xerysherry.uEmuera");
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.MANAGE_APP_ALL_FILES_ACCESS_PERMISSION");
            intent.Call<AndroidJavaObject>("setData", uri);
            activity.Call("startActivity", intent);
	    }
#endif

	}
}
