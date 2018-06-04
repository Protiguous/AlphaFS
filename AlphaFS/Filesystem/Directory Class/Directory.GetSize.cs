/*  Copyright (C) 2008-2018 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation files (the "Software"), to deal 
 *  in the Software without restriction, including without limitation the rights 
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 *  copies of the Software, and to permit persons to whom the Software is 
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 *  THE SOFTWARE. 
 */

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace Alphaleonis.Win32.Filesystem
{
   public static partial class Directory
   {
      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="path">The path to the directory.</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SecurityCritical]
      public static long GetSize(string path)
      {
         return GetSizeCore(null, path, false, PathFormat.RelativePath);
      }


      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="path">The path to the directory.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SecurityCritical]
      public static long GetSize(string path, PathFormat pathFormat)
      {
         return GetSizeCore(null, path, false, pathFormat);
      }


      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="path">The path to the directory.</param>
      /// <param name="recursive"><c>true</c> to include subdirectories.</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SecurityCritical]
      public static long GetSize(string path, bool recursive)
      {
         return GetSizeCore(null, path, recursive, PathFormat.RelativePath);
      }


      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="path">The path to the directory.</param>
      /// <param name="recursive"><c>true</c> to include subdirectories.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SecurityCritical]
      public static long GetSize(string path, bool recursive, PathFormat pathFormat)
      {
         return GetSizeCore(null, path, recursive, pathFormat);
      }


      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the directory.</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SecurityCritical]
      public static long GetSizeTransacted(KernelTransaction transaction, string path)
      {
         return GetSizeCore(transaction, path, false, PathFormat.RelativePath);
      }


      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the directory.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SecurityCritical]
      public static long GetSizeTransacted(KernelTransaction transaction, string path, PathFormat pathFormat)
      {
         return GetSizeCore(transaction, path, false, pathFormat);
      }


      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the directory.</param>
      /// <param name="recursive"><c>true</c> to include subdirectories.</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SecurityCritical]
      public static long GetSizeTransacted(KernelTransaction transaction, string path, bool recursive)
      {
         return GetSizeCore(transaction, path, recursive, PathFormat.RelativePath);
      }


      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the directory.</param>
      /// <param name="recursive"><c>true</c> to include subdirectories.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SecurityCritical]
      public static long GetSizeTransacted(KernelTransaction transaction, string path, bool recursive, PathFormat pathFormat)
      {
         return GetSizeCore(transaction, path, recursive, pathFormat);
      }


      /// <summary>[AlphaFS] Retrieves the size of all alternate data streams of the specified directory and it files.</summary>
      /// <param name="transaction">The transaction.</param>
      /// <param name="path">The path to the directory.</param>
      /// <param name="recursive"><c>true</c> to include subdirectories.</param>
      /// <param name="pathFormat">Indicates the format of the path parameter(s).</param>
      /// <returns>The size of all alternate data streams of the specified directory and its files.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      internal static long GetSizeCore(KernelTransaction transaction, string path, bool recursive, PathFormat pathFormat)
      {
         var pathLp = Path.GetExtendedLengthPathCore(transaction, path, pathFormat, GetFullPathOptions.RemoveTrailingDirectorySeparator | GetFullPathOptions.FullCheck);

         var streamSizes = new Collection<long> {File.FindAllStreamsNative(transaction, pathLp)};
         

         foreach (var fsei in EnumerateFileSystemEntryInfosCore<FileSystemEntryInfo>(null, transaction, pathLp, Path.WildcardStarMatchAll,

            recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly, DirectoryEnumerationOptions.SkipReparsePoints, null, PathFormat.LongFullPath))
         {
            streamSizes.Add(File.FindAllStreamsNative(transaction, fsei.LongFullPath));
         }


         return streamSizes.Sum();
      }
   }
}