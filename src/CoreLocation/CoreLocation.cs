//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// The class can be either constructed from a string (from user code)
// or from a handle (from iphone-sharp.dll internal calls).  This
// delays the creation of the actual managed string until actually
// required
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
using System.Runtime.InteropServices;

namespace MonoMac.CoreLocation {

	[StructLayout (LayoutKind.Sequential)]
	public struct CLLocationCoordinate2D {
		public double Latitude;
		public double Longitude;

		public CLLocationCoordinate2D (double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		[DllImport (Constants.CoreLocationLibrary)]
		static extern int CLLocationCoordinate2DIsValid (CLLocationCoordinate2D cord);
		
		public bool IsValid ()
		{
			return CLLocationCoordinate2DIsValid (this) != 0;
		}
	}
	
}