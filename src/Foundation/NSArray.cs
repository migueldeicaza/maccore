//
// Copyright 2009-2010, Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MonoMac.ObjCRuntime;

namespace MonoMac.Foundation {

	public partial class NSArray {
		internal NSArray (bool empty)
		{
			Handle = IntPtr.Zero;
		}

		//
		// Creates an array with the elements;   If the value passed is null, it
		// still creates an NSArray object, but the Handle is set to IntPtr.Zero,
		// this is so it makes it simpler for the generator to support
		// [NullAllowed] on array parameters.
		//
		static public NSArray FromNSObjects (params NSObject [] items)
		{
			IList<NSObject> _items = items;
			return FromNSObjects (_items);
		}

		public static NSArray FromObjects (params object [] items)
		{
			if (items == null)
				return new NSArray (true);
			NSObject [] nsoa = new NSObject [items.Length];
			for (int i = 0; i < items.Length; i++){
				var k = NSObject.FromObject (items [i]);
				if (k == null)
					throw new Exception (String.Format ("Do not know how to marshal object of type {0} to an NSObject", k.GetType ()));
				nsoa [i] = k;
			}
			return FromNSObjects (nsoa);
		}
		
		internal static NSArray FromNSObjects (IList<NSObject> items)
		{
			if (items == null)
				return new NSArray (true);
			
			IntPtr buf = Marshal.AllocHGlobal (items.Count * IntPtr.Size);
			for (int i = 0; i < items.Count; i++)
				Marshal.WriteIntPtr (buf, i * IntPtr.Size, items [i].Handle);
			NSArray arr = NSArray.FromObjects (buf, items.Count);
			Marshal.FreeHGlobal (buf);
			return arr;
		}

		static public NSArray FromStrings (params string [] items)
		{
			if (items == null)
				throw new ArgumentNullException ("items");
			
			IntPtr buf = Marshal.AllocHGlobal (items.Length * IntPtr.Size);
			NSString [] strings = new NSString [items.Length];
			
			for (int i = 0; i < items.Length; i++){
				IntPtr val;
				
				if (items [i] != null){
					strings [i] = new NSString (items [i]);
					val = strings [i].Handle;
				} else
					val = IntPtr.Zero;
				
				Marshal.WriteIntPtr (buf, i * IntPtr.Size, val);
			}
			NSArray arr = NSArray.FromObjects (buf, items.Length);
			foreach (NSString ns in strings){
				if (ns != null)
					ns.Dispose ();
			}
			
			Marshal.FreeHGlobal (buf);
			return arr;
		}

		static public NSArray FromIntPtrs (IntPtr [] vals)
		{
			if (vals == null)
				throw new ArgumentNullException ("vals");
			int n = vals.Length;
			IntPtr buf = Marshal.AllocHGlobal (n * IntPtr.Size);
			for (int i = 0; i < n; i++)
				Marshal.WriteIntPtr (buf, i * IntPtr.Size, vals [i]);

			NSArray arr = NSArray.FromObjects (buf, vals.Length);

			Marshal.FreeHGlobal (buf);
			return arr;
		}
		
		static public string [] StringArrayFromHandle (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;
			
			uint c = Messaging.UInt32_objc_msgSend (handle, selCount);
			string [] ret = new string [c];

			for (uint i = 0; i < c; i++){
				IntPtr p = Messaging.IntPtr_objc_msgSend_UInt32 (handle, selObjectAtIndex, i);
				ret [i] = NSString.FromHandle (p);
			}
			return ret;
		}

		// Used for NSObjects only
		static public T [] ArrayFromHandle<T> (IntPtr handle) where T : NSObject
		{
			if (handle == IntPtr.Zero)
				return null;
			
			uint c = Messaging.UInt32_objc_msgSend (handle, selCount);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++){
				IntPtr p = Messaging.IntPtr_objc_msgSend_UInt32 (handle, selObjectAtIndex, i);

				ret [i] = (T) Runtime.GetNSObject (p);
				ret [i].Handle = p;
			}
			return ret;
		}

		static public T [] FromArray<T> (NSArray weakArray) where T : NSObject
		{
			if (weakArray == null || weakArray.Handle == IntPtr.Zero)
				return null;
			try {
				uint n = weakArray.Count;
				T [] ret = new T [n];
				for (uint i = 0; i < n; i++){
					ret [i] = (T) Runtime.GetNSObject (weakArray.ValueAt (i));
				}
				return ret;
			} catch {
				return null;
			}
		}
		
		// Used when we need to provide our constructor
		static public T [] ArrayFromHandleFunc<T> (IntPtr handle, Func<IntPtr,T> createObject) 
		{
			if (handle == IntPtr.Zero)
				return null;
			
			uint c = Messaging.UInt32_objc_msgSend (handle, selCount);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++){
				IntPtr p = Messaging.IntPtr_objc_msgSend_UInt32 (handle, selObjectAtIndex, i);

				ret [i] = createObject (p);
			}
			return ret;
		}
		
		static public T [] ArrayFromHandle<T> (IntPtr handle, Converter<IntPtr, T> creator) 
		{
			if (handle == IntPtr.Zero)
				return null;
			
			uint c = Messaging.UInt32_objc_msgSend (handle, selCount);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++){
				IntPtr p = Messaging.IntPtr_objc_msgSend_UInt32 (handle, selObjectAtIndex, i);

				ret [i] = creator (p);
			}
			return ret;
		}
		
	}
}
